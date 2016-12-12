using System;
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


	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using DeploymentEntity = org.activiti.engine.impl.persistence.entity.DeploymentEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ResourceEntity = org.activiti.engine.impl.persistence.entity.ResourceEntity;
	using DeploymentBuilderImpl = org.activiti.engine.impl.repository.DeploymentBuilderImpl;
	using Deployment = org.activiti.engine.repository.Deployment;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class DeployCmd<T> : Command<Deployment>
	{

	  private const long serialVersionUID = 1L;
	  protected internal DeploymentBuilderImpl deploymentBuilder;

	  public DeployCmd(DeploymentBuilderImpl deploymentBuilder)
	  {
		this.deploymentBuilder = deploymentBuilder;
	  }

	  public virtual Deployment execute(CommandContext commandContext)
	  {
		DeploymentEntity deployment = deploymentBuilder.Deployment;

		deployment.DeploymentTime = commandContext.ProcessEngineConfiguration.Clock.CurrentTime;

		if (deploymentBuilder.DuplicateFilterEnabled)
		{

			IList<Deployment> existingDeployments = new List<Deployment>();
		  if (deployment.TenantId == null || ProcessEngineConfiguration.NO_TENANT_ID.Equals(deployment.TenantId))
		  {
			  DeploymentEntity existingDeployment = commandContext.DeploymentEntityManager.findLatestDeploymentByName(deployment.Name);
			  if (existingDeployment != null)
			  {
				  existingDeployments.Add(existingDeployment);
			  }
		  }
		  else
		  {
			   IList<Deployment> deploymentList = commandContext.ProcessEngineConfiguration.RepositoryService.createDeploymentQuery().deploymentName(deployment.Name).deploymentTenantId(deployment.TenantId).orderByDeploymentId().desc().list();

			   if (deploymentList.Count > 0)
			   {
				   existingDeployments.AddRange(deploymentList);
			   }
		  }

		  DeploymentEntity existingDeployment = null;
		  if (existingDeployments.Count > 0)
		  {
			existingDeployment = (DeploymentEntity) existingDeployments[0];
		  }

		  if ((existingDeployment != null) && !deploymentsDiffer(deployment, existingDeployment))
		  {
			return existingDeployment;
		  }
		}

		deployment.New = true;

		// Save the data
		commandContext.DeploymentEntityManager.insertDeployment(deployment);

		if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, deployment));
		}

		// Deployment settings
		IDictionary<string, object> deploymentSettings = new Dictionary<string, object>();
		deploymentSettings[DeploymentSettings_Fields.IS_BPMN20_XSD_VALIDATION_ENABLED] = deploymentBuilder.Bpmn20XsdValidationEnabled;
		deploymentSettings[DeploymentSettings_Fields.IS_PROCESS_VALIDATION_ENABLED] = deploymentBuilder.ProcessValidationEnabled;

		// Actually deploy
		commandContext.ProcessEngineConfiguration.DeploymentManager.deploy(deployment, deploymentSettings);

		if (deploymentBuilder.ProcessDefinitionsActivationDate != null)
		{
		  scheduleProcessDefinitionActivation(commandContext, deployment);
		}

		if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_INITIALIZED, deployment));
		}

		return deployment;
	  }

	  protected internal virtual bool deploymentsDiffer(DeploymentEntity deployment, DeploymentEntity saved)
	  {

		if (deployment.Resources == null || saved.Resources == null)
		{
		  return true;
		}

		IDictionary<string, ResourceEntity> resources = deployment.Resources;
		IDictionary<string, ResourceEntity> savedResources = saved.Resources;

		foreach (string resourceName in resources.Keys)
		{
		  ResourceEntity savedResource = savedResources[resourceName];

		  if (savedResource == null)
		  {
			  return true;
		  }

		  if (!savedResource.Generated)
		  {
			ResourceEntity resource = resources[resourceName];

			sbyte[] bytes = resource.Bytes;
			sbyte[] savedBytes = savedResource.Bytes;
			if (!Arrays.Equals(bytes, savedBytes))
			{
			  return true;
			}
		  }
		}
		return false;
	  }

	  protected internal virtual void scheduleProcessDefinitionActivation(CommandContext commandContext, DeploymentEntity deployment)
	  {
		foreach (ProcessDefinitionEntity processDefinitionEntity in deployment.getDeployedArtifacts(typeof(ProcessDefinitionEntity)))
		{

		  // If activation date is set, we first suspend all the process definition
		  SuspendProcessDefinitionCmd suspendProcessDefinitionCmd = new SuspendProcessDefinitionCmd(processDefinitionEntity, false, null, deployment.TenantId);
		  suspendProcessDefinitionCmd.execute(commandContext);

		  // And we schedule an activation at the provided date
		  ActivateProcessDefinitionCmd activateProcessDefinitionCmd = new ActivateProcessDefinitionCmd(processDefinitionEntity, false, deploymentBuilder.ProcessDefinitionsActivationDate, deployment.TenantId);
		  activateProcessDefinitionCmd.execute(commandContext);
		}
	  }

	//  private boolean resourcesDiffer(ByteArrayEntity value, ByteArrayEntity other) {
	//    if (value == null && other == null) {
	//      return false;
	//    }
	//    String bytes = createKey(value.getBytes());
	//    String savedBytes = other == null ? null : createKey(other.getBytes());
	//    return !bytes.equals(savedBytes);
	//  }
	//
	//  private String createKey(byte[] bytes) {
	//    if (bytes == null) {
	//      return "";
	//    }
	//    MessageDigest digest;
	//    try {
	//      digest = MessageDigest.getInstance("MD5");
	//    } catch (NoSuchAlgorithmException e) {
	//      throw new IllegalStateException("MD5 algorithm not available.  Fatal (should be in the JDK).");
	//    }
	//    bytes = digest.digest(bytes);
	//    return String.format("%032x", new BigInteger(1, bytes));
	//  }
	}

}