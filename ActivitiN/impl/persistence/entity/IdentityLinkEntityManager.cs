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
	using Context = org.activiti.engine.impl.context.Context;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Saeid Mirzaei
	/// </summary>
	public class IdentityLinkEntityManager : AbstractManager
	{

	  public virtual void deleteIdentityLink(IdentityLinkEntity identityLink, bool cascadeHistory)
	  {
		DbSqlSession.delete(identityLink);
		if (cascadeHistory)
		{
		  HistoryManager.deleteHistoricIdentityLink(identityLink.Id);
		}

		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_DELETED, identityLink));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<IdentityLinkEntity> findIdentityLinksByTaskId(String taskId)
	  public virtual IList<IdentityLinkEntity> findIdentityLinksByTaskId(string taskId)
	  {
		return DbSqlSession.selectList("selectIdentityLinksByTask", taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<IdentityLinkEntity> findIdentityLinksByProcessInstanceId(String processInstanceId)
	  public virtual IList<IdentityLinkEntity> findIdentityLinksByProcessInstanceId(string processInstanceId)
	  {
		return DbSqlSession.selectList("selectIdentityLinksByProcessInstance", processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<IdentityLinkEntity> findIdentityLinksByProcessDefinitionId(String processDefinitionId)
	  public virtual IList<IdentityLinkEntity> findIdentityLinksByProcessDefinitionId(string processDefinitionId)
	  {
		return DbSqlSession.selectList("selectIdentityLinksByProcessDefinition", processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<IdentityLinkEntity> findIdentityLinks()
	  public virtual IList<IdentityLinkEntity> findIdentityLinks()
	  {
		return DbSqlSession.selectList("selectIdentityLinks");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<IdentityLinkEntity> findIdentityLinkByTaskUserGroupAndType(String taskId, String userId, String groupId, String type)
	  public virtual IList<IdentityLinkEntity> findIdentityLinkByTaskUserGroupAndType(string taskId, string userId, string groupId, string type)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["taskId"] = taskId;
		parameters["userId"] = userId;
		parameters["groupId"] = groupId;
		parameters["type"] = type;
		return DbSqlSession.selectList("selectIdentityLinkByTaskUserGroupAndType", parameters);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<IdentityLinkEntity> findIdentityLinkByProcessInstanceUserGroupAndType(String processInstanceId, String userId, String groupId, String type)
	  public virtual IList<IdentityLinkEntity> findIdentityLinkByProcessInstanceUserGroupAndType(string processInstanceId, string userId, string groupId, string type)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["processInstanceId"] = processInstanceId;
		parameters["userId"] = userId;
		parameters["groupId"] = groupId;
		parameters["type"] = type;
		return DbSqlSession.selectList("selectIdentityLinkByProcessInstanceUserGroupAndType", parameters);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<IdentityLinkEntity> findIdentityLinkByProcessDefinitionUserAndGroup(String processDefinitionId, String userId, String groupId)
	  public virtual IList<IdentityLinkEntity> findIdentityLinkByProcessDefinitionUserAndGroup(string processDefinitionId, string userId, string groupId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["processDefinitionId"] = processDefinitionId;
		parameters["userId"] = userId;
		parameters["groupId"] = groupId;
		return DbSqlSession.selectList("selectIdentityLinkByProcessDefinitionUserAndGroup", parameters);
	  }

	  public virtual void deleteIdentityLinksByTaskId(string taskId)
	  {
		IList<IdentityLinkEntity> identityLinks = findIdentityLinksByTaskId(taskId);
		foreach (IdentityLinkEntity identityLink in identityLinks)
		{
		  deleteIdentityLink(identityLink, false);
		}
	  }

	  public virtual void deleteIdentityLinksByProcInstance(string processInstanceId)
	  {

		// Identity links from db
		IList<IdentityLinkEntity> identityLinks = findIdentityLinksByProcessInstanceId(processInstanceId);
		// Delete
		foreach (IdentityLinkEntity identityLink in identityLinks)
		{
		  deleteIdentityLink(identityLink, false);
		}

		// Identity links from cache, if not already deleted
		IList<IdentityLinkEntity> identityLinksFromCache = Context.CommandContext.DbSqlSession.findInCache(typeof(IdentityLinkEntity));
		bool alreadyDeleted = false;
		foreach (IdentityLinkEntity identityLinkEntity in identityLinksFromCache)
		{
		  if (processInstanceId.Equals(identityLinkEntity.ProcessInstanceId))
		  {
			  alreadyDeleted = false;
			  foreach (IdentityLinkEntity deleted in identityLinks)
			  {
				  if (deleted.Id != null && deleted.Id.Equals(identityLinkEntity.Id))
				  {
					  alreadyDeleted = true;
					  break;
				  }
			  }

			  if (!alreadyDeleted)
			  {
				  deleteIdentityLink(identityLinkEntity, false);
			  }
		  }
		}
	  }

	  public virtual void deleteIdentityLinksByProcDef(string processDefId)
	  {
		DbSqlSession.delete("deleteIdentityLinkByProcDef", processDefId);
	  }

	}

}