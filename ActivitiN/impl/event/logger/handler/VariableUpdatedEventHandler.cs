using System.Collections.Generic;

namespace org.activiti.engine.impl.@event.logger.handler
{

	using ActivitiVariableEvent = org.activiti.engine.@delegate.@event.ActivitiVariableEvent;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using EventLogEntryEntity = org.activiti.engine.impl.persistence.entity.EventLogEntryEntity;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class VariableUpdatedEventHandler : VariableEventHandler
	{

		public override EventLogEntryEntity generateEventLogEntry(CommandContext commandContext)
		{
			ActivitiVariableEvent variableEvent = (ActivitiVariableEvent) @event;
			IDictionary<string, object> data = createData(variableEvent);

		return createEventLogEntry(variableEvent.ProcessDefinitionId, variableEvent.ProcessInstanceId, variableEvent.ExecutionId, variableEvent.TaskId, data);
		}

	}

}