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
namespace org.activiti.engine.impl
{


	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using SuspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState;
	using Execution = org.activiti.engine.runtime.Execution;
	using ExecutionQuery = org.activiti.engine.runtime.ExecutionQuery;

	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;


	/// <summary>
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// @author Daniel Meyer
	/// </summary>
	public class ExecutionQueryImpl : AbstractVariableQueryImpl<ExecutionQuery, Execution>, ExecutionQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string processDefinitionId_Renamed;
	  protected internal string processDefinitionKey_Renamed;
	  protected internal string processDefinitionCategory_Renamed;
	  protected internal string processDefinitionName_Renamed;
	  protected internal int? processDefinitionVersion_Renamed;
	  protected internal string activityId_Renamed;
	  protected internal string executionId_Renamed;
	  protected internal string parentId_Renamed;
	  protected internal string processInstanceId_Renamed;
	  protected internal IList<EventSubscriptionQueryValue> eventSubscriptions;

	  protected internal string tenantId;
	  protected internal string tenantIdLike;
	  protected internal bool withoutTenantId;
	  protected internal string locale_Renamed;
	  protected internal bool withLocalizationFallback_Renamed;

	  // Not used by end-users, but needed for dynamic ibatis query
	  protected internal string superProcessInstanceId;
	  protected internal string subProcessInstanceId;
	  protected internal bool excludeSubprocesses;
	  protected internal SuspensionState suspensionState;
	  protected internal string businessKey;
	  protected internal bool includeChildExecutionsWithBusinessKeyQuery;
	  protected internal bool isActive;
	  protected internal string involvedUser;
	  protected internal Set<string> processDefinitionKeys_Renamed;
	  protected internal Set<string> processDefinitionIds;

	  // Not exposed in API, but here for the ProcessInstanceQuery support, since the name lives on the
	  // Execution entity/table
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string nameLikeIgnoreCase;
	  protected internal string deploymentId;
	  protected internal IList<string> deploymentIds;
	  protected internal IList<ExecutionQueryImpl> orQueryObjects = new List<ExecutionQueryImpl>();

	  public ExecutionQueryImpl()
	  {
	  }

	  public ExecutionQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public ExecutionQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual bool ProcessInstancesOnly
	  {
		  get
		  {
			return false; // see dynamic query
		  }
	  }

	  public virtual ExecutionQueryImpl processDefinitionId(string processDefinitionId)
	  {
		if (processDefinitionId == null)
		{
		  throw new ActivitiIllegalArgumentException("Process definition id is null");
		}
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual ExecutionQueryImpl processDefinitionKey(string processDefinitionKey)
	  {
		if (processDefinitionKey == null)
		{
		  throw new ActivitiIllegalArgumentException("Process definition key is null");
		}
		this.processDefinitionKey_Renamed = processDefinitionKey;
		return this;
	  }

	  public override ExecutionQuery processDefinitionCategory(string processDefinitionCategory)
	  {
		if (processDefinitionCategory == null)
		{
		  throw new ActivitiIllegalArgumentException("Process definition category is null");
		}
		this.processDefinitionCategory_Renamed = processDefinitionCategory;
		return this;
	  }

	  public override ExecutionQuery processDefinitionName(string processDefinitionName)
	  {
		if (processDefinitionName == null)
		{
		  throw new ActivitiIllegalArgumentException("Process definition name is null");
		}
		this.processDefinitionName_Renamed = processDefinitionName;
		return this;
	  }

	  public override ExecutionQuery processDefinitionVersion(int? processDefinitionVersion)
	  {
		if (processDefinitionVersion == null)
		{
		  throw new ActivitiIllegalArgumentException("Process definition version is null");
		}
		this.processDefinitionVersion_Renamed = processDefinitionVersion;
		return this;
	  }

	  public virtual ExecutionQueryImpl processInstanceId(string processInstanceId)
	  {
		if (processInstanceId == null)
		{
		  throw new ActivitiIllegalArgumentException("Process instance id is null");
		}
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual ExecutionQuery processInstanceBusinessKey(string businessKey)
	  {
		if (businessKey == null)
		{
		  throw new ActivitiIllegalArgumentException("Business key is null");
		}
		this.businessKey = businessKey;
		return this;
	  }

	  public virtual ExecutionQuery processInstanceBusinessKey(string processInstanceBusinessKey, bool includeChildExecutions)
	  {
		if (!includeChildExecutions)
		{
		  return processInstanceBusinessKey(processInstanceBusinessKey);
		}
		else
		{
		  if (processInstanceBusinessKey == null)
		  {
			throw new ActivitiIllegalArgumentException("Business key is null");
		  }
		  this.businessKey = processInstanceBusinessKey;
		  this.includeChildExecutionsWithBusinessKeyQuery = includeChildExecutions;
		  return this;
		}
	  }

	  public virtual ExecutionQuery processDefinitionKeys(Set<string> processDefinitionKeys)
	  {
		if (processDefinitionKeys == null)
		{
		  throw new ActivitiIllegalArgumentException("Process definition keys is null");
		}
		this.processDefinitionKeys_Renamed = processDefinitionKeys;
		return this;
	  }

	  public virtual ExecutionQueryImpl executionId(string executionId)
	  {
		if (executionId == null)
		{
		  throw new ActivitiIllegalArgumentException("Execution id is null");
		}
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual ExecutionQueryImpl activityId(string activityId)
	  {
		this.activityId_Renamed = activityId;

		if (activityId != null)
		{
		  isActive = true;
		}
		return this;
	  }

	  public virtual ExecutionQueryImpl parentId(string parentId)
	  {
		if (parentId == null)
		{
		  throw new ActivitiIllegalArgumentException("Parent id is null");
		}
		this.parentId_Renamed = parentId;
		return this;
	  }

	  public virtual ExecutionQueryImpl executionTenantId(string tenantId)
	  {
		  if (tenantId == null)
		  {
			  throw new ActivitiIllegalArgumentException("execution tenant id is null");
		  }
		  this.tenantId = tenantId;
		  return this;
	  }

	  public virtual ExecutionQueryImpl executionTenantIdLike(string tenantIdLike)
	  {
		  if (tenantIdLike == null)
		  {
			  throw new ActivitiIllegalArgumentException("execution tenant id is null");
		  }
		  this.tenantIdLike = tenantIdLike;
		  return this;
	  }

	  public virtual ExecutionQueryImpl executionWithoutTenantId()
	  {
		  this.withoutTenantId = true;
		  return this;
	  }

	  public virtual ExecutionQuery signalEventSubscription(string signalName)
	  {
		return eventSubscription("signal", signalName);
	  }

	  public virtual ExecutionQuery signalEventSubscriptionName(string signalName)
	  {
		return eventSubscription("signal", signalName);
	  }

	  public virtual ExecutionQuery messageEventSubscriptionName(string messageName)
	  {
		return eventSubscription("message", messageName);
	  }

	  public virtual ExecutionQuery eventSubscription(string eventType, string eventName)
	  {
		if (eventName == null)
		{
		  throw new ActivitiIllegalArgumentException("event name is null");
		}
		if (eventType == null)
		{
		  throw new ActivitiIllegalArgumentException("event type is null");
		}
		if (eventSubscriptions == null)
		{
		  eventSubscriptions = new List<EventSubscriptionQueryValue>();
		}
		eventSubscriptions.Add(new EventSubscriptionQueryValue(eventName, eventType));
		return this;
	  }

	  public virtual ExecutionQuery processVariableValueEquals(string variableName, object variableValue)
	  {
		return variableValueEquals(variableName, variableValue, false);
	  }

	  public virtual ExecutionQuery processVariableValueEquals(object variableValue)
	  {
		return variableValueEquals(variableValue, false);
	  }

	  public virtual ExecutionQuery processVariableValueNotEquals(string variableName, object variableValue)
	  {
		return variableValueNotEquals(variableName, variableValue, false);
	  }

	  public virtual ExecutionQuery processVariableValueEqualsIgnoreCase(string name, string value)
	  {
		return variableValueEqualsIgnoreCase(name, value, false);
	  }

	  public virtual ExecutionQuery processVariableValueNotEqualsIgnoreCase(string name, string value)
	  {
		return variableValueNotEqualsIgnoreCase(name, value, false);
	  }

	  public virtual ExecutionQuery processVariableValueLike(string name, string value)
	  {
		return variableValueLike(name, value, false);
	  }

	  public virtual ExecutionQuery processVariableValueLikeIgnoreCase(string name, string value)
	  {
		return variableValueLikeIgnoreCase(name, value, false);
	  }

	  public override ExecutionQuery locale(string locale)
	  {
		this.locale_Renamed = locale;
		return this;
	  }

	  public virtual ExecutionQuery withLocalizationFallback()
	  {
		withLocalizationFallback_Renamed = true;
		return this;
	  }

	  //ordering ////////////////////////////////////////////////////

	  public virtual ExecutionQueryImpl orderByProcessInstanceId()
	  {
		this.orderProperty = ExecutionQueryProperty.PROCESS_INSTANCE_ID;
		return this;
	  }

	  public virtual ExecutionQueryImpl orderByProcessDefinitionId()
	  {
		this.orderProperty = ExecutionQueryProperty.PROCESS_DEFINITION_ID;
		return this;
	  }

	  public virtual ExecutionQueryImpl orderByProcessDefinitionKey()
	  {
		this.orderProperty = ExecutionQueryProperty.PROCESS_DEFINITION_KEY;
		return this;
	  }

	  public virtual ExecutionQueryImpl orderByTenantId()
	  {
		  this.orderProperty = ExecutionQueryProperty.TENANT_ID;
		  return this;
	  }

	  //results ////////////////////////////////////////////////////

	  public virtual long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.ExecutionEntityManager.findExecutionCountByQueryCriteria(this);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked" }) public java.util.List<org.activiti.engine.runtime.Execution> executeList(org.activiti.engine.impl.interceptor.CommandContext commandContext, Page page)
	  public virtual IList<Execution> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<?> executions = commandContext.getExecutionEntityManager().findExecutionsByQueryCriteria(this, page);
		IList<?> executions = commandContext.ExecutionEntityManager.findExecutionsByQueryCriteria(this, page);

		foreach (ExecutionEntity execution in (IList<ExecutionEntity>) executions)
		{
		  string activityId = null;
		  if (execution.Id.Equals(execution.ProcessInstanceId))
		  {
			if (execution.ProcessDefinitionId != null)
			{
			  ProcessDefinitionEntity processDefinition = commandContext.ProcessEngineConfiguration.DeploymentManager.findDeployedProcessDefinitionById(execution.ProcessDefinitionId);
			  activityId = processDefinition.Key;
			}

		  }
		  else
		  {
			activityId = execution.ActivityId;
		  }

		  if (activityId != null)
		  {
			localize(execution, activityId);
		  }
		}

		return (IList<Execution>) executions;
	  }

	  protected internal virtual void localize(Execution execution, string activityId)
	  {
		ExecutionEntity executionEntity = (ExecutionEntity) execution;
		executionEntity.LocalizedName = null;
		executionEntity.LocalizedDescription = null;

		string processDefinitionId = executionEntity.ProcessDefinitionId;
		if (locale_Renamed != null && processDefinitionId != null)
		{
		  ObjectNode languageNode = Context.getLocalizationElementProperties(locale_Renamed, activityId, processDefinitionId, withLocalizationFallback_Renamed);
		  if (languageNode != null)
		  {
			JsonNode languageNameNode = languageNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_NAME);
			if (languageNameNode != null && languageNameNode.Null == false)
			{
			  executionEntity.LocalizedName = languageNameNode.asText();
			}

			JsonNode languageDescriptionNode = languageNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_DESCRIPTION);
			if (languageDescriptionNode != null && languageDescriptionNode.Null == false)
			{
			  executionEntity.LocalizedDescription = languageDescriptionNode.asText();
			}
		  }
		}
	  }

	  //getters ////////////////////////////////////////////////////

	  public virtual bool OnlyProcessInstances
	  {
		  get
		  {
			return false;
		  }
	  }
	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Renamed;
		  }
	  }
	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Renamed;
		  }
	  }
	  public virtual string ProcessDefinitionCategory
	  {
		  get
		  {
			return processDefinitionCategory_Renamed;
		  }
	  }
	  public virtual string ProcessDefinitionName
	  {
		  get
		  {
			return processDefinitionName_Renamed;
		  }
	  }
	  public virtual int? ProcessDefinitionVersion
	  {
		  get
		  {
			return processDefinitionVersion_Renamed;
		  }
	  }
	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId_Renamed;
		  }
	  }
	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Renamed;
		  }
	  }
	  public virtual string ProcessInstanceIds
	  {
		  get
		  {
			return null;
		  }
	  }
	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
	  }
	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Renamed;
		  }
	  }
	  public virtual string SuperProcessInstanceId
	  {
		  get
		  {
			return superProcessInstanceId;
		  }
	  }
	  public virtual string SubProcessInstanceId
	  {
		  get
		  {
			return subProcessInstanceId;
		  }
	  }
	  public virtual bool ExcludeSubprocesses
	  {
		  get
		  {
			return excludeSubprocesses;
		  }
	  }
	  public virtual SuspensionState SuspensionState
	  {
		  get
		  {
			return suspensionState;
		  }
		  set
		  {
			this.suspensionState = value;
		  }
	  }
	  public virtual IList<EventSubscriptionQueryValue> EventSubscriptions
	  {
		  get
		  {
			return eventSubscriptions;
		  }
		  set
		  {
			this.eventSubscriptions = value;
		  }
	  }
	  public virtual bool IncludeChildExecutionsWithBusinessKeyQuery
	  {
		  get
		  {
			return includeChildExecutionsWithBusinessKeyQuery;
		  }
	  }

	  public virtual bool Active
	  {
		  get
		  {
			return isActive;
		  }
	  }
	  public virtual string InvolvedUser
	  {
		  get
		  {
			return involvedUser;
		  }
		  set
		  {
			this.involvedUser = value;
		  }
	  }
	  public virtual Set<string> ProcessDefinitionIds
	  {
		  get
		  {
			return processDefinitionIds;
		  }
	  }
	  public virtual Set<string> ProcessDefinitionKeys
	  {
		  get
		  {
			return processDefinitionKeys_Renamed;
		  }
	  }
	  public virtual string ParentId
	  {
		  get
		  {
			return parentId_Renamed;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual string TenantIdLike
	  {
		  get
		  {
			return tenantIdLike;
		  }
	  }

	  public virtual bool WithoutTenantId
	  {
		  get
		  {
			return withoutTenantId;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }

	  public virtual string NameLike
	  {
		  get
		  {
			return nameLike;
		  }
		  set
		  {
			this.nameLike = value;
		  }
	  }



		public virtual string NameLikeIgnoreCase
		{
			get
			{
				return nameLikeIgnoreCase;
			}
			set
			{
				this.nameLikeIgnoreCase = value;
			}
		}



	}

}