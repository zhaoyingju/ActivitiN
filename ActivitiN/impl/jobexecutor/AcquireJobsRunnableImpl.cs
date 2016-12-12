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
namespace org.activiti.engine.impl.jobexecutor
{


	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class AcquireJobsRunnableImpl : AcquireJobsRunnable
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(AcquireJobsRunnableImpl));

	  protected internal readonly JobExecutor jobExecutor;

	  protected internal volatile bool isInterrupted = false;
	  protected internal volatile bool isJobAdded = false;
	  protected internal readonly object MONITOR = new object();
	  protected internal readonly AtomicBoolean isWaiting = new AtomicBoolean(false);

	  protected internal long millisToWait = 0;

	  public AcquireJobsRunnableImpl(JobExecutor jobExecutor)
	  {
		this.jobExecutor = jobExecutor;
	  }

	  public virtual void run()
	  {
		  lock (this)
		  {
			log.info("{} starting to acquire jobs", jobExecutor.Name);
        
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.activiti.engine.impl.interceptor.CommandExecutor commandExecutor = jobExecutor.getCommandExecutor();
			CommandExecutor commandExecutor = jobExecutor.CommandExecutor;
        
			while (!isInterrupted)
			{
			  isJobAdded = false;
			  int maxJobsPerAcquisition = jobExecutor.MaxJobsPerAcquisition;
        
			  try
			  {
				AcquiredJobs acquiredJobs = commandExecutor.execute(jobExecutor.AcquireJobsCmd);
        
				foreach (IList<string> jobIds in acquiredJobs.JobIdBatches)
				{
				  jobExecutor.executeJobs(jobIds);
				}
        
				// if all jobs were executed
				millisToWait = jobExecutor.getWaitTimeInMillis();
				int jobsAcquired = acquiredJobs.JobIdBatches.Count;
				if (jobsAcquired >= maxJobsPerAcquisition)
				{
				  millisToWait = 0;
				}
        
			  }
			  catch (ActivitiOptimisticLockingException optimisticLockingException)
			  {
				// See https://activiti.atlassian.net/browse/ACT-1390
				if (log.DebugEnabled)
				{
				  log.debug("Optimistic locking exception during job acquisition. If you have multiple job executors running against the same database, " + "this exception means that this thread tried to acquire a job, which already was acquired by another job executor acquisition thread." + "This is expected behavior in a clustered environment. " + "You can ignore this message if you indeed have multiple job executor acquisition threads running against the same database. " + "Exception message: {}", optimisticLockingException.Message);
				}
			  }
			  catch (Exception e)
			  {
				log.error("exception during job acquisition: {}", e.Message, e);
				millisToWait = jobExecutor.getWaitTimeInMillis();
			  }
        
			  if ((millisToWait > 0) && (!isJobAdded))
			  {
				try
				{
				  if (log.DebugEnabled)
				  {
					log.debug("job acquisition thread sleeping for {} millis", millisToWait);
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
					log.debug("job acquisition thread woke up");
				  }
				}
				catch (InterruptedException)
				{
				  if (log.DebugEnabled)
				  {
					log.debug("job acquisition wait interrupted");
				  }
				}
				finally
				{
				  isWaiting.set(false);
				}
			  }
			}
        
			log.info("{} stopped job acquisition", jobExecutor.Name);
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

	  public virtual void jobWasAdded()
	  {
		isJobAdded = true;
		if (isWaiting.compareAndSet(true, false))
		{
		  // ensures we only notify once
		  // I am OK with the race condition      
		  lock (MONITOR)
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