using System;
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


	using HistoricProcessInstance = org.activiti.engine.history.HistoricProcessInstance;
	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class HistoricProcessInstanceEntityManager : AbstractManager
	{

	  public virtual HistoricProcessInstanceEntity findHistoricProcessInstance(string processInstanceId)
	  {
		if (HistoryManager.HistoryEnabled)
		{
		  return (HistoricProcessInstanceEntity) DbSqlSession.selectById(typeof(HistoricProcessInstanceEntity), processInstanceId);
		}
		return null;
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void deleteHistoricProcessInstanceByProcessDefinitionId(String processDefinitionId)
	  public virtual void deleteHistoricProcessInstanceByProcessDefinitionId(string processDefinitionId)
	  {
		if (HistoryManager.HistoryEnabled)
		{
		  IList<string> historicProcessInstanceIds = DbSqlSession.selectList("selectHistoricProcessInstanceIdsByProcessDefinitionId", processDefinitionId);

		  foreach (string historicProcessInstanceId in historicProcessInstanceIds)
		  {
			deleteHistoricProcessInstanceById(historicProcessInstanceId);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void deleteHistoricProcessInstanceById(String historicProcessInstanceId)
	  public virtual void deleteHistoricProcessInstanceById(string historicProcessInstanceId)
	  {
		if (HistoryManager.HistoryEnabled)
		{
		  CommandContext commandContext = Context.CommandContext;
		  HistoricProcessInstanceEntity historicProcessInstance = findHistoricProcessInstance(historicProcessInstanceId);

		  commandContext.HistoricDetailEntityManager.deleteHistoricDetailsByProcessInstanceId(historicProcessInstanceId);

		  commandContext.HistoricVariableInstanceEntityManager.deleteHistoricVariableInstanceByProcessInstanceId(historicProcessInstanceId);

		  commandContext.HistoricActivityInstanceEntityManager.deleteHistoricActivityInstancesByProcessInstanceId(historicProcessInstanceId);

		  commandContext.HistoricTaskInstanceEntityManager.deleteHistoricTaskInstancesByProcessInstanceId(historicProcessInstanceId);

		  commandContext.HistoricIdentityLinkEntityManager.deleteHistoricIdentityLinksByProcInstance(historicProcessInstanceId);

		  commandContext.CommentEntityManager.deleteCommentsByProcessInstanceId(historicProcessInstanceId);

		  DbSqlSession.delete(historicProcessInstance);

		  // Also delete any sub-processes that may be active (ACT-821)
		  HistoricProcessInstanceQueryImpl subProcessesQueryImpl = new HistoricProcessInstanceQueryImpl();
		  subProcessesQueryImpl.superProcessInstanceId(historicProcessInstanceId);

		  IList<HistoricProcessInstance> selectList = DbSqlSession.selectList("selectHistoricProcessInstancesByQueryCriteria", subProcessesQueryImpl);
		  foreach (HistoricProcessInstance child in selectList)
		  {
			  deleteHistoricProcessInstanceById(child.Id);
		  }
		}
	  }

	  public virtual long findHistoricProcessInstanceCountByQueryCriteria(HistoricProcessInstanceQueryImpl historicProcessInstanceQuery)
	  {
		if (HistoryManager.HistoryEnabled)
		{
		  return (long?) DbSqlSession.selectOne("selectHistoricProcessInstanceCountByQueryCriteria", historicProcessInstanceQuery);
		}
		return 0;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.history.HistoricProcessInstance> findHistoricProcessInstancesByQueryCriteria(org.activiti.engine.impl.HistoricProcessInstanceQueryImpl historicProcessInstanceQuery)
	  public virtual IList<HistoricProcessInstance> findHistoricProcessInstancesByQueryCriteria(HistoricProcessInstanceQueryImpl historicProcessInstanceQuery)
	  {
		if (HistoryManager.HistoryEnabled)
		{
		  return DbSqlSession.selectList("selectHistoricProcessInstancesByQueryCriteria", historicProcessInstanceQuery);
		}
		return Collections.EMPTY_LIST;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.history.HistoricProcessInstance> findHistoricProcessInstancesAndVariablesByQueryCriteria(org.activiti.engine.impl.HistoricProcessInstanceQueryImpl historicProcessInstanceQuery)
	  public virtual IList<HistoricProcessInstance> findHistoricProcessInstancesAndVariablesByQueryCriteria(HistoricProcessInstanceQueryImpl historicProcessInstanceQuery)
	  {
		if (HistoryManager.HistoryEnabled)
		{
		  // paging doesn't work for combining process instances and variables due to an outer join, so doing it in-memory
		  if (historicProcessInstanceQuery.FirstResult < 0 || historicProcessInstanceQuery.MaxResults <= 0)
		  {
			return Collections.EMPTY_LIST;
		  }

		  int firstResult = historicProcessInstanceQuery.FirstResult;
		  int maxResults = historicProcessInstanceQuery.MaxResults;

		  // setting max results, limit to 20000 results for performance reasons
		  if (historicProcessInstanceQuery.ProcessInstanceVariablesLimit != null)
		  {
			historicProcessInstanceQuery.MaxResults = historicProcessInstanceQuery.ProcessInstanceVariablesLimit;
		  }
		  else
		  {
			historicProcessInstanceQuery.MaxResults = Context.ProcessEngineConfiguration.HistoricProcessInstancesQueryLimit;
		  }
		  historicProcessInstanceQuery.FirstResult = 0;

		  IList<HistoricProcessInstance> instanceList = DbSqlSession.selectListWithRawParameterWithoutFilter("selectHistoricProcessInstancesWithVariablesByQueryCriteria", historicProcessInstanceQuery, historicProcessInstanceQuery.FirstResult, historicProcessInstanceQuery.MaxResults);

		  if (instanceList != null && instanceList.Count > 0)
		  {
			if (firstResult > 0)
			{
			  if (firstResult <= instanceList.Count)
			  {
				int toIndex = firstResult + Math.Min(maxResults, instanceList.Count - firstResult);
				return instanceList.subList(firstResult, toIndex);
			  }
			  else
			  {
				return Collections.EMPTY_LIST;
			  }
			}
			else
			{
			  int toIndex = Math.Min(maxResults, instanceList.Count);
			  return instanceList.subList(0, toIndex);
			}
		  }
		}
		return Collections.EMPTY_LIST;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.history.HistoricProcessInstance> findHistoricProcessInstancesByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<HistoricProcessInstance> findHistoricProcessInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbSqlSession.selectListWithRawParameter("selectHistoricProcessInstanceByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findHistoricProcessInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbSqlSession.selectOne("selectHistoricProcessInstanceCountByNativeQuery", parameterMap);
	  }
	}

}