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


	using HistoricVariableInstance = org.activiti.engine.history.HistoricVariableInstance;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using HistoryLevel = org.activiti.engine.impl.history.HistoryLevel;


	/// <summary>
	/// @author Christian Lipphardt (camunda)
	/// </summary>
	public class HistoricVariableInstanceEntityManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) public void deleteHistoricVariableInstanceByProcessInstanceId(String historicProcessInstanceId)
	  public virtual void deleteHistoricVariableInstanceByProcessInstanceId(string historicProcessInstanceId)
	  {
		if (HistoryManager.isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{

		  // Delete entries in DB
		  IList<HistoricVariableInstanceEntity> historicProcessVariables = (IList) DbSqlSession.createHistoricVariableInstanceQuery().processInstanceId(historicProcessInstanceId).excludeVariableInitialization().list();
		  foreach (HistoricVariableInstanceEntity historicProcessVariable in historicProcessVariables)
		  {
			historicProcessVariable.delete();
		  }

		  // Delete entries in Cache
		  IList<HistoricVariableInstanceEntity> cachedHistoricVariableInstances = DbSqlSession.findInCache(typeof(HistoricVariableInstanceEntity));
		  foreach (HistoricVariableInstanceEntity historicProcessVariable in cachedHistoricVariableInstances)
		  {
			// Make sure we only delete the right ones (as we cannot make a proper query in the cache)
			if (historicProcessInstanceId.Equals(historicProcessVariable.ProcessInstanceId))
			{
			  historicProcessVariable.delete();
			}
		  }
		}
	  }

	  public virtual long findHistoricVariableInstanceCountByQueryCriteria(HistoricVariableInstanceQueryImpl historicProcessVariableQuery)
	  {
		return (long?) DbSqlSession.selectOne("selectHistoricVariableInstanceCountByQueryCriteria", historicProcessVariableQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.history.HistoricVariableInstance> findHistoricVariableInstancesByQueryCriteria(org.activiti.engine.impl.HistoricVariableInstanceQueryImpl historicProcessVariableQuery, org.activiti.engine.impl.Page page)
	  public virtual IList<HistoricVariableInstance> findHistoricVariableInstancesByQueryCriteria(HistoricVariableInstanceQueryImpl historicProcessVariableQuery, Page page)
	  {
		return DbSqlSession.selectList("selectHistoricVariableInstanceByQueryCriteria", historicProcessVariableQuery, page);
	  }

	  public virtual HistoricVariableInstanceEntity findHistoricVariableInstanceByVariableInstanceId(string variableInstanceId)
	  {
		return (HistoricVariableInstanceEntity) DbSqlSession.selectOne("selectHistoricVariableInstanceByVariableInstanceId", variableInstanceId);
	  }

	  public virtual void deleteHistoricVariableInstancesByTaskId(string taskId)
	  {
		if (HistoryManager.isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{
		  IList<HistoricVariableInstance> historicProcessVariables = (new HistoricVariableInstanceQueryImpl()).taskId(taskId).list();

		  foreach (HistoricVariableInstance historicProcessVariable in historicProcessVariables)
		  {
			((HistoricVariableInstanceEntity) historicProcessVariable).delete();
		  }
		}
	  }

	  public override void delete(PersistentObject persistentObject)
	  {
		HistoricVariableInstanceEntity variableInstanceEntity = (HistoricVariableInstanceEntity) persistentObject;
		variableInstanceEntity.delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.history.HistoricVariableInstance> findHistoricVariableInstancesByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<HistoricVariableInstance> findHistoricVariableInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbSqlSession.selectListWithRawParameter("selectHistoricVariableInstanceByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findHistoricVariableInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbSqlSession.selectOne("selectHistoricVariableInstanceCountByNativeQuery", parameterMap);
	  }
	}

}