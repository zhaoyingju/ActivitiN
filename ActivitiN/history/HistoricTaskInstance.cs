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

namespace org.activiti.engine.history
{

	using TaskInfo = org.activiti.engine.task.TaskInfo;


	/// <summary>
	/// Represents a historic task instance (waiting, finished or deleted) that is stored permanent for 
	/// statistics, audit and other business intelligence purposes.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public interface HistoricTaskInstance : TaskInfo, HistoricData
	{

	  /// <summary>
	  /// The reason why this task was deleted {'completed' | 'deleted' | any other user defined string }. </summary>
	  string DeleteReason {get;}

	  /// <summary>
	  /// Time when the task started. </summary>
	  DateTime StartTime {get;}

	  /// <summary>
	  /// Time when the task was deleted or completed. </summary>
	  DateTime EndTime {get;}

	  /// <summary>
	  /// Difference between <seealso cref="#getEndTime()"/> and <seealso cref="#getStartTime()"/> in milliseconds. </summary>
	  long? DurationInMillis {get;}

	  /// <summary>
	  /// Difference between <seealso cref="#getEndTime()"/> and <seealso cref="#getClaimTime()"/> in milliseconds. </summary>
	  long? WorkTimeInMillis {get;}

	  /// <summary>
	  /// Time when the task was claimed. </summary>
	  DateTime ClaimTime {get;}

	  /// <summary>
	  /// Sets an optional localized name for the task. </summary>
	  string LocalizedName {set;}

	  /// <summary>
	  /// Sets an optional localized description for the task. </summary>
	  string LocalizedDescription {set;}

	}

}