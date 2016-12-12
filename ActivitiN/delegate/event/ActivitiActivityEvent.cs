namespace org.activiti.engine.@delegate.@event
{


	/// <summary>
	/// An <seealso cref="ActivitiEvent"/> related to an activity within an execution;
	/// </summary>
	public interface ActivitiActivityEvent : ActivitiEvent
	{

		/// <returns> the id of the activity this event is related to. This corresponds to an 
		/// id defined in the process definition. </returns>
		string ActivityId {get;}

		/// <returns> the name of the activity this event is related to. </returns>
		string ActivityName {get;}

		/// <returns> the type of the activity (if set during parsing). </returns>
		string ActivityType {get;}

		/// <returns> the behaviourclass of the activity (if it could be determined) </returns>
		string BehaviorClass {get;}

	}

}