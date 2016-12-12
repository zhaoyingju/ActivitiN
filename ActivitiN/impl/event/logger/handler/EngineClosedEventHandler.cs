using System.Collections.Generic;

namespace org.activiti.engine.impl.@event.logger.handler
{


	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using EventLogEntryEntity = org.activiti.engine.impl.persistence.entity.EventLogEntryEntity;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class EngineClosedEventHandler : AbstractDatabaseEventLoggerEventHandler
	{

		public override EventLogEntryEntity generateEventLogEntry(CommandContext commandContext)
		{
			IDictionary<string, object> data = new Dictionary<string, object>();
			try
			{
			data["ip"] = InetAddress.LocalHost.HostAddress; // Note that this might give the wrong ip address in case of multiple network interfaces - but it's better than nothing.
			}
		catch (UnknownHostException)
		{
			// Best effort
		}
			return createEventLogEntry(data);
		}

	}

}