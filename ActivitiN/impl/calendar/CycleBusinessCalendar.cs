using System;

/* Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.activiti.engine.impl.calendar
{

	using ClockReader = org.activiti.engine.runtime.ClockReader;

	public class CycleBusinessCalendar : BusinessCalendarImpl
	{

	  public static string NAME = "cycle";

	  public CycleBusinessCalendar(ClockReader clockReader) : base(clockReader)
	  {
	  }

	  public override DateTime resolveDuedate(string duedateDescription, int maxIterations)
	  {
		try
		{
		  if (duedateDescription != null && duedateDescription.StartsWith("R"))
		  {
			return (new DurationHelper(duedateDescription, maxIterations, clockReader)).DateAfter;
		  }
		  else
		  {
			CronExpression ce = new CronExpression(duedateDescription, clockReader);
			return ce.getTimeAfter(clockReader.CurrentTime);
		  }

		}
		catch (Exception e)
		{
		  throw new ActivitiException("Failed to parse cron expression: " + duedateDescription, e);
		}

	  }


	  public override bool? validateDuedate(string duedateDescription, int maxIterations, DateTime endDate, DateTime newTimer)
	  {
		if (endDate != null)
		{
		  return base.validateDuedate(duedateDescription, maxIterations, endDate, newTimer);
		}
		//end date could be part of the chron expression
		try
		{
		  if (duedateDescription != null && duedateDescription.StartsWith("R"))
		  {
			return (new DurationHelper(duedateDescription, maxIterations, clockReader)).isValidDate(newTimer);
		  }
		  else
		  {
			return true;
		  }

		}
		catch (Exception e)
		{
		  throw new ActivitiException("Failed to parse cron expression: " + duedateDescription, e);
		}

	  }

	}

}