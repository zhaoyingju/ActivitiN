using System.Collections;
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


	using Context = org.activiti.engine.impl.context.Context;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using PvmTransition = org.activiti.engine.impl.pvm.PvmTransition;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class BoundaryEventActivityBehavior : FlowNodeActivityBehavior
	{

	  protected internal bool interrupting;
	  protected internal string activityId;

	  public BoundaryEventActivityBehavior()
	  {

	  }

	  public BoundaryEventActivityBehavior(bool interrupting, string activityId)
	  {
		this.interrupting = interrupting;
		this.activityId = activityId;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void execute(ActivityExecution execution)
	  {
		ExecutionEntity executionEntity = (ExecutionEntity) execution;
		ActivityImpl boundaryActivity = executionEntity.ProcessDefinition.findActivity(activityId);
		ActivityImpl interruptedActivity = executionEntity.Activity;

		IList<PvmTransition> outgoingTransitions = boundaryActivity.OutgoingTransitions;
		IList<ExecutionEntity> interruptedExecutions = null;

		if (interrupting)
		{

		  // Call activity
		  if (executionEntity.getSubProcessInstance() != null)
		  {
			executionEntity.getSubProcessInstance().deleteCascade(executionEntity.DeleteReason);
		  }
		  else
		  {
			Context.CommandContext.HistoryManager.recordActivityEnd(executionEntity);
		  }

		  executionEntity.Activity = boundaryActivity;

		  interruptedExecutions = new List<ExecutionEntity>(executionEntity.Executions);
		  foreach (ExecutionEntity interruptedExecution in interruptedExecutions)
		  {
			interruptedExecution.deleteCascade("interrupting boundary event '" + execution.Activity.Id + "' fired");
		  }

		  execution.takeAll(outgoingTransitions, (IList) interruptedExecutions);
		}
		else
		{
		  // non interrupting event, introduced with BPMN 2.0, we need to create a new execution in this case

		  // create a new execution and move it out from the timer activity
		  ExecutionEntity concurrentRoot = executionEntity.getParent().Concurrent ? executionEntity.getParent() : executionEntity;
		  ExecutionEntity outgoingExecution = concurrentRoot.createExecution();

		  outgoingExecution.Active = true;
		  outgoingExecution.Scope = false;
		  outgoingExecution.Concurrent = true;

		  outgoingExecution.takeAll(outgoingTransitions, Collections.EMPTY_LIST);
		  outgoingExecution.remove();
		  // now we have to move the execution back to the real activity
		  // since the execution stays there (non interrupting) and it was
		  // set to the boundary event before
		  executionEntity.Activity = interruptedActivity;
		}
	  }

	  public virtual bool Interrupting
	  {
		  get
		  {
			return interrupting;
		  }
		  set
		  {
			this.interrupting = value;
		  }
	  }


	}

}