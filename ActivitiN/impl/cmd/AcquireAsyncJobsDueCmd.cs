using System;
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
namespace org.activiti.engine.impl.cmd
{


	using AcquiredJobEntities = org.activiti.engine.impl.asyncexecutor.AcquiredJobEntities;
	using AsyncExecutor = org.activiti.engine.impl.asyncexecutor.AsyncExecutor;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;


	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	public class AcquireAsyncJobsDueCmd : Command<AcquiredJobEntities>
	{

	  private readonly AsyncExecutor asyncExecutor;

	  public AcquireAsyncJobsDueCmd(AsyncExecutor asyncExecutor)
	  {
		this.asyncExecutor = asyncExecutor;
	  }

	  public virtual AcquiredJobEntities execute(CommandContext commandContext)
	  {
		AcquiredJobEntities acquiredJobs = new AcquiredJobEntities();
		IList<JobEntity> jobs = commandContext.JobEntityManager.findAsyncJobsDueToExecute(new Page(0, asyncExecutor.MaxAsyncJobsDuePerAcquisition));

		foreach (JobEntity job in jobs)
		{
		  lockJob(commandContext, job, asyncExecutor.AsyncJobLockTimeInMillis);
		  acquiredJobs.addJob(job);
		}

		return acquiredJobs;
	  }

	  protected internal virtual void lockJob(CommandContext commandContext, JobEntity job, int lockTimeInMillis)
	  {
		GregorianCalendar gregorianCalendar = new GregorianCalendar();
		gregorianCalendar.Time = commandContext.ProcessEngineConfiguration.Clock.CurrentTime;
		gregorianCalendar.add(DateTime.MILLISECOND, lockTimeInMillis);
		job.LockExpirationTime = gregorianCalendar.Time;
	  }
	}
}