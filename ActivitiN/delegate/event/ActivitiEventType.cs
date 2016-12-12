using System.Collections.Generic;

namespace org.activiti.engine.@delegate.@event
{


	using HistoricActivityInstance = org.activiti.engine.history.HistoricActivityInstance;
	using HistoricProcessInstance = org.activiti.engine.history.HistoricProcessInstance;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	/// <summary>
	/// Enumeration containing all possible types of <seealso cref="ActivitiEvent"/>s
	/// 
	/// </summary>
	public enum ActivitiEventType
	{

	  /// <summary>
	  /// New entity is created.
	  /// </summary>
	  ENTITY_CREATED,

	  /// <summary>
	  /// New entity has been created and all child-entities that are created as a result of the creation of this
	  /// particular entity are also created and initialized.
	  /// </summary>
	  ENTITY_INITIALIZED,

	  /// <summary>
	  /// Existing entity us updated.
	  /// </summary>
	  ENTITY_UPDATED,

	  /// <summary>
	  /// Existing entity is deleted.
	  /// </summary>
	  ENTITY_DELETED,

	  /// <summary>
	  /// Existing entity has been suspended.              
	  /// </summary>
	  ENTITY_SUSPENDED,

	  /// <summary>
	  /// Existing entity has been activated.              
	  /// </summary>
	  ENTITY_ACTIVATED,

	  /// <summary>
	  /// Timer has been fired successfully.
	  /// </summary>
	  TIMER_FIRED,

	  /// <summary>
	  /// Timer has been cancelled (e.g. user task on which it was bounded has been completed earlier than expected)
	  /// </summary>
	  JOB_CANCELED,

	  /// <summary>
	  /// A job has been successfully executed.
	  /// </summary>
	  JOB_EXECUTION_SUCCESS,

	  /// <summary>
	  /// A job has been executed, but failed. Event should be an instance of a <seealso cref="ActivitiExceptionEvent"/>.
	  /// </summary>
	  JOB_EXECUTION_FAILURE,

	  /// <summary>
	  /// The retry-count on a job has been decremented.
	  /// </summary>
	  JOB_RETRIES_DECREMENTED,

	  /// <summary>
	  /// An event type to be used by custom events. These types of events are never thrown by the engine itself,
	  /// only be an external API call to dispatch an event.
	  /// </summary>
	  CUSTOM,

	  /// <summary>
	  /// The process-engine that dispatched this event has been created and is ready for use.
	  /// </summary>
	  ENGINE_CREATED,

	  /// <summary>
	  /// The process-engine that dispatched this event has been closed and cannot be used anymore.
	  /// </summary>
	  ENGINE_CLOSED,

	  /// <summary>
	  /// An activity is starting to execute. This event is dispatch right before an activity is executed.
	  /// </summary>
	  ACTIVITY_STARTED,

	  /// <summary>
	  /// An activity has been completed successfully.
	  /// </summary>
	  ACTIVITY_COMPLETED,

	  /// <summary>
	  /// An activity has been cancelled because of boundary event.
	  /// </summary>
	  ACTIVITY_CANCELLED,

	  /// <summary>
	  /// An activity has received a signal. Dispatched after the activity has responded to the signal.
	  /// </summary>
	  ACTIVITY_SIGNALED,

	  /// <summary>
	  /// An activity is about to be executed as a compensation for another activity. The event targets the
	  /// activity that is about to be executed for compensation.
	  /// </summary>
	  ACTIVITY_COMPENSATE,

	  /// <summary>
	  /// An activity has received a message event. Dispatched before the actual message has been received by
	  /// the activity. This event will be either followed by a <seealso cref="#ACTIVITY_SIGNALLED"/> event or <seealso cref="#ACTIVITY_COMPLETE"/>
	  /// for the involved activity, if the message was delivered successfully.
	  /// </summary>
	  ACTIVITY_MESSAGE_RECEIVED,

	  /// <summary>
	  /// An activity has received an error event. Dispatched before the actual error has been received by
	  /// the activity. This event will be either followed by a <seealso cref="#ACTIVITY_SIGNALLED"/> event or <seealso cref="#ACTIVITY_COMPLETE"/>
	  /// for the involved activity, if the error was delivered successfully.
	  /// </summary>
	  ACTIVITY_ERROR_RECEIVED,

	  /// <summary>
	  /// A event dispatched when a <seealso cref="HistoricActivityInstance"/> is created. 
	  /// This is a specialized version of the <seealso cref="ActivitiEventType#ENTITY_CREATED"/> and <seealso cref="ActivitiEventType#ENTITY_INITIALIZED"/> event,
	  /// with the same use case as the <seealso cref="ActivitiEventType#ACTIVITY_STARTED"/>, but containing
	  /// slightly different data.
	  /// 
	  /// Note this will be an <seealso cref="ActivitiEntityEvent"/>, where the entity is the <seealso cref="HistoricActivityInstance"/>.
	  ///  
	  /// Note that history (minimum level ACTIVITY) must be enabled to receive this event.  
	  /// </summary>
	  HISTORIC_ACTIVITY_INSTANCE_CREATED,

	  /// <summary>
	  /// A event dispatched when a <seealso cref="HistoricActivityInstance"/> is marked as ended. 
	  /// his is a specialized version of the <seealso cref="ActivitiEventType#ENTITY_UPDATED"/> event,
	  /// with the same use case as the <seealso cref="ActivitiEventType#ACTIVITY_COMPLETED"/>, but containing
	  /// slightly different data (e.g. the end time, the duration, etc.). 
	  ///  
	  /// Note that history (minimum level ACTIVITY) must be enabled to receive this event.  
	  /// </summary>
	  HISTORIC_ACTIVITY_INSTANCE_ENDED,

	  /// <summary>
	  /// Indicates the engine has taken (ie. followed) a sequenceflow from a source activity to a target activity.
	  /// </summary>
	  SEQUENCEFLOW_TAKEN,

	  /// <summary>
	  /// When a BPMN Error was thrown, but was not caught within in the process.
	  /// </summary>
	  UNCAUGHT_BPMN_ERROR,

	  /// <summary>
	  /// A new variable has been created.
	  /// </summary>
	  VARIABLE_CREATED,

	  /// <summary>
	  /// An existing variable has been updated.
	  /// </summary>
	  VARIABLE_UPDATED,

	  /// <summary>
	  /// An existing variable has been deleted.
	  /// </summary>
	  VARIABLE_DELETED,

	  /// <summary>
	  /// A task has been created. This is thrown when task is fully initialized (before TaskListener.EVENTNAME_CREATE).
	  /// </summary>
	  TASK_CREATED,

	  /// <summary>
	  /// A task as been assigned. This is thrown alongside with an <seealso cref="#ENTITY_UPDATED"/> event.
	  /// </summary>
	  TASK_ASSIGNED,

	  /// <summary>
	  /// A task has been completed. Dispatched before the task entity is deleted (<seealso cref="#ENTITY_DELETED"/>).
	  /// If the task is part of a process, this event is dispatched before the process moves on, as a result of
	  /// the task completion. In that case, a <seealso cref="#ACTIVITY_COMPLETED"/> will be dispatched after an event of this type
	  /// for the activity corresponding to the task. 
	  /// </summary>
	  TASK_COMPLETED,

		/// <summary>
		/// A process instance has been started. Dispatched when starting a process instance previously created. The event
		/// PROCESS_STARTED is dispatched after the associated event ENTITY_INITIALIZED.
		/// </summary>
	  PROCESS_STARTED,

	  /// <summary>
	  /// A process has been completed. Dispatched after the last activity is ACTIVITY_COMPLETED. Process is completed
	  /// when it reaches state in which process instance does not have any transition to take.
	  /// </summary>
	  PROCESS_COMPLETED,

	  /// <summary>
	  /// A process has been completed with an error end event.
	  /// </summary>
	  PROCESS_COMPLETED_WITH_ERROR_END_EVENT,

	  /// <summary>
	  /// A process has been cancelled. Dispatched when process instance is deleted by </summary>
	  /// <seealso cref= org.activiti.engine.impl.RuntimeServiceImpl#deleteProcessInstance(java.lang.String, java.lang.String), before
	  /// DB delete. </seealso>
	  PROCESS_CANCELLED,

	  /// <summary>
	  /// A event dispatched when a <seealso cref="HistoricProcessInstance"/> is created. 
	  /// This is a specialized version of the <seealso cref="ActivitiEventType#ENTITY_CREATED"/> and <seealso cref="ActivitiEventType#ENTITY_INITIALIZED"/> event,
	  /// with the same use case as the <seealso cref="ActivitiEventType#PROCESS_STARTED"/>, but containing
	  /// slightly different data (e.g. the start time, the start user id, etc.). 
	  /// 
	  /// Note this will be an <seealso cref="ActivitiEntityEvent"/>, where the entity is the <seealso cref="HistoricProcessInstance"/>.
	  ///  
	  /// Note that history (minimum level ACTIVITY) must be enabled to receive this event.  
	  /// </summary>
	  HISTORIC_PROCESS_INSTANCE_CREATED,

	  /// <summary>
	  /// A event dispatched when a <seealso cref="HistoricProcessInstance"/> is marked as ended. 
	  /// his is a specialized version of the <seealso cref="ActivitiEventType#ENTITY_UPDATED"/> event,
	  /// with the same use case as the <seealso cref="ActivitiEventType#PROCESS_COMPLETED"/>, but containing
	  /// slightly different data (e.g. the end time, the duration, etc.). 
	  ///  
	  /// Note that history (minimum level ACTIVITY) must be enabled to receive this event.  
	  /// </summary>
	  HISTORIC_PROCESS_INSTANCE_ENDED,

	  /// <summary>
	  /// A new membership has been created.
	  /// </summary>
	  MEMBERSHIP_CREATED,

	  /// <summary>
	  /// A single membership has been deleted.
	  /// </summary>
	  MEMBERSHIP_DELETED,

	  /// <summary>
	  /// All memberships in the related group have been deleted. No individual <seealso cref="#MEMBERSHIP_DELETED"/> events will
	  /// be dispatched due to possible performance reasons. The event is dispatched before the memberships are deleted,
	  /// so they can still be accessed in the dispatch method of the listener.
	  /// </summary>
	  MEMBERSHIPS_DELETED

//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
	  public static final ActivitiEventType[] EMPTY_ARRAY = new ActivitiEventType[] {
	}

	  /// <param name="string"> the string containing a comma-separated list of event-type names </param>
	  /// <returns> a list of <seealso cref="ActivitiEventType"/> based on the given list. </returns>
	  /// <exception cref="ActivitiIllegalArgumentException"> when one of the given string is not a valid type name </exception>
	  public static ActivitiEventType[] getTypesFromString(string @string)
	  {
		IList<ActivitiEventType> result = new List<ActivitiEventType>();
		if (@string != null && @string.Length > 0)
		{
		  string[] split = StringUtils.Split(@string, ",");
		  foreach (string typeName in split)
		  {
			bool found = false;
			foreach (ActivitiEventType type in values())
			{
			  if (typeName.Equals(type.name()))
			  {
				result.Add(type);
				found = true;
				break;
			  }
			}
			if (!found)
			{
			  throw new ActivitiIllegalArgumentException("Invalid event-type: " + typeName);
			}
		  }
		}

		return result.toArray(EMPTY_ARRAY);
	  }
	}

}