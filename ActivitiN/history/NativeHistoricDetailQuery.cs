namespace org.activiti.engine.history
{

	using org.activiti.engine.query;

	/// <summary>
	/// Allows querying of <seealso cref="org.activiti.engine.history.HistoricDetail"/>s via native (SQL) queries
	/// @author Henry Yan(http://www.kafeitu.me)
	/// </summary>
	public interface NativeHistoricDetailQuery : NativeQuery<NativeHistoricDetailQuery, HistoricDetail>
	{

	}
}