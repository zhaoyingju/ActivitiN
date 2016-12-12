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

	using DefaultFormHandler = org.activiti.engine.impl.form.DefaultFormHandler;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using TaskDefinition = org.activiti.engine.impl.task.TaskDefinition;


	/// <summary>
	/// Command for retrieving start or task form keys.
	/// 
	/// @author Falko Menge (camunda)
	/// </summary>
	public class GetFormKeyCmd : Command<string>
	{

	  protected internal string taskDefinitionKey;
	  protected internal string processDefinitionId;

	  /// <summary>
	  /// Retrieves a start form key.
	  /// </summary>
	  public GetFormKeyCmd(string processDefinitionId)
	  {
		ProcessDefinitionId = processDefinitionId;
	  }

	  /// <summary>
	  /// Retrieves a task form key.
	  /// </summary>
	  public GetFormKeyCmd(string processDefinitionId, string taskDefinitionKey)
	  {
		ProcessDefinitionId = processDefinitionId;
		if (taskDefinitionKey == null || taskDefinitionKey.Length < 1)
		{
		  throw new ActivitiIllegalArgumentException("The task definition key is mandatory, but '" + taskDefinitionKey + "' has been provided.");
		}
		this.taskDefinitionKey = taskDefinitionKey;
	  }

	  protected internal virtual string ProcessDefinitionId
	  {
		  set
		  {
			if (value == null || value.Length < 1)
			{
			  throw new ActivitiIllegalArgumentException("The process definition id is mandatory, but '" + value + "' has been provided.");
			}
			this.processDefinitionId = value;
		  }
	  }

	  public virtual string execute(CommandContext commandContext)
	  {
		ProcessDefinitionEntity processDefinition = commandContext.ProcessEngineConfiguration.DeploymentManager.findDeployedProcessDefinitionById(processDefinitionId);
		DefaultFormHandler formHandler;
		if (taskDefinitionKey == null)
		{
		  // TODO: Maybe add getFormKey() to FormHandler interface to avoid the following cast
		  formHandler = (DefaultFormHandler) processDefinition.StartFormHandler;
		}
		else
		{
		  TaskDefinition taskDefinition = processDefinition.TaskDefinitions[taskDefinitionKey];
		  // TODO: Maybe add getFormKey() to FormHandler interface to avoid the following cast
		  formHandler = (DefaultFormHandler) taskDefinition.TaskFormHandler;
		}
		string formKey = null;
		if (formHandler.FormKey != null)
		{
		  formKey = formHandler.FormKey.ExpressionText;
		}
		return formKey;
	  }

	}

}