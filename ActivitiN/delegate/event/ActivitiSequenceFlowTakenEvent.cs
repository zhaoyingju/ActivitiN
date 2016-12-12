namespace org.activiti.engine.@delegate.@event
{


	/// <summary>
	/// An <seealso cref="ActivitiEvent"/> that indicates a certain sequence flow has been taken.
	/// </summary>
	public interface ActivitiSequenceFlowTakenEvent : ActivitiEvent
	{

		string Id {get;}

		string SourceActivityId {get;}

		string SourceActivityName {get;}

		string SourceActivityType {get;}

		string SourceActivityBehaviorClass {get;}

		string TargetActivityId {get;}

		string TargetActivityName {get;}

		string TargetActivityType {get;}

		string TargetActivityBehaviorClass {get;}


	}

}