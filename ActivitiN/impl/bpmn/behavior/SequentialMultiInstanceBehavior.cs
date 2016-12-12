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
namespace org.activiti.engine.impl.bpmn.behavior
{

	using BpmnError = org.activiti.engine.@delegate.BpmnError;
	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
	public class SequentialMultiInstanceBehavior : MultiInstanceActivityBehavior
	{

	  public SequentialMultiInstanceBehavior(ActivityImpl activity, AbstractBpmnActivityBehavior innerActivityBehavior) : base(activity, innerActivityBehavior)
	  {
	  }

	  /// <summary>
	  /// Handles the sequential case of spawning the instances.
	  /// Will only create one instance, since at most one instance can be active.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void createInstances(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  protected internal virtual void createInstances(ActivityExecution execution)
	  {
		int nrOfInstances = resolveNrOfInstances(execution);
		if (nrOfInstances < 0)
		{
		  throw new ActivitiIllegalArgumentException("Invalid number of instances: must be a non-negative integer value" + ", but was " + nrOfInstances);
		}

		setLoopVariable(execution, NUMBER_OF_INSTANCES, nrOfInstances);
		setLoopVariable(execution, NUMBER_OF_COMPLETED_INSTANCES, 0);
		setLoopVariable(execution, CollectionElementIndexVariable, 0);
		setLoopVariable(execution, NUMBER_OF_ACTIVE_INSTANCES, 1);
		logLoopDetails(execution, "initialized", 0, 0, 1, nrOfInstances);

		if (nrOfInstances > 0)
		{
			executeOriginalBehavior(execution, 0);
		}
	  }

	  /// <summary>
	  /// Called when the wrapped <seealso cref="ActivityBehavior"/> calls the 
	  /// <seealso cref="AbstractBpmnActivityBehavior#leave(ActivityExecution)"/> method.
	  /// Handles the completion of one instance, and executes the logic for the sequential behavior.    
	  /// </summary>
	  public override void leave(ActivityExecution execution)
	  {
		int loopCounter = getLoopVariable(execution, CollectionElementIndexVariable) + 1;
		int nrOfInstances = getLoopVariable(execution, NUMBER_OF_INSTANCES);
		int nrOfCompletedInstances = getLoopVariable(execution, NUMBER_OF_COMPLETED_INSTANCES) + 1;
		int nrOfActiveInstances = getLoopVariable(execution, NUMBER_OF_ACTIVE_INSTANCES);

		if (loopCounter != nrOfInstances && !completionConditionSatisfied(execution))
		{
		  callActivityEndListeners(execution);
		}

		setLoopVariable(execution, CollectionElementIndexVariable, loopCounter);
		setLoopVariable(execution, NUMBER_OF_COMPLETED_INSTANCES, nrOfCompletedInstances);
		logLoopDetails(execution, "instance completed", loopCounter, nrOfCompletedInstances, nrOfActiveInstances, nrOfInstances);

		if (loopCounter >= nrOfInstances || completionConditionSatisfied(execution))
		{
		  base.leave(execution);
		}
		else
		{
		  try
		  {
			executeOriginalBehavior(execution, loopCounter);
		  }
		  catch (BpmnError error)
		  {
			// re-throw business fault so that it can be caught by an Error Intermediate Event or Error Event Sub-Process in the process
			throw error;
		  }
		  catch (Exception e)
		  {
			throw new ActivitiException("Could not execute inner activity behavior of multi instance behavior", e);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public override void execute(ActivityExecution execution)
	  {
		base.execute(execution);

		if (innerActivityBehavior is SubProcessActivityBehavior)
		{
		  // ACT-1185: end-event in subprocess may have inactivated execution
		  if (!execution.Active && execution.Ended && (execution.Executions == null || execution.Executions.Count == 0))
		  {
			execution.Active = true;
		  }
		}
	  }

	}

}