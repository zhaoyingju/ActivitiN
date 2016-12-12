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
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;


	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	[Serializable]
	public class AddIdentityLinkForProcessDefinitionCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string processDefinitionId;

	  protected internal string userId;

	  protected internal string groupId;

	  public AddIdentityLinkForProcessDefinitionCmd(string processDefinitionId, string userId, string groupId)
	  {
		validateParams(userId, groupId, processDefinitionId);
		this.processDefinitionId = processDefinitionId;
		this.userId = userId;
		this.groupId = groupId;
	  }

	  protected internal virtual void validateParams(string userId, string groupId, string processDefinitionId)
	  {
		if (processDefinitionId == null)
		{
		  throw new ActivitiIllegalArgumentException("processDefinitionId is null");
		}

		if (userId == null && groupId == null)
		{
		  throw new ActivitiIllegalArgumentException("userId and groupId cannot both be null");
		}
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ProcessDefinitionEntity processDefinition = commandContext.ProcessDefinitionEntityManager.findProcessDefinitionById(processDefinitionId);

		if (processDefinition == null)
		{
		  throw new ActivitiObjectNotFoundException("Cannot find process definition with id " + processDefinitionId, typeof(ProcessDefinition));
		}

		processDefinition.addIdentityLink(userId, groupId);

		return null;
	  }

	}

}