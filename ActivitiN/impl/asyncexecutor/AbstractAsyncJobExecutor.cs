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
namespace org.activiti.engine.impl.asyncexecutor
{


	using Context = org.activiti.engine.impl.context.Context;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// @author Joram Barrez
	/// @author Tijs Rademakers
	/// </summary>
	public abstract class AbstractAsyncJobExecutor : AsyncExecutor
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(AbstractAsyncJobExecutor));

	  /// <summary>
	  /// The time (in milliseconds) a thread used for job execution must be kept alive before it is
	  /// destroyed. Default setting is 0. Having a non-default setting of 0 takes resources,
	  /// but in the case of many job executions it avoids creating new threads all the time. 
	  /// </summary>
	  protected internal long keepAliveTime = 5000L;

	  protected internal Thread timerJobAcquisitionThread;
	  protected internal Thread asyncJobAcquisitionThread;
	  protected internal AcquireTimerJobsRunnable timerJobRunnable;
	  protected internal AcquireAsyncJobsDueRunnable asyncJobsDueRunnable;

	  protected internal ExecuteAsyncRunnableFactory executeAsyncRunnableFactory;

	  protected internal bool isAutoActivate = false;
	  protected internal bool isActive = false;

	  protected internal int maxTimerJobsPerAcquisition = 1;
	  protected internal int maxAsyncJobsDuePerAcquisition = 1;
	  protected internal int defaultTimerJobAcquireWaitTimeInMillis = 10 * 1000;
	  protected internal int defaultAsyncJobAcquireWaitTimeInMillis = 10 * 1000;
	  protected internal int defaultQueueSizeFullWaitTime = 0;

	  protected internal string lockOwner = UUID.randomUUID().ToString();
	  protected internal int timerLockTimeInMillis = 5 * 60 * 1000;
	  protected internal int asyncJobLockTimeInMillis = 5 * 60 * 1000;
	  protected internal int retryWaitTimeInMillis = 500;

	  // Job queue used when async executor is not yet started and jobs are already added.
	  // This is mainly used for testing purpose.
	  protected internal LinkedList<JobEntity> temporaryJobQueue = new LinkedList<JobEntity>();

	  protected internal CommandExecutor commandExecutor;

	  public virtual bool executeAsyncJob(JobEntity job)
	  {
		if (isActive)
		{
		  Runnable runnable = createRunnableForJob(job);
		  bool result = executeAsyncJob(runnable);
		  if (!result)
		  {
			doUnlockJob(job);
		  }
		  return result; // false indicates that the job was rejected.
		}
		else
		{
		  temporaryJobQueue.AddLast(job);
		  return true;
		}
	  }

	  protected internal abstract bool executeAsyncJob(Runnable runnable);

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void doUnlockJob(final org.activiti.engine.impl.persistence.entity.JobEntity job)
	  protected internal virtual void doUnlockJob(JobEntity job)
	  {
		// The job will now be 'unlocked', meaning that the lock owner/time is set to null,
		// so other executors can pick the job up (or this async executor, the next time the 
		// acquire query is executed.

		// This can happen while already in a command context (for example in a transaction listener
		// after the async executor has been hinted that a new async job is created)
		// or not (when executed in the aquire thread runnable)
		CommandContext commandContext = Context.CommandContext;
		if (commandContext != null)
		{
		  unlockJob(job, commandContext);
		}
		else
		{
		  commandExecutor.execute(new CommandAnonymousInnerClassHelper(this, job, commandContext));
		}
	  }

	  private class CommandAnonymousInnerClassHelper : Command<Void>
	  {
		  private readonly AbstractAsyncJobExecutor outerInstance;

		  private JobEntity job;
		  private CommandContext commandContext;

		  public CommandAnonymousInnerClassHelper(AbstractAsyncJobExecutor outerInstance, JobEntity job, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.job = job;
			  this.commandContext = commandContext;
		  }

		  public virtual Void execute(CommandContext commandContext)
		  {
			outerInstance.unlockJob(job, commandContext);
			return null;
		  }
	  }

	  protected internal virtual void unlockJob(JobEntity job, CommandContext commandContext)
	  {
		commandContext.JobEntityManager.unacquireJob(job.Id);
	  }

	  protected internal virtual Runnable createRunnableForJob(JobEntity job)
	  {
		return executeAsyncRunnableFactory.createExecuteAsyncRunnable(job, commandExecutor);
	  }

	  /// <summary>
	  /// Starts the async executor </summary>
	  public virtual void start()
	  {
		if (isActive)
		{
		  return;
		}

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		log.info("Starting up the default async job executor [{}].", this.GetType().FullName);
		initialize();
		startExecutingAsyncJobs();

		isActive = true;

		while (temporaryJobQueue.Count == 0 == false)
		{
		  JobEntity job = temporaryJobQueue.pop();
		  executeAsyncJob(job);
		}
		isActive = true;
	  }

	  protected internal virtual void initialize()
	  {
		if (timerJobRunnable == null)
		{
		  timerJobRunnable = new AcquireTimerJobsRunnable(this);
		}
		if (asyncJobsDueRunnable == null)
		{
		  asyncJobsDueRunnable = new AcquireAsyncJobsDueRunnable(this);
		}
		if (executeAsyncRunnableFactory == null)
		{
		  executeAsyncRunnableFactory = new DefaultExecuteAsyncRunnableFactory();
		}
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
			log.info("Shutting down the default async job executor [{}].", this.GetType().FullName);
			timerJobRunnable.stop();
			asyncJobsDueRunnable.stop();
			stopExecutingAsyncJobs();
        
			timerJobRunnable = null;
			asyncJobsDueRunnable = null;
			isActive = false;
		  }
	  }

	  protected internal abstract void startExecutingAsyncJobs();

	  protected internal abstract void stopExecutingAsyncJobs();

	  /// <summary>
	  /// Starts the acquisition thread </summary>
	  protected internal virtual void startJobAcquisitionThread()
	  {
		if (timerJobAcquisitionThread == null)
		{
		  timerJobAcquisitionThread = new Thread(timerJobRunnable);
		}
		timerJobAcquisitionThread.Start();

		if (asyncJobAcquisitionThread == null)
		{
		  asyncJobAcquisitionThread = new Thread(asyncJobsDueRunnable);
		}
		asyncJobAcquisitionThread.Start();
	  }

	  /// <summary>
	  /// Stops the acquisition thread </summary>
	  protected internal virtual void stopJobAcquisitionThread()
	  {
		try
		{
		  timerJobAcquisitionThread.Join();
		}
		catch (InterruptedException e)
		{
		  log.warn("Interrupted while waiting for the timer job acquisition thread to terminate", e);
		}

		try
		{
		  asyncJobAcquisitionThread.Join();
		}
		catch (InterruptedException e)
		{
		  log.warn("Interrupted while waiting for the async job acquisition thread to terminate", e);
		}

		timerJobAcquisitionThread = null;
		asyncJobAcquisitionThread = null;
	  }

	  /* getters and setters */ 

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


	  public virtual bool Active
	  {
		  get
		  {
			return isActive;
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


	  public virtual int TimerLockTimeInMillis
	  {
		  get
		  {
			return timerLockTimeInMillis;
		  }
		  set
		  {
			this.timerLockTimeInMillis = value;
		  }
	  }


	  public virtual int AsyncJobLockTimeInMillis
	  {
		  get
		  {
			return asyncJobLockTimeInMillis;
		  }
		  set
		  {
			this.asyncJobLockTimeInMillis = value;
		  }
	  }


	  public virtual int MaxTimerJobsPerAcquisition
	  {
		  get
		  {
			return maxTimerJobsPerAcquisition;
		  }
		  set
		  {
			this.maxTimerJobsPerAcquisition = value;
		  }
	  }


	  public virtual int MaxAsyncJobsDuePerAcquisition
	  {
		  get
		  {
			return maxAsyncJobsDuePerAcquisition;
		  }
		  set
		  {
			this.maxAsyncJobsDuePerAcquisition = value;
		  }
	  }


	  public virtual int DefaultTimerJobAcquireWaitTimeInMillis
	  {
		  get
		  {
			return defaultTimerJobAcquireWaitTimeInMillis;
		  }
		  set
		  {
			this.defaultTimerJobAcquireWaitTimeInMillis = value;
		  }
	  }


	  public virtual int DefaultAsyncJobAcquireWaitTimeInMillis
	  {
		  get
		  {
			return defaultAsyncJobAcquireWaitTimeInMillis;
		  }
		  set
		  {
			this.defaultAsyncJobAcquireWaitTimeInMillis = value;
		  }
	  }


	  public virtual int DefaultQueueSizeFullWaitTimeInMillis
	  {
		  get
		  {
			return defaultQueueSizeFullWaitTime;
		  }
		  set
		  {
			this.defaultQueueSizeFullWaitTime = value;
		  }
	  }


	  public virtual AcquireTimerJobsRunnable TimerJobRunnable
	  {
		  set
		  {
			this.timerJobRunnable = value;
		  }
	  }

	  public virtual AcquireAsyncJobsDueRunnable AsyncJobsDueRunnable
	  {
		  set
		  {
			this.asyncJobsDueRunnable = value;
		  }
	  }

	  public virtual int RetryWaitTimeInMillis
	  {
		  get
		  {
			return retryWaitTimeInMillis;
		  }
		  set
		  {
			this.retryWaitTimeInMillis = value;
		  }
	  }


	  public virtual ExecuteAsyncRunnableFactory getExecuteAsyncRunnableFactory()
	  {
		return executeAsyncRunnableFactory;
	  }

	  public virtual void setExecuteAsyncRunnableFactory(ExecuteAsyncRunnableFactory executeAsyncRunnableFactory)
	  {
		this.executeAsyncRunnableFactory = executeAsyncRunnableFactory;
	  }
	}

}