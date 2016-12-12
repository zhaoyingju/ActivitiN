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


	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using Execution = org.activiti.engine.runtime.Execution;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class ExecutionEntityManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void deleteProcessInstancesByProcessDefinition(String processDefinitionId, String deleteReason, boolean cascade)
	  public virtual void deleteProcessInstancesByProcessDefinition(string processDefinitionId, string deleteReason, bool cascade)
	  {
		IList<string> processInstanceIds = DbSqlSession.selectList("selectProcessInstanceIdsByProcessDefinitionId", processDefinitionId);

		foreach (string processInstanceId in processInstanceIds)
		{
		  deleteProcessInstance(processInstanceId, deleteReason, cascade);
		}

		if (cascade)
		{
		  Context.CommandContext.HistoricProcessInstanceEntityManager.deleteHistoricProcessInstanceByProcessDefinitionId(processDefinitionId);
		}
	  }

	  public virtual void deleteProcessInstance(string processInstanceId, string deleteReason)
	  {
		deleteProcessInstance(processInstanceId, deleteReason, false);
	  }

	  public virtual void deleteProcessInstance(string processInstanceId, string deleteReason, bool cascade)
	  {
		ExecutionEntity execution = findExecutionById(processInstanceId);

		if (execution == null)
		{
		  throw new ActivitiObjectNotFoundException("No process instance found for id '" + processInstanceId + "'", typeof(ProcessInstance));
		}

		deleteProcessInstanceCascade(execution, deleteReason, cascade);
	  }

	  private void deleteProcessInstanceCascade(ExecutionEntity execution, string deleteReason, bool deleteHistory)
	  {
		CommandContext commandContext = Context.CommandContext;

		ProcessInstanceQueryImpl processInstanceQuery = new ProcessInstanceQueryImpl(commandContext);
		IList<ProcessInstance> subProcesses = processInstanceQuery.superProcessInstanceId(execution.ProcessInstanceId).list();
		foreach (ProcessInstance subProcess in subProcesses)
		{
		  deleteProcessInstanceCascade((ExecutionEntity) subProcess, deleteReason, deleteHistory);
		}

		commandContext.TaskEntityManager.deleteTasksByProcessInstanceId(execution.Id, deleteReason, deleteHistory);

		// fill default reason if none provided
		if (deleteReason == null)
		{
		  deleteReason = "ACTIVITY_DELETED";
		}

		if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled && execution.ProcessInstanceType)
		{
		  commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createCancelledEvent(execution.ProcessInstanceId, execution.ProcessInstanceId, execution.ProcessDefinitionId, deleteReason));
		}

		// delete the execution BEFORE we delete the history, otherwise we will produce orphan HistoricVariableInstance instances
		execution.deleteCascade(deleteReason);

		if (deleteHistory)
		{
		  commandContext.HistoricProcessInstanceEntityManager.deleteHistoricProcessInstanceById(execution.Id);
		}
	  }

	  public virtual ExecutionEntity findSubProcessInstanceBySuperExecutionId(string superExecutionId)
	  {
		return (ExecutionEntity) DbSqlSession.selectOne("selectSubProcessInstanceBySuperExecutionId", superExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ExecutionEntity> findChildExecutionsByParentExecutionId(String parentExecutionId)
	  public virtual IList<ExecutionEntity> findChildExecutionsByParentExecutionId(string parentExecutionId)
	  {
		return DbSqlSession.selectList("selectExecutionsByParentExecutionId", parentExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ExecutionEntity> findChildExecutionsByProcessInstanceId(String processInstanceId)
	  public virtual IList<ExecutionEntity> findChildExecutionsByProcessInstanceId(string processInstanceId)
	  {
		return DbSqlSession.selectList("selectExecutionsByProcessInstanceId", processInstanceId);
	  }

	  public virtual ExecutionEntity findExecutionById(string executionId)
	  {
		return DbSqlSession.selectById(typeof(ExecutionEntity), executionId);
	  }

	  public virtual long findExecutionCountByQueryCriteria(ExecutionQueryImpl executionQuery)
	  {
		return (long?) DbSqlSession.selectOne("selectExecutionCountByQueryCriteria", executionQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ExecutionEntity> findExecutionsByQueryCriteria(org.activiti.engine.impl.ExecutionQueryImpl executionQuery, org.activiti.engine.impl.Page page)
	  public virtual IList<ExecutionEntity> findExecutionsByQueryCriteria(ExecutionQueryImpl executionQuery, Page page)
	  {
		return DbSqlSession.selectList("selectExecutionsByQueryCriteria", executionQuery, page);
	  }

	  public virtual long findProcessInstanceCountByQueryCriteria(ProcessInstanceQueryImpl executionQuery)
	  {
		return (long?) DbSqlSession.selectOne("selectProcessInstanceCountByQueryCriteria", executionQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.runtime.ProcessInstance> findProcessInstanceByQueryCriteria(org.activiti.engine.impl.ProcessInstanceQueryImpl executionQuery)
	  public virtual IList<ProcessInstance> findProcessInstanceByQueryCriteria(ProcessInstanceQueryImpl executionQuery)
	  {
		return DbSqlSession.selectList("selectProcessInstanceByQueryCriteria", executionQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.runtime.ProcessInstance> findProcessInstanceAndVariablesByQueryCriteria(org.activiti.engine.impl.ProcessInstanceQueryImpl executionQuery)
	  public virtual IList<ProcessInstance> findProcessInstanceAndVariablesByQueryCriteria(ProcessInstanceQueryImpl executionQuery)
	  {
		// paging doesn't work for combining process instances and variables due to an outer join, so doing it in-memory
		if (executionQuery.FirstResult < 0 || executionQuery.MaxResults <= 0)
		{
		  return Collections.EMPTY_LIST;
		}

		int firstResult = executionQuery.FirstResult;
		int maxResults = executionQuery.MaxResults;

		// setting max results, limit to 20000 results for performance reasons
		if (executionQuery.ProcessInstanceVariablesLimit != null)
		{
		  executionQuery.MaxResults = executionQuery.ProcessInstanceVariablesLimit;
		}
		else
		{
		  executionQuery.MaxResults = Context.ProcessEngineConfiguration.ExecutionQueryLimit;
		}
		executionQuery.FirstResult = 0;

		IList<ProcessInstance> instanceList = DbSqlSession.selectListWithRawParameterWithoutFilter("selectProcessInstanceWithVariablesByQueryCriteria", executionQuery, executionQuery.FirstResult, executionQuery.MaxResults);

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
		return Collections.EMPTY_LIST;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ExecutionEntity> findEventScopeExecutionsByActivityId(String activityRef, String parentExecutionId)
	  public virtual IList<ExecutionEntity> findEventScopeExecutionsByActivityId(string activityRef, string parentExecutionId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["activityId"] = activityRef;
		parameters["parentExecutionId"] = parentExecutionId;

		return DbSqlSession.selectList("selectExecutionsByParentExecutionId", parameters);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.runtime.Execution> findExecutionsByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<Execution> findExecutionsByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbSqlSession.selectListWithRawParameter("selectExecutionByNativeQuery", parameterMap, firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.runtime.ProcessInstance> findProcessInstanceByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<ProcessInstance> findProcessInstanceByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbSqlSession.selectListWithRawParameter("selectExecutionByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findExecutionCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbSqlSession.selectOne("selectExecutionCountByNativeQuery", parameterMap);
	  }

	  public virtual void updateExecutionTenantIdForDeployment(string deploymentId, string newTenantId)
	  {
		  Dictionary<string, object> @params = new Dictionary<string, object>();
		  @params["deploymentId"] = deploymentId;
		  @params["tenantId"] = newTenantId;
		  DbSqlSession.update("updateExecutionTenantIdForDeployment", @params);
	  }

	  public virtual void updateProcessInstanceLockTime(string processInstanceId)
	  {
		CommandContext commandContext = Context.CommandContext;
		DateTime expirationTime = commandContext.ProcessEngineConfiguration.Clock.CurrentTime;
		int lockMillis = commandContext.ProcessEngineConfiguration.AsyncExecutor.AsyncJobLockTimeInMillis;
		GregorianCalendar lockCal = new GregorianCalendar();
		lockCal.Time = expirationTime;
		lockCal.add(DateTime.MILLISECOND, lockMillis);

		Dictionary<string, object> @params = new Dictionary<string, object>();
		@params["id"] = processInstanceId;
		@params["lockTime"] = lockCal.Time;
		@params["expirationTime"] = expirationTime;

		int result = DbSqlSession.update("updateProcessInstanceLockTime", @params);
		if (result == 0)
		{
			throw new ActivitiOptimisticLockingException("Could not lock process instance");
		}
	  }

	  public virtual void clearProcessInstanceLockTime(string processInstanceId)
	  {
		Dictionary<string, object> @params = new Dictionary<string, object>();
		@params["id"] = processInstanceId;

		DbSqlSession.update("clearProcessInstanceLockTime", @params);
	  }

	}

}