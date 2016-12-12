using System.Collections.Generic;

namespace org.activiti.engine.impl
{

	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using NativeProcessDefinitionQuery = org.activiti.engine.repository.NativeProcessDefinitionQuery;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;


	public class NativeProcessDefinitionQueryImpl : AbstractNativeQuery<NativeProcessDefinitionQuery, ProcessDefinition>, NativeProcessDefinitionQuery
	{

	  private const long serialVersionUID = 1L;

	  public NativeProcessDefinitionQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeProcessDefinitionQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }


	 //results ////////////////////////////////////////////////////////////////

	  public virtual IList<ProcessDefinition> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return commandContext.ProcessDefinitionEntityManager.findProcessDefinitionsByNativeQuery(parameterMap, firstResult, maxResults);
	  }

	  public virtual long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
		return commandContext.ProcessDefinitionEntityManager.findProcessDefinitionCountByNativeQuery(parameterMap);
	  }

	}

}