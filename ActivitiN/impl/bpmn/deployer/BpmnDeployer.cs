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
namespace org.activiti.engine.impl.bpmn.deployer
{


	using BpmnXMLConstants = org.activiti.bpmn.constants.BpmnXMLConstants;
	using BpmnModel = org.activiti.bpmn.model.BpmnModel;
	using ExtensionElement = org.activiti.bpmn.model.ExtensionElement;
	using FlowElement = org.activiti.bpmn.model.FlowElement;
	using Process = org.activiti.bpmn.model.Process;
	using SubProcess = org.activiti.bpmn.model.SubProcess;
	using UserTask = org.activiti.bpmn.model.UserTask;
	using ValuedDataObject = org.activiti.bpmn.model.ValuedDataObject;
	using Expression = org.activiti.engine.@delegate.Expression;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using BpmnParser = org.activiti.engine.impl.bpmn.parser.BpmnParser;
	using EventSubscriptionDeclaration = org.activiti.engine.impl.bpmn.parser.EventSubscriptionDeclaration;
	using IdGenerator = org.activiti.engine.impl.cfg.IdGenerator;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CancelJobsCmd = org.activiti.engine.impl.cmd.CancelJobsCmd;
	using DeploymentSettings = org.activiti.engine.impl.cmd.DeploymentSettings;
	using Context = org.activiti.engine.impl.context.Context;
	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using ExpressionManager = org.activiti.engine.impl.el.ExpressionManager;
	using MessageEventHandler = org.activiti.engine.impl.@event.MessageEventHandler;
	using SignalEventHandler = org.activiti.engine.impl.@event.SignalEventHandler;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using TimerDeclarationImpl = org.activiti.engine.impl.jobexecutor.TimerDeclarationImpl;
	using TimerStartEventJobHandler = org.activiti.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using Deployer = org.activiti.engine.impl.persistence.deploy.Deployer;
	using DeploymentManager = org.activiti.engine.impl.persistence.deploy.DeploymentManager;
	using ProcessDefinitionInfoCacheObject = org.activiti.engine.impl.persistence.deploy.ProcessDefinitionInfoCacheObject;
	using DeploymentEntity = org.activiti.engine.impl.persistence.entity.DeploymentEntity;
	using EventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.EventSubscriptionEntity;
	using IdentityLinkEntity = org.activiti.engine.impl.persistence.entity.IdentityLinkEntity;
	using MessageEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.MessageEventSubscriptionEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinitionEntityManager = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntityManager;
	using ProcessDefinitionInfoEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionInfoEntity;
	using ProcessDefinitionInfoEntityManager = org.activiti.engine.impl.persistence.entity.ProcessDefinitionInfoEntityManager;
	using ResourceEntity = org.activiti.engine.impl.persistence.entity.ResourceEntity;
	using SignalEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.SignalEventSubscriptionEntity;
	using TimerEntity = org.activiti.engine.impl.persistence.entity.TimerEntity;
	using IoUtil = org.activiti.engine.impl.util.IoUtil;
	using Job = org.activiti.engine.runtime.Job;
	using IdentityLinkType = org.activiti.engine.task.IdentityLinkType;
	using StringUtils = org.apache.commons.lang3.StringUtils;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class BpmnDeployer : Deployer
	{

	  private static readonly Logger log = LoggerFactory.getLogger(typeof(BpmnDeployer));

	  public static readonly string[] BPMN_RESOURCE_SUFFIXES = new string[] {"bpmn20.xml", "bpmn"};
	  public static readonly string[] DIAGRAM_SUFFIXES = new string[]{"png", "jpg", "gif", "svg"};

	  protected internal ExpressionManager expressionManager;
	  protected internal BpmnParser bpmnParser;
	  protected internal IdGenerator idGenerator;

	  public virtual void deploy(DeploymentEntity deployment, IDictionary<string, object> deploymentSettings)
	  {
		log.debug("Processing deployment {}", deployment.Name);

		IList<ProcessDefinitionEntity> processDefinitions = new List<ProcessDefinitionEntity>();
		IDictionary<string, ResourceEntity> resources = deployment.Resources;
		IDictionary<string, BpmnModel> bpmnModelMap = new Dictionary<string, BpmnModel>();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration = org.activiti.engine.impl.context.Context.getProcessEngineConfiguration();
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		foreach (string resourceName in resources.Keys)
		{

		  log.info("Processing resource {}", resourceName);
		  if (isBpmnResource(resourceName))
		  {
			ResourceEntity resource = resources[resourceName];
			sbyte[] bytes = resource.Bytes;
			ByteArrayInputStream inputStream = new ByteArrayInputStream(bytes);

			BpmnParse bpmnParse = bpmnParser.createParse().sourceInputStream(inputStream).setSourceSystemId(resourceName).deployment(deployment).name(resourceName);

			if (deploymentSettings != null)
			{

				// Schema validation if needed
				if (deploymentSettings.ContainsKey(org.activiti.engine.impl.cmd.DeploymentSettings_Fields.IS_BPMN20_XSD_VALIDATION_ENABLED))
				{
					bpmnParse.ValidateSchema = (bool?) deploymentSettings[org.activiti.engine.impl.cmd.DeploymentSettings_Fields.IS_BPMN20_XSD_VALIDATION_ENABLED];
				}

				// Process validation if needed
				if (deploymentSettings.ContainsKey(org.activiti.engine.impl.cmd.DeploymentSettings_Fields.IS_PROCESS_VALIDATION_ENABLED))
				{
					bpmnParse.ValidateProcess = (bool?) deploymentSettings[org.activiti.engine.impl.cmd.DeploymentSettings_Fields.IS_PROCESS_VALIDATION_ENABLED];
				}

			}
			else
			{
				// On redeploy, we assume it is validated at the first deploy
				bpmnParse.ValidateSchema = false;
				bpmnParse.ValidateProcess = false;
			}

			bpmnParse.execute();

			foreach (ProcessDefinitionEntity processDefinition in bpmnParse.ProcessDefinitions)
			{
			  processDefinition.ResourceName = resourceName;

			  if (deployment.TenantId != null)
			  {
				  processDefinition.TenantId = deployment.TenantId; // process definition inherits the tenant id
			  }

			  string diagramResourceName = getDiagramResourceForProcess(resourceName, processDefinition.Key, resources);

			  // Only generate the resource when deployment is new to prevent modification of deployment resources 
			  // after the process-definition is actually deployed. Also to prevent resource-generation failure every
			  // time the process definition is added to the deployment-cache when diagram-generation has failed the first time.
			  if (deployment.New)
			  {
				if (processEngineConfiguration.CreateDiagramOnDeploy && diagramResourceName == null && processDefinition.GraphicalNotationDefined)
				{
				  try
				  {
					  sbyte[] diagramBytes = IoUtil.readInputStream(processEngineConfiguration.ProcessDiagramGenerator.generateDiagram(bpmnParse.BpmnModel, "png", processEngineConfiguration.ActivityFontName, processEngineConfiguration.LabelFontName,processEngineConfiguration.AnnotationFontName, processEngineConfiguration.ClassLoader), null);
					  diagramResourceName = getProcessImageResourceName(resourceName, processDefinition.Key, "png");
					  createResource(diagramResourceName, diagramBytes, deployment);
				  } // if anything goes wrong, we don't store the image (the process will still be executable).
				  catch (Exception t)
				  {
					log.warn("Error while generating process diagram, image will not be stored in repository", t);
				  }
				}
			  }

			  processDefinition.DiagramResourceName = diagramResourceName;
			  processDefinitions.Add(processDefinition);
			  bpmnModelMap[processDefinition.Key] = bpmnParse.BpmnModel;
			}
		  }
		}

		// check if there are process definitions with the same process key to prevent database unique index violation
		IList<string> keyList = new List<string>();
		foreach (ProcessDefinitionEntity processDefinition in processDefinitions)
		{
		  if (keyList.Contains(processDefinition.Key))
		  {
			throw new ActivitiException("The deployment contains process definitions with the same key '" + processDefinition.Key + "' (process id atrribute), this is not allowed");
		  }
		  keyList.Add(processDefinition.Key);
		}

		CommandContext commandContext = Context.CommandContext;
		ProcessDefinitionEntityManager processDefinitionManager = commandContext.ProcessDefinitionEntityManager;
		DbSqlSession dbSqlSession = commandContext.getSession(typeof(DbSqlSession));
		foreach (ProcessDefinitionEntity processDefinition in processDefinitions)
		{
		  IList<TimerEntity> timers = new List<TimerEntity>();
		  if (deployment.New)
		  {
			int processDefinitionVersion;

			ProcessDefinitionEntity latestProcessDefinition = null;
			if (processDefinition.TenantId != null && !ProcessEngineConfiguration.NO_TENANT_ID.Equals(processDefinition.TenantId))
			{
				latestProcessDefinition = processDefinitionManager.findLatestProcessDefinitionByKeyAndTenantId(processDefinition.Key, processDefinition.TenantId);
			}
			else
			{
				latestProcessDefinition = processDefinitionManager.findLatestProcessDefinitionByKey(processDefinition.Key);
			}

			if (latestProcessDefinition != null)
			{
			  processDefinitionVersion = latestProcessDefinition.Version + 1;
			}
			else
			{
			  processDefinitionVersion = 1;
			}

			processDefinition.Version = processDefinitionVersion;
			processDefinition.DeploymentId = deployment.Id;

			string nextId = idGenerator.NextId;
			string processDefinitionId = processDefinition.Key + ":" + processDefinition.Version + ":" + nextId; // ACT-505

			// ACT-115: maximum id length is 64 charcaters
			if (processDefinitionId.Length > 64)
			{
			  processDefinitionId = nextId;
			}
			processDefinition.Id = processDefinitionId;

			if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
			{
				commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, processDefinition));
			}

			removeObsoleteTimers(processDefinition);
			addTimerDeclarations(processDefinition, timers);

			removeExistingMessageEventSubscriptions(processDefinition, latestProcessDefinition);
			addMessageEventSubscriptions(processDefinition);

			removeExistingSignalEventSubScription(processDefinition, latestProcessDefinition);
			addSignalEventSubscriptions(processDefinition);

			dbSqlSession.insert(processDefinition);
			addAuthorizations(processDefinition);

			if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
			{
				commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_INITIALIZED, processDefinition));
			}

			scheduleTimers(timers);

		  }
		  else
		  {
			string deploymentId = deployment.Id;
			processDefinition.DeploymentId = deploymentId;

			ProcessDefinitionEntity persistedProcessDefinition = null;
			if (processDefinition.TenantId == null || ProcessEngineConfiguration.NO_TENANT_ID.Equals(processDefinition.TenantId))
			{
				persistedProcessDefinition = processDefinitionManager.findProcessDefinitionByDeploymentAndKey(deploymentId, processDefinition.Key);
			}
			else
			{
				persistedProcessDefinition = processDefinitionManager.findProcessDefinitionByDeploymentAndKeyAndTenantId(deploymentId, processDefinition.Key, processDefinition.TenantId);
			}

			if (persistedProcessDefinition != null)
			{
				processDefinition.Id = persistedProcessDefinition.Id;
				processDefinition.Version = persistedProcessDefinition.Version;
				processDefinition.SuspensionState = persistedProcessDefinition.SuspensionState;
			}
		  }

		  // Add to cache
		  DeploymentManager deploymentManager = processEngineConfiguration.DeploymentManager;
		  deploymentManager.getProcessDefinitionCache().add(processDefinition.Id, processDefinition);
		  addDefinitionInfoToCache(processDefinition, processEngineConfiguration, commandContext);

		  // Add to deployment for further usage
		  deployment.addDeployedArtifact(processDefinition);

		  createLocalizationValues(processDefinition.Id, bpmnModelMap[processDefinition.Key].getProcessById(processDefinition.Key));
		}
	  }

	  protected internal virtual void addDefinitionInfoToCache(ProcessDefinitionEntity processDefinition, ProcessEngineConfigurationImpl processEngineConfiguration, CommandContext commandContext)
	  {

		if (processEngineConfiguration.EnableProcessDefinitionInfoCache == false)
		{
		  return;
		}

		DeploymentManager deploymentManager = processEngineConfiguration.DeploymentManager;
		ProcessDefinitionInfoEntityManager definitionInfoEntityManager = commandContext.ProcessDefinitionInfoEntityManager;
		ObjectMapper objectMapper = commandContext.ProcessEngineConfiguration.ObjectMapper;
		ProcessDefinitionInfoEntity definitionInfoEntity = definitionInfoEntityManager.findProcessDefinitionInfoByProcessDefinitionId(processDefinition.Id);

		ObjectNode infoNode = null;
		if (definitionInfoEntity != null && definitionInfoEntity.InfoJsonId != null)
		{
		  sbyte[] infoBytes = definitionInfoEntityManager.findInfoJsonById(definitionInfoEntity.InfoJsonId);
		  if (infoBytes != null)
		  {
			try
			{
			  infoNode = (ObjectNode) objectMapper.readTree(infoBytes);
			}
			catch (Exception)
			{
			  throw new ActivitiException("Error deserializing json info for process definition " + processDefinition.Id);
			}
		  }
		}

		ProcessDefinitionInfoCacheObject definitionCacheObject = new ProcessDefinitionInfoCacheObject();
		if (definitionInfoEntity == null)
		{
		  definitionCacheObject.Revision = 0;
		}
		else
		{
		  definitionCacheObject.Id = definitionInfoEntity.Id;
		  definitionCacheObject.Revision = definitionInfoEntity.Revision;
		}

		if (infoNode == null)
		{
		  infoNode = objectMapper.createObjectNode();
		}
		definitionCacheObject.InfoNode = infoNode;

		deploymentManager.getProcessDefinitionInfoCache().add(processDefinition.Id, definitionCacheObject);
	  }

	  protected internal virtual void scheduleTimers(IList<TimerEntity> timers)
	  {
		foreach (TimerEntity timer in timers)
		{
		  Context.CommandContext.JobEntityManager.schedule(timer);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void addTimerDeclarations(org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity processDefinition, java.util.List<org.activiti.engine.impl.persistence.entity.TimerEntity> timers)
	  protected internal virtual void addTimerDeclarations(ProcessDefinitionEntity processDefinition, IList<TimerEntity> timers)
	  {
		IList<TimerDeclarationImpl> timerDeclarations = (IList<TimerDeclarationImpl>) processDefinition.getProperty(BpmnParse.PROPERTYNAME_START_TIMER);
		if (timerDeclarations != null)
		{
		  foreach (TimerDeclarationImpl timerDeclaration in timerDeclarations)
		  {
			TimerEntity timer = timerDeclaration.prepareTimerEntity(null);
			if (timer != null)
			{
			  timer.ProcessDefinitionId = processDefinition.Id;

			  // Inherit timer (if appliccable)
			  if (processDefinition.TenantId != null)
			  {
				timer.TenantId = processDefinition.TenantId;
			  }
			  timers.Add(timer);
			}
		  }
		}
	  }

	  protected internal virtual void removeObsoleteTimers(ProcessDefinitionEntity processDefinition)
	  {

		  IList<Job> jobsToDelete = null;

		  if (processDefinition.TenantId != null && !ProcessEngineConfiguration.NO_TENANT_ID.Equals(processDefinition.TenantId))
		  {
			  jobsToDelete = Context.CommandContext.JobEntityManager.findJobsByTypeAndProcessDefinitionKeyAndTenantId(TimerStartEventJobHandler.TYPE, processDefinition.Key, processDefinition.TenantId);
		  }
		else
		{
			jobsToDelete = Context.CommandContext.JobEntityManager.findJobsByTypeAndProcessDefinitionKeyNoTenantId(TimerStartEventJobHandler.TYPE, processDefinition.Key);
		}

		  if (jobsToDelete != null)
		  {
			foreach (Job job in jobsToDelete)
			{
				(new CancelJobsCmd(job.Id)).execute(Context.CommandContext);
			}
		  }
	  }

	  protected internal virtual void removeExistingMessageEventSubscriptions(ProcessDefinitionEntity processDefinition, ProcessDefinitionEntity latestProcessDefinition)
	  {
		if (latestProcessDefinition != null)
		{
		  CommandContext commandContext = Context.CommandContext;

		  IList<EventSubscriptionEntity> subscriptionsToDisable = commandContext.EventSubscriptionEntityManager.findEventSubscriptionsByTypeAndProcessDefinitionId(MessageEventHandler.EVENT_HANDLER_TYPE, latestProcessDefinition.Id, latestProcessDefinition.TenantId);

		  foreach (EventSubscriptionEntity eventSubscriptionEntity in subscriptionsToDisable)
		  {
			eventSubscriptionEntity.delete();
		  }

		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void addMessageEventSubscriptions(org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity processDefinition)
	  protected internal virtual void addMessageEventSubscriptions(ProcessDefinitionEntity processDefinition)
	  {
		CommandContext commandContext = Context.CommandContext;
		IList<EventSubscriptionDeclaration> eventDefinitions = (IList<EventSubscriptionDeclaration>) processDefinition.getProperty(BpmnParse.PROPERTYNAME_EVENT_SUBSCRIPTION_DECLARATION);
		if (eventDefinitions != null)
		{

		  Set<string> messageNames = new HashSet<string>();
		  foreach (EventSubscriptionDeclaration eventDefinition in eventDefinitions)
		  {
			if (eventDefinition.EventType.Equals("message") && eventDefinition.StartEvent)
			{

			  if (!messageNames.contains(eventDefinition.EventName))
			  {
				messageNames.add(eventDefinition.EventName);
			  }
			  else
			  {
				throw new ActivitiException("Cannot deploy process definition '" + processDefinition.ResourceName + "': there are multiple message event subscriptions for the message with name '" + eventDefinition.EventName + "'.");
			  }

			  // look for subscriptions for the same name in db:
			  IList<EventSubscriptionEntity> subscriptionsForSameMessageName = commandContext.EventSubscriptionEntityManager.findEventSubscriptionsByName(MessageEventHandler.EVENT_HANDLER_TYPE, eventDefinition.EventName, processDefinition.TenantId);

			  // also look for subscriptions created in the session:
			  IList<MessageEventSubscriptionEntity> cachedSubscriptions = commandContext.DbSqlSession.findInCache(typeof(MessageEventSubscriptionEntity));
			  foreach (MessageEventSubscriptionEntity cachedSubscription in cachedSubscriptions)
			  {
				if (eventDefinition.EventName.Equals(cachedSubscription.EventName) && !subscriptionsForSameMessageName.Contains(cachedSubscription))
				{
				  subscriptionsForSameMessageName.Add(cachedSubscription);
				}
			  }

			  // remove subscriptions deleted in the same command
			  subscriptionsForSameMessageName = commandContext.DbSqlSession.pruneDeletedEntities(subscriptionsForSameMessageName);

			  foreach (EventSubscriptionEntity eventSubscriptionEntity in subscriptionsForSameMessageName)
			  {
				// throw exception only if there's already a subscription as start event

				// no process instance-id = it's a message start event
				if (StringUtils.isEmpty(eventSubscriptionEntity.ProcessInstanceId))
				{
				  throw new ActivitiException("Cannot deploy process definition '" + processDefinition.ResourceName + "': there already is a message event subscription for the message with name '" + eventDefinition.EventName + "'.");
				}
			  }

			  MessageEventSubscriptionEntity newSubscription = new MessageEventSubscriptionEntity();
			  newSubscription.EventName = eventDefinition.EventName;
			  newSubscription.ActivityId = eventDefinition.ActivityId;
			  newSubscription.Configuration = processDefinition.Id;
			  newSubscription.ProcessDefinitionId = processDefinition.Id;

			  if (processDefinition.TenantId != null)
			  {
				  newSubscription.TenantId = processDefinition.TenantId;
			  }

			  newSubscription.insert();
			}
		  }
		}
	  }

	  protected internal virtual void removeExistingSignalEventSubScription(ProcessDefinitionEntity processDefinition, ProcessDefinitionEntity latestProcessDefinition)
	  {
		if (latestProcessDefinition != null)
		{
		  CommandContext commandContext = Context.CommandContext;

		  IList<EventSubscriptionEntity> subscriptionsToDisable = commandContext.EventSubscriptionEntityManager.findEventSubscriptionsByTypeAndProcessDefinitionId(SignalEventHandler.EVENT_HANDLER_TYPE, latestProcessDefinition.Id, latestProcessDefinition.TenantId);

		  foreach (EventSubscriptionEntity eventSubscriptionEntity in subscriptionsToDisable)
		  {
			eventSubscriptionEntity.delete();
		  }

		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void addSignalEventSubscriptions(org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity processDefinition)
	  protected internal virtual void addSignalEventSubscriptions(ProcessDefinitionEntity processDefinition)
	  {
		 IList<EventSubscriptionDeclaration> eventDefinitions = (IList<EventSubscriptionDeclaration>) processDefinition.getProperty(BpmnParse.PROPERTYNAME_EVENT_SUBSCRIPTION_DECLARATION);
		 if (eventDefinitions != null)
		 {
		   foreach (EventSubscriptionDeclaration eventDefinition in eventDefinitions)
		   {
			 if (eventDefinition.EventType.Equals("signal") && eventDefinition.StartEvent)
			 {

				 SignalEventSubscriptionEntity subscriptionEntity = new SignalEventSubscriptionEntity();
				 subscriptionEntity.EventName = eventDefinition.EventName;
				 subscriptionEntity.ActivityId = eventDefinition.ActivityId;
				 subscriptionEntity.ProcessDefinitionId = processDefinition.Id;
				 if (processDefinition.TenantId != null)
				 {
					 subscriptionEntity.TenantId = processDefinition.TenantId;
				 }
				 subscriptionEntity.insert();

			 }
		   }
		 }
	  }

	  protected internal virtual void createLocalizationValues(string processDefinitionId, Process process)
	  {
		if (process == null)
		{
			return;
		}

		CommandContext commandContext = Context.CommandContext;
		DynamicBpmnService dynamicBpmnService = commandContext.ProcessEngineConfiguration.DynamicBpmnService;
		ObjectNode infoNode = dynamicBpmnService.getProcessDefinitionInfo(processDefinitionId);

		bool localizationValuesChanged = false;
		IList<ExtensionElement> localizationElements = process.ExtensionElements.get("localization");
		if (localizationElements != null)
		{
		  foreach (ExtensionElement localizationElement in localizationElements)
		  {
			if (BpmnXMLConstants.ACTIVITI_EXTENSIONS_PREFIX.Equals(localizationElement.NamespacePrefix))
			{
			  string locale = localizationElement.getAttributeValue(null, "locale");
			  string name = localizationElement.getAttributeValue(null, "name");
			  string documentation = null;
			  IList<ExtensionElement> documentationElements = localizationElement.ChildElements.get("documentation");
			  if (documentationElements != null)
			  {
				foreach (ExtensionElement documentationElement in documentationElements)
				{
				  documentation = StringUtils.trimToNull(documentationElement.ElementText);
				  break;
				}
			  }

			  string processId = process.Id;
			  if (isEqualToCurrentLocalizationValue(locale, processId, "name", name, infoNode) == false)
			  {
				dynamicBpmnService.changeLocalizationName(locale, processId, name, infoNode);
				localizationValuesChanged = true;
			  }

			  if (documentation != null && isEqualToCurrentLocalizationValue(locale, processId, "description", documentation, infoNode) == false)
			  {
				dynamicBpmnService.changeLocalizationDescription(locale, processId, documentation, infoNode);
				localizationValuesChanged = true;
			  }

			  break;
			}
		  }
		}

		bool isFlowElementLocalizationChanged = localizeFlowElements(process.FlowElements, infoNode);
		bool isDataObjectLocalizationChanged = localizeDataObjectElements(process.DataObjects, infoNode);
		if (isFlowElementLocalizationChanged || isDataObjectLocalizationChanged)
		{
		  localizationValuesChanged = true;
		}

		if (localizationValuesChanged)
		{
		  dynamicBpmnService.saveProcessDefinitionInfo(processDefinitionId, infoNode);
		}
	  }

	  protected internal virtual bool localizeFlowElements(ICollection<FlowElement> flowElements, ObjectNode infoNode)
	  {
		bool localizationValuesChanged = false;

		if (flowElements == null)
		{
			return localizationValuesChanged;
		}

		CommandContext commandContext = Context.CommandContext;
		DynamicBpmnService dynamicBpmnService = commandContext.ProcessEngineConfiguration.DynamicBpmnService;

		foreach (FlowElement flowElement in flowElements)
		{
		  if (flowElement is UserTask || flowElement is SubProcess)
		  {
			IList<ExtensionElement> localizationElements = flowElement.ExtensionElements.get("localization");
			if (localizationElements != null)
			{
			  foreach (ExtensionElement localizationElement in localizationElements)
			  {
				if (BpmnXMLConstants.ACTIVITI_EXTENSIONS_PREFIX.Equals(localizationElement.NamespacePrefix))
				{
				  string locale = localizationElement.getAttributeValue(null, "locale");
				  string name = localizationElement.getAttributeValue(null, "name");
				  string documentation = null;
				  IList<ExtensionElement> documentationElements = localizationElement.ChildElements.get("documentation");
				  if (documentationElements != null)
				  {
					foreach (ExtensionElement documentationElement in documentationElements)
					{
					  documentation = StringUtils.trimToNull(documentationElement.ElementText);
					  break;
					}
				  }

				  string flowElementId = flowElement.Id;
				  if (isEqualToCurrentLocalizationValue(locale, flowElementId, "name", name, infoNode) == false)
				  {
					dynamicBpmnService.changeLocalizationName(locale, flowElementId, name, infoNode);
					localizationValuesChanged = true;
				  }

				  if (documentation != null && isEqualToCurrentLocalizationValue(locale, flowElementId, "description", documentation, infoNode) == false)
				  {
					dynamicBpmnService.changeLocalizationDescription(locale, flowElementId, documentation, infoNode);
					localizationValuesChanged = true;
				  }

				  break;
				}
			  }
			}

			if (flowElement is SubProcess)
			{
			  SubProcess subprocess = (SubProcess) flowElement;
			  bool isFlowElementLocalizationChanged = localizeFlowElements(subprocess.FlowElements, infoNode);
			  bool isDataObjectLocalizationChanged = localizeDataObjectElements(subprocess.DataObjects, infoNode);
			  if (isFlowElementLocalizationChanged || isDataObjectLocalizationChanged)
			  {
				localizationValuesChanged = true;
			  }
			}
		  }
		}

		return localizationValuesChanged;
	  }

	  protected internal virtual bool isEqualToCurrentLocalizationValue(string language, string id, string propertyName, string propertyValue, ObjectNode infoNode)
	  {
		bool isEqual = false;
		JsonNode localizationNode = infoNode.path("localization").path(language).path(id).path(propertyName);
		if (localizationNode.MissingNode == false && localizationNode.Null == false && localizationNode.asText().Equals(propertyValue))
		{
		  isEqual = true;
		}
		return isEqual;
	  }

	  protected internal virtual bool localizeDataObjectElements(IList<ValuedDataObject> dataObjects, ObjectNode infoNode)
	  {
		bool localizationValuesChanged = false;
		CommandContext commandContext = Context.CommandContext;
		DynamicBpmnService dynamicBpmnService = commandContext.ProcessEngineConfiguration.DynamicBpmnService;

		foreach (ValuedDataObject dataObject in dataObjects)
		{
		  IList<ExtensionElement> localizationElements = dataObject.ExtensionElements.get("localization");
		  if (localizationElements != null)
		  {
			foreach (ExtensionElement localizationElement in localizationElements)
			{
			  if (BpmnXMLConstants.ACTIVITI_EXTENSIONS_PREFIX.Equals(localizationElement.NamespacePrefix))
			  {
				string locale = localizationElement.getAttributeValue(null, "locale");
				string name = localizationElement.getAttributeValue(null, "name");
				string documentation = null;

				IList<ExtensionElement> documentationElements = localizationElement.ChildElements.get("documentation");
				if (documentationElements != null)
				{
				  foreach (ExtensionElement documentationElement in documentationElements)
				  {
					documentation = StringUtils.trimToNull(documentationElement.ElementText);
					break;
				  }
				}

				if (name != null && isEqualToCurrentLocalizationValue(locale, dataObject.Name, org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_NAME, name, infoNode) == false)
				{
				  dynamicBpmnService.changeLocalizationName(locale, dataObject.Name, name, infoNode);
				  localizationValuesChanged = true;
				}

				if (documentation != null && isEqualToCurrentLocalizationValue(locale, dataObject.Name, org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_DESCRIPTION, documentation, infoNode) == false)
				{

				  dynamicBpmnService.changeLocalizationDescription(locale, dataObject.Name, documentation, infoNode);
				  localizationValuesChanged = true;
				}
			  }
			}
		  }
		}

		return localizationValuesChanged;
	  }

	  internal enum ExprType
	  {
		  USER,
		  GROUP
	  }

	  private void addAuthorizationsFromIterator(Set<Expression> exprSet, ProcessDefinitionEntity processDefinition, ExprType exprType)
	  {
		if (exprSet != null)
		{
		  IEnumerator<Expression> iterator = exprSet.GetEnumerator();
		  while (iterator.MoveNext())
		  {
			Expression expr = (Expression) iterator.Current;
			IdentityLinkEntity identityLink = new IdentityLinkEntity();
			identityLink.setProcessDef(processDefinition);
			if (exprType.Equals(ExprType.USER))
			{
			   identityLink.UserId = expr.ToString();
			}
			else if (exprType.Equals(ExprType.GROUP))
			{
			  identityLink.GroupId = expr.ToString();
			}
			identityLink.Type = IdentityLinkType.CANDIDATE;
			identityLink.insert();
		  }
		}
	  }

	  protected internal virtual void addAuthorizations(ProcessDefinitionEntity processDefinition)
	  {
		addAuthorizationsFromIterator(processDefinition.CandidateStarterUserIdExpressions, processDefinition, ExprType.USER);
		addAuthorizationsFromIterator(processDefinition.CandidateStarterGroupIdExpressions, processDefinition, ExprType.GROUP);
	  }

	  /// <summary>
	  /// Returns the default name of the image resource for a certain process.
	  /// 
	  /// It will first look for an image resource which matches the process
	  /// specifically, before resorting to an image resource which matches the BPMN
	  /// 2.0 xml file resource.
	  /// 
	  /// Example: if the deployment contains a BPMN 2.0 xml resource called
	  /// 'abc.bpmn20.xml' containing only one process with key 'myProcess', then
	  /// this method will look for an image resources called 'abc.myProcess.png'
	  /// (or .jpg, or .gif, etc.) or 'abc.png' if the previous one wasn't found.
	  /// 
	  /// Example 2: if the deployment contains a BPMN 2.0 xml resource called 
	  /// 'abc.bpmn20.xml' containing three processes (with keys a, b and c),
	  /// then this method will first look for an image resource called 'abc.a.png' 
	  /// before looking for 'abc.png' (likewise for b and c).
	  /// Note that if abc.a.png, abc.b.png and abc.c.png don't exist, all
	  /// processes will have the same image: abc.png.
	  /// </summary>
	  /// <returns> null if no matching image resource is found. </returns>
	  protected internal virtual string getDiagramResourceForProcess(string bpmnFileResource, string processKey, IDictionary<string, ResourceEntity> resources)
	  {
		foreach (string diagramSuffix in DIAGRAM_SUFFIXES)
		{
		  string diagramForBpmnFileResource = getBpmnFileImageResourceName(bpmnFileResource, diagramSuffix);
		  string processDiagramResource = getProcessImageResourceName(bpmnFileResource, processKey, diagramSuffix);
		  if (resources.ContainsKey(processDiagramResource))
		  {
			return processDiagramResource;
		  }
		  else if (resources.ContainsKey(diagramForBpmnFileResource))
		  {
			return diagramForBpmnFileResource;
		  }
		}
		return null;
	  }

	  protected internal virtual string getBpmnFileImageResourceName(string bpmnFileResource, string diagramSuffix)
	  {
		string bpmnFileResourceBase = stripBpmnFileSuffix(bpmnFileResource);
		return bpmnFileResourceBase + diagramSuffix;
	  }

	  protected internal virtual string getProcessImageResourceName(string bpmnFileResource, string processKey, string diagramSuffix)
	  {
		string bpmnFileResourceBase = stripBpmnFileSuffix(bpmnFileResource);
		return bpmnFileResourceBase + processKey + "." + diagramSuffix;
	  }

	  protected internal virtual string stripBpmnFileSuffix(string bpmnFileResource)
	  {
		foreach (string suffix in BPMN_RESOURCE_SUFFIXES)
		{
		  if (bpmnFileResource.EndsWith(suffix))
		  {
			return bpmnFileResource.Substring(0, bpmnFileResource.Length - suffix.Length);
		  }
		}
		return bpmnFileResource;
	  }

	  protected internal virtual void createResource(string name, sbyte[] bytes, DeploymentEntity deploymentEntity)
	  {
		ResourceEntity resource = new ResourceEntity();
		resource.Name = name;
		resource.Bytes = bytes;
		resource.DeploymentId = deploymentEntity.Id;

		// Mark the resource as 'generated'
		resource.Generated = true;

		Context.CommandContext.DbSqlSession.insert(resource);
	  }

	  protected internal virtual bool isBpmnResource(string resourceName)
	  {
		foreach (string suffix in BPMN_RESOURCE_SUFFIXES)
		{
		  if (resourceName.EndsWith(suffix))
		  {
			return true;
		  }
		}
		return false;
	  }

	  public virtual ExpressionManager ExpressionManager
	  {
		  get
		  {
			return expressionManager;
		  }
		  set
		  {
			this.expressionManager = value;
		  }
	  }


	  public virtual BpmnParser BpmnParser
	  {
		  get
		  {
			return bpmnParser;
		  }
		  set
		  {
			this.bpmnParser = value;
		  }
	  }


	  public virtual IdGenerator IdGenerator
	  {
		  get
		  {
			return idGenerator;
		  }
		  set
		  {
			this.idGenerator = value;
		  }
	  }


	}

}