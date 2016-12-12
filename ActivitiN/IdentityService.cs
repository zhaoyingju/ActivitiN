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
namespace org.activiti.engine
{

	using Group = org.activiti.engine.identity.Group;
	using GroupQuery = org.activiti.engine.identity.GroupQuery;
	using NativeGroupQuery = org.activiti.engine.identity.NativeGroupQuery;
	using NativeUserQuery = org.activiti.engine.identity.NativeUserQuery;
	using Picture = org.activiti.engine.identity.Picture;
	using User = org.activiti.engine.identity.User;
	using UserQuery = org.activiti.engine.identity.UserQuery;


	/// <summary>
	/// Service to manage <seealso cref="User"/>s and <seealso cref="Group"/>s.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public interface IdentityService
	{

	  /// <summary>
	  /// Creates a new user. The user is transient and must be saved using 
	  /// <seealso cref="#saveUser(User)"/>. </summary>
	  /// <param name="userId"> id for the new user, cannot be null. </param>
	  User newUser(string userId);

	  /// <summary>
	  /// Saves the user. If the user already existed, the user is updated. </summary>
	  /// <param name="user"> user to save, cannot be null. </param>
	  /// <exception cref="RuntimeException"> when a user with the same name already exists. </exception>
	  void saveUser(User user);

	  /// <summary>
	  /// Creates a <seealso cref="UserQuery"/> that allows to programmatically query the users.
	  /// </summary>
	  UserQuery createUserQuery();

	  /// <summary>
	  /// Returns a new <seealso cref="org.activiti.engine.query.NativeQuery"/> for tasks.
	  /// </summary>
	  NativeUserQuery createNativeUserQuery();

	  /// <param name="userId"> id of user to delete, cannot be null. When an id is passed
	  /// for an unexisting user, this operation is ignored. </param>
	  void deleteUser(string userId);

	  /// <summary>
	  /// Creates a new group. The group is transient and must be saved using 
	  /// <seealso cref="#saveGroup(Group)"/>. </summary>
	  /// <param name="groupId"> id for the new group, cannot be null. </param>
	  Group newGroup(string groupId);

	  /// <summary>
	  /// Creates a <seealso cref="GroupQuery"/> thats allows to programmatically query the groups.
	  /// </summary>
	  GroupQuery createGroupQuery();

	  /// <summary>
	  /// Returns a new <seealso cref="org.activiti.engine.query.NativeQuery"/> for tasks.
	  /// </summary>
	  NativeGroupQuery createNativeGroupQuery();

	  /// <summary>
	  /// Saves the group. If the group already existed, the group is updated. </summary>
	  /// <param name="group"> group to save. Cannot be null. </param>
	  /// <exception cref="RuntimeException"> when a group with the same name already exists. </exception>
	  void saveGroup(Group group);

	  /// <summary>
	  /// Deletes the group. When no group exists with the given id, this operation
	  /// is ignored. </summary>
	  /// <param name="groupId"> id of the group that should be deleted, cannot be null. </param>
	  void deleteGroup(string groupId);

	  /// <param name="userId"> the userId, cannot be null. </param>
	  /// <param name="groupId"> the groupId, cannot be null. </param>
	  /// <exception cref="RuntimeException"> when the given user or group doesn't exist or when the user
	  /// is already member of the group. </exception>
	  void createMembership(string userId, string groupId);

	  /// <summary>
	  /// Delete the membership of the user in the group. When the group or user don't exist 
	  /// or when the user is not a member of the group, this operation is ignored. </summary>
	  /// <param name="userId"> the user's id, cannot be null. </param>
	  /// <param name="groupId"> the group's id, cannot be null. </param>
	  void deleteMembership(string userId, string groupId);

	  /// <summary>
	  /// Checks if the password is valid for the given user. Arguments userId
	  /// and password are nullsafe.
	  /// </summary>
	  bool checkPassword(string userId, string password);

	  /// <summary>
	  /// Passes the authenticated user id for this particular thread.
	  /// All service method (from any service) invocations done by the same
	  /// thread will have access to this authenticatedUserId. 
	  /// </summary>
	  string AuthenticatedUserId {set;}

	  /// <summary>
	  /// Sets the picture for a given user. </summary>
	  /// <exception cref="ActivitiObjectNotFoundException"> if the user doesn't exist. </exception>
	  /// <param name="picture"> can be null to delete the picture.  </param>
	  void setUserPicture(string userId, Picture picture);

	  /// <summary>
	  /// Retrieves the picture for a given user. </summary>
	  /// <exception cref="ActivitiObjectNotFoundException"> if the user doesn't exist.
	  /// @returns null if the user doesn't have a picture.  </exception>
	  Picture getUserPicture(string userId);

	  /// <summary>
	  /// Generic extensibility key-value pairs associated with a user </summary>
	  void setUserInfo(string userId, string key, string value);

	  /// <summary>
	  /// Generic extensibility key-value pairs associated with a user </summary>
	  string getUserInfo(string userId, string key);

	  /// <summary>
	  /// Generic extensibility keys associated with a user </summary>
	  IList<string> getUserInfoKeys(string userId);

	  /// <summary>
	  /// Delete an entry of the generic extensibility key-value pairs associated with a user </summary>
	  void deleteUserInfo(string userId, string key);
	}

}