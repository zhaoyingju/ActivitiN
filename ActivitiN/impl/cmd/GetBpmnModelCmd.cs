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

	using BpmnModel = org.activiti.bpmn.model.BpmnModel;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class GetBpmnModelCmd : Command<BpmnModel>
	{

	  private const long serialVersionUID = 8167762371289445046L;

	  protected internal string processDefinitionId;

	  public GetBpmnModelCmd(string processDefinitionId)
	  {
		this.processDefinitionId = processDefinitionId;
	  }

	  public virtual BpmnModel execute(CommandContext commandContext)
	  {
		if (processDefinitionId == null)
		{
		  throw new ActivitiIllegalArgumentException("processDefinitionId is null");
		}

		return commandContext.ProcessEngineConfiguration.DeploymentManager.getBpmnModelById(processDefinitionId);
	  }
	}
}