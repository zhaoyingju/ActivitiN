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
	using Context = org.activiti.engine.impl.context.Context;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;
	using Task = org.activiti.engine.task.Task;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class SaveTaskCmd : Command<Void>
	{

		private const long serialVersionUID = 1L;

		protected internal TaskEntity task;

		public SaveTaskCmd(Task task)
		{
			this.task = (TaskEntity) task;
		}

		public virtual Void execute(CommandContext commandContext)
		{
		  if (task == null)
		  {
			throw new ActivitiIllegalArgumentException("task is null");
		  }

		if (task.Revision == 0)
		{
		  task.insert(null);

		  // Need to to be done here, we can't make it generic for standalone tasks 
		  // and tasks from a process, as the order of setting properties is
		  // completely different.
		  if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.TASK_CREATED, task));

			if (task.Assignee != null)
			{
				// The assignment event is normally fired when calling setAssignee. However, this
				// doesn't work for standalone tasks as the commandcontext is not availble.
				Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.TASK_ASSIGNED, task));
			}
		  }
		}
		else
		{
		  task.update();
		}

		return null;
		}

	}

}