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
	using CancelEventDefinition = org.activiti.bpmn.model.CancelEventDefinition;
	using EndEvent = org.activiti.bpmn.model.EndEvent;
	using EventDefinition = org.activiti.bpmn.model.EventDefinition;
	using TerminateEventDefinition = org.activiti.bpmn.model.TerminateEventDefinition;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
	using StringUtils = org.apache.commons.lang3.StringUtils;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class EndEventParseHandler : AbstractActivityBpmnParseHandler<EndEvent>
	{

		private static readonly Logger logger = LoggerFactory.getLogger(typeof(EndEventParseHandler));

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(EndEvent);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, EndEvent endEvent)
	  {

		ActivityImpl endEventActivity = createActivityOnCurrentScope(bpmnParse, endEvent, BpmnXMLConstants.ELEMENT_EVENT_END);
		EventDefinition eventDefinition = null;
		if (!endEvent.EventDefinitions.Empty)
		{
		  eventDefinition = endEvent.EventDefinitions.get(0);
		}

		// Error end event
		if (eventDefinition is org.activiti.bpmn.model.ErrorEventDefinition)
		{
		  org.activiti.bpmn.model.ErrorEventDefinition errorDefinition = (org.activiti.bpmn.model.ErrorEventDefinition) eventDefinition;
		  if (bpmnParse.BpmnModel.containsErrorRef(errorDefinition.ErrorCode))
		  {
			string errorCode = bpmnParse.BpmnModel.Errors.get(errorDefinition.ErrorCode);
			if (StringUtils.isEmpty(errorCode))
			{
			  logger.warn("errorCode is required for an error event " + endEvent.Id);
			}
			endEventActivity.setProperty("type", "errorEndEvent");
			errorDefinition.ErrorCode = errorCode;
		  }
		  endEventActivity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createErrorEndEventActivityBehavior(endEvent, errorDefinition);

		// Cancel end event      
		}
		else if (eventDefinition is CancelEventDefinition)
		{
		  ScopeImpl scope = bpmnParse.CurrentScope;
		  if (scope.getProperty("type") == null || !scope.getProperty("type").Equals("transaction"))
		  {
			logger.warn("end event with cancelEventDefinition only supported inside transaction subprocess (id=" + endEvent.Id + ")");
		  }
		  else
		  {
			endEventActivity.setProperty("type", "cancelEndEvent");
			endEventActivity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createCancelEndEventActivityBehavior(endEvent);
		  }

		// Terminate end event  
		}
		else if (eventDefinition is TerminateEventDefinition)
		{
		  endEventActivity.Async = endEvent.Asynchronous;
		  endEventActivity.Exclusive = !endEvent.NotExclusive;
		  endEventActivity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createTerminateEndEventActivityBehavior(endEvent);

		// None end event  
		}
		else if (eventDefinition == null)
		{
		  endEventActivity.Async = endEvent.Asynchronous;
		  endEventActivity.Exclusive = !endEvent.NotExclusive;
		  endEventActivity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createNoneEndEventActivityBehavior(endEvent);
		}
	  }

	}

}