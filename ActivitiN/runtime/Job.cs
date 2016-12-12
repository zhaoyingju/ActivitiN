using System;

namespace org.activiti.engine.runtime
{



    /// <summary>
    /// 表示一个作业（定时器，消息等）。
    /// </summary>
    public interface Job
	{

	  /// <summary>
	  /// Returns the unique identifier for this job.
	  /// </summary>
	  string Id {get;}

	  /// <summary>
	  /// Returns the date on which this job is supposed to be processed.
	  /// </summary>
	  DateTime Duedate {get;}

	  /// <summary>
	  /// Returns the id of the process instance which execution created the job.
	  /// </summary>
	  string ProcessInstanceId {get;}

	  /// <summary>
	  /// Returns the specific execution on which the job was created. 
	  /// </summary>
	  string ExecutionId {get;}

	  /// <summary>
	  /// Returns the specific process definition on which the job was created
	  /// </summary>
	  string ProcessDefinitionId {get;}

	  /// <summary>
	  /// Returns the number of retries this job has left. 
	  /// Whenever the jobexecutor fails to execute the job, this value is decremented. 
	  /// When it hits zero, the job is supposed to be dead and not retried again 
	  /// (ie a manual retry is required then).
	  /// </summary>
	  int Retries {get;}

	  /// <summary>
	  /// Returns the message of the exception that occurred, the last time the job was
	  /// executed. Returns null when no exception occurred.
	  /// 
	  /// To get the full exception stacktrace, 
	  /// use <seealso cref="ManagementService#getJobExceptionStacktrace(String)"/>
	  /// </summary>
	  string ExceptionMessage {get;}

	  /// <summary>
	  /// Get the tenant identifier for this job.
	  /// </summary>
	  string TenantId {get;}

	}

}