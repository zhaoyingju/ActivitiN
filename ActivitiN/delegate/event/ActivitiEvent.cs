namespace org.activiti.engine.@delegate.@event
{

	/// <summary>
	/// Describes an event that occurred in the Activiti Engine which is dispatched to external
	/// listeners, if any.
	/// 
	/// </summary>
	public interface ActivitiEvent
	{

		/// <returns> type of event. </returns>
		ActivitiEventType Type {get;}

		/// <returns> the id of the execution this event is associated with. Returns null, if the event
		/// was not dispatched from within an active execution. </returns>
		string ExecutionId {get;}

		/// <returns> the id of the process instance this event is associated with. Returns null, if the event
		/// was not dispatched from within an active execution. </returns>
		string ProcessInstanceId {get;}

		/// <returns> the id of the process definition this event is associated with. Returns null, if the event
		/// was not dispatched from within an active execution. </returns>
		string ProcessDefinitionId {get;}

		/// <returns> the <seealso cref="EngineServices"/> associated to the engine this event
		/// originated from. Returns null, when not called from within a listener call or when no
		/// Activiti context is active. </returns>
		EngineServices EngineServices {get;}
	}

}