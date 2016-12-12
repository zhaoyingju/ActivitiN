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
namespace org.activiti.engine.impl.bpmn.behavior
{

	using Expression = org.activiti.engine.@delegate.Expression;
	using SkipExpressionUtil = org.activiti.engine.impl.bpmn.helper.SkipExpressionUtil;
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using PvmTransition = org.activiti.engine.impl.pvm.PvmTransition;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// implementation of the Exclusive Gateway/XOR gateway/exclusive data-based gateway
	/// as defined in the BPMN specification.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class ExclusiveGatewayActivityBehavior : GatewayActivityBehavior
	{

	  private const long serialVersionUID = 1L;

	  private static Logger log = LoggerFactory.getLogger(typeof(ExclusiveGatewayActivityBehavior));

	  /// <summary>
	  /// The default behaviour of BPMN, taking every outgoing sequence flow
	  /// (where the condition evaluates to true), is not valid for an exclusive
	  /// gateway. 
	  /// 
	  /// Hence, this behaviour is overriden and replaced by the correct behavior:
	  /// selecting the first sequence flow which condition evaluates to true
	  /// (or which hasn't got a condition) and leaving the activity through that
	  /// sequence flow. 
	  /// 
	  /// If no sequence flow is selected (ie all conditions evaluate to false),
	  /// then the default sequence flow is taken (if defined).
	  /// </summary>
	  protected internal override void leave(ActivityExecution execution)
	  {

		if (log.DebugEnabled)
		{
		  log.debug("Leaving activity '{}'", execution.Activity.Id);
		}

		PvmTransition outgoingSeqFlow = null;
		string defaultSequenceFlow = (string) execution.Activity.getProperty("default");
		IEnumerator<PvmTransition> transitionIterator = execution.Activity.OutgoingTransitions.GetEnumerator();
		while (outgoingSeqFlow == null && transitionIterator.MoveNext())
		{
		  PvmTransition seqFlow = transitionIterator.Current;
		  Expression skipExpression = seqFlow.SkipExpression;

		  if (!SkipExpressionUtil.isSkipExpressionEnabled(execution, skipExpression))
		  {
			Condition condition = (Condition) seqFlow.getProperty(BpmnParse.PROPERTYNAME_CONDITION);
			if ((condition == null && (defaultSequenceFlow == null || !defaultSequenceFlow.Equals(seqFlow.Id))) || (condition != null && condition.evaluate(seqFlow.Id, execution)))
			{
			  if (log.DebugEnabled)
			  {
				log.debug("Sequence flow '{}'selected as outgoing sequence flow.", seqFlow.Id);
			  }
			  outgoingSeqFlow = seqFlow;
			}
		  }
		  else if (SkipExpressionUtil.shouldSkipFlowElement(execution, skipExpression))
		  {
			outgoingSeqFlow = seqFlow;
		  }
		}

		if (outgoingSeqFlow != null)
		{
		  execution.take(outgoingSeqFlow);
		}
		else
		{

		  if (defaultSequenceFlow != null)
		  {
			PvmTransition defaultTransition = execution.Activity.findOutgoingTransition(defaultSequenceFlow);
			if (defaultTransition != null)
			{
			  execution.take(defaultTransition);
			}
			else
			{
			  throw new ActivitiException("Default sequence flow '" + defaultSequenceFlow + "' not found");
			}
		  }
		  else
		  {
			//No sequence flow could be found, not even a default one
			throw new ActivitiException("No outgoing sequence flow of the exclusive gateway '" + execution.Activity.Id + "' could be selected for continuing the process");
		  }
		}
	  }

	}

}