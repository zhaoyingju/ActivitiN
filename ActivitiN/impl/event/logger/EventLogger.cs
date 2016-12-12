using System;
using System.Collections.Generic;

namespace org.activiti.engine.impl.@event.logger
{


	using ActivitiEntityEvent = org.activiti.engine.@delegate.@event.ActivitiEntityEvent;
	using ActivitiEvent = org.activiti.engine.@delegate.@event.ActivitiEvent;
	using ActivitiEventListener = org.activiti.engine.@delegate.@event.ActivitiEventListener;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using Context = org.activiti.engine.impl.context.Context;
	using ActivityCompensatedEventHandler = org.activiti.engine.impl.@event.logger.handler.ActivityCompensatedEventHandler;
	using ActivityCompletedEventHandler = org.activiti.engine.impl.@event.logger.handler.ActivityCompletedEventHandler;
	using ActivityErrorReceivedEventHandler = org.activiti.engine.impl.@event.logger.handler.ActivityErrorReceivedEventHandler;
	using ActivityMessageEventHandler = org.activiti.engine.impl.@event.logger.handler.ActivityMessageEventHandler;
	using ActivitySignaledEventHandler = org.activiti.engine.impl.@event.logger.handler.ActivitySignaledEventHandler;
	using ActivityStartedEventHandler = org.activiti.engine.impl.@event.logger.handler.ActivityStartedEventHandler;
	using EventLoggerEventHandler = org.activiti.engine.impl.@event.logger.handler.EventLoggerEventHandler;
	using ProcessInstanceEndedEventHandler = org.activiti.engine.impl.@event.logger.handler.ProcessInstanceEndedEventHandler;
	using ProcessInstanceStartedEventHandler = org.activiti.engine.impl.@event.logger.handler.ProcessInstanceStartedEventHandler;
	using SequenceFlowTakenEventHandler = org.activiti.engine.impl.@event.logger.handler.SequenceFlowTakenEventHandler;
	using TaskAssignedEventHandler = org.activiti.engine.impl.@event.logger.handler.TaskAssignedEventHandler;
	using TaskCompletedEventHandler = org.activiti.engine.impl.@event.logger.handler.TaskCompletedEventHandler;
	using TaskCreatedEventHandler = org.activiti.engine.impl.@event.logger.handler.TaskCreatedEventHandler;
	using VariableCreatedEventHandler = org.activiti.engine.impl.@event.logger.handler.VariableCreatedEventHandler;
	using VariableDeletedEventHandler = org.activiti.engine.impl.@event.logger.handler.VariableDeletedEventHandler;
	using VariableUpdatedEventHandler = org.activiti.engine.impl.@event.logger.handler.VariableUpdatedEventHandler;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandContextCloseListener = org.activiti.engine.impl.interceptor.CommandContextCloseListener;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using Clock = org.activiti.engine.runtime.Clock;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class EventLogger : ActivitiEventListener
	{

		private static readonly Logger logger = LoggerFactory.getLogger(typeof(EventLogger));

		private const string EVENT_FLUSHER_KEY = "eventFlusher";

		protected internal Clock clock;
		protected internal ObjectMapper objectMapper;

		// Mapping of type -> handler
		protected internal IDictionary<ActivitiEventType, Type> eventHandlers = new Dictionary<ActivitiEventType, Type>();

		// Listeners for new events
		protected internal IList<EventLoggerListener> listeners;

		public EventLogger()
		{
			initializeDefaultHandlers();
		}

		public EventLogger(Clock clock, ObjectMapper objectMapper) : this()
		{
			this.clock = clock;
			this.objectMapper = objectMapper;
		}

		protected internal virtual void initializeDefaultHandlers()
		{
		  addEventHandler(ActivitiEventType.TASK_CREATED, typeof(TaskCreatedEventHandler));
			addEventHandler(ActivitiEventType.TASK_COMPLETED, typeof(TaskCompletedEventHandler));
			addEventHandler(ActivitiEventType.TASK_ASSIGNED, typeof(TaskAssignedEventHandler));

			addEventHandler(ActivitiEventType.SEQUENCEFLOW_TAKEN, typeof(SequenceFlowTakenEventHandler));

			addEventHandler(ActivitiEventType.ACTIVITY_COMPLETED, typeof(ActivityCompletedEventHandler));
			addEventHandler(ActivitiEventType.ACTIVITY_STARTED, typeof(ActivityStartedEventHandler));
			addEventHandler(ActivitiEventType.ACTIVITY_SIGNALED, typeof(ActivitySignaledEventHandler));
			addEventHandler(ActivitiEventType.ACTIVITY_MESSAGE_RECEIVED, typeof(ActivityMessageEventHandler));
			addEventHandler(ActivitiEventType.ACTIVITY_COMPENSATE, typeof(ActivityCompensatedEventHandler));
			addEventHandler(ActivitiEventType.ACTIVITY_ERROR_RECEIVED, typeof(ActivityErrorReceivedEventHandler));

			addEventHandler(ActivitiEventType.VARIABLE_CREATED, typeof(VariableCreatedEventHandler));
			addEventHandler(ActivitiEventType.VARIABLE_DELETED, typeof(VariableDeletedEventHandler));
			addEventHandler(ActivitiEventType.VARIABLE_UPDATED, typeof(VariableUpdatedEventHandler));
		}

		public override void onEvent(ActivitiEvent @event)
		{
			EventLoggerEventHandler eventHandler = getEventHandler(@event);
			if (eventHandler != null)
			{

				// Events are flushed when command context is closed
				CommandContext currentCommandContext = Context.CommandContext;
				EventFlusher eventFlusher = (EventFlusher) currentCommandContext.getAttribute(EVENT_FLUSHER_KEY);

				if (eventHandler != null && eventFlusher == null)
				{

					eventFlusher = createEventFlusher();
					if (eventFlusher == null)
					{
						eventFlusher = new DatabaseEventFlusher(); // Default
					}
					currentCommandContext.addAttribute(EVENT_FLUSHER_KEY, eventFlusher);

					currentCommandContext.addCloseListener(eventFlusher);
					currentCommandContext.addCloseListener(new CommandContextCloseListenerAnonymousInnerClassHelper(this));
				}

				eventFlusher.addEventHandler(eventHandler);
			}
		}

		private class CommandContextCloseListenerAnonymousInnerClassHelper : CommandContextCloseListener
		{
			private readonly EventLogger outerInstance;

			public CommandContextCloseListenerAnonymousInnerClassHelper(EventLogger outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual void closing(CommandContext commandContext)
			{
			}

			public virtual void closed(CommandContext commandContext)
			{
				// For those who are interested: we can now broadcast the events were added
					if (outerInstance.listeners != null)
					{
						foreach (EventLoggerListener listener in outerInstance.listeners)
						{
							listener.eventsAdded(outerInstance);
						}
					}
			}

		}

		// Subclasses can override this if defaults are not ok
		protected internal virtual EventLoggerEventHandler getEventHandler(ActivitiEvent @event)
		{

			Type eventHandlerClass = null;
			if (@event.Type.Equals(ActivitiEventType.ENTITY_INITIALIZED))
			{
				object entity = ((ActivitiEntityEvent) @event).Entity;
				if (entity is ExecutionEntity)
				{
					ExecutionEntity executionEntity = (ExecutionEntity) entity;
					if (executionEntity.ProcessInstanceId.Equals(executionEntity.Id))
					{
						eventHandlerClass = typeof(ProcessInstanceStartedEventHandler);
					}
				}
			}
			else if (@event.Type.Equals(ActivitiEventType.ENTITY_DELETED))
			{
				object entity = ((ActivitiEntityEvent) @event).Entity;
				if (entity is ExecutionEntity)
				{
					ExecutionEntity executionEntity = (ExecutionEntity) entity;
					if (executionEntity.ProcessInstanceId.Equals(executionEntity.Id))
					{
						eventHandlerClass = typeof(ProcessInstanceEndedEventHandler);
					}
				}
			}
			else
			{
				// Default: dedicated mapper for the type
				eventHandlerClass = eventHandlers[@event.Type];
			}

			if (eventHandlerClass != null)
			{
				return instantiateEventHandler(@event, eventHandlerClass);
			}

			return null;
		}

		protected internal virtual EventLoggerEventHandler instantiateEventHandler(ActivitiEvent @event, Type eventHandlerClass)
		{
			try
			{
				EventLoggerEventHandler eventHandler = eventHandlerClass.newInstance();
				eventHandler.TimeStamp = clock.CurrentTime;
				eventHandler.Event = @event;
				eventHandler.ObjectMapper = objectMapper;
				return eventHandler;
			}
			catch (Exception)
			{
				logger.warn("Could not instantiate " + eventHandlerClass + ", this is most likely a programmatic error");
			}
			return null;
		}

		public override bool FailOnException
		{
			get
			{
				return false;
			}
		}

		public virtual void addEventHandler(ActivitiEventType eventType, Type eventHandlerClass)
		{
			eventHandlers[eventType] = eventHandlerClass;
		}

		public virtual void addEventLoggerListener(EventLoggerListener listener)
		{
			if (listeners == null)
			{
				listeners = new List<EventLoggerListener>(1);
			}
			listeners.Add(listener);
		}

		/// <summary>
		/// Subclasses that want something else than the database flusher should override this method
		/// </summary>
		protected internal virtual EventFlusher createEventFlusher()
		{
			return null;
		}

		public virtual Clock Clock
		{
			get
			{
				return clock;
			}
			set
			{
				this.clock = value;
			}
		}


		public virtual ObjectMapper ObjectMapper
		{
			get
			{
				return objectMapper;
			}
			set
			{
				this.objectMapper = value;
			}
		}


		public virtual IList<EventLoggerListener> Listeners
		{
			get
			{
				return listeners;
			}
			set
			{
				this.listeners = value;
			}
		}


	}

}