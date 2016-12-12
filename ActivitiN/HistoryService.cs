using System.Collections.Generic;

/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace org.activiti.engine
{

	using HistoricActivityInstance = org.activiti.engine.history.HistoricActivityInstance;
	using HistoricActivityInstanceQuery = org.activiti.engine.history.HistoricActivityInstanceQuery;
	using HistoricDetail = org.activiti.engine.history.HistoricDetail;
	using HistoricDetailQuery = org.activiti.engine.history.HistoricDetailQuery;
	using HistoricIdentityLink = org.activiti.engine.history.HistoricIdentityLink;
	using HistoricProcessInstance = org.activiti.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.activiti.engine.history.HistoricProcessInstanceQuery;
	using HistoricTaskInstance = org.activiti.engine.history.HistoricTaskInstance;
	using HistoricTaskInstanceQuery = org.activiti.engine.history.HistoricTaskInstanceQuery;
	using HistoricVariableInstance = org.activiti.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.activiti.engine.history.HistoricVariableInstanceQuery;
	using NativeHistoricActivityInstanceQuery = org.activiti.engine.history.NativeHistoricActivityInstanceQuery;
	using NativeHistoricDetailQuery = org.activiti.engine.history.NativeHistoricDetailQuery;
	using NativeHistoricProcessInstanceQuery = org.activiti.engine.history.NativeHistoricProcessInstanceQuery;
	using NativeHistoricTaskInstanceQuery = org.activiti.engine.history.NativeHistoricTaskInstanceQuery;
	using NativeHistoricVariableInstanceQuery = org.activiti.engine.history.NativeHistoricVariableInstanceQuery;
	using ProcessInstanceHistoryLog = org.activiti.engine.history.ProcessInstanceHistoryLog;
	using ProcessInstanceHistoryLogQuery = org.activiti.engine.history.ProcessInstanceHistoryLogQuery;
	using IdentityLink = org.activiti.engine.task.IdentityLink;

	/// <summary>
	/// Service exposing information about ongoing and past process instances.  This is different
	/// from the runtime information in the sense that this runtime information only contains 
	/// the actual runtime state at any given moment and it is optimized for runtime 
	/// process execution performance.  The history information is optimized for easy 
	/// querying and remains permanent in the persistent storage.
	/// 
	/// @author Christian Stettler
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public interface HistoryService
	{

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricProcessInstance"/>s. </summary>
	  HistoricProcessInstanceQuery createHistoricProcessInstanceQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricActivityInstance"/>s. </summary>
	  HistoricActivityInstanceQuery createHistoricActivityInstanceQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricTaskInstance"/>s. </summary>
	  HistoricTaskInstanceQuery createHistoricTaskInstanceQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricDetail"/>s. </summary>
	  HistoricDetailQuery createHistoricDetailQuery();

	  /// <summary>
	  /// Returns a new <seealso cref="org.activiti.engine.query.NativeQuery"/> for process definitions.
	  /// </summary>
	  NativeHistoricDetailQuery createNativeHistoricDetailQuery();

	  /// <summary>
	  /// Creates a new programmatic query to search for <seealso cref="HistoricVariableInstance"/>s. </summary>
	  HistoricVariableInstanceQuery createHistoricVariableInstanceQuery();

	  /// <summary>
	  /// Returns a new <seealso cref="org.activiti.engine.query.NativeQuery"/> for process definitions.
	  /// </summary>
	  NativeHistoricVariableInstanceQuery createNativeHistoricVariableInstanceQuery();

	  /// <summary>
	  /// Deletes historic task instance.  This might be useful for tasks that are 
	  /// <seealso cref="TaskService#newTask() dynamically created"/> and then <seealso cref="TaskService#complete(String) completed"/>. 
	  /// If the historic task instance doesn't exist, no exception is thrown and the 
	  /// method returns normal.
	  /// </summary>
	  void deleteHistoricTaskInstance(string taskId);

	  /// <summary>
	  /// Deletes historic process instance. All historic activities, historic task and
	  /// historic details (variable updates, form properties) are deleted as well.
	  /// </summary>
	  void deleteHistoricProcessInstance(string processInstanceId);

	  /// <summary>
	  /// creates a native query to search for <seealso cref="HistoricProcessInstance"/>s via SQL
	  /// </summary>
	  NativeHistoricProcessInstanceQuery createNativeHistoricProcessInstanceQuery();

	  /// <summary>
	  /// creates a native query to search for <seealso cref="HistoricTaskInstance"/>s via SQL
	  /// </summary>
	  NativeHistoricTaskInstanceQuery createNativeHistoricTaskInstanceQuery();

	  /// <summary>
	  /// creates a native query to search for <seealso cref="HistoricActivityInstance"/>s via SQL
	  /// </summary>
	  NativeHistoricActivityInstanceQuery createNativeHistoricActivityInstanceQuery();

	  /// <summary>
	  /// Retrieves the <seealso cref="HistoricIdentityLink"/>s associated with the given task.
	  /// Such an <seealso cref="IdentityLink"/> informs how a certain identity (eg. group or user)
	  /// is associated with a certain task (eg. as candidate, assignee, etc.), even if the
	  /// task is completed as opposed to <seealso cref="IdentityLink"/>s which only exist for active
	  /// tasks.
	  /// </summary>
	  IList<HistoricIdentityLink> getHistoricIdentityLinksForTask(string taskId);

	  /// <summary>
	  /// Retrieves the <seealso cref="HistoricIdentityLink"/>s associated with the given process instance.
	  /// Such an <seealso cref="IdentityLink"/> informs how a certain identity (eg. group or user)
	  /// is associated with a certain process instance, even if the instance is completed as 
	  /// opposed to <seealso cref="IdentityLink"/>s which only exist for active instances.
	  /// </summary>
	  IList<HistoricIdentityLink> getHistoricIdentityLinksForProcessInstance(string processInstanceId);

	  /// <summary>
	  /// Allows to retrieve the <seealso cref="ProcessInstanceHistoryLog"/> for one process instance.
	  /// </summary>
	  ProcessInstanceHistoryLogQuery createProcessInstanceHistoryLogQuery(string processInstanceId);

	}

}