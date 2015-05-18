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

// Packages for using JSONObject.
import org.json.simple.JSONObject;

public class IncomingListener {
	private HttpServer server;

	public IncomingListener(int portNumber, HttpHandler handler) {
		try {
			server = HttpServer.create(new InetSocketAddress(portNumber), 0);

			// No webpage exists.
			// Using custom handler that targets for database access.
			server.createContext("", handler);

			// Using default executor.
			server.setExecutor(null);
		} catch (IOException e) {
			System.err.println("Having problem initiate the HTTP server.");
			System.exit(4);
		}
	}

	public void start() {
		server.start();
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

	public void handle(HttpExchange exchange) throws IOException {
		// Create a response form the request queried
		URI uri = exchange.getRequestURI();
		String method = exchange.getRequestMethod();

		String response = createResponseFromQuery(method, uri);
		System.err.println("Response: " + response);


		// TODO: set the status
		//Set the response header status and length
        exchange.sendResponseHeaders(HTTP_OK_STATUS, response.getBytes().length);
        //Write the response string
        OutputStream os = t.getResponseBody();
        os.write(response.getBytes());
        os.close();

	}

	private String createResponseFromQuery(String method, URI uri) {
		String rawQuery = uri.getQuery();

		// Return empty string if the request don't generate query.
		if (rawQuery == null) {
			return "";
		}

		System.err.println("Query: " + rawQuery);
		String[] queries = rawQuery.split(AND_DELIMITER);

		// Return empty string if nothing inside the query.
		if(queries.length == 0)
			return "";

		for(String query : queries) {
			String[] parameters = query.split(EQUAL_DELIMITER);
			if(parameters.length > 0) {
				for(int i = 0; i < parameters.length; i++) {

				}
			}
		}

		// Parse the parameters to map

		// Send the map to different functions according to method

		// Return the response
	}

	private String getUserInfo(String cardId) {
		JSONObject response = new JSONObject();



		return response.toString();
	}
}