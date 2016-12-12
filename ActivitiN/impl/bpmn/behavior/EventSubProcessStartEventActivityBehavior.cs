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

	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;


	/// <summary>
	/// Specialization of the Start Event for Event Sub-Processes.
	/// 
	/// Assumes that we enter with the "right" execution, 
	/// which is the top-most execution for the current scope
	/// 
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// </summary>
	public class EventSubProcessStartEventActivityBehavior : NoneStartEventActivityBehavior
	{

	  // default = true
	  protected internal bool isInterrupting = true;
	  protected internal string activityId;

	  public EventSubProcessStartEventActivityBehavior(string activityId)
	  {
		this.activityId = activityId;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public override void execute(ActivityExecution execution)
	  {

		InterpretableExecution interpretableExecution = (InterpretableExecution) execution;
		ActivityImpl activity = interpretableExecution.ProcessDefinition.findActivity(activityId);

		ActivityExecution outgoingExecution = execution;

		if (isInterrupting)
		{
		  execution.destroyScope("Event subprocess triggered using activity " + activityId);
		}
		else
		{
		  outgoingExecution = execution.createExecution();
		  outgoingExecution.Active = true;
		  outgoingExecution.Scope = false;
		  outgoingExecution.Concurrent = true;
		}

		// set the outgoing execution to this activity
		((InterpretableExecution)outgoingExecution).Activity = activity;

		// continue execution
		outgoingExecution.takeAll(activity.setOutgoingTransitions, Collections.EMPTY_LIST);
	  }

	  public virtual bool Interrupting
	  {
		  set
		  {
			isInterrupting = value;
		  }
		  get
		  {
			return isInterrupting;
		  }
	  }


	}

}