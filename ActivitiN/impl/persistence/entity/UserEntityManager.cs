using System.Collections;
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


	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using Group = org.activiti.engine.identity.Group;
	using Picture = org.activiti.engine.identity.Picture;
	using User = org.activiti.engine.identity.User;
	using UserQuery = org.activiti.engine.identity.UserQuery;
	using Context = org.activiti.engine.impl.context.Context;
	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Saeid Mirzaei
	/// @author Joram Barrez
	/// </summary>
	public class UserEntityManager : AbstractManager, UserIdentityManager
	{

	  public virtual User createNewUser(string userId)
	  {
		return new UserEntity(userId);
	  }

	  public virtual void insertUser(User user)
	  {
		DbSqlSession.insert((PersistentObject) user);

		if (ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, user));
			ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_INITIALIZED, user));
		}
	  }

	  public virtual void updateUser(User updatedUser)
	  {
		CommandContext commandContext = Context.CommandContext;
		DbSqlSession dbSqlSession = commandContext.DbSqlSession;
		dbSqlSession.update((PersistentObject) updatedUser);

		if (ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, updatedUser));
		}
	  }

	  public virtual User findUserById(string userId)
	  {
		return (UserEntity) DbSqlSession.selectOne("selectUserById", userId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void deleteUser(String userId)
	  public virtual void deleteUser(string userId)
	  {
		UserEntity user = (UserEntity) findUserById(userId);
		if (user != null)
		{
		  IList<IdentityInfoEntity> identityInfos = DbSqlSession.selectList("selectIdentityInfoByUserId", userId);
		  foreach (IdentityInfoEntity identityInfo in identityInfos)
		  {
			IdentityInfoManager.deleteIdentityInfo(identityInfo);
		  }
		  DbSqlSession.delete("deleteMembershipsByUserId", userId);

		  user.delete();

		  if (ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
			  ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_DELETED, user));
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.identity.User> findUserByQueryCriteria(org.activiti.engine.impl.UserQueryImpl query, org.activiti.engine.impl.Page page)
	  public virtual IList<User> findUserByQueryCriteria(UserQueryImpl query, Page page)
	  {
		return DbSqlSession.selectList("selectUserByQueryCriteria", query, page);
	  }

	  public virtual long findUserCountByQueryCriteria(UserQueryImpl query)
	  {
		return (long?) DbSqlSession.selectOne("selectUserCountByQueryCriteria", query);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.identity.Group> findGroupsByUser(String userId)
	  public virtual IList<Group> findGroupsByUser(string userId)
	  {
		return DbSqlSession.selectList("selectGroupsByUserId", userId);
	  }

	  public virtual UserQuery createNewUserQuery()
	  {
		return new UserQueryImpl(Context.ProcessEngineConfiguration.CommandExecutor);
	  }

	  public virtual IdentityInfoEntity findUserInfoByUserIdAndKey(string userId, string key)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["userId"] = userId;
		parameters["key"] = key;
		return (IdentityInfoEntity) DbSqlSession.selectOne("selectIdentityInfoByUserIdAndKey", parameters);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) public java.util.List<String> findUserInfoKeysByUserIdAndType(String userId, String type)
	  public virtual IList<string> findUserInfoKeysByUserIdAndType(string userId, string type)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["userId"] = userId;
		parameters["type"] = type;
		return (IList) DbSqlSession.SqlSession.selectList("selectIdentityInfoKeysByUserIdAndType", parameters);
	  }

	  public virtual bool? checkPassword(string userId, string password)
	  {
		User user = findUserById(userId);
		if ((user != null) && (password != null) && (password.Equals(user.Password)))
		{
		  return true;
		}
		return false;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.identity.User> findPotentialStarterUsers(String proceDefId)
	  public virtual IList<User> findPotentialStarterUsers(string proceDefId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["procDefId"] = proceDefId;
		return (IList<User>) DbSqlSession.selectOne("selectUserByQueryCriteria", parameters);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.identity.User> findUsersByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<User> findUsersByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbSqlSession.selectListWithRawParameter("selectUserByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findUserCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbSqlSession.selectOne("selectUserCountByNativeQuery", parameterMap);
	  }

	  public override bool isNewUser(User user)
	  {
		  return ((UserEntity) user).Revision == 0;
	  }

	  public override Picture getUserPicture(string userId)
	  {
		  UserEntity user = (UserEntity) findUserById(userId);
		return user.Picture;
	  }

	  public override void setUserPicture(string userId, Picture picture)
	  {
		  UserEntity user = (UserEntity) findUserById(userId);
		  if (user == null)
		  {
			  throw new ActivitiObjectNotFoundException("user " + userId + " doesn't exist", typeof(User));
		  }

		user.Picture = picture;
	  }

	}

}