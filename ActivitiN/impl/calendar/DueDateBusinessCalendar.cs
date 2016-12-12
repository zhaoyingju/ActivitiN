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
	using DateTime = org.joda.time.DateTime;
	using Period = org.joda.time.Period;


	public class DueDateBusinessCalendar : BusinessCalendarImpl
	{

	  public const string NAME = "dueDate";

	  public DueDateBusinessCalendar(ClockReader clockReader) : base(clockReader)
	  {
	  }

	  public override DateTime resolveDuedate(string duedate, int maxIterations)
	  {
		try
		{
		  // check if due period was specified
		  if (duedate.StartsWith("P"))
		  {
			return (new DateTime(clockReader.CurrentTime)).plus(Period.parse(duedate)).toDate();
		  }

		  return DateTime.parse(duedate).toDate();

		}
		catch (Exception e)
		{
		  throw new ActivitiException("couldn't resolve duedate: " + e.Message, e);
		}
	  }
	}

}