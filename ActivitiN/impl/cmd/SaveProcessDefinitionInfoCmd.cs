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
	using ProcessDefinitionInfoEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionInfoEntity;
	using ProcessDefinitionInfoEntityManager = org.activiti.engine.impl.persistence.entity.ProcessDefinitionInfoEntityManager;

	using ObjectWriter = com.fasterxml.jackson.databind.ObjectWriter;
	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;


	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	[Serializable]
	public class SaveProcessDefinitionInfoCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string processDefinitionId;
	  protected internal ObjectNode infoNode;

	  public SaveProcessDefinitionInfoCmd(string processDefinitionId, ObjectNode infoNode)
	  {
		this.processDefinitionId = processDefinitionId;
		this.infoNode = infoNode;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		if (processDefinitionId == null)
		{
		  throw new ActivitiIllegalArgumentException("process definition id is null");
		}

		if (infoNode == null)
		{
		  throw new ActivitiIllegalArgumentException("process definition info node is null");
		}

		ProcessDefinitionInfoEntityManager definitionInfoEntityManager = commandContext.ProcessDefinitionInfoEntityManager;
		ProcessDefinitionInfoEntity definitionInfoEntity = definitionInfoEntityManager.findProcessDefinitionInfoByProcessDefinitionId(processDefinitionId);
		if (definitionInfoEntity == null)
		{
		  definitionInfoEntity = new ProcessDefinitionInfoEntity();
		  definitionInfoEntity.ProcessDefinitionId = processDefinitionId;
		  commandContext.ProcessDefinitionInfoEntityManager.insertProcessDefinitionInfo(definitionInfoEntity);
		}
		else
		{
		  commandContext.ProcessDefinitionInfoEntityManager.updateProcessDefinitionInfo(definitionInfoEntity);
		}

		if (infoNode != null)
		{
		  try
		  {
			ObjectWriter writer = commandContext.ProcessEngineConfiguration.ObjectMapper.writer();
			commandContext.ProcessDefinitionInfoEntityManager.updateInfoJson(definitionInfoEntity.Id, writer.writeValueAsBytes(infoNode));
		  }
		  catch (Exception)
		  {
			throw new ActivitiException("Unable to serialize info node " + infoNode);
		  }
		}

		return null;
	  }

	}

}