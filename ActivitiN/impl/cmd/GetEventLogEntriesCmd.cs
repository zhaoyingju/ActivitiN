using System.Collections.Generic;

namespace org.activiti.engine.impl.cmd
{

	using EventLogEntry = org.activiti.engine.@event.EventLogEntry;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class GetEventLogEntriesCmd : Command<IList<EventLogEntry>>
	{

	  protected internal string processInstanceId = null;
		protected internal long? startLogNr = null;
		protected internal long? pageSize = null;

		public GetEventLogEntriesCmd()
		{

		}

		public GetEventLogEntriesCmd(string processInstanceId)
		{
		this.processInstanceId = processInstanceId;
		}

		public GetEventLogEntriesCmd(long? startLogNr, long? pageSize)
		{
			this.startLogNr = startLogNr;
			this.pageSize = pageSize;
		}

		public override IList<EventLogEntry> execute(CommandContext commandContext)
		{
		  if (processInstanceId != null)
		  {
			return commandContext.EventLogEntryEntityManager.findEventLogEntriesByProcessInstanceId(processInstanceId);

		  }
		  else if (startLogNr != null)
		  {
			return commandContext.EventLogEntryEntityManager.findEventLogEntries(startLogNr, pageSize != null ? pageSize : -1);

		  }
		  else
		  {
			return commandContext.EventLogEntryEntityManager.findAllEventLogEntries();
		  }
		}

	}

}