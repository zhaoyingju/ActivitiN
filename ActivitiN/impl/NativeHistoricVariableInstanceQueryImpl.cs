using System.Collections.Generic;

namespace org.activiti.engine.impl
{


	using HistoricVariableInstance = org.activiti.engine.history.HistoricVariableInstance;
	using NativeHistoricVariableInstanceQuery = org.activiti.engine.history.NativeHistoricVariableInstanceQuery;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;

	public class NativeHistoricVariableInstanceQueryImpl : AbstractNativeQuery<NativeHistoricVariableInstanceQuery, HistoricVariableInstance>, NativeHistoricVariableInstanceQuery
	{

	  private const long serialVersionUID = 1L;

	  public NativeHistoricVariableInstanceQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeHistoricVariableInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	 //results ////////////////////////////////////////////////////////////////

	  public virtual IList<HistoricVariableInstance> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return commandContext.HistoricVariableInstanceEntityManager.findHistoricVariableInstancesByNativeQuery(parameterMap, firstResult, maxResults);
	  }

	  public virtual long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
		return commandContext.HistoricVariableInstanceEntityManager.findHistoricVariableInstanceCountByNativeQuery(parameterMap);
	  }

	}
}