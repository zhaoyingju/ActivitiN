namespace org.activiti.engine.@delegate.@event
{


	/// <summary>
	/// An <seealso cref="ActivitiEvent"/> related to a signal being sent to an activity.
	/// </summary>
	public interface ActivitiSignalEvent : ActivitiActivityEvent
	{

		/// <returns> the name of the signal. Returns null, if no specific signal name has been specified
		/// when signaling. </returns>
		string SignalName {get;}

		/// <returns> the payload that was passed when signaling. Returns null, if no payload was passed. </returns>
		object SignalData {get;}

	}

}