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


	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class DeleteTaskCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string taskId;
	  protected internal ICollection<string> taskIds;
	  protected internal bool cascade;
	  protected internal string deleteReason;

	  public DeleteTaskCmd(string taskId, string deleteReason, bool cascade)
	  {
		this.taskId = taskId;
		this.cascade = cascade;
		this.deleteReason = deleteReason;
	  }

	  public DeleteTaskCmd(ICollection<string> taskIds, string deleteReason, bool cascade)
	  {
		this.taskIds = taskIds;
		this.cascade = cascade;
		this.deleteReason = deleteReason;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		if (taskId != null)
		{
		  deleteTask(commandContext, taskId);
		}
		else if (taskIds != null)
		{
			foreach (string taskId in taskIds)
			{
			  deleteTask(commandContext, taskId);
			}
		}
		else
		{
		  throw new ActivitiIllegalArgumentException("taskId and taskIds are null");
		}


		return null;
	  }

	  protected internal virtual void deleteTask(CommandContext commandContext, string taskId)
	  {
		commandContext.TaskEntityManager.deleteTask(taskId, deleteReason, cascade);
	  }
	}

}