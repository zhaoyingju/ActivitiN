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
namespace org.activiti.engine.impl.cmd
{


	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using AcquiredJobs = org.activiti.engine.impl.jobexecutor.AcquiredJobs;
	using JobExecutor = org.activiti.engine.impl.jobexecutor.JobExecutor;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using MessageEntity = org.activiti.engine.impl.persistence.entity.MessageEntity;


	/// <summary>
	/// @author Nick Burch
	/// @author Daniel Meyer
	/// </summary>
	public class AcquireJobsCmd : Command<AcquiredJobs>
	{

	  private readonly JobExecutor jobExecutor;

	  public AcquireJobsCmd(JobExecutor jobExecutor)
	  {
		this.jobExecutor = jobExecutor;
	  }

	  public virtual AcquiredJobs execute(CommandContext commandContext)
	  {

		string lockOwner = jobExecutor.LockOwner;
		int lockTimeInMillis = jobExecutor.LockTimeInMillis;
		int maxNonExclusiveJobsPerAcquisition = jobExecutor.MaxJobsPerAcquisition;

		AcquiredJobs acquiredJobs = new AcquiredJobs();
		IList<JobEntity> jobs = commandContext.JobEntityManager.findNextJobsToExecute(new Page(0, maxNonExclusiveJobsPerAcquisition));

		foreach (JobEntity job in jobs)
		{
		  IList<string> jobIds = new List<string>();
		  if (job != null && !acquiredJobs.contains(job.Id))
		  {
			if (job is MessageEntity && job.Exclusive && job.ProcessInstanceId != null)
			{
			  // wait to get exclusive jobs within 100ms
			  try
			  {
				Thread.Sleep(100);
			  }
			  catch (InterruptedException)
			  {
			  }

			  // acquire all exclusive jobs in the same process instance
			  // (includes the current job)
			  IList<JobEntity> exclusiveJobs = commandContext.JobEntityManager.findExclusiveJobsToExecute(job.ProcessInstanceId);
			  foreach (JobEntity exclusiveJob in exclusiveJobs)
			  {
				if (exclusiveJob != null)
				{
				  lockJob(commandContext, exclusiveJob, lockOwner, lockTimeInMillis);
				  jobIds.Add(exclusiveJob.Id);
				}
			  }

			}
			else
			{
			  lockJob(commandContext, job, lockOwner, lockTimeInMillis);
			  jobIds.Add(job.Id);
			}

		  }

		  acquiredJobs.addJobIdBatch(jobIds);
		}

		return acquiredJobs;
	  }

	  protected internal virtual void lockJob(CommandContext commandContext, JobEntity job, string lockOwner, int lockTimeInMillis)
	  {
		job.LockOwner = lockOwner;
		GregorianCalendar gregorianCalendar = new GregorianCalendar();
		gregorianCalendar.Time = commandContext.ProcessEngineConfiguration.Clock.CurrentTime;
		gregorianCalendar.add(DateTime.MILLISECOND, lockTimeInMillis);
		job.LockExpirationTime = gregorianCalendar.Time;
	  }
	}
}