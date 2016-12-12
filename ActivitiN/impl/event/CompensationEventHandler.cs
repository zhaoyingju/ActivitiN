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

namespace org.activiti.engine.impl.@event
{

	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using ScopeUtil = org.activiti.engine.impl.bpmn.helper.ScopeUtil;
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CompensateEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.CompensateEventSubscriptionEntity;
	using EventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using AtomicOperation = org.activiti.engine.impl.pvm.runtime.AtomicOperation;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class CompensationEventHandler : EventHandler
	{

	  public const string EVENT_HANDLER_TYPE = "compensate";

	  public virtual string EventHandlerType
	  {
		  get
		  {
			return EVENT_HANDLER_TYPE;
		  }
	  }

	  public virtual void handleEvent(EventSubscriptionEntity eventSubscription, object payload, CommandContext commandContext)
	  {

		string configuration = eventSubscription.Configuration;
		if (configuration == null)
		{
		  throw new ActivitiException("Compensating execution not set for compensate event subscription with id " + eventSubscription.Id);
		}

		ExecutionEntity compensatingExecution = commandContext.ExecutionEntityManager.findExecutionById(configuration);

		ActivityImpl compensationHandler = eventSubscription.Activity;

		if ((compensationHandler.getProperty(BpmnParse.PROPERTYNAME_IS_FOR_COMPENSATION) == null || !(bool?)compensationHandler.getProperty(BpmnParse.PROPERTYNAME_IS_FOR_COMPENSATION)) && compensationHandler.Scope)
		{

		  // descend into scope:
		  IList<CompensateEventSubscriptionEntity> eventsForThisScope = compensatingExecution.CompensateEventSubscriptions;
		  ScopeUtil.throwCompensationEvent(eventsForThisScope, compensatingExecution, false);

		}
		else
		{
		  try
		  {

			  if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
			  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				  commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createActivityEvent(ActivitiEventType.ACTIVITY_COMPENSATE, compensationHandler.Id, (string) compensationHandler.getProperty("name"), compensatingExecution.Id, compensatingExecution.ProcessInstanceId, compensatingExecution.ProcessDefinitionId, (string) compensatingExecution.Activity.Properties["type"], compensatingExecution.Activity.ActivityBehavior.GetType().FullName));
			  }
			compensatingExecution.Activity = compensationHandler;

			// executing the atomic operation makes sure activity start events are fired
			compensatingExecution.performOperation(org.activiti.engine.impl.pvm.runtime.AtomicOperation_Fields.ACTIVITY_START);

		  }
		  catch (Exception e)
		  {
			throw new ActivitiException("Error while handling compensation event " + eventSubscription, e);
		  }

		}
	  }

	}

}