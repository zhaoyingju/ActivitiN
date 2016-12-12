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
	using Execution = org.activiti.engine.runtime.Execution;


	/// <summary>
	/// Programmatic querying for <seealso cref="HistoricDetail"/>s.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public interface HistoricDetailQuery : Query<HistoricDetailQuery, HistoricDetail>
	{

	  /// <summary>
	  /// Only select historic info with the given id. </summary>
	  HistoricDetailQuery id(string id);

	  /// <summary>
	  /// Only select historic variable updates with the given process instance.
	  /// {@link ProcessInstance) ids and <seealso cref="HistoricProcessInstance"/> ids match. 
	  /// </summary>
	  HistoricDetailQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select historic variable updates with the given execution.
	  /// Note that <seealso cref="Execution"/> ids are not stored in the history as first class citizen, 
	  /// only process instances are.
	  /// </summary>
	  HistoricDetailQuery executionId(string executionId);

	  /// <summary>
	  /// Only select historic variable updates associated to the given <seealso cref="HistoricActivityInstance activity instance"/>. </summary>
	  HistoricDetailQuery activityInstanceId(string activityInstanceId);

	  /// <summary>
	  /// Only select historic variable updates associated to the given <seealso cref="HistoricTaskInstance historic task instance"/>. </summary>
	  HistoricDetailQuery taskId(string taskId);

	  /// <summary>
	  /// Only select <seealso cref="HistoricFormProperty"/>s. </summary>
	  HistoricDetailQuery formProperties();

	  /// <summary>
	  /// Only select <seealso cref="HistoricVariableUpdate"/>s. </summary>
	  HistoricDetailQuery variableUpdates();

	  /// <summary>
	  /// Exclude all task-related <seealso cref="HistoricDetail"/>s, so only items which have no 
	  /// task-id set will be selected. When used togheter with <seealso cref="#taskId(String)"/>, this
	  /// call is ignored task details are NOT excluded.
	  /// </summary>
	  HistoricDetailQuery excludeTaskDetails();

	  HistoricDetailQuery orderByProcessInstanceId();

	  HistoricDetailQuery orderByVariableName();

	  HistoricDetailQuery orderByFormPropertyId();

	  HistoricDetailQuery orderByVariableType();

	  HistoricDetailQuery orderByVariableRevision();

	  HistoricDetailQuery orderByTime();
	}

}