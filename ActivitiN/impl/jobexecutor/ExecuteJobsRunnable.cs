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
namespace org.activiti.engine.impl.jobexecutor
{

	using ExecuteJobsCmd = org.activiti.engine.impl.cmd.ExecuteJobsCmd;
	using Context = org.activiti.engine.impl.context.Context;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public class ExecuteJobsRunnable : Runnable
	{

		private static Logger log = LoggerFactory.getLogger(typeof(ExecuteJobsRunnable));

		private JobEntity job;
		private IList<string> jobIds;
		private JobExecutor jobExecutor;

		public ExecuteJobsRunnable(JobExecutor jobExecutor, JobEntity job)
		{
			this.jobExecutor = jobExecutor;
			this.job = job;
		}

		public ExecuteJobsRunnable(JobExecutor jobExecutor, IList<string> jobIds)
		{
			this.jobExecutor = jobExecutor;
			this.jobIds = jobIds;
		}

		public virtual void run()
		{
			if (jobIds != null)
			{
				handleMultipleJobs();
			}
			if (job != null)
			{
				handleSingleJob();
			}
		}

		protected internal virtual void handleSingleJob()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SingleJobExecutorContext jobExecutorContext = new SingleJobExecutorContext();
			SingleJobExecutorContext jobExecutorContext = new SingleJobExecutorContext();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.activiti.engine.impl.persistence.entity.JobEntity> currentProcessorJobQueue = jobExecutorContext.getCurrentProcessorJobQueue();
			IList<JobEntity> currentProcessorJobQueue = jobExecutorContext.CurrentProcessorJobQueue;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.activiti.engine.impl.interceptor.CommandExecutor commandExecutor = jobExecutor.getCommandExecutor();
			CommandExecutor commandExecutor = jobExecutor.CommandExecutor;

			currentProcessorJobQueue.Add(job);

			Context.JobExecutorContext = jobExecutorContext;
			try
			{
				while (currentProcessorJobQueue.Count > 0)
				{

					JobEntity currentJob = currentProcessorJobQueue.Remove(0);
					try
					{
						commandExecutor.execute(new ExecuteJobsCmd(currentJob));
					}
					catch (Exception e)
					{
						log.error("exception during job execution: {}", e.Message, e);
					}
					finally
					{
						jobExecutor.jobDone(currentJob);
					}
				}
			}
			finally
			{
				Context.removeJobExecutorContext();
			}
		}

		protected internal virtual void handleMultipleJobs()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MultipleJobsExecutorContext jobExecutorContext = new MultipleJobsExecutorContext();
			MultipleJobsExecutorContext jobExecutorContext = new MultipleJobsExecutorContext();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> currentProcessorJobQueue = jobExecutorContext.getCurrentProcessorJobQueue();
			IList<string> currentProcessorJobQueue = jobExecutorContext.CurrentProcessorJobQueue;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.activiti.engine.impl.interceptor.CommandExecutor commandExecutor = jobExecutor.getCommandExecutor();
			CommandExecutor commandExecutor = jobExecutor.CommandExecutor;

			currentProcessorJobQueue.AddRange(jobIds);

			Context.JobExecutorContext = jobExecutorContext;
			try
			{
				while (currentProcessorJobQueue.Count > 0)
				{

					string currentJobId = currentProcessorJobQueue.Remove(0);
					try
					{
						commandExecutor.execute(new ExecuteJobsCmd(currentJobId));
					}
					catch (Exception e)
					{
						log.error("exception during job execution: {}", e.Message, e);
					}
					finally
					{
						jobExecutor.jobDone(currentJobId);
					}
				}
			}
			finally
			{
				Context.removeJobExecutorContext();
			}
		}
	}

}