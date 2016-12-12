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


	using HistoricProcessInstance = org.activiti.engine.history.HistoricProcessInstance;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using DeploymentManager = org.activiti.engine.impl.persistence.deploy.DeploymentManager;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionEntityManager = org.activiti.engine.impl.persistence.entity.ExecutionEntityManager;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;
	using ProcessDefinitionImpl = org.activiti.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;


	/// <summary>
	/// <seealso cref="Command"/> that changes the process definition version of an existing
	/// process instance.
	/// 
	/// Warning: This command will NOT perform any migration magic and simply set the
	/// process definition version in the database, assuming that the user knows,
	/// what he or she is doing.
	/// 
	/// This is only useful for simple migrations. The new process definition MUST
	/// have the exact same activity id to make it still run.
	/// 
	/// Furthermore, activities referenced by sub-executions and jobs that belong to
	/// the process instance MUST exist in the new process definition version.
	/// 
	/// The command will fail, if there is already a <seealso cref="ProcessInstance"/> or
	/// <seealso cref="HistoricProcessInstance"/> using the new process definition version and
	/// the same business key as the <seealso cref="ProcessInstance"/> that is to be migrated.
	/// 
	/// If the process instance is not currently waiting but actively running, then
	/// this would be a case for optimistic locking, meaning either the version
	/// update or the "real work" wins, i.e., this is a race condition.
	/// </summary>
	/// <seealso cref= http://forums.activiti.org/en/viewtopic.php?t=2918
	/// @author Falko Menge </seealso>
	[Serializable]
	public class SetProcessDefinitionVersionCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  private readonly string processInstanceId;
	  private readonly int? processDefinitionVersion;

	  public SetProcessDefinitionVersionCmd(string processInstanceId, int? processDefinitionVersion)
	  {
		if (processInstanceId == null || processInstanceId.Length < 1)
		{
		  throw new ActivitiIllegalArgumentException("The process instance id is mandatory, but '" + processInstanceId + "' has been provided.");
		}
		if (processDefinitionVersion == null)
		{
		  throw new ActivitiIllegalArgumentException("The process definition version is mandatory, but 'null' has been provided.");
		}
		if (processDefinitionVersion < 1)
		{
		  throw new ActivitiIllegalArgumentException("The process definition version must be positive, but '" + processDefinitionVersion + "' has been provided.");
		}
		this.processInstanceId = processInstanceId;
		this.processDefinitionVersion = processDefinitionVersion;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		// check that the new process definition is just another version of the same
		// process definition that the process instance is using
		ExecutionEntityManager executionManager = commandContext.ExecutionEntityManager;
		ExecutionEntity processInstance = executionManager.findExecutionById(processInstanceId);
		if (processInstance == null)
		{
		  throw new ActivitiObjectNotFoundException("No process instance found for id = '" + processInstanceId + "'.", typeof(ProcessInstance));
		}
		else if (!processInstance.ProcessInstanceType)
		{
		  throw new ActivitiIllegalArgumentException("A process instance id is required, but the provided id " + "'" + processInstanceId + "' " + "points to a child execution of process instance " + "'" + processInstance.ProcessInstanceId + "'. " + "Please invoke the " + this.GetType().Name + " with a root execution id.");
		}
		ProcessDefinitionImpl currentProcessDefinitionImpl = processInstance.ProcessDefinition;

		DeploymentManager deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentManager;
		ProcessDefinitionEntity currentProcessDefinition;
		if (currentProcessDefinitionImpl is ProcessDefinitionEntity)
		{
		  currentProcessDefinition = (ProcessDefinitionEntity) currentProcessDefinitionImpl;
		}
		else
		{
		  currentProcessDefinition = deploymentCache.findDeployedProcessDefinitionById(currentProcessDefinitionImpl.Id);
		}

		ProcessDefinitionEntity newProcessDefinition = deploymentCache.findDeployedProcessDefinitionByKeyAndVersion(currentProcessDefinition.Key, processDefinitionVersion);

		validateAndSwitchVersionOfExecution(commandContext, processInstance, newProcessDefinition);

		// switch the historic process instance to the new process definition version
		commandContext.HistoryManager.recordProcessDefinitionChange(processInstanceId, newProcessDefinition.Id);

		// switch all sub-executions of the process instance to the new process definition version
		IList<ExecutionEntity> childExecutions = executionManager.findChildExecutionsByProcessInstanceId(processInstanceId);
		foreach (ExecutionEntity executionEntity in childExecutions)
		{
		  validateAndSwitchVersionOfExecution(commandContext, executionEntity, newProcessDefinition);
		}

		return null;
	  }

	  protected internal virtual void validateAndSwitchVersionOfExecution(CommandContext commandContext, ExecutionEntity execution, ProcessDefinitionEntity newProcessDefinition)
	  {
		// check that the new process definition version contains the current activity
		if (execution.Activity != null && !newProcessDefinition.contains(execution.Activity))
		{
		  throw new ActivitiException("The new process definition " + "(key = '" + newProcessDefinition.Key + "') " + "does not contain the current activity " + "(id = '" + execution.Activity.Id + "') " + "of the process instance " + "(id = '" + processInstanceId + "').");
		}

		// switch the process instance to the new process definition version
		execution.ProcessDefinition = newProcessDefinition;

		// and change possible existing tasks (as the process definition id is stored there too)
		IList<TaskEntity> tasks = commandContext.TaskEntityManager.findTasksByExecutionId(execution.Id);
		foreach (TaskEntity taskEntity in tasks)
		{
		  taskEntity.ProcessDefinitionId = newProcessDefinition.Id;
		  commandContext.HistoryManager.recordTaskProcessDefinitionChange(taskEntity.Id, newProcessDefinition.Id);
		}
	  }

	}

}