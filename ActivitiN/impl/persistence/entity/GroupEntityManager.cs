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
	using GroupQuery = org.activiti.engine.identity.GroupQuery;
	using Context = org.activiti.engine.impl.context.Context;
	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Saeid Mirzaei
	/// @author Joram Barrez
	/// </summary>
	public class GroupEntityManager : AbstractManager, GroupIdentityManager
	{

	  public virtual Group createNewGroup(string groupId)
	  {
		return new GroupEntity(groupId);
	  }

	  public virtual void insertGroup(Group group)
	  {
		DbSqlSession.insert((PersistentObject) group);

		if (ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, group));
			ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_INITIALIZED, group));
		}
	  }

	  public virtual void updateGroup(Group updatedGroup)
	  {
		CommandContext commandContext = Context.CommandContext;
		DbSqlSession dbSqlSession = commandContext.DbSqlSession;
		dbSqlSession.update((GroupEntity) updatedGroup);

		if (ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, updatedGroup));
		}
	  }

	  public virtual void deleteGroup(string groupId)
	  {
		GroupEntity group = DbSqlSession.selectById(typeof(GroupEntity), groupId);

		if (group != null)
		{
			if (ProcessEngineConfiguration.EventDispatcher.Enabled)
			{
			  ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createMembershipEvent(ActivitiEventType.MEMBERSHIPS_DELETED, groupId, null));
			}

			DbSqlSession.delete("deleteMembershipsByGroupId", groupId);
			DbSqlSession.delete(group);

			if (ProcessEngineConfiguration.EventDispatcher.Enabled)
			{
				ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_DELETED, group));
			}
		}
	  }

	  public virtual GroupQuery createNewGroupQuery()
	  {
		return new GroupQueryImpl(Context.ProcessEngineConfiguration.CommandExecutor);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.identity.Group> findGroupByQueryCriteria(org.activiti.engine.impl.GroupQueryImpl query, org.activiti.engine.impl.Page page)
	  public virtual IList<Group> findGroupByQueryCriteria(GroupQueryImpl query, Page page)
	  {
		return DbSqlSession.selectList("selectGroupByQueryCriteria", query, page);
	  }

	  public virtual long findGroupCountByQueryCriteria(GroupQueryImpl query)
	  {
		return (long?) DbSqlSession.selectOne("selectGroupCountByQueryCriteria", query);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.identity.Group> findGroupsByUser(String userId)
	  public virtual IList<Group> findGroupsByUser(string userId)
	  {
		return DbSqlSession.selectList("selectGroupsByUserId", userId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.identity.Group> findGroupsByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<Group> findGroupsByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbSqlSession.selectListWithRawParameter("selectGroupByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findGroupCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbSqlSession.selectOne("selectGroupCountByNativeQuery", parameterMap);
	  }

	  public override bool isNewGroup(Group group)
	  {
		return ((GroupEntity) group).Revision == 0;
	  }

	}

}