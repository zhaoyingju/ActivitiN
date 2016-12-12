using System.Collections.Generic;

namespace org.activiti.engine.impl.@event.logger
{

	using EventLoggerEventHandler = org.activiti.engine.impl.@event.logger.handler.EventLoggerEventHandler;
	using CommandContextCloseListener = org.activiti.engine.impl.interceptor.CommandContextCloseListener;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public interface EventFlusher : CommandContextCloseListener
	{

		IList<EventLoggerEventHandler> EventHandlers {get;set;}


		void addEventHandler(EventLoggerEventHandler databaseEventLoggerEventHandler);

	}

}