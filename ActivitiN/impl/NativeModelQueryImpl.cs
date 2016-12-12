using System.Collections.Generic;

namespace org.activiti.engine.impl
{

	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using NativeModelQuery = org.activiti.engine.repository.NativeModelQuery;
	using Model = org.activiti.engine.repository.Model;


	public class NativeModelQueryImpl : AbstractNativeQuery<NativeModelQuery, Model>, NativeModelQuery
	{

	  private const long serialVersionUID = 1L;

	  public NativeModelQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeModelQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	 //results ////////////////////////////////////////////////////////////////

	  public virtual IList<Model> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return commandContext.ModelEntityManager.findModelsByNativeQuery(parameterMap, firstResult, maxResults);
	  }

	  public virtual long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
		return commandContext.ModelEntityManager.findModelCountByNativeQuery(parameterMap);
	  }

	}

}