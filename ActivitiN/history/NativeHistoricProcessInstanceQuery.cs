namespace org.activiti.engine.history
{

	using org.activiti.engine.query;

	/// <summary>
	/// Allows querying of <seealso cref="HistoricTaskInstanceQuery"/>s via native (SQL) queries
	/// @author Bernd Ruecker (camunda)
	/// </summary>
	public interface NativeHistoricProcessInstanceQuery : NativeQuery<NativeHistoricProcessInstanceQuery, HistoricProcessInstance>
	{

	}

}