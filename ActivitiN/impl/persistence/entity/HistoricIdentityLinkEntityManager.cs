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


	using Context = org.activiti.engine.impl.context.Context;


	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class HistoricIdentityLinkEntityManager : AbstractManager
	{

	  public virtual void deleteHistoricIdentityLink(HistoricIdentityLinkEntity identityLink)
	  {
		DbSqlSession.delete(identityLink);
	  }

	  public virtual void deleteHistoricIdentityLink(string id)
	  {
		DbSqlSession.delete("deleteHistoricIdentityLink", id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<HistoricIdentityLinkEntity> findHistoricIdentityLinksByTaskId(String taskId)
	  public virtual IList<HistoricIdentityLinkEntity> findHistoricIdentityLinksByTaskId(string taskId)
	  {
		return DbSqlSession.selectList("selectHistoricIdentityLinksByTask", taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<HistoricIdentityLinkEntity> findHistoricIdentityLinksByProcessInstanceId(String processInstanceId)
	  public virtual IList<HistoricIdentityLinkEntity> findHistoricIdentityLinksByProcessInstanceId(string processInstanceId)
	  {
		return DbSqlSession.selectList("selectHistoricIdentityLinksByProcessInstance", processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<HistoricIdentityLinkEntity> findHistoricIdentityLinksByProcessDefinitionId(String processDefinitionId)
	  public virtual IList<HistoricIdentityLinkEntity> findHistoricIdentityLinksByProcessDefinitionId(string processDefinitionId)
	  {
		return DbSqlSession.selectList("selectHistoricIdentityLinksByProcessDefinition", processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<HistoricIdentityLinkEntity> findHistoricIdentityLinks()
	  public virtual IList<HistoricIdentityLinkEntity> findHistoricIdentityLinks()
	  {
		return DbSqlSession.selectList("selectHistoricIdentityLinks");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<HistoricIdentityLinkEntity> findHistoricIdentityLinkByTaskUserGroupAndType(String taskId, String userId, String groupId, String type)
	  public virtual IList<HistoricIdentityLinkEntity> findHistoricIdentityLinkByTaskUserGroupAndType(string taskId, string userId, string groupId, string type)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["taskId"] = taskId;
		parameters["userId"] = userId;
		parameters["groupId"] = groupId;
		parameters["type"] = type;
		return DbSqlSession.selectList("selectHistoricIdentityLinkByTaskUserGroupAndType", parameters);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<HistoricIdentityLinkEntity> findHistoricIdentityLinkByProcessDefinitionUserAndGroup(String processDefinitionId, String userId, String groupId)
	  public virtual IList<HistoricIdentityLinkEntity> findHistoricIdentityLinkByProcessDefinitionUserAndGroup(string processDefinitionId, string userId, string groupId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["processDefinitionId"] = processDefinitionId;
		parameters["userId"] = userId;
		parameters["groupId"] = groupId;
		return DbSqlSession.selectList("selectHistoricIdentityLinkByProcessDefinitionUserAndGroup", parameters);
	  }

	  public virtual void deleteHistoricIdentityLinksByTaskId(string taskId)
	  {
		IList<HistoricIdentityLinkEntity> identityLinks = findHistoricIdentityLinksByTaskId(taskId);
		foreach (HistoricIdentityLinkEntity identityLink in identityLinks)
		{
		  deleteHistoricIdentityLink(identityLink);
		}
	  }

	  public virtual void deleteHistoricIdentityLinksByProcInstance(string processInstanceId)
	  {

		// Identity links from db
		IList<HistoricIdentityLinkEntity> identityLinks = findHistoricIdentityLinksByProcessInstanceId(processInstanceId);
		// Delete
		foreach (HistoricIdentityLinkEntity identityLink in identityLinks)
		{
		  deleteHistoricIdentityLink(identityLink);
		}

		// Identity links from cache
		IList<HistoricIdentityLinkEntity> identityLinksFromCache = Context.CommandContext.DbSqlSession.findInCache(typeof(HistoricIdentityLinkEntity));
		foreach (HistoricIdentityLinkEntity identityLinkEntity in identityLinksFromCache)
		{
		  if (processInstanceId.Equals(identityLinkEntity.ProcessInstanceId))
		  {
			deleteHistoricIdentityLink(identityLinkEntity);
		  }
		}
	  }

	  public virtual void deleteHistoricIdentityLinksByProcDef(string processDefId)
	  {
		DbSqlSession.delete("deleteHistoricIdentityLinkByProcDef", processDefId);
	  }
	}

}