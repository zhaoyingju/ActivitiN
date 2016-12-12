namespace org.activiti.engine.history
{

	using Comment = org.activiti.engine.task.Comment;

	/// <summary>
	/// Allows to fetch the <seealso cref="ProcessInstanceHistoryLog"/> for a process instance.
	/// 
	/// Note that every includeXXX() method below will lead to an additional query.
	/// 
	/// This class is actually a convenience on top of the other specific queries such 
	/// as <seealso cref="HistoricTaskInstanceQuery"/>, <seealso cref="HistoricActivityInstanceQuery"/>, ...
	/// It will execute separate queries for each included type, order the 
	/// data according to the date (ascending) and wrap the results in the <seealso cref="ProcessInstanceHistoryLog"/>.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public interface ProcessInstanceHistoryLogQuery
	{

		/// <summary>
		/// The <seealso cref="ProcessInstanceHistoryLog"/> will contain the <seealso cref="HistoricTaskInstance"/> instances. </summary>
		ProcessInstanceHistoryLogQuery includeTasks();

		/// <summary>
		/// The <seealso cref="ProcessInstanceHistoryLog"/> will contain the <seealso cref="HistoricActivityInstance"/> instances. </summary>
		ProcessInstanceHistoryLogQuery includeActivities();

		/// <summary>
		/// The <seealso cref="ProcessInstanceHistoryLog"/> will contain the <seealso cref="HistoricVariableInstance"/> instances. </summary>
		ProcessInstanceHistoryLogQuery includeVariables();

		/// <summary>
		/// The <seealso cref="ProcessInstanceHistoryLog"/> will contain the <seealso cref="Comment"/> instances. </summary>
		ProcessInstanceHistoryLogQuery includeComments();

		/// <summary>
		/// The <seealso cref="ProcessInstanceHistoryLog"/> will contain the <seealso cref="HistoricVariableUpdate"/> instances. </summary>
		ProcessInstanceHistoryLogQuery includeVariableUpdates();

		/// <summary>
		/// The <seealso cref="ProcessInstanceHistoryLog"/> will contain the <seealso cref="HistoricFormProperty"/> instances. </summary>
		ProcessInstanceHistoryLogQuery includeFormProperties();

		/// <summary>
		/// Executes the query. </summary>
		ProcessInstanceHistoryLog singleResult();

	}

}