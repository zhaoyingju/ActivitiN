using System;

namespace org.activiti.engine.@delegate.@event
{

	/// <summary>
	/// Base event listener that can be used when implementing an
	/// <seealso cref="ActivitiEventListener"/> to get notified when an entity is created,
	/// updated, deleted or if another entity-related event occurs.
	/// 
	/// Override the <code>onXX(..)</code> methods to respond to entity changes
	/// accordingly.
	/// 
	/// </summary>
	public class BaseEntityEventListener : ActivitiEventListener
	{

		protected internal bool failOnException = false;
		protected internal Type entityClass;

		/// <summary>
		/// Create a new BaseEntityEventListener, notified when an event that targets
		/// any type of entity is received. Returning true when
		/// <seealso cref="#isFailOnException()"/> is called.
		/// </summary>
		public BaseEntityEventListener() : this(true, null)
		{
		}

		/// <summary>
		/// Create a new BaseEntityEventListener.
		/// </summary>
		/// <param name="failOnException">
		///          return value for <seealso cref="#isFailOnException()"/>. </param>
		public BaseEntityEventListener(bool failOnException) : this(failOnException, null)
		{
		}

		public BaseEntityEventListener(bool failOnException, Type entityClass)
		{
			this.failOnException = failOnException;
			this.entityClass = entityClass;
		}

		public override void onEvent(ActivitiEvent @event)
		{
			if (isValidEvent(@event))
			{
				// Check if this event
				if (@event.Type == ActivitiEventType.ENTITY_CREATED)
				{
					onCreate(@event);
				}
				else if (@event.Type == ActivitiEventType.ENTITY_INITIALIZED)
				{
					onInitialized(@event);
				}
				else if (@event.Type == ActivitiEventType.ENTITY_DELETED)
				{
					onDelete(@event);
				}
				else if (@event.Type == ActivitiEventType.ENTITY_UPDATED)
				{
					onUpdate(@event);
				}
				else
				{
					// Entity-specific event
					onEntityEvent(@event);
				}
			}
		}

		public override bool FailOnException
		{
			get
			{
				return failOnException;
			}
		}

		/// <returns> true, if the event is an <seealso cref="ActivitiEntityEvent"/> and (if needed) the entityClass
		/// set in this instance, is assignable from the entity class in the event.  </returns>
		protected internal virtual bool isValidEvent(ActivitiEvent @event)
		{
			bool valid = false;
			if (@event is ActivitiEntityEvent)
			{
				if (entityClass == null)
				{
					valid = true;
				}
				else
				{
					valid = entityClass.IsAssignableFrom(((ActivitiEntityEvent) @event).Entity.GetType());
				}
			}
			return valid;
		}
		/// <summary>
		/// Called when an entity create event is received.
		/// </summary>
		protected internal virtual void onCreate(ActivitiEvent @event)
		{
			// Default implementation is a NO-OP
		}

		/// <summary>
		/// Called when an entity initialized event is received.
		/// </summary>
		protected internal virtual void onInitialized(ActivitiEvent @event)
		{
			// Default implementation is a NO-OP
		}

		/// <summary>
		/// Called when an entity delete event is received.
		/// </summary>
		protected internal virtual void onDelete(ActivitiEvent @event)
		{
			// Default implementation is a NO-OP
		}

		/// <summary>
		/// Called when an entity update event is received.
		/// </summary>
		protected internal virtual void onUpdate(ActivitiEvent @event)
		{
			// Default implementation is a NO-OP
		}

		/// <summary>
		/// Called when an event is received, which is not a create, an update or
		/// delete.
		/// </summary>
		protected internal virtual void onEntityEvent(ActivitiEvent @event)
		{
			// Default implementation is a NO-OP
		}
	}

}