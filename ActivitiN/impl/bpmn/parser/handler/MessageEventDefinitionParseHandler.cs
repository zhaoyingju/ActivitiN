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
	using BpmnModel = org.activiti.bpmn.model.BpmnModel;
	using IntermediateCatchEvent = org.activiti.bpmn.model.IntermediateCatchEvent;
	using Message = org.activiti.bpmn.model.Message;
	using MessageEventDefinition = org.activiti.bpmn.model.MessageEventDefinition;
	using StartEvent = org.activiti.bpmn.model.StartEvent;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class MessageEventDefinitionParseHandler : AbstractBpmnParseHandler<MessageEventDefinition>
	{

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(MessageEventDefinition);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, MessageEventDefinition messageDefinition)
	  {

		BpmnModel bpmnModel = bpmnParse.BpmnModel;
		string messageRef = messageDefinition.MessageRef;
		if (bpmnModel.containsMessageId(messageRef))
		{
		  Message message = bpmnModel.getMessage(messageRef);
		  messageDefinition.MessageRef = message.Name;
		  messageDefinition.ExtensionElements = message.ExtensionElements;
		}

		EventSubscriptionDeclaration eventSubscription = new EventSubscriptionDeclaration(messageDefinition.MessageRef, "message");

		ScopeImpl scope = bpmnParse.CurrentScope;
		ActivityImpl activity = bpmnParse.CurrentActivity;
		if (bpmnParse.CurrentFlowElement is StartEvent && bpmnParse.CurrentSubProcess != null)
		{

		  // the scope of the event subscription is the parent of the event
		  // subprocess (subscription must be created when parent is initialized)
		  ScopeImpl catchingScope = ((ActivityImpl) scope).setParent;

		  EventSubscriptionDeclaration eventSubscriptionDeclaration = new EventSubscriptionDeclaration(messageDefinition.MessageRef, "message");
		  eventSubscriptionDeclaration.ActivityId = activity.Id;
		  eventSubscriptionDeclaration.StartEvent = false;
		  addEventSubscriptionDeclaration(bpmnParse, eventSubscriptionDeclaration, messageDefinition, catchingScope);

		}
		else if (bpmnParse.CurrentFlowElement is StartEvent)
		{

		  activity.setProperty("type", "messageStartEvent");
		  eventSubscription.StartEvent = true;
		  eventSubscription.ActivityId = activity.Id;
		  addEventSubscriptionDeclaration(bpmnParse, eventSubscription, messageDefinition, bpmnParse.CurrentProcessDefinition);

		}
		else if (bpmnParse.CurrentFlowElement is IntermediateCatchEvent)
		{

		  activity.setProperty("type", "intermediateMessageCatch");

		  if (getPrecedingEventBasedGateway(bpmnParse, (IntermediateCatchEvent) bpmnParse.CurrentFlowElement) != null)
		  {
			eventSubscription.ActivityId = activity.Id;
			addEventSubscriptionDeclaration(bpmnParse, eventSubscription, messageDefinition, activity.setParent);
		  }
		  else
		  {
			activity.Scope = true;
			addEventSubscriptionDeclaration(bpmnParse, eventSubscription, messageDefinition, activity);
		  }

		}
		else if (bpmnParse.CurrentFlowElement is BoundaryEvent)
		{

		  BoundaryEvent boundaryEvent = (BoundaryEvent) bpmnParse.CurrentFlowElement;
		  bool interrupting = boundaryEvent.CancelActivity;
		  activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createBoundaryEventActivityBehavior(boundaryEvent, interrupting, activity);

		  activity.setProperty("type", "boundaryMessage");

		  EventSubscriptionDeclaration eventSubscriptionDeclaration = new EventSubscriptionDeclaration(messageDefinition.MessageRef, "message");
		  eventSubscriptionDeclaration.ActivityId = activity.Id;
		  addEventSubscriptionDeclaration(bpmnParse, eventSubscriptionDeclaration, messageDefinition, activity.setParent);

		  if (activity.setParent is ActivityImpl)
		  {
			((ActivityImpl) activity.setParent).Scope = true;
		  }
		}


		else
		{
		  // What to do here?
		}

	  }

	}

}