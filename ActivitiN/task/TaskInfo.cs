using System;
using System.Collections.Generic;


namespace org.activiti.engine.task
{

	public interface TaskInfo
	{

		/// <summary>
		/// DB id of the task. </summary>
		string Id {get;}

		/// <summary>
		/// Name or title of the task.
		/// </summary>
		string Name {get;}

		/// <summary>
		/// Free text description of the task.
		/// </summary>
		string Description {get;}

		/// <summary>
		/// Indication of how important/urgent this task is
		/// </summary>
		int Priority {get;}

		/// <summary>
		/// The <seealso cref="User.getId() userId"/> of the person that is responsible for this
		/// task.
		/// </summary>
		string Owner {get;}

		/// <summary>
		/// The <seealso cref="User.getId() userId"/> of the person to which this task is
		/// delegated.
		/// </summary>
		string Assignee {get;}

		/// <summary>
		/// Reference to the process instance or null if it is not related to a process
		/// instance.
		/// </summary>
		string ProcessInstanceId {get;}

		/// <summary>
		/// Reference to the path of execution or null if it is not related to a
		/// process instance.
		/// </summary>
		string ExecutionId {get;}

		/// <summary>
		/// Reference to the process definition or null if it is not related to a
		/// process.
		/// </summary>
		string ProcessDefinitionId {get;}

		/// <summary>
		/// The date/time when this task was created </summary>
		DateTime CreateTime {get;}

		/// <summary>
		/// The id of the activity in the process defining this task or null if this is
		/// not related to a process
		/// </summary>
		string TaskDefinitionKey {get;}

		/// <summary>
		/// Due date of the task.
		/// </summary>
		DateTime DueDate {get;}

		/// <summary>
		/// The category of the task. This is an optional field and allows to 'tag'
		/// tasks as belonging to a certain category.
		/// </summary>
		string Category {get;}

		/// <summary>
		/// The parent task for which this task is a subtask
		/// </summary>
		string ParentTaskId {get;}

		/// <summary>
		/// The tenant identifier of this task
		/// </summary>
		string TenantId {get;}

		/// <summary>
		/// The form key for the user task
		/// </summary>
		string FormKey {get;}

		/// <summary>
		/// Returns the local task variables if requested in the task query
		/// </summary>
		IDictionary<string, object> TaskLocalVariables {get;}

		/// <summary>
		/// Returns the process variables if requested in the task query
		/// </summary>
		IDictionary<string, object> ProcessVariables {get;}

	}

}