package com.nevermind.signup.server;

// Packages for initiating the HTTP server.
import com.sun.net.httpserver.HttpServer;
import java.net.InetSocketAddress;
import java.io.IOException;

// Packages for custom HTTP handler.
import com.sun.net.httpserver.HttpHandler;
import com.sun.net.httpserver.HttpExchange;

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
	public void handle(HttpExchange exchange) throws IOException {

	}
}