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
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.activiti.engine.impl.pvm.PvmActivity;
	using PvmTransition = org.activiti.engine.impl.pvm.PvmTransition;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// Implementation of the Inclusive Gateway/OR gateway/inclusive data-based
	/// gateway as defined in the BPMN specification.
	/// 
	/// @author Tijs Rademakers
	/// @author Tom Van Buskirk
	/// @author Joram Barrez
	/// </summary>
	public class InclusiveGatewayActivityBehavior : GatewayActivityBehavior
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static Logger log = LoggerFactory.getLogger(typeof(InclusiveGatewayActivityBehavior).FullName);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {

		execution.inactivate();
		lockConcurrentRoot(execution);

		PvmActivity activity = execution.Activity;
		if (!activeConcurrentExecutionsExist(execution))
		{

		  if (log.DebugEnabled)
		  {
			log.debug("inclusive gateway '{}' activates", activity.Id);
		  }

		  IList<ActivityExecution> joinedExecutions = execution.findInactiveConcurrentExecutions(activity);
		  string defaultSequenceFlow = (string) execution.Activity.getProperty("default");
		  IList<PvmTransition> transitionsToTake = new List<PvmTransition>();

		  foreach (PvmTransition outgoingTransition in execution.Activity.OutgoingTransitions)
		  {

			Expression skipExpression = outgoingTransition.SkipExpression;
			if (!SkipExpressionUtil.isSkipExpressionEnabled(execution, skipExpression))
			{
			  if (defaultSequenceFlow == null || !outgoingTransition.Id.Equals(defaultSequenceFlow))
			  {
				Condition condition = (Condition) outgoingTransition.getProperty(BpmnParse.PROPERTYNAME_CONDITION);
				if (condition == null || condition.evaluate(outgoingTransition.Id, execution))
				{
				  transitionsToTake.Add(outgoingTransition);
				}
			  }
			}
			else if (SkipExpressionUtil.shouldSkipFlowElement(execution, skipExpression))
			{
			  transitionsToTake.Add(outgoingTransition);
			}
		  }

		  if (transitionsToTake.Count > 0)
		  {
			execution.takeAll(transitionsToTake, joinedExecutions);

		  }
		  else
		  {

			if (defaultSequenceFlow != null)
			{
			  PvmTransition defaultTransition = execution.Activity.findOutgoingTransition(defaultSequenceFlow);
			  if (defaultTransition != null)
			  {
				transitionsToTake.Add(defaultTransition);
				execution.takeAll(transitionsToTake, joinedExecutions);
			  }
			  else
			  {
				throw new ActivitiException("Default sequence flow '" + defaultSequenceFlow + "' could not be not found");
			  }
			}
			else
			{
			  // No sequence flow could be found, not even a default one
			  throw new ActivitiException("No outgoing sequence flow of the inclusive gateway '" + execution.Activity.Id + "' could be selected for continuing the process");
			}
		  }

		}
		else
		{
		  if (log.DebugEnabled)
		  {
			log.debug("Inclusive gateway '{}' does not activate", activity.Id);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<? extends org.activiti.engine.impl.pvm.delegate.ActivityExecution> getLeaveExecutions(org.activiti.engine.impl.pvm.delegate.ActivityExecution parent)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<? extends org.activiti.engine.impl.pvm.delegate.ActivityExecution> getLeaveExecutions(org.activiti.engine.impl.pvm.delegate.ActivityExecution parent)
	  internal virtual IList<?> getLeaveExecutions(ActivityExecution parent) where ? : org.activiti.engine.impl.pvm.@delegate.ActivityExecution
	  {
		IList<ActivityExecution> executionlist = new List<ActivityExecution>();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<? extends org.activiti.engine.impl.pvm.delegate.ActivityExecution> subExecutions = parent.getExecutions();
		IList<?> subExecutions = parent.Executions;
		if (subExecutions.Count == 0)
		{
		  executionlist.Add(parent);
		}
		else
		{
		  foreach (ActivityExecution concurrentExecution in subExecutions)
		  {
			executionlist.AddRange(getLeaveExecutions(concurrentExecution));
		  }
		}
		return executionlist;
	  }

	  public virtual bool activeConcurrentExecutionsExist(ActivityExecution execution)
	  {
		PvmActivity activity = execution.Activity;
		if (execution.Concurrent)
		{
		  foreach (ActivityExecution concurrentExecution in getLeaveExecutions(execution.Parent))
		  {
			if (concurrentExecution.Active && concurrentExecution.Id.Equals(execution.Id) == false)
			{
			  // TODO: when is transitionBeingTaken cleared? Should we clear it?
			  bool reachable = false;
			  PvmTransition pvmTransition = ((ExecutionEntity) concurrentExecution).TransitionBeingTaken;
			  if (pvmTransition != null)
			  {
				reachable = isReachable(pvmTransition.Destination, activity, new HashSet<PvmActivity>());
			  }
			  else
			  {
				reachable = isReachable(concurrentExecution.Activity, activity, new HashSet<PvmActivity>());
			  }

			  if (reachable)
			  {
				if (log.DebugEnabled)
				{
				  log.debug("an active concurrent execution found: '{}'", concurrentExecution.Activity);
				}
				return true;
			  }
			}
		  }
		} // is this ever true?
		else if (execution.Active)
		{
		  if (log.DebugEnabled)
		  {
			log.debug("an active concurrent execution found: '{}'", execution.Activity);
		  }
		  return true;
		}

		return false;
	  }

	  protected internal virtual bool isReachable(PvmActivity srcActivity, PvmActivity targetActivity, Set<PvmActivity> visitedActivities)
	  {

		// if source has no outputs, it is the end of the process, and its parent process should be checked.
		if (srcActivity.OutgoingTransitions.Count == 0)
		{
		  visitedActivities.add(srcActivity);
		  if (!(srcActivity.Parent is PvmActivity))
		  {
			return false;
		  }
		  srcActivity = (PvmActivity) srcActivity.Parent;
		}

		if (srcActivity.Equals(targetActivity))
		{
		  return true;
		}

		// To avoid infinite looping, we must capture every node we visit
		// and check before going further in the graph if we have already visitied
		// the node.
		visitedActivities.add(srcActivity);

		IList<PvmTransition> transitionList = srcActivity.OutgoingTransitions;
		if (transitionList != null && transitionList.Count > 0)
		{
		  foreach (PvmTransition pvmTransition in transitionList)
		  {
			PvmActivity destinationActivity = pvmTransition.Destination;
			if (destinationActivity != null && !visitedActivities.contains(destinationActivity))
			{
			  bool reachable = isReachable(destinationActivity, targetActivity, visitedActivities);

			  // If false, we should investigate other paths, and not yet return the
			  // result
			  if (reachable)
			  {
				return true;
			  }

			}
		  }
		}
		return false;
	  }

	}

}