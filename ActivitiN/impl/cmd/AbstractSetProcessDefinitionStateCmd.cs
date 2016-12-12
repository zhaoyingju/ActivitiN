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
namespace org.activiti.engine.impl.cmd
{


	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using JobHandler = org.activiti.engine.impl.jobexecutor.JobHandler;
	using TimerChangeProcessDefinitionSuspensionStateJobHandler = org.activiti.engine.impl.jobexecutor.TimerChangeProcessDefinitionSuspensionStateJobHandler;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinitionEntityManager = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntityManager;
	using SuspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState;
	using SuspensionState_SuspensionStateUtil = org.activiti.engine.impl.persistence.entity.SuspensionState_SuspensionStateUtil;
	using TimerEntity = org.activiti.engine.impl.persistence.entity.TimerEntity;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public abstract class AbstractSetProcessDefinitionStateCmd : Command<Void>
	{

	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal ProcessDefinitionEntity processDefinitionEntity;
	  protected internal bool includeProcessInstances = false;
	  protected internal DateTime executionDate;
	  protected internal string tenantId;

	  public AbstractSetProcessDefinitionStateCmd(ProcessDefinitionEntity processDefinitionEntity, bool includeProcessInstances, DateTime executionDate, string tenantId)
	  {
		this.processDefinitionEntity = processDefinitionEntity;
		this.includeProcessInstances = includeProcessInstances;
		this.executionDate = executionDate;
		this.tenantId = tenantId;
	  }

	  public AbstractSetProcessDefinitionStateCmd(string processDefinitionId, string processDefinitionKey, bool includeProcessInstances, DateTime executionDate, string tenantId)
	  {
		this.processDefinitionId = processDefinitionId;
		this.processDefinitionKey = processDefinitionKey;
		this.includeProcessInstances = includeProcessInstances;
		this.executionDate = executionDate;
		this.tenantId = tenantId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {

		IList<ProcessDefinitionEntity> processDefinitions = findProcessDefinition(commandContext);

		if (executionDate != null) // Process definition state change is delayed
		{
		  createTimerForDelayedExecution(commandContext, processDefinitions);
		} // Process definition state is changed now
		else
		{
		  changeProcessDefinitionState(commandContext, processDefinitions);
		}

		return null;
	  }

	  protected internal virtual IList<ProcessDefinitionEntity> findProcessDefinition(CommandContext commandContext)
	  {

		// If process definition is already provided (eg. when command is called through the DeployCmd) 
		// we don't need to do an extra database fetch and we can simply return it, wrapped in a list
		if (processDefinitionEntity != null)
		{
		  return Arrays.asList(processDefinitionEntity);
		}

		// Validation of input parameters
		if (processDefinitionId == null && processDefinitionKey == null)
		{
		  throw new ActivitiIllegalArgumentException("Process definition id or key cannot be null");
		}

		IList<ProcessDefinitionEntity> processDefinitionEntities = new List<ProcessDefinitionEntity>();
		ProcessDefinitionEntityManager processDefinitionManager = commandContext.ProcessDefinitionEntityManager;

		if (processDefinitionId != null)
		{

		  ProcessDefinitionEntity processDefinitionEntity = processDefinitionManager.findProcessDefinitionById(processDefinitionId);
		  if (processDefinitionEntity == null)
		  {
			throw new ActivitiObjectNotFoundException("Cannot find process definition for id '" + processDefinitionId + "'", typeof(ProcessDefinition));
		  }
		  processDefinitionEntities.Add(processDefinitionEntity);

		}
		else
		{

		   ProcessDefinitionQueryImpl query = (new ProcessDefinitionQueryImpl(commandContext)).processDefinitionKey(processDefinitionKey);

		  if (tenantId == null || ProcessEngineConfiguration.NO_TENANT_ID.Equals(tenantId))
		  {
			  query.processDefinitionWithoutTenantId();
		  }
		  else
		  {
			  query.processDefinitionTenantId(tenantId);
		  }

		  IList<ProcessDefinition> processDefinitions = query.list();
		  if (processDefinitions.Count == 0)
		  {
			throw new ActivitiException("Cannot find process definition for key '" + processDefinitionKey + "'");
		  }

		  foreach (ProcessDefinition processDefinition in processDefinitions)
		  {
			processDefinitionEntities.Add((ProcessDefinitionEntity) processDefinition);
		  }

		}
		return processDefinitionEntities;
	  }

	  protected internal virtual void createTimerForDelayedExecution(CommandContext commandContext, IList<ProcessDefinitionEntity> processDefinitions)
	  {
		foreach (ProcessDefinitionEntity processDefinition in processDefinitions)
		{
		  TimerEntity timer = new TimerEntity();
		  timer.ProcessDefinitionId = processDefinition.Id;

		  // Inherit tenant identifier (if applicable)
		  if (processDefinition.TenantId != null)
		  {
			  timer.TenantId = processDefinition.TenantId;
		  }

		  timer.Duedate = executionDate;
		  timer.JobHandlerType = DelayedExecutionJobHandlerType;
		  timer.JobHandlerConfiguration = TimerChangeProcessDefinitionSuspensionStateJobHandler.createJobHandlerConfiguration(includeProcessInstances);
		  commandContext.JobEntityManager.schedule(timer);
		}
	  }

	  protected internal virtual void changeProcessDefinitionState(CommandContext commandContext, IList<ProcessDefinitionEntity> processDefinitions)
	  {
		foreach (ProcessDefinitionEntity processDefinition in processDefinitions)
		{

		  SuspensionState_SuspensionStateUtil.setSuspensionState(processDefinition, ProcessDefinitionSuspensionState);

		  // Evict cache
		  commandContext.ProcessEngineConfiguration.DeploymentManager.ProcessDefinitionCache.remove(processDefinition.Id);

		  // Suspend process instances (if needed)
		  if (includeProcessInstances)
		  {

			int currentStartIndex = 0;
			IList<ProcessInstance> processInstances = fetchProcessInstancesPage(commandContext, processDefinition, currentStartIndex);
			while (processInstances.Count > 0)
			{

			  foreach (ProcessInstance processInstance in processInstances)
			  {
				AbstractSetProcessInstanceStateCmd processInstanceCmd = getProcessInstanceChangeStateCmd(processInstance);
				processInstanceCmd.execute(commandContext);
			  }

			  // Fetch new batch of process instances
			  currentStartIndex += processInstances.Count;
			  processInstances = fetchProcessInstancesPage(commandContext, processDefinition, currentStartIndex);
			}
		  }
		}
	  }

	  protected internal virtual IList<ProcessInstance> fetchProcessInstancesPage(CommandContext commandContext, ProcessDefinition processDefinition, int currentPageStartIndex)
	  {

		  if (org.activiti.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE.Equals(ProcessDefinitionSuspensionState))
		  {
				return (new ProcessInstanceQueryImpl(commandContext)).processDefinitionId(processDefinition.Id).suspended().listPage(currentPageStartIndex, commandContext.ProcessEngineConfiguration.BatchSizeProcessInstances);
		  }
			else
			{
				  return (new ProcessInstanceQueryImpl(commandContext)).processDefinitionId(processDefinition.Id).active().listPage(currentPageStartIndex, commandContext.ProcessEngineConfiguration.BatchSizeProcessInstances);
			}
	  }


	  // ABSTRACT METHODS ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Subclasses should return the wanted <seealso cref="SuspensionState"/> here.
	  /// </summary>
	  protected internal abstract SuspensionState ProcessDefinitionSuspensionState {get;}

	  /// <summary>
	  /// Subclasses should return the type of the <seealso cref="JobHandler"/> here. it will be used when
	  /// the user provides an execution date on which the actual state change will happen.
	  /// </summary>
	  protected internal abstract string DelayedExecutionJobHandlerType {get;}

	  /// <summary>
	  /// Subclasses should return a <seealso cref="Command"/> implementation that matches the process definition
	  /// state change.
	  /// </summary>
	  protected internal abstract AbstractSetProcessInstanceStateCmd getProcessInstanceChangeStateCmd(ProcessInstance processInstance);

	}

}