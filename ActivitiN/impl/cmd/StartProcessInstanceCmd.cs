using System;
using System.Collections.Generic;

/* Licensed under the Apache License, Version 2.0 (the "License");
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
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessInstanceBuilderImpl = org.activiti.engine.impl.runtime.ProcessInstanceBuilderImpl;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class StartProcessInstanceCmd<T> : Command<ProcessInstance>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string processDefinitionKey;
	  protected internal string processDefinitionId;
	  protected internal IDictionary<string, object> variables;
	  protected internal string businessKey;
	  protected internal string tenantId;
	  protected internal string processInstanceName;

	  public StartProcessInstanceCmd(string processDefinitionKey, string processDefinitionId, string businessKey, IDictionary<string, object> variables)
	  {
		this.processDefinitionKey = processDefinitionKey;
		this.processDefinitionId = processDefinitionId;
		this.businessKey = businessKey;
		this.variables = variables;
	  }

	  public StartProcessInstanceCmd(string processDefinitionKey, string processDefinitionId, string businessKey, IDictionary<string, object> variables, string tenantId) : this(processDefinitionKey, processDefinitionId, businessKey, variables)
	  {
		  this.tenantId = tenantId;
	  }

	  public StartProcessInstanceCmd(ProcessInstanceBuilderImpl processInstanceBuilder) : this(processInstanceBuilder.ProcessDefinitionKey, processInstanceBuilder.ProcessDefinitionId, processInstanceBuilder.BusinessKey, processInstanceBuilder.Variables, processInstanceBuilder.TenantId)
	  {
		this.processInstanceName = processInstanceBuilder.ProcessInstanceName;
	  }

	  public virtual ProcessInstance execute(CommandContext commandContext)
	  {
		DeploymentManager deploymentManager = commandContext.ProcessEngineConfiguration.DeploymentManager;

		// Find the process definition
		ProcessDefinitionEntity processDefinition = null;
		if (processDefinitionId != null)
		{
		  processDefinition = deploymentManager.findDeployedProcessDefinitionById(processDefinitionId);
		  if (processDefinition == null)
		  {
			throw new ActivitiObjectNotFoundException("No process definition found for id = '" + processDefinitionId + "'", typeof(ProcessDefinition));
		  }
		}
		else if (processDefinitionKey != null && (tenantId == null || ProcessEngineConfiguration.NO_TENANT_ID.Equals(tenantId)))
		{
		  processDefinition = deploymentManager.findDeployedLatestProcessDefinitionByKey(processDefinitionKey);
		  if (processDefinition == null)
		  {
			throw new ActivitiObjectNotFoundException("No process definition found for key '" + processDefinitionKey + "'", typeof(ProcessDefinition));
		  }
		}
		else if (processDefinitionKey != null && tenantId != null && !ProcessEngineConfiguration.NO_TENANT_ID.Equals(tenantId))
		{
			 processDefinition = deploymentManager.findDeployedLatestProcessDefinitionByKeyAndTenantId(processDefinitionKey, tenantId);
		   if (processDefinition == null)
		   {
			 throw new ActivitiObjectNotFoundException("No process definition found for key '" + processDefinitionKey + "' for tenant identifier " + tenantId, typeof(ProcessDefinition));
		   }
		}
		else
		{
		  throw new ActivitiIllegalArgumentException("processDefinitionKey and processDefinitionId are null");
		}

		// Do not start process a process instance if the process definition is suspended
		if (deploymentManager.isProcessDefinitionSuspended(processDefinition.Id))
		{
		  throw new ActivitiException("Cannot start process instance. Process definition " + processDefinition.Name + " (id = " + processDefinition.Id + ") is suspended");
		}

		// Start the process instance
		ExecutionEntity processInstance = processDefinition.createProcessInstance(businessKey);

		// now set the variables passed into the start command
		initializeVariables(processInstance);

		// now set processInstance name
		if (processInstanceName != null)
		{
		  processInstance.Name = processInstanceName;
		  commandContext.HistoryManager.recordProcessInstanceNameChange(processInstance.Id, processInstanceName);
		}

		processInstance.start();

		return processInstance;
	  }

	  protected internal virtual void initializeVariables(ExecutionEntity processInstance)
	  {
		if (variables != null)
		{
		  processInstance.Variables = variables;
		}
	  }
	}

}