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
namespace org.activiti.engine.impl.jobexecutor
{


	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// <para>This is a simple implementation of the <seealso cref="JobExecutor"/> using self-managed
	/// threads for performing background work.</para>
	/// 
	/// <para>This implementation uses a <seealso cref="ThreadPoolExecutor"/> backed by a queue to which
	/// work is submitted.</para>
	/// 
	/// <para><em>NOTE: use this class in environments in which self-management of threads 
	/// is permitted. Consider using a different thread-management strategy in 
	/// J(2)EE-Environments.</em></para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class DefaultJobExecutor : JobExecutor
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(DefaultJobExecutor));

	  protected internal int queueSize = 3;
	  protected internal int corePoolSize = 3;
	  protected internal int maxPoolSize = 10;
	  protected internal long keepAliveTime = 0L;

	  protected internal BlockingQueue<Runnable> threadPoolQueue;
	  protected internal ThreadPoolExecutor threadPoolExecutor;

	  protected internal override void startExecutingJobs()
	  {
		if (threadPoolQueue == null)
		{
		  threadPoolQueue = new ArrayBlockingQueue<Runnable>(queueSize);
		}
		if (threadPoolExecutor == null)
		{
		  threadPoolExecutor = new ThreadPoolExecutor(corePoolSize, maxPoolSize, keepAliveTime, TimeUnit.MILLISECONDS, threadPoolQueue);
		  threadPoolExecutor.RejectedExecutionHandler = new ThreadPoolExecutor.AbortPolicy();
		}
		startJobAcquisitionThread();
	  }

	  protected internal override void stopExecutingJobs()
	  {
		stopJobAcquisitionThread();

		// Ask the thread pool to finish and exit
		threadPoolExecutor.shutdown();

		// Waits for 1 minute to finish all currently executing jobs
		try
		{
		  if (!threadPoolExecutor.awaitTermination(60L, TimeUnit.SECONDS))
		  {
			log.warn("Timeout during shutdown of job executor. " + "The current running jobs could not end within 60 seconds after shutdown operation.");
		  }
		}
		catch (InterruptedException e)
		{
		  log.warn("Interrupted while shutting down the job executor. ", e);
		}

		threadPoolExecutor = null;
	  }

	  public override void executeJobs(IList<string> jobIds)
	  {
		try
		{
		  threadPoolExecutor.execute(new ExecuteJobsRunnable(this, jobIds));
		}
		catch (RejectedExecutionException)
		{
		  rejectedJobsHandler.jobsRejected(this, jobIds);
		}
	  }

	  // getters and setters ////////////////////////////////////////////////////// 

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


	  public virtual long KeepAliveTime
	  {
		  get
		  {
				return keepAliveTime;
		  }
		  set
		  {
				this.keepAliveTime = value;
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


	  public virtual ThreadPoolExecutor ThreadPoolExecutor
	  {
		  get
		  {
			return threadPoolExecutor;
		  }
		  set
		  {
			this.threadPoolExecutor = value;
		  }
	  }


	}


}