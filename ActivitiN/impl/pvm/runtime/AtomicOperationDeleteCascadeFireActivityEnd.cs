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

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class AtomicOperationDeleteCascadeFireActivityEnd : AbstractEventAtomicOperation
	{

	  protected internal override ScopeImpl getScope(InterpretableExecution execution)
	  {
		ActivityImpl activity = (ActivityImpl) execution.Activity;

		if (activity != null)
		{
		  return activity;
		}
		else
		{
		  InterpretableExecution parent = (InterpretableExecution) execution.Parent;
		  if (parent != null)
		  {
			return getScope((InterpretableExecution) execution.Parent);
		  }
		  return execution.ProcessDefinition;
		}
	  }

	  protected internal override string EventName
	  {
		  get
		  {
			return org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_END;
		  }
	  }

	  protected internal override void eventNotificationsCompleted(InterpretableExecution execution)
	  {
		ActivityImpl activity = (ActivityImpl) execution.Activity;
		if ((execution.Scope) && (activity != null))
		{
		  execution.Activity = activity.ParentActivity;
		  execution.performOperation(AtomicOperation_Fields.DELETE_CASCADE_FIRE_ACTIVITY_END);

		}
		else
		{
		  if (execution.Scope)
		  {
			execution.destroy();
		  }

		  execution.remove();

		  if (!execution.DeleteRoot)
		  {
			InterpretableExecution parent = (InterpretableExecution) execution.Parent;
			if (parent != null)
			{
			  parent.performOperation(AtomicOperation_Fields.DELETE_CASCADE);
			}
		  }
		}
	  }
	}

}