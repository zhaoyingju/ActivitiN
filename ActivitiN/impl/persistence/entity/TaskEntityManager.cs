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


	using TaskListener = org.activiti.engine.@delegate.TaskListener;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using UserTaskActivityBehavior = org.activiti.engine.impl.bpmn.behavior.UserTaskActivityBehavior;
	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using Task = org.activiti.engine.task.Task;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class TaskEntityManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) public void deleteTasksByProcessInstanceId(String processInstanceId, String deleteReason, boolean cascade)
	  public virtual void deleteTasksByProcessInstanceId(string processInstanceId, string deleteReason, bool cascade)
	  {
		IList<TaskEntity> tasks = (IList) DbSqlSession.createTaskQuery().processInstanceId(processInstanceId).list();

		string reason = (deleteReason == null || deleteReason.Length == 0) ? TaskEntity.DELETE_REASON_DELETED : deleteReason;

		CommandContext commandContext = Context.CommandContext;

		foreach (TaskEntity task in tasks)
		{
		  if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createActivityCancelledEvent(task.getExecution().ActivityId, task.Name, task.ExecutionId, task.ProcessInstanceId, task.ProcessDefinitionId, "userTask", typeof(UserTaskActivityBehavior).FullName, deleteReason));
		  }

		  deleteTask(task, reason, cascade);
		}
	  }

	  public virtual void deleteTask(TaskEntity task, string deleteReason, bool cascade)
	  {
		if (!task.Deleted)
		{
			 task.fireEvent(org.activiti.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE);
		  task.Deleted = true;

		  CommandContext commandContext = Context.CommandContext;
		  string taskId = task.Id;

		  IList<Task> subTasks = findTasksByParentTaskId(taskId);
		  foreach (Task subTask in subTasks)
		  {
			deleteTask((TaskEntity) subTask, deleteReason, cascade);
		  }

		  commandContext.IdentityLinkEntityManager.deleteIdentityLinksByTaskId(taskId);

		  commandContext.VariableInstanceEntityManager.deleteVariableInstanceByTask(task);

		  if (cascade)
		  {
			commandContext.HistoricTaskInstanceEntityManager.deleteHistoricTaskInstanceById(taskId);
		  }
		  else
		  {
			commandContext.HistoryManager.recordTaskEnd(taskId, deleteReason);
		  }

		  DbSqlSession.delete(task);

		  if (commandContext.EventDispatcher.Enabled)
		  {
			  commandContext.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_DELETED, task));
		  }
		}
	  }


	  public virtual TaskEntity findTaskById(string id)
	  {
		if (id == null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid task id : null");
		}
		return (TaskEntity) DbSqlSession.selectById(typeof(TaskEntity), id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<TaskEntity> findTasksByExecutionId(String executionId)
	  public virtual IList<TaskEntity> findTasksByExecutionId(string executionId)
	  {
		return DbSqlSession.selectList("selectTasksByExecutionId", executionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<TaskEntity> findTasksByProcessInstanceId(String processInstanceId)
	  public virtual IList<TaskEntity> findTasksByProcessInstanceId(string processInstanceId)
	  {
		return DbSqlSession.selectList("selectTasksByProcessInstanceId", processInstanceId);
	  }

	  [Obsolete]
	  public virtual IList<Task> findTasksByQueryCriteria(TaskQueryImpl taskQuery, Page page)
	  {
		taskQuery.FirstResult = page.FirstResult;
		taskQuery.MaxResults = page.MaxResults;
		return findTasksByQueryCriteria(taskQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.task.Task> findTasksByQueryCriteria(org.activiti.engine.impl.TaskQueryImpl taskQuery)
	  public virtual IList<Task> findTasksByQueryCriteria(TaskQueryImpl taskQuery)
	  {
		const string query = "selectTaskByQueryCriteria";
		return DbSqlSession.selectList(query, taskQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.task.Task> findTasksAndVariablesByQueryCriteria(org.activiti.engine.impl.TaskQueryImpl taskQuery)
	  public virtual IList<Task> findTasksAndVariablesByQueryCriteria(TaskQueryImpl taskQuery)
	  {
		const string query = "selectTaskWithVariablesByQueryCriteria";
		// paging doesn't work for combining task instances and variables due to an outer join, so doing it in-memory
		if (taskQuery.FirstResult < 0 || taskQuery.MaxResults <= 0)
		{
		  return Collections.EMPTY_LIST;
		}

		int firstResult = taskQuery.FirstResult;
		int maxResults = taskQuery.MaxResults;

		// setting max results, limit to 20000 results for performance reasons
		if (taskQuery.TaskVariablesLimit != null)
		{
		  taskQuery.MaxResults = taskQuery.TaskVariablesLimit;
		}
		else
		{
		  taskQuery.MaxResults = Context.ProcessEngineConfiguration.TaskQueryLimit;
		}
		taskQuery.FirstResult = 0;

		IList<Task> instanceList = DbSqlSession.selectListWithRawParameterWithoutFilter(query, taskQuery, taskQuery.FirstResult, taskQuery.MaxResults);

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

	  public virtual long findTaskCountByQueryCriteria(TaskQueryImpl taskQuery)
	  {
		return (long?) DbSqlSession.selectOne("selectTaskCountByQueryCriteria", taskQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.task.Task> findTasksByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<Task> findTasksByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbSqlSession.selectListWithRawParameter("selectTaskByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findTaskCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbSqlSession.selectOne("selectTaskCountByNativeQuery", parameterMap);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.task.Task> findTasksByParentTaskId(String parentTaskId)
	  public virtual IList<Task> findTasksByParentTaskId(string parentTaskId)
	  {
		return DbSqlSession.selectList("selectTasksByParentTaskId", parentTaskId);
	  }

	  public virtual void deleteTask(string taskId, string deleteReason, bool cascade)
	  {
		TaskEntity task = Context.CommandContext.TaskEntityManager.findTaskById(taskId);

		if (task != null)
		{
		  if (task.ExecutionId != null)
		  {
			throw new ActivitiException("The task cannot be deleted because is part of a running process");
		  }

		  string reason = (deleteReason == null || deleteReason.Length == 0) ? TaskEntity.DELETE_REASON_DELETED : deleteReason;
		  deleteTask(task, reason, cascade);
		}
		else if (cascade)
		{
		  Context.CommandContext.HistoricTaskInstanceEntityManager.deleteHistoricTaskInstanceById(taskId);
		}
	  }

	  public virtual void updateTaskTenantIdForDeployment(string deploymentId, string newTenantId)
	  {
		  Dictionary<string, object> @params = new Dictionary<string, object>();
		  @params["deploymentId"] = deploymentId;
		  @params["tenantId"] = newTenantId;
		  DbSqlSession.update("updateTaskTenantIdForDeployment", @params);
	  }

	}

}