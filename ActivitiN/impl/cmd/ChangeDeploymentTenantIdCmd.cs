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


	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using DeploymentEntity = org.activiti.engine.impl.persistence.entity.DeploymentEntity;
	using Deployment = org.activiti.engine.repository.Deployment;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class ChangeDeploymentTenantIdCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string deploymentId;
	  protected internal string newTenantId;

	  public ChangeDeploymentTenantIdCmd(string deploymentId, string newTenantId)
	  {
		this.deploymentId = deploymentId;
		this.newTenantId = newTenantId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		if (deploymentId == null)
		{
		  throw new ActivitiIllegalArgumentException("deploymentId is null");
		}

		// Update all entities

		DeploymentEntity deployment = commandContext.DeploymentEntityManager.findDeploymentById(deploymentId);
		if (deployment == null)
		{
			throw new ActivitiObjectNotFoundException("Could not find deployment with id " + deploymentId, typeof(Deployment));
		}
		string oldTenantId = deployment.TenantId;
		deployment.TenantId = newTenantId;


		// Doing process instances, executions and tasks with direct SQL updates (otherwise would not be performant) 
		commandContext.ProcessDefinitionEntityManager.updateProcessDefinitionTenantIdForDeployment(deploymentId, newTenantId);
		commandContext.ExecutionEntityManager.updateExecutionTenantIdForDeployment(deploymentId, newTenantId);
		commandContext.TaskEntityManager.updateTaskTenantIdForDeployment(deploymentId, newTenantId);
		commandContext.JobEntityManager.updateJobTenantIdForDeployment(deploymentId, newTenantId);
		commandContext.EventSubscriptionEntityManager.updateEventSubscriptionTenantId(oldTenantId, newTenantId);

		// Doing process definitions in memory, cause we need to clear the process definition cache
		IList<ProcessDefinition> processDefinitions = commandContext.DbSqlSession.createProcessDefinitionQuery().deploymentId(deploymentId).list();
		foreach (ProcessDefinition processDefinition in processDefinitions)
		{
			commandContext.ProcessEngineConfiguration.ProcessDefinitionCache.remove(processDefinition.Id);
		}

		// Clear process definition cache
		commandContext.ProcessEngineConfiguration.ProcessDefinitionCache.clear();

		return null;

	  }


	}

}