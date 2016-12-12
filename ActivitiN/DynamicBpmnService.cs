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

namespace org.activiti.engine
{

	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;

	/// <summary>
	/// Service providing access to the repository of process definitions and deployments.
	/// 
	/// @author Tijs Rademakers
	/// </summary>
	public interface DynamicBpmnService
	{

	  ObjectNode getProcessDefinitionInfo(string processDefinitionId);

	  void saveProcessDefinitionInfo(string processDefinitionId, ObjectNode infoNode);

	  ObjectNode changeServiceTaskClassName(string id, string className);

	  void changeServiceTaskClassName(string id, string className, ObjectNode infoNode);

	  ObjectNode changeServiceTaskExpression(string id, string expression);

	  void changeServiceTaskExpression(string id, string expression, ObjectNode infoNode);

	  ObjectNode changeServiceTaskDelegateExpression(string id, string expression);

	  void changeServiceTaskDelegateExpression(string id, string expression, ObjectNode infoNode);

	  ObjectNode changeScriptTaskScript(string id, string script);

	  void changeScriptTaskScript(string id, string script, ObjectNode infoNode);

	  ObjectNode changeUserTaskName(string id, string name);

	  void changeUserTaskName(string id, string name, ObjectNode infoNode);

	  ObjectNode changeUserTaskDescription(string id, string description);

	  void changeUserTaskDescription(string id, string description, ObjectNode infoNode);

	  ObjectNode changeUserTaskDueDate(string id, string dueDate);

	  void changeUserTaskDueDate(string id, string dueDate, ObjectNode infoNode);

	  ObjectNode changeUserTaskPriority(string id, string priority);

	  void changeUserTaskPriority(string id, string priority, ObjectNode infoNode);

	  ObjectNode changeUserTaskCategory(string id, string category);

	  void changeUserTaskCategory(string id, string category, ObjectNode infoNode);

	  ObjectNode changeUserTaskFormKey(string id, string formKey);

	  void changeUserTaskFormKey(string id, string formKey, ObjectNode infoNode);

	  ObjectNode changeUserTaskAssignee(string id, string assignee);

	  void changeUserTaskAssignee(string id, string assignee, ObjectNode infoNode);

	  ObjectNode changeUserTaskOwner(string id, string owner);

	  void changeUserTaskOwner(string id, string owner, ObjectNode infoNode);

	  ObjectNode changeUserTaskCandidateUser(string id, string candidateUser, bool overwriteOtherChangedEntries);

	  void changeUserTaskCandidateUser(string id, string candidateUser, bool overwriteOtherChangedEntries, ObjectNode infoNode);

	  ObjectNode changeUserTaskCandidateGroup(string id, string candidateGroup, bool overwriteOtherChangedEntries);

	  void changeUserTaskCandidateGroup(string id, string candidateGroup, bool overwriteOtherChangedEntries, ObjectNode infoNode);

	  ObjectNode changeSequenceFlowCondition(string id, string condition);

	  void changeSequenceFlowCondition(string id, string condition, ObjectNode infoNode);

	  ObjectNode getBpmnElementProperties(string id, ObjectNode infoNode);

	  ObjectNode changeLocalizationName(string language, string id, string value);

	  void changeLocalizationName(string language, string id, string value, ObjectNode infoNode);

	  ObjectNode changeLocalizationDescription(string language, string id, string value);

	  void changeLocalizationDescription(string language, string id, string value, ObjectNode infoNode);

	  ObjectNode getLocalizationElementProperties(string language, string id, ObjectNode infoNode);
	}

}