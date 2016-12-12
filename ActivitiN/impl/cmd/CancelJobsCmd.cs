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


	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;


	/// <summary>
	/// Send job cancelled event and delete job
	/// 
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class CancelJobsCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;
	  internal IList<string> jobIds;

	  public CancelJobsCmd(IList<string> jobIds)
	  {
		this.jobIds = jobIds;
	  }

	  public CancelJobsCmd(string jobId)
	  {
		this.jobIds = new List<string>();
		jobIds.Add(jobId);
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		JobEntity jobToDelete = null;
		foreach (string jobId in jobIds)
		{
		  jobToDelete = commandContext.JobEntityManager.findJobById(jobId);

		  if (jobToDelete != null)
		  {
			// When given job doesn't exist, ignore
			if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
			{
			  commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.JOB_CANCELED, jobToDelete));
			}

			jobToDelete.delete();
		  }
		}
		return null;
	  }
	}

}