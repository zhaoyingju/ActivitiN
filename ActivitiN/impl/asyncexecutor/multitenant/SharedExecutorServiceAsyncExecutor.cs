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
namespace org.activiti.engine.impl.asyncexecutor.multitenant
{


	using TenantInfoHolder = org.activiti.engine.impl.cfg.multitenant.TenantInfoHolder;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// Multi tenant <seealso cref="AsyncExecutor"/>.
	/// 
	/// For each tenant, there will be acquire threads, but only one <seealso cref="ExecutorService"/> will be used
	/// once the jobs are acquired.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class SharedExecutorServiceAsyncExecutor : DefaultAsyncJobExecutor, TenantAwareAsyncExecutor
	{

	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(SharedExecutorServiceAsyncExecutor));

	  protected internal TenantInfoHolder tenantInfoHolder;

	  protected internal IDictionary<string, Thread> timerJobAcquisitionThreads = new Dictionary<string, Thread>();
	  protected internal IDictionary<string, TenantAwareAcquireTimerJobsRunnable> timerJobAcquisitionRunnables = new Dictionary<string, TenantAwareAcquireTimerJobsRunnable>();

	  protected internal IDictionary<string, Thread> asyncJobAcquisitionThreads = new Dictionary<string, Thread>();
	  protected internal IDictionary<string, TenantAwareAcquireAsyncJobsDueRunnable> asyncJobAcquisitionRunnables = new Dictionary<string, TenantAwareAcquireAsyncJobsDueRunnable>();

	  public SharedExecutorServiceAsyncExecutor(TenantInfoHolder tenantInfoHolder)
	  {
		this.tenantInfoHolder = tenantInfoHolder;

		ExecuteAsyncRunnableFactory = new ExecuteAsyncRunnableFactoryAnonymousInnerClassHelper(this);
	  }

	  private class ExecuteAsyncRunnableFactoryAnonymousInnerClassHelper : ExecuteAsyncRunnableFactory
	  {
		  private readonly SharedExecutorServiceAsyncExecutor outerInstance;

		  public ExecuteAsyncRunnableFactoryAnonymousInnerClassHelper(SharedExecutorServiceAsyncExecutor outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public virtual Runnable createExecuteAsyncRunnable(JobEntity jobEntity, CommandExecutor commandExecutor)
		  {

			// Here, the runnable will be created by for example the acquire thread, which has already set the current id.
			// But it will be executed later on, by the executorService and thus we need to set it explicitely again then

			return new TenantAwareExecuteAsyncRunnable(jobEntity, commandExecutor, outerInstance.tenantInfoHolder, outerInstance.tenantInfoHolder.CurrentTenantId);
		  }

	  }

	  public override Set<string> TenantIds
	  {
		  get
		  {
			return timerJobAcquisitionThreads.Keys;
		  }
	  }

	  public virtual void addTenantAsyncExecutor(string tenantId, bool startExecutor)
	  {

		TenantAwareAcquireTimerJobsRunnable timerRunnable = new TenantAwareAcquireTimerJobsRunnable(this, tenantInfoHolder, tenantId);
		timerJobAcquisitionRunnables[tenantId] = timerRunnable;
		timerJobAcquisitionThreads[tenantId] = new Thread(timerRunnable);

		TenantAwareAcquireAsyncJobsDueRunnable asyncJobsRunnable = new TenantAwareAcquireAsyncJobsDueRunnable(this, tenantInfoHolder, tenantId);
		asyncJobAcquisitionRunnables[tenantId] = asyncJobsRunnable;
		asyncJobAcquisitionThreads[tenantId] = new Thread(asyncJobsRunnable);

		if (startExecutor)
		{
		  startTimerJobAcquisitionForTenant(tenantId);
		  startAsyncJobAcquisitionForTenant(tenantId);
		}
	  }

	  public override void removeTenantAsyncExecutor(string tenantId)
	  {
		stopThreadsForTenant(tenantId);
	  }

	  protected internal override void startJobAcquisitionThread()
	  {
		foreach (string tenantId in timerJobAcquisitionThreads.Keys)
		{
		  startTimerJobAcquisitionForTenant(tenantId);
		}

		foreach (string tenantId in asyncJobAcquisitionThreads.Keys)
		{
		  asyncJobAcquisitionThreads[tenantId].Start();
		}
	  }

	  protected internal virtual void startTimerJobAcquisitionForTenant(string tenantId)
	  {
		timerJobAcquisitionThreads[tenantId].Start();
	  }

	  protected internal virtual void startAsyncJobAcquisitionForTenant(string tenantId)
	  {
		asyncJobAcquisitionThreads[tenantId].Start();
	  }

	  protected internal override void stopJobAcquisitionThread()
	  {
		foreach (string tenantId in timerJobAcquisitionRunnables.Keys)
		{
		  stopThreadsForTenant(tenantId);
		}
	  }

	  protected internal virtual void stopThreadsForTenant(string tenantId)
	  {
		timerJobAcquisitionRunnables[tenantId].stop();
		asyncJobAcquisitionRunnables[tenantId].stop();

		try
		{
		  timerJobAcquisitionThreads[tenantId].Join();
		}
		catch (InterruptedException e)
		{
		  logger.warn("Interrupted while waiting for the timer job acquisition thread to terminate", e);
		}

		try
		{
		  asyncJobAcquisitionThreads[tenantId].Join();
		}
		catch (InterruptedException e)
		{
		  logger.warn("Interrupted while waiting for the timer job acquisition thread to terminate", e);
		}
	  }

	}

}