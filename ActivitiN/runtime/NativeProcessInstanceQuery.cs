namespace org.activiti.engine.runtime
{

	using org.activiti.engine.query;

	/// <summary>
	/// Allows querying of <seealso cref="ProcessInstance"/>s via native (SQL) queries
	/// @author Bernd Ruecker (camunda)
	/// </summary>
	public interface NativeProcessInstanceQuery : NativeQuery<NativeProcessInstanceQuery, ProcessInstance>
	{

	}

}