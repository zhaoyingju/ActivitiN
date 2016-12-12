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

namespace org.activiti.engine.impl.persistence.entity
{


	using Group = org.activiti.engine.identity.Group;
	using Picture = org.activiti.engine.identity.Picture;
	using User = org.activiti.engine.identity.User;
	using UserQuery = org.activiti.engine.identity.UserQuery;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public interface UserIdentityManager
	{

	  User createNewUser(string userId);

	  void insertUser(User user);

	  void updateUser(User updatedUser);

	  User findUserById(string userId);

	  void deleteUser(string userId);

	  IList<User> findUserByQueryCriteria(UserQueryImpl query, Page page);

	  long findUserCountByQueryCriteria(UserQueryImpl query);

	  IList<Group> findGroupsByUser(string userId);

	  UserQuery createNewUserQuery();

	  IdentityInfoEntity findUserInfoByUserIdAndKey(string userId, string key);

	  IList<string> findUserInfoKeysByUserIdAndType(string userId, string type);

	  bool? checkPassword(string userId, string password);

	  IList<User> findPotentialStarterUsers(string proceDefId);

	  IList<User> findUsersByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults);

	  long findUserCountByNativeQuery(IDictionary<string, object> parameterMap);

		bool isNewUser(User user);

		Picture getUserPicture(string userId);

		void setUserPicture(string userId, Picture picture);

	}

}