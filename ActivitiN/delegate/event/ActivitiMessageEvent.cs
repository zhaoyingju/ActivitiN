namespace org.activiti.engine.@delegate.@event
{


	/// <summary>
	/// An <seealso cref="ActivitiEvent"/> related to an message being sent to an activity.
	/// </summary>
	public interface ActivitiMessageEvent : ActivitiActivityEvent
	{

		/// <returns> the name of the message. </returns>
		string MessageName {get;}

		/// <returns> the payload that was passed when sending the message. Returns null, if no payload was passed. </returns>
		object MessageData {get;}

	}

}