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
	using BoundaryEvent = org.activiti.bpmn.model.BoundaryEvent;
	using CancelEventDefinition = org.activiti.bpmn.model.CancelEventDefinition;
	using EventDefinition = org.activiti.bpmn.model.EventDefinition;
	using MessageEventDefinition = org.activiti.bpmn.model.MessageEventDefinition;
	using SignalEventDefinition = org.activiti.bpmn.model.SignalEventDefinition;
	using TimerEventDefinition = org.activiti.bpmn.model.TimerEventDefinition;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class BoundaryEventParseHandler : AbstractFlowNodeBpmnParseHandler<BoundaryEvent>
	{

		private static readonly Logger logger = LoggerFactory.getLogger(typeof(BoundaryEventParseHandler));

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(BoundaryEvent);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, BoundaryEvent boundaryEvent)
	  {

		ActivityImpl parentActivity = findActivity(bpmnParse, boundaryEvent.AttachedToRefId);
		if (parentActivity == null)
		{
		  logger.warn("Invalid reference in boundary event. Make sure that the referenced activity is defined in the same scope as the boundary event " + boundaryEvent.Id);
		  return;
		}

		ActivityImpl nestedActivity = createActivityOnScope(bpmnParse, boundaryEvent, BpmnXMLConstants.ELEMENT_EVENT_BOUNDARY, parentActivity);
		bpmnParse.CurrentActivity = nestedActivity;

		EventDefinition eventDefinition = null;
		if (!boundaryEvent.EventDefinitions.Empty)
		{
		  eventDefinition = boundaryEvent.EventDefinitions.get(0);
		}

		if (eventDefinition is TimerEventDefinition || eventDefinition is org.activiti.bpmn.model.ErrorEventDefinition || eventDefinition is SignalEventDefinition || eventDefinition is CancelEventDefinition || eventDefinition is MessageEventDefinition || eventDefinition is org.activiti.bpmn.model.CompensateEventDefinition)
		{

		  bpmnParse.getBpmnParserHandlers().parseElement(bpmnParse, eventDefinition);

		}
		else
		{
		  logger.warn("Unsupported boundary event type for boundary event " + boundaryEvent.Id);
		}
	  }

	}

}