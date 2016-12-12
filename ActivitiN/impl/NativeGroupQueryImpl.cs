using System.Collections.Generic;

namespace org.activiti.engine.impl
{

	using NativeGroupQuery = org.activiti.engine.identity.NativeGroupQuery;
	using Group = org.activiti.engine.identity.Group;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;


	public class NativeGroupQueryImpl : AbstractNativeQuery<NativeGroupQuery, Group>, NativeGroupQuery
	{

	  private const long serialVersionUID = 1L;

	  public NativeGroupQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeGroupQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }


	 //results ////////////////////////////////////////////////////////////////

	  public virtual IList<Group> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return commandContext.GroupIdentityManager.findGroupsByNativeQuery(parameterMap, firstResult, maxResults);
	  }

	  public virtual long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
		return commandContext.GroupIdentityManager.findGroupCountByNativeQuery(parameterMap);
	  }

	}
}