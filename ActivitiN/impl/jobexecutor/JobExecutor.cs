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


	using AcquireJobsCmd = org.activiti.engine.impl.cmd.AcquireJobsCmd;
	using org.activiti.engine.impl.interceptor;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using ClockReader = org.activiti.engine.runtime.ClockReader;
	using Job = org.activiti.engine.runtime.Job;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// <para>Interface to the work management component of activiti.</para>
	/// 
	/// <para>This component is responsible for performing all background work 
	/// (<seealso cref="Job Jobs"/>) scheduled by activiti.</para>
	/// 
	/// <para>You should generally only have one of these per Activiti instance (process 
	/// engine) in a JVM.
	/// In clustered situations, you can have multiple of these running against the
	/// same queue + pending job list.</para>
	/// 
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public abstract class JobExecutor
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(JobExecutor));

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  protected internal string name = "JobExecutor[" + this.GetType().FullName + "]";
	  protected internal CommandExecutor commandExecutor;
	  protected internal Command<AcquiredJobs> acquireJobsCmd;
	  protected internal AcquireJobsRunnable acquireJobsRunnable;
	  protected internal RejectedJobsHandler rejectedJobsHandler;
	  protected internal Thread jobAcquisitionThread;

	  protected internal bool isAutoActivate = false;
	  protected internal bool isActive = false;

	  /// <summary>
	  /// To avoid deadlocks, the default for this is one.
	  /// This way, in a clustered setup, multiple job executors can acquire jobs
	  /// without creating a deadlock due to fetching multiple jobs at once and
	  /// trying to lock them all at once.
	  /// 
	  /// In a non-clustered setup, this setting can be changed to any value > 0
	  /// without problems.
	  /// 
	  /// See https://activiti.atlassian.net/browse/ACT-1879 for more information.
	  /// </summary>
	  protected internal int maxJobsPerAcquisition = 1;
	  protected internal long waitTimeInMillis = 5000L;
	  protected internal string lockOwner = UUID.randomUUID().ToString();
	  protected internal int lockTimeInMillis = 5 * 60 * 1000;
	  protected internal ClockReader clockReader;

	  /// <summary>
	  /// Starts the job executor </summary>
	  public virtual void start()
	  {
		if (isActive)
		{
		  return;
		}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		log.info("Starting up the JobExecutor[{}].", this.GetType().FullName);
		ensureInitialization();
		startExecutingJobs();
		isActive = true;
	  }

	  /// <summary>
	  /// Shuts down the whole job executor </summary>
	  public virtual void shutdown()
	  {
		  lock (this)
		  {
			if (!isActive)
			{
			  return;
			}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			log.info("Shutting down the JobExecutor[{}].", this.GetType().FullName);
			acquireJobsRunnable.stop();
			stopExecutingJobs();
			ensureCleanup();
			isActive = false;
		  }
	  }

	  /// <summary>
	  /// Possibility to ensure everything is nicely initialized before starting the threads </summary>
	  protected internal virtual void ensureInitialization()
	  {
		  if (acquireJobsCmd == null)
		  {
			  acquireJobsCmd = new AcquireJobsCmd(this);
		  }
		  if (acquireJobsRunnable == null)
		  {
			  acquireJobsRunnable = new AcquireJobsRunnableImpl(this);
		  }
	  }

	  /// <summary>
	  /// Possibility to clean up resources </summary>
	  protected internal virtual void ensureCleanup()
	  {
		acquireJobsCmd = null;
		acquireJobsRunnable = null;
	  }

	  /// <summary>
	  /// Called when a new job was added by the process engine to which
	  /// this job executor belongs. This is a hint, that for example
	  /// the acquiring needs to start again when it would be sleeping.
	  /// </summary>
	  public virtual void jobWasAdded()
	  {
		if (isActive)
		{
		  acquireJobsRunnable.jobWasAdded();
		}
	  }

	  /// <summary>
	  /// Starts the acquisition thread </summary>
	  protected internal virtual void startJobAcquisitionThread()
	  {
			if (jobAcquisitionThread == null)
			{
				jobAcquisitionThread = new Thread(acquireJobsRunnable);
			}
			jobAcquisitionThread.Start();
	  }

	  /// <summary>
	  /// Stops the acquisition thread </summary>
		protected internal virtual void stopJobAcquisitionThread()
		{
			try
			{
				jobAcquisitionThread.Join();
			}
			catch (InterruptedException e)
			{
				log.warn("Interrupted while waiting for the job Acquisition thread to terminate", e);
			}
			jobAcquisitionThread = null;
		}

	  /* Need to be implemented by concrete subclasses */

		public abstract void executeJobs(IList<string> jobIds);
	  protected internal abstract void startExecutingJobs();
	  protected internal abstract void stopExecutingJobs();

	  /* Can be overridden by subclasses if wanted */
	  public virtual void jobDone(JobEntity job)
	  {
		  // Default: do nothing
	  }

	  /* Can be overridden by subclasses if wanted */
	  public virtual void jobDone(string jobId)
	  {
		  // Default: do nothing
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual CommandExecutor CommandExecutor
	  {
		  get
		  {
			return commandExecutor;
		  }
		  set
		  {
			this.commandExecutor = value;
		  }
	  }

	  public virtual long getWaitTimeInMillis()
	  {
		return waitTimeInMillis;
	  }

	  public virtual void setWaitTimeInMillis(int waitTimeInMillis)
	  {
		this.waitTimeInMillis = waitTimeInMillis;
	  }

	  public virtual int LockTimeInMillis
	  {
		  get
		  {
			return lockTimeInMillis;
		  }
		  set
		  {
			this.lockTimeInMillis = value;
		  }
	  }


	  public virtual string LockOwner
	  {
		  get
		  {
			return lockOwner;
		  }
		  set
		  {
			this.lockOwner = value;
		  }
	  }


	  public virtual bool AutoActivate
	  {
		  get
		  {
			return isAutoActivate;
		  }
		  set
		  {
			this.isAutoActivate = value;
		  }
	  }



	  public virtual int MaxJobsPerAcquisition
	  {
		  get
		  {
			return maxJobsPerAcquisition;
		  }
		  set
		  {
			this.maxJobsPerAcquisition = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual Command<AcquiredJobs> AcquireJobsCmd
	  {
		  get
		  {
			return acquireJobsCmd;
		  }
		  set
		  {
			this.acquireJobsCmd = value;
		  }
	  }


	  public virtual AcquireJobsRunnable getAcquireJobsRunnable()
	  {
			return acquireJobsRunnable;
	  }

		public virtual void setAcquireJobsRunnable(AcquireJobsRunnable acquireJobsRunnable)
		{
			this.acquireJobsRunnable = acquireJobsRunnable;
		}

		public virtual bool Active
		{
			get
			{
			return isActive;
			}
		}

	  public virtual RejectedJobsHandler getRejectedJobsHandler()
	  {
		return rejectedJobsHandler;
	  }

	  public virtual void setRejectedJobsHandler(RejectedJobsHandler rejectedJobsHandler)
	  {
		this.rejectedJobsHandler = rejectedJobsHandler;
	  }

	  public virtual DateTime CurrentTime
	  {
		  get
		  {
			return clockReader.CurrentTime;
		  }
	  }

	  public virtual ClockReader ClockReader
	  {
		  set
		  {
			this.clockReader = value;
		  }
	  }
	}

}