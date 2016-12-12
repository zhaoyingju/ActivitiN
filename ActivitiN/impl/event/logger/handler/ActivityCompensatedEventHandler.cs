using System.Collections.Generic;

namespace org.activiti.engine.impl.@event.logger.handler
{


	using ActivitiActivityEvent = org.activiti.engine.@delegate.@event.ActivitiActivityEvent;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using EventLogEntryEntity = org.activiti.engine.impl.persistence.entity.EventLogEntryEntity;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ActivityCompensatedEventHandler : AbstractDatabaseEventLoggerEventHandler
	{

		public override EventLogEntryEntity generateEventLogEntry(CommandContext commandContext)
		{
			ActivitiActivityEvent activityEvent = (ActivitiActivityEvent) @event;

			IDictionary<string, object> data = new Dictionary<string, object>();
			putInMapIfNotNull(data, Fields_Fields.ACTIVITY_ID, activityEvent.ActivityId);
			putInMapIfNotNull(data, Fields_Fields.ACTIVITY_NAME, activityEvent.ActivityName);
			putInMapIfNotNull(data, Fields_Fields.PROCESS_DEFINITION_ID, activityEvent.ProcessDefinitionId);
			putInMapIfNotNull(data, Fields_Fields.PROCESS_INSTANCE_ID, activityEvent.ProcessInstanceId);
			putInMapIfNotNull(data, Fields_Fields.EXECUTION_ID, activityEvent.ExecutionId);
			putInMapIfNotNull(data, Fields_Fields.ACTIVITY_TYPE, activityEvent.ActivityType);
			putInMapIfNotNull(data, Fields_Fields.BEHAVIOR_CLASS, activityEvent.BehaviorClass);


			return createEventLogEntry(activityEvent.ProcessDefinitionId, activityEvent.ProcessInstanceId, activityEvent.ExecutionId, null, data);
		}

	}

}