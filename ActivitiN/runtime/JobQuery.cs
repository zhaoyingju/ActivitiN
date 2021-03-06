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

namespace org.activiti.engine.runtime
{

	using org.activiti.engine.query;


	/// <summary>
	/// Allows programmatic querying of <seealso cref="Job"/>s.
	/// 
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
	public interface JobQuery : Query<JobQuery, Job>
	{

	  /// <summary>
	  /// Only select jobs with the given id </summary>
	  JobQuery jobId(string jobId);

	  /// <summary>
	  /// Only select jobs which exist for the given process instance. * </summary>
	  JobQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select jobs which exist for the given execution </summary>
	  JobQuery executionId(string executionId);

	  /// <summary>
	  /// Only select jobs which exist for the given process definition id </summary>
	  JobQuery processDefinitionId(string processDefinitionid);

	  /// <summary>
	  /// Only select jobs which have retries left </summary>
	  JobQuery withRetriesLeft();

	  /// <summary>
	  /// Only select jobs which have no retries left </summary>
	  JobQuery noRetriesLeft();

	  /// <summary>
	  /// Only select jobs which are executable, 
	  /// ie. retries &gt; 0 and duedate is null or duedate is in the past *
	  /// </summary>
	  JobQuery executable();

	  /// <summary>
	  /// Only select jobs that are timers. 
	  /// Cannot be used together with <seealso cref="#messages()"/> 
	  /// </summary>
	  JobQuery timers();

	  /// <summary>
	  /// Only select jobs that are messages. 
	  /// Cannot be used together with <seealso cref="#timers()"/> 
	  /// </summary>
	  JobQuery messages();

	  /// <summary>
	  /// Only select jobs where the duedate is lower than the given date. </summary>
	  JobQuery duedateLowerThan(DateTime date);

	  /// <summary>
	  /// Only select jobs where the duedate is higher then the given date. </summary>
	  JobQuery duedateHigherThan(DateTime date);

	  /// <summary>
	  /// Only select jobs where the duedate is lower then the given date. </summary>
	  /// @deprecated  
	  JobQuery duedateLowerThen(DateTime date);

	  /// <summary>
	  /// Only select jobs where the duedate is lower then or equals the given date. </summary>
	  /// @deprecated  
	  JobQuery duedateLowerThenOrEquals(DateTime date);

	  /// <summary>
	  /// Only select jobs where the duedate is higher then the given date. </summary>
	  /// @deprecated  
	  JobQuery duedateHigherThen(DateTime date);

	  /// <summary>
	  /// Only select jobs where the duedate is higher then or equals the given date. </summary>
	  /// @deprecated  
	  JobQuery duedateHigherThenOrEquals(DateTime date);

	  /// <summary>
	  /// Only select jobs that failed due to an exception. </summary>
	  JobQuery withException();

	  /// <summary>
	  /// Only select jobs that failed due to an exception with the given message. </summary>
	  JobQuery exceptionMessage(string exceptionMessage);

	  /// <summary>
	  /// Only select jobs that have the given tenant id.
	  /// </summary>
	  JobQuery jobTenantId(string tenantId);

		/// <summary>
		/// Only select jobs with a tenant id like the given one.
		/// </summary>
	  JobQuery jobTenantIdLike(string tenantIdLike);

		/// <summary>
		/// Only select jobs that do not have a tenant id.
		/// </summary>
	  JobQuery jobWithoutTenantId();

	  //sorting //////////////////////////////////////////

	  /// <summary>
	  /// Order by job id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  JobQuery orderByJobId();

	  /// <summary>
	  /// Order by duedate (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  JobQuery orderByJobDuedate();

	  /// <summary>
	  /// Order by retries (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  JobQuery orderByJobRetries();

	  /// <summary>
	  /// Order by process instance id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  JobQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by execution id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  JobQuery orderByExecutionId();

		/// <summary>
		/// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  JobQuery orderByTenantId();

	}

}