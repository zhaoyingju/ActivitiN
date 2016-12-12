using System.Collections.Generic;

namespace org.activiti.engine.impl.@event.logger.handler
{


	using ActivitiSignalEvent = org.activiti.engine.@delegate.@event.ActivitiSignalEvent;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using EventLogEntryEntity = org.activiti.engine.impl.persistence.entity.EventLogEntryEntity;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ActivitySignaledEventHandler : AbstractDatabaseEventLoggerEventHandler
	{

		public override EventLogEntryEntity generateEventLogEntry(CommandContext commandContext)
		{
			ActivitiSignalEvent signalEvent = (ActivitiSignalEvent) @event;

			IDictionary<string, object> data = new Dictionary<string, object>();
			putInMapIfNotNull(data, Fields_Fields.ACTIVITY_ID, signalEvent.ActivityId);
			putInMapIfNotNull(data, Fields_Fields.ACTIVITY_NAME, signalEvent.ActivityName);
			putInMapIfNotNull(data, Fields_Fields.PROCESS_DEFINITION_ID, signalEvent.ProcessDefinitionId);
			putInMapIfNotNull(data, Fields_Fields.PROCESS_INSTANCE_ID, signalEvent.ProcessInstanceId);
			putInMapIfNotNull(data, Fields_Fields.EXECUTION_ID, signalEvent.ExecutionId);
			putInMapIfNotNull(data, Fields_Fields.ACTIVITY_TYPE, signalEvent.ActivityType);
			putInMapIfNotNull(data, Fields_Fields.BEHAVIOR_CLASS, signalEvent.BehaviorClass);

			putInMapIfNotNull(data, Fields_Fields.SIGNAL_NAME, signalEvent.SignalName);
			putInMapIfNotNull(data, Fields_Fields.SIGNAL_DATA, signalEvent.SignalData);

			return createEventLogEntry(signalEvent.ProcessDefinitionId, signalEvent.ProcessInstanceId, signalEvent.ExecutionId, null, data);
		}

	}

}