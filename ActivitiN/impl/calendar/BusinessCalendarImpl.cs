using System;

namespace org.activiti.engine.impl.calendar
{

	using ClockReader = org.activiti.engine.runtime.ClockReader;
	using DateTimeZone = org.joda.time.DateTimeZone;
	using ISODateTimeFormat = org.joda.time.format.ISODateTimeFormat;

	/// <summary>
	/// This class implements business calendar based on internal clock
	/// </summary>
	public abstract class BusinessCalendarImpl : BusinessCalendar
	{

	  protected internal ClockReader clockReader;

	  public BusinessCalendarImpl(ClockReader clockReader)
	  {
		this.clockReader = clockReader;
	  }

	  public override DateTime resolveDuedate(string duedateDescription)
	  {
		return resolveDuedate(duedateDescription,-1);
	  }

	  public abstract DateTime resolveDuedate(string duedateDescription, int maxIterations);

	  public override bool? validateDuedate(string duedateDescription, int maxIterations, DateTime endDate, DateTime newTimer)
	  {
		return endDate == null || endDate > newTimer || endDate.Equals(newTimer);
	  }

	  public override DateTime resolveEndDate(string endDateString)
	  {
		  return ISODateTimeFormat.dateTimeParser().withZone(DateTimeZone.forTimeZone(clockReader.CurrentTimeZone)).parseDateTime(endDateString).toCalendar(null).Time;
	  }

	}

}