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
	using IdentityLinkType = org.activiti.engine.task.IdentityLinkType;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class AddIdentityLinkCmd : NeedsActiveTaskCmd<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string userId;

	  protected internal string groupId;

	  protected internal string type;

	  public AddIdentityLinkCmd(string taskId, string userId, string groupId, string type) : base(taskId)
	  {
		validateParams(userId, groupId, type, taskId);
		this.taskId = taskId;
		this.userId = userId;
		this.groupId = groupId;
		this.type = type;
	  }

	  protected internal virtual void validateParams(string userId, string groupId, string type, string taskId)
	  {
		if (taskId == null)
		{
		  throw new ActivitiIllegalArgumentException("taskId is null");
		}

		if (type == null)
		{
		  throw new ActivitiIllegalArgumentException("type is required when adding a new task identity link");
		}

		// Special treatment for assignee, group cannot be used an userId may be null
		if (IdentityLinkType.ASSIGNEE.Equals(type))
		{
		  if (groupId != null)
		  {
			throw new ActivitiIllegalArgumentException("Incompatible usage: cannot use ASSIGNEE" + " together with a groupId");
		  }
		}
		else
		{
		  if (userId == null && groupId == null)
		  {
			throw new ActivitiIllegalArgumentException("userId and groupId cannot both be null");
		  }
		}
	  }

	  protected internal virtual Void execute(CommandContext commandContext, TaskEntity task)
	  {

		bool assignedToNoOne = false;
		if (IdentityLinkType.ASSIGNEE.Equals(type))
		{
		  task.setAssignee(userId, true, true);
		  assignedToNoOne = userId == null;
		}
		else if (IdentityLinkType.OWNER.Equals(type))
		{
		  task.setOwner(userId, true);
		}
		else
		{
		  task.addIdentityLink(userId, groupId, type);
		}

		if (assignedToNoOne)
		{
		  // ACT-1317: Special handling when assignee is set to NULL, a CommentEntity notifying of assignee-delete should be created
		  commandContext.HistoryManager.createIdentityLinkComment(taskId, userId, groupId, type, false, true);
		}
		else
		{
		  commandContext.HistoryManager.createIdentityLinkComment(taskId, userId, groupId, type, true);
		}

		return null;
	  }

	}

}