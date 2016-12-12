using System;

namespace org.activiti.engine.task
{



    /// <summary>
    /// 表示人类用户的一个任务。
    /// </summary>
    public interface Task : TaskInfo
	{

	  /// <summary>
	  /// Default value used for priority when a new <seealso cref="Task"/> is created.
	  /// </summary>


	  /// <summary>
	  /// Name or title of the task. </summary>
		string Name {set;}

		/// <summary>
		/// Sets an optional localized name for the task. </summary>
	  string LocalizedName {set;}

	  /// <summary>
	  /// Change the description of the task </summary>
		string Description {set;}

		/// <summary>
		/// Sets an optional localized description for the task. </summary>
	  string LocalizedDescription {set;}

		/// <summary>
		/// Sets the indication of how important/urgent this task is </summary>
		int Priority {set;}

	  /// <summary>
	  /// The <seealso cref="User.getId() userId"/> of the person that is responsible for this task. </summary>
	  string Owner {set;}

		/// <summary>
		/// The <seealso cref="User.getId() userId"/> of the person to which this task is delegated. </summary>
		string Assignee {set;}

		/// <summary>
		/// The current <seealso cref="DelegationState"/> for this task. </summary>
	  DelegationState DelegationState {get;set;}


		/// <summary>
		/// Change due date of the task. </summary>
		DateTime DueDate {set;}

		/// <summary>
		/// Change the category of the task. This is an optional field and allows to 'tag' tasks as belonging to a certain category. </summary>
		string Category {set;}

		/// <summary>
		/// delegates this task to the given user and sets the <seealso cref="#getDelegationState() delegationState"/> to <seealso cref="DelegationState#PENDING"/>.
		/// If no owner is set on the task, the owner is set to the current assignee of the task. 
		/// </summary>
	  void @delegate(string userId);

	  /// <summary>
	  /// the parent task for which this task is a subtask </summary>
	  string ParentTaskId {set;}

	  /// <summary>
	  /// Change the tenantId of the task </summary>
	  string TenantId {set;}

	  /// <summary>
	  /// Change the form key of the task </summary>
	  string FormKey {set;}

	  /// <summary>
	  /// Indicates whether this task is suspended or not. </summary>
		bool Suspended {get;}


	}

	public static class Task_Fields
	{
	  public const int DEFAULT_PRIORITY = 50;
	}

}