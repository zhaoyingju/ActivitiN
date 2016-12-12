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


	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// @author Joram Barrez
	/// @author Tijs Rademakers
	/// </summary>
	public class DefaultAsyncJobExecutor : AbstractAsyncJobExecutor
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(DefaultAsyncJobExecutor));

	  /// <summary>
	  /// The minimal number of threads that are kept alive in the threadpool for job execution </summary>
	  protected internal int corePoolSize = 2;

	  /// <summary>
	  /// The maximum number of threads that are kept alive in the threadpool for job execution </summary>
	  protected internal int maxPoolSize = 10;

	  /// <summary>
	  /// The size of the queue on which jobs to be executed are placed </summary>
	  protected internal int queueSize = 100;

	  /// <summary>
	  /// The queue used for job execution work </summary>
	  protected internal BlockingQueue<Runnable> threadPoolQueue;

	  /// <summary>
	  /// The executor service used for job execution </summary>
	  protected internal ExecutorService executorService;

	  /// <summary>
	  /// The time (in seconds) that is waited to gracefully shut down the threadpool used for job execution </summary>
	  protected internal long secondsToWaitOnShutdown = 60L;

	  protected internal override bool executeAsyncJob(Runnable runnable)
	  {
		try
		{
		  executorService.execute(runnable);
		  return true;
		}
		catch (RejectedExecutionException)
		{
		  // When a RejectedExecutionException is caught, this means that the queue for holding the jobs 
		  // that are to be executed is full and can't store more.
		  // Return false so the job can be unlocked and (if wanted) the acquiring can be throttled.
		  return false;
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected Runnable createRunnableForJob(final org.activiti.engine.impl.persistence.entity.JobEntity job)
	  protected internal override Runnable createRunnableForJob(JobEntity job)
	  {
		return executeAsyncRunnableFactory.createExecuteAsyncRunnable(job, commandExecutor);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void unlockJob(final org.activiti.engine.impl.persistence.entity.JobEntity job, org.activiti.engine.impl.interceptor.CommandContext commandContext)
	  protected internal override void unlockJob(JobEntity job, CommandContext commandContext)
	  {
		commandContext.JobEntityManager.unacquireJob(job.Id);
	  }

	  protected internal override void startExecutingAsyncJobs()
	  {
		if (threadPoolQueue == null)
		{
		  log.info("Creating thread pool queue of size {}", queueSize);
		  threadPoolQueue = new ArrayBlockingQueue<Runnable>(queueSize);
		}

		if (executorService == null)
		{
		  log.info("Creating executor service with corePoolSize {}, maxPoolSize {} and keepAliveTime {}", corePoolSize, maxPoolSize, keepAliveTime);

		  executorService = new ThreadPoolExecutor(corePoolSize, maxPoolSize, keepAliveTime, TimeUnit.MILLISECONDS, threadPoolQueue);
		}

		startJobAcquisitionThread();
	  }

	  protected internal override void stopExecutingAsyncJobs()
	  {
		stopJobAcquisitionThread();

		// Ask the thread pool to finish and exit
		executorService.shutdown();

		// Waits for 1 minute to finish all currently executing jobs
		try
		{
		  if (!executorService.awaitTermination(secondsToWaitOnShutdown, TimeUnit.SECONDS))
		  {
			log.warn("Timeout during shutdown of async job executor. " + "The current running jobs could not end within " + secondsToWaitOnShutdown + " seconds after shutdown operation.");
		  }
		}
		catch (InterruptedException e)
		{
		  log.warn("Interrupted while shutting down the async job executor. ", e);
		}

		executorService = null;
	  }

	  public virtual int QueueSize
	  {
		  get
		  {
			return queueSize;
		  }
		  set
		  {
			this.queueSize = value;
		  }
	  }


	  public virtual int CorePoolSize
	  {
		  get
		  {
			return corePoolSize;
		  }
		  set
		  {
			this.corePoolSize = value;
		  }
	  }


	  public virtual int MaxPoolSize
	  {
		  get
		  {
			return maxPoolSize;
		  }
		  set
		  {
			this.maxPoolSize = value;
		  }
	  }


	  public virtual long SecondsToWaitOnShutdown
	  {
		  get
		  {
			return secondsToWaitOnShutdown;
		  }
		  set
		  {
			this.secondsToWaitOnShutdown = value;
		  }
	  }


	  public virtual BlockingQueue<Runnable> ThreadPoolQueue
	  {
		  get
		  {
			return threadPoolQueue;
		  }
		  set
		  {
			this.threadPoolQueue = value;
		  }
	  }


	  public virtual ExecutorService ExecutorService
	  {
		  get
		  {
			return executorService;
		  }
		  set
		  {
			this.executorService = value;
		  }
	  }

	}

}