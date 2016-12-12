using System;
using System.Collections;

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
namespace org.activiti.engine.@delegate.@event.impl
{

	using Context = org.activiti.engine.impl.context.Context;
	using ExecutionContext = org.activiti.engine.impl.context.ExecutionContext;
	using IdentityLinkEntity = org.activiti.engine.impl.persistence.entity.IdentityLinkEntity;
	using VariableType = org.activiti.engine.impl.variable.VariableType;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using Job = org.activiti.engine.runtime.Job;
	using Task = org.activiti.engine.task.Task;

	/// <summary>
	/// Builder class used to create <seealso cref="ActivitiEvent"/> implementations.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class ActivitiEventBuilder
	{

		/// <param name="type"> type of event </param>
		/// <returns> an <seealso cref="ActivitiEvent"/> that doesn't have it's execution context-fields filled,
		/// as the event is a global event, independant of any running execution. </returns>
		public static ActivitiEvent createGlobalEvent(ActivitiEventType type)
		{
			ActivitiEventImpl newEvent = new ActivitiEventImpl(type);
			return newEvent;
		}

		public static ActivitiEvent createEvent(ActivitiEventType type, string executionId, string processInstanceId, string processDefinitionId)
		{
			ActivitiEventImpl newEvent = new ActivitiEventImpl(type);
			newEvent.ExecutionId = executionId;
			newEvent.ProcessDefinitionId = processDefinitionId;
			newEvent.ProcessInstanceId = processInstanceId;
			return newEvent;
		}

		/// <param name="type"> type of event </param>
		/// <param name="entity"> the entity this event targets </param>
		/// <returns> an <seealso cref="ActivitiEntityEvent"/>. In case an <seealso cref="ExecutionContext"/> is active, the execution related
		/// event fields will be populated. If not, execution details will be reteived from the <seealso cref="Object"/> if possible. </returns>
		public static ActivitiEntityEvent createEntityEvent(ActivitiEventType type, object entity)
		{
			ActivitiEntityEventImpl newEvent = new ActivitiEntityEventImpl(entity, type);

			// In case an execution-context is active, populate the event fields related to the execution
			populateEventWithCurrentContext(newEvent);
			return newEvent;
		}

	  /// <param name="entity">
	  ///            the entity this event targets </param>
	  /// <param name="variables">
	  ///            the variables associated with this entity </param>
	  /// <returns> an <seealso cref="ActivitiEntityEvent"/>. In case an <seealso cref="ExecutionContext"/> is active, the execution related
	  ///         event fields will be populated. If not, execution details will be reteived from the <seealso cref="Object"/> if
	  ///         possible. </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static org.activiti.engine.delegate.event.ActivitiProcessStartedEvent createProcessStartedEvent(final Object entity, final java.util.Map variables, final boolean localScope)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public static ActivitiProcessStartedEvent createProcessStartedEvent(object entity, IDictionary variables, bool localScope)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ActivitiProcessStartedEventImpl newEvent = new ActivitiProcessStartedEventImpl(entity, variables, localScope);
		ActivitiProcessStartedEventImpl newEvent = new ActivitiProcessStartedEventImpl(entity, variables, localScope);

		// In case an execution-context is active, populate the event fields related to the execution
		populateEventWithCurrentContext(newEvent);
		return newEvent;
	  }

		/// <param name="type"> type of event </param>
		/// <param name="entity"> the entity this event targets </param>
		/// <param name="variables"> the variables associated with this entity </param>
		/// <returns> an <seealso cref="ActivitiEntityEvent"/>. In case an <seealso cref="ExecutionContext"/> is active, the execution related
		/// event fields will be populated. If not, execution details will be reteived from the <seealso cref="Object"/> if possible. </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static org.activiti.engine.delegate.event.ActivitiEntityWithVariablesEvent createEntityWithVariablesEvent(org.activiti.engine.delegate.event.ActivitiEventType type, Object entity, java.util.Map variables, boolean localScope)
	  public static ActivitiEntityWithVariablesEvent createEntityWithVariablesEvent(ActivitiEventType type, object entity, IDictionary variables, bool localScope)
	  {
		ActivitiEntityWithVariablesEventImpl newEvent = new ActivitiEntityWithVariablesEventImpl(entity, variables, localScope, type);

		// In case an execution-context is active, populate the event fields related to the execution
		populateEventWithCurrentContext(newEvent);
		return newEvent;
	  }

		public static ActivitiSequenceFlowTakenEvent createSequenceFlowTakenEvent(ActivitiEventType type, string sequenceFlowId, string sourceActivityId, string sourceActivityName, string sourceActivityType, string sourceActivityBehaviorClass, string targetActivityId, string targetActivityName, string targetActivityType, string targetActivityBehaviorClass)
		{
			ActivitiSequenceFlowTakenEventImpl newEvent = new ActivitiSequenceFlowTakenEventImpl(type);

			populateEventWithCurrentContext(newEvent);

			newEvent.Id = sequenceFlowId;
			newEvent.SourceActivityId = sourceActivityId;
			newEvent.SourceActivityName = sourceActivityName;
			newEvent.SourceActivityType = sourceActivityType;
			newEvent.SourceActivityBehaviorClass = sourceActivityBehaviorClass;
			newEvent.TargetActivityId = targetActivityId;
			newEvent.TargetActivityName = targetActivityName;
			newEvent.TargetActivityType = targetActivityType;
			newEvent.TargetActivityBehaviorClass = targetActivityBehaviorClass;

			return newEvent;
		}

		/// <param name="type"> type of event </param>
		/// <param name="entity"> the entity this event targets </param>
		/// <returns> an <seealso cref="ActivitiEntityEvent"/> </returns>
		public static ActivitiEntityEvent createEntityEvent(ActivitiEventType type, object entity, string executionId, string processInstanceId, string processDefinitionId)
		{
			ActivitiEntityEventImpl newEvent = new ActivitiEntityEventImpl(entity, type);

			newEvent.ExecutionId = executionId;
			newEvent.ProcessInstanceId = processInstanceId;
			newEvent.ProcessDefinitionId = processDefinitionId;
			return newEvent;
		}

		/// <param name="type"> type of event </param>
		/// <param name="entity"> the entity this event targets </param>
		/// <param name="cause"> the cause of the event </param>
		/// <returns> an <seealso cref="ActivitiEntityEvent"/> that is also instance of <seealso cref="ActivitiExceptionEvent"/>. 
		/// In case an <seealso cref="ExecutionContext"/> is active, the execution related event fields will be populated. </returns>
		public static ActivitiEntityEvent createEntityExceptionEvent(ActivitiEventType type, object entity, Exception cause)
		{
			ActivitiEntityExceptionEventImpl newEvent = new ActivitiEntityExceptionEventImpl(entity, type, cause);

			// In case an execution-context is active, populate the event fields related to the execution
			populateEventWithCurrentContext(newEvent);
			return newEvent;
		}

		/// <param name="type"> type of event </param>
		/// <param name="entity"> the entity this event targets </param>
		/// <param name="cause"> the cause of the event </param>
		/// <returns> an <seealso cref="ActivitiEntityEvent"/> that is also instance of <seealso cref="ActivitiExceptionEvent"/>.  </returns>
		public static ActivitiEntityEvent createEntityExceptionEvent(ActivitiEventType type, object entity, Exception cause, string executionId, string processInstanceId, string processDefinitionId)
		{
			ActivitiEntityExceptionEventImpl newEvent = new ActivitiEntityExceptionEventImpl(entity, type, cause);

			newEvent.ExecutionId = executionId;
			newEvent.ProcessInstanceId = processInstanceId;
			newEvent.ProcessDefinitionId = processDefinitionId;
			return newEvent;
		}

		public static ActivitiActivityEvent createActivityEvent(ActivitiEventType type, string activityId, string activityName, string executionId, string processInstanceId, string processDefinitionId, string activityType, string behaviourClass)
		{
			ActivitiActivityEventImpl newEvent = new ActivitiActivityEventImpl(type);
			newEvent.ActivityId = activityId;
			newEvent.ActivityName = activityName;
			newEvent.ExecutionId = executionId;
			newEvent.ProcessDefinitionId = processDefinitionId;
			newEvent.ProcessInstanceId = processInstanceId;
			newEvent.ActivityType = activityType;
			newEvent.BehaviorClass = behaviourClass;
			return newEvent;
		}

	  public static ActivitiActivityCancelledEvent createActivityCancelledEvent(string activityId, string activityName, string executionId, string processInstanceId, string processDefinitionId, string activityType, string behaviourClass, object cause)
	  {
		ActivitiActivityCancelledEventImpl newEvent = new ActivitiActivityCancelledEventImpl();
		newEvent.ActivityId = activityId;
		newEvent.ActivityName = activityName;
		newEvent.ExecutionId = executionId;
		newEvent.ProcessDefinitionId = processDefinitionId;
		newEvent.ProcessInstanceId = processInstanceId;
		newEvent.ActivityType = activityType;
		newEvent.BehaviorClass = behaviourClass;
		newEvent.Cause = cause;
		return newEvent;
	  }

		public static ActivitiCancelledEvent createCancelledEvent(string executionId, string processInstanceId, string processDefinitionId, object cause)
		{
			ActivitiProcessCancelledEventImpl newEvent = new ActivitiProcessCancelledEventImpl();
			newEvent.ExecutionId = executionId;
			newEvent.ProcessDefinitionId = processDefinitionId;
			newEvent.ProcessInstanceId = processInstanceId;
			newEvent.Cause = cause;
			return newEvent;
		}

		public static ActivitiSignalEvent createSignalEvent(ActivitiEventType type, string activityId, string signalName, object signalData, string executionId, string processInstanceId, string processDefinitionId)
		{
			ActivitiSignalEventImpl newEvent = new ActivitiSignalEventImpl(type);
			newEvent.ActivityId = activityId;
			newEvent.ExecutionId = executionId;
			newEvent.ProcessDefinitionId = processDefinitionId;
			newEvent.ProcessInstanceId = processInstanceId;
			newEvent.SignalName = signalName;
			newEvent.SignalData = signalData;
			return newEvent;
		}

		public static ActivitiMessageEvent createMessageEvent(ActivitiEventType type, string activityId, string messageName, object payload, string executionId, string processInstanceId, string processDefinitionId)
		{
			ActivitiMessageEventImpl newEvent = new ActivitiMessageEventImpl(type);
			newEvent.ActivityId = activityId;
			newEvent.ExecutionId = executionId;
			newEvent.ProcessDefinitionId = processDefinitionId;
			newEvent.ProcessInstanceId = processInstanceId;
			newEvent.MessageName = messageName;
			newEvent.MessageData = payload;
			return newEvent;
		}

		public static ActivitiErrorEvent createErrorEvent(ActivitiEventType type, string activityId, string errorCode, string executionId, string processInstanceId, string processDefinitionId)
		{
			ActivitiErrorEventImpl newEvent = new ActivitiErrorEventImpl(type);
			newEvent.ActivityId = activityId;
			newEvent.ExecutionId = executionId;
			newEvent.ProcessDefinitionId = processDefinitionId;
			newEvent.ProcessInstanceId = processInstanceId;
			newEvent.ErrorCode = errorCode;
			return newEvent;
		}

		public static ActivitiVariableEvent createVariableEvent(ActivitiEventType type, string variableName, object variableValue, VariableType variableType, string taskId, string executionId, string processInstanceId, string processDefinitionId)
		{
			ActivitiVariableEventImpl newEvent = new ActivitiVariableEventImpl(type);
			newEvent.VariableName = variableName;
			newEvent.VariableValue = variableValue;
			newEvent.VariableType = variableType;
			newEvent.TaskId = taskId;
			newEvent.ExecutionId = executionId;
			newEvent.ProcessDefinitionId = processDefinitionId;
			newEvent.ProcessInstanceId = processInstanceId;
			return newEvent;
		}

		public static ActivitiMembershipEvent createMembershipEvent(ActivitiEventType type, string groupId, string userId)
		{
			ActivitiMembershipEventImpl newEvent = new ActivitiMembershipEventImpl(type);
			newEvent.UserId = userId;
			newEvent.GroupId = groupId;
			return newEvent;
		}

		protected internal static void populateEventWithCurrentContext(ActivitiEventImpl @event)
		{
			bool extractedFromContext = false;
			if (Context.ExecutionContextActive)
			{
				ExecutionContext executionContext = Context.ExecutionContext;
				if (executionContext != null)
				{
					extractedFromContext = true;
					@event.ExecutionId = executionContext.Execution.Id;
					@event.ProcessInstanceId = executionContext.Execution.ProcessInstanceId;
					@event.ProcessDefinitionId = executionContext.Execution.ProcessDefinitionId;
				}
			}

			// Fallback to fetching context from the object itself
			if (!extractedFromContext)
			{
				if (@event is ActivitiEntityEvent)
				{
					object persistendObject = ((ActivitiEntityEvent) @event).Entity;
					if (persistendObject is Job)
					{
						@event.ExecutionId = ((Job) persistendObject).ExecutionId;
						@event.ProcessInstanceId = ((Job) persistendObject).ProcessInstanceId;
						@event.ProcessDefinitionId = ((Job) persistendObject).ProcessDefinitionId;
					}
					else if (persistendObject is DelegateExecution)
					{
						@event.ExecutionId = ((DelegateExecution) persistendObject).Id;
						@event.ProcessInstanceId = ((DelegateExecution) persistendObject).ProcessInstanceId;
						@event.ProcessDefinitionId = ((DelegateExecution) persistendObject).ProcessDefinitionId;
					}
					else if (persistendObject is IdentityLinkEntity)
					{
						IdentityLinkEntity idLink = (IdentityLinkEntity) persistendObject;
						if (idLink.ProcessDefinitionId != null)
						{
							@event.ProcessDefinitionId = idLink.ProcessDefId;
						}
						else if (idLink.getProcessInstance() != null)
						{
							@event.ProcessDefinitionId = idLink.getProcessInstance().ProcessDefinitionId;
							@event.ProcessInstanceId = idLink.ProcessInstanceId;
							@event.ExecutionId = idLink.ProcessInstanceId;
						}
						else if (idLink.getTask() != null)
						{
							@event.ProcessDefinitionId = idLink.getTask().ProcessDefinitionId;
							@event.ProcessInstanceId = idLink.getTask().ProcessInstanceId;
							@event.ExecutionId = idLink.getTask().ExecutionId;
						}
					}
					else if (persistendObject is Task)
					{
						@event.ProcessInstanceId = ((Task)persistendObject).ProcessInstanceId;
						@event.ExecutionId = ((Task)persistendObject).ExecutionId;
						@event.ProcessDefinitionId = ((Task)persistendObject).ProcessDefinitionId;
					}
					else if (persistendObject is ProcessDefinition)
					{
						@event.ProcessDefinitionId = ((ProcessDefinition) persistendObject).Id;
					}
				}
			}
		}
	}

}