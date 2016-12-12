using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace org.activiti.engine.impl.calendar
{


	using ClockReader = org.activiti.engine.runtime.ClockReader;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// An Activiti BusinessCalendar for cycle based schedules that takes into
	/// account a different daylight savings time zone than the one that the server
	/// is configured for.
	/// <para>
	/// For CRON strings DSTZONE is used as the time zone that the CRON schedule
	/// refers to. Leave it out to use the server time zone.
	/// </para>
	/// <para>
	/// For ISO strings the time zone offset for the date/time specified is part of
	/// the string itself. DSTZONE is used to determine what the offset should be
	/// NOW, which may be different than when the workflow was scheduled if it is
	/// scheduled to run across a DST event.
	/// 
	/// <pre>
	///   For example:
	///      R/2013-10-01T20:30:00/P1D DSTZONE:US/Eastern
	///      R/2013-10-01T20:30:00/P1D DSTZONE:UTC
	///      R/2013-10-01T20:30:00/P1D DSTZONE:US/Arizona
	///      0 30 20 ? * MON,TUE,WED,THU,FRI * DSTZONE:US/Eastern
	///      0 30 20 ? * MON,TUE,WED,THU,FRI * DSTZONE:UTC
	///      0 30 20 ? * MON,TUE,WED,THU,FRI * DSTZONE:US/Arizona
	/// </pre>
	/// 
	/// Removing the DSTZONE key will cause Activiti to use the server's time zone.
	/// This is the original behavior.
	/// </para>
	/// <para>
	/// Schedule strings are versioned. Version 1 strings will use the original
	/// Activiti CycleBusinessCalendar. All new properties are ignored. Version 2
	/// strings will use the new daylight saving time logic.
	/// 
	/// <pre>
	///   For example:
	///      R/2013-10-01T20:30:00/P1D VER:2 DSTZONE:US/Eastern
	///      0 30 20 ? * MON,TUE,WED,THU,FRI * VER:1 DSTZONE:US/Arizona
	/// </pre>
	/// 
	/// By default (if no VER key is included in the string), it assumes version 2.
	/// This can be changed by modifying the defaultScheduleVersion property.
	/// </para>
	/// <para>
	/// 
	/// @author mseiden
	/// </para>
	/// </summary>
	public class AdvancedCycleBusinessCalendar : CycleBusinessCalendar
	{

	  private int? defaultScheduleVersion;

	  private const int? DEFAULT_VERSION = 2;

	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(AdvancedCycleBusinessCalendar));

	  private static readonly IDictionary<int?, AdvancedSchedulerResolver> resolvers;

	  static AdvancedCycleBusinessCalendar()
	  {
		resolvers = new ConcurrentDictionary<int?, AdvancedSchedulerResolver>();
		resolvers[1] = new AdvancedSchedulerResolverWithoutTimeZone();
		resolvers[2] = new AdvancedSchedulerResolverWithTimeZone();
	  }

	  public AdvancedCycleBusinessCalendar(ClockReader clockReader) : base(clockReader)
	  {
	  }

	  public AdvancedCycleBusinessCalendar(ClockReader clockReader, int? defaultScheduleVersion) : this(clockReader)
	  {
		this.defaultScheduleVersion = defaultScheduleVersion;
	  }

	  public virtual int? DefaultScheduleVersion
	  {
		  get
		  {
			return defaultScheduleVersion == null ? DEFAULT_VERSION : defaultScheduleVersion;
		  }
		  set
		  {
			this.defaultScheduleVersion = value;
		  }
	  }


	  public override DateTime resolveDuedate(string duedateDescription, int maxIterations)
	  {
		logger.info("Resolving Due Date: " + duedateDescription);

		string timeZone = getValueFrom("DSTZONE", duedateDescription);
		string version = getValueFrom("VER", duedateDescription);

		// START is a legacy value that is no longer used, but may still exist in
		// deployed job schedules
		// Could be used in the future as a start date for a CRON job
		// String startDate = getValueFrom("START", duedateDescription);

		duedateDescription = removeValueFrom("VER", removeValueFrom("START", removeValueFrom("DSTZONE", duedateDescription))).Trim();

		try
		{
		  logger.info("Base Due Date: " + duedateDescription);

		  DateTime date = resolvers[version == null ? DefaultScheduleVersion : Convert.ToInt32(version)].resolve(duedateDescription, clockReader, timeZone == null ? clockReader.CurrentTimeZone : TimeZone.getTimeZone(timeZone));

		  logger.info("Calculated Date: " + (date == null ? "Will Not Run Again" : date));

		  return date;

		}
		catch (Exception e)
		{
		  throw new ActivitiIllegalArgumentException("Cannot parse duration", e);
		}

	  }

	  private string getValueFrom(string field, string duedateDescription)
	  {
		int fieldIndex = duedateDescription.IndexOf(field + ":");

		if (fieldIndex > -1)
		{
		  int nextWhiteSpace = duedateDescription.IndexOf(" ", fieldIndex);

		  fieldIndex += field.Length + 1;

		  if (nextWhiteSpace > -1)
		  {
			return duedateDescription.Substring(fieldIndex, nextWhiteSpace - fieldIndex);
		  }
		  else
		  {
			return duedateDescription.Substring(fieldIndex);
		  }
		}

		return null;
	  }

	  private string removeValueFrom(string field, string duedateDescription)
	  {
		int fieldIndex = duedateDescription.IndexOf(field + ":");

		if (fieldIndex > -1)
		{
		  int nextWhiteSpace = duedateDescription.IndexOf(" ", fieldIndex);

		  if (nextWhiteSpace > -1)
		  {
			return duedateDescription.Replace(duedateDescription.Substring(fieldIndex, nextWhiteSpace - fieldIndex), "");
		  }
		  else
		  {
			return duedateDescription.Substring(0, fieldIndex);
		  }
		}

		return duedateDescription;
	  }
	}

}