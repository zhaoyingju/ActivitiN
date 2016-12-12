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


namespace org.activiti.engine.impl.test
{


	using AsyncExecutor = org.activiti.engine.impl.asyncexecutor.AsyncExecutor;
	using JobExecutor = org.activiti.engine.impl.jobexecutor.JobExecutor;
	using ActivitiRule = org.activiti.engine.test.ActivitiRule;



	/// <summary>
	/// @author Joram Barrez
	/// @author Tijs Rademakers
	/// @author Saeid Mirzaei
	/// </summary>

	// This helper class helps sharing the same code for jobExector test helpers, between Junit3 and junit 4 test support classes
	public class JobTestHelper
	{

	  public static void waitForJobExecutorToProcessAllJobs(ActivitiRule activitiRule, long maxMillisToWait, long intervalMillis)
	  {
		waitForJobExecutorToProcessAllJobs(activitiRule.ProcessEngine.ProcessEngineConfiguration, activitiRule.ManagementService, maxMillisToWait, intervalMillis);
	  }

	  public static void waitForJobExecutorToProcessAllJobs(ProcessEngineConfiguration processEngineConfiguration, ManagementService managementService, long maxMillisToWait, long intervalMillis)
	  {

		  waitForJobExecutorToProcessAllJobs(processEngineConfiguration, managementService, maxMillisToWait, intervalMillis, true);

	  }

	  public static void waitForJobExecutorToProcessAllJobs(ProcessEngineConfiguration processEngineConfiguration, ManagementService managementService, long maxMillisToWait, long intervalMillis, bool shutdownExecutorWhenFinished)
	  {

		JobExecutor jobExecutor = null;
		AsyncExecutor asyncExecutor = null;
		if (processEngineConfiguration.AsyncExecutorEnabled == false)
		{
		  jobExecutor = processEngineConfiguration.JobExecutor;
		  jobExecutor.start();

		}
		else
		{
		  asyncExecutor = processEngineConfiguration.AsyncExecutor;
		  asyncExecutor.start();
		}

		try
		{
		  Timer timer = new Timer();
		  InteruptTask task = new InteruptTask(Thread.CurrentThread);
		  timer.schedule(task, maxMillisToWait);
		  bool areJobsAvailable = true;
		  try
		  {
			while (areJobsAvailable && !task.TimeLimitExceeded)
			{
			  Thread.Sleep(intervalMillis);
			  try
			  {
				areJobsAvailable = areJobsAvailable(managementService);
			  }
			  catch (Exception)
			  {
				// Ignore, possible that exception occurs due to locking/updating of table on MSSQL when
				// isolation level doesn't allow READ of the table
			  }
			}
		  }
		  catch (InterruptedException)
		  {
			// ignore
		  }
		  finally
		  {
			timer.cancel();
		  }
		  if (areJobsAvailable)
		  {
			throw new ActivitiException("time limit of " + maxMillisToWait + " was exceeded");
		  }

		}
		finally
		{
			if (shutdownExecutorWhenFinished)
			{
			  if (processEngineConfiguration.AsyncExecutorEnabled == false)
			  {
				jobExecutor.shutdown();
			  }
			  else
			  {
				asyncExecutor.shutdown();
			  }
			}
		}
	  }

	  public static void waitForJobExecutorOnCondition(ActivitiRule activitiRule, long maxMillisToWait, long intervalMillis, Callable<bool?> condition)
	  {
		waitForJobExecutorOnCondition(activitiRule.ProcessEngine.ProcessEngineConfiguration, maxMillisToWait, intervalMillis, condition);
	  }

	  public static void waitForJobExecutorOnCondition(ProcessEngineConfiguration processEngineConfiguration, long maxMillisToWait, long intervalMillis, Callable<bool?> condition)
	  {

		JobExecutor jobExecutor = null;
		AsyncExecutor asyncExecutor = null;
		if (processEngineConfiguration.AsyncExecutorEnabled == false)
		{
		  jobExecutor = processEngineConfiguration.JobExecutor;
		  jobExecutor.start();

		}
		else
		{
		  asyncExecutor = processEngineConfiguration.AsyncExecutor;
		  asyncExecutor.start();
		}

		try
		{
		  Timer timer = new Timer();
		  InteruptTask task = new InteruptTask(Thread.CurrentThread);
		  timer.schedule(task, maxMillisToWait);
		  bool conditionIsViolated = true;
		  try
		  {
			while (conditionIsViolated)
			{
			  Thread.Sleep(intervalMillis);
			  conditionIsViolated = !condition.call();
			}
		  }
		  catch (InterruptedException)
		  {
			// ignore
		  }
		  catch (Exception e)
		  {
			throw new ActivitiException("Exception while waiting on condition: " + e.Message, e);
		  }
		  finally
		  {
			timer.cancel();
		  }

		  if (conditionIsViolated)
		  {
			throw new ActivitiException("time limit of " + maxMillisToWait + " was exceeded");
		  }

		}
		finally
		{
		  if (processEngineConfiguration.AsyncExecutorEnabled == false)
		  {
			jobExecutor.shutdown();
		  }
		  else
		  {
			asyncExecutor.shutdown();
		  }
		}
	  }

	  public static void executeJobExecutorForTime(ActivitiRule activitiRule, long maxMillisToWait, long intervalMillis)
	  {
		executeJobExecutorForTime(activitiRule.ProcessEngine.ProcessEngineConfiguration, maxMillisToWait, intervalMillis);
	  }

	  public static void executeJobExecutorForTime(ProcessEngineConfiguration processEngineConfiguration, long maxMillisToWait, long intervalMillis)
	  {
		JobExecutor jobExecutor = null;
		AsyncExecutor asyncExecutor = null;
		if (processEngineConfiguration.AsyncExecutorEnabled == false)
		{
		  jobExecutor = processEngineConfiguration.JobExecutor;
		  jobExecutor.start();

		}
		else
		{
		  asyncExecutor = processEngineConfiguration.AsyncExecutor;
		  asyncExecutor.start();
		}

		try
		{
		  Timer timer = new Timer();
		  InteruptTask task = new InteruptTask(Thread.CurrentThread);
		  timer.schedule(task, maxMillisToWait);
		  try
		  {
			while (!task.TimeLimitExceeded)
			{
			  Thread.Sleep(intervalMillis);
			}
		  }
		  catch (InterruptedException)
		  {
			// ignore
		  }
		  finally
		  {
			timer.cancel();
		  }

		}
		finally
		{
		  if (processEngineConfiguration.AsyncExecutorEnabled == false)
		  {
			jobExecutor.shutdown();
		  }
		  else
		  {
			asyncExecutor.shutdown();
		  }
		}
	  }

	  public static bool areJobsAvailable(ActivitiRule activitiRule)
	  {
		return areJobsAvailable(activitiRule.ManagementService);

	  }

	  public static bool areJobsAvailable(ManagementService managementService)
	  {
		return managementService.createJobQuery().list().Count > 0;
	  }

	  private class InteruptTask : TimerTask
	  {

		protected internal bool timeLimitExceeded = false;
		protected internal Thread thread;

		public InteruptTask(Thread thread)
		{
		  this.thread = thread;
		}
		public virtual bool TimeLimitExceeded
		{
			get
			{
			  return timeLimitExceeded;
			}
		}
		public virtual void run()
		{
		  timeLimitExceeded = true;
		  thread.Interrupt();
		}
	  }
	}
}