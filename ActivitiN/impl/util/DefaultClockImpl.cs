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
namespace org.activiti.engine.impl.util
{


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class DefaultClockImpl : org.activiti.engine.runtime.Clock
	{

	  private static volatile DateTime CURRENT_TIME = null;

	  public override DateTime CurrentTime
	  {
		  set
		  {
			DateTime time = null;
    
			if (value != null)
			{
			  time = new GregorianCalendar();
			  time = new DateTime(value);
			}
    
			CurrentCalendar = time;
		  }
		  get
		  {
			return CURRENT_TIME == null ? DateTime.Now : CURRENT_TIME.Ticks;
		  }
	  }

	  public override DateTime CurrentCalendar
	  {
		  set
		  {
			CURRENT_TIME = value;
		  }
		  get
		  {
			return CURRENT_TIME == null ? new GregorianCalendar() : (DateTime)CURRENT_TIME.clone();
		  }
	  }

	  public override void reset()
	  {
		CURRENT_TIME = null;
	  }



	  public override DateTime getCurrentCalendar(TimeZone timeZone)
	  {
		return TimeZoneUtil.convertToTimeZone(CurrentCalendar, timeZone);
	  }

	  public override TimeZone CurrentTimeZone
	  {
		  get
		  {
			return CurrentCalendar.TimeZone;
		  }
	  }

	}


}