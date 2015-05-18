package com.nevermind.signup.server;

// Packages for the lookup table.
import java.util.HashMap;
import java.util.Map;

// Packages for date parser.
import java.util.Date;
import java.text.SimpleDateFormat;
import java.text.ParseException;

public class CompareTime {
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

	public CompareTime() {
		initMap();
	}

	public CompareTime(String filepath) {
		initMap(filepath);
	}

	// Initialize the map, may replace with loop.
	private void initMap(String filepath) {
		if (filepath.length() == 0) {
			initMap();
			return;
		}
	}

	private void initMap() {
		lookup.put(new TimeRange("2015-05-17 08:00:00", "2015-05-17 09:00:00"), "track1");
		lookup.put(new TimeRange("2015-05-17 09:10:00", "2015-05-17 10:10:00"), "track2");
		lookup.put(new TimeRange("2015-05-17 10:20:00", "2015-05-17 11:20:00"), "track3");
	}

	public String getTrack(String currentTimestamp) throws ParseException {
		Date currentTime = format.parse(currentTimestamp);
		for (TimeRange range : lookup.keySet()) {
			if (currentTime.after(range.start) && currentTime.before(range.end)) {
				return lookup.get(range);
			}
		}
		return "";
	}
}

/*
class TimeCompareDemo {
	public static void main(String[] args) {
		TimeCompare compare = new TimeCompare();
		String data = "2015-05-17 09:10:02";

		System.out.println("Current time is " + data);
		try {
			System.out.println("-> " + compare.getTrack(data));
		} catch (ParseException e) {
			System.err.println("main: Format error in \"data\"...");
			System.err.println(e.getMessage());
		}
	}
}
*/
