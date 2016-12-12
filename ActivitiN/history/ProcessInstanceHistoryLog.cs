using System;
using System.Collections.Generic;

namespace org.activiti.engine.history
{


	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;

	/// <summary>
	/// A trail of data for a given process instance.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public interface ProcessInstanceHistoryLog
	{

		 /// <summary>
		 /// The process instance id (== as the id for the runtime <seealso cref="ProcessInstance process instance"/>). </summary>
	  string Id {get;}

	  /// <summary>
	  /// The user provided unique reference to this process instance. </summary>
	  string BusinessKey {get;}

	  /// <summary>
	  /// The process definition reference. </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// The time the process was started. </summary>
	  DateTime StartTime {get;}

	  /// <summary>
	  /// The time the process was ended. </summary>
	  DateTime EndTime {get;}

	  /// <summary>
	  /// The difference between <seealso cref="#getEndTime()"/> and <seealso cref="#getStartTime()"/> . </summary>
	  long? DurationInMillis {get;}

	  /// <summary>
	  /// The authenticated user that started this process instance. </summary>
	  /// <seealso cref= IdentityService#setAuthenticatedUserId(String)  </seealso>
	  string StartUserId {get;}

	  /// <summary>
	  /// The start activity. </summary>
	  string StartActivityId {get;}

	  /// <summary>
	  /// Obtains the reason for the process instance's deletion. </summary>
	  string DeleteReason {get;}

	  /// <summary>
	  /// The process instance id of a potential super process instance or null if no super process instance exists
	  /// </summary>
	  string SuperProcessInstanceId {get;}

	  /// <summary>
	  /// The tenant identifier for the process instance.
	  /// </summary>
	  string TenantId {get;}

	  /// <summary>
	  /// The trail of data, ordered by date (ascending).
	  /// Gives a replay of the process instance.
	  /// </summary>
	  IList<HistoricData> HistoricData {get;}

	}

}