using System;

namespace org.activiti.engine.impl.@event.logger
{

	using EventLoggerEventHandler = org.activiti.engine.impl.@event.logger.handler.EventLoggerEventHandler;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using EventLogEntryEntityManager = org.activiti.engine.impl.persistence.entity.EventLogEntryEntityManager;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class DatabaseEventFlusher : AbstractEventFlusher
	{

		private static readonly Logger logger = LoggerFactory.getLogger(typeof(DatabaseEventFlusher));

		public override void closing(CommandContext commandContext)
		{
			EventLogEntryEntityManager eventLogEntryEntityManager = commandContext.EventLogEntryEntityManager;
			foreach (EventLoggerEventHandler eventHandler in eventHandlers)
			{
				try
				{
					eventLogEntryEntityManager.insert(eventHandler.generateEventLogEntry(commandContext));
				}
				catch (Exception e)
				{
					logger.warn("Could not create event log", e);
				}
			}
		}

	}

}