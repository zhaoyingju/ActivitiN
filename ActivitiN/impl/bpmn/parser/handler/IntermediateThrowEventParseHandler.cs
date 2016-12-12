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
	using SignalEventDefinition = org.activiti.bpmn.model.SignalEventDefinition;
	using ThrowEvent = org.activiti.bpmn.model.ThrowEvent;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class IntermediateThrowEventParseHandler : AbstractActivityBpmnParseHandler<ThrowEvent>
	{

		private static readonly Logger logger = LoggerFactory.getLogger(typeof(IntermediateThrowEventParseHandler));

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(ThrowEvent);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, ThrowEvent intermediateEvent)
	  {

		ActivityImpl nestedActivityImpl = createActivityOnCurrentScope(bpmnParse, intermediateEvent, BpmnXMLConstants.ELEMENT_EVENT_THROW);

		EventDefinition eventDefinition = null;
		if (!intermediateEvent.EventDefinitions.Empty)
		{
		  eventDefinition = intermediateEvent.EventDefinitions.get(0);
		}

		nestedActivityImpl.Async = intermediateEvent.Asynchronous;
		nestedActivityImpl.Exclusive = !intermediateEvent.NotExclusive;

		if (eventDefinition is SignalEventDefinition)
		{
		  bpmnParse.getBpmnParserHandlers().parseElement(bpmnParse, eventDefinition);
		}
		else if (eventDefinition is org.activiti.bpmn.model.CompensateEventDefinition)
		{
		  bpmnParse.getBpmnParserHandlers().parseElement(bpmnParse, eventDefinition);
		}
		else if (eventDefinition == null)
		{
		  nestedActivityImpl.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createIntermediateThrowNoneEventActivityBehavior(intermediateEvent);
		}
		else
		{
		  logger.warn("Unsupported intermediate throw event type for throw event " + intermediateEvent.Id);
		}
	  }

	  //
	  // Seems not to be used anymore?
	  //
	//  protected CompensateEventDefinition createCompensateEventDefinition(BpmnParse bpmnParse, org.activiti.bpmn.model.CompensateEventDefinition eventDefinition, ScopeImpl scopeElement) {
	//    if(StringUtils.isNotEmpty(eventDefinition.getActivityRef())) {
	//      if(scopeElement.findActivity(eventDefinition.getActivityRef()) == null) {
	//        bpmnParse.getBpmnModel().addProblem("Invalid attribute value for 'activityRef': no activity with id '" + eventDefinition.getActivityRef() +
	//            "' in current scope " + scopeElement.getId(), eventDefinition);
	//      }
	//    }
	//    
	//    CompensateEventDefinition compensateEventDefinition =  new CompensateEventDefinition();
	//    compensateEventDefinition.setActivityRef(eventDefinition.getActivityRef());
	//    compensateEventDefinition.setWaitForCompletion(eventDefinition.isWaitForCompletion());
	//    
	//    return compensateEventDefinition;
	//  }

	}

}