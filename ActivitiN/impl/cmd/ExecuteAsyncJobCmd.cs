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

	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	[Serializable]
	public class ExecuteAsyncJobCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;

	  private static Logger log = LoggerFactory.getLogger(typeof(ExecuteAsyncJobCmd));

	  protected internal JobEntity job;

	  public ExecuteAsyncJobCmd(JobEntity job)
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
		  log.debug("Executing async job {}", job.Id);
		}

		job.execute(commandContext);

		if (commandContext.EventDispatcher.Enabled)
		{
			commandContext.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.JOB_EXECUTION_SUCCESS, job));
		}

		return null;
	  }
	}
}