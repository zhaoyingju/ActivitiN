namespace org.activiti.engine.impl.cmd
{

	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class DeleteEventLogEntry : Command<Void>
	{

		protected internal long logNr;

		public DeleteEventLogEntry(long logNr)
		{
			this.logNr = logNr;
		}

		public override Void execute(CommandContext commandContext)
		{
			commandContext.EventLogEntryEntityManager.deleteEventLogEntry(logNr);
			return null;
		}

	}

}