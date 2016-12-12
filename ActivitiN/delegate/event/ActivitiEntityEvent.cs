namespace org.activiti.engine.@delegate.@event
{


	/// <summary>
	/// An <seealso cref="ActivitiEvent"/> related to a single entity.
	/// </summary>
	public interface ActivitiEntityEvent : ActivitiEvent
	{

		/// <returns> the entity that is targeted by this event. </returns>
		object Entity {get;}
	}

}