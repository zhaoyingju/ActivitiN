using System.Collections.Generic;

namespace org.activiti.engine.impl.@event.logger.handler
{


	using ActivitiSequenceFlowTakenEvent = org.activiti.engine.@delegate.@event.ActivitiSequenceFlowTakenEvent;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using EventLogEntryEntity = org.activiti.engine.impl.persistence.entity.EventLogEntryEntity;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class SequenceFlowTakenEventHandler : AbstractDatabaseEventLoggerEventHandler
	{

		public override EventLogEntryEntity generateEventLogEntry(CommandContext commandContext)
		{
			ActivitiSequenceFlowTakenEvent sequenceFlowTakenEvent = (ActivitiSequenceFlowTakenEvent) @event;

			IDictionary<string, object> data = new Dictionary<string, object>();
			putInMapIfNotNull(data, Fields_Fields.ID, sequenceFlowTakenEvent.Id);

			putInMapIfNotNull(data, Fields_Fields.SOURCE_ACTIVITY_ID, sequenceFlowTakenEvent.SourceActivityId);
			putInMapIfNotNull(data, Fields_Fields.SOURCE_ACTIVITY_NAME, sequenceFlowTakenEvent.SourceActivityName);
			putInMapIfNotNull(data, Fields_Fields.SOURCE_ACTIVITY_TYPE, sequenceFlowTakenEvent.SourceActivityType);
			putInMapIfNotNull(data, Fields_Fields.SOURCE_ACTIVITY_BEHAVIOR_CLASS, sequenceFlowTakenEvent.SourceActivityBehaviorClass);

			putInMapIfNotNull(data, Fields_Fields.TARGET_ACTIVITY_ID, sequenceFlowTakenEvent.TargetActivityId);
			putInMapIfNotNull(data, Fields_Fields.TARGET_ACTIVITY_NAME, sequenceFlowTakenEvent.TargetActivityName);
			putInMapIfNotNull(data, Fields_Fields.TARGET_ACTIVITY_TYPE, sequenceFlowTakenEvent.TargetActivityType);
			putInMapIfNotNull(data, Fields_Fields.TARGET_ACTIVITY_BEHAVIOR_CLASS, sequenceFlowTakenEvent.TargetActivityBehaviorClass);

			return createEventLogEntry(@event.ProcessDefinitionId, @event.ProcessInstanceId, @event.ExecutionId, null, data);
		}

	}

}