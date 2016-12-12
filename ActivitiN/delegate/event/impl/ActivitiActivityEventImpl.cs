namespace org.activiti.engine.@delegate.@event.impl
{


	/// <summary>
	/// Implementation of an <seealso cref="ActivitiActivityEvent"/>.
	/// 
	/// @author Frederik Heremans
	/// @author Joram Barrez
	/// </summary>
	public class ActivitiActivityEventImpl : ActivitiEventImpl, ActivitiActivityEvent
	{

		protected internal string activityId;
		protected internal string activityName;
		protected internal string activityType;
		protected internal string behaviorClass;

		public ActivitiActivityEventImpl(ActivitiEventType type) : base(type)
		{
		}

		public override string ActivityId
		{
			get
			{
				return activityId;
			}
			set
			{
			  this.activityId = value;
			}
		}


		public virtual string ActivityName
		{
			get
			{
				return activityName;
			}
			set
			{
				this.activityName = value;
			}
		}


		public override string ActivityType
		{
			get
			{
				return activityType;
			}
			set
			{
				this.activityType = value;
			}
		}


		public override string BehaviorClass
		{
			get
			{
				return behaviorClass;
			}
			set
			{
				this.behaviorClass = value;
			}
		}


	}

}