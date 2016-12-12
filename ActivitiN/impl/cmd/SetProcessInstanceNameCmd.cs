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
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;

	[Serializable]
	public class SetProcessInstanceNameCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string processInstanceId;
	  protected internal string name;

	  public SetProcessInstanceNameCmd(string processInstanceId, string name)
	  {
		this.processInstanceId = processInstanceId;
		this.name = name;
	  }

	  public override Void execute(CommandContext commandContext)
	  {
		if (processInstanceId == null)
		{
		  throw new ActivitiIllegalArgumentException("processInstanceId is null");
		}

		ExecutionEntity execution = commandContext.ExecutionEntityManager.findExecutionById(processInstanceId);

		if (execution == null)
		{
		  throw new ActivitiObjectNotFoundException("process instance " + processInstanceId + " doesn't exist", typeof(ProcessInstance));
		}

		if (!execution.ProcessInstanceType)
		{
		  throw new ActivitiObjectNotFoundException("process instance " + processInstanceId + " doesn't exist, the given ID references an execution, though", typeof(ProcessInstance));
		}

		if (execution.Suspended)
		{
		  throw new ActivitiException("process instance " + processInstanceId + " is suspended, cannot set name");
		}

		// Actually set the name
		execution.Name = name;

		// Record the change in history
		commandContext.HistoryManager.recordProcessInstanceNameChange(processInstanceId, name);

		return null;
	  }

	}

}