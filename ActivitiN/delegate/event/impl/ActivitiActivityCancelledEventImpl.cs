namespace org.activiti.engine.@delegate.@event.impl
{


	/// <summary>
	/// An <seealso cref="org.activiti.engine.delegate.event.ActivitiActivityCancelledEvent"/> implementation.
	/// 
	/// @author martin.grofcik
	/// </summary>
	public class ActivitiActivityCancelledEventImpl : ActivitiActivityEventImpl, ActivitiActivityCancelledEvent
	{

		protected internal object cause;

		public ActivitiActivityCancelledEventImpl() : base(ActivitiEventType.ACTIVITY_CANCELLED)
		{
		}

		public virtual object Cause
		{
			set
			{
			  this.cause = value;
			}
			get
			{
			  return cause;
			}
		}


	}

}