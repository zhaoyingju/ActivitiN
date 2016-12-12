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
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using SignalEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.SignalEventSubscriptionEntity;

	/// <summary>
	/// An <seealso cref="ActivitiEventListener"/> that throws a signal event when an event is
	/// dispatched to it.
	/// 
	/// @author Frederik Heremans
	/// 
	/// </summary>
	public class SignalThrowingEventListener : BaseDelegateEventListener
	{

		protected internal string signalName;
		protected internal bool processInstanceScope = true;

		public override void onEvent(ActivitiEvent @event)
		{
			if (isValidEvent(@event))
			{

				if (@event.ProcessInstanceId == null && processInstanceScope)
				{
					throw new ActivitiIllegalArgumentException("Cannot throw process-instance scoped signal, since the dispatched event is not part of an ongoing process instance");
				}

				CommandContext commandContext = Context.CommandContext;
				IList<SignalEventSubscriptionEntity> subscriptionEntities = null;
				if (processInstanceScope)
				{
					subscriptionEntities = commandContext.EventSubscriptionEntityManager.findSignalEventSubscriptionsByProcessInstanceAndEventName(@event.ProcessInstanceId, signalName);
				}
				else
				{
					string tenantId = null;
					if (@event.ProcessDefinitionId != null)
					{
						ProcessDefinitionEntity processDefinitionEntity = commandContext.ProcessEngineConfiguration.DeploymentManager.findDeployedProcessDefinitionById(@event.ProcessDefinitionId);
						tenantId = processDefinitionEntity.TenantId;
					}
					subscriptionEntities = commandContext.EventSubscriptionEntityManager.findSignalEventSubscriptionsByEventName(signalName, tenantId);
				}

				foreach (SignalEventSubscriptionEntity signalEventSubscriptionEntity in subscriptionEntities)
				{
					signalEventSubscriptionEntity.eventReceived(null, false);
				}
			}
		}

		public virtual string SignalName
		{
			set
			{
				this.signalName = value;
			}
		}

		public virtual bool ProcessInstanceScope
		{
			set
			{
				this.processInstanceScope = value;
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