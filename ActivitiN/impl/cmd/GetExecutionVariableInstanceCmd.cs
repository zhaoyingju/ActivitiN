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
namespace org.activiti.engine.impl.cmd
{

	using Context = org.activiti.engine.impl.context.Context;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using VariableInstance = org.activiti.engine.impl.persistence.entity.VariableInstance;
	using Execution = org.activiti.engine.runtime.Execution;

	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;

	[Serializable]
	public class GetExecutionVariableInstanceCmd : Command<VariableInstance>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string executionId;
	  protected internal string variableName;
	  protected internal bool isLocal;
	  protected internal string locale;
	  protected internal bool withLocalizationFallback;

	  public GetExecutionVariableInstanceCmd(string executionId, string variableName, bool isLocal)
	  {
		this.executionId = executionId;
		this.variableName = variableName;
		this.isLocal = isLocal;
	  }

	  public GetExecutionVariableInstanceCmd(string executionId, string variableName, bool isLocal, string locale, bool withLocalizationFallback)
	  {
		this.executionId = executionId;
		this.variableName = variableName;
		this.isLocal = isLocal;
		this.locale = locale;
		this.withLocalizationFallback = withLocalizationFallback;
	  }

	  public virtual VariableInstance execute(CommandContext commandContext)
	  {
		if (executionId == null)
		{
		  throw new ActivitiIllegalArgumentException("executionId is null");
		}
		if (variableName == null)
		{
		  throw new ActivitiIllegalArgumentException("variableName is null");
		}

		ExecutionEntity execution = commandContext.ExecutionEntityManager.findExecutionById(executionId);

		if (execution == null)
		{
		  throw new ActivitiObjectNotFoundException("execution " + executionId + " doesn't exist", typeof(Execution));
		}

		VariableInstance variableEntity = null;
		if (isLocal)
		{
		  variableEntity = execution.getVariableInstanceLocal(variableName, false);
		}
		else
		{
		  variableEntity = execution.getVariableInstance(variableName, false);
		}

		if (locale != null)
		{
		  string localizedName = null;
		  string localizedDescription = null;

		  ObjectNode languageNode = Context.getLocalizationElementProperties(locale, variableName, execution.ProcessDefinitionId, withLocalizationFallback);
		  if (languageNode != null)
		  {
			JsonNode nameNode = languageNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_NAME);
			if (nameNode != null)
			{
			  localizedName = nameNode.asText();
			}
			JsonNode descriptionNode = languageNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_DESCRIPTION);
			if (descriptionNode != null)
			{
			  localizedDescription = descriptionNode.asText();
			}
		  }

		  if (variableEntity != null)
		  {
			variableEntity.LocalizedName = localizedName;
			variableEntity.LocalizedDescription = localizedDescription;
		  }
		}

		return variableEntity;
	  }
	}
}