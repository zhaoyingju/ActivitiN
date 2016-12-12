using System.Collections.Generic;

namespace org.activiti.engine.impl
{

	using NativeUserQuery = org.activiti.engine.identity.NativeUserQuery;
	using User = org.activiti.engine.identity.User;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;


	public class NativeUserQueryImpl : AbstractNativeQuery<NativeUserQuery, User>, NativeUserQuery
	{

	  private const long serialVersionUID = 1L;

	  public NativeUserQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeUserQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }


	 //results ////////////////////////////////////////////////////////////////

	  public virtual IList<User> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return commandContext.UserIdentityManager.findUsersByNativeQuery(parameterMap, firstResult, maxResults);
	  }

	  public virtual long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
		return commandContext.UserIdentityManager.findUserCountByNativeQuery(parameterMap);
	  }

	}
}