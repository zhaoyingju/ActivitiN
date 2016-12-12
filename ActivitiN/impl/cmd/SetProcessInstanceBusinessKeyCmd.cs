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

	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionEntityManager = org.activiti.engine.impl.persistence.entity.ExecutionEntityManager;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;


	/// <summary>
	/// <seealso cref="Command"/> that changes the business key of an existing
	/// process instance.
	/// 
	/// @author Tijs Rademakers
	/// </summary>
	[Serializable]
	public class SetProcessInstanceBusinessKeyCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  private readonly string processInstanceId;
	  private readonly string businessKey;

	  public SetProcessInstanceBusinessKeyCmd(string processInstanceId, string businessKey)
	  {
		if (processInstanceId == null || processInstanceId.Length < 1)
		{
		  throw new ActivitiIllegalArgumentException("The process instance id is mandatory, but '" + processInstanceId + "' has been provided.");
		}
		if (businessKey == null)
		{
		  throw new ActivitiIllegalArgumentException("The business key is mandatory, but 'null' has been provided.");
		}

		this.processInstanceId = processInstanceId;
		this.businessKey = businessKey;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
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

		processInstance.updateProcessBusinessKey(businessKey);

		return null;
	  }
	}

}