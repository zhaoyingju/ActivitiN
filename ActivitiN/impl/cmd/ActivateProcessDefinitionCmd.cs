using System;

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

	using TimerActivateProcessDefinitionHandler = org.activiti.engine.impl.jobexecutor.TimerActivateProcessDefinitionHandler;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using SuspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public class ActivateProcessDefinitionCmd : AbstractSetProcessDefinitionStateCmd
	{

	  public ActivateProcessDefinitionCmd(ProcessDefinitionEntity processDefinitionEntity, bool includeProcessInstances, DateTime executionDate, string tenantId) : base(processDefinitionEntity, includeProcessInstances, executionDate, tenantId)
	  {
	  }

	  public ActivateProcessDefinitionCmd(string processDefinitionId, string processDefinitionKey, bool includeProcessInstances, DateTime executionDate, string tenantId) : base(processDefinitionId, processDefinitionKey, includeProcessInstances, executionDate, tenantId)
	  {
	  }

	  protected internal override SuspensionState ProcessDefinitionSuspensionState
	  {
		  get
		  {
			return org.activiti.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		  }
	  }

	  protected internal override string DelayedExecutionJobHandlerType
	  {
		  get
		  {
			return TimerActivateProcessDefinitionHandler.TYPE;
		  }
	  }

	  protected internal override AbstractSetProcessInstanceStateCmd getProcessInstanceChangeStateCmd(ProcessInstance processInstance)
	  {
		return new ActivateProcessInstanceCmd(processInstance.Id);
	  }
	}

}