using System.Collections.Generic;

namespace org.activiti.engine.impl.@event.logger.handler
{


	using ActivitiEntityWithVariablesEvent = org.activiti.engine.@delegate.@event.ActivitiEntityWithVariablesEvent;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using EventLogEntryEntity = org.activiti.engine.impl.persistence.entity.EventLogEntryEntity;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class TaskCompletedEventHandler : AbstractTaskEventHandler
	{

	  public override EventLogEntryEntity generateEventLogEntry(CommandContext commandContext)
	  {

		  ActivitiEntityWithVariablesEvent eventWithVariables = (ActivitiEntityWithVariablesEvent) @event;
			TaskEntity task = (TaskEntity) eventWithVariables.Entity;
			IDictionary<string, object> data = handleCommonTaskFields(task);

			long duration = timeStamp.Ticks - task.CreateTime.Ticks;
			putInMapIfNotNull(data, Fields_Fields.DURATION, duration);

			if (eventWithVariables.Variables != null && eventWithVariables.Variables.Count > 0)
			{
			  IDictionary<string, object> variableMap = new Dictionary<string, object>();
			  foreach (object variableName in eventWithVariables.Variables.Keys)
			  {
			putInMapIfNotNull(variableMap, (string) variableName, eventWithVariables.Variables[variableName]);
			  }
			  if (eventWithVariables.LocalScope)
			  {
				putInMapIfNotNull(data, Fields_Fields.LOCAL_VARIABLES, variableMap);
			  }
			  else
			  {
				putInMapIfNotNull(data, Fields_Fields.VARIABLES, variableMap);
			  }
			}

		return createEventLogEntry(task.ProcessDefinitionId, task.ProcessInstanceId, task.ExecutionId, task.Id, data);
	  }

	}

}