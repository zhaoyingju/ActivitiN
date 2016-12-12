namespace org.activiti.engine.runtime
{

	using org.activiti.engine.query;

	/// <summary>
	/// Allows querying of <seealso cref="Execution"/>s via native (SQL) queries
	/// @author Bernd Ruecker (camunda)
	/// </summary>
	public interface NativeExecutionQuery : NativeQuery<NativeExecutionQuery, Execution>
	{

	}

}