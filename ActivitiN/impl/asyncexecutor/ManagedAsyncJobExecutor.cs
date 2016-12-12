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


	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// Simple JSR-236 async job executor to allocate threads through <seealso cref="ManagedThreadFactory"/>. Falls back to <seealso cref="AsyncExecutor"/> 
	/// when a thread factory was not referenced in configuration.
	/// 
	/// In Java EE 7, all application servers should provide access to a <seealso cref="ManagedThreadFactory"/>.
	/// 
	/// @author Dimitris Mandalidis
	/// </summary>
	public class ManagedAsyncJobExecutor : DefaultAsyncJobExecutor
	{

	private static Logger log = LoggerFactory.getLogger(typeof(ManagedAsyncJobExecutor));

	  protected internal ManagedThreadFactory threadFactory;

	  public virtual ManagedThreadFactory ThreadFactory
	  {
		  get
		  {
			return threadFactory;
		  }
		  set
		  {
			this.threadFactory = value;
		  }
	  }



	  protected internal override void startExecutingAsyncJobs()
	  {
		if (threadFactory == null)
		{
		  log.warn("A managed thread factory was not found, falling back to self-managed threads");
		  base.startExecutingAsyncJobs();
		}
		else
		{
		  if (threadPoolQueue == null)
		  {
			log.info("Creating thread pool queue of size {}", queueSize);
			threadPoolQueue = new ArrayBlockingQueue<Runnable>(queueSize);
		  }

		  if (executorService == null)
		  {
			log.info("Creating executor service with corePoolSize {}, maxPoolSize {} and keepAliveTime {}", corePoolSize, maxPoolSize, keepAliveTime);

			ThreadPoolExecutor threadPoolExecutor = new ThreadPoolExecutor(corePoolSize, maxPoolSize, keepAliveTime, TimeUnit.MILLISECONDS, threadPoolQueue, threadFactory);
			threadPoolExecutor.RejectedExecutionHandler = new ThreadPoolExecutor.CallerRunsPolicy();
			executorService = threadPoolExecutor;

		  }

		  startJobAcquisitionThread();
		}
	  }
	}

}