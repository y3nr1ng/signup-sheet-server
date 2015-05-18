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
import java.io.OutputStream;
import java.text.ParseException;

// Packages for MongoDB access.
import com.mongodb.DB;
import com.mongodb.BasicDBObject;
import com.mongodb.DBCollection;

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

	/*
	 * Response code
	 *  201: User successfully signed.
	 *  304: User already signed.
	 *  404: Invalid user.
	 */
	private final int HTTP_SIGNED = 201;
	private final int HTTP_RESIGN = 304;
	private final int HTTP_INVALID = 404;
	private final int HTTP_NOT_ALLOWED = 405;
	private final int HTTP_BAD_REQUEST = 400;

	class Header {
		public int status;
		public String content;

		public Header() {
			this.status = HTTP_NOT_ALLOWED;
			this.content = "";
		}
	}

	private DB database = null;

	// Create the signed document schema.
	private final BasicDBObject signDocument = new BasicDBObject().append("$set",
	        new BasicDBObject().append("signed", true));


	// Pass the database object through the constructor.
	public IncomingHandler(DB database) {
		this.database = database;
	}

	public void handle(HttpExchange exchange) throws IOException {
		// Create a response form the request queried
		URI uri = exchange.getRequestURI();
		String method = exchange.getRequestMethod();

		Header response = createResponseFromQuery(method, uri);
		System.err.println("Response: " + response);

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
	private Header createResponseFromQuery(String method, URI uri) {
		String rawQuery = uri.getQuery();
		Header response = new Header();

		// Return empty string if the request don't generate query.
		if (rawQuery == null) {
			return response;
		}

		// Parse the card ID from the incoming JSON object.
		JSONParser parser = new JSONParser();
		JSONObject input = null;
		try {
			input = (JSONObject)parser.parse(rawQuery);
		} catch (org.json.simple.parser.ParseException e) {
			System.err.println("Having trouble parsing the JSON object from the query.");

			response.status = HTTP_BAD_REQUEST;
			return response;
		}
		String cardId = (String)input.get("card_id");
		System.err.println("Card ID: " + cardId);

		switch (method) {
			case "GET":
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
		DBCollection collection = database.getCollection("user");

		// Create card ID search object.
		BasicDBObject queryCard = new BasicDBObject("card_id", cardId);

		if (collection.find(queryCard).count() == 0) {
			// Card ID doesn't exist.
			response.status = HTTP_INVALID;
			return response;
		}

		
		
		return response;
	}

	private Header signupUsingCardId(String cardId) {
		Header response = new Header();
		JSONObject content = new JSONObject();

		// Get the track by time.
		DBCollection collection = database.getCollection("<track>");

		// Create card ID search object.
		BasicDBObject queryCard = new BasicDBObject("card_id", cardId);

		if (collection.find(queryCard).count() == 0) {
			// Card ID doesn't exist.
			response.status = HTTP_INVALID;
		} else {
			if (collection.find(queryCard.append("signed", true)).count() == 1) {
				// Specified card is already signed.
				response.status = HTTP_RESIGN;
			} else {
				// Create the date schema.
				String timestamp = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(new Date());
				BasicDBObject date = new BasicDBObject().append("$set",
				        new BasicDBObject().append("date", true));

				// Update the document.
				collection.update(queryCard, signDocument);
				collection.update(queryCard, date);

				response.status = HTTP_SIGNED;
			}
		}

		return response;
	}
}