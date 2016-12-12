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

namespace org.activiti.engine.impl.persistence.entity
{


	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using EventSubscriptionDeclaration = org.activiti.engine.impl.bpmn.parser.EventSubscriptionDeclaration;
	using Context = org.activiti.engine.impl.context.Context;
	using TimerDeclarationImpl = org.activiti.engine.impl.jobexecutor.TimerDeclarationImpl;
	using TimerStartEventJobHandler = org.activiti.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using Deployment = org.activiti.engine.repository.Deployment;
	using Model = org.activiti.engine.repository.Model;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using Job = org.activiti.engine.runtime.Job;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class DeploymentEntityManager : AbstractManager
	{

	  public virtual void insertDeployment(DeploymentEntity deployment)
	  {
		DbSqlSession.insert(deployment);

		foreach (ResourceEntity resource in deployment.Resources.Values)
		{
		  resource.DeploymentId = deployment.Id;
		  ResourceManager.insertResource(resource);
		}
	  }

	  public virtual void deleteDeployment(string deploymentId, bool cascade)
	  {
		IList<ProcessDefinition> processDefinitions = DbSqlSession.createProcessDefinitionQuery().deploymentId(deploymentId).list();

		// Remove the deployment link from any model. 
		// The model will still exists, as a model is a source for a deployment model and has a different lifecycle
		IList<Model> models = DbSqlSession.createModelQueryImpl().deploymentId(deploymentId).list();

		foreach (Model model in models)
		{
		  ModelEntity modelEntity = (ModelEntity) model;
		  modelEntity.DeploymentId = null;
		  ModelManager.updateModel(modelEntity);
		}

		if (cascade)
		{

		  // delete process instances
		  foreach (ProcessDefinition processDefinition in processDefinitions)
		  {
			string processDefinitionId = processDefinition.Id;

			ProcessInstanceManager.deleteProcessInstancesByProcessDefinition(processDefinitionId, "deleted deployment", cascade);
		  }
		}

		foreach (ProcessDefinition processDefinition in processDefinitions)
		{
		  string processDefinitionId = processDefinition.Id;
		  // remove related authorization parameters in IdentityLink table
		  IdentityLinkManager.deleteIdentityLinksByProcDef(processDefinitionId);

		  // event subscriptions
		  IList<EventSubscriptionEntity> eventSubscriptionEntities = EventSubscriptionManager.findEventSubscriptionsByTypeAndProcessDefinitionId(null, processDefinitionId, processDefinition.TenantId); // null type ==> all types
		  foreach (EventSubscriptionEntity eventSubscriptionEntity in eventSubscriptionEntities)
		  {
			eventSubscriptionEntity.delete();
		  }

		  ProcessDefinitionInfoManager.deleteProcessDefinitionInfo(processDefinitionId);

		}

		// delete process definitions from db
		ProcessDefinitionManager.deleteProcessDefinitionsByDeploymentId(deploymentId);

		foreach (ProcessDefinition processDefinition in processDefinitions)
		{

		  // remove timer start events for current process definition:

			IList<Job> timerStartJobs = Context.CommandContext.JobEntityManager.findJobsByTypeAndProcessDefinitionId(TimerStartEventJobHandler.TYPE, processDefinition.Id);
			if (timerStartJobs != null && timerStartJobs.Count > 0)
			{
				foreach (Job timerStartJob in timerStartJobs)
				{
						if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
						{
							Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.JOB_CANCELED, timerStartJob, null, null, processDefinition.Id));
						}

						((JobEntity) timerStartJob).delete();
				}
			}

			// If previous process definition version has a timer/message/signal start event, it must be added
			ProcessDefinitionEntity latestProcessDefinition = findLatestProcessDefinition(processDefinition);

		  // Only if the currently deleted process definition is the latest version, we fall back to the previous start event type
			if (processDefinition.Id.Equals(latestProcessDefinition.Id))
			{

				// Try to find a previous version (it could be some versions are missing due to deletions)
				ProcessDefinition previousProcessDefinition = findNewLatestProcessDefinitionAfterRemovalOf(processDefinition);
				if (previousProcessDefinition != null)
				{

					// Need to resolve process definition to make sure it's parsed
					ProcessDefinitionEntity resolvedProcessDefinition = Context.ProcessEngineConfiguration.DeploymentManager.resolveProcessDefinition((ProcessDefinitionEntity) previousProcessDefinition);

					// Timer start
					IList<TimerDeclarationImpl> timerDeclarations = (IList<TimerDeclarationImpl>) resolvedProcessDefinition.getProperty(BpmnParse.PROPERTYNAME_START_TIMER);
				if (timerDeclarations != null)
				{
				  foreach (TimerDeclarationImpl timerDeclaration in timerDeclarations)
				  {
					TimerEntity timer = timerDeclaration.prepareTimerEntity(null);
					timer.ProcessDefinitionId = previousProcessDefinition.Id;

					if (previousProcessDefinition.TenantId != null)
					{
						timer.TenantId = previousProcessDefinition.TenantId;
					}

					Context.CommandContext.JobEntityManager.schedule(timer);
				  }
				}

				// Signal / Message start
				IList<EventSubscriptionDeclaration> signalEventDefinitions = (IList<EventSubscriptionDeclaration>) resolvedProcessDefinition.getProperty(BpmnParse.PROPERTYNAME_EVENT_SUBSCRIPTION_DECLARATION);
				 if (signalEventDefinitions != null)
				 {
				   foreach (EventSubscriptionDeclaration eventDefinition in signalEventDefinitions)
				   {
					 if (eventDefinition.EventType.Equals("signal") && eventDefinition.StartEvent)
					 {

						 SignalEventSubscriptionEntity subscriptionEntity = new SignalEventSubscriptionEntity();
						 subscriptionEntity.EventName = eventDefinition.EventName;
						 subscriptionEntity.ActivityId = eventDefinition.ActivityId;
						 subscriptionEntity.ProcessDefinitionId = previousProcessDefinition.Id;
							subscriptionEntity.TenantId = previousProcessDefinition.TenantId;
						 subscriptionEntity.insert();

					 }
					 else if (eventDefinition.EventType.Equals("message") && eventDefinition.StartEvent)
					 {
						 MessageEventSubscriptionEntity newSubscription = new MessageEventSubscriptionEntity();
					 newSubscription.EventName = eventDefinition.EventName;
					 newSubscription.ActivityId = eventDefinition.ActivityId;
					 newSubscription.Configuration = previousProcessDefinition.Id;
					 newSubscription.ProcessDefinitionId = previousProcessDefinition.Id;
					 newSubscription.TenantId = previousProcessDefinition.TenantId;
					 newSubscription.insert();
					 }
				   }
				 }

				}

			}

		}

		ResourceManager.deleteResourcesByDeploymentId(deploymentId);

		DbSqlSession.delete("deleteDeployment", deploymentId);
	  }

	  protected internal virtual ProcessDefinitionEntity findLatestProcessDefinition(ProcessDefinition processDefinition)
	  {
		ProcessDefinitionEntity latestProcessDefinition = null;
		if (processDefinition.TenantId != null && !ProcessEngineConfiguration.NO_TENANT_ID.Equals(processDefinition.TenantId))
		{
			latestProcessDefinition = Context.CommandContext.ProcessDefinitionEntityManager.findLatestProcessDefinitionByKeyAndTenantId(processDefinition.Key, processDefinition.TenantId);
		}
		else
		{
			latestProcessDefinition = Context.CommandContext.ProcessDefinitionEntityManager.findLatestProcessDefinitionByKey(processDefinition.Key);
		}
		return latestProcessDefinition;
	  }

	  protected internal virtual ProcessDefinition findNewLatestProcessDefinitionAfterRemovalOf(ProcessDefinition processDefinitionToBeRemoved)
	  {

		// The latest process definition is not necessarily the one with 'version -1' (some versions could have been deleted)
		// Hence, the following logic

		ProcessDefinitionQueryImpl query = new ProcessDefinitionQueryImpl();
		query.processDefinitionKey(processDefinitionToBeRemoved.Key);

		if (processDefinitionToBeRemoved.TenantId != null && !ProcessEngineConfiguration.NO_TENANT_ID.Equals(processDefinitionToBeRemoved.TenantId))
		{
		  query.processDefinitionTenantId(processDefinitionToBeRemoved.TenantId);
		}
		else
		{
		  query.processDefinitionWithoutTenantId();
		}

		query.processDefinitionVersionLowerThan(processDefinitionToBeRemoved.Version);
		query.orderByProcessDefinitionVersion().desc();

		IList<ProcessDefinition> processDefinitions = ProcessDefinitionManager.findProcessDefinitionsByQueryCriteria(query, new Page(0, 1));
		if (processDefinitions != null && processDefinitions.Count > 0)
		{
		  return processDefinitions[0];
		}
		return null;
	  }


	  public virtual DeploymentEntity findLatestDeploymentByName(string deploymentName)
	  {
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<?> list = getDbSqlSession().selectList("selectDeploymentsByName", deploymentName, 0, 1);
		IList<?> list = DbSqlSession.selectList("selectDeploymentsByName", deploymentName, 0, 1);
		if (list != null && list.Count > 0)
		{
		  return (DeploymentEntity) list[0];
		}
		return null;
	  }

	  public virtual DeploymentEntity findDeploymentById(string deploymentId)
	  {
		return (DeploymentEntity) DbSqlSession.selectOne("selectDeploymentById", deploymentId);
	  }

	  public virtual long findDeploymentCountByQueryCriteria(DeploymentQueryImpl deploymentQuery)
	  {
		return (long?) DbSqlSession.selectOne("selectDeploymentCountByQueryCriteria", deploymentQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.repository.Deployment> findDeploymentsByQueryCriteria(org.activiti.engine.impl.DeploymentQueryImpl deploymentQuery, org.activiti.engine.impl.Page page)
	  public virtual IList<Deployment> findDeploymentsByQueryCriteria(DeploymentQueryImpl deploymentQuery, Page page)
	  {
		const string query = "selectDeploymentsByQueryCriteria";
		return DbSqlSession.selectList(query, deploymentQuery, page);
	  }

	  public virtual IList<string> getDeploymentResourceNames(string deploymentId)
	  {
		return DbSqlSession.SqlSession.selectList("selectResourceNamesByDeploymentId", deploymentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.repository.Deployment> findDeploymentsByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<Deployment> findDeploymentsByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbSqlSession.selectListWithRawParameter("selectDeploymentByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findDeploymentCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbSqlSession.selectOne("selectDeploymentCountByNativeQuery", parameterMap);
	  }

	  public override void close()
	  {
	  }

	  public override void flush()
	  {
	  }
	}

}