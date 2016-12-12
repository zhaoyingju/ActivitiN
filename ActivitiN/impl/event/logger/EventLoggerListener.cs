namespace org.activiti.engine.impl.@event.logger
{

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public interface EventLoggerListener
	{

		void eventsAdded(EventLogger databaseEventLogger);

	}

}