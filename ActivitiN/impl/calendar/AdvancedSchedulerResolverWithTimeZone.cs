using System;

namespace org.activiti.engine.impl.calendar
{


	using ClockReader = org.activiti.engine.runtime.ClockReader;

	/// <summary>
	/// Resolves a due date taking into account the specified time zone.
	/// 
	/// @author mseiden
	/// </summary>
	public class AdvancedSchedulerResolverWithTimeZone : AdvancedSchedulerResolver
	{

	  public override DateTime resolve(string duedateDescription, ClockReader clockReader, TimeZone timeZone)
	  {
		DateTime nextRun = null;

		try
		{
		  if (duedateDescription.StartsWith("R"))
		  {
			nextRun = (new DurationHelper(duedateDescription, clockReader)).getCalendarAfter(clockReader.getCurrentCalendar(timeZone));
		  }
		  else
		  {
			nextRun = (new CronExpression(duedateDescription, clockReader, timeZone)).getTimeAfter(clockReader.getCurrentCalendar(timeZone));
		  }

		}
		catch (Exception e)
		{
		  throw new ActivitiException("Failed to parse scheduler expression: " + duedateDescription, e);
		}

		return nextRun == null ? null : nextRun.Ticks;
	  }

	}

}