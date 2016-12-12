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


	using ActivitiEventDispatcher = org.activiti.engine.@delegate.@event.ActivitiEventDispatcher;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using DurationHelper = org.activiti.engine.impl.calendar.DurationHelper;
	using TransactionContext = org.activiti.engine.impl.cfg.TransactionContext;
	using TransactionState = org.activiti.engine.impl.cfg.TransactionState;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using AsyncContinuationJobHandler = org.activiti.engine.impl.jobexecutor.AsyncContinuationJobHandler;
	using JobAddedNotification = org.activiti.engine.impl.jobexecutor.JobAddedNotification;
	using JobExecutor = org.activiti.engine.impl.jobexecutor.JobExecutor;
	using TimerCatchIntermediateEventJobHandler = org.activiti.engine.impl.jobexecutor.TimerCatchIntermediateEventJobHandler;
	using TimerEventHandler = org.activiti.engine.impl.jobexecutor.TimerEventHandler;
	using TimerExecuteNestedActivityJobHandler = org.activiti.engine.impl.jobexecutor.TimerExecuteNestedActivityJobHandler;
	using TimerStartEventJobHandler = org.activiti.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using DeploymentManager = org.activiti.engine.impl.persistence.deploy.DeploymentManager;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using TimerEntity = org.activiti.engine.impl.persistence.entity.TimerEntity;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// @author Saeid Mirzaei
	/// </summary>

	public class JobRetryCmd : Command<object>
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger log = LoggerFactory.getLogger(typeof(JobRetryCmd).FullName);

	  protected internal string jobId;
	  protected internal Exception exception;

	  public JobRetryCmd(string jobId, Exception exception)
	  {
		this.jobId = jobId;
		this.exception = exception;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		JobEntity job = commandContext.JobEntityManager.findJobById(jobId);
		if (job == null)
		{
		  return null;
		}

		ActivityImpl activity = getCurrentActivity(commandContext, job);
		ProcessEngineConfiguration processEngineConfig = commandContext.ProcessEngineConfiguration;

		if (activity == null || activity.FailedJobRetryTimeCycleValue == null)
		{
		  log.debug("activitiy or FailedJobRetryTimerCycleValue is null in job " + jobId + "'. only decrementing retries.");
		  job.Retries = job.Retries - 1;
		  job.LockOwner = null;
		  job.LockExpirationTime = null;
		  if (job.Duedate == null)
		  {
			// add wait time for failed async job
			job.Duedate = calculateDueDate(commandContext, processEngineConfig.AsyncFailedJobWaitTime, null);
		  }
		  else
		  {
			// add default wait time for failed job
			job.Duedate = calculateDueDate(commandContext, processEngineConfig.DefaultFailedJobWaitTime, job.Duedate);
		  }

		}
		else
		{
		  string failedJobRetryTimeCycle = activity.FailedJobRetryTimeCycleValue;
		  try
		  {
			DurationHelper durationHelper = new DurationHelper(failedJobRetryTimeCycle, processEngineConfig.Clock);
			job.LockOwner = null;
			job.LockExpirationTime = null;
			job.Duedate = durationHelper.DateAfter;

			if (job.ExceptionMessage == null) // is it the first exception
			{
			  log.debug("Applying JobRetryStrategy '" + failedJobRetryTimeCycle + "' the first time for job " + job.Id + " with " + durationHelper.Times + " retries");
			  // then change default retries to the ones configured
			  job.Retries = durationHelper.Times;

			}
			else
			{
			  log.debug("Decrementing retries of JobRetryStrategy '" + failedJobRetryTimeCycle + "' for job " + job.Id);
			}
			job.Retries = job.Retries - 1;

		  }
		  catch (Exception)
		  {
			throw new ActivitiException("failedJobRetryTimeCylcle has wrong format:" + failedJobRetryTimeCycle, exception);
		  }
		}

		if (exception != null)
		{
		  job.ExceptionMessage = exception.Message;
		  job.ExceptionStacktrace = ExceptionStacktrace;
		}

		// Dispatch both an update and a retry-decrement event
		ActivitiEventDispatcher eventDispatcher = commandContext.EventDispatcher;
		if (eventDispatcher.Enabled)
		{
			eventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, job));
			eventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.JOB_RETRIES_DECREMENTED, job));
		}

		if (processEngineConfig.AsyncExecutorEnabled == false)
		{
		  JobExecutor jobExecutor = processEngineConfig.JobExecutor;
		  JobAddedNotification messageAddedNotification = new JobAddedNotification(jobExecutor);
		  TransactionContext transactionContext = commandContext.TransactionContext;
		  transactionContext.addTransactionListener(TransactionState.COMMITTED, messageAddedNotification);
		}

		return null;
	  }

	  protected internal virtual DateTime calculateDueDate(CommandContext commandContext, int waitTimeInSeconds, DateTime oldDate)
	  {
		DateTime newDateCal = new GregorianCalendar();
		if (oldDate != null)
		{
		  newDateCal = new DateTime(oldDate);

		}
		else
		{
		  newDateCal = new DateTime(commandContext.ProcessEngineConfiguration.Clock.CurrentTime);
		}

		newDateCal.AddSeconds(waitTimeInSeconds);
		return newDateCal.Ticks;
	  }

	  private ActivityImpl getCurrentActivity(CommandContext commandContext, JobEntity job)
	  {
		string type = job.JobHandlerType;
		ActivityImpl activity = null;

		if (TimerExecuteNestedActivityJobHandler.TYPE.Equals(type) || TimerCatchIntermediateEventJobHandler.TYPE.Equals(type))
		{
		  ExecutionEntity execution = fetchExecutionEntity(commandContext, job.ExecutionId);
		  if (execution != null)
		  {
			activity = execution.ProcessDefinition.findActivity(job.JobHandlerConfiguration);
		  }
		}
		else if (TimerStartEventJobHandler.TYPE.Equals(type))
		{

		  DeploymentManager deploymentManager = commandContext.ProcessEngineConfiguration.DeploymentManager;
		  if (TimerEventHandler.hasRealActivityId(job.JobHandlerConfiguration))
		  {

			ProcessDefinitionEntity processDefinition = deploymentManager.findDeployedProcessDefinitionById(job.ProcessDefinitionId);
			string activityId = TimerEventHandler.getActivityIdFromConfiguration(job.JobHandlerConfiguration);
			activity = processDefinition.findActivity(activityId);

		  }
		  else
		  {
			string processId = job.JobHandlerConfiguration;
			if (job is TimerEntity)
			{
			   processId = TimerEventHandler.getActivityIdFromConfiguration(job.JobHandlerConfiguration);
			}

			ProcessDefinitionEntity processDefinition = null;
			if (job.TenantId != null && job.TenantId.Length > 0)
			{
			  processDefinition = deploymentManager.findDeployedLatestProcessDefinitionByKeyAndTenantId(processId, job.TenantId);
			}
			else
			{
			  processDefinition = deploymentManager.findDeployedLatestProcessDefinitionByKey(processId);
			}

			if (processDefinition != null)
			{
			  activity = processDefinition.Initial;
			}
		  }

		}
		else if (AsyncContinuationJobHandler.TYPE.Equals(type))
		{
		  ExecutionEntity execution = fetchExecutionEntity(commandContext, job.ExecutionId);
		  if (execution != null)
		  {
			activity = execution.Activity;
		  }
		}
		else
		{
		  // nop, because activity type is not supported
		}

		return activity;
	  }

	  private string ExceptionStacktrace
	  {
		  get
		  {
			StringWriter stringWriter = new StringWriter();
			exception.printStackTrace(new PrintWriter(stringWriter));
			return stringWriter.ToString();
		  }
	  }

	  private ExecutionEntity fetchExecutionEntity(CommandContext commandContext, string executionId)
	  {
		return commandContext.ExecutionEntityManager.findExecutionById(executionId);
	  }

	}

}