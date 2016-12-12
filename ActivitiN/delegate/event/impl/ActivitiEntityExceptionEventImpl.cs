using System;

namespace org.activiti.engine.@delegate.@event.impl
{


	/// <summary>
	/// Base class for all <seealso cref="ActivitiEvent"/> implementations, represents an exception occured, related 
	/// to an entity.
	/// </summary>
	public class ActivitiEntityExceptionEventImpl : ActivitiEventImpl, ActivitiEntityEvent, ActivitiExceptionEvent
	{

		protected internal object entity;
		protected internal Exception cause;

		public ActivitiEntityExceptionEventImpl(object entity, ActivitiEventType type, Exception cause) : base(type)
		{
			if (entity == null)
			{
				throw new ActivitiIllegalArgumentException("Entity cannot be null.");
			}
		  this.entity = entity;
		  this.cause = cause;
		}

		public override object Entity
		{
			get
			{
				return entity;
			}
		}

		public override Exception Cause
		{
			get
			{
			  return cause;
			}
		}
	}

}