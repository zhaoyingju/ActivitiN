namespace org.activiti.engine.repository
{

	using org.activiti.engine.query;

	/// <summary>
	/// Allows querying of <seealso cref="org.activiti.engine.repository.ProcessDefinition"/>s via native (SQL) queries
	/// @author Henry Yan(http://www.kafeitu.me)
	/// </summary>
	public interface NativeProcessDefinitionQuery : NativeQuery<NativeProcessDefinitionQuery, ProcessDefinition>
	{

	}
}