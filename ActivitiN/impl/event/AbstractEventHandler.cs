using System;
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

	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using BoundaryEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.BoundaryEventActivityBehavior;
	using EventSubProcessStartEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.EventSubProcessStartEventActivityBehavior;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using EventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// </summary>
	public abstract class AbstractEventHandler : EventHandler
	{
		public abstract string EventHandlerType {get;}

	  public virtual void handleEvent(EventSubscriptionEntity eventSubscription, object payload, CommandContext commandContext)
	  {

		ExecutionEntity execution = eventSubscription.getExecution();
		ActivityImpl activity = eventSubscription.Activity;

		if (activity == null)
		{
		  throw new ActivitiException("Error while sending signal for event subscription '" + eventSubscription.Id + "': " + "no activity associated with event subscription");
		}

		if (payload is IDictionary)
		{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Map<String, Object> processVariables = (java.util.Map<String, Object>) payload;
		  IDictionary<string, object> processVariables = (IDictionary<string, object>) payload;
		  execution.Variables = processVariables;
		}

		ActivityBehavior activityBehavior = activity.ActivityBehavior;
		if (activityBehavior is BoundaryEventActivityBehavior || activityBehavior is EventSubProcessStartEventActivityBehavior)
		{

		  try
		  {

			dispatchActivitiesCanceledIfNeeded(eventSubscription, execution, activity, commandContext);

			activityBehavior.execute(execution);

		  }
		  catch (Exception e)
		  {
			throw e;
		  }
		  catch (Exception e)
		  {
			throw new ActivitiException("exception while sending signal for event subscription '" + eventSubscription + "':" + e.Message, e);
		  }

		} // not boundary
		else
		{
		  if (!activity.Equals(execution.Activity))
		  {
			execution.Activity = activity;
		  }
		  execution.signal(eventSubscription.EventName, payload);
		}
	  }

	  protected internal virtual void dispatchActivitiesCanceledIfNeeded(EventSubscriptionEntity eventSubscription, ExecutionEntity execution, ActivityImpl boundaryEventActivity, CommandContext commandContext)
	  {
		ActivityBehavior boundaryActivityBehavior = boundaryEventActivity.ActivityBehavior;
		if (boundaryActivityBehavior is BoundaryEventActivityBehavior)
		{
		  BoundaryEventActivityBehavior boundaryEventActivityBehavior = (BoundaryEventActivityBehavior) boundaryActivityBehavior;
		  if (boundaryEventActivityBehavior.Interrupting)
		  {
			dispatchExecutionCancelled(eventSubscription, execution, commandContext);
		  }
		}
	  }

	  protected internal virtual void dispatchExecutionCancelled(EventSubscriptionEntity eventSubscription, ExecutionEntity execution, CommandContext commandContext)
	  {
		// subprocesses
		foreach (ExecutionEntity subExecution in execution.Executions)
		{
		  dispatchExecutionCancelled(eventSubscription, subExecution, commandContext);
		}

		// call activities
		ExecutionEntity subProcessInstance = commandContext.ExecutionEntityManager.findSubProcessInstanceBySuperExecutionId(execution.Id);
		if (subProcessInstance != null)
		{
		  dispatchExecutionCancelled(eventSubscription, subProcessInstance, commandContext);
		}

		// activity with message/signal boundary events
		ActivityImpl activity = execution.Activity;
		if (activity != null && activity.ActivityBehavior != null)
		{
		  dispatchActivityCancelled(eventSubscription, execution, activity, commandContext);
		}
	  }

	  protected internal virtual void dispatchActivityCancelled(EventSubscriptionEntity eventSubscription, ExecutionEntity execution, ActivityImpl activity, CommandContext commandContext)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		commandContext.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createActivityCancelledEvent(activity.Id, (string) activity.Properties["name"], execution.Id, execution.ProcessInstanceId, execution.ProcessDefinitionId, (string) activity.Properties["type"], activity.ActivityBehavior.GetType().FullName, eventSubscription));
	  }

	}

}