using System.Collections.Generic;

namespace org.activiti.engine.impl.@event.logger
{


	using EventLoggerEventHandler = org.activiti.engine.impl.@event.logger.handler.EventLoggerEventHandler;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public abstract class AbstractEventFlusher : EventFlusher
	{
		public abstract void closing(CommandContext commandContext);

		protected internal IList<EventLoggerEventHandler> eventHandlers = new List<EventLoggerEventHandler>();

		public override void closed(CommandContext commandContext)
		{
			// Not interested in closed
		}

		public virtual IList<EventLoggerEventHandler> EventHandlers
		{
			get
			{
				return eventHandlers;
			}
			set
			{
				this.eventHandlers = value;
			}
		}


		public virtual void addEventHandler(EventLoggerEventHandler databaseEventLoggerEventHandler)
		{
			eventHandlers.Add(databaseEventLoggerEventHandler);
		}

	}

}