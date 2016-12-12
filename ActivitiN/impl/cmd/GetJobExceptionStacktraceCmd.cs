using System;

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
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using Job = org.activiti.engine.runtime.Job;


	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	[Serializable]
	public class GetJobExceptionStacktraceCmd : Command<string>
	{

	  private const long serialVersionUID = 1L;
	  private string jobId;

	  public GetJobExceptionStacktraceCmd(string jobId)
	  {
		this.jobId = jobId;
	  }


	  public virtual string execute(CommandContext commandContext)
	  {
		if (jobId == null)
		{
		  throw new ActivitiIllegalArgumentException("jobId is null");
		}

		JobEntity job = commandContext.JobEntityManager.findJobById(jobId);

		if (job == null)
		{
		  throw new ActivitiObjectNotFoundException("No job found with id " + jobId, typeof(Job));
		}

		return job.ExceptionStacktrace;
	  }


	}

}