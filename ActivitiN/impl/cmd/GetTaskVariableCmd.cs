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
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class GetTaskVariableCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string taskId;
	  protected internal string variableName;
	  protected internal bool isLocal;

	  public GetTaskVariableCmd(string taskId, string variableName, bool isLocal)
	  {
		this.taskId = taskId;
		this.variableName = variableName;
		this.isLocal = isLocal;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		if (taskId == null)
		{
		  throw new ActivitiIllegalArgumentException("taskId is null");
		}
		if (variableName == null)
		{
		  throw new ActivitiIllegalArgumentException("variableName is null");
		}

		TaskEntity task = commandContext.TaskEntityManager.findTaskById(taskId);

		if (task == null)
		{
		  throw new ActivitiObjectNotFoundException("task " + taskId + " doesn't exist", typeof(Task));
		}

		object value;

		if (isLocal)
		{
		  value = task.getVariableLocal(variableName, false);
		}
		else
		{
		  value = task.getVariable(variableName, false);
		}

		return value;
	  }
	}

}