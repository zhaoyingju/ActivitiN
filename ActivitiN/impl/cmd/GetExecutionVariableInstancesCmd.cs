using System;
using System.Collections.Generic;

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
	public class GetExecutionVariableInstancesCmd : Command<IDictionary<string, VariableInstance>>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string executionId;
	  protected internal ICollection<string> variableNames;
	  protected internal bool isLocal;
	  protected internal string locale;
	  protected internal bool withLocalizationFallback;

	  public GetExecutionVariableInstancesCmd(string executionId, ICollection<string> variableNames, bool isLocal)
	  {
		this.executionId = executionId;
		this.variableNames = variableNames;
		this.isLocal = isLocal;
	  }

	  public GetExecutionVariableInstancesCmd(string executionId, ICollection<string> variableNames, bool isLocal, string locale, bool withLocalizationFallback)
	  {
		this.executionId = executionId;
		this.variableNames = variableNames;
		this.isLocal = isLocal;
		this.locale = locale;
		this.withLocalizationFallback = withLocalizationFallback;
	  }

	  public virtual IDictionary<string, VariableInstance> execute(CommandContext commandContext)
	  {

		// Verify existance of execution
		if (executionId == null)
		{
		  throw new ActivitiIllegalArgumentException("executionId is null");
		}

		ExecutionEntity execution = commandContext.ExecutionEntityManager.findExecutionById(executionId);

		if (execution == null)
		{
		  throw new ActivitiObjectNotFoundException("execution " + executionId + " doesn't exist", typeof(Execution));
		}

		IDictionary<string, VariableInstance> variables = null;
		if (variableNames == null || variableNames.Count == 0)
		{
		  // Fetch all
		  if (isLocal)
		  {
			variables = execution.VariableInstancesLocal;
		  }
		  else
		  {
			variables = execution.VariableInstances;
		  }

		}
		else
		{
		  // Fetch specific collection of variables
		  if (isLocal)
		  {
			variables = execution.getVariableInstancesLocal(variableNames, false);
		  }
		  else
		  {
			variables = execution.getVariableInstances(variableNames, false);
		  }
		}

		if (variables != null && locale != null)
		{
		  foreach (KeyValuePair<string, VariableInstance> entry in variables)
		  {
			string variableName = entry.Key;
			VariableInstance variableEntity = entry.Value;

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

			variableEntity.LocalizedName = localizedName;
			variableEntity.LocalizedDescription = localizedDescription;
		  }
		}

		return variables;
	  }
	}
}