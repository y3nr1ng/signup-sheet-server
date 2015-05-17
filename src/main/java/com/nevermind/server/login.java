package com.nevermind.server;

import com.mongodb.BasicDBObject;
import com.mongodb.BulkWriteOperation;
import com.mongodb.BulkWriteResult;
import com.mongodb.Cursor;
import com.mongodb.DB;
import com.mongodb.DBCollection;
import com.mongodb.DBCursor;
import com.mongodb.DBObject;
import com.mongodb.MongoClient;
import com.mongodb.MongoCredential;
import com.mongodb.ParallelScanOptions;
import com.mongodb.ServerAddress;


import java.util.List;
import java.util.Set;
import java.util.Calendar;
import java.util.Arrays;
import java.util.ArrayList;

import java.io.IOException;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Date;
import java.text.SimpleDateFormat;
import java.util.HashMap;
import java.util.Map;
import java.text.ParseException;

import java.io.InputStream;
import java.io.OutputStream;

//import org.json.JSONObject;

import static java.util.concurrent.TimeUnit.SECONDS;

class TimeCompare {
        // Generalized date format parser.
        final private SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
        private Map<TimeRange, String> lookup = new HashMap<TimeRange, String>();
 
        // Object key for the lookup table, define the time range.
        class TimeRange {
                public Date start;
                public Date end;
 
                public TimeRange(Date _start, Date _end) {
                        start = _start;
                        end = _end;
                }
                public TimeRange(String _start, String _end) {
                        try {
                                start = format.parse(_start);
                                end = format.parse(_end);
                        } catch (ParseException e) {
                                System.err.println("class TimeRange: Having trouble parsing the input.");
                        }
                }
        }
 
        // Initialize the map, may replace with loop.
        private void initMap() {
                lookup.put(new TimeRange("2015-05-17 08:00:00", "2015-05-17 09:00:00"), "track1");
                lookup.put(new TimeRange("2015-05-17 09:10:00", "2015-05-17 10:10:00"), "track2");
                lookup.put(new TimeRange("2015-05-17 10:20:00", "2015-05-17 11:20:00"), "track3");
        }
 
        public String getTrack(String currentTimestamp) throws ParseException {
                System.err.println("In getTrack...");
 
                Date currentTime = format.parse(currentTimestamp);
 
                System.err.println("Parse currentTime...");
 
                for (TimeRange range : lookup.keySet()) {
                        System.out.println("Start: " + range.start + ", End: " + range.end + ", Current: " + currentTime);
                        if (currentTime.after(range.start) && currentTime.before(range.end)) {
                                return lookup.get(range);
                        }
                }
                return "";
        }
 
        public TimeCompare() {
                initMap();
        }
}


public class login {
	private SimpleDateFormat format = new SimpleDateFormat("HH:mm:ss");
	private Date[] time = new Date[4];

	public void listen(DB database, int portNumber) throws IOException, java.text.ParseException {
		ServerSocket listener = new ServerSocket(portNumber);
		String cardId = "", timeStamp = "";
		StringBuilder body = new StringBuilder();
		// Raw HTTP requests.
		String request;
		String type, path;
		System.out.println("On port: " + portNumber);


		try {
		    Socket socket = listener.accept();
		    BufferedReader in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
		    //BufferedWriter out = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));
		    // read request
		    String line;
		    line = in.readLine();
		    //StringBuilder raw = new StringBuilder();
		    //aw.append("" + line);
		    boolean isPost = line.startsWith("POST");
		    int contentLength = 0;
		    while (!(line = in.readLine()).equals("")) {
		        //raw.append('\n' + line);
		        if (isPost) {
		            final String contentHeader = "Content-Length: ";
		            if (line.startsWith(contentHeader)) {
		                contentLength = Integer.parseInt(line.substring(contentHeader.length()));
		            }
		        }
		    }
		    body = new StringBuilder();
		    if (isPost) {
		    	timeStamp = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(new Date()); 
		        int c = 0;
		        for (int i = 0; i < contentLength; i++) {
		            c = in.read();
		            body.append((char) c);
		            //Log.d("JCD", "POST: " + ((char) c) + " " + c);
		        }
		    }

		    // Search in database
    		String key = body.toString().split("=")[1];
    		System.out.println("Card ID: " + key + ", Time: " + timeStamp);
    		//String track = compare_time(key);
    		TimeCompare time_cmp = new TimeCompare();
    		String pseudo_time = "2015-05-17 09:10:02";
 			String track = time_cmp.getTrack(pseudo_time);
    		// Primary key
    		DBObject query = new BasicDBObject("card_id", key);

    		// Collection handleres
    		DBCollection user_collection = database.getCollection("user");
    		DBCollection track_collection = database.getCollection("d-2015-05-17-" + track);

    		// Sign up
    		BasicDBObject new_data_signed = new BasicDBObject();
    		new_data_signed.append("$set", new BasicDBObject().append("signed", true));
    		BasicDBObject new_data_date = new BasicDBObject();
    		new_data_date.append("$set", new BasicDBObject().append("date", timeStamp));
    		track_collection.update(query, new_data_signed);
    		track_collection.update(query, new_data_date);

    		// Get user info
    		DBObject user = user_collection.findOne(query);
    		String output_name = "", output_avatar = "";
    		try {
	    		output_name =  (String)user.get("name");
	    		output_avatar =  (String)user.get("avatar");
    		} catch(NullPointerException e) {
    			output_name = output_avatar = "";
    		}

    		System.err.println("Name: " + output_name);
    		
    		StringBuilder output_string = new StringBuilder();
    		output_string.append("{");
    		output_string.append("\"name\":\"").append(output_name);
    		output_string.append("\",");
    		output_string.append("\"avatar\":\"").append(output_avatar);
    		output_string.append("\"}");

    		System.err.println("JSON payload...");
    		System.err.println(output_string.toString());

    		// Generating the packet
		    OutputStream out = socket.getOutputStream();
		    byte [] response = output_string.toString().getBytes("ASCII");

		    String statusLine = "";
		    if(output_name.length() > 0) {
		    	statusLine = "HTTP/1.1 200 OK\r\n";
		    } else {
		    	statusLine = "HTTP/1.1 400 OK\r\n"; 
		    }
            out.write(statusLine.getBytes("ASCII"));

            String contentLength_str = "Content-Length: " + response.length + "\r\n";
            out.write(contentLength_str.getBytes("ASCII"));

            // signal end of headers
            out.write( "\r\n".getBytes("ASCII"));

            // write actual response and flush
            out.write(response);
            out.flush();
		    
		   	out.close();
		    socket.close();
		    
		} finally{
			listener.close();
		}	
	}

	public String compare_time(String time_string) {
		try {
			//System.out.println("Insdie compare_time input is: " + time_string);
			time[0] = format.parse("09:00:00");
			time[1] = format.parse("10:00:00");
			time[2] = format.parse("11:00:00");
			time[3] = format.parse("12:00:00");

			return "track1";

			/*
			Date signup_time = format.parse(time_string.split(" ")[1]);
			if((signup_time.compareTo(time1) <= 0 && signup_time.compareTo(time2) > 0)) return "track1";
			else if(signup_time.compareTo(time2) <= 0 && signup_time.compareTo(time3) > 0) return "track2";
			else if(signup_time.compareTo(time3) <= 0 && signup_time.compareTo(time4) >= 0) return "track3";
			else return "-1";
			*/
		} catch(Exception e) {
		}

		System.out.println("Exception caught");
		return "-1";
	}
	public static void main(String[] args) throws IOException, java.text.ParseException{
		if (args.length != 1) {
			System.err.println("Usage: java Adapter <port number>");
			System.exit(1);
		}

		// Register the port using the first parameter.
		int portNumber = Integer.parseInt(args[0]);
		System.out.println("Port number: " + portNumber);

    	List<ServerAddress> seeds = new ArrayList<ServerAddress>();
	    seeds.add(new ServerAddress("localhost" , 27017));
	    //add a credendtial for the seed
	    List<MongoCredential> credential = new ArrayList();
	    credential.add( MongoCredential.createCredential("gistaiwan", "signup-sheet-db", "nevermind".toCharArray()));

	   	MongoClient mongoClient = new MongoClient(seeds, credential);
	   	//creata a preson object to write to the database
		//Person p1 = new  Person("2015-07-15 00:00:00", "pi_Liu", "0816", false, false, false);
		DB database =  mongoClient.getDB("signup-sheet-db");

		while(true){
			login handler = new login();
    		handler.listen(database, portNumber);
		}
	}
}