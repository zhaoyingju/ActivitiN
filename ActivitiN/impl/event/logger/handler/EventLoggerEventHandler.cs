using System;

namespace org.activiti.engine.impl.@event.logger.handler
{

	using ActivitiEvent = org.activiti.engine.@delegate.@event.ActivitiEvent;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using EventLogEntryEntity = org.activiti.engine.impl.persistence.entity.EventLogEntryEntity;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public interface EventLoggerEventHandler
	{

		EventLogEntryEntity generateEventLogEntry(CommandContext commandContext);

		ActivitiEvent Event {set;}

		DateTime TimeStamp {set;}

		ObjectMapper ObjectMapper {set;}

	}

}