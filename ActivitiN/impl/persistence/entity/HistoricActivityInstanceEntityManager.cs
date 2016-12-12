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


	using HistoricActivityInstance = org.activiti.engine.history.HistoricActivityInstance;
	using HistoryLevel = org.activiti.engine.impl.history.HistoryLevel;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class HistoricActivityInstanceEntityManager : AbstractManager
	{

	  public virtual void deleteHistoricActivityInstancesByProcessInstanceId(string historicProcessInstanceId)
	  {
		if (HistoryManager.isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{
		  DbSqlSession.delete("deleteHistoricActivityInstancesByProcessInstanceId", historicProcessInstanceId);
		}
	  }

	  public virtual void insertHistoricActivityInstance(HistoricActivityInstanceEntity historicActivityInstance)
	  {
		DbSqlSession.insert(historicActivityInstance);
	  }

	  public virtual HistoricActivityInstanceEntity findHistoricActivityInstance(string activityId, string processInstanceId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["activityId"] = activityId;
		parameters["processInstanceId"] = processInstanceId;

		return (HistoricActivityInstanceEntity) DbSqlSession.selectOne("selectHistoricActivityInstance", parameters);
	  }

	  public virtual long findHistoricActivityInstanceCountByQueryCriteria(HistoricActivityInstanceQueryImpl historicActivityInstanceQuery)
	  {
		return (long?) DbSqlSession.selectOne("selectHistoricActivityInstanceCountByQueryCriteria", historicActivityInstanceQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.history.HistoricActivityInstance> findHistoricActivityInstancesByQueryCriteria(org.activiti.engine.impl.HistoricActivityInstanceQueryImpl historicActivityInstanceQuery, org.activiti.engine.impl.Page page)
	  public virtual IList<HistoricActivityInstance> findHistoricActivityInstancesByQueryCriteria(HistoricActivityInstanceQueryImpl historicActivityInstanceQuery, Page page)
	  {
		return DbSqlSession.selectList("selectHistoricActivityInstancesByQueryCriteria", historicActivityInstanceQuery, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.history.HistoricActivityInstance> findHistoricActivityInstancesByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<HistoricActivityInstance> findHistoricActivityInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbSqlSession.selectListWithRawParameter("selectHistoricActivityInstanceByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findHistoricActivityInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbSqlSession.selectOne("selectHistoricActivityInstanceCountByNativeQuery", parameterMap);
	  }
	}

}