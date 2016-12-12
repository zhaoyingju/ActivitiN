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


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class AtomicOperationTransitionNotifyListenerStart : AbstractEventAtomicOperation
	{

	  protected internal override ScopeImpl getScope(InterpretableExecution execution)
	  {
		return (ScopeImpl) execution.Activity;
	  }

	  protected internal override string EventName
	  {
		  get
		  {
			return org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_START;
		  }
	  }

	  protected internal override void eventNotificationsCompleted(InterpretableExecution execution)
	  {
		TransitionImpl transition = execution.Transition;
		ActivityImpl destination = null;
		if (transition == null) // this is null after async cont. -> transition is not stored in execution
		{
		  destination = (ActivityImpl) execution.Activity;
		}
		else
		{
		  destination = transition.Destination;
		}
		ActivityImpl activity = (ActivityImpl) execution.Activity;
		if (activity != destination)
		{
		  ActivityImpl nextScope = AtomicOperationTransitionNotifyListenerTake.findNextScope(activity, destination);
		  execution.Activity = nextScope;
		  execution.performOperation(AtomicOperation_Fields.TRANSITION_CREATE_SCOPE);
		}
		else
		{
		  execution.Transition = null;
		  execution.Activity = destination;
		  execution.performOperation(AtomicOperation_Fields.ACTIVITY_EXECUTE);
		}
	  }
	}

}