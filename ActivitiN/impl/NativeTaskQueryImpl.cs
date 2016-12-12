using System.Collections.Generic;

namespace org.activiti.engine.impl
{


	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using NativeTaskQuery = org.activiti.engine.task.NativeTaskQuery;
	using Task = org.activiti.engine.task.Task;


	public class NativeTaskQueryImpl : AbstractNativeQuery<NativeTaskQuery, Task>, NativeTaskQuery
	{

	  private const long serialVersionUID = 1L;

	  public NativeTaskQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeTaskQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }


	 //results ////////////////////////////////////////////////////////////////

	  public virtual IList<Task> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return commandContext.TaskEntityManager.findTasksByNativeQuery(parameterMap, firstResult, maxResults);
	  }

	  public virtual long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
		return commandContext.TaskEntityManager.findTaskCountByNativeQuery(parameterMap);
	  }

	}

}