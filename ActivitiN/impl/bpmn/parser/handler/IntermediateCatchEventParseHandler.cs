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
namespace org.activiti.engine.impl.bpmn.parser.handler
{

	using BpmnXMLConstants = org.activiti.bpmn.constants.BpmnXMLConstants;
	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using EventDefinition = org.activiti.bpmn.model.EventDefinition;
	using IntermediateCatchEvent = org.activiti.bpmn.model.IntermediateCatchEvent;
	using MessageEventDefinition = org.activiti.bpmn.model.MessageEventDefinition;
	using SignalEventDefinition = org.activiti.bpmn.model.SignalEventDefinition;
	using TimerEventDefinition = org.activiti.bpmn.model.TimerEventDefinition;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class IntermediateCatchEventParseHandler : AbstractFlowNodeBpmnParseHandler<IntermediateCatchEvent>
	{

		private static readonly Logger logger = LoggerFactory.getLogger(typeof(IntermediateCatchEventParseHandler));

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(IntermediateCatchEvent);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, IntermediateCatchEvent @event)
	  {

		ActivityImpl nestedActivity = null;
		EventDefinition eventDefinition = null;
		if (!@event.EventDefinitions.Empty)
		{
		  eventDefinition = @event.EventDefinitions.get(0);
		}

		if (eventDefinition == null)
		{

		  nestedActivity = createActivityOnCurrentScope(bpmnParse, @event, BpmnXMLConstants.ELEMENT_EVENT_CATCH);
		  nestedActivity.Async = @event.Asynchronous;
		  nestedActivity.Exclusive = !@event.NotExclusive;

		}
		else
		{

		  ScopeImpl scope = bpmnParse.CurrentScope;
		  string eventBasedGatewayId = getPrecedingEventBasedGateway(bpmnParse, @event);
		  if (eventBasedGatewayId != null)
		  {
			ActivityImpl gatewayActivity = scope.findActivity(eventBasedGatewayId);
			nestedActivity = createActivityOnScope(bpmnParse, @event, BpmnXMLConstants.ELEMENT_EVENT_CATCH, gatewayActivity);
		  }
		  else
		  {
			nestedActivity = createActivityOnScope(bpmnParse, @event, BpmnXMLConstants.ELEMENT_EVENT_CATCH, scope);
		  }

		  nestedActivity.Async = @event.Asynchronous;
		  nestedActivity.Exclusive = !@event.NotExclusive;

		  // Catch event behavior is the same for all types
		  nestedActivity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createIntermediateCatchEventActivityBehavior(@event);

		  if (eventDefinition is TimerEventDefinition || eventDefinition is SignalEventDefinition || eventDefinition is MessageEventDefinition)
		  {

			bpmnParse.getBpmnParserHandlers().parseElement(bpmnParse, eventDefinition);

		  }
		  else
		  {
			logger.warn("Unsupported intermediate catch event type for event " + @event.Id);
		  }
		}
	  }

	}

}