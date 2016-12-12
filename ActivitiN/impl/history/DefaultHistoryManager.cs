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

namespace org.activiti.engine.impl.history
{


	using Expression = org.activiti.engine.@delegate.Expression;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using HistoricActivityInstance = org.activiti.engine.history.HistoricActivityInstance;
	using IdGenerator = org.activiti.engine.impl.cfg.IdGenerator;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.activiti.engine.impl.context.Context;
	using Authentication = org.activiti.engine.impl.identity.Authentication;
	using AbstractManager = org.activiti.engine.impl.persistence.AbstractManager;
	using CommentEntity = org.activiti.engine.impl.persistence.entity.CommentEntity;
	using CommentEntityManager = org.activiti.engine.impl.persistence.entity.CommentEntityManager;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using HistoricActivityInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricActivityInstanceEntity;
	using HistoricDetailVariableInstanceUpdateEntity = org.activiti.engine.impl.persistence.entity.HistoricDetailVariableInstanceUpdateEntity;
	using HistoricFormPropertyEntity = org.activiti.engine.impl.persistence.entity.HistoricFormPropertyEntity;
	using HistoricIdentityLinkEntity = org.activiti.engine.impl.persistence.entity.HistoricIdentityLinkEntity;
	using HistoricProcessInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricProcessInstanceEntity;
	using HistoricTaskInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricTaskInstanceEntity;
	using HistoricVariableInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using IdentityLinkEntity = org.activiti.engine.impl.persistence.entity.IdentityLinkEntity;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;
	using VariableInstanceEntity = org.activiti.engine.impl.persistence.entity.VariableInstanceEntity;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;
	using Event = org.activiti.engine.task.Event;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// Manager class that centralises recording of all history-related operations
	/// that are originated from inside the engine.
	/// 
	/// @author Frederik Heremans
	/// @author Joram Barrez
	/// </summary>
	public class DefaultHistoryManager : AbstractManager, HistoryManager
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static Logger log = LoggerFactory.getLogger(typeof(DefaultHistoryManager).FullName);

	  private HistoryLevel historyLevel;

	  public DefaultHistoryManager()
	  {
		this.historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#isHistoryLevelAtLeast(org.activiti.engine.impl.history.HistoryLevel)
	   */
	  public override bool isHistoryLevelAtLeast(HistoryLevel level)
	  {
		if (log.DebugEnabled)
		{
		  log.debug("Current history level: {}, level required: {}", historyLevel, level);
		}
		// Comparing enums actually compares the location of values declared in the enum
		return historyLevel.isAtLeast(level);
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#isHistoryEnabled()
	   */
	  public override bool HistoryEnabled
	  {
		  get
		  {
			if (log.DebugEnabled)
			{
			  log.debug("Current history level: {}", historyLevel);
			}
			return !historyLevel.Equals(HistoryLevel.NONE);
		  }
	  }

	  // Process related history

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordProcessInstanceEnd(java.lang.String, java.lang.String, java.lang.String)
	   */
	  public override void recordProcessInstanceEnd(string processInstanceId, string deleteReason, string activityId)
	  {

		if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{
		  HistoricProcessInstanceEntity historicProcessInstance = HistoricProcessInstanceManager.findHistoricProcessInstance(processInstanceId);

		  if (historicProcessInstance != null)
		  {
			historicProcessInstance.markEnded(deleteReason);
			historicProcessInstance.EndActivityId = activityId;

			// Fire event
			ProcessEngineConfigurationImpl config = Context.ProcessEngineConfiguration;
				if (config != null && config.EventDispatcher.Enabled)
				{
					config.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.HISTORIC_PROCESS_INSTANCE_ENDED, historicProcessInstance));
				}
		  }
		}
	  }

	  public override void recordProcessInstanceNameChange(string processInstanceId, string newName)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{
		  HistoricProcessInstanceEntity historicProcessInstance = HistoricProcessInstanceManager.findHistoricProcessInstance(processInstanceId);

		  if (historicProcessInstance != null)
		  {
			historicProcessInstance.Name = newName;
		  }
		}
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordProcessInstanceStart(org.activiti.engine.impl.persistence.entity.ExecutionEntity)
	   */
	  public override void recordProcessInstanceStart(ExecutionEntity processInstance)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{
		  HistoricProcessInstanceEntity historicProcessInstance = new HistoricProcessInstanceEntity(processInstance);

		  // Insert historic process-instance
		  DbSqlSession.insert(historicProcessInstance);

		  // Fire event
		  ProcessEngineConfigurationImpl config = Context.ProcessEngineConfiguration;
			  if (config != null && config.EventDispatcher.Enabled)
			  {
				  config.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.HISTORIC_PROCESS_INSTANCE_CREATED, historicProcessInstance));
			  }

		  // Also record the start-event manually, as there is no "start" activity history listener for this
		  IdGenerator idGenerator = Context.ProcessEngineConfiguration.IdGenerator;

		  string processDefinitionId = processInstance.ProcessDefinitionId;
		  string processInstanceId = processInstance.ProcessInstanceId;
		  string executionId = processInstance.Id;

		  HistoricActivityInstanceEntity historicActivityInstance = new HistoricActivityInstanceEntity();
		  historicActivityInstance.Id = idGenerator.NextId;
		  historicActivityInstance.ProcessDefinitionId = processDefinitionId;
		  historicActivityInstance.ProcessInstanceId = processInstanceId;
		  historicActivityInstance.ExecutionId = executionId;
		  historicActivityInstance.ActivityId = processInstance.ActivityId;
		  historicActivityInstance.ActivityName = (string) processInstance.Activity.getProperty("name");
		  historicActivityInstance.ActivityType = (string) processInstance.Activity.getProperty("type");
		  DateTime now = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		  historicActivityInstance.StartTime = now;

		  // Inherit tenant id (if applicable)
		  if (processInstance.TenantId != null)
		  {
			  historicActivityInstance.TenantId = processInstance.TenantId;
		  }

		  DbSqlSession.insert(historicActivityInstance);

		  // Fire event
			  if (config != null && config.EventDispatcher.Enabled)
			  {
				  config.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.HISTORIC_ACTIVITY_INSTANCE_CREATED, historicActivityInstance));
			  }
		}
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordSubProcessInstanceStart(org.activiti.engine.impl.persistence.entity.ExecutionEntity, org.activiti.engine.impl.persistence.entity.ExecutionEntity)
	   */
	  public override void recordSubProcessInstanceStart(ExecutionEntity parentExecution, ExecutionEntity subProcessInstance)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{

		  HistoricProcessInstanceEntity historicProcessInstance = new HistoricProcessInstanceEntity((ExecutionEntity) subProcessInstance);

		  ActivityImpl initialActivity = subProcessInstance.Activity;
		  // Fix for ACT-1728: startActivityId not initialized with subprocess-instance
		  if (historicProcessInstance.StartActivityId == null)
		  {
			  historicProcessInstance.StartActivityId = subProcessInstance.ProcessDefinition.Initial.Id;
			  initialActivity = subProcessInstance.ProcessDefinition.Initial;
		  }
		  DbSqlSession.insert(historicProcessInstance);

		  // Fire event
		  ProcessEngineConfigurationImpl config = Context.ProcessEngineConfiguration;
			  if (config != null && config.EventDispatcher.Enabled)
			  {
				  config.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.HISTORIC_PROCESS_INSTANCE_CREATED, historicProcessInstance));
			  }


		  HistoricActivityInstanceEntity activitiyInstance = findActivityInstance(parentExecution);
		  if (activitiyInstance != null)
		  {
			activitiyInstance.CalledProcessInstanceId = subProcessInstance.ProcessInstanceId;
		  }

		  // Fix for ACT-1728: start-event not recorded for subprocesses
		  IdGenerator idGenerator = Context.ProcessEngineConfiguration.IdGenerator;

		  // Also record the start-event manually, as there is no "start" activity history listener for this
		  HistoricActivityInstanceEntity historicActivityInstance = new HistoricActivityInstanceEntity();
		  historicActivityInstance.Id = idGenerator.NextId;
		  historicActivityInstance.ProcessDefinitionId = subProcessInstance.ProcessDefinitionId;
		  historicActivityInstance.ProcessInstanceId = subProcessInstance.ProcessInstanceId;
		  historicActivityInstance.ExecutionId = subProcessInstance.Id;
		  historicActivityInstance.ActivityId = initialActivity.Id;
		  historicActivityInstance.ActivityName = (string) initialActivity.getProperty("name");
		  historicActivityInstance.ActivityType = (string) initialActivity.getProperty("type");
		  DateTime now = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		  historicActivityInstance.StartTime = now;

		  DbSqlSession.insert(historicActivityInstance);

		  // Fire event
			  if (config != null && config.EventDispatcher.Enabled)
			  {
				  config.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.HISTORIC_ACTIVITY_INSTANCE_CREATED, historicActivityInstance));
			  }
		}
	  }

	  // Activity related history

	  /* (non-Javadoc)
	 * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordActivityStart(org.activiti.engine.impl.persistence.entity.ExecutionEntity)
	 */
	  public override void recordActivityStart(ExecutionEntity executionEntity)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{
			if (executionEntity.Activity != null)
			{
				IdGenerator idGenerator = Context.ProcessEngineConfiguration.IdGenerator;

				string processDefinitionId = executionEntity.ProcessDefinitionId;
				string processInstanceId = executionEntity.ProcessInstanceId;
				string executionId = executionEntity.Id;

				HistoricActivityInstanceEntity historicActivityInstance = new HistoricActivityInstanceEntity();
				historicActivityInstance.Id = idGenerator.NextId;
				historicActivityInstance.ProcessDefinitionId = processDefinitionId;
				historicActivityInstance.ProcessInstanceId = processInstanceId;
				historicActivityInstance.ExecutionId = executionId;
				historicActivityInstance.ActivityId = executionEntity.ActivityId;
				historicActivityInstance.ActivityName = (string) executionEntity.Activity.getProperty("name");
				historicActivityInstance.ActivityType = (string) executionEntity.Activity.getProperty("type");
				historicActivityInstance.StartTime = Context.ProcessEngineConfiguration.Clock.CurrentTime;

			  // Inherit tenant id (if applicable)
			if (executionEntity.TenantId != null)
			{
				historicActivityInstance.TenantId = executionEntity.TenantId;
			}

				DbSqlSession.insert(historicActivityInstance);

			// Fire event
			ProcessEngineConfigurationImpl config = Context.ProcessEngineConfiguration;
				if (config != null && config.EventDispatcher.Enabled)
				{
					config.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.HISTORIC_ACTIVITY_INSTANCE_CREATED, historicActivityInstance));
				}
			}
		}
	  }

	  /* (non-Javadoc)
	  * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordActivityEnd(org.activiti.engine.impl.persistence.entity.ExecutionEntity)
	  */
	  public override void recordActivityEnd(ExecutionEntity executionEntity)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{
		  HistoricActivityInstanceEntity historicActivityInstance = findActivityInstance(executionEntity);
		  if (historicActivityInstance != null)
		  {
			endHistoricActivityInstance(historicActivityInstance);
		  }
		}
	  }

	  protected internal virtual void endHistoricActivityInstance(HistoricActivityInstanceEntity historicActivityInstance)
	  {
		historicActivityInstance.markEnded(null);

		// Fire event
		ProcessEngineConfigurationImpl config = Context.ProcessEngineConfiguration;
		if (config != null && config.EventDispatcher.Enabled)
		{
			config.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.HISTORIC_ACTIVITY_INSTANCE_ENDED, historicActivityInstance));
		}
	  }

	  /* (non-Javadoc)
	  * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordStartEventEnded(java.lang.String, java.lang.String)
	  */
	  public override void recordStartEventEnded(ExecutionEntity execution, string activityId)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{

		  // Interrupted executions might not have an activityId set, skip recording history.
		  if (activityId == null)
		  {
			return;
		  }

		  HistoricActivityInstanceEntity historicActivityInstance = findActivityInstance(execution, activityId, false); // false -> no need to check the persistent store, as process just started
		  if (historicActivityInstance != null)
		  {
			endHistoricActivityInstance(historicActivityInstance);
		  }
		}
	  }

	  public override HistoricActivityInstanceEntity findActivityInstance(ExecutionEntity execution)
	  {
		return findActivityInstance(execution, execution.ActivityId, true);
	  }

	  /*
	   * (non-Javadoc)
	   * 
	   * @see
	   * org.activiti.engine.impl.history.HistoryManagerInterface#findActivityInstance
	   * (org.activiti.engine.impl.persistence.entity.ExecutionEntity)
	   */
	  protected internal virtual HistoricActivityInstanceEntity findActivityInstance(ExecutionEntity execution, string activityId, bool checkPersistentStore)
	  {

		string executionId = execution.Id;

		// search for the historic activity instance in the dbsqlsession cache
		IList<HistoricActivityInstanceEntity> cachedHistoricActivityInstances = DbSqlSession.findInCache(typeof(HistoricActivityInstanceEntity));
		foreach (HistoricActivityInstanceEntity cachedHistoricActivityInstance in cachedHistoricActivityInstances)
		{
		  if (executionId.Equals(cachedHistoricActivityInstance.ExecutionId) && activityId != null && (activityId.Equals(cachedHistoricActivityInstance.ActivityId)) && (cachedHistoricActivityInstance.EndTime == null))
		  {
			return cachedHistoricActivityInstance;
		  }
		}

		IList<HistoricActivityInstance> historicActivityInstances = null;
		if (checkPersistentStore)
		{
		  historicActivityInstances = (new HistoricActivityInstanceQueryImpl(Context.CommandContext)).executionId(executionId).activityId(activityId).unfinished().listPage(0, 1);
		}

		if (historicActivityInstances != null && historicActivityInstances.Count > 0)
		{
		  return (HistoricActivityInstanceEntity) historicActivityInstances[0];
		}

		if (execution.ParentId != null)
		{
		  return findActivityInstance((ExecutionEntity) execution.getParent(), activityId, checkPersistentStore);
		}

		return null;
	  }

	  /* (non-Javadoc)
	 * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordExecutionReplacedBy(org.activiti.engine.impl.persistence.entity.ExecutionEntity, org.activiti.engine.impl.pvm.runtime.InterpretableExecution)
	 */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings({ "unchecked", "rawtypes" }) public void recordExecutionReplacedBy(org.activiti.engine.impl.persistence.entity.ExecutionEntity execution, org.activiti.engine.impl.pvm.runtime.InterpretableExecution replacedBy)
	  public override void recordExecutionReplacedBy(ExecutionEntity execution, InterpretableExecution replacedBy)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{

		  // Update the cached historic activity instances that are open
		  IList<HistoricActivityInstanceEntity> cachedHistoricActivityInstances = DbSqlSession.findInCache(typeof(HistoricActivityInstanceEntity));
		  foreach (HistoricActivityInstanceEntity cachedHistoricActivityInstance in cachedHistoricActivityInstances)
		  {
			if ((cachedHistoricActivityInstance.EndTime == null) && (execution.Id.Equals(cachedHistoricActivityInstance.ExecutionId)))
			{
			  cachedHistoricActivityInstance.ExecutionId = replacedBy.Id;
			}
		  }

		  // Update the persisted historic activity instances that are open
		  IList<HistoricActivityInstanceEntity> historicActivityInstances = (IList) (new HistoricActivityInstanceQueryImpl(Context.CommandContext)).executionId(execution.Id).unfinished().list();
		  foreach (HistoricActivityInstanceEntity historicActivityInstance in historicActivityInstances)
		  {
			historicActivityInstance.ExecutionId = replacedBy.Id;
		  }
		}
	  }
	  /* (non-Javadoc)
	 * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordProcessDefinitionChange(java.lang.String, java.lang.String)
	 */
	  public override void recordProcessDefinitionChange(string processInstanceId, string processDefinitionId)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{
		  HistoricProcessInstanceEntity historicProcessInstance = HistoricProcessInstanceManager.findHistoricProcessInstance(processInstanceId);
		  if (historicProcessInstance != null)
		  {
			historicProcessInstance.ProcessDefinitionId = processDefinitionId;
		  }
		}
	  }


	  // Task related history 

	  /* (non-Javadoc)
	 * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskCreated(org.activiti.engine.impl.persistence.entity.TaskEntity, org.activiti.engine.impl.persistence.entity.ExecutionEntity)
	 */
	  public override void recordTaskCreated(TaskEntity task, ExecutionEntity execution)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = new HistoricTaskInstanceEntity(task, execution);
		  DbSqlSession.insert(historicTaskInstance);
		}
	  }

	  /* (non-Javadoc)
	 * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskAssignment(org.activiti.engine.impl.persistence.entity.TaskEntity)
	 */
	  public override void recordTaskAssignment(TaskEntity task)
	  {
		ExecutionEntity executionEntity = task.getExecution();
		if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{
		  if (executionEntity != null)
		  {
			HistoricActivityInstanceEntity historicActivityInstance = findActivityInstance(executionEntity);
			if (historicActivityInstance != null)
			{
			  historicActivityInstance.Assignee = task.Assignee;
			}
		  }
		}
	  }

	  /* (non-Javadoc)
	 * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskClaim(java.lang.String)
	 */

	  public override void recordTaskClaim(string taskId)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), taskId);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.ClaimTime = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		  }
		}
	  }


	  /* (non-Javadoc)
	  * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskId(org.activiti.engine.impl.persistence.entity.TaskEntity)
	  */
	  public override void recordTaskId(TaskEntity task)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{
		  ExecutionEntity execution = task.getExecution();
		  if (execution != null)
		  {
			HistoricActivityInstanceEntity historicActivityInstance = findActivityInstance(execution);
			if (historicActivityInstance != null)
			{
			  historicActivityInstance.TaskId = task.Id;
			}
		  }
		}
	  }

	  /* (non-Javadoc)
	  * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskEnd(java.lang.String, java.lang.String)
	  */
	  public override void recordTaskEnd(string taskId, string deleteReason)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), taskId);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.markEnded(deleteReason);
		  }
		}
	  }

	  /* (non-Javadoc)
	  * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskAssigneeChange(java.lang.String, java.lang.String)
	  */
	  public override void recordTaskAssigneeChange(string taskId, string assignee)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), taskId);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.Assignee = assignee;
		  }
		}
	  }

	  /* (non-Javadoc)
	  * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskOwnerChange(java.lang.String, java.lang.String)
	  */
	  public override void recordTaskOwnerChange(string taskId, string owner)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), taskId);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.Owner = owner;
		  }
		}
	  }

	  /* (non-Javadoc)
	  * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskNameChange(java.lang.String, java.lang.String)
	  */
	  public override void recordTaskNameChange(string taskId, string taskName)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), taskId);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.Name = taskName;
		  }
		}
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskDescriptionChange(java.lang.String, java.lang.String)
	   */
	  public override void recordTaskDescriptionChange(string taskId, string description)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), taskId);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.Description = description;
		  }
		}
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskDueDateChange(java.lang.String, java.util.Date)
	   */
	  public override void recordTaskDueDateChange(string taskId, DateTime dueDate)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), taskId);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.DueDate = dueDate;
		  }
		}
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskPriorityChange(java.lang.String, int)
	   */
	  public override void recordTaskPriorityChange(string taskId, int priority)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), taskId);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.Priority = priority;
		  }
		}
	  }

	 /* (non-Javadoc)
	  * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskCategoryChange(java.lang.String, java.lang.String)
	  */
	  public override void recordTaskCategoryChange(string taskId, string category)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), taskId);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.Category = category;
		  }
		}
	  }

	  public override void recordTaskFormKeyChange(string taskId, string formKey)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), taskId);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.FormKey = formKey;
		  }
		}
	  }


	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskParentTaskIdChange(java.lang.String, java.lang.String)
	   */
	  public override void recordTaskParentTaskIdChange(string taskId, string parentTaskId)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), taskId);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.ParentTaskId = parentTaskId;
		  }
		}
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskExecutionIdChange(java.lang.String, java.lang.String)
	   */
	  public override void recordTaskExecutionIdChange(string taskId, string executionId)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), taskId);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.ExecutionId = executionId;
		  }
		}
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskDefinitionKeyChange(org.activiti.engine.impl.persistence.entity.TaskEntity, java.lang.String)
	   */
	  public override void recordTaskDefinitionKeyChange(TaskEntity task, string taskDefinitionKey)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), task.Id);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.TaskDefinitionKey = taskDefinitionKey;

			if (taskDefinitionKey != null)
			{
			  Expression taskFormExpression = task.TaskDefinition.FormKeyExpression;
			  if (taskFormExpression != null)
			  {
				object formValue = taskFormExpression.getValue(task.getExecution());
				if (formValue != null)
				{
				  historicTaskInstance.FormKey = formValue.ToString();
				}
			  }
			}
		  }
		}
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordTaskProcessDefinitionChange(java.lang.String, java.lang.String)
	   */
	  public override void recordTaskProcessDefinitionChange(string taskId, string processDefinitionId)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{
		  HistoricTaskInstanceEntity historicTaskInstance = DbSqlSession.selectById(typeof(HistoricTaskInstanceEntity), taskId);
		  if (historicTaskInstance != null)
		  {
			historicTaskInstance.ProcessDefinitionId = processDefinitionId;
		  }
		}
	  }

	  // Variables related history

	  /* (non-Javadoc)
	  * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordVariableCreate(org.activiti.engine.impl.persistence.entity.VariableInstanceEntity)
	  */
	  public override void recordVariableCreate(VariableInstanceEntity variable)
	  {
		// Historic variables
		if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
		{
		  HistoricVariableInstanceEntity.copyAndInsert(variable);
		}
	  }

	  /* (non-Javadoc)
	  * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordHistoricDetailVariableCreate(org.activiti.engine.impl.persistence.entity.VariableInstanceEntity, org.activiti.engine.impl.persistence.entity.ExecutionEntity, boolean)
	  */
	  public override void recordHistoricDetailVariableCreate(VariableInstanceEntity variable, ExecutionEntity sourceActivityExecution, bool useActivityId)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.FULL))
		{

		  HistoricDetailVariableInstanceUpdateEntity historicVariableUpdate = HistoricDetailVariableInstanceUpdateEntity.copyAndInsert(variable);

		  if (useActivityId && sourceActivityExecution != null)
		  {
			HistoricActivityInstanceEntity historicActivityInstance = findActivityInstance(sourceActivityExecution);
			if (historicActivityInstance != null)
			{
			  historicVariableUpdate.ActivityInstanceId = historicActivityInstance.Id;
			}
		  }
		}
	  }

		/*
		 * (non-Javadoc)
		 * 
		 * @see
		 * org.activiti.engine.impl.history.HistoryManagerInterface#recordVariableUpdate
		 * (org.activiti.engine.impl.persistence.entity.VariableInstanceEntity)
		 */
		public override void recordVariableUpdate(VariableInstanceEntity variable)
		{
			if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
			{
				HistoricVariableInstanceEntity historicProcessVariable = DbSqlSession.findInCache(typeof(HistoricVariableInstanceEntity), variable.Id);
				if (historicProcessVariable == null)
				{
					historicProcessVariable = Context.CommandContext.HistoricVariableInstanceEntityManager.findHistoricVariableInstanceByVariableInstanceId(variable.Id);
				}

				if (historicProcessVariable != null)
				{
					historicProcessVariable.copyValue(variable);
				}
				else
				{
					HistoricVariableInstanceEntity.copyAndInsert(variable);
				}
			}
		}

		public override void recordVariableRemoved(VariableInstanceEntity variable)
		{
			if (isHistoryLevelAtLeast(HistoryLevel.ACTIVITY))
			{
				HistoricVariableInstanceEntity historicProcessVariable = DbSqlSession.findInCache(typeof(HistoricVariableInstanceEntity), variable.Id);
				if (historicProcessVariable == null)
				{
					historicProcessVariable = Context.CommandContext.HistoricVariableInstanceEntityManager.findHistoricVariableInstanceByVariableInstanceId(variable.Id);
				}

				if (historicProcessVariable != null)
				{
					Context.CommandContext.HistoricVariableInstanceEntityManager.delete(historicProcessVariable);
				}
			}
		}

	  // Comment related history

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#createIdentityLinkComment(java.lang.String, java.lang.String, java.lang.String, java.lang.String, boolean)
	   */
	  public override void createIdentityLinkComment(string taskId, string userId, string groupId, string type, bool create)
	  {
		createIdentityLinkComment(taskId, userId, groupId, type, create, false);
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#createIdentityLinkComment(java.lang.String, java.lang.String, java.lang.String, java.lang.String, boolean, boolean)
	   */
	  public override void createIdentityLinkComment(string taskId, string userId, string groupId, string type, bool create, bool forceNullUserId)
	  {
		if (HistoryEnabled)
		{
		  string authenticatedUserId = Authentication.AuthenticatedUserId;
		  CommentEntity comment = new CommentEntity();
		  comment.UserId = authenticatedUserId;
		  comment.Type = CommentEntity.TYPE_EVENT;
		  comment.Time = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		  comment.TaskId = taskId;
		  if (userId != null || forceNullUserId)
		  {
			if (create)
			{
			  comment.Action = org.activiti.engine.task.Event_Fields.ACTION_ADD_USER_LINK;
			}
			else
			{
			  comment.Action = org.activiti.engine.task.Event_Fields.ACTION_DELETE_USER_LINK;
			}
			comment.Message = new string[]{userId, type};
		  }
		  else
		  {
			if (create)
			{
			  comment.Action = org.activiti.engine.task.Event_Fields.ACTION_ADD_GROUP_LINK;
			}
			else
			{
			  comment.Action = org.activiti.engine.task.Event_Fields.ACTION_DELETE_GROUP_LINK;
			}
			comment.Message = new string[]{groupId, type};
		  }
		  getSession(typeof(CommentEntityManager)).insert(comment);
		}
	  }

	  public override void createProcessInstanceIdentityLinkComment(string processInstanceId, string userId, string groupId, string type, bool create)
	  {
		createProcessInstanceIdentityLinkComment(processInstanceId, userId, groupId, type, create, false);
	  }

	  public override void createProcessInstanceIdentityLinkComment(string processInstanceId, string userId, string groupId, string type, bool create, bool forceNullUserId)
	  {
		if (HistoryEnabled)
		{
		  string authenticatedUserId = Authentication.AuthenticatedUserId;
		  CommentEntity comment = new CommentEntity();
		  comment.UserId = authenticatedUserId;
		  comment.Type = CommentEntity.TYPE_EVENT;
		  comment.Time = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		  comment.ProcessInstanceId = processInstanceId;
		  if (userId != null || forceNullUserId)
		  {
			if (create)
			{
			  comment.Action = org.activiti.engine.task.Event_Fields.ACTION_ADD_USER_LINK;
			}
			else
			{
			  comment.Action = org.activiti.engine.task.Event_Fields.ACTION_DELETE_USER_LINK;
			}
			comment.Message = new string[]{userId, type};
		  }
		  else
		  {
			if (create)
			{
			  comment.Action = org.activiti.engine.task.Event_Fields.ACTION_ADD_GROUP_LINK;
			}
			else
			{
			  comment.Action = org.activiti.engine.task.Event_Fields.ACTION_DELETE_GROUP_LINK;
			}
			comment.Message = new string[]{groupId, type};
		  }
		  getSession(typeof(CommentEntityManager)).insert(comment);
		}
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#createAttachmentComment(java.lang.String, java.lang.String, java.lang.String, boolean)
	   */
	  public override void createAttachmentComment(string taskId, string processInstanceId, string attachmentName, bool create)
	  {
		if (HistoryEnabled)
		{
		  string userId = Authentication.AuthenticatedUserId;
		  CommentEntity comment = new CommentEntity();
		  comment.UserId = userId;
		  comment.Type = CommentEntity.TYPE_EVENT;
		  comment.Time = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		  comment.TaskId = taskId;
		  comment.ProcessInstanceId = processInstanceId;
		  if (create)
		  {
			comment.Action = org.activiti.engine.task.Event_Fields.ACTION_ADD_ATTACHMENT;
		  }
		  else
		  {
			comment.Action = org.activiti.engine.task.Event_Fields.ACTION_DELETE_ATTACHMENT;
		  }
		  comment.Message = attachmentName;
		  getSession(typeof(CommentEntityManager)).insert(comment);
		}
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#reportFormPropertiesSubmitted(org.activiti.engine.impl.persistence.entity.ExecutionEntity, java.util.Map, java.lang.String)
	   */
	  public override void reportFormPropertiesSubmitted(ExecutionEntity processInstance, IDictionary<string, string> properties, string taskId)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  foreach (string propertyId in properties.Keys)
		  {
			string propertyValue = properties[propertyId];
			HistoricFormPropertyEntity historicFormProperty = new HistoricFormPropertyEntity(processInstance, propertyId, propertyValue, taskId);
			DbSqlSession.insert(historicFormProperty);
		  }
		}
	  }

	  // Identity link related history
	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#recordIdentityLinkCreated(org.activiti.engine.impl.persistence.entity.IdentityLinkEntity)
	   */
	  public override void recordIdentityLinkCreated(IdentityLinkEntity identityLink)
	  {
		// It makes no sense storing historic counterpart for an identity-link that is related
		// to a process-definition only as this is never kept in history
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT) && (identityLink.ProcessInstanceId != null || identityLink.TaskId != null))
		{
		  HistoricIdentityLinkEntity historicIdentityLinkEntity = new HistoricIdentityLinkEntity(identityLink);
		  DbSqlSession.insert(historicIdentityLinkEntity);
		}
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#deleteHistoricIdentityLink(java.lang.String)
	   */
	  public override void deleteHistoricIdentityLink(string id)
	  {
		if (isHistoryLevelAtLeast(HistoryLevel.AUDIT))
		{
		  HistoricIdentityLinkEntityManager.deleteHistoricIdentityLink(id);
		}
	  }

	  /* (non-Javadoc)
	   * @see org.activiti.engine.impl.history.HistoryManagerInterface#updateProcessBusinessKeyInHistory(org.activiti.engine.impl.persistence.entity.ExecutionEntity)
	   */
	  public override void updateProcessBusinessKeyInHistory(ExecutionEntity processInstance)
	  {
			if (HistoryEnabled)
			{
				if (log.DebugEnabled)
				{
					log.debug("updateProcessBusinessKeyInHistory : {}",processInstance.Id);
				}
				if (processInstance != null)
				{
					HistoricProcessInstanceEntity historicProcessInstance = DbSqlSession.selectById(typeof(HistoricProcessInstanceEntity), processInstance.Id);
					if (historicProcessInstance != null)
					{
						historicProcessInstance.BusinessKey = processInstance.ProcessBusinessKey;
						DbSqlSession.update(historicProcessInstance);
					}
				}
			}
	  }

	}

}