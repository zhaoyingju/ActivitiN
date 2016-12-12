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

namespace org.activiti.engine.impl.jobexecutor
{

	using CycleBusinessCalendar = org.activiti.engine.impl.calendar.CycleBusinessCalendar;
	using DueDateBusinessCalendar = org.activiti.engine.impl.calendar.DueDateBusinessCalendar;
	using DurationBusinessCalendar = org.activiti.engine.impl.calendar.DurationBusinessCalendar;

	public enum TimerDeclarationType
	{
	  DATE = org.activiti.engine.impl.calendar.DueDateBusinessCalendar.NAME,
	  DURATION = org.activiti.engine.impl.calendar.DurationBusinessCalendar.NAME,
	  CYCLE = org.activiti.engine.impl.calendar.CycleBusinessCalendar.NAME

//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain fields in .NET:
//	  public final String calendarName;

//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain methods in .NET:
//	  TimerDeclarationType(String caledarName)
	//  {
	//	this.calendarName = caledarName;
	//  }
	}

}