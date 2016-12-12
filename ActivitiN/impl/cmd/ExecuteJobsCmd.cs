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
namespace org.activiti.engine.impl.cmd
{

	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using TransactionState = org.activiti.engine.impl.cfg.TransactionState;
	using Context = org.activiti.engine.impl.context.Context;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using FailedJobListener = org.activiti.engine.impl.jobexecutor.FailedJobListener;
	using JobExecutorContext = org.activiti.engine.impl.jobexecutor.JobExecutorContext;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class ExecuteJobsCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;

	  private static Logger log = LoggerFactory.getLogger(typeof(ExecuteJobsCmd));

	  protected internal string jobId;
	  protected internal JobEntity job;

	  public ExecuteJobsCmd(string jobId)
	  {
		this.jobId = jobId;
	  }

	  public ExecuteJobsCmd(JobEntity job)
	  {
		  this.job = job;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {

		if (jobId == null && job == null)
		{
		  throw new ActivitiIllegalArgumentException("jobId and job is null");
		}

		if (job == null)
		{
			job = commandContext.JobEntityManager.findJobById(jobId);
		}

		if (job == null)
		{
		  throw new JobNotFoundException(jobId);
		}

		if (log.DebugEnabled)
		{
		  log.debug("Executing job {}", job.Id);
		}

		JobExecutorContext jobExecutorContext = Context.JobExecutorContext;
		if (jobExecutorContext != null) // if null, then we are not called by the job executor
		{
		  jobExecutorContext.CurrentJob = job;
		}

		FailedJobListener failedJobListener = null;
		try
		{
		  // When transaction is rolled back, decrement retries
		  failedJobListener = new FailedJobListener(commandContext.ProcessEngineConfiguration.CommandExecutor, jobId);
		  commandContext.TransactionContext.addTransactionListener(TransactionState.ROLLED_BACK, failedJobListener);

		  job.execute(commandContext);

		  if (commandContext.EventDispatcher.Enabled)
		  {
			  commandContext.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.JOB_EXECUTION_SUCCESS, job));
		  }

		}
		catch (Exception exception)
		{
		  failedJobListener.Exception = exception;

		  // Dispatch an event, indicating job execution failed in a try-catch block, to prevent the original
		  // exception to be swallowed
		  if (commandContext.EventDispatcher.Enabled)
		  {
			  try
			  {
				  commandContext.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityExceptionEvent(ActivitiEventType.JOB_EXECUTION_FAILURE, job, exception));
			  }
			  catch (Exception ignore)
			  {
				  log.warn("Exception occured while dispatching job failure event, ignoring.", ignore);
			  }
		  }

		  // Finally, Throw the exception to indicate the ExecuteJobCmd failed
		  throw new ActivitiException("Job " + jobId + " failed", exception);
		}
		finally
		{
		  if (jobExecutorContext != null)
		  {
			jobExecutorContext.CurrentJob = null;
		  }
		}
		return null;
	  }

	  public virtual string JobId
	  {
		  get
		  {
				return jobId;
		  }
	  }

	}

}