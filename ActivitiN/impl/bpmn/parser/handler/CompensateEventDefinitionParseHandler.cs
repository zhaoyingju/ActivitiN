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

	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using BoundaryEvent = org.activiti.bpmn.model.BoundaryEvent;
	using CompensateEventDefinition = org.activiti.bpmn.model.CompensateEventDefinition;
	using ThrowEvent = org.activiti.bpmn.model.ThrowEvent;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
	using StringUtils = org.apache.commons.lang3.StringUtils;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class CompensateEventDefinitionParseHandler : AbstractBpmnParseHandler<CompensateEventDefinition>
	{

		private static readonly Logger logger = LoggerFactory.getLogger(typeof(CompensateEventDefinitionParseHandler));

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(CompensateEventDefinition);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, CompensateEventDefinition eventDefinition)
	  {

		ScopeImpl scope = bpmnParse.CurrentScope;
		if (StringUtils.isNotEmpty(eventDefinition.ActivityRef))
		{
		  if (scope.findActivity(eventDefinition.ActivityRef) == null)
		  {
			logger.warn("Invalid attribute value for 'activityRef': no activity with id '" + eventDefinition.ActivityRef + "' in current scope " + scope.Id);
		  }
		}

		org.activiti.engine.impl.bpmn.parser.CompensateEventDefinition compensateEventDefinition = new org.activiti.engine.impl.bpmn.parser.CompensateEventDefinition();
		compensateEventDefinition.ActivityRef = eventDefinition.ActivityRef;
		compensateEventDefinition.WaitForCompletion = eventDefinition.WaitForCompletion;

		ActivityImpl activity = bpmnParse.CurrentActivity;
		if (bpmnParse.CurrentFlowElement is ThrowEvent)
		{

		  activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createIntermediateThrowCompensationEventActivityBehavior((ThrowEvent) bpmnParse.CurrentFlowElement, compensateEventDefinition);

		}
		else if (bpmnParse.CurrentFlowElement is BoundaryEvent)
		{

		  BoundaryEvent boundaryEvent = (BoundaryEvent) bpmnParse.CurrentFlowElement;
		  bool interrupting = boundaryEvent.CancelActivity;

		  activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createBoundaryEventActivityBehavior(boundaryEvent, interrupting, activity);
		  activity.setProperty("type", "compensationBoundaryCatch");

		}
		else
		{

		  // What to do?

		}

	  }

	}

}