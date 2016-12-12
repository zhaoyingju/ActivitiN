namespace org.activiti.engine.impl.asyncexecutor
{

	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;

	public class DefaultExecuteAsyncRunnableFactory : ExecuteAsyncRunnableFactory
	{

	  public override Runnable createExecuteAsyncRunnable(JobEntity jobEntity, CommandExecutor commandExecutor)
	  {
		return new ExecuteAsyncRunnable(jobEntity, commandExecutor);
	  }
	}

}