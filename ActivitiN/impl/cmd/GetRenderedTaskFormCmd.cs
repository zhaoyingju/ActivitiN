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
	using FormEngine = org.activiti.engine.impl.form.FormEngine;
	using TaskFormHandler = org.activiti.engine.impl.form.TaskFormHandler;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;
	using Task = org.activiti.engine.task.Task;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class GetRenderedTaskFormCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string taskId;
	  protected internal string formEngineName;

	  public GetRenderedTaskFormCmd(string taskId, string formEngineName)
	  {
		this.taskId = taskId;
		this.formEngineName = formEngineName;
	  }


	  public virtual object execute(CommandContext commandContext)
	  {
		TaskEntity task = commandContext.TaskEntityManager.findTaskById(taskId);
		if (task == null)
		{
		  throw new ActivitiObjectNotFoundException("Task '" + taskId + "' not found", typeof(Task));
		}

		if (task.TaskDefinition == null)
		{
		  throw new ActivitiException("Task form definition for '" + taskId + "' not found");
		}

		TaskFormHandler taskFormHandler = task.TaskDefinition.TaskFormHandler;
		if (taskFormHandler == null)
		{
		  return null;
		}

		FormEngine formEngine = commandContext.ProcessEngineConfiguration.FormEngines[formEngineName];

		if (formEngine == null)
		{
		  throw new ActivitiException("No formEngine '" + formEngineName + "' defined process engine configuration");
		}

		TaskFormData taskForm = taskFormHandler.createTaskForm(task);

		return formEngine.renderTaskForm(taskForm);
	  }
	}

}