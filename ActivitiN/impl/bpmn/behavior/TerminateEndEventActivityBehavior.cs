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


	using EndEvent = org.activiti.bpmn.model.EndEvent;
	using EventDefinition = org.activiti.bpmn.model.EventDefinition;
	using TerminateEventDefinition = org.activiti.bpmn.model.TerminateEventDefinition;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using HistoricActivityInstance = org.activiti.engine.history.HistoricActivityInstance;
	using ScopeUtil = org.activiti.engine.impl.bpmn.helper.ScopeUtil;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.activiti.engine.impl.context.Context;
	using HistoryLevel = org.activiti.engine.impl.history.HistoryLevel;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using HistoricActivityInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricActivityInstanceEntity;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;

	/// <summary>
	/// @author Martin Grofcik
	/// @author Tijs Rademakers
	/// @author Joram Barrez
	/// </summary>
	public class TerminateEndEventActivityBehavior : FlowNodeActivityBehavior
	{

	  private const long serialVersionUID = 1L;

	  protected internal EndEvent endEvent;
	  protected internal bool terminateAll;

	  public TerminateEndEventActivityBehavior(EndEvent endEvent)
	  {
		this.endEvent = endEvent.clone();

		// Terminate all attribute
		if (endEvent.EventDefinitions != null)
		{
			foreach (EventDefinition eventDefinition in endEvent.EventDefinitions)
			{
				if (eventDefinition is TerminateEventDefinition)
				{
					TerminateEventDefinition terminateEventDefinition = (TerminateEventDefinition) eventDefinition;
					if (terminateEventDefinition.TerminateAll)
					{
						this.terminateAll = true;
						break;
					}
				}
			}
		}

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		ActivityImpl terminateEndEventActivity = (ActivityImpl) execution.Activity;

		if (terminateAll)
		{
			ActivityExecution processInstanceExecution = findRootProcessInstanceExecution((ExecutionEntity) execution);
			terminateProcessInstanceExecution(execution, terminateEndEventActivity, processInstanceExecution);
		}
		else
		{
			ActivityExecution scopeExecution = ScopeUtil.findScopeExecution(execution);
			if (scopeExecution != null)
			{
				terminateExecution(execution, terminateEndEventActivity, scopeExecution);
			}
		}

	  }

	  /// <summary>
	  /// Finds the parent execution that is a process instance.
	  /// For a callactivity, this will be the process instance representing the called process instance
	  /// and NOT the root process instance! 
	  /// </summary>
	  protected internal virtual ActivityExecution findProcessInstanceExecution(ActivityExecution execution)
	  {
		  ActivityExecution currentExecution = execution;
		  while (currentExecution.Parent != null)
		  {
			  currentExecution = currentExecution.Parent;
		  }
		  return currentExecution;
	  }


	  protected internal virtual ActivityExecution findRootProcessInstanceExecution(ExecutionEntity execution)
	  {
		ExecutionEntity currentExecution = execution;
		while (currentExecution.ParentId != null || currentExecution.SuperExecutionId != null)
		{
		  ExecutionEntity parentExecution = currentExecution.getParent();
		  if (parentExecution != null)
		  {
			currentExecution = parentExecution;
		  }
		  else if (currentExecution.SuperExecutionId != null)
		  {
			currentExecution = currentExecution.getSuperExecution();
		  }
		}
		return currentExecution;
	  }

	  protected internal virtual void terminateExecution(ActivityExecution execution, ActivityImpl terminateEndEventActivity, ActivityExecution scopeExecution)
	  {
		// send cancelled event
		sendCancelledEvent(execution, terminateEndEventActivity, scopeExecution);

		// destroy the scope
		scopeExecution.destroyScope("terminate end event fired");

		// set the scope execution to the terminate end event and make it end here.
		// (the history should reflect that the execution ended here and we want an 'end time' for the
		// historic activity instance.)
		((InterpretableExecution)scopeExecution).Activity = terminateEndEventActivity;
		// end the scope execution
		scopeExecution.end();

		// Scope execution can already have been ended (for example when multiple seq flow arrive in the same terminate end event)
		// in that case, we need to make sure the activity instance is ended
		if (scopeExecution.Ended)
		{
		  Context.CommandContext.HistoryManager.recordActivityEnd((ExecutionEntity) execution);
		}

	  }

	  protected internal virtual void terminateProcessInstanceExecution(ActivityExecution execution, ActivityImpl terminateEndEventActivity, ActivityExecution processInstanceExecution)
	  {
		sendCancelledEvent(execution, terminateEndEventActivity, processInstanceExecution);
		deleteProcessInstance((ExecutionEntity) processInstanceExecution, execution, "terminate end event (" + terminateEndEventActivity.Id + ")");
	  }

	  protected internal virtual void deleteProcessInstance(ExecutionEntity processInstanceExecution, ActivityExecution execution, string deleteReason)
	  {

		IList<ExecutionEntity> orderedExecutions = orderExecutionsRootToLeaf(processInstanceExecution);
		orderedExecutions.Reverse();

		endAllHistoricActivities(processInstanceExecution.Id);

		foreach (ExecutionEntity executionToDelete in orderedExecutions)
		{

			executionToDelete.DeleteReason = deleteReason;
			executionToDelete.Ended = true;
			executionToDelete.Active = false;
			executionToDelete.DeleteRoot = true;

			executionToDelete.remove();
		}

		Context.CommandContext.HistoryManager.recordProcessInstanceEnd(processInstanceExecution.Id, deleteReason, execution.Activity.Id);
	  }

	  protected internal virtual IList<ExecutionEntity> orderExecutionsRootToLeaf(ExecutionEntity execution)
	  {

		  // Find root process instance
		  ExecutionEntity rootExecution = execution;
		  while (rootExecution.getParent() != null || rootExecution.getSuperExecution() != null)
		  {
			  rootExecution = rootExecution.getParent() != null ? rootExecution.getParent() : rootExecution.getSuperExecution();
		  }

		  return orderExecutionsRootToLeaf(rootExecution, new List<ExecutionEntity>());
	  }

	  protected internal virtual IList<ExecutionEntity> orderExecutionsRootToLeaf(ExecutionEntity rootExecution, IList<ExecutionEntity> orderedExecutions)
	  {
		orderedExecutions.Add(rootExecution);


		// Children
		if (rootExecution.Executions != null && rootExecution.Executions.Count > 0)
		{
			foreach (ExecutionEntity childExecution in rootExecution.Executions)
			{
				orderExecutionsRootToLeaf(childExecution, orderedExecutions);
			}
		}

		// Called process instances (subprocess)
		if (rootExecution.getSubProcessInstance() != null)
		{
			orderExecutionsRootToLeaf(rootExecution.getSubProcessInstance(), orderedExecutions);
		}

		return orderedExecutions;
	  }

	  protected internal virtual void endAllHistoricActivities(string processInstanceId)
	  {

		if (!Context.ProcessEngineConfiguration.HistoryLevel.isAtLeast(HistoryLevel.ACTIVITY))
		{
		  return;
		}

		IDictionary<string, HistoricActivityInstanceEntity> historicActivityInstancMap = new Dictionary<string, HistoricActivityInstanceEntity>();

		IList<HistoricActivityInstance> historicActivityInstances = (new HistoricActivityInstanceQueryImpl(Context.CommandContext)).processInstanceId(processInstanceId).unfinished().list();
		foreach (HistoricActivityInstance historicActivityInstance in historicActivityInstances)
		{
		  historicActivityInstancMap[historicActivityInstance.Id] = (HistoricActivityInstanceEntity) historicActivityInstance;
		}

		// Cached version overwites entity
		IList<HistoricActivityInstanceEntity> cachedHistoricActivityInstances = Context.CommandContext.DbSqlSession.findInCache(typeof(HistoricActivityInstanceEntity));
		foreach (HistoricActivityInstanceEntity cachedHistoricActivityInstance in cachedHistoricActivityInstances)
		{
		  if (processInstanceId.Equals(cachedHistoricActivityInstance.ProcessInstanceId) && (cachedHistoricActivityInstance.EndTime == null))
		  {
			historicActivityInstancMap[cachedHistoricActivityInstance.Id] = cachedHistoricActivityInstance;
		  }
		}

		foreach (HistoricActivityInstanceEntity historicActivityInstance in historicActivityInstancMap.Values)
		{
		  historicActivityInstance.markEnded(null);

		  // Fire event
		  ProcessEngineConfigurationImpl config = Context.ProcessEngineConfiguration;
		  if (config != null && config.EventDispatcher.Enabled)
		  {
			config.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.HISTORIC_ACTIVITY_INSTANCE_ENDED, historicActivityInstance));
		  }
		}

	  }

	  protected internal virtual void sendCancelledEvent(ActivityExecution execution, ActivityImpl terminateEndEventActivity, ActivityExecution scopeExecution)
	  {
		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
		  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createCancelledEvent(execution.Id, execution.ProcessInstanceId, execution.ProcessDefinitionId, terminateEndEventActivity));
		}
		dispatchExecutionCancelled(scopeExecution, terminateEndEventActivity);
	  }

	  private void dispatchExecutionCancelled(ActivityExecution execution, ActivityImpl causeActivity)
	  {
		// subprocesses
		foreach (ActivityExecution subExecution in execution.Executions)
		{
		  dispatchExecutionCancelled(subExecution, causeActivity);
		}

		// call activities
		ExecutionEntity subProcessInstance = Context.CommandContext.ExecutionEntityManager.findSubProcessInstanceBySuperExecutionId(execution.Id);
		if (subProcessInstance != null)
		{
		  dispatchExecutionCancelled(subProcessInstance, causeActivity);
		}

		// activity with message/signal boundary events
		ActivityImpl activity = (ActivityImpl) execution.Activity;
		if (activity != null && activity.ActivityBehavior != null && activity != causeActivity)
		{
		  dispatchActivityCancelled(execution, activity, causeActivity);
		}
	  }

	  protected internal virtual void dispatchActivityCancelled(ActivityExecution execution, ActivityImpl activity, ActivityImpl causeActivity)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createActivityCancelledEvent(activity.Id, (string) activity.Properties["name"], execution.Id, execution.ProcessInstanceId, execution.ProcessDefinitionId, (string) activity.Properties["type"], activity.ActivityBehavior.GetType().FullName, causeActivity));
	  }

	  public virtual EndEvent EndEvent
	  {
		  get
		  {
			return this.endEvent;
		  }
	  }

	}
}