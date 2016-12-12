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
namespace org.activiti.engine.impl.pvm.runtime
{

	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.activiti.engine.impl.pvm.process.TransitionImpl;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class AtomicOperationTransitionDestroyScope : AtomicOperation
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(AtomicOperationTransitionDestroyScope));

	  public virtual bool isAsync(InterpretableExecution execution)
	  {
		return false;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void execute(InterpretableExecution execution)
	  public virtual void execute(InterpretableExecution execution)
	  {
		InterpretableExecution propagatingExecution = null;

		ActivityImpl activity = (ActivityImpl) execution.Activity;
		// if this transition is crossing a scope boundary
		if (activity.Scope)
		{

		  InterpretableExecution parentScopeInstance = null;
		  // if this is a concurrent execution crossing a scope boundary
		  if (execution.Concurrent && !execution.Scope)
		  {
			// first remove the execution from the current root
			InterpretableExecution concurrentRoot = (InterpretableExecution) execution.Parent;
			parentScopeInstance = (InterpretableExecution) execution.Parent.Parent;

			log.debug("moving concurrent {} one scope up under {}", execution, parentScopeInstance);
			IList<InterpretableExecution> parentScopeInstanceExecutions = (IList<InterpretableExecution>) parentScopeInstance.Executions;
			IList<InterpretableExecution> concurrentRootExecutions = (IList<InterpretableExecution>) concurrentRoot.Executions;
			// if the parent scope had only one single scope child
			if (parentScopeInstanceExecutions.Count == 1)
			{
			  // it now becomes a concurrent execution
			  parentScopeInstanceExecutions[0].Concurrent = true;
			}

			concurrentRootExecutions.Remove(execution);
			parentScopeInstanceExecutions.Add(execution);
			execution.Parent = parentScopeInstance;
			execution.Activity = activity;
			propagatingExecution = execution;

			// if there is only a single concurrent execution left
			// in the concurrent root, auto-prune it.  meaning, the 
			// last concurrent child execution data should be cloned into
			// the concurrent root.   
			if (concurrentRootExecutions.Count == 1)
			{
			  InterpretableExecution lastConcurrent = concurrentRootExecutions[0];
			  if (lastConcurrent.Scope)
			  {
				lastConcurrent.Concurrent = false;

			  }
			  else
			  {
				log.debug("merging last concurrent {} into concurrent root {}", lastConcurrent, concurrentRoot);

				// We can't just merge the data of the lastConcurrent into the concurrentRoot.
				// This is because the concurrent root might be in a takeAll-loop.  So the 
				// concurrent execution is the one that will be receiving the take
				concurrentRoot.Activity = (ActivityImpl) lastConcurrent.Activity;
				concurrentRoot.Active = lastConcurrent.Active;
				lastConcurrent.setReplacedBy(concurrentRoot);
				lastConcurrent.remove();
			  }
			}

		  }
		  else if (execution.Concurrent && execution.Scope)
		  {
			log.debug("scoped concurrent {} becomes concurrent and remains under {}", execution, execution.Parent);

			// TODO!
			execution.destroy();
			propagatingExecution = execution;

		  }
		  else
		  {
			propagatingExecution = (InterpretableExecution) execution.Parent;
			propagatingExecution.Activity = (ActivityImpl) execution.Activity;
			propagatingExecution.Transition = execution.Transition;
			propagatingExecution.Active = true;
			log.debug("destroy scope: scoped {} continues as parent scope {}", execution, propagatingExecution);
			execution.destroy();
			execution.remove();
		  }

		}
		else
		{
		  propagatingExecution = execution;
		}

		// if there is another scope element that is ended
		ScopeImpl nextOuterScopeElement = activity.Parent;
		TransitionImpl transition = propagatingExecution.Transition;
		ActivityImpl destination = transition.Destination;
		if (transitionLeavesNextOuterScope(nextOuterScopeElement, destination))
		{
		  propagatingExecution.Activity = (ActivityImpl) nextOuterScopeElement;
		  propagatingExecution.performOperation(AtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_END);
		}
		else
		{
		  propagatingExecution.performOperation(AtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_TAKE);
		}
	  }

	  public virtual bool transitionLeavesNextOuterScope(ScopeImpl nextScopeElement, ActivityImpl destination)
	  {
		return !nextScopeElement.contains(destination);
	  }
	}

}