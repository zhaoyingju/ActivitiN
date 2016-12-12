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


	using HistoricTaskInstance = org.activiti.engine.history.HistoricTaskInstance;
	using Context = org.activiti.engine.impl.context.Context;
	using HistoryLevel = org.activiti.engine.impl.history.HistoryLevel;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class HistoricTaskInstanceEntityManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void deleteHistoricTaskInstancesByProcessInstanceId(String processInstanceId)
	  public virtual void deleteHistoricTaskInstancesByProcessInstanceId(string processInstanceId)
	  {
		if (HistoryManager.isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  IList<string> taskInstanceIds = (IList<string>) DbSqlSession.selectList("selectHistoricTaskInstanceIdsByProcessInstanceId", processInstanceId);
		  foreach (string taskInstanceId in taskInstanceIds)
		  {
			deleteHistoricTaskInstanceById(taskInstanceId);
		  }
		}
	  }

	  public virtual long findHistoricTaskInstanceCountByQueryCriteria(HistoricTaskInstanceQueryImpl historicTaskInstanceQuery)
	  {
		if (HistoryManager.HistoryEnabled)
		{
		  return (long?) DbSqlSession.selectOne("selectHistoricTaskInstanceCountByQueryCriteria", historicTaskInstanceQuery);
		}
		return 0;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.history.HistoricTaskInstance> findHistoricTaskInstancesByQueryCriteria(org.activiti.engine.impl.HistoricTaskInstanceQueryImpl historicTaskInstanceQuery)
	  public virtual IList<HistoricTaskInstance> findHistoricTaskInstancesByQueryCriteria(HistoricTaskInstanceQueryImpl historicTaskInstanceQuery)
	  {
		if (HistoryManager.HistoryEnabled)
		{
		  return DbSqlSession.selectList("selectHistoricTaskInstancesByQueryCriteria", historicTaskInstanceQuery);
		}
		return Collections.EMPTY_LIST;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.history.HistoricTaskInstance> findHistoricTaskInstancesAndVariablesByQueryCriteria(org.activiti.engine.impl.HistoricTaskInstanceQueryImpl historicTaskInstanceQuery)
	  public virtual IList<HistoricTaskInstance> findHistoricTaskInstancesAndVariablesByQueryCriteria(HistoricTaskInstanceQueryImpl historicTaskInstanceQuery)
	  {
		if (HistoryManager.HistoryEnabled)
		{
		  // paging doesn't work for combining task instances and variables due to an outer join, so doing it in-memory
		  if (historicTaskInstanceQuery.FirstResult < 0 || historicTaskInstanceQuery.MaxResults <= 0)
		  {
			return Collections.EMPTY_LIST;
		  }

		  int firstResult = historicTaskInstanceQuery.FirstResult;
		  int maxResults = historicTaskInstanceQuery.MaxResults;

		  // setting max results, limit to 20000 results for performance reasons
		  if (historicTaskInstanceQuery.TaskVariablesLimit != null)
		  {
			historicTaskInstanceQuery.MaxResults = historicTaskInstanceQuery.TaskVariablesLimit;
		  }
		  else
		  {
			historicTaskInstanceQuery.MaxResults = Context.ProcessEngineConfiguration.HistoricTaskQueryLimit;
		  }
		  historicTaskInstanceQuery.FirstResult = 0;

		  IList<HistoricTaskInstance> instanceList = DbSqlSession.selectListWithRawParameterWithoutFilter("selectHistoricTaskInstancesWithVariablesByQueryCriteria", historicTaskInstanceQuery, historicTaskInstanceQuery.FirstResult, historicTaskInstanceQuery.MaxResults);

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

	  public virtual HistoricTaskInstanceEntity findHistoricTaskInstanceById(string taskId)
	  {
		if (taskId == null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid historic task id : null");
		}
		if (HistoryManager.HistoryEnabled)
		{
		  return (HistoricTaskInstanceEntity) DbSqlSession.selectOne("selectHistoricTaskInstance", taskId);
		}
		return null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.history.HistoricTaskInstance> findHistoricTasksByParentTaskId(String parentTaskId)
	  public virtual IList<HistoricTaskInstance> findHistoricTasksByParentTaskId(string parentTaskId)
	  {
		return DbSqlSession.selectList("selectHistoricTasksByParentTaskId", parentTaskId);
	  }

	  public virtual void deleteHistoricTaskInstanceById(string taskId)
	  {
		if (HistoryManager.HistoryEnabled)
		{
		  HistoricTaskInstanceEntity historicTaskInstance = findHistoricTaskInstanceById(taskId);
		  if (historicTaskInstance != null)
		  {
			CommandContext commandContext = Context.CommandContext;

			IList<HistoricTaskInstance> subTasks = findHistoricTasksByParentTaskId(taskId);
			foreach (HistoricTaskInstance subTask in subTasks)
			{
			  deleteHistoricTaskInstanceById(subTask.Id);
			}

			commandContext.HistoricDetailEntityManager.deleteHistoricDetailsByTaskId(taskId);

			commandContext.HistoricVariableInstanceEntityManager.deleteHistoricVariableInstancesByTaskId(taskId);

			commandContext.CommentEntityManager.deleteCommentsByTaskId(taskId);

			commandContext.AttachmentEntityManager.deleteAttachmentsByTaskId(taskId);

			commandContext.HistoricIdentityLinkEntityManager.deleteHistoricIdentityLinksByTaskId(taskId);

			DbSqlSession.delete(historicTaskInstance);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.history.HistoricTaskInstance> findHistoricTaskInstancesByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<HistoricTaskInstance> findHistoricTaskInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbSqlSession.selectListWithRawParameter("selectHistoricTaskInstanceByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findHistoricTaskInstanceCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbSqlSession.selectOne("selectHistoricTaskInstanceCountByNativeQuery", parameterMap);
	  }
	}

}