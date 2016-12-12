using System;

namespace org.activiti.engine.runtime
{


	/// <summary>
	/// This interface provides clock reading functionality
	/// </summary>
	public interface ClockReader
	{

	  DateTime CurrentTime {get;}

	  DateTime CurrentCalendar {get;}

	  DateTime getCurrentCalendar(TimeZone timeZone);

	  TimeZone CurrentTimeZone {get;}

	}

}