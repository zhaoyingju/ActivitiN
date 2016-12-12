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

namespace org.activiti.engine.impl
{

	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using GetProcessDefinitionInfoCmd = org.activiti.engine.impl.cmd.GetProcessDefinitionInfoCmd;
	using SaveProcessDefinitionInfoCmd = org.activiti.engine.impl.cmd.SaveProcessDefinitionInfoCmd;

	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ArrayNode = com.fasterxml.jackson.databind.node.ArrayNode;
	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;



	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	public class DynamicBpmnServiceImpl : ServiceImpl, DynamicBpmnService, DynamicBpmnConstants
	{

	  public DynamicBpmnServiceImpl(ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
	  {
	  }

	  public virtual ObjectNode getProcessDefinitionInfo(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetProcessDefinitionInfoCmd(processDefinitionId));
	  }

	  public virtual void saveProcessDefinitionInfo(string processDefinitionId, ObjectNode infoNode)
	  {
		commandExecutor.execute(new SaveProcessDefinitionInfoCmd(processDefinitionId, infoNode));
	  }

	  public virtual ObjectNode changeServiceTaskClassName(string id, string className)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeServiceTaskClassName(id, className, infoNode);
		return infoNode;
	  }

	  public virtual void changeServiceTaskClassName(string id, string className, ObjectNode infoNode)
	  {
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.SERVICE_TASK_CLASS_NAME, className, infoNode);
	  }

	  public virtual ObjectNode changeServiceTaskExpression(string id, string expression)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeServiceTaskExpression(id, expression, infoNode);
		return infoNode;
	  }

	  public virtual void changeServiceTaskExpression(string id, string expression, ObjectNode infoNode)
	  {
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.SERVICE_TASK_EXPRESSION, expression, infoNode);
	  }

	  public virtual ObjectNode changeServiceTaskDelegateExpression(string id, string expression)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeServiceTaskDelegateExpression(id, expression, infoNode);
		return infoNode;
	  }

	  public virtual void changeServiceTaskDelegateExpression(string id, string expression, ObjectNode infoNode)
	  {
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.SERVICE_TASK_DELEGATE_EXPRESSION, expression, infoNode);
	  }

	  public virtual ObjectNode changeScriptTaskScript(string id, string script)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeScriptTaskScript(id, script, infoNode);
		return infoNode;
	  }

	  public virtual void changeScriptTaskScript(string id, string script, ObjectNode infoNode)
	  {
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.SCRIPT_TASK_SCRIPT, script, infoNode);
	  }

	  public virtual ObjectNode changeUserTaskName(string id, string name)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeUserTaskName(id, name, infoNode);
		return infoNode;
	  }

	  public virtual void changeUserTaskName(string id, string name, ObjectNode infoNode)
	  {
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_NAME, name, infoNode);
	  }

	  public virtual ObjectNode changeUserTaskDescription(string id, string description)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeUserTaskDescription(id, description, infoNode);
		return infoNode;
	  }

	  public virtual void changeUserTaskDescription(string id, string description, ObjectNode infoNode)
	  {
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_DESCRIPTION, description, infoNode);
	  }

	  public virtual ObjectNode changeUserTaskDueDate(string id, string dueDate)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeUserTaskDueDate(id, dueDate, infoNode);
		return infoNode;
	  }

	  public virtual void changeUserTaskDueDate(string id, string dueDate, ObjectNode infoNode)
	  {
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_DUEDATE, dueDate, infoNode);
	  }

	  public virtual ObjectNode changeUserTaskPriority(string id, string priority)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeUserTaskPriority(id, priority, infoNode);
		return infoNode;
	  }

	  public virtual void changeUserTaskPriority(string id, string priority, ObjectNode infoNode)
	  {
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_PRIORITY, priority, infoNode);
	  }

	  public virtual ObjectNode changeUserTaskCategory(string id, string category)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeUserTaskCategory(id, category, infoNode);
		return infoNode;
	  }

	  public virtual void changeUserTaskCategory(string id, string category, ObjectNode infoNode)
	  {
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_CATEGORY, category, infoNode);
	  }

	  public virtual ObjectNode changeUserTaskFormKey(string id, string formKey)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeUserTaskFormKey(id, formKey, infoNode);
		return infoNode;
	  }

	  public virtual void changeUserTaskFormKey(string id, string formKey, ObjectNode infoNode)
	  {
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_FORM_KEY, formKey, infoNode);
	  }

	  public virtual ObjectNode changeUserTaskAssignee(string id, string assignee)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeUserTaskAssignee(id, assignee, infoNode);
		return infoNode;
	  }

	  public virtual void changeUserTaskAssignee(string id, string assignee, ObjectNode infoNode)
	  {
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_ASSIGNEE, assignee, infoNode);
	  }

	  public virtual ObjectNode changeUserTaskOwner(string id, string owner)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeUserTaskOwner(id, owner, infoNode);
		return infoNode;
	  }

	  public virtual void changeUserTaskOwner(string id, string owner, ObjectNode infoNode)
	  {
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_OWNER, owner, infoNode);
	  }

	  public virtual ObjectNode changeUserTaskCandidateUser(string id, string candidateUser, bool overwriteOtherChangedEntries)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeUserTaskCandidateUser(id, candidateUser, overwriteOtherChangedEntries, infoNode);
		return infoNode;
	  }

	  public virtual void changeUserTaskCandidateUser(string id, string candidateUser, bool overwriteOtherChangedEntries, ObjectNode infoNode)
	  {
		ArrayNode valuesNode = null;
		if (overwriteOtherChangedEntries)
		{
		  valuesNode = processEngineConfiguration.ObjectMapper.createArrayNode();
		}
		else
		{
		  if (doesElementPropertyExist(id, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_CANDIDATE_USERS, infoNode))
		  {
			valuesNode = (ArrayNode) infoNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.BPMN_NODE).get(id).get(org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_CANDIDATE_USERS);
		  }

		  if (valuesNode == null || valuesNode.Null)
		  {
			valuesNode = processEngineConfiguration.ObjectMapper.createArrayNode();
		  }
		}

		valuesNode.add(candidateUser);
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_CANDIDATE_USERS, valuesNode, infoNode);
	  }

	  public virtual ObjectNode changeUserTaskCandidateGroup(string id, string candidateGroup, bool overwriteOtherChangedEntries)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeUserTaskCandidateGroup(id, candidateGroup, overwriteOtherChangedEntries, infoNode);
		return infoNode;
	  }

	  public virtual void changeUserTaskCandidateGroup(string id, string candidateGroup, bool overwriteOtherChangedEntries, ObjectNode infoNode)
	  {
		ArrayNode valuesNode = null;
		if (overwriteOtherChangedEntries)
		{
		  valuesNode = processEngineConfiguration.ObjectMapper.createArrayNode();
		}
		else
		{
		  if (doesElementPropertyExist(id, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_CANDIDATE_GROUPS, infoNode))
		  {
			valuesNode = (ArrayNode) infoNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.BPMN_NODE).get(id).get(org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_CANDIDATE_GROUPS);
		  }

		  if (valuesNode == null || valuesNode.Null)
		  {
			valuesNode = processEngineConfiguration.ObjectMapper.createArrayNode();
		  }
		}

		valuesNode.add(candidateGroup);
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_CANDIDATE_GROUPS, valuesNode, infoNode);
	  }

	  public virtual ObjectNode changeSequenceFlowCondition(string id, string condition)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeSequenceFlowCondition(id, condition, infoNode);
		return infoNode;
	  }

	  public virtual void changeSequenceFlowCondition(string id, string condition, ObjectNode infoNode)
	  {
		setElementProperty(id, org.activiti.engine.DynamicBpmnConstants_Fields.SEQUENCE_FLOW_CONDITION, condition, infoNode);
	  }

	  public virtual ObjectNode getBpmnElementProperties(string id, ObjectNode infoNode)
	  {
		ObjectNode propertiesNode = null;
		ObjectNode bpmnNode = getBpmnNode(infoNode);
		if (bpmnNode != null)
		{
		  propertiesNode = (ObjectNode) bpmnNode.get(id);
		}
		return propertiesNode;
	  }

	  public virtual ObjectNode changeLocalizationName(string language, string id, string value)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeLocalizationName(language, id, value, infoNode);
		return infoNode;
	  }

	  public virtual void changeLocalizationName(string language, string id, string value, ObjectNode infoNode)
	  {
		setLocalizationProperty(language, id, org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_NAME, value, infoNode);
	  }

	  public virtual ObjectNode changeLocalizationDescription(string language, string id, string value)
	  {
		ObjectNode infoNode = processEngineConfiguration.ObjectMapper.createObjectNode();
		changeLocalizationDescription(language, id, value, infoNode);
		return infoNode;
	  }

	  public virtual void changeLocalizationDescription(string language, string id, string value, ObjectNode infoNode)
	  {
		setLocalizationProperty(language, id, org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_DESCRIPTION, value, infoNode);
	  }

	  public virtual ObjectNode getLocalizationElementProperties(string language, string id, ObjectNode infoNode)
	  {
		ObjectNode propertiesNode = null;
		ObjectNode localizationNode = getLocalizationNode(infoNode);
		if (localizationNode != null)
		{
		  JsonNode languageNode = localizationNode.get(language);
		  if (languageNode != null)
		  {
			propertiesNode = (ObjectNode) languageNode.get(id);
		  }
		}
		return propertiesNode;
	  }

	  protected internal virtual bool doesElementPropertyExist(string id, string propertyName, ObjectNode infoNode)
	  {
		bool exists = false;
		if (infoNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.BPMN_NODE) != null && infoNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.BPMN_NODE).get(id) != null && infoNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.BPMN_NODE).get(id).get(propertyName) != null)
		{
		  JsonNode propNode = infoNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.BPMN_NODE).get(id).get(propertyName);
		  if (propNode.Null == false)
		  {
			exists = true;
		  }
		}
		return exists;
	  }

	  protected internal virtual void setElementProperty(string id, string propertyName, string propertyValue, ObjectNode infoNode)
	  {
		ObjectNode bpmnNode = createOrGetBpmnNode(infoNode);
		if (bpmnNode.has(id) == false)
		{
		  bpmnNode.put(id, processEngineConfiguration.ObjectMapper.createObjectNode());
		}

		((ObjectNode) bpmnNode.get(id)).put(propertyName, propertyValue);
	  }

	  protected internal virtual void setElementProperty(string id, string propertyName, JsonNode propertyValue, ObjectNode infoNode)
	  {
		ObjectNode bpmnNode = createOrGetBpmnNode(infoNode);
		if (bpmnNode.has(id) == false)
		{
		  bpmnNode.put(id, processEngineConfiguration.ObjectMapper.createObjectNode());
		}

		((ObjectNode) bpmnNode.get(id)).put(propertyName, propertyValue);
	  }

	  protected internal virtual ObjectNode createOrGetBpmnNode(ObjectNode infoNode)
	  {
		if (infoNode.has(org.activiti.engine.DynamicBpmnConstants_Fields.BPMN_NODE) == false)
		{
		  infoNode.put(org.activiti.engine.DynamicBpmnConstants_Fields.BPMN_NODE, processEngineConfiguration.ObjectMapper.createObjectNode());
		}
		return (ObjectNode) infoNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.BPMN_NODE);
	  }

	  protected internal virtual ObjectNode getBpmnNode(ObjectNode infoNode)
	  {
		return (ObjectNode) infoNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.BPMN_NODE);
	  }

	  protected internal virtual void setLocalizationProperty(string language, string id, string propertyName, string propertyValue, ObjectNode infoNode)
	  {
		ObjectNode localizationNode = createOrGetLocalizationNode(infoNode);
		if (localizationNode.has(language) == false)
		{
		  localizationNode.put(language, processEngineConfiguration.ObjectMapper.createObjectNode());
		}

		ObjectNode languageNode = (ObjectNode) localizationNode.get(language);
		if (languageNode.has(id) == false)
		{
		  languageNode.put(id, processEngineConfiguration.ObjectMapper.createObjectNode());
		}

		((ObjectNode) languageNode.get(id)).put(propertyName, propertyValue);
	  }

	  protected internal virtual ObjectNode createOrGetLocalizationNode(ObjectNode infoNode)
	  {
		if (infoNode.has(org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_NODE) == false)
		{
		  infoNode.put(org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_NODE, processEngineConfiguration.ObjectMapper.createObjectNode());
		}
		return (ObjectNode) infoNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_NODE);
	  }

	  protected internal virtual ObjectNode getLocalizationNode(ObjectNode infoNode)
	  {
		return (ObjectNode) infoNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_NODE);
	  }

	}

}