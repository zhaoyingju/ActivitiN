using System.Collections.Generic;

namespace org.activiti.engine.impl
{


	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using NativeProcessInstanceQuery = org.activiti.engine.runtime.NativeProcessInstanceQuery;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;


	public class NativeProcessInstanceQueryImpl : AbstractNativeQuery<NativeProcessInstanceQuery, ProcessInstance>, NativeProcessInstanceQuery
	{

	  private const long serialVersionUID = 1L;

	  public NativeProcessInstanceQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeProcessInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }


	 //results ////////////////////////////////////////////////////////////////

	  public virtual IList<ProcessInstance> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return commandContext.ExecutionEntityManager.findProcessInstanceByNativeQuery(parameterMap, firstResult, maxResults);
	  }

	  public virtual long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
		return commandContext.ExecutionEntityManager.findExecutionCountByNativeQuery(parameterMap);
		  // can use execution count, since the result type doesn't matter
	  }

	}

}