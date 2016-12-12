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
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// Gives access to a deployed process diagram, e.g., a PNG image, through a
	/// stream of bytes.
	/// 
	/// @author Falko Menge
	/// </summary>
	[Serializable]
	public class GetDeploymentProcessDiagramCmd : Command<InputStream>
	{

	  private const long serialVersionUID = 1L;
	  private static Logger log = LoggerFactory.getLogger(typeof(GetDeploymentProcessDiagramCmd));

	  protected internal string processDefinitionId;

	  public GetDeploymentProcessDiagramCmd(string processDefinitionId)
	  {
		if (processDefinitionId == null || processDefinitionId.Length < 1)
		{
		  throw new ActivitiIllegalArgumentException("The process definition id is mandatory, but '" + processDefinitionId + "' has been provided.");
		}
		this.processDefinitionId = processDefinitionId;
	  }

	  public virtual InputStream execute(CommandContext commandContext)
	  {
		ProcessDefinitionEntity processDefinition = commandContext.ProcessEngineConfiguration.DeploymentManager.findDeployedProcessDefinitionById(processDefinitionId);
		string deploymentId = processDefinition.DeploymentId;
		string resourceName = processDefinition.DiagramResourceName;
		if (resourceName == null)
		{
		  log.info("Resource name is null! No process diagram stream exists.");
		  return null;
		}
		else
		{
		  InputStream processDiagramStream = (new GetDeploymentResourceCmd(deploymentId, resourceName)).execute(commandContext);
		  return processDiagramStream;
		}
	  }

	}

}