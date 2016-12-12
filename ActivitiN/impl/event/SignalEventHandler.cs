using System.Collections;
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

namespace org.activiti.engine.impl.@event
{

	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using DeploymentManager = org.activiti.engine.impl.persistence.deploy.DeploymentManager;
	using EventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;



	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public class SignalEventHandler : AbstractEventHandler
	{

	  public const string EVENT_HANDLER_TYPE = "signal";

	  public override string EventHandlerType
	  {
		  get
		  {
			return EVENT_HANDLER_TYPE;
		  }
	  }

	  public override void handleEvent(EventSubscriptionEntity eventSubscription, object payload, CommandContext commandContext)
	  {
		  if (eventSubscription.ExecutionId != null)
		  {
			  base.handleEvent(eventSubscription, payload, commandContext);
		  }
		  else if (eventSubscription.ProcessDefinitionId != null)
		  {
			  // Start event
			  string processDefinitionId = eventSubscription.ProcessDefinitionId;
			  DeploymentManager deploymentCache = Context.ProcessEngineConfiguration.DeploymentManager;

			  ProcessDefinitionEntity processDefinition = deploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
			  if (processDefinition == null)
			  {
				  throw new ActivitiObjectNotFoundException("No process definition found for id '" + processDefinitionId + "'", typeof(ProcessDefinition));
			  }

			  ActivityImpl startActivity = processDefinition.findActivity(eventSubscription.ActivityId);
			  if (startActivity == null)
			  {
				  throw new ActivitiException("Could no handle signal: no start activity found with id " + eventSubscription.ActivityId);
			  }
			  ExecutionEntity processInstance = processDefinition.createProcessInstance(null, startActivity);
			  if (processInstance == null)
			  {
				  throw new ActivitiException("Could not handle signal: no process instance started");
			  }

			  if (payload != null)
			  {
				  if (payload is IDictionary)
				  {
					  IDictionary<string, object> variables = (IDictionary<string, object>) payload;
					  processInstance.Variables = variables;
				  }
			  }

			  processInstance.start();
		  }
		  else
		  {
			  throw new ActivitiException("Invalid signal handling: no execution nor process definition set");
		  }
	  }

	}

}