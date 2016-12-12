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
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using SuspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState;
	using SuspensionState_SuspensionStateUtil = org.activiti.engine.impl.persistence.entity.SuspensionState_SuspensionStateUtil;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;
	using Execution = org.activiti.engine.runtime.Execution;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public abstract class AbstractSetProcessInstanceStateCmd : Command<Void>
	{

	  protected internal readonly string executionId;


	  public AbstractSetProcessInstanceStateCmd(string executionId)
	  {
		this.executionId = executionId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {

		if (executionId == null)
		{
		  throw new ActivitiIllegalArgumentException("ProcessInstanceId cannot be null.");
		}

		ExecutionEntity executionEntity = commandContext.ExecutionEntityManager.findExecutionById(executionId);

		if (executionEntity == null)
		{
		  throw new ActivitiObjectNotFoundException("Cannot find processInstance for id '" + executionId + "'.", typeof(Execution));
		}
		if (!executionEntity.ProcessInstanceType)
		{
		  throw new ActivitiException("Cannot set suspension state for execution '" + executionId + "': not a process instance.");
		}

		SuspensionState_SuspensionStateUtil.setSuspensionState(executionEntity, NewState);

		// All child executions are suspended
		IList<ExecutionEntity> childExecutions = commandContext.ExecutionEntityManager.findChildExecutionsByProcessInstanceId(executionId);
		foreach (ExecutionEntity childExecution in childExecutions)
		{
		  if (!childExecution.Id.Equals(executionId))
		  {
			SuspensionState_SuspensionStateUtil.setSuspensionState(childExecution, NewState);
		  }
		}

		// All tasks are suspended
		IList<TaskEntity> tasks = commandContext.TaskEntityManager.findTasksByProcessInstanceId(executionId);
		foreach (TaskEntity taskEntity in tasks)
		{
		  SuspensionState_SuspensionStateUtil.setSuspensionState(taskEntity, NewState);
		}

		return null;
	  }

	  protected internal abstract SuspensionState NewState {get;}

	}

}