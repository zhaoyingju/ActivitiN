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


	using HistoricDetail = org.activiti.engine.history.HistoricDetail;
	using HistoryLevel = org.activiti.engine.impl.history.HistoryLevel;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class HistoricDetailEntityManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) public void deleteHistoricDetailsByProcessInstanceId(String historicProcessInstanceId)
	  public virtual void deleteHistoricDetailsByProcessInstanceId(string historicProcessInstanceId)
	  {
		if (HistoryManager.isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  IList<HistoricDetailEntity> historicDetails = (IList) DbSqlSession.createHistoricDetailQuery().processInstanceId(historicProcessInstanceId).list();

		  foreach (HistoricDetailEntity historicDetail in historicDetails)
		  {
			historicDetail.delete();
		  }
		}
	  }

	  public virtual long findHistoricDetailCountByQueryCriteria(HistoricDetailQueryImpl historicVariableUpdateQuery)
	  {
		return (long?) DbSqlSession.selectOne("selectHistoricDetailCountByQueryCriteria", historicVariableUpdateQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.history.HistoricDetail> findHistoricDetailsByQueryCriteria(org.activiti.engine.impl.HistoricDetailQueryImpl historicVariableUpdateQuery, org.activiti.engine.impl.Page page)
	  public virtual IList<HistoricDetail> findHistoricDetailsByQueryCriteria(HistoricDetailQueryImpl historicVariableUpdateQuery, Page page)
	  {
		return DbSqlSession.selectList("selectHistoricDetailsByQueryCriteria", historicVariableUpdateQuery, page);
	  }

	  public virtual void deleteHistoricDetailsByTaskId(string taskId)
	  {
		if (HistoryManager.isHistoryLevelAtLeast(HistoryLevel.FULL))
		{
		  HistoricDetailQueryImpl detailsQuery = (HistoricDetailQueryImpl) (new HistoricDetailQueryImpl()).taskId(taskId);
		  IList<HistoricDetail> details = detailsQuery.list();
		  foreach (HistoricDetail detail in details)
		  {
			((HistoricDetailEntity) detail).delete();
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.history.HistoricDetail> findHistoricDetailsByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<HistoricDetail> findHistoricDetailsByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbSqlSession.selectListWithRawParameter("selectHistoricDetailByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findHistoricDetailCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbSqlSession.selectOne("selectHistoricDetailCountByNativeQuery", parameterMap);
	  }
	}

}