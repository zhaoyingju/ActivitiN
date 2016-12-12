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

	using Activity = org.activiti.bpmn.model.Activity;
	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using BpmnModel = org.activiti.bpmn.model.BpmnModel;
	using FlowNode = org.activiti.bpmn.model.FlowNode;
	using MultiInstanceLoopCharacteristics = org.activiti.bpmn.model.MultiInstanceLoopCharacteristics;
	using AbstractBpmnActivityBehavior = org.activiti.engine.impl.bpmn.behavior.AbstractBpmnActivityBehavior;
	using MultiInstanceActivityBehavior = org.activiti.engine.impl.bpmn.behavior.MultiInstanceActivityBehavior;
	using ExpressionManager = org.activiti.engine.impl.el.ExpressionManager;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using StringUtils = org.apache.commons.lang3.StringUtils;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public abstract class AbstractActivityBpmnParseHandler<T> : AbstractFlowNodeBpmnParseHandler<T> where T : org.activiti.bpmn.model.FlowNode
	{

	  public override void parse(BpmnParse bpmnParse, BaseElement element)
	  {
		base.parse(bpmnParse, element);

		if (element is Activity && ((Activity) element).LoopCharacteristics != null)
		{
		  createMultiInstanceLoopCharacteristics(bpmnParse, (Activity) element);
		}
	  }

	  protected internal virtual void createMultiInstanceLoopCharacteristics(BpmnParse bpmnParse, Activity modelActivity)
	  {

		MultiInstanceLoopCharacteristics loopCharacteristics = modelActivity.LoopCharacteristics;

		// Activity Behavior
		MultiInstanceActivityBehavior miActivityBehavior = null;
		ActivityImpl activity = bpmnParse.CurrentScope.findActivity(modelActivity.Id);
		if (activity == null)
		{
		  throw new ActivitiException("Activity " + modelActivity.Id + " needed for multi instance cannot bv found");
		}

		if (loopCharacteristics.Sequential)
		{
		  miActivityBehavior = bpmnParse.ActivityBehaviorFactory.createSequentialMultiInstanceBehavior(activity, (AbstractBpmnActivityBehavior) activity.ActivityBehavior);
		}
		else
		{
		  miActivityBehavior = bpmnParse.ActivityBehaviorFactory.createParallelMultiInstanceBehavior(activity, (AbstractBpmnActivityBehavior) activity.ActivityBehavior);
		}

		// ActivityImpl settings
		activity.Scope = true;
		activity.setProperty("multiInstance", loopCharacteristics.Sequential ? "sequential" : "parallel");
		activity.ActivityBehavior = miActivityBehavior;

		ExpressionManager expressionManager = bpmnParse.ExpressionManager;
		BpmnModel bpmnModel = bpmnParse.BpmnModel;

		// loopcardinality
		if (StringUtils.isNotEmpty(loopCharacteristics.LoopCardinality))
		{
		  miActivityBehavior.LoopCardinalityExpression = expressionManager.createExpression(loopCharacteristics.LoopCardinality);
		}

		// completion condition
		if (StringUtils.isNotEmpty(loopCharacteristics.CompletionCondition))
		{
		  miActivityBehavior.CompletionConditionExpression = expressionManager.createExpression(loopCharacteristics.CompletionCondition);
		}

		// activiti:collection
		if (StringUtils.isNotEmpty(loopCharacteristics.InputDataItem))
		{
		  if (loopCharacteristics.InputDataItem.contains("{"))
		  {
			miActivityBehavior.CollectionExpression = expressionManager.createExpression(loopCharacteristics.InputDataItem);
		  }
		  else
		  {
			miActivityBehavior.CollectionVariable = loopCharacteristics.InputDataItem;
		  }
		}

		// activiti:elementVariable
		if (StringUtils.isNotEmpty(loopCharacteristics.ElementVariable))
		{
		  miActivityBehavior.CollectionElementVariable = loopCharacteristics.ElementVariable;
		}

		// activiti:elementIndexVariable
		if (StringUtils.isNotEmpty(loopCharacteristics.ElementIndexVariable))
		{
		  miActivityBehavior.CollectionElementIndexVariable = loopCharacteristics.ElementIndexVariable;
		}

	  }

	}

}