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
	using DeploymentManager = org.activiti.engine.impl.persistence.deploy.DeploymentManager;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using MessageEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.MessageEventSubscriptionEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// @author Vasile Dirla
	/// </summary>
	public class StartProcessInstanceByMessageCmd : Command<ProcessInstance>
	{

	  protected internal readonly string messageName;
	  protected internal readonly string businessKey;
	  protected internal readonly IDictionary<string, object> processVariables;
	  protected internal readonly string tenantId;

	  public StartProcessInstanceByMessageCmd(string messageName, string businessKey, IDictionary<string, object> processVariables, string tenantId)
	  {
		this.messageName = messageName;
		this.businessKey = businessKey;
		this.processVariables = processVariables;
		this.tenantId = tenantId;
	  }

	  public virtual ProcessInstance execute(CommandContext commandContext)
	  {

		if (messageName == null)
		{
		  throw new ActivitiIllegalArgumentException("Cannot start process instance by message: message name is null");
		}

		MessageEventSubscriptionEntity messageEventSubscription = commandContext.EventSubscriptionEntityManager.findMessageStartEventSubscriptionByName(messageName, tenantId);

		if (messageEventSubscription == null)
		{
		  throw new ActivitiObjectNotFoundException("Cannot start process instance by message: no subscription to message with name '" + messageName + "' found.", typeof(MessageEventSubscriptionEntity));
		}

		string processDefinitionId = messageEventSubscription.Configuration;
		if (processDefinitionId == null)
		{
		  throw new ActivitiException("Cannot start process instance by message: subscription to message with name '" + messageName + "' is not a message start event.");
		}

		DeploymentManager deploymentManager = commandContext.ProcessEngineConfiguration.DeploymentManager;

		ProcessDefinitionEntity processDefinition = deploymentManager.findDeployedProcessDefinitionById(processDefinitionId);
		if (processDefinition == null)
		{
		  throw new ActivitiObjectNotFoundException("No process definition found for id '" + processDefinitionId + "'", typeof(ProcessDefinition));
		}

		// Do not start process a process instance if the process definition is suspended
		if (deploymentManager.isProcessDefinitionSuspended(processDefinition.Id))
		{
		  throw new ActivitiException("Cannot start process instance. Process definition " + processDefinition.Name + " (id = " + processDefinition.Id + ") is suspended");
		}

		ActivityImpl startActivity = processDefinition.findActivity(messageEventSubscription.ActivityId);
		ExecutionEntity processInstance = processDefinition.createProcessInstance(businessKey, startActivity);

		if (processVariables != null)
		{
		  processInstance.Variables = processVariables;
		}

		processInstance.start();

		return processInstance;
	  }

	}

}