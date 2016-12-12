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

	using TaskFormHandler = org.activiti.engine.impl.form.TaskFormHandler;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class SubmitTaskFormCmd : NeedsActiveTaskCmd<object>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string taskId;
	  protected internal IDictionary<string, string> properties;
	  protected internal bool completeTask;

	  public SubmitTaskFormCmd(string taskId, IDictionary<string, string> properties, bool completeTask) : base(taskId)
	  {
		this.taskId = taskId;
		this.properties = properties;
		this.completeTask = completeTask;
	  }

	  protected internal virtual object execute(CommandContext commandContext, TaskEntity task)
	  {
		commandContext.HistoryManager.reportFormPropertiesSubmitted(task.getExecution(), properties, taskId);

		TaskFormHandler taskFormHandler = task.TaskDefinition.TaskFormHandler;
		taskFormHandler.submitFormProperties(properties, task.getExecution());

		if (completeTask)
		{
		  task.complete(properties, false);
		}

		return null;
	  }

	  protected internal override string SuspendedTaskException
	  {
		  get
		  {
			return "Cannot submit a form to a suspended task";
		  }
	  }

	}

}