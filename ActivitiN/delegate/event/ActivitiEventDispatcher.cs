namespace org.activiti.engine.@delegate.@event
{

	/// <summary>
	/// Dispatcher which allows for adding and removing <seealso cref="ActivitiEventListener"/>s to the Activiti Engine as well
	/// as dispatching <seealso cref="ActivitiEvent"/> to all the listeners registered.
	/// </summary>
	public interface ActivitiEventDispatcher
	{

		/// <summary>
		/// Adds an event-listener which will be notified of ALL events by the dispatcher. </summary>
		/// <param name="listenerToAdd"> the listener to add </param>
		void addEventListener(ActivitiEventListener listenerToAdd);

		/// <summary>
		/// Adds an event-listener which will only be notified when an event of the given types occurs. </summary>
		/// <param name="listenerToAdd"> the listener to add </param>
		/// <param name="types"> types of events the listener should be notified for </param>
		void addEventListener(ActivitiEventListener listenerToAdd, params ActivitiEventType[] types);

		/// <summary>
		/// Removes the given listener from this dispatcher. The listener will no longer be notified,
		/// regardless of the type(s) it was registered for in the first place. </summary>
		/// <param name="listenerToRemove"> listener to remove </param>
		 void removeEventListener(ActivitiEventListener listenerToRemove);

		/// <summary>
		/// Dispatches the given event to any listeners that are registered. </summary>
		/// <param name="event"> event to dispatch. </param>
		 void dispatchEvent(ActivitiEvent @event);

		 /// <param name="enabled"> true, if event dispatching should be enabled. </param>
		 bool Enabled {set;get;}

	}

}