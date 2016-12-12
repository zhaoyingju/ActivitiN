using System;
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

namespace org.activiti.engine.impl.bpmn.helper
{


	using Context = org.activiti.engine.impl.context.Context;
	using CompensateEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.CompensateEventSubscriptionEntity;
	using EventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using PvmProcessDefinition = org.activiti.engine.impl.pvm.PvmProcessDefinition;
	using PvmScope = org.activiti.engine.impl.pvm.PvmScope;
	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Nico Rehwaldt
	/// @author Joram Barrez
	/// </summary>
	public class ScopeUtil
	{

	  /// <summary>
	  /// Find the next scope execution in the parent execution hierarchy
	  /// That method works different than <seealso cref="#findScopeExecutionForScope(org.activiti.engine.impl.persistence.entity.ExecutionEntity, org.activiti.engine.impl.pvm.PvmScope)"/> 
	  /// which returns the most outer scope execution.
	  /// </summary>
	  /// <param name="execution"> the execution from which to start the search </param>
	  /// <returns> the next scope execution in the parent execution hierarchy </returns>
	  public static ActivityExecution findScopeExecution(ActivityExecution execution)
	  {

		while (execution.ParentId != null && !execution.Scope)
		{
		  execution = execution.Parent;
		}

		if (execution != null && execution.Concurrent)
		{
		  execution = execution.Parent;
		}

		return execution;

	  }
	  /// <summary>
	  /// returns the top-most execution sitting in an activity part of the scope defined by 'scopeActivitiy'.
	  /// </summary>
	  public static ExecutionEntity findScopeExecutionForScope(ExecutionEntity execution, PvmScope scopeActivity)
	  {

		// TODO: this feels hacky!

		if (scopeActivity is PvmProcessDefinition)
		{
		  return execution.getProcessInstance();

		}
		else
		{

		  ActivityImpl currentActivity = execution.Activity;
		  ExecutionEntity candiadateExecution = null;
		  ExecutionEntity originalExecution = execution;

		  while (execution != null)
		  {
			currentActivity = execution.Activity;
			if (scopeActivity.Activities.Contains(currentActivity) || scopeActivity.Equals(currentActivity)) // does not search rec
			{
			  // found a candidate execution; lets still check whether we find an
			  // execution which is also sitting in an activity part of this scope
			  // higher up the hierarchy
			  candiadateExecution = execution;
			}
			else if (currentActivity != null && currentActivity.contains((ActivityImpl)scopeActivity)) //searches rec
			{
			  // now we're too "high", the candidate execution is the one.
			  break;
			}

			execution = execution.getParent();
		  }

		  // if activity is scope, we need to get the parent at least:
		  if (originalExecution == candiadateExecution && originalExecution.Activity.Scope && !originalExecution.Activity.Equals(scopeActivity))
		  {
			candiadateExecution = originalExecution.getParent();
		  }

		  return candiadateExecution;
		}
	  }

	  public static ActivityImpl findInParentScopesByBehaviorType(ActivityImpl activity, Type behaviorType)
	  {
		while (activity != null)
		{
		  foreach (ActivityImpl childActivity in activity.Activities)
		  {
			if (behaviorType.IsAssignableFrom(childActivity.ActivityBehavior.GetType()))
			{
			  return childActivity;
			}
		  }
		  activity = activity.ParentActivity;
		}
		return null;
	  }

	  /// <summary>
	  /// we create a separate execution for each compensation handler invocation. 
	  /// </summary>
	  public static void throwCompensationEvent(IList<CompensateEventSubscriptionEntity> eventSubscriptions, ActivityExecution execution, bool @async)
	  {

		// first spawn the compensating executions
		foreach (EventSubscriptionEntity eventSubscription in eventSubscriptions)
		{
		  ExecutionEntity compensatingExecution = null;
		  // check whether compensating execution is already created 
		  // (which is the case when compensating an embedded subprocess, 
		  // where the compensating execution is created when leaving the subprocess 
		  // and holds snapshot data).
		  if (eventSubscription.Configuration != null)
		  {
			compensatingExecution = Context.CommandContext.ExecutionEntityManager.findExecutionById(eventSubscription.Configuration);
			// move the compensating execution under this execution:
			compensatingExecution.setParent((InterpretableExecution) execution);
			compensatingExecution.EventScope = false;
		  }
		  else
		  {
			compensatingExecution = (ExecutionEntity) execution.createExecution();
			eventSubscription.Configuration = compensatingExecution.Id;
		  }
		  compensatingExecution.Concurrent = true;
		}

		// signal compensation events in reverse order of their 'created' timestamp
		eventSubscriptions.Sort(new ComparatorAnonymousInnerClassHelper());

		foreach (CompensateEventSubscriptionEntity compensateEventSubscriptionEntity in eventSubscriptions)
		{
		  compensateEventSubscriptionEntity.eventReceived(null, @async);
		}
	  }

	  private class ComparatorAnonymousInnerClassHelper : IComparer<EventSubscriptionEntity>
	  {
		  public ComparatorAnonymousInnerClassHelper()
		  {
		  }

		  public virtual int Compare(EventSubscriptionEntity o1, EventSubscriptionEntity o2)
		  {
			return o2.Created.compareTo(o1.Created);
		  }
	  }

	  /// <summary>
	  /// creates an event scope for the given execution:
	  /// 
	  /// create a new event scope execution under the parent of the given 
	  /// execution and move all event subscriptions to that execution.
	  /// 
	  /// this allows us to "remember" the event subscriptions after finishing a 
	  /// scope
	  /// </summary>
	  public static void createEventScopeExecution(ExecutionEntity execution)
	  {

		ExecutionEntity eventScope = ScopeUtil.findScopeExecutionForScope(execution, execution.Activity.Parent);

		IList<CompensateEventSubscriptionEntity> eventSubscriptions = execution.CompensateEventSubscriptions;

		if (eventSubscriptions.Count > 0)
		{

		  ExecutionEntity eventScopeExecution = eventScope.createExecution();
		  eventScopeExecution.Active = false;
		  eventScopeExecution.Concurrent = false;
		  eventScopeExecution.EventScope = true;
		  eventScopeExecution.Activity = (ActivityImpl) execution.Activity;

		  execution.Concurrent = false;

		  // copy local variables to eventScopeExecution by value. This way, 
		  // the eventScopeExecution references a 'snapshot' of the local variables      
		  IDictionary<string, object> variables = execution.VariablesLocal;
		  foreach (KeyValuePair<string, object> variable in variables)
		  {
			eventScopeExecution.setVariableLocal(variable.Key, variable.Value);
		  }

		  // set event subscriptions to the event scope execution:
		  foreach (CompensateEventSubscriptionEntity eventSubscriptionEntity in eventSubscriptions)
		  {
			eventSubscriptionEntity = eventSubscriptionEntity.moveUnder(eventScopeExecution);
		  }

		  CompensateEventSubscriptionEntity eventSubscription = CompensateEventSubscriptionEntity.createAndInsert(eventScope);
		  eventSubscription.Activity = execution.Activity;
		  eventSubscription.Configuration = eventScopeExecution.Id;

		}
	  }

	}

}