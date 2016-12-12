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

	using org.activiti.engine.query;


	/// <summary>
	/// Programmatic querying for <seealso cref="HistoricActivityInstance"/>s.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public interface HistoricActivityInstanceQuery : Query<HistoricActivityInstanceQuery, HistoricActivityInstance>
	{

	  /// <summary>
	  /// Only select historic activity instances with the given id (primary key within history tables). </summary>
	  HistoricActivityInstanceQuery activityInstanceId(string activityInstanceId);

	  /// <summary>
	  /// Only select historic activity instances with the given process instance.
	  /// {@link ProcessInstance) ids and <seealso cref="HistoricProcessInstance"/> ids match. 
	  /// </summary>
	  HistoricActivityInstanceQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select historic activity instances for the given process definition </summary>
	  HistoricActivityInstanceQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select historic activity instances for the given execution </summary>
	  HistoricActivityInstanceQuery executionId(string executionId);

	  /// <summary>
	  /// Only select historic activity instances for the given activity (id from BPMN 2.0 XML) </summary>
	  HistoricActivityInstanceQuery activityId(string activityId);

	  /// <summary>
	  /// Only select historic activity instances for activities with the given name </summary>
	  HistoricActivityInstanceQuery activityName(string activityName);

	  /// <summary>
	  /// Only select historic activity instances for activities with the given activity type </summary>
	  HistoricActivityInstanceQuery activityType(string activityType);

	  /// <summary>
	  /// Only select historic activity instances for userTask activities assigned to the given user </summary>
	  HistoricActivityInstanceQuery taskAssignee(string userId);

	  /// <summary>
	  /// Only select historic activity instances that are finished. </summary>
	  HistoricActivityInstanceQuery finished();

	  /// <summary>
	  /// Only select historic activity instances that are not finished yet. </summary>
	  HistoricActivityInstanceQuery unfinished();

		/// <summary>
		/// Only select historic activity instances that have the given tenant id. </summary>
	  HistoricActivityInstanceQuery activityTenantId(string tenantId);

		/// <summary>
		/// Only select historic activity instances with a tenant id like the given one. </summary>
	  HistoricActivityInstanceQuery activityTenantIdLike(string tenantIdLike);

		/// <summary>
		/// Only select historic activity instances that do not have a tenant id. </summary>
	  HistoricActivityInstanceQuery activityWithoutTenantId();


	  // ordering /////////////////////////////////////////////////////////////////
	  /// <summary>
	  /// Order by id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByHistoricActivityInstanceId();

	  /// <summary>
	  /// Order by processInstanceId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by executionId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByExecutionId();

	  /// <summary>
	  /// Order by activityId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByActivityId();

	  /// <summary>
	  /// Order by activityName (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByActivityName();

	  /// <summary>
	  /// Order by activityType (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByActivityType();

	  /// <summary>
	  /// Order by start (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByHistoricActivityInstanceStartTime();

	  /// <summary>
	  /// Order by end (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByHistoricActivityInstanceEndTime();

	  /// <summary>
	  /// Order by duration (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByHistoricActivityInstanceDuration();

	  /// <summary>
	  /// Order by processDefinitionId (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByProcessDefinitionId();

		/// <summary>
		/// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  HistoricActivityInstanceQuery orderByTenantId();

	}

}