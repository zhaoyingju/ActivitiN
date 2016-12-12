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
	using MessageEventDefinition = org.activiti.bpmn.model.MessageEventDefinition;
	using SignalEventDefinition = org.activiti.bpmn.model.SignalEventDefinition;
	using StartEvent = org.activiti.bpmn.model.StartEvent;
	using TimerEventDefinition = org.activiti.bpmn.model.TimerEventDefinition;
	using EventSubProcessStartEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.EventSubProcessStartEventActivityBehavior;
	using DefaultStartFormHandler = org.activiti.engine.impl.form.DefaultStartFormHandler;
	using StartFormHandler = org.activiti.engine.impl.form.StartFormHandler;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
	using StringUtils = org.apache.commons.lang3.StringUtils;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class StartEventParseHandler : AbstractActivityBpmnParseHandler<StartEvent>
	{

		private static Logger logger = LoggerFactory.getLogger(typeof(StartEventParseHandler));

	  public const string PROPERTYNAME_INITIATOR_VARIABLE_NAME = "initiatorVariableName";
	  public const string PROPERTYNAME_INITIAL = "initial";

	  public override Type HandledType
	  {
		  get
		  {
			return typeof(StartEvent);
		  }
	  }

	  protected internal override void executeParse(BpmnParse bpmnParse, StartEvent startEvent)
	  {
		ActivityImpl startEventActivity = createActivityOnCurrentScope(bpmnParse, startEvent, BpmnXMLConstants.ELEMENT_EVENT_START);

		ScopeImpl scope = bpmnParse.CurrentScope;
		if (scope is ProcessDefinitionEntity)
		{
		  createProcessDefinitionStartEvent(bpmnParse, startEventActivity, startEvent, (ProcessDefinitionEntity) scope);
		  selectInitial(bpmnParse, startEventActivity, startEvent, (ProcessDefinitionEntity) scope);
		  createStartFormHandlers(bpmnParse, startEvent, (ProcessDefinitionEntity) scope);
		}
		else
		{
		  createScopeStartEvent(bpmnParse, startEventActivity, startEvent);
		}
	  }

	  protected internal virtual void selectInitial(BpmnParse bpmnParse, ActivityImpl startEventActivity, StartEvent startEvent, ProcessDefinitionEntity processDefinition)
	  {
		if (processDefinition.Initial == null)
		{
		  processDefinition.Initial = startEventActivity;
		}
		else
		{
		  // validate that there is a single none start event / timer start event:
		  if (!startEventActivity.getProperty("type").Equals("messageStartEvent") && !startEventActivity.getProperty("type").Equals("signalStartEvent") && !startEventActivity.getProperty("type").Equals("startTimerEvent"))
		  {
			string currentInitialType = (string) processDefinition.Initial.getProperty("type");
			if (currentInitialType.Equals("messageStartEvent"))
			{
			  processDefinition.Initial = startEventActivity;
			}
			else
			{
			  throw new ActivitiException("multiple none start events or timer start events not supported on process definition");
			}
		  }
		}
	  }

	  protected internal virtual void createStartFormHandlers(BpmnParse bpmnParse, StartEvent startEvent, ProcessDefinitionEntity processDefinition)
	  {
		if (processDefinition.Initial != null)
		{
		  if (startEvent.Id.Equals(processDefinition.Initial.Id))
		  {
			StartFormHandler startFormHandler = new DefaultStartFormHandler();
			startFormHandler.parseConfiguration(startEvent.FormProperties, startEvent.FormKey, bpmnParse.Deployment, processDefinition);
			processDefinition.StartFormHandler = startFormHandler;
		  }
		}
	  }

	  protected internal virtual void createProcessDefinitionStartEvent(BpmnParse bpmnParse, ActivityImpl startEventActivity, StartEvent startEvent, ProcessDefinitionEntity processDefinition)
	  {
		if (StringUtils.isNotEmpty(startEvent.Initiator))
		{
		  processDefinition.setProperty(PROPERTYNAME_INITIATOR_VARIABLE_NAME, startEvent.Initiator);
		}

		// all start events share the same behavior:
		startEventActivity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createNoneStartEventActivityBehavior(startEvent);
		if (!startEvent.EventDefinitions.Empty)
		{
		  EventDefinition eventDefinition = startEvent.EventDefinitions.get(0);
		  if (eventDefinition is TimerEventDefinition || eventDefinition is MessageEventDefinition || eventDefinition is SignalEventDefinition)
		  {
			bpmnParse.getBpmnParserHandlers().parseElement(bpmnParse, eventDefinition);
		  }
		  else
		  {
			logger.warn("Unsupported event definition on start event", eventDefinition);
		  }
		}
	  }

	  protected internal virtual void createScopeStartEvent(BpmnParse bpmnParse, ActivityImpl startEventActivity, StartEvent startEvent)
	  {

		ScopeImpl scope = bpmnParse.CurrentScope;
		object triggeredByEvent = scope.getProperty("triggeredByEvent");
		bool isTriggeredByEvent = triggeredByEvent != null && ((bool?) triggeredByEvent == true);

		if (isTriggeredByEvent) // event subprocess
		{

		  // all start events of an event subprocess share common behavior
		  EventSubProcessStartEventActivityBehavior activityBehavior = bpmnParse.ActivityBehaviorFactory.createEventSubProcessStartEventActivityBehavior(startEvent, startEventActivity.Id);
		  startEventActivity.ActivityBehavior = activityBehavior;

		  if (!startEvent.EventDefinitions.Empty)
		  {
			EventDefinition eventDefinition = startEvent.EventDefinitions.get(0);

			if (eventDefinition is org.activiti.bpmn.model.ErrorEventDefinition || eventDefinition is MessageEventDefinition || eventDefinition is SignalEventDefinition)
			{
			  bpmnParse.getBpmnParserHandlers().parseElement(bpmnParse, eventDefinition);
			}
			else
			{
			  logger.warn("start event of event subprocess must be of type 'error', 'message' or 'signal' for start event " + startEvent.Id);
			}
		  }

		} // "regular" subprocess
		else
		{

		  if (!startEvent.EventDefinitions.Empty)
		  {
			logger.warn("event definitions only allowed on start event if subprocess is an event subprocess " + bpmnParse.CurrentSubProcess.Id);
		  }
		  if (scope.getProperty(PROPERTYNAME_INITIAL) == null)
		  {
			scope.setProperty(PROPERTYNAME_INITIAL, startEventActivity);
			startEventActivity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createNoneStartEventActivityBehavior(startEvent);
		  }
		  else
		  {
			logger.warn("multiple start events not supported for subprocess", bpmnParse.CurrentSubProcess.Id);
		  }
		}

	  }

	}

}