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
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// @author Marcus Klimstra
	/// </summary>
	[Serializable]
	public class AddIdentityLinkForProcessInstanceCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string processInstanceId;

	  protected internal string userId;

	  protected internal string groupId;

	  protected internal string type;

	  public AddIdentityLinkForProcessInstanceCmd(string processInstanceId, string userId, string groupId, string type)
	  {
		validateParams(processInstanceId, userId, groupId, type);
		this.processInstanceId = processInstanceId;
		this.userId = userId;
		this.groupId = groupId;
		this.type = type;
	  }

	  protected internal virtual void validateParams(string processInstanceId, string userId, string groupId, string type)
	  {

		if (processInstanceId == null)
		{
		  throw new ActivitiIllegalArgumentException("processInstanceId is null");
		}

		if (type == null)
		{
		  throw new ActivitiIllegalArgumentException("type is required when adding a new process instance identity link");
		}

		if (userId == null && groupId == null)
		{
		  throw new ActivitiIllegalArgumentException("userId and groupId cannot both be null");
		}

	  }

	  public virtual Void execute(CommandContext commandContext)
	  {

		ExecutionEntity processInstance = commandContext.ExecutionEntityManager.findExecutionById(processInstanceId);

		if (processInstance == null)
		{
		  throw new ActivitiObjectNotFoundException("Cannot find process instance with id " + processInstanceId, typeof(ExecutionEntity));
		}

		processInstance.addIdentityLink(userId, groupId, type);

		commandContext.HistoryManager.createProcessInstanceIdentityLinkComment(processInstanceId, userId, groupId, type, true);

		return null;

	  }

	}

}