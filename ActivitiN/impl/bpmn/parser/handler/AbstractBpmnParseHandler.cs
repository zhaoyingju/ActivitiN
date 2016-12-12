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
namespace org.activiti.engine.impl.bpmn.parser.handler
{


	using ActivitiListener = org.activiti.bpmn.model.ActivitiListener;
	using Activity = org.activiti.bpmn.model.Activity;
	using Artifact = org.activiti.bpmn.model.Artifact;
	using Association = org.activiti.bpmn.model.Association;
	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using BpmnModel = org.activiti.bpmn.model.BpmnModel;
	using DataSpec = org.activiti.bpmn.model.DataSpec;
	using EventDefinition = org.activiti.bpmn.model.EventDefinition;
	using EventGateway = org.activiti.bpmn.model.EventGateway;
	using FlowElement = org.activiti.bpmn.model.FlowElement;
	using Gateway = org.activiti.bpmn.model.Gateway;
	using ImplementationType = org.activiti.bpmn.model.ImplementationType;
	using IntermediateCatchEvent = org.activiti.bpmn.model.IntermediateCatchEvent;
	using SequenceFlow = org.activiti.bpmn.model.SequenceFlow;
	using ValuedDataObject = org.activiti.bpmn.model.ValuedDataObject;
	using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;
	using Data = org.activiti.engine.impl.bpmn.data.Data;
	using DataRef = org.activiti.engine.impl.bpmn.data.DataRef;
	using IOSpecification = org.activiti.engine.impl.bpmn.data.IOSpecification;
	using ItemDefinition = org.activiti.engine.impl.bpmn.data.ItemDefinition;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.activiti.engine.impl.pvm.process.TransitionImpl;
	using BpmnParseHandler = org.activiti.engine.parse.BpmnParseHandler;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public abstract class AbstractBpmnParseHandler<T> : BpmnParseHandler where T : org.activiti.bpmn.model.BaseElement
	{

		private static readonly Logger logger = LoggerFactory.getLogger(typeof(AbstractBpmnParseHandler));

	  private static readonly Logger LOGGER = LoggerFactory.getLogger(typeof(AbstractBpmnParseHandler));

	  public const string PROPERTYNAME_IS_FOR_COMPENSATION = "isForCompensation";

	  public const string PROPERTYNAME_EVENT_SUBSCRIPTION_DECLARATION = "eventDefinitions";

	  public const string PROPERTYNAME_ERROR_EVENT_DEFINITIONS = "errorEventDefinitions";

	  public const string PROPERTYNAME_TIMER_DECLARATION = "timerDeclarations";

	  public virtual Set<Type> HandledTypes
	  {
		  get
		  {
			Set<Type> types = new HashSet<Type>();
			types.add(HandledType);
			return types;
		  }
	  }

	  protected internal abstract Type HandledType {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void parse(org.activiti.engine.impl.bpmn.parser.BpmnParse bpmnParse, org.activiti.bpmn.model.BaseElement element)
	  public virtual void parse(BpmnParse bpmnParse, BaseElement element)
	  {
		T baseElement = (T) element;
		executeParse(bpmnParse, baseElement);
	  }

	  protected internal abstract void executeParse(BpmnParse bpmnParse, T element);

	  protected internal virtual ActivityImpl findActivity(BpmnParse bpmnParse, string id)
	  {
		return bpmnParse.CurrentScope.findActivity(id);
	  }

	  public virtual ActivityImpl createActivityOnCurrentScope(BpmnParse bpmnParse, FlowElement flowElement, string xmlLocalName)
	  {
		return createActivityOnScope(bpmnParse, flowElement, xmlLocalName, bpmnParse.CurrentScope);
	  }

	  public virtual ActivityImpl createActivityOnScope(BpmnParse bpmnParse, FlowElement flowElement, string xmlLocalName, ScopeImpl scopeElement)
	  {
		if (LOGGER.DebugEnabled)
		{
		  LOGGER.debug("Parsing activity {}", flowElement.Id);
		}

		ActivityImpl activity = scopeElement.createActivity(flowElement.Id);
		bpmnParse.CurrentActivity = activity;

		activity.setProperty("name", flowElement.Name);
		activity.setProperty("documentation", flowElement.Documentation);
		if (flowElement is Activity)
		{
		  Activity modelActivity = (Activity) flowElement;
		  activity.setProperty("default", modelActivity.DefaultFlow);
		  if (modelActivity.ForCompensation)
		  {
			activity.setProperty(PROPERTYNAME_IS_FOR_COMPENSATION, true);
		  }
		}
		else if (flowElement is Gateway)
		{
		  activity.setProperty("default", ((Gateway) flowElement).DefaultFlow);
		}
		activity.setProperty("type", xmlLocalName);

		return activity;
	  }

	  protected internal virtual void createExecutionListenersOnScope(BpmnParse bpmnParse, IList<ActivitiListener> activitiListenerList, ScopeImpl scope)
	  {
		foreach (ActivitiListener activitiListener in activitiListenerList)
		{
		  scope.addExecutionListener(activitiListener.Event, createExecutionListener(bpmnParse, activitiListener));
		}
	  }

	  protected internal virtual void createExecutionListenersOnTransition(BpmnParse bpmnParse, IList<ActivitiListener> activitiListenerList, TransitionImpl transition)
	  {
		foreach (ActivitiListener activitiListener in activitiListenerList)
		{
		  transition.addExecutionListener(createExecutionListener(bpmnParse, activitiListener));
		}
	  }

	  protected internal virtual ExecutionListener createExecutionListener(BpmnParse bpmnParse, ActivitiListener activitiListener)
	  {
		ExecutionListener executionListener = null;

		if (ImplementationType.IMPLEMENTATION_TYPE_CLASS.equalsIgnoreCase(activitiListener.ImplementationType))
		{
		  executionListener = bpmnParse.ListenerFactory.createClassDelegateExecutionListener(activitiListener);
		}
		else if (ImplementationType.IMPLEMENTATION_TYPE_EXPRESSION.equalsIgnoreCase(activitiListener.ImplementationType))
		{
		  executionListener = bpmnParse.ListenerFactory.createExpressionExecutionListener(activitiListener);
		}
		else if (ImplementationType.IMPLEMENTATION_TYPE_DELEGATEEXPRESSION.equalsIgnoreCase(activitiListener.ImplementationType))
		{
		  executionListener = bpmnParse.ListenerFactory.createDelegateExpressionExecutionListener(activitiListener);
		}
		return executionListener;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void addEventSubscriptionDeclaration(org.activiti.engine.impl.bpmn.parser.BpmnParse bpmnParse, org.activiti.engine.impl.bpmn.parser.EventSubscriptionDeclaration subscription, org.activiti.bpmn.model.EventDefinition parsedEventDefinition, org.activiti.engine.impl.pvm.process.ScopeImpl scope)
	  protected internal virtual void addEventSubscriptionDeclaration(BpmnParse bpmnParse, EventSubscriptionDeclaration subscription, EventDefinition parsedEventDefinition, ScopeImpl scope)
	  {
		IList<EventSubscriptionDeclaration> eventDefinitions = (IList<EventSubscriptionDeclaration>) scope.getProperty(PROPERTYNAME_EVENT_SUBSCRIPTION_DECLARATION);
		if (eventDefinitions == null)
		{
		  eventDefinitions = new List<EventSubscriptionDeclaration>();
		  scope.setProperty(PROPERTYNAME_EVENT_SUBSCRIPTION_DECLARATION, eventDefinitions);
		}
		else
		{
		  // if this is a message event, validate that it is the only one with the provided name for this scope
		  if (subscription.EventType.Equals("message"))
		  {
			foreach (EventSubscriptionDeclaration eventDefinition in eventDefinitions)
			{
			  if (eventDefinition.EventType.Equals("message") && eventDefinition.EventName.Equals(subscription.EventName) && eventDefinition.StartEvent == subscription.StartEvent)
			  {

				logger.warn("Cannot have more than one message event subscription with name '" + subscription.EventName + "' for scope '" + scope.Id + "'");
			  }
			}
		  }
		}
		eventDefinitions.Add(subscription);
	  }

	  protected internal virtual string getPrecedingEventBasedGateway(BpmnParse bpmnParse, IntermediateCatchEvent @event)
	  {
		string eventBasedGatewayId = null;
		foreach (SequenceFlow sequenceFlow in @event.IncomingFlows)
		{
		  FlowElement sourceElement = bpmnParse.BpmnModel.getFlowElement(sequenceFlow.SourceRef);
		  if (sourceElement is EventGateway)
		  {
			eventBasedGatewayId = sourceElement.Id;
			break;
		  }
		}
		return eventBasedGatewayId;
	  }

	  protected internal virtual IOSpecification createIOSpecification(BpmnParse bpmnParse, org.activiti.bpmn.model.IOSpecification specificationModel)
	  {
		IOSpecification ioSpecification = new IOSpecification();

		foreach (DataSpec dataInputElement in specificationModel.DataInputs)
		{
		  ItemDefinition itemDefinition = bpmnParse.ItemDefinitions[dataInputElement.ItemSubjectRef];
		  Data dataInput = new Data(bpmnParse.TargetNamespace + ":" + dataInputElement.Id, dataInputElement.Id, itemDefinition);
		  ioSpecification.addInput(dataInput);
		}

		foreach (DataSpec dataOutputElement in specificationModel.DataOutputs)
		{
		  ItemDefinition itemDefinition = bpmnParse.ItemDefinitions[dataOutputElement.ItemSubjectRef];
		  Data dataOutput = new Data(bpmnParse.TargetNamespace + ":" + dataOutputElement.Id, dataOutputElement.Id, itemDefinition);
		  ioSpecification.addOutput(dataOutput);
		}

		foreach (string dataInputRef in specificationModel.DataInputRefs)
		{
		  DataRef dataRef = new DataRef(dataInputRef);
		  ioSpecification.addInputRef(dataRef);
		}

		foreach (string dataOutputRef in specificationModel.DataOutputRefs)
		{
		  DataRef dataRef = new DataRef(dataOutputRef);
		  ioSpecification.addOutputRef(dataRef);
		}

		return ioSpecification;
	  }

	  protected internal virtual void processArtifacts(BpmnParse bpmnParse, ICollection<Artifact> artifacts, ScopeImpl scope)
	  {
		// associations  
		foreach (Artifact artifact in artifacts)
		{
		  if (artifact is Association)
		  {
			createAssociation(bpmnParse, (Association) artifact, scope);
		  }
		}
	  }

	  protected internal virtual void createAssociation(BpmnParse bpmnParse, Association association, ScopeImpl parentScope)
	  {
		BpmnModel bpmnModel = bpmnParse.BpmnModel;
		if (bpmnModel.getArtifact(association.SourceRef) != null || bpmnModel.getArtifact(association.TargetRef) != null)
		{

		  // connected to a text annotation so skipping it
		  return;
		}

		ActivityImpl sourceActivity = parentScope.findActivity(association.SourceRef);
		ActivityImpl targetActivity = parentScope.findActivity(association.TargetRef);

		// an association may reference elements that are not parsed as activities (like for instance 
		// text annotations so do not throw an exception if sourceActivity or targetActivity are null)
		// However, we make sure they reference 'something':
		if (sourceActivity == null)
		{
		  //bpmnModel.addProblem("Invalid reference sourceRef '" + association.getSourceRef() + "' of association element ", association.getId());
		}
		else if (targetActivity == null)
		{
		  //bpmnModel.addProblem("Invalid reference targetRef '" + association.getTargetRef() + "' of association element ", association.getId());
		}
		else
		{
		  if (sourceActivity.getProperty("type").Equals("compensationBoundaryCatch"))
		  {
			object isForCompensation = targetActivity.getProperty(PROPERTYNAME_IS_FOR_COMPENSATION);
			if (isForCompensation == null || !(bool?) isForCompensation)
			{
			  logger.warn("compensation boundary catch must be connected to element with isForCompensation=true");
			}
			else
			{
			  ActivityImpl compensatedActivity = sourceActivity.ParentActivity;
			  compensatedActivity.setProperty(BpmnParse.PROPERTYNAME_COMPENSATION_HANDLER_ID, targetActivity.Id);
			}
		  }
		}
	  }

	  protected internal virtual IDictionary<string, object> processDataObjects(BpmnParse bpmnParse, ICollection<ValuedDataObject> dataObjects, ScopeImpl scope)
	  {
		IDictionary<string, object> variablesMap = new Dictionary<string, object>();
		// convert data objects to process variables  
		if (dataObjects != null)
		{
		  foreach (ValuedDataObject dataObject in dataObjects)
		  {
			variablesMap[dataObject.Name] = dataObject.Value;
		  }
		}
		return variablesMap;
	  }
	}

}