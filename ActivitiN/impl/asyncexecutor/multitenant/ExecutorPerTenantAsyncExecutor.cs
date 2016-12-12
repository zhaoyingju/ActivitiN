using System.Collections.Generic;

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

namespace org.activiti.engine.impl.asyncexecutor.multitenant
{


	using TenantInfoHolder = org.activiti.engine.impl.cfg.multitenant.TenantInfoHolder;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// An <seealso cref="AsyncExecutor"/> that has one <seealso cref="AsyncExecutor"/> per tenant.
	/// So each tenant has its own acquiring threads and it's own threadpool for executing jobs.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class ExecutorPerTenantAsyncExecutor : TenantAwareAsyncExecutor
	{

	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(ExecutorPerTenantAsyncExecutor));

	  protected internal TenantInfoHolder tenantInfoHolder;
	  protected internal TenantAwareAsyncExecutorFactory tenantAwareAyncExecutorFactory;

	  protected internal IDictionary<string, AsyncExecutor> tenantExecutors = new Dictionary<string, AsyncExecutor>();

	  protected internal CommandExecutor commandExecutor;
	  protected internal bool active;
	  protected internal bool autoActivate;

	  public ExecutorPerTenantAsyncExecutor(TenantInfoHolder tenantInfoHolder) : this(tenantInfoHolder, null)
	  {
	  }

	  public ExecutorPerTenantAsyncExecutor(TenantInfoHolder tenantInfoHolder, TenantAwareAsyncExecutorFactory tenantAwareAyncExecutorFactory)
	  {
		this.tenantInfoHolder = tenantInfoHolder;
		this.tenantAwareAyncExecutorFactory = tenantAwareAyncExecutorFactory;
	  }

	  public override Set<string> TenantIds
	  {
		  get
		  {
			return tenantExecutors.Keys;
		  }
	  }

	  public virtual void addTenantAsyncExecutor(string tenantId, bool startExecutor)
	  {
		AsyncExecutor tenantExecutor = null;

		if (tenantAwareAyncExecutorFactory == null)
		{
		  tenantExecutor = new DefaultAsyncJobExecutor();
		}
		else
		{
		  tenantExecutor = tenantAwareAyncExecutorFactory.createAsyncExecutor(tenantId);
		}

		if (tenantExecutor is DefaultAsyncJobExecutor)
		{
		  DefaultAsyncJobExecutor defaultAsyncJobExecutor = (DefaultAsyncJobExecutor) tenantExecutor;
		  defaultAsyncJobExecutor.AsyncJobsDueRunnable = new TenantAwareAcquireAsyncJobsDueRunnable(defaultAsyncJobExecutor, tenantInfoHolder, tenantId);
		  defaultAsyncJobExecutor.TimerJobRunnable = new TenantAwareAcquireTimerJobsRunnable(defaultAsyncJobExecutor, tenantInfoHolder, tenantId);
		  defaultAsyncJobExecutor.ExecuteAsyncRunnableFactory = new TenantAwareExecuteAsyncRunnableFactory(tenantInfoHolder, tenantId);
		}

		tenantExecutor.CommandExecutor = commandExecutor; // Needs to be done for job executors created after boot. Doesn't hurt on boot.

		tenantExecutors[tenantId] = tenantExecutor;

		if (startExecutor)
		{
		  tenantExecutor.start();
		}
	  }

	  public override void removeTenantAsyncExecutor(string tenantId)
	  {
		shutdownTenantExecutor(tenantId);
		tenantExecutors.Remove(tenantId);
	  }

	  protected internal virtual AsyncExecutor determineAsyncExecutor()
	  {
		return tenantExecutors[tenantInfoHolder.CurrentTenantId];
	  }

	  public virtual bool executeAsyncJob(JobEntity job)
	  {
		return determineAsyncExecutor().executeAsyncJob(job);
	  }

	  public virtual CommandExecutor CommandExecutor
	  {
		  set
		  {
			this.commandExecutor = value;
			foreach (AsyncExecutor asyncExecutor in tenantExecutors.Values)
			{
			  asyncExecutor.CommandExecutor = value;
			}
		  }
		  get
		  {
			// Should never be accessed on this class, should be accessed on the actual AsyncExecutor
			throw new System.NotSupportedException();
		  }
	  }


	  public virtual bool AutoActivate
	  {
		  get
		  {
			return autoActivate;
		  }
		  set
		  {
			autoActivate = value;
		  }
	  }


	  public virtual bool Active
	  {
		  get
		  {
			return active;
		  }
	  }

	  public virtual void start()
	  {
		foreach (AsyncExecutor asyncExecutor in tenantExecutors.Values)
		{
		  asyncExecutor.start();
		}
		active = true;
	  }

	  public virtual void shutdown()
	  {
		  lock (this)
		  {
			foreach (string tenantId in tenantExecutors.Keys)
			{
			  shutdownTenantExecutor(tenantId);
			}
			active = false;
		  }
	  }

	  protected internal virtual void shutdownTenantExecutor(string tenantId)
	  {
		logger.info("Shutting down async executor for tenant " + tenantId);
		tenantExecutors[tenantId].shutdown();
	  }

	  public virtual string LockOwner
	  {
		  get
		  {
			return determineAsyncExecutor().LockOwner;
		  }
	  }

	  public virtual int TimerLockTimeInMillis
	  {
		  get
		  {
			return determineAsyncExecutor().TimerLockTimeInMillis;
		  }
		  set
		  {
			foreach (AsyncExecutor asyncExecutor in tenantExecutors.Values)
			{
			  asyncExecutor.TimerLockTimeInMillis = value;
			}
		  }
	  }


	  public virtual int AsyncJobLockTimeInMillis
	  {
		  get
		  {
			return determineAsyncExecutor().AsyncJobLockTimeInMillis;
		  }
		  set
		  {
			foreach (AsyncExecutor asyncExecutor in tenantExecutors.Values)
			{
			  asyncExecutor.AsyncJobLockTimeInMillis = value;
			}
		  }
	  }


	  public virtual int DefaultTimerJobAcquireWaitTimeInMillis
	  {
		  get
		  {
			return determineAsyncExecutor().DefaultTimerJobAcquireWaitTimeInMillis;
		  }
		  set
		  {
			foreach (AsyncExecutor asyncExecutor in tenantExecutors.Values)
			{
			  asyncExecutor.DefaultTimerJobAcquireWaitTimeInMillis = value;
			}
		  }
	  }


	  public virtual int DefaultAsyncJobAcquireWaitTimeInMillis
	  {
		  get
		  {
			return determineAsyncExecutor().DefaultAsyncJobAcquireWaitTimeInMillis;
		  }
		  set
		  {
			foreach (AsyncExecutor asyncExecutor in tenantExecutors.Values)
			{
			  asyncExecutor.DefaultAsyncJobAcquireWaitTimeInMillis = value;
			}
		  }
	  }


	  public override int DefaultQueueSizeFullWaitTimeInMillis
	  {
		  get
		  {
			return determineAsyncExecutor().DefaultQueueSizeFullWaitTimeInMillis;
		  }
		  set
		  {
			foreach (AsyncExecutor asyncExecutor in tenantExecutors.Values)
			{
			  asyncExecutor.DefaultQueueSizeFullWaitTimeInMillis = value;
			}
		  }
	  }


	  public virtual int MaxAsyncJobsDuePerAcquisition
	  {
		  get
		  {
			return determineAsyncExecutor().MaxAsyncJobsDuePerAcquisition;
		  }
		  set
		  {
			foreach (AsyncExecutor asyncExecutor in tenantExecutors.Values)
			{
			  asyncExecutor.MaxAsyncJobsDuePerAcquisition = value;
			}
		  }
	  }


	  public virtual int MaxTimerJobsPerAcquisition
	  {
		  get
		  {
			return determineAsyncExecutor().MaxTimerJobsPerAcquisition;
		  }
		  set
		  {
			foreach (AsyncExecutor asyncExecutor in tenantExecutors.Values)
			{
			  asyncExecutor.MaxTimerJobsPerAcquisition = value;
			}
		  }
	  }


	  public virtual int RetryWaitTimeInMillis
	  {
		  get
		  {
			return determineAsyncExecutor().RetryWaitTimeInMillis;
		  }
		  set
		  {
			foreach (AsyncExecutor asyncExecutor in tenantExecutors.Values)
			{
			  asyncExecutor.RetryWaitTimeInMillis = value;
			}
		  }
	  }


	}

}