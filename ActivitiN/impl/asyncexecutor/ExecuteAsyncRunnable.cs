using System;

/* Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.activiti.engine.impl.asyncexecutor
{

	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using ExecuteAsyncJobCmd = org.activiti.engine.impl.cmd.ExecuteAsyncJobCmd;
	using LockExclusiveJobCmd = org.activiti.engine.impl.cmd.LockExclusiveJobCmd;
	using UnlockExclusiveJobCmd = org.activiti.engine.impl.cmd.UnlockExclusiveJobCmd;
	using Context = org.activiti.engine.impl.context.Context;
	using org.activiti.engine.impl.interceptor;
	using CommandConfig = org.activiti.engine.impl.interceptor.CommandConfig;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using FailedJobCommandFactory = org.activiti.engine.impl.jobexecutor.FailedJobCommandFactory;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// @author Joram Barrez
	/// @author Tijs Rademakers
	/// </summary>
	public class ExecuteAsyncRunnable : Runnable
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(ExecuteAsyncRunnable));

	  protected internal JobEntity job;
	  protected internal CommandExecutor commandExecutor;

	  public ExecuteAsyncRunnable(JobEntity job, CommandExecutor commandExecutor)
	  {
		this.job = job;
		this.commandExecutor = commandExecutor;
	  }

	  public virtual void run()
	  {
		bool lockNotNeededOrSuccess = lockJobIfNeeded();
		if (lockNotNeededOrSuccess)
		{
		  executeJob();
		  unlockJobIfNeeded();
		}
	  }

	  /// <summary>
	  /// Returns true if lock succeeded, or no lock was needed.
	  /// Returns false if locking was unsuccessfull. 
	  /// </summary>
	  protected internal virtual bool lockJobIfNeeded()
	  {
		try
		{
		  if (job.Exclusive)
		  {
			commandExecutor.execute(new LockExclusiveJobCmd(job));
		  }

		}
		catch (Exception lockException)
		{
		  if (log.DebugEnabled)
		  {
			log.debug("Could not lock exclusive job. Unlocking job so it can be acquired again. Catched exception: " + lockException.Message);
		  }

		  // Release the job again so it can be acquired later or by another node
		  unacquireJob();

		  return false;
		}

		return true;
	  }

	  protected internal virtual void unacquireJob()
	  {
		CommandContext commandContext = Context.CommandContext;
		if (commandContext != null)
		{
		  commandContext.JobEntityManager.unacquireJob(job.Id);
		}
		else
		{
		  commandExecutor.execute(new CommandAnonymousInnerClassHelper(this, commandContext));
		}
	  }

	  private class CommandAnonymousInnerClassHelper : Command<Void>
	  {
		  private readonly ExecuteAsyncRunnable outerInstance;

		  private CommandContext commandContext;

		  public CommandAnonymousInnerClassHelper(ExecuteAsyncRunnable outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

		  public virtual Void execute(CommandContext commandContext)
		  {
			commandContext.JobEntityManager.unacquireJob(outerInstance.job.Id);
			return null;
		  }
	  }

	  protected internal virtual void executeJob()
	  {
		try
		{
		  commandExecutor.execute(new ExecuteAsyncJobCmd(job));

		}
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not allowed in C#:
//ORIGINAL LINE: catch (final org.activiti.engine.ActivitiOptimisticLockingException e)
		catch (ActivitiOptimisticLockingException e)
		{

		  handleFailedJob(e);

		  if (log.DebugEnabled)
		  {
			log.debug("Optimistic locking exception during job execution. If you have multiple async executors running against the same database, " + "this exception means that this thread tried to acquire an exclusive job, which already was changed by another async executor thread." + "This is expected behavior in a clustered environment. " + "You can ignore this message if you indeed have multiple job executor threads running against the same database. " + "Exception message: {}", e.Message);
		  }

		}
		catch (Exception exception)
		{
		  handleFailedJob(exception);

		  // Finally, Throw the exception to indicate the ExecuteAsyncJobCmd failed
		  string message = "Job " + job.Id + " failed";
		  log.error(message, exception);
		}
	  }

	  protected internal virtual void unlockJobIfNeeded()
	  {
		try
		{
		  if (job.Exclusive)
		  {
			commandExecutor.execute(new UnlockExclusiveJobCmd(job));
		  }

		}
		catch (ActivitiOptimisticLockingException optimisticLockingException)
		{
		  if (log.DebugEnabled)
		  {
			log.debug("Optimistic locking exception while unlocking the job. If you have multiple async executors running against the same database, " + "this exception means that this thread tried to acquire an exclusive job, which already was changed by another async executor thread." + "This is expected behavior in a clustered environment. " + "You can ignore this message if you indeed have multiple job executor acquisition threads running against the same database. " + "Exception message: {}", optimisticLockingException.Message);
		  }

		}
		catch (Exception t)
		{
		  log.error("Error while unlocking exclusive job " + job.Id, t);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void handleFailedJob(final Throwable exception)
	  protected internal virtual void handleFailedJob(Exception exception)
	  {
		commandExecutor.execute(new CommandAnonymousInnerClassHelper2(this, exception));
	  }

	  private class CommandAnonymousInnerClassHelper2 : Command<Void>
	  {
		  private readonly ExecuteAsyncRunnable outerInstance;

		  private Exception exception;

		  public CommandAnonymousInnerClassHelper2(ExecuteAsyncRunnable outerInstance, Exception exception)
		  {
			  this.outerInstance = outerInstance;
			  this.exception = exception;
		  }


		  public virtual Void execute(CommandContext commandContext)
		  {
			CommandConfig commandConfig = outerInstance.commandExecutor.DefaultConfig.transactionRequiresNew();
			FailedJobCommandFactory failedJobCommandFactory = commandContext.FailedJobCommandFactory;
			Command<object> cmd = failedJobCommandFactory.getCommand(outerInstance.job.Id, exception);

			log.trace("Using FailedJobCommandFactory '" + failedJobCommandFactory.GetType() + "' and command of type '" + cmd.GetType() + "'");
			outerInstance.commandExecutor.execute(commandConfig, cmd);

			// Dispatch an event, indicating job execution failed in a try-catch block, to prevent the original
			// exception to be swallowed
			if (commandContext.EventDispatcher.Enabled)
			{
			  try
			  {
				commandContext.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityExceptionEvent(ActivitiEventType.JOB_EXECUTION_FAILURE, outerInstance.job, exception));
			  }
			  catch (Exception ignore)
			  {
				log.warn("Exception occured while dispatching job failure event, ignoring.", ignore);
			  }
			}

			return null;
		  }

	  }
	}

}