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
	using SequenceFlow = org.activiti.bpmn.model.SequenceFlow;
	using Expression = org.activiti.engine.@delegate.Expression;
	using ExpressionManager = org.activiti.engine.impl.el.ExpressionManager;
	using UelExpressionCondition = org.activiti.engine.impl.el.UelExpressionCondition;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.activiti.engine.impl.pvm.process.TransitionImpl;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class SequenceFlowParseHandler : AbstractBpmnParseHandler<SequenceFlow>
	{

	  public const string PROPERTYNAME_CONDITION = "condition";
	  public const string PROPERTYNAME_CONDITION_TEXT = "conditionText";

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(SequenceFlow);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, SequenceFlow sequenceFlow)
	  {

		ScopeImpl scope = bpmnParse.CurrentScope;

		ActivityImpl sourceActivity = scope.findActivity(sequenceFlow.SourceRef);
		ActivityImpl destinationActivity = scope.findActivity(sequenceFlow.TargetRef);

		Expression skipExpression;
		if (StringUtils.isNotEmpty(sequenceFlow.SkipExpression))
		{
		  ExpressionManager expressionManager = bpmnParse.ExpressionManager;
		  skipExpression = expressionManager.createExpression(sequenceFlow.SkipExpression);
		}
		else
		{
		  skipExpression = null;
		}

		TransitionImpl transition = sourceActivity.createOutgoingTransition(sequenceFlow.Id, skipExpression);
		bpmnParse.SequenceFlows[sequenceFlow.Id] = transition;
		transition.setProperty("name", sequenceFlow.Name);
		transition.setProperty("documentation", sequenceFlow.Documentation);
		transition.Destination = destinationActivity;

		if (StringUtils.isNotEmpty(sequenceFlow.ConditionExpression))
		{
		  Condition expressionCondition = new UelExpressionCondition(sequenceFlow.ConditionExpression);
		  transition.setProperty(PROPERTYNAME_CONDITION_TEXT, sequenceFlow.ConditionExpression);
		  transition.setProperty(PROPERTYNAME_CONDITION, expressionCondition);
		}

		createExecutionListenersOnTransition(bpmnParse, sequenceFlow.ExecutionListeners, transition);

	  }

	}

}