namespace org.activiti.engine.@delegate.@event
{


	/// <summary>
	/// An <seealso cref="ActivitiEvent"/> related to an error being sent to an activity.
	/// </summary>
	public interface ActivitiErrorEvent : ActivitiActivityEvent
	{

		/// <returns> the error-code of the error. Returns null, if no specific error-code has been specified
		/// when the error was thrown. </returns>
		string ErrorCode {get;}
	}

}