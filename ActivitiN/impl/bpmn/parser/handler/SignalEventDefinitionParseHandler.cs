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
	using IntermediateCatchEvent = org.activiti.bpmn.model.IntermediateCatchEvent;
	using Signal = org.activiti.bpmn.model.Signal;
	using SignalEventDefinition = org.activiti.bpmn.model.SignalEventDefinition;
	using StartEvent = org.activiti.bpmn.model.StartEvent;
	using ThrowEvent = org.activiti.bpmn.model.ThrowEvent;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class SignalEventDefinitionParseHandler : AbstractBpmnParseHandler<SignalEventDefinition>
	{

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(SignalEventDefinition);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, SignalEventDefinition signalDefinition)
	  {

		Signal signal = null;
		if (bpmnParse.BpmnModel.containsSignalId(signalDefinition.SignalRef))
		{
		  signal = bpmnParse.BpmnModel.getSignal(signalDefinition.SignalRef);
		  string signalName = signal.Name;
		  signalDefinition.SignalRef = signalName;
		}

		if (signal == null)
		{
		  return;
		}

		ActivityImpl activity = bpmnParse.CurrentActivity;
		if (bpmnParse.CurrentFlowElement is StartEvent)
		{

		  activity.setProperty("type", "signalStartEvent");

		  EventSubscriptionDeclaration eventSubscriptionDeclaration = new EventSubscriptionDeclaration(signalDefinition.SignalRef, "signal");
		  eventSubscriptionDeclaration.ActivityId = activity.Id;
		  eventSubscriptionDeclaration.StartEvent = true;
		  addEventSubscriptionDeclaration(bpmnParse, eventSubscriptionDeclaration, signalDefinition, bpmnParse.CurrentScope);

		}
		else if (bpmnParse.CurrentFlowElement is IntermediateCatchEvent)
		{

		  activity.setProperty("type", "intermediateSignalCatch");

		  EventSubscriptionDeclaration eventSubscriptionDeclaration = new EventSubscriptionDeclaration(signalDefinition.SignalRef, "signal");

		  if (signal.Scope != null)
		  {
			eventSubscriptionDeclaration.Configuration = signal.Scope;
		  }

		  if (getPrecedingEventBasedGateway(bpmnParse, (IntermediateCatchEvent) bpmnParse.CurrentFlowElement) != null)
		  {
			eventSubscriptionDeclaration.ActivityId = activity.Id;
			addEventSubscriptionDeclaration(bpmnParse, eventSubscriptionDeclaration, signalDefinition, activity.Parent);
		  }
		  else
		  {
			activity.Scope = true;
			addEventSubscriptionDeclaration(bpmnParse, eventSubscriptionDeclaration, signalDefinition, activity);
		  }

		}
		else if (bpmnParse.CurrentFlowElement is ThrowEvent)
		{

		  ThrowEvent throwEvent = (ThrowEvent) bpmnParse.CurrentFlowElement;

		  activity.setProperty("type", "intermediateSignalThrow");
		  EventSubscriptionDeclaration eventSubscriptionDeclaration = new EventSubscriptionDeclaration(signalDefinition.SignalRef, "signal");
		  eventSubscriptionDeclaration.Async = signalDefinition.Async;

		  activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createIntermediateThrowSignalEventActivityBehavior(throwEvent, signal, eventSubscriptionDeclaration);

		}
		else if (bpmnParse.CurrentFlowElement is BoundaryEvent)
		{

		  BoundaryEvent boundaryEvent = (BoundaryEvent) bpmnParse.CurrentFlowElement;
		  bool interrupting = boundaryEvent.CancelActivity;
		  activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createBoundaryEventActivityBehavior(boundaryEvent, interrupting, activity);

		  activity.setProperty("type", "boundarySignal");

		  EventSubscriptionDeclaration eventSubscriptionDeclaration = new EventSubscriptionDeclaration(signalDefinition.SignalRef, "signal");
		  eventSubscriptionDeclaration.ActivityId = activity.Id;

		  if (signal.Scope != null)
		  {
			eventSubscriptionDeclaration.Configuration = signal.Scope;
		  }

		  addEventSubscriptionDeclaration(bpmnParse, eventSubscriptionDeclaration, signalDefinition, activity.Parent);

		  if (activity.Parent is ActivityImpl)
		  {
			((ActivityImpl) activity.Parent).Scope = true;
		  }

		}


	  }

	}

}