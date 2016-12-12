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
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;
	using Task = org.activiti.engine.task.Task;

	/// <summary>
	/// An abstract superclass for <seealso cref="Command"/> implementations that 
	/// want to verify the provided task is always active (ie. not suspended).
	/// 
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public abstract class NeedsActiveTaskCmd<T> : Command<T>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string taskId;

	  public NeedsActiveTaskCmd(string taskId)
	  {
		this.taskId = taskId;
	  }

	  public virtual T execute(CommandContext commandContext)
	  {

		if (taskId == null)
		{
		  throw new ActivitiIllegalArgumentException("taskId is null");
		}

		TaskEntity task = commandContext.TaskEntityManager.findTaskById(taskId);

		if (task == null)
		{
		  throw new ActivitiObjectNotFoundException("Cannot find task with id " + taskId, typeof(Task));
		}

		if (task.Suspended)
		{
		  throw new ActivitiException(SuspendedTaskException);
		}

		return execute(commandContext, task);
	  }

	  /// <summary>
	  /// Subclasses must implement in this method their normal command logic.
	  /// The provided task is ensured to be active.
	  /// </summary>
	  protected internal abstract T execute(CommandContext commandContext, TaskEntity task);

	  /// <summary>
	  /// Subclasses can override this method to provide a customized exception message
	  /// that will be thrown when the task is suspended.
	  /// </summary>
	  protected internal virtual string SuspendedTaskException
	  {
		  get
		  {
			return "Cannot execute operation: task is suspended";
		  }
	  }

	}

}