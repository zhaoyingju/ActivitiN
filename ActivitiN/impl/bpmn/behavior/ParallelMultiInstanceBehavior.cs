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


	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ParallelMultiInstanceBehavior : MultiInstanceActivityBehavior
	{

	  public ParallelMultiInstanceBehavior(ActivityImpl activity, AbstractBpmnActivityBehavior originalActivityBehavior) : base(activity, originalActivityBehavior)
	  {
	  }

	  /// <summary>
	  /// Handles the parallel case of spawning the instances.
	  /// Will create child executions accordingly for every instance needed.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void createInstances(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	   protected internal virtual void createInstances(ActivityExecution execution)
	   {
		int nrOfInstances = resolveNrOfInstances(execution);
		if (nrOfInstances < 0)
		{
		  throw new ActivitiIllegalArgumentException("Invalid number of instances: must be non-negative integer value" + ", but was " + nrOfInstances);
		}

		setLoopVariable(execution, NUMBER_OF_INSTANCES, nrOfInstances);
		setLoopVariable(execution, NUMBER_OF_COMPLETED_INSTANCES, 0);
		setLoopVariable(execution, NUMBER_OF_ACTIVE_INSTANCES, nrOfInstances);

		IList<ActivityExecution> concurrentExecutions = new List<ActivityExecution>();
		for (int loopCounter = 0; loopCounter < nrOfInstances; loopCounter++)
		{
		  ActivityExecution concurrentExecution = execution.createExecution();
		  concurrentExecution.Active = true;
		  concurrentExecution.Concurrent = true;
		  concurrentExecution.Scope = false;

		  // In case of an embedded subprocess, and extra child execution is required
		  // Otherwise, all child executions would end up under the same parent,
		  // without any differentiation to which embedded subprocess they belong
		  if (ExtraScopeNeeded)
		  {
			ActivityExecution extraScopedExecution = concurrentExecution.createExecution();
			extraScopedExecution.Active = true;
			extraScopedExecution.Concurrent = false;
			extraScopedExecution.Scope = true;
			concurrentExecution = extraScopedExecution;
		  }

		  concurrentExecutions.Add(concurrentExecution);
		  logLoopDetails(concurrentExecution, "initialized", loopCounter, 0, nrOfInstances, nrOfInstances);
		}

		// Before the activities are executed, all executions MUST be created up front
		// Do not try to merge this loop with the previous one, as it will lead to bugs,
		// due to possible child execution pruning.
		for (int loopCounter = 0; loopCounter < nrOfInstances; loopCounter++)
		{
		  ActivityExecution concurrentExecution = concurrentExecutions[loopCounter];
		  // executions can be inactive, if instances are all automatics (no-waitstate)
		  // and completionCondition has been met in the meantime
		  if (concurrentExecution.Active && !concurrentExecution.Ended && concurrentExecution.Parent.Active && !concurrentExecution.Parent.Ended)
		  {
			setLoopVariable(concurrentExecution, CollectionElementIndexVariable, loopCounter);
			executeOriginalBehavior(concurrentExecution, loopCounter);
		  }
		}

		// See ACT-1586: ExecutionQuery returns wrong results when using multi instance on a receive task
		// The parent execution must be set to false, so it wouldn't show up in the execution query
		// when using .activityId(something). Do not we cannot nullify the activityId (that would
		// have been a better solution), as it would break boundary event behavior.
		if (concurrentExecutions.Count > 0)
		{
		  ExecutionEntity executionEntity = (ExecutionEntity) execution;
		  executionEntity.Active = false;
		}
	   }

	  /// <summary>
	  /// Called when the wrapped <seealso cref="ActivityBehavior"/> calls the 
	  /// <seealso cref="AbstractBpmnActivityBehavior#leave(ActivityExecution)"/> method.
	  /// Handles the completion of one of the parallel instances
	  /// </summary>
	  public override void leave(ActivityExecution execution)
	  {
		callActivityEndListeners(execution);

		int nrOfInstances = getLoopVariable(execution, NUMBER_OF_INSTANCES);
		if (nrOfInstances == 0)
		{
			// Empty collection, just leave.
			base.leave(execution);
			return;
		}

		int loopCounter = getLoopVariable(execution, CollectionElementIndexVariable);
		int nrOfCompletedInstances = getLoopVariable(execution, NUMBER_OF_COMPLETED_INSTANCES) + 1;
		int nrOfActiveInstances = getLoopVariable(execution, NUMBER_OF_ACTIVE_INSTANCES) - 1;

		if (ExtraScopeNeeded)
		{
		  // In case an extra scope was created, it must be destroyed first before going further
		  ExecutionEntity extraScope = (ExecutionEntity) execution;
		  execution = execution.Parent;
		  extraScope.remove();
		}

		if (execution.Parent != null) // will be null in case of empty collection
		{
			setLoopVariable(execution.Parent, NUMBER_OF_COMPLETED_INSTANCES, nrOfCompletedInstances);
			setLoopVariable(execution.Parent, NUMBER_OF_ACTIVE_INSTANCES, nrOfActiveInstances);
		}
		logLoopDetails(execution, "instance completed", loopCounter, nrOfCompletedInstances, nrOfActiveInstances, nrOfInstances);

		ExecutionEntity executionEntity = (ExecutionEntity) execution;

		if (executionEntity.getParent() != null)
		{

			executionEntity.inactivate();
			executionEntity.getParent().forceUpdate();

			IList<ActivityExecution> joinedExecutions = executionEntity.findInactiveConcurrentExecutions(execution.Activity);
			if (joinedExecutions.Count >= nrOfInstances || completionConditionSatisfied(execution))
			{

			  // Removing all active child executions (ie because completionCondition is true)
			  IList<ExecutionEntity> executionsToRemove = new List<ExecutionEntity>();
			  foreach (ActivityExecution childExecution in executionEntity.getParent().Executions)
			  {
				if (childExecution.Active)
				{
				  executionsToRemove.Add((ExecutionEntity) childExecution);
				}
			  }
			  foreach (ExecutionEntity executionToRemove in executionsToRemove)
			  {
				if (LOGGER.DebugEnabled)
				{
				  LOGGER.debug("Execution {} still active, but multi-instance is completed. Removing this execution.", executionToRemove);
				}
				executionToRemove.inactivate();
				executionToRemove.deleteCascade("multi-instance completed");
			  }
			  executionEntity.takeAll(executionEntity.Activity.setOutgoingTransitions, joinedExecutions);
			}

		}
		else
		{
			base.leave(executionEntity);
		}
	  }

	}

}