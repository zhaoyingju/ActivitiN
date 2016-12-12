using System;
using System.Collections.Generic;
using System.Threading;

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

namespace org.activiti.engine.impl.persistence.entity
{


	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using AsyncExecutor = org.activiti.engine.impl.asyncexecutor.AsyncExecutor;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using TransactionListener = org.activiti.engine.impl.cfg.TransactionListener;
	using TransactionState = org.activiti.engine.impl.cfg.TransactionState;
	using Context = org.activiti.engine.impl.context.Context;
	using AsyncJobAddedNotification = org.activiti.engine.impl.jobexecutor.AsyncJobAddedNotification;
	using JobAddedNotification = org.activiti.engine.impl.jobexecutor.JobAddedNotification;
	using JobExecutor = org.activiti.engine.impl.jobexecutor.JobExecutor;
	using Job = org.activiti.engine.runtime.Job;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public class JobEntityManager : AbstractManager
	{

	  public virtual void send(MessageEntity message)
	  {

		  ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

		  if (processEngineConfiguration.AsyncExecutorEnabled)
		  {

			  // If the async executor is enabled, we need to set the duedate of the job to the current date + the default lock time. 
			  // This is cope with the case where the async job executor or the process engine goes down
			  // before executing the job. This way, other async job executors can pick the job up after the max lock time.
			  DateTime dueDate = new DateTime(processEngineConfiguration.Clock.CurrentTime.Ticks + processEngineConfiguration.AsyncExecutor.AsyncJobLockTimeInMillis);
			  message.Duedate = dueDate;
			  message.LockExpirationTime = null; // was set before, but to be quickly picked up needs to be set to null

		  }
		  else if (!processEngineConfiguration.JobExecutorActivate)
		  {

			  // If the async executor is disabled AND there is no old school job executor,
			  // The job needs to be picked up as soon as possible. So the due date is now set to the current time
			  message.Duedate = processEngineConfiguration.Clock.CurrentTime;
			  message.LockExpirationTime = null; // was set before, but to be quickly picked up needs to be set to null
		  }

		message.insert();
		if (processEngineConfiguration.AsyncExecutorEnabled)
		{
		  hintAsyncExecutor(message);
		}
		else
		{
		  hintJobExecutor(message);
		}
	  }

	  public virtual void schedule(TimerEntity timer)
	  {
		DateTime duedate = timer.Duedate;
		if (duedate == null)
		{
		  throw new ActivitiIllegalArgumentException("duedate is null");
		}

		timer.insert();

		ProcessEngineConfiguration engineConfiguration = Context.ProcessEngineConfiguration;
		if (engineConfiguration.AsyncExecutorEnabled == false && timer.Duedate.Ticks <= (engineConfiguration.Clock.CurrentTime.Ticks))
		{

		  hintJobExecutor(timer);
		}
	  }

	  /*"Not used anymore. Will be removed in a future release." */
	  [Obsolete()]
	  public virtual void retryAsyncJob(JobEntity job)
	  {
		AsyncExecutor asyncExecutor = Context.ProcessEngineConfiguration.AsyncExecutor;
		try
		{

			// If a job has to be retried, we wait for a certain amount of time,
			// otherwise the job will be continuously be retried without delay (and thus seriously stressing the database).
			Thread.Sleep(asyncExecutor.RetryWaitTimeInMillis);

		}
		catch (InterruptedException)
		{
		}
		asyncExecutor.executeAsyncJob(job);
	  }

	  protected internal virtual void hintAsyncExecutor(JobEntity job)
	  {
		AsyncExecutor asyncExecutor = Context.ProcessEngineConfiguration.AsyncExecutor;

		// notify job executor:      
		TransactionListener transactionListener = new AsyncJobAddedNotification(job, asyncExecutor);
		Context.CommandContext.TransactionContext.addTransactionListener(TransactionState.COMMITTED, transactionListener);
	  }

	  protected internal virtual void hintJobExecutor(JobEntity job)
	  {
		JobExecutor jobExecutor = Context.ProcessEngineConfiguration.JobExecutor;

		// notify job executor:      
		TransactionListener transactionListener = new JobAddedNotification(jobExecutor);
		Context.CommandContext.TransactionContext.addTransactionListener(TransactionState.COMMITTED, transactionListener);
	  }

	  public virtual void cancelTimers(ExecutionEntity execution)
	  {
		IList<TimerEntity> timers = Context.CommandContext.JobEntityManager.findTimersByExecutionId(execution.Id);

		foreach (TimerEntity timer in timers)
		{
		  if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.JOB_CANCELED, timer));
		  }
		  timer.delete();
		}
	  }

	  public virtual JobEntity findJobById(string jobId)
	  {
		return (JobEntity) DbSqlSession.selectOne("selectJob", jobId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<JobEntity> findNextJobsToExecute(org.activiti.engine.impl.Page page)
	  public virtual IList<JobEntity> findNextJobsToExecute(Page page)
	  {
		ProcessEngineConfiguration processEngineConfig = Context.ProcessEngineConfiguration;
		DateTime now = processEngineConfig.Clock.CurrentTime;
		return DbSqlSession.selectList("selectNextJobsToExecute", now, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<JobEntity> findNextTimerJobsToExecute(org.activiti.engine.impl.Page page)
	  public virtual IList<JobEntity> findNextTimerJobsToExecute(Page page)
	  {
		ProcessEngineConfiguration processEngineConfig = Context.ProcessEngineConfiguration;
		DateTime now = processEngineConfig.Clock.CurrentTime;
		return DbSqlSession.selectList("selectNextTimerJobsToExecute", now, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<JobEntity> findAsyncJobsDueToExecute(org.activiti.engine.impl.Page page)
	  public virtual IList<JobEntity> findAsyncJobsDueToExecute(Page page)
	  {
		ProcessEngineConfiguration processEngineConfig = Context.ProcessEngineConfiguration;
		DateTime now = processEngineConfig.Clock.CurrentTime;
		return DbSqlSession.selectList("selectAsyncJobsDueToExecute", now, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<JobEntity> findJobsByLockOwner(String lockOwner, int start, int maxNrOfJobs)
	  public virtual IList<JobEntity> findJobsByLockOwner(string lockOwner, int start, int maxNrOfJobs)
	  {
		  return DbSqlSession.selectList("selectJobsByLockOwner", lockOwner, start, maxNrOfJobs);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.runtime.Job> findJobsByExecutionId(String executionId)
	  public virtual IList<Job> findJobsByExecutionId(string executionId)
	  {
		return DbSqlSession.selectList("selectJobsByExecutionId", executionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<JobEntity> findExclusiveJobsToExecute(String processInstanceId)
	  public virtual IList<JobEntity> findExclusiveJobsToExecute(string processInstanceId)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["pid"] = processInstanceId;
		@params["now"] = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		return DbSqlSession.selectList("selectExclusiveJobsToExecute", @params);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<TimerEntity> findUnlockedTimersByDuedate(java.util.Date duedate, org.activiti.engine.impl.Page page)
	  public virtual IList<TimerEntity> findUnlockedTimersByDuedate(DateTime duedate, Page page)
	  {
		const string query = "selectUnlockedTimersByDuedate";
		return DbSqlSession.selectList(query, duedate, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<TimerEntity> findTimersByExecutionId(String executionId)
	  public virtual IList<TimerEntity> findTimersByExecutionId(string executionId)
	  {
		return DbSqlSession.selectList("selectTimersByExecutionId", executionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.runtime.Job> findJobsByQueryCriteria(org.activiti.engine.impl.JobQueryImpl jobQuery, org.activiti.engine.impl.Page page)
	  public virtual IList<Job> findJobsByQueryCriteria(JobQueryImpl jobQuery, Page page)
	  {
		const string query = "selectJobByQueryCriteria";
		return DbSqlSession.selectList(query, jobQuery, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.runtime.Job> findJobsByTypeAndProcessDefinitionKeyNoTenantId(String jobHandlerType, String processDefinitionKey)
	  public virtual IList<Job> findJobsByTypeAndProcessDefinitionKeyNoTenantId(string jobHandlerType, string processDefinitionKey)
	  {
		   IDictionary<string, string> @params = new Dictionary<string, string>(2);
		 @params["handlerType"] = jobHandlerType;
		 @params["processDefinitionKey"] = processDefinitionKey;
		 return DbSqlSession.selectList("selectJobByTypeAndProcessDefinitionKeyNoTenantId", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.runtime.Job> findJobsByTypeAndProcessDefinitionKeyAndTenantId(String jobHandlerType, String processDefinitionKey, String tenantId)
	  public virtual IList<Job> findJobsByTypeAndProcessDefinitionKeyAndTenantId(string jobHandlerType, string processDefinitionKey, string tenantId)
	  {
		   IDictionary<string, string> @params = new Dictionary<string, string>(3);
		 @params["handlerType"] = jobHandlerType;
		 @params["processDefinitionKey"] = processDefinitionKey;
		 @params["tenantId"] = tenantId;
		 return DbSqlSession.selectList("selectJobByTypeAndProcessDefinitionKeyAndTenantId", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.runtime.Job> findJobsByTypeAndProcessDefinitionId(String jobHandlerType, String processDefinitionId)
	  public virtual IList<Job> findJobsByTypeAndProcessDefinitionId(string jobHandlerType, string processDefinitionId)
	  {
		   IDictionary<string, string> @params = new Dictionary<string, string>(2);
		 @params["handlerType"] = jobHandlerType;
		 @params["processDefinitionId"] = processDefinitionId;
		 return DbSqlSession.selectList("selectJobByTypeAndProcessDefinitionId", @params);
	  }

	  public virtual void unacquireJob(string jobId)
	  {
		  IDictionary<string, object> @params = new Dictionary<string, object>(2);
		  @params["id"] = jobId;
		  @params["dueDate"] = new DateTime(ProcessEngineConfiguration.Clock.CurrentTime.Ticks);
		  DbSqlSession.update("unacquireJob", @params);
	  }

	  public virtual long findJobCountByQueryCriteria(JobQueryImpl jobQuery)
	  {
		return (long?) DbSqlSession.selectOne("selectJobCountByQueryCriteria", jobQuery);
	  }

	  public virtual void updateJobTenantIdForDeployment(string deploymentId, string newTenantId)
	  {
		  Dictionary<string, object> @params = new Dictionary<string, object>();
		  @params["deploymentId"] = deploymentId;
		  @params["tenantId"] = newTenantId;
		  DbSqlSession.update("updateJobTenantIdForDeployment", @params);
	  }

	  public virtual int updateJobLockForAllJobs(string lockOwner, DateTime expirationTime)
	  {
		Dictionary<string, object> @params = new Dictionary<string, object>();
		@params["lockOwner"] = lockOwner;
		@params["lockExpirationTime"] = expirationTime;
		@params["dueDate"] = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		return DbSqlSession.update("updateJobLockForAllJobs", @params);
	  }

	}

}