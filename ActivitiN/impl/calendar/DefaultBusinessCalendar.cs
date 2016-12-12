using System;
using System.Collections.Generic;

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


	using Context = org.activiti.engine.impl.context.Context;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DefaultBusinessCalendar : BusinessCalendar
	{

	  private static IDictionary<string, int?> units = new Dictionary<string, int?>();
	  static DefaultBusinessCalendar()
	  {
		units["millis"] = DateTime.MILLISECOND;
		units["seconds"] = DateTime.SECOND;
		units["second"] = DateTime.SECOND;
		units["minute"] = DateTime.MINUTE;
		units["minutes"] = DateTime.MINUTE;
		units["hour"] = DateTime.HOUR;
		units["hours"] = DateTime.HOUR;
		units["day"] = DateTime.DAY_OF_YEAR;
		units["days"] = DateTime.DAY_OF_YEAR;
		units["week"] = DateTime.WEEK_OF_YEAR;
		units["weeks"] = DateTime.WEEK_OF_YEAR;
		units["month"] = DateTime.MONTH;
		units["months"] = DateTime.MONTH;
		units["year"] = DateTime.YEAR;
		units["years"] = DateTime.YEAR;
	  }

	  public override DateTime resolveDuedate(string duedateDescription, int maxIterations)
	  {
		return resolveDuedate(duedateDescription);
	  }

	  public virtual DateTime resolveDuedate(string duedate)
	  {
		DateTime resolvedDuedate = Context.ProcessEngineConfiguration.Clock.CurrentTime;

		string[] tokens = duedate.Split(" and ", true);
		foreach (string token in tokens)
		{
		  resolvedDuedate = addSingleUnitQuantity(resolvedDuedate, token);
		}

		return resolvedDuedate;
	  }

	  public override bool? validateDuedate(string duedateDescription, int maxIterations, DateTime endDate, DateTime newTimer)
	  {
		return true;
	  }

	  public override DateTime resolveEndDate(string endDate)
	  {
		return null;
	  }

	  protected internal virtual DateTime addSingleUnitQuantity(DateTime startDate, string singleUnitQuantity)
	  {
		int spaceIndex = singleUnitQuantity.IndexOf(" ");
		if (spaceIndex == -1 || singleUnitQuantity.Length < spaceIndex + 1)
		{
		  throw new ActivitiIllegalArgumentException("invalid duedate format: " + singleUnitQuantity);
		}

		string quantityText = singleUnitQuantity.Substring(0, spaceIndex);
		int? quantity = Convert.ToInt32(quantityText);

		string unitText = singleUnitQuantity.Substring(spaceIndex + 1).Trim().ToLower();

		int unit = units[unitText];

		GregorianCalendar calendar = new GregorianCalendar();
		calendar.Time = startDate;
		calendar.add(unit, quantity);

		return calendar.Time;
	  }
	}

}