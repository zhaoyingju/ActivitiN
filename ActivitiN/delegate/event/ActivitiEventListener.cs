namespace org.activiti.engine.@delegate.@event
{

	/// <summary>
	/// Describes a class that listens for <seealso cref="ActivitiEvent"/>s dispatched by the engine.
	/// </summary>
	public interface ActivitiEventListener
	{

		/// <summary>
		/// Called when an event has been fired </summary>
		/// <param name="event"> the event </param>
		void onEvent(ActivitiEvent @event);

		/// <returns> whether or not the current operation should fail when this listeners execution
		/// throws an exception.  </returns>
		bool FailOnException {get;}
	}

}