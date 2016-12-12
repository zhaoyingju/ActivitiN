namespace org.activiti.engine.history
{

	using org.activiti.engine.query;

	/// <summary>
	/// Allows querying of <seealso cref="HistoricActivityInstanceQuery"/>s via native (SQL) queries
	/// @author Bernd Ruecker (camunda)
	/// </summary>
	public interface NativeHistoricActivityInstanceQuery : NativeQuery<NativeHistoricActivityInstanceQuery, HistoricActivityInstance>
	{

	}

}