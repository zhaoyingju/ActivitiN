using System.Collections.Generic;

namespace org.activiti.engine.impl.@event.logger.handler
{


	using ActivitiEntityWithVariablesEvent = org.activiti.engine.@delegate.@event.ActivitiEntityWithVariablesEvent;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using EventLogEntryEntity = org.activiti.engine.impl.persistence.entity.EventLogEntryEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ProcessInstanceStartedEventHandler : AbstractDatabaseEventLoggerEventHandler
	{

		private const string TYPE = "PROCESSINSTANCE_START";

		public override EventLogEntryEntity generateEventLogEntry(CommandContext commandContext)
		{

		  ActivitiEntityWithVariablesEvent eventWithVariables = (ActivitiEntityWithVariablesEvent) @event;
			ExecutionEntity processInstanceEntity = (ExecutionEntity) eventWithVariables.Entity;

			IDictionary<string, object> data = new Dictionary<string, object>();
			putInMapIfNotNull(data, Fields_Fields.ID, processInstanceEntity.Id);
			putInMapIfNotNull(data, Fields_Fields.BUSINESS_KEY, processInstanceEntity.BusinessKey);
			putInMapIfNotNull(data, Fields_Fields.PROCESS_DEFINITION_ID, processInstanceEntity.ProcessDefinitionId);
			putInMapIfNotNull(data, Fields_Fields.NAME, processInstanceEntity.Name);
			putInMapIfNotNull(data, Fields_Fields.CREATE_TIME, timeStamp);

			if (eventWithVariables.Variables != null && eventWithVariables.Variables.Count > 0)
			{
			  IDictionary<string, object> variableMap = new Dictionary<string, object>();
		  foreach (object variableName in eventWithVariables.Variables.Keys)
		  {
			putInMapIfNotNull(variableMap, (string) variableName, eventWithVariables.Variables[variableName]);
		  }
		  putInMapIfNotNull(data, Fields_Fields.VARIABLES, variableMap);
			}

			return createEventLogEntry(TYPE, processInstanceEntity.ProcessDefinitionId, processInstanceEntity.Id, null, null, data);
		}

	}

}