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
namespace org.activiti.engine.impl.bpmn.helper
{

	using ActivitiEvent = org.activiti.engine.@delegate.@event.ActivitiEvent;
	using ActivitiEventListener = org.activiti.engine.@delegate.@event.ActivitiEventListener;
	using Context = org.activiti.engine.impl.context.Context;
	using MessageEventHandler = org.activiti.engine.impl.@event.MessageEventHandler;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using EventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.EventSubscriptionEntity;

	/// <summary>
	/// An <seealso cref="ActivitiEventListener"/> that throws a message event when an event is
	/// dispatched to it. Sends the message to the execution the event was fired from. If the execution
	/// is not subscribed to a message, the process-instance is checked.
	/// 
	/// @author Frederik Heremans
	/// 
	/// </summary>
	public class MessageThrowingEventListener : BaseDelegateEventListener
	{

		protected internal string messageName;
		protected internal new Type entityClass;

		public override void onEvent(ActivitiEvent @event)
		{
			if (isValidEvent(@event))
			{

				if (@event.ProcessInstanceId == null)
				{
					throw new ActivitiIllegalArgumentException("Cannot throw process-instance scoped message, since the dispatched event is not part of an ongoing process instance");
				}

				CommandContext commandContext = Context.CommandContext;
				IList<EventSubscriptionEntity> subscriptionEntities = commandContext.EventSubscriptionEntityManager.findEventSubscriptionsByNameAndExecution(MessageEventHandler.EVENT_HANDLER_TYPE, messageName, @event.ExecutionId);

				// Revert to messaging the process instance
				if (subscriptionEntities.Count == 0 && @event.ProcessInstanceId != null && !@event.ExecutionId.Equals(@event.ProcessInstanceId))
				{
					subscriptionEntities = commandContext.EventSubscriptionEntityManager.findEventSubscriptionsByNameAndExecution(MessageEventHandler.EVENT_HANDLER_TYPE, messageName, @event.ProcessInstanceId);
				}

				foreach (EventSubscriptionEntity signalEventSubscriptionEntity in subscriptionEntities)
				{
					signalEventSubscriptionEntity.eventReceived(null, false);
				}
			}
		}

		public virtual string MessageName
		{
			set
			{
			  this.messageName = value;
			}
		}

		public override bool FailOnException
		{
			get
			{
				return true;
			}
		}
	}

}