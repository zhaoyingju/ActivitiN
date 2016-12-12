namespace org.activiti.engine.@delegate.@event.impl
{


	/// <summary>
	/// Base class for all <seealso cref="ActivitiEvent"/> implementations, related to entities.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class ActivitiEntityEventImpl : ActivitiEventImpl, ActivitiEntityEvent
	{

		protected internal object entity;

		public ActivitiEntityEventImpl(object entity, ActivitiEventType type) : base(type)
		{
			if (entity == null)
			{
				throw new ActivitiIllegalArgumentException("Entity cannot be null.");
			}
		  this.entity = entity;
		}

		public override object Entity
		{
			get
			{
				return entity;
			}
		}
	}

}