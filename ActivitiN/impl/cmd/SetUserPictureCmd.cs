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

	using Picture = org.activiti.engine.identity.Picture;
	using User = org.activiti.engine.identity.User;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class SetUserPictureCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string userId;
	  protected internal Picture picture;


	  public SetUserPictureCmd(string userId, Picture picture)
	  {
		this.userId = userId;
		this.picture = picture;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		if (userId == null)
		{
		  throw new ActivitiIllegalArgumentException("userId is null");
		}
		User user = commandContext.UserIdentityManager.findUserById(userId);
		if (user == null)
		{
		  throw new ActivitiObjectNotFoundException("user " + userId + " doesn't exist", typeof(User));
		}
		commandContext.UserIdentityManager.setUserPicture(userId, picture);
		return null;
	  }

	}

}