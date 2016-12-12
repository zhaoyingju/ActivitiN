using System;

namespace org.activiti.engine.@delegate.@event
{

	/// <summary>
	/// Indicates the <seealso cref="ActivitiEvent"/> also contains information about a <seealso cref="Throwable"/>
	/// that occurred, triggering the event.
	/// </summary>
	public interface ActivitiExceptionEvent
	{

		/// <returns> the throwable that caused this event to be dispatched.  </returns>
		Exception Cause {get;}
	}

}