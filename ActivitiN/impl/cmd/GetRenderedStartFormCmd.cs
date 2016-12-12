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

	using StartFormData = org.activiti.engine.form.StartFormData;
	using FormEngine = org.activiti.engine.impl.form.FormEngine;
	using StartFormHandler = org.activiti.engine.impl.form.StartFormHandler;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class GetRenderedStartFormCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string processDefinitionId;
	  protected internal string formEngineName;

	  public GetRenderedStartFormCmd(string processDefinitionId, string formEngineName)
	  {
		this.processDefinitionId = processDefinitionId;
		this.formEngineName = formEngineName;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		ProcessDefinitionEntity processDefinition = commandContext.ProcessEngineConfiguration.DeploymentManager.findDeployedProcessDefinitionById(processDefinitionId);
		if (processDefinition == null)
		{
		  throw new ActivitiObjectNotFoundException("Process Definition '" + processDefinitionId + "' not found", typeof(ProcessDefinition));
		}
		StartFormHandler startFormHandler = processDefinition.StartFormHandler;
		if (startFormHandler == null)
		{
		  return null;
		}

		FormEngine formEngine = commandContext.ProcessEngineConfiguration.FormEngines[formEngineName];

		if (formEngine == null)
		{
		  throw new ActivitiException("No formEngine '" + formEngineName + "' defined process engine configuration");
		}

		StartFormData startForm = startFormHandler.createStartFormData(processDefinition);

		return formEngine.renderStartForm(startForm);
	  }
	}

}