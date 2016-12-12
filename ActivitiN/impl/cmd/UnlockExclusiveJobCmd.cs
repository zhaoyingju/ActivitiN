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
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tijs Rademakers
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class UnlockExclusiveJobCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;

	  private static Logger log = LoggerFactory.getLogger(typeof(UnlockExclusiveJobCmd));

	  protected internal JobEntity job;

	  public UnlockExclusiveJobCmd(JobEntity job)
	  {
		  this.job = job;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {

		if (job == null)
		{
		  throw new ActivitiIllegalArgumentException("job is null");
		}

		if (log.DebugEnabled)
		{
		  log.debug("Unlocking exclusive job {}", job.Id);
		}

		if (job.Exclusive)
		{
		  if (job.ProcessInstanceId != null)
		  {
			ExecutionEntity execution = commandContext.ExecutionEntityManager.findExecutionById(job.ProcessInstanceId);
			if (execution != null)
			{
			  commandContext.ExecutionEntityManager.clearProcessInstanceLockTime(execution.Id);
			}
		  }
		}

		return null;
	  }
	}
}