using System;
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
namespace org.activiti.engine.impl.asyncexecutor
{

	using AcquireTimerJobsCmd = org.activiti.engine.impl.cmd.AcquireTimerJobsCmd;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// 
	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	public class AcquireTimerJobsRunnable : Runnable
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(AcquireTimerJobsRunnable));

	  protected internal readonly AsyncExecutor asyncExecutor;

	  protected internal volatile bool isInterrupted = false;
	  protected internal readonly object MONITOR = new object();
	  protected internal readonly AtomicBoolean isWaiting = new AtomicBoolean(false);

	  protected internal long millisToWait = 0;

	  public AcquireTimerJobsRunnable(AsyncExecutor asyncExecutor)
	  {
		this.asyncExecutor = asyncExecutor;
	  }

	  public virtual void run()
	  {
		  lock (this)
		  {
			log.info("starting to acquire async jobs due");
        
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.activiti.engine.impl.interceptor.CommandExecutor commandExecutor = asyncExecutor.getCommandExecutor();
			CommandExecutor commandExecutor = asyncExecutor.CommandExecutor;
        
			while (!isInterrupted)
			{
        
			  try
			  {
				AcquiredJobEntities acquiredJobs = commandExecutor.execute(new AcquireTimerJobsCmd(asyncExecutor.LockOwner, asyncExecutor.TimerLockTimeInMillis, asyncExecutor.MaxTimerJobsPerAcquisition));
        
				bool allJobsSuccessfullyOffered = true;
				foreach (JobEntity job in acquiredJobs.Jobs)
				{
				  bool jobSuccessFullyOffered = asyncExecutor.executeAsyncJob(job);
				  if (!jobSuccessFullyOffered)
				  {
					allJobsSuccessfullyOffered = false;
				  }
				}
        
				// if all jobs were executed
				millisToWait = asyncExecutor.DefaultTimerJobAcquireWaitTimeInMillis;
				int jobsAcquired = acquiredJobs.size();
				if (jobsAcquired >= asyncExecutor.MaxTimerJobsPerAcquisition)
				{
				  millisToWait = 0;
				}
        
				// If the queue was full, we wait too (even if we got enough jobs back), as not overload the queue
				if (millisToWait == 0 && !allJobsSuccessfullyOffered)
				{
				  millisToWait = asyncExecutor.DefaultQueueSizeFullWaitTimeInMillis;
				}
        
			  }
			  catch (ActivitiOptimisticLockingException optimisticLockingException)
			  {
				if (log.DebugEnabled)
				{
				  log.debug("Optimistic locking exception during timer job acquisition. If you have multiple timer executors running against the same database, " + "this exception means that this thread tried to acquire a timer job, which already was acquired by another timer executor acquisition thread." + "This is expected behavior in a clustered environment. " + "You can ignore this message if you indeed have multiple timer executor acquisition threads running against the same database. " + "Exception message: {}", optimisticLockingException.Message);
				}
			  }
			  catch (Exception e)
			  {
				log.error("exception during timer job acquisition: {}", e.Message, e);
				millisToWait = asyncExecutor.DefaultTimerJobAcquireWaitTimeInMillis;
			  }
        
			  if (millisToWait > 0)
			  {
				try
				{
				  if (log.DebugEnabled)
				  {
					log.debug("timer job acquisition thread sleeping for {} millis", millisToWait);
				  }
				  lock (MONITOR)
				  {
					if (!isInterrupted)
					{
					  isWaiting.set(true);
					  Monitor.Wait(MONITOR, TimeSpan.FromMilliseconds(millisToWait));
					}
				  }
        
				  if (log.DebugEnabled)
				  {
					log.debug("timer job acquisition thread woke up");
				  }
				}
				catch (InterruptedException)
				{
				  if (log.DebugEnabled)
				  {
					log.debug("timer job acquisition wait interrupted");
				  }
				}
				finally
				{
				  isWaiting.set(false);
				}
			  }
			}
        
			log.info("stopped async job due acquisition");
		  }
	  }

	  public virtual void stop()
	  {
		lock (MONITOR)
		{
		  isInterrupted = true;
		  if (isWaiting.compareAndSet(true, false))
		  {
			  Monitor.PulseAll(MONITOR);
		  }
		}
	  }

	  public virtual long MillisToWait
	  {
		  get
		  {
			return millisToWait;
		  }
		  set
		  {
			this.millisToWait = value;
		  }
	  }

	}

}