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

	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ClaimTaskCmd : NeedsActiveTaskCmd<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string userId;

	  public ClaimTaskCmd(string taskId, string userId) : base(taskId)
	  {
		this.userId = userId;
	  }

	  protected internal virtual Void execute(CommandContext commandContext, TaskEntity task)
	  {

		if (userId != null)
		{
		  if (task.Assignee != null)
		  {
			if (!task.Assignee.Equals(userId))
			{
			  // When the task is already claimed by another user, throw exception. Otherwise, ignore
			  // this, post-conditions of method already met.
			  throw new ActivitiTaskAlreadyClaimedException(task.Id, task.Assignee);
			}
		  }
		  else
		  {
			task.setAssignee(userId, true, true);
		  }
		}
		else
		{
		  // Task should be assigned to no one
		  task.setAssignee(null, true, true);
		}

		// Add claim time
		commandContext.HistoryManager.recordTaskClaim(taskId);

		return null;
	  }

	  protected internal override string SuspendedTaskException
	  {
		  get
		  {
			return "Cannot claim a suspended task";
		  }
	  }

	}

}