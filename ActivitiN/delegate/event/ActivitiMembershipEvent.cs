namespace org.activiti.engine.@delegate.@event
{

	/// <summary>
	/// An event related to group memberships.
	/// </summary>
	public interface ActivitiMembershipEvent : ActivitiEvent
	{

		/// <returns> related user. Returns null, if not related to a sigle user but rather to all
		/// members of the group. </returns>
		string UserId {get;}

		/// <returns> related group </returns>
		string GroupId {get;}
	}

}