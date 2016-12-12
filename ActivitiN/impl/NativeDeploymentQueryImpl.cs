using System.Collections.Generic;

namespace org.activiti.engine.impl
{

	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using NativeDeploymentQuery = org.activiti.engine.repository.NativeDeploymentQuery;
	using Deployment = org.activiti.engine.repository.Deployment;


	public class NativeDeploymentQueryImpl : AbstractNativeQuery<NativeDeploymentQuery, Deployment>, NativeDeploymentQuery
	{

	  private const long serialVersionUID = 1L;

	  public NativeDeploymentQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeDeploymentQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }


	 //results ////////////////////////////////////////////////////////////////

	  public virtual IList<Deployment> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return commandContext.DeploymentEntityManager.findDeploymentsByNativeQuery(parameterMap, firstResult, maxResults);
	  }

	  public virtual long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
		return commandContext.DeploymentEntityManager.findDeploymentCountByNativeQuery(parameterMap);
	  }

	}

}