using System;

namespace org.activiti.engine.impl.calendar
{


	using ClockReader = org.activiti.engine.runtime.ClockReader;

	/// <summary>
	/// Resolves a due date using the original Activiti due date resolver. This does
	/// not take into account the passed time zone.
	/// 
	/// @author mseiden
	/// </summary>
	public class AdvancedSchedulerResolverWithoutTimeZone : AdvancedSchedulerResolver
	{

	  public override DateTime resolve(string duedateDescription, ClockReader clockReader, TimeZone timeZone)
	  {
		return (new CycleBusinessCalendar(clockReader)).resolveDuedate(duedateDescription);
	  }

	}

}