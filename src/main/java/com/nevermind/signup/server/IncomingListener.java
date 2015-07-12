package com.nevermind.signup.server;

// Packages for initiating the HTTP server.
import com.sun.net.httpserver.HttpServer;
import java.net.InetSocketAddress;
import java.io.IOException;

// Packages for custom HTTP handler.
import com.sun.net.httpserver.HttpHandler;
import com.sun.net.httpserver.HttpExchange;

// Packages for response creator and sender.
import java.net.URI;
import java.io.InputStream;
import java.io.OutputStream;
import java.text.ParseException;

// Packages for MongoDB access.
import com.mongodb.client.MongoDatabase;
import com.mongodb.client.MongoCollection;
import org.bson.Document;
import java.util.List;
import static com.mongodb.client.model.Filters.*;

// Packages for using JSONObject.
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;

// Packages for timestamp generation.
import java.text.SimpleDateFormat;
import java.util.Date;

public class IncomingListener {
	private HttpServer server;

	public IncomingListener(int portNumber, HttpHandler handler) {
		try {
			this.server = HttpServer.create(new InetSocketAddress(portNumber), 0);

			// No webpage exists.
			// Using custom handler that targets for database access.
			this.server.createContext("/", handler);

			// Using default executor.
			this.server.setExecutor(null);
		} catch (IOException e) {
			System.err.println("Having problem initiate the HTTP server.");
			System.exit(4);
		}
	}

	public void start() {
		this.server.start();
	}
}

class IncomingHandler implements HttpHandler {
	private final String AND_DELIMITER = "&";
	private final String EQUAL_DELIMITER = "=";

	// User successfully signed.
	private final int HTTP_SIGNED = 201;
	// User already signed.
	private final int HTTP_RESIGN = 304;
	// Invalid user.
	private final int HTTP_INVALID = 404;
	// Invalid HTTP request method.
	private final int HTTP_NOT_ALLOWED = 405;
	// Query JSON format is wrong.
	private final int HTTP_BAD_REQUEST = 400;
	// No valid signup sheet.
	private final int HTTP_TIMEOUT = 408;
	// Everything check out when asking for user info.
	private final int HTTP_OK = 200;

	class Header {
		public int status;
		public String content;

		public Header() {
			this.status = HTTP_NOT_ALLOWED;
			this.content = "";
		}
	}

	private MongoDatabase database = null;

	// Create the signed document schema.
	private final Document signDocument = new Document("$set", new Document("signed", true));

	// Pass the database object through the constructor.
	public IncomingHandler(MongoDatabase database) {
		this.database = database;
	}

	public void handle(HttpExchange exchange) throws IOException {
		// Create a response form the request queried
		String method = exchange.getRequestMethod();
		InputStream input = exchange.getRequestBody();

		// Parse the JSON string in the exchange.
		StringBuilder jsonString = new StringBuilder();
		int tmpByte;
		while ((tmpByte = input.read()) != -1) {
			jsonString.append((char)tmpByte);
		}
		input.close();

		Header response = createResponseFromQuery(method, jsonString.toString());
		System.err.println("Response: [" + response.status + "] " + response.content);

		//Set the response header status and length
		byte[] contentBytes = response.content.getBytes();
		exchange.sendResponseHeaders(response.status, contentBytes.length);

		//Write the response string
		OutputStream output = exchange.getResponseBody();
		output.write(contentBytes);
		output.close();
	}

	/*
	 * Methods and their usages
	 *  PUT: Sign up for the specified card ID.
	 *		{
	 * 			"card_id": "<card_id>"
	 *		}
	 *
	 *  GET: Get the user info that are associated with the card ID.
	 *		{
	 *			"card_id": "<card_id>"
	 *		}
	 *		----------
	 *		{
	 *			"first_name": "<first_name>",
	 *			"last_name" : "<last_name>",
	 *			"avatar"	: "<avatar>"
	 *		}
	 */
	private Header createResponseFromQuery(String method, String jsonString) {
		Header response = new Header();

		System.err.println("Method: " + method);

		// Return empty string if the request don't generate query.
		if (jsonString == null) {
			System.err.println("JSON string is empty.");
			return response;
		} else {
			System.err.println("JSON string: " + jsonString);
		}

		// Parse the card ID from the incoming JSON object.
		JSONParser parser = new JSONParser();
		JSONObject input = null;
		try {
			input = (JSONObject)parser.parse(jsonString);
		} catch (org.json.simple.parser.ParseException e) {
			System.err.println("Having trouble parsing JSON objects from the string.");

			response.status = HTTP_BAD_REQUEST;
			return response;
		}
		String cardId = (String)input.get("card_id");
		System.err.println("Card ID: " + cardId);

		// TODO: Determine the proper method to use.
		switch (method) {
			case "POST":
				response = getUserInfoUsingCardId(cardId);
				break;
			case "PUT":
				response = signupUsingCardId(cardId);
				break;
		}

		return response;
	}

	private Header getUserInfoUsingCardId(String cardId) {
		Header response = new Header();
		JSONObject content = new JSONObject();

		// Get the track by time.
		MongoCollection<Document> collection = database.getCollection("user");

		if (collection.count(eq("card_id", cardId)) == 0) {
			// Card ID doesn't exist.
			response.status = HTTP_INVALID;
		} else {
			// Get user info from the collection.
			Document users = collection.find(eq("card_id", cardId)).first();

			// Get the fields.
			String firstName = users.getString("first_name");
			String lastName = users.getString("last_name");
			String avatar = users.getString("avatar");

			// Create the JSON object.
			content.put("first_name", firstName);
			content.put("last_name", lastName);
			content.put("avatar", avatar);

			response.status = HTTP_OK;
			response.content = content.toString();
		}

		return response;
	}

	private Header signupUsingCardId(String cardId) {
		Header response = new Header();

		// Acquire the timestamp.
		String timestamp = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(new Date());

		/*
		// Get the collection by time.
		CompareTime time = new CompareTime("<filepath>");
		String collectionName = "";
		try {
			collectionName = time.getCollectionName(timestamp);
		} catch (ParseException e) {
			response.status = HTTP_TIMEOUT;
			return response;
		}
		*/

		// Bypass collection name.
		String collectionName = "d-2015-05-17-track2";
		
		MongoCollection<Document> collection = database.getCollection(collectionName);

		if (collection.count(eq("card_id", cardId)) == 0) {
			// Card ID doesn't exist.
			response.status = HTTP_INVALID;
		} else {
			if (collection.count(and(eq("card_id", cardId), eq("signed", true))) == 1) {
				// Specified card is already signed.
				response.status = HTTP_RESIGN;
			} else {
				// Create the date schema.
				Document date = new Document("$set", new Document("date", timestamp));

				// Update the document.
				collection.updateOne(eq("card_id", cardId), signDocument);
				collection.updateOne(eq("card_id", cardId), date);

				response.status = HTTP_SIGNED;
			}
		}

		return response;
	}
}