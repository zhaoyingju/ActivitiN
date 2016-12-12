using System.Collections.Generic;

namespace org.activiti.engine.impl.@event.logger.handler
{

	using ActivitiEntityEvent = org.activiti.engine.@delegate.@event.ActivitiEntityEvent;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using EventLogEntryEntity = org.activiti.engine.impl.persistence.entity.EventLogEntryEntity;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class TaskAssignedEventHandler : AbstractTaskEventHandler
	{

		public override EventLogEntryEntity generateEventLogEntry(CommandContext commandContext)
		{
			TaskEntity task = (TaskEntity)((ActivitiEntityEvent) @event).Entity;
			IDictionary<string, object> data = handleCommonTaskFields(task);
		return createEventLogEntry(task.ProcessDefinitionId, task.ProcessInstanceId, task.ExecutionId, task.Id, data);
		}

	}

}