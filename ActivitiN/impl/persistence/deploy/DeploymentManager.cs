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

namespace org.activiti.engine.impl.persistence.deploy
{


	using BpmnXMLConverter = org.activiti.bpmn.converter.BpmnXMLConverter;
	using BpmnModel = org.activiti.bpmn.model.BpmnModel;
	using ActivitiEventDispatcher = org.activiti.engine.@delegate.@event.ActivitiEventDispatcher;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using Context = org.activiti.engine.impl.context.Context;
	using DeploymentEntity = org.activiti.engine.impl.persistence.entity.DeploymentEntity;
	using DeploymentEntityManager = org.activiti.engine.impl.persistence.entity.DeploymentEntityManager;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ResourceEntity = org.activiti.engine.impl.persistence.entity.ResourceEntity;
	using BytesStreamSource = org.activiti.engine.impl.util.io.BytesStreamSource;
	using Deployment = org.activiti.engine.repository.Deployment;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// @author Joram Barrez
	/// </summary>
	public class DeploymentManager
	{

	  protected internal DeploymentCache<ProcessDefinitionEntity> processDefinitionCache;
	  protected internal DeploymentCache<BpmnModel> bpmnModelCache;
	  protected internal ProcessDefinitionInfoCache processDefinitionInfoCache;
	  protected internal DeploymentCache<object> knowledgeBaseCache; // Needs to be object to avoid an import to Drools in this core class
	  protected internal IList<Deployer> deployers;

	  public virtual void deploy(DeploymentEntity deployment)
	  {
		deploy(deployment, null);
	  }

	  public virtual void deploy(DeploymentEntity deployment, IDictionary<string, object> deploymentSettings)
	  {
		foreach (Deployer deployer in deployers)
		{
		  deployer.deploy(deployment, deploymentSettings);
		}
	  }

	  public virtual ProcessDefinitionEntity findDeployedProcessDefinitionById(string processDefinitionId)
	  {
		if (processDefinitionId == null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid process definition id : null");
		}

		// first try the cache
		ProcessDefinitionEntity processDefinition = processDefinitionCache.get(processDefinitionId);

		if (processDefinition == null)
		{
		  processDefinition = Context.CommandContext.ProcessDefinitionEntityManager.findProcessDefinitionById(processDefinitionId);
		  if (processDefinition == null)
		  {
			throw new ActivitiObjectNotFoundException("no deployed process definition found with id '" + processDefinitionId + "'", typeof(ProcessDefinition));
		  }
		  processDefinition = resolveProcessDefinition(processDefinition);
		}
		return processDefinition;
	  }

	  public virtual ProcessDefinitionEntity findProcessDefinitionByIdFromDatabase(string processDefinitionId)
	  {
		if (processDefinitionId == null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid process definition id : null");
		}

		ProcessDefinitionEntity processDefinition = Context.CommandContext.ProcessDefinitionEntityManager.findProcessDefinitionById(processDefinitionId);

		if (processDefinition == null)
		{
		  throw new ActivitiObjectNotFoundException("no deployed process definition found with id '" + processDefinitionId + "'", typeof(ProcessDefinition));
		}

		return processDefinition;
	  }

	  public virtual bool isProcessDefinitionSuspended(string processDefinitionId)
	  {
		return findProcessDefinitionByIdFromDatabase(processDefinitionId).Suspended;
	  }

	  public virtual BpmnModel getBpmnModelById(string processDefinitionId)
	  {
		if (processDefinitionId == null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid process definition id : null");
		}

		// first try the cache
		BpmnModel bpmnModel = bpmnModelCache.get(processDefinitionId);

		if (bpmnModel == null)
		{
		  ProcessDefinitionEntity processDefinition = findDeployedProcessDefinitionById(processDefinitionId);
		  if (processDefinition == null)
		  {
			throw new ActivitiObjectNotFoundException("no deployed process definition found with id '" + processDefinitionId + "'", typeof(ProcessDefinition));
		  }

		  // Fetch the resource
		  string resourceName = processDefinition.ResourceName;
		  ResourceEntity resource = Context.CommandContext.ResourceEntityManager.findResourceByDeploymentIdAndResourceName(processDefinition.DeploymentId, resourceName);
		  if (resource == null)
		  {
			if (Context.CommandContext.DeploymentEntityManager.findDeploymentById(processDefinition.DeploymentId) == null)
			{
			  throw new ActivitiObjectNotFoundException("deployment for process definition does not exist: " + processDefinition.DeploymentId, typeof(Deployment));
			}
			else
			{
			  throw new ActivitiObjectNotFoundException("no resource found with name '" + resourceName + "' in deployment '" + processDefinition.DeploymentId + "'", typeof(InputStream));
			}
		  }

		  // Convert the bpmn 2.0 xml to a bpmn model
		  BpmnXMLConverter bpmnXMLConverter = new BpmnXMLConverter();
		  bpmnModel = bpmnXMLConverter.convertToBpmnModel(new BytesStreamSource(resource.Bytes), false, false);
		  bpmnModelCache.add(processDefinition.Id, bpmnModel);
		}
		return bpmnModel;
	  }

	  public virtual ProcessDefinitionEntity findDeployedLatestProcessDefinitionByKey(string processDefinitionKey)
	  {
		ProcessDefinitionEntity processDefinition = Context.CommandContext.ProcessDefinitionEntityManager.findLatestProcessDefinitionByKey(processDefinitionKey);

		if (processDefinition == null)
		{
		  throw new ActivitiObjectNotFoundException("no processes deployed with key '" + processDefinitionKey + "'", typeof(ProcessDefinition));
		}
		processDefinition = resolveProcessDefinition(processDefinition);
		return processDefinition;
	  }

	  public virtual ProcessDefinitionEntity findDeployedLatestProcessDefinitionByKeyAndTenantId(string processDefinitionKey, string tenantId)
	  {
		ProcessDefinitionEntity processDefinition = Context.CommandContext.ProcessDefinitionEntityManager.findLatestProcessDefinitionByKeyAndTenantId(processDefinitionKey, tenantId);
		if (processDefinition == null)
		{
		  throw new ActivitiObjectNotFoundException("no processes deployed with key '" + processDefinitionKey + "' for tenant identifier '" + tenantId + "'", typeof(ProcessDefinition));
		}
		processDefinition = resolveProcessDefinition(processDefinition);
		return processDefinition;
	  }

	  public virtual ProcessDefinitionEntity findDeployedProcessDefinitionByKeyAndVersion(string processDefinitionKey, int? processDefinitionVersion)
	  {
		ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) Context.CommandContext.ProcessDefinitionEntityManager.findProcessDefinitionByKeyAndVersion(processDefinitionKey, processDefinitionVersion);
		if (processDefinition == null)
		{
		  throw new ActivitiObjectNotFoundException("no processes deployed with key = '" + processDefinitionKey + "' and version = '" + processDefinitionVersion + "'", typeof(ProcessDefinition));
		}
		processDefinition = resolveProcessDefinition(processDefinition);
		return processDefinition;
	  }

	  public virtual ProcessDefinitionEntity resolveProcessDefinition(ProcessDefinitionEntity processDefinition)
	  {
		string processDefinitionId = processDefinition.Id;
		string deploymentId = processDefinition.DeploymentId;
		processDefinition = processDefinitionCache.get(processDefinitionId);
		if (processDefinition == null)
		{
		  DeploymentEntity deployment = Context.CommandContext.DeploymentEntityManager.findDeploymentById(deploymentId);
		  deployment.New = false;
		  deploy(deployment, null);
		  processDefinition = processDefinitionCache.get(processDefinitionId);

		  if (processDefinition == null)
		  {
			throw new ActivitiException("deployment '" + deploymentId + "' didn't put process definition '" + processDefinitionId + "' in the cache");
		  }
		}
		return processDefinition;
	  }

	  public virtual void removeDeployment(string deploymentId, bool cascade)
	  {
		  DeploymentEntityManager deploymentEntityManager = Context.CommandContext.DeploymentEntityManager;

		  DeploymentEntity deployment = deploymentEntityManager.findDeploymentById(deploymentId);
		  if (deployment == null)
		  {
			  throw new ActivitiObjectNotFoundException("Could not find a deployment with id '" + deploymentId + "'.", typeof(DeploymentEntity));
		  }

		// Remove any process definition from the cache
		IList<ProcessDefinition> processDefinitions = (new ProcessDefinitionQueryImpl(Context.CommandContext)).deploymentId(deploymentId).list();
		ActivitiEventDispatcher eventDispatcher = Context.ProcessEngineConfiguration.EventDispatcher;

		foreach (ProcessDefinition processDefinition in processDefinitions)
		{

		  // Since all process definitions are deleted by a single query, we should dispatch the events in this loop
		  if (eventDispatcher.Enabled)
		  {
			  eventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_DELETED, processDefinition));
		  }
		}

		// Delete data
		deploymentEntityManager.deleteDeployment(deploymentId, cascade);

		// Since we use a delete by query, delete-events are not automatically dispatched
		if (eventDispatcher.Enabled)
		{
			eventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_DELETED, deployment));
		}

		foreach (ProcessDefinition processDefinition in processDefinitions)
		{
		  processDefinitionCache.remove(processDefinition.Id);
		}
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual IList<Deployer> Deployers
	  {
		  get
		  {
			return deployers;
		  }
		  set
		  {
			this.deployers = value;
		  }
	  }


	  public virtual DeploymentCache<ProcessDefinitionEntity> getProcessDefinitionCache()
	  {
		return processDefinitionCache;
	  }

	  public virtual void setProcessDefinitionCache(DeploymentCache<ProcessDefinitionEntity> processDefinitionCache)
	  {
		this.processDefinitionCache = processDefinitionCache;
	  }

	  public virtual DeploymentCache<BpmnModel> getBpmnModelCache()
	  {
		return bpmnModelCache;
	  }

	  public virtual void setBpmnModelCache(DeploymentCache<BpmnModel> bpmnModelCache)
	  {
		this.bpmnModelCache = bpmnModelCache;
	  }

	  public virtual ProcessDefinitionInfoCache getProcessDefinitionInfoCache()
	  {
		return processDefinitionInfoCache;
	  }

	  public virtual void setProcessDefinitionInfoCache(ProcessDefinitionInfoCache processDefinitionInfoCache)
	  {
		this.processDefinitionInfoCache = processDefinitionInfoCache;
	  }

	  public virtual DeploymentCache<object> getKnowledgeBaseCache()
	  {
		return knowledgeBaseCache;
	  }

	  public virtual void setKnowledgeBaseCache(DeploymentCache<object> knowledgeBaseCache)
	  {
		this.knowledgeBaseCache = knowledgeBaseCache;
	  }

	}

}