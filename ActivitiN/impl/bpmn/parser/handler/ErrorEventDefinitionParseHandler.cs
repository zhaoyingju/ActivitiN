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


	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using BoundaryEvent = org.activiti.bpmn.model.BoundaryEvent;
	using ErrorEventDefinition = org.activiti.bpmn.model.ErrorEventDefinition;
	using StartEvent = org.activiti.bpmn.model.StartEvent;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
	using StringUtils = org.apache.commons.lang3.StringUtils;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ErrorEventDefinitionParseHandler : AbstractBpmnParseHandler<ErrorEventDefinition>
	{

	  public const string PROPERTYNAME_INITIAL = "initial";

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(ErrorEventDefinition);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, ErrorEventDefinition eventDefinition)
	  {

		ErrorEventDefinition modelErrorEvent = (ErrorEventDefinition) eventDefinition;
		if (bpmnParse.BpmnModel.containsErrorRef(modelErrorEvent.ErrorCode))
		{
		  string errorCode = bpmnParse.BpmnModel.Errors.get(modelErrorEvent.ErrorCode);
		  modelErrorEvent.ErrorCode = errorCode;
		}

		ScopeImpl scope = bpmnParse.CurrentScope;
		ActivityImpl activity = bpmnParse.CurrentActivity;
		if (bpmnParse.CurrentFlowElement is StartEvent)
		{

		  if (scope.getProperty(PROPERTYNAME_INITIAL) == null)
		  {
			scope.setProperty(PROPERTYNAME_INITIAL, activity);

			// the scope of the event subscription is the parent of the event
			// subprocess (subscription must be created when parent is initialized)
			ScopeImpl catchingScope = ((ActivityImpl) scope).Parent;

			createErrorStartEventDefinition(modelErrorEvent, activity, catchingScope);
		  }

		}
		else if (bpmnParse.CurrentFlowElement is BoundaryEvent)
		{

		  BoundaryEvent boundaryEvent = (BoundaryEvent) bpmnParse.CurrentFlowElement;
		  bool interrupting = true; // non-interrupting not yet supported
		  activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createBoundaryEventActivityBehavior(boundaryEvent, interrupting, activity);
		  ActivityImpl parentActivity = scope.findActivity(boundaryEvent.AttachedToRefId);
		  createBoundaryErrorEventDefinition(modelErrorEvent, interrupting, parentActivity, activity);

		}
	  }

	  protected internal virtual void createErrorStartEventDefinition(ErrorEventDefinition errorEventDefinition, ActivityImpl startEventActivity, ScopeImpl scope)
	  {
		org.activiti.engine.impl.bpmn.parser.ErrorEventDefinition definition = new org.activiti.engine.impl.bpmn.parser.ErrorEventDefinition(startEventActivity.Id);
		if (StringUtils.isNotEmpty(errorEventDefinition.ErrorCode))
		{
		  definition.ErrorCode = errorEventDefinition.ErrorCode;
		}
		definition.Precedence = 10;
		addErrorEventDefinition(definition, scope);
	  }

	  public virtual void createBoundaryErrorEventDefinition(ErrorEventDefinition errorEventDefinition, bool interrupting, ActivityImpl activity, ActivityImpl nestedErrorEventActivity)
	  {

		nestedErrorEventActivity.setProperty("type", "boundaryError");
		ScopeImpl catchingScope = nestedErrorEventActivity.Parent;
		((ActivityImpl) catchingScope).Scope = true;

		org.activiti.engine.impl.bpmn.parser.ErrorEventDefinition definition = new org.activiti.engine.impl.bpmn.parser.ErrorEventDefinition(nestedErrorEventActivity.Id);
		definition.ErrorCode = errorEventDefinition.ErrorCode;

		addErrorEventDefinition(definition, catchingScope);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void addErrorEventDefinition(org.activiti.engine.impl.bpmn.parser.ErrorEventDefinition errorEventDefinition, org.activiti.engine.impl.pvm.process.ScopeImpl catchingScope)
	  protected internal virtual void addErrorEventDefinition(org.activiti.engine.impl.bpmn.parser.ErrorEventDefinition errorEventDefinition, ScopeImpl catchingScope)
	  {
		IList<org.activiti.engine.impl.bpmn.parser.ErrorEventDefinition> errorEventDefinitions = (IList<org.activiti.engine.impl.bpmn.parser.ErrorEventDefinition>) catchingScope.getProperty(PROPERTYNAME_ERROR_EVENT_DEFINITIONS);
		if (errorEventDefinitions == null)
		{
		  errorEventDefinitions = new List<org.activiti.engine.impl.bpmn.parser.ErrorEventDefinition>();
		  catchingScope.setProperty(PROPERTYNAME_ERROR_EVENT_DEFINITIONS, errorEventDefinitions);
		}
		errorEventDefinitions.Add(errorEventDefinition);
		errorEventDefinitions.Sort(org.activiti.engine.impl.bpmn.parser.ErrorEventDefinition.comparator);
	  }

	}

}