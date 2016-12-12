using System.Collections.Generic;

namespace org.activiti.engine.impl
{


	using HistoricDetail = org.activiti.engine.history.HistoricDetail;
	using NativeHistoricDetailQuery = org.activiti.engine.history.NativeHistoricDetailQuery;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;

	public class NativeHistoricDetailQueryImpl : AbstractNativeQuery<NativeHistoricDetailQuery, HistoricDetail>, NativeHistoricDetailQuery
	{

	  private const long serialVersionUID = 1L;

	  public NativeHistoricDetailQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeHistoricDetailQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	 //results ////////////////////////////////////////////////////////////////

	  public virtual IList<HistoricDetail> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return commandContext.HistoricDetailEntityManager.findHistoricDetailsByNativeQuery(parameterMap, firstResult, maxResults);
	  }

	  public virtual long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
		return commandContext.HistoricDetailEntityManager.findHistoricDetailCountByNativeQuery(parameterMap);
	  }

	}
}