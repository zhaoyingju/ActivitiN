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

	using TaskFormData = org.activiti.engine.form.TaskFormData;
	using TaskFormHandler = org.activiti.engine.impl.form.TaskFormHandler;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;
	using Task = org.activiti.engine.task.Task;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class GetTaskFormCmd : Command<TaskFormData>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string taskId;

	  public GetTaskFormCmd(string taskId)
	  {
		this.taskId = taskId;
	  }

	  public virtual TaskFormData execute(CommandContext commandContext)
	  {
		TaskEntity task = commandContext.TaskEntityManager.findTaskById(taskId);
		if (task == null)
		{
		  throw new ActivitiObjectNotFoundException("No task found for taskId '" + taskId + "'", typeof(Task));
		}

		if (task.TaskDefinition != null)
		{
		  TaskFormHandler taskFormHandler = task.TaskDefinition.TaskFormHandler;
		  if (taskFormHandler == null)
		  {
			throw new ActivitiException("No taskFormHandler specified for task '" + taskId + "'");
		  }

		  return taskFormHandler.createTaskForm(task);
		}
		else
		{
		  // Standalone task, no TaskFormData available
		  return null;
		}
	  }

	}

}