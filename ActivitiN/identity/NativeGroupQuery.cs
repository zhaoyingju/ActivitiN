namespace org.activiti.engine.identity
{

	using org.activiti.engine.query;

	/// <summary>
	/// Allows querying of <seealso cref="org.activiti.engine.identity.Group"/>s via native (SQL) queries
	/// @author Henry Yan(http://www.kafeitu.me)
	/// </summary>
	public interface NativeGroupQuery : NativeQuery<NativeGroupQuery, Group>
	{

	}
}