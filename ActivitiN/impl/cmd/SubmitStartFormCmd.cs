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

	using StartFormHandler = org.activiti.engine.impl.form.StartFormHandler;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class SubmitStartFormCmd : NeedsActiveProcessDefinitionCmd<ProcessInstance>
	{

	  private const long serialVersionUID = 1L;

	  protected internal readonly string businessKey;
	  protected internal IDictionary<string, string> properties;

	  public SubmitStartFormCmd(string processDefinitionId, string businessKey, IDictionary<string, string> properties) : base(processDefinitionId)
	  {
		this.businessKey = businessKey;
		this.properties = properties;
	  }

	  protected internal virtual ProcessInstance execute(CommandContext commandContext, ProcessDefinitionEntity processDefinition)
	  {
		ExecutionEntity processInstance = null;
		if (businessKey != null)
		{
		  processInstance = processDefinition.createProcessInstance(businessKey);
		}
		else
		{
		  processInstance = processDefinition.createProcessInstance();
		}

		commandContext.HistoryManager.reportFormPropertiesSubmitted(processInstance, properties, null);

		StartFormHandler startFormHandler = processDefinition.StartFormHandler;
		startFormHandler.submitFormProperties(properties, processInstance);

		processInstance.start();

		return processInstance;
	  }
	}

}