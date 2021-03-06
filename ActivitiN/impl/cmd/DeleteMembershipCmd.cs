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


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class DeleteMembershipCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;
	  internal string userId;
	  internal string groupId;

	  public DeleteMembershipCmd(string userId, string groupId)
	  {
		this.userId = userId;
		this.groupId = groupId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		if (userId == null)
		{
		  throw new ActivitiIllegalArgumentException("userId is null");
		}
		if (groupId == null)
		{
		  throw new ActivitiIllegalArgumentException("groupId is null");
		}

		commandContext.MembershipIdentityManager.deleteMembership(userId, groupId);

		return null;
	  }

	}

}