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
namespace org.activiti.engine.impl
{

	using Group = org.activiti.engine.identity.Group;
	using GroupQuery = org.activiti.engine.identity.GroupQuery;
	using NativeGroupQuery = org.activiti.engine.identity.NativeGroupQuery;
	using NativeUserQuery = org.activiti.engine.identity.NativeUserQuery;
	using Picture = org.activiti.engine.identity.Picture;
	using User = org.activiti.engine.identity.User;
	using UserQuery = org.activiti.engine.identity.UserQuery;
	using CheckPassword = org.activiti.engine.impl.cmd.CheckPassword;
	using CreateGroupCmd = org.activiti.engine.impl.cmd.CreateGroupCmd;
	using CreateGroupQueryCmd = org.activiti.engine.impl.cmd.CreateGroupQueryCmd;
	using CreateMembershipCmd = org.activiti.engine.impl.cmd.CreateMembershipCmd;
	using CreateUserCmd = org.activiti.engine.impl.cmd.CreateUserCmd;
	using CreateUserQueryCmd = org.activiti.engine.impl.cmd.CreateUserQueryCmd;
	using DeleteGroupCmd = org.activiti.engine.impl.cmd.DeleteGroupCmd;
	using DeleteMembershipCmd = org.activiti.engine.impl.cmd.DeleteMembershipCmd;
	using DeleteUserCmd = org.activiti.engine.impl.cmd.DeleteUserCmd;
	using DeleteUserInfoCmd = org.activiti.engine.impl.cmd.DeleteUserInfoCmd;
	using GetUserInfoCmd = org.activiti.engine.impl.cmd.GetUserInfoCmd;
	using GetUserInfoKeysCmd = org.activiti.engine.impl.cmd.GetUserInfoKeysCmd;
	using GetUserPictureCmd = org.activiti.engine.impl.cmd.GetUserPictureCmd;
	using SaveGroupCmd = org.activiti.engine.impl.cmd.SaveGroupCmd;
	using SaveUserCmd = org.activiti.engine.impl.cmd.SaveUserCmd;
	using SetUserInfoCmd = org.activiti.engine.impl.cmd.SetUserInfoCmd;
	using SetUserPictureCmd = org.activiti.engine.impl.cmd.SetUserPictureCmd;
	using Authentication = org.activiti.engine.impl.identity.Authentication;
	using GroupEntity = org.activiti.engine.impl.persistence.entity.GroupEntity;
	using IdentityInfoEntity = org.activiti.engine.impl.persistence.entity.IdentityInfoEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class IdentityServiceImpl : ServiceImpl, IdentityService
	{

	  public virtual Group newGroup(string groupId)
	  {
		return commandExecutor.execute(new CreateGroupCmd(groupId));
	  }

	  public virtual User newUser(string userId)
	  {
		return commandExecutor.execute(new CreateUserCmd(userId));
	  }

	  public virtual void saveGroup(Group group)
	  {
		commandExecutor.execute(new SaveGroupCmd(group));
	  }

	  public virtual void saveUser(User user)
	  {
		commandExecutor.execute(new SaveUserCmd(user));
	  }

	  public virtual UserQuery createUserQuery()
	  {
		return commandExecutor.execute(new CreateUserQueryCmd());
	  }

	  public override NativeUserQuery createNativeUserQuery()
	  {
		return new NativeUserQueryImpl(commandExecutor);
	  }

	  public virtual GroupQuery createGroupQuery()
	  {
		return commandExecutor.execute(new CreateGroupQueryCmd());
	  }

	  public override NativeGroupQuery createNativeGroupQuery()
	  {
		return new NativeGroupQueryImpl(commandExecutor);
	  }

	  public virtual void createMembership(string userId, string groupId)
	  {
		commandExecutor.execute(new CreateMembershipCmd(userId, groupId));
	  }

	  public virtual void deleteGroup(string groupId)
	  {
		commandExecutor.execute(new DeleteGroupCmd(groupId));
	  }

	  public virtual void deleteMembership(string userId, string groupId)
	  {
		commandExecutor.execute(new DeleteMembershipCmd(userId, groupId));
	  }

	  public virtual bool checkPassword(string userId, string password)
	  {
		return commandExecutor.execute(new CheckPassword(userId, password));
	  }

	  public virtual void deleteUser(string userId)
	  {
		commandExecutor.execute(new DeleteUserCmd(userId));
	  }

	  public virtual void setUserPicture(string userId, Picture picture)
	  {
		commandExecutor.execute(new SetUserPictureCmd(userId, picture));
	  }

	  public virtual Picture getUserPicture(string userId)
	  {
		return commandExecutor.execute(new GetUserPictureCmd(userId));
	  }

	  public virtual string AuthenticatedUserId
	  {
		  set
		  {
			Authentication.AuthenticatedUserId = value;
		  }
	  }

	  public virtual string getUserInfo(string userId, string key)
	  {
		return commandExecutor.execute(new GetUserInfoCmd(userId, key));
	  }

	  public virtual IList<string> getUserInfoKeys(string userId)
	  {
		return commandExecutor.execute(new GetUserInfoKeysCmd(userId, IdentityInfoEntity.TYPE_USERINFO));
	  }

	  public virtual void setUserInfo(string userId, string key, string value)
	  {
		commandExecutor.execute(new SetUserInfoCmd(userId, key, value));
	  }

	  public virtual void deleteUserInfo(string userId, string key)
	  {
		commandExecutor.execute(new DeleteUserInfoCmd(userId, key));
	  }
	}

}