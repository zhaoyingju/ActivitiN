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
	using ResourceEntity = org.activiti.engine.impl.persistence.entity.ResourceEntity;
	using Deployment = org.activiti.engine.repository.Deployment;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class GetDeploymentResourceCmd : Command<InputStream>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string deploymentId;
	  protected internal string resourceName;

	  public GetDeploymentResourceCmd(string deploymentId, string resourceName)
	  {
		this.deploymentId = deploymentId;
		this.resourceName = resourceName;
	  }

	  public virtual InputStream execute(CommandContext commandContext)
	  {
		if (deploymentId == null)
		{
		  throw new ActivitiIllegalArgumentException("deploymentId is null");
		}
		if (resourceName == null)
		{
		  throw new ActivitiIllegalArgumentException("resourceName is null");
		}

		ResourceEntity resource = commandContext.ResourceEntityManager.findResourceByDeploymentIdAndResourceName(deploymentId, resourceName);
		if (resource == null)
		{
		  if (commandContext.DeploymentEntityManager.findDeploymentById(deploymentId) == null)
		  {
			throw new ActivitiObjectNotFoundException("deployment does not exist: " + deploymentId, typeof(Deployment));
		  }
		  else
		  {
			throw new ActivitiObjectNotFoundException("no resource found with name '" + resourceName + "' in deployment '" + deploymentId + "'", typeof(InputStream));
		  }
		}
		return new ByteArrayInputStream(resource.Bytes);
	  }

	}

}