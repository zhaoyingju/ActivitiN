using System;
using System.Collections.Generic;

namespace org.activiti.engine.impl.@event.logger.handler
{


	using ActivitiEntityEvent = org.activiti.engine.@delegate.@event.ActivitiEntityEvent;
	using ActivitiEvent = org.activiti.engine.@delegate.@event.ActivitiEvent;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.activiti.engine.impl.context.Context;
	using Authentication = org.activiti.engine.impl.identity.Authentication;
	using org.activiti.engine.impl.persistence.deploy;
	using EventLogEntryEntity = org.activiti.engine.impl.persistence.entity.EventLogEntryEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public abstract class AbstractDatabaseEventLoggerEventHandler : EventLoggerEventHandler
	{
		public abstract EventLogEntryEntity generateEventLogEntry(org.activiti.engine.impl.interceptor.CommandContext commandContext);

		private static readonly Logger logger = LoggerFactory.getLogger(typeof(AbstractDatabaseEventLoggerEventHandler));

		protected internal ActivitiEvent @event;
		protected internal DateTime timeStamp;
		protected internal ObjectMapper objectMapper;

		public AbstractDatabaseEventLoggerEventHandler()
		{
		}

		protected internal virtual EventLogEntryEntity createEventLogEntry(IDictionary<string, object> data)
		{
			return createEventLogEntry(null, null, null, null, data);
		}

		protected internal virtual EventLogEntryEntity createEventLogEntry(string processDefinitionId, string processInstanceId, string executionId, string taskId, IDictionary<string, object> data)
		{
			return createEventLogEntry(@event.Type.name(), processDefinitionId, processInstanceId, executionId, taskId, data);
		}

		protected internal virtual EventLogEntryEntity createEventLogEntry(string type, string processDefinitionId, string processInstanceId, string executionId, string taskId, IDictionary<string, object> data)
		{

			EventLogEntryEntity eventLogEntry = new EventLogEntryEntity();
			eventLogEntry.ProcessDefinitionId = processDefinitionId;
			eventLogEntry.ProcessInstanceId = processInstanceId;
			eventLogEntry.ExecutionId = executionId;
			eventLogEntry.TaskId = taskId;
			eventLogEntry.Type = type;
			eventLogEntry.TimeStamp = timeStamp;
			putInMapIfNotNull(data, Fields_Fields.TIMESTAMP, timeStamp);

			// Current user
			string userId = Authentication.AuthenticatedUserId;
			if (userId != null)
			{
				eventLogEntry.UserId = userId;
				putInMapIfNotNull(data, "userId", userId);
			}

			// Current tenant
			if (!data.ContainsKey(Fields_Fields.TENANT_ID) && processDefinitionId != null)
			{
				DeploymentCache<ProcessDefinitionEntity> processDefinitionCache = Context.ProcessEngineConfiguration.ProcessDefinitionCache;
				if (processDefinitionCache != null)
				{
					ProcessDefinitionEntity processDefinitionEntity = processDefinitionCache.get(processDefinitionId);
					if (processDefinitionEntity != null && !ProcessEngineConfigurationImpl.NO_TENANT_ID.Equals(processDefinitionEntity.TenantId))
					{
						putInMapIfNotNull(data, Fields_Fields.TENANT_ID, processDefinitionEntity.TenantId);
					}
				}
			}

			try
			{
				eventLogEntry.Data = objectMapper.writeValueAsBytes(data);
			}
			catch (Exception e)
			{
				logger.warn("Could not serialize event data. Data will not be written to the database", e);
			}

			return eventLogEntry;

		}

		public override ActivitiEvent Event
		{
			set
			{
				this.@event = value;
			}
		}

		public override DateTime TimeStamp
		{
			set
			{
				this.timeStamp = value;
			}
		}

		public override ObjectMapper ObjectMapper
		{
			set
			{
				this.objectMapper = value;
			}
		}

		// Helper methods //////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T getEntityFromEvent()
		public virtual T getEntityFromEvent<T>()
		{
			get
			{
				return (T)((ActivitiEntityEvent) @event).Entity;
			}
		}

		public virtual void putInMapIfNotNull(IDictionary<string, object> map, string key, object value)
		{
			if (value != null)
			{
				map[key] = value;
			}
		}

	}

}