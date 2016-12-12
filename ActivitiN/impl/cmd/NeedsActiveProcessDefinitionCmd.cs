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

	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using DeploymentManager = org.activiti.engine.impl.persistence.deploy.DeploymentManager;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public abstract class NeedsActiveProcessDefinitionCmd<T> : Command<T>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string processDefinitionId;

	  public NeedsActiveProcessDefinitionCmd(string processDefinitionId)
	  {
		this.processDefinitionId = processDefinitionId;
	  }

	  public virtual T execute(CommandContext commandContext)
	  {
		DeploymentManager deploymentManager = commandContext.ProcessEngineConfiguration.DeploymentManager;
		ProcessDefinitionEntity processDefinition = deploymentManager.findDeployedProcessDefinitionById(processDefinitionId);

		if (deploymentManager.isProcessDefinitionSuspended(processDefinitionId))
		{
		  throw new ActivitiException("Cannot execute operation because process definition '" + processDefinition.Name + "' (id=" + processDefinition.Id + ") is supended");
		}

		return execute(commandContext, processDefinition);
	  }

	  /// <summary>
	  /// Subclasses should implement this. The provided <seealso cref="ProcessDefinition"/> is 
	  /// guaranteed to be an active process definition (ie. not suspended).
	  /// </summary>
	  protected internal abstract T execute(CommandContext commandContext, ProcessDefinitionEntity processDefinition);

	}

}