namespace org.activiti.engine.@delegate.@event.impl
{


	/// <summary>
	/// Implementation of an <seealso cref="ActivitiErrorEvent"/>.
	/// @author Frederik Heremans
	/// </summary>
	public class ActivitiErrorEventImpl : ActivitiActivityEventImpl, ActivitiErrorEvent
	{

		protected internal string errorCode;

		public ActivitiErrorEventImpl(ActivitiEventType type) : base(type)
		{
		}

		public virtual string ErrorCode
		{
			set
			{
			  this.errorCode = value;
			}
			get
			{
			  return errorCode;
			}
		}


	}

}