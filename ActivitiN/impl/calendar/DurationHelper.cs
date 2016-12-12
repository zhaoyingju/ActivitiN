using System;
using System.Collections.Generic;

/*
 * Licensed under the Apache License, Version 2.0 (the "License");
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



	using TimeZoneUtil = org.activiti.engine.impl.util.TimeZoneUtil;
	using ClockReader = org.activiti.engine.runtime.ClockReader;
	using DateTimeZone = org.joda.time.DateTimeZone;
	using ISODateTimeFormat = org.joda.time.format.ISODateTimeFormat;

	/// <summary>
	/// helper class for parsing ISO8601 duration format (also recurring) and
	/// computing next timer date
	/// </summary>
	public class DurationHelper
	{

	  private DateTime start;
	  private DateTime end;
	  private Duration period;
	  private bool isRepeat;
	  private int times;
	  private int maxIterations = -1;
	  private bool repeatWithNoBounds;

	  private DatatypeFactory datatypeFactory;

	  public virtual DateTime Start
	  {
		  get
		  {
			return start;
		  }
	  }

	  public virtual DateTime End
	  {
		  get
		  {
			return end;
		  }
	  }

	  public virtual Duration Period
	  {
		  get
		  {
			return period;
		  }
	  }

	  public virtual bool Repeat
	  {
		  get
		  {
			return isRepeat;
		  }
	  }

	  public virtual int Times
	  {
		  get
		  {
			return times;
		  }
	  }

	  protected internal ClockReader clockReader;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DurationHelper(String expressionS, int maxIterations, org.activiti.engine.runtime.ClockReader clockReader) throws Exception
	  public DurationHelper(string expressionS, int maxIterations, ClockReader clockReader)
	  {
		this.clockReader = clockReader;
		this.maxIterations = maxIterations;
		IList<string> expression = Arrays.asList(expressionS.Split("/", true));
		datatypeFactory = DatatypeFactory.newInstance();

		if (expression.Count > 3 || expression.Count == 0)
		{
		  throw new ActivitiIllegalArgumentException("Cannot parse duration");
		}
		if (expression[0].StartsWith("R"))
		{
		  isRepeat = true;
		  times = expression[0].Length == 1 ? int.MaxValue-1 : Convert.ToInt32(expression[0].Substring(1));

		  if (expression[0].Equals("R")) // R without params
		  {
			  repeatWithNoBounds = true;
		  }

		  expression = expression.subList(1, expression.Count);
		}

		if (isDuration(expression[0]))
		{
		  period = parsePeriod(expression[0]);
		  end = expression.Count == 1 ? null : parseDate(expression[1]);
		}
		else
		{
		  start = parseDate(expression[0]);
		  if (isDuration(expression[1]))
		  {
			period = parsePeriod(expression[1]);
		  }
		  else
		  {
			end = parseDate(expression[1]);
			period = datatypeFactory.newDuration(end.TimeInMillis - start.TimeInMillis);
		  }
		}
		if (start == null)
		{
		  start = clockReader.CurrentCalendar;
		}

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DurationHelper(String expressionS, org.activiti.engine.runtime.ClockReader clockReader) throws Exception
	  public DurationHelper(string expressionS, ClockReader clockReader) : this(expressionS,-1,clockReader)
	  {
	  }

	  public virtual DateTime CalendarAfter
	  {
		  get
		  {
			return getCalendarAfter(clockReader.CurrentCalendar);
		  }
	  }

	  public virtual DateTime getCalendarAfter(DateTime time)
	  {
		if (isRepeat)
		{
		  return getDateAfterRepeat(time);
		}
		// TODO: is this correct?
		if (end != null)
		{
		  return end;
		}
		return add(start, period);
	  }

	  public virtual bool? isValidDate(DateTime newTimer)
	  {
		return end == null || end.Ticks.after(newTimer) || end.Ticks.Equals(newTimer);
	  }

	  public virtual DateTime DateAfter
	  {
		  get
		  {
			DateTime date = CalendarAfter;
    
			return date == null ? null : date.Ticks;
		  }
	  }

	  private DateTime getDateAfterRepeat(DateTime date)
	  {
		  DateTime current = TimeZoneUtil.convertToTimeZone(start, date.TimeZone);

		  if (repeatWithNoBounds)
		  {

		  while (current < date || current.Equals(date)) // As long as current date is not past the engine date, we keep looping
		  {
			  DateTime newTime = add(current, period);
			if (newTime.Equals(current) || newTime < current)
			{
				break;
			}
			current = newTime;
		  }


		  }
		  else
		  {

			int maxLoops = times;
			if (maxIterations > 0)
			{
			  maxLoops = maxIterations - times;
			}
			for (int i = 0; i < maxLoops + 1 && !current > date; i++)
			{
			  current = add(current, period);
			}

		  }
		  return current < date ? date : TimeZoneUtil.convertToTimeZone(current, clockReader.CurrentTimeZone);

	  }

	  private DateTime add(DateTime date, Duration duration)
	  {
		DateTime calendar = (DateTime) date.clone();

		// duration.addTo does not account for daylight saving time (xerces),
		// reversing order of addition fixes the problem
		calendar.AddSeconds(duration.Seconds * duration.Sign);
		calendar.AddMinutes(duration.Minutes * duration.Sign);
		calendar.AddHours(duration.Hours * duration.Sign);
		calendar.AddDays(duration.Days * duration.Sign);
		calendar.AddMonths(duration.Months * duration.Sign);
		calendar.AddYears(duration.Years * duration.Sign);

		return calendar;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.util.Calendar parseDate(String date) throws Exception
	  private DateTime parseDate(string date)
	  {
		return ISODateTimeFormat.dateTimeParser().withZone(DateTimeZone.forTimeZone(clockReader.CurrentTimeZone)).parseDateTime(date).toCalendar(null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private javax.xml.datatype.Duration parsePeriod(String period) throws Exception
	  private Duration parsePeriod(string period)
	  {
		return datatypeFactory.newDuration(period);
	  }

	  private bool isDuration(string time)
	  {
		return time.StartsWith("P");
	  }

	}

}