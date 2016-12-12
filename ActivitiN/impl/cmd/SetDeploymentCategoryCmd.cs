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

	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using DeploymentEntity = org.activiti.engine.impl.persistence.entity.DeploymentEntity;
	using Deployment = org.activiti.engine.repository.Deployment;

	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	public class SetDeploymentCategoryCmd : Command<Void>
	{

	  protected internal string deploymentId;
	  protected internal string category;

	  public SetDeploymentCategoryCmd(string deploymentId, string category)
	  {
		this.deploymentId = deploymentId;
		this.category = category;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {

		if (deploymentId == null)
		{
		  throw new ActivitiIllegalArgumentException("Deployment id is null");
		}

		DeploymentEntity deployment = commandContext.DeploymentEntityManager.findDeploymentById(deploymentId);

		if (deployment == null)
		{
		  throw new ActivitiObjectNotFoundException("No deployment found for id = '" + deploymentId + "'", typeof(Deployment));
		}

		// Update category
		deployment.Category = category;

		if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
		  commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, deployment));
		}

		return null;
	  }

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
		  }
	  }


	  public virtual string Category
	  {
		  get
		  {
			return category;
		  }
		  set
		  {
			this.category = value;
		  }
	  }


	}

}