namespace org.activiti.engine.task
{

	using org.activiti.engine.query;

	/// <summary>
	/// Allows querying of <seealso cref="Task"/>s via native (SQL) queries
	/// @author Bernd Ruecker (camunda)
	/// </summary>
	public interface NativeTaskQuery : NativeQuery<NativeTaskQuery, Task>
	{

	}

}