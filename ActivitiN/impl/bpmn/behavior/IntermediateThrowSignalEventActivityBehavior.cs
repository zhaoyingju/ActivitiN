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

namespace org.activiti.engine.impl.bpmn.behavior
{

	using Signal = org.activiti.bpmn.model.Signal;
	using ThrowEvent = org.activiti.bpmn.model.ThrowEvent;
	using EventSubscriptionDeclaration = org.activiti.engine.impl.bpmn.parser.EventSubscriptionDeclaration;
	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using SignalEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.SignalEventSubscriptionEntity;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class IntermediateThrowSignalEventActivityBehavior : AbstractBpmnActivityBehavior
	{

	  private const long serialVersionUID = -2961893934810190972L;

	  protected internal readonly bool processInstanceScope;
	  protected internal readonly EventSubscriptionDeclaration signalDefinition;

	  public IntermediateThrowSignalEventActivityBehavior(ThrowEvent throwEvent, Signal signal, EventSubscriptionDeclaration signalDefinition)
	  {
		this.processInstanceScope = Signal.SCOPE_PROCESS_INSTANCE.Equals(signal.Scope);
		this.signalDefinition = signalDefinition;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {

		CommandContext commandContext = Context.CommandContext;

		IList<SignalEventSubscriptionEntity> subscriptionEntities = null;
		if (processInstanceScope)
		{
		  subscriptionEntities = commandContext.EventSubscriptionEntityManager.findSignalEventSubscriptionsByProcessInstanceAndEventName(execution.ProcessInstanceId, signalDefinition.EventName);
		}
		else
		{
		  subscriptionEntities = commandContext.EventSubscriptionEntityManager.findSignalEventSubscriptionsByEventName(signalDefinition.EventName, execution.TenantId);
		}

		foreach (SignalEventSubscriptionEntity signalEventSubscriptionEntity in subscriptionEntities)
		{
		  signalEventSubscriptionEntity.eventReceived(null, signalDefinition.Async);
		}

		if (execution.Activity != null) // dont continue if process has already finished
		{
		  leave(execution);
		}
	  }

	}

}