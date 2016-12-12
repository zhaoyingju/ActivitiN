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

namespace org.activiti.engine.impl
{


	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using SuspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.activiti.engine.runtime.ProcessInstanceQuery;

	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Tijs Rademakers
	/// @author Frederik Heremans
	/// @author Falko Menge
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class ProcessInstanceQueryImpl : AbstractVariableQueryImpl<ProcessInstanceQuery, ProcessInstance>, ProcessInstanceQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string executionId;
	  protected internal string businessKey;
	  protected internal bool includeChildExecutionsWithBusinessKeyQuery;
	  protected internal string processDefinitionId_Renamed;
	  protected internal Set<string> processDefinitionIds_Renamed;
	  protected internal string processDefinitionCategory_Renamed;
	  protected internal string processDefinitionName_Renamed;
	  protected internal int? processDefinitionVersion_Renamed;
	  protected internal Set<string> processInstanceIds_Renamed;
	  protected internal string processDefinitionKey_Renamed;
	  protected internal Set<string> processDefinitionKeys_Renamed;
	  protected internal string deploymentId_Renamed;
	  protected internal IList<string> deploymentIds;
	  protected internal string superProcessInstanceId_Renamed;
	  protected internal string subProcessInstanceId_Renamed;
	  protected internal bool excludeSubprocesses_Renamed;
	  protected internal string involvedUser_Renamed;
	  protected internal SuspensionState suspensionState;
	  protected internal bool includeProcessVariables_Renamed;
	  protected internal int? processInstanceVariablesLimit;
	  protected internal bool withJobException_Renamed;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string nameLikeIgnoreCase;
	  protected internal string locale_Renamed;
	  protected internal bool withLocalizationFallback_Renamed;

	  protected internal string tenantId;
	  protected internal string tenantIdLike;
	  protected internal bool withoutTenantId;

	  protected internal IList<ProcessInstanceQueryImpl> orQueryObjects = new List<ProcessInstanceQueryImpl>();
	  protected internal ProcessInstanceQueryImpl currentOrQueryObject = null;
	  protected internal bool inOrStatement = false;

	  // Unused, see dynamic query
	  protected internal string activityId;
	  protected internal IList<EventSubscriptionQueryValue> eventSubscriptions;

	  public ProcessInstanceQueryImpl()
	  {
	  }

	  public ProcessInstanceQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public ProcessInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual ProcessInstanceQueryImpl processInstanceId(string processInstanceId)
	  {
		if (processInstanceId == null)
		{
		  throw new ActivitiIllegalArgumentException("Process instance id is null");
		}
		if (inOrStatement)
		{
		  this.currentOrQueryObject.executionId = processInstanceId;
		}
		else
		{
		  this.executionId = processInstanceId;
		}
		return this;
	  }

	  public virtual ProcessInstanceQuery processInstanceIds(Set<string> processInstanceIds)
	  {
		if (processInstanceIds == null)
		{
		  throw new ActivitiIllegalArgumentException("Set of process instance ids is null");
		}
		if (processInstanceIds.Empty)
		{
		  throw new ActivitiIllegalArgumentException("Set of process instance ids is empty");
		}

		if (inOrStatement)
		{
		  this.currentOrQueryObject.processInstanceIds_Renamed = processInstanceIds;
		}
		else
		{
		  this.processInstanceIds_Renamed = processInstanceIds;
		}
		return this;
	  }

	  public virtual ProcessInstanceQuery processInstanceBusinessKey(string businessKey)
	  {
		if (businessKey == null)
		{
		  throw new ActivitiIllegalArgumentException("Business key is null");
		}
		if (inOrStatement)
		{
		  this.currentOrQueryObject.businessKey = businessKey;
		}
		else
		{
		  this.businessKey = businessKey;
		}
		return this;
	  }

	  public virtual ProcessInstanceQuery processInstanceBusinessKey(string businessKey, string processDefinitionKey)
	  {
		if (businessKey == null)
		{
		  throw new ActivitiIllegalArgumentException("Business key is null");
		}
		if (inOrStatement)
		{
		  throw new ActivitiIllegalArgumentException("This method is not supported in an OR statement");
		}

		this.businessKey = businessKey;
		this.processDefinitionKey_Renamed = processDefinitionKey;
		return this;
	  }

	  public virtual ProcessInstanceQuery processInstanceTenantId(string tenantId)
	  {
		  if (tenantId == null)
		  {
			  throw new ActivitiIllegalArgumentException("process instance tenant id is null");
		  }
		  if (inOrStatement)
		  {
		  this.currentOrQueryObject.tenantId = tenantId;
		  }
		else
		{
		  this.tenantId = tenantId;
		}
		  return this;
	  }

	  public virtual ProcessInstanceQuery processInstanceTenantIdLike(string tenantIdLike)
	  {
		  if (tenantIdLike == null)
		  {
			  throw new ActivitiIllegalArgumentException("process instance tenant id is null");
		  }
		  if (inOrStatement)
		  {
		  this.currentOrQueryObject.tenantIdLike = tenantIdLike;
		  }
		else
		{
		  this.tenantIdLike = tenantIdLike;
		}
		  return this;
	  }

	  public virtual ProcessInstanceQuery processInstanceWithoutTenantId()
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.withoutTenantId = true;
		}
		else
		{
		  this.withoutTenantId = true;
		}
		  return this;
	  }

	  public override ProcessInstanceQuery processDefinitionCategory(string processDefinitionCategory)
	  {
		if (processDefinitionCategory == null)
		{
		  throw new ActivitiIllegalArgumentException("Process definition category is null");
		}

		if (inOrStatement)
		{
		  this.currentOrQueryObject.processDefinitionCategory_Renamed = processDefinitionCategory;
		}
		else
		{
		  this.processDefinitionCategory_Renamed = processDefinitionCategory;
		}
		return this;
	  }

	  public override ProcessInstanceQuery processDefinitionName(string processDefinitionName)
	  {
		if (processDefinitionName == null)
		{
		  throw new ActivitiIllegalArgumentException("Process definition name is null");
		}

		if (inOrStatement)
		{
		  this.currentOrQueryObject.processDefinitionName_Renamed = processDefinitionName;
		}
		else
		{
		  this.processDefinitionName_Renamed = processDefinitionName;
		}
		return this;
	  }

	  public override ProcessInstanceQuery processDefinitionVersion(int? processDefinitionVersion)
	  {
		if (processDefinitionVersion == null)
		{
		  throw new ActivitiIllegalArgumentException("Process definition version is null");
		}

		if (inOrStatement)
		{
		  this.currentOrQueryObject.processDefinitionVersion_Renamed = processDefinitionVersion;
		}
		else
		{
		  this.processDefinitionVersion_Renamed = processDefinitionVersion;
		}
		return this;
	  }

	  public virtual ProcessInstanceQueryImpl processDefinitionId(string processDefinitionId)
	  {
		if (processDefinitionId == null)
		{
		  throw new ActivitiIllegalArgumentException("Process definition id is null");
		}

		if (inOrStatement)
		{
		  this.currentOrQueryObject.processDefinitionId_Renamed = processDefinitionId;
		}
		else
		{
		  this.processDefinitionId_Renamed = processDefinitionId;
		}
		return this;
	  }

	  public override ProcessInstanceQuery processDefinitionIds(Set<string> processDefinitionIds)
	  {
		if (processDefinitionIds == null)
		{
		  throw new ActivitiIllegalArgumentException("Set of process definition ids is null");
		}
		if (processDefinitionIds.Empty)
		{
		  throw new ActivitiIllegalArgumentException("Set of process definition ids is empty");
		}

		if (inOrStatement)
		{
		  this.currentOrQueryObject.processDefinitionIds_Renamed = processDefinitionIds;
		}
		else
		{
		  this.processDefinitionIds_Renamed = processDefinitionIds;
		}
		return this;
	  }

	  public virtual ProcessInstanceQueryImpl processDefinitionKey(string processDefinitionKey)
	  {
		if (processDefinitionKey == null)
		{
		  throw new ActivitiIllegalArgumentException("Process definition key is null");
		}

		if (inOrStatement)
		{
		  this.currentOrQueryObject.processDefinitionKey_Renamed = processDefinitionKey;
		}
		else
		{
		  this.processDefinitionKey_Renamed = processDefinitionKey;
		}
		return this;
	  }

	  public override ProcessInstanceQuery processDefinitionKeys(Set<string> processDefinitionKeys)
	  {
		if (processDefinitionKeys == null)
		{
		  throw new ActivitiIllegalArgumentException("Set of process definition keys is null");
		}
		if (processDefinitionKeys.Empty)
		{
		  throw new ActivitiIllegalArgumentException("Set of process definition keys is empty");
		}

		if (inOrStatement)
		{
		  this.currentOrQueryObject.processDefinitionKeys_Renamed = processDefinitionKeys;
		}
		else
		{
		  this.processDefinitionKeys_Renamed = processDefinitionKeys;
		}
		return this;
	  }

	  public virtual ProcessInstanceQueryImpl deploymentId(string deploymentId)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.deploymentId_Renamed = deploymentId;
		}
		else
		{
		  this.deploymentId_Renamed = deploymentId;
		}
		return this;
	  }

	  public virtual ProcessInstanceQueryImpl deploymentIdIn(IList<string> deploymentIds)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.deploymentIds = deploymentIds;
		}
		else
		{
		  this.deploymentIds = deploymentIds;
		}
		return this;
	  }

	  public virtual ProcessInstanceQuery superProcessInstanceId(string superProcessInstanceId)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.superProcessInstanceId_Renamed = superProcessInstanceId;
		}
		else
		{
		  this.superProcessInstanceId_Renamed = superProcessInstanceId;
		}
		return this;
	  }

	  public virtual ProcessInstanceQuery subProcessInstanceId(string subProcessInstanceId)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.subProcessInstanceId_Renamed = subProcessInstanceId;
		}
		else
		{
		  this.subProcessInstanceId_Renamed = subProcessInstanceId;
		}
		return this;
	  }

	  public virtual ProcessInstanceQuery excludeSubprocesses(bool excludeSubprocesses)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.excludeSubprocesses_Renamed = excludeSubprocesses;
		}
		else
		{
		  this.excludeSubprocesses_Renamed = excludeSubprocesses;
		}
		return this;
	  }

	  public virtual ProcessInstanceQuery involvedUser(string involvedUser)
	  {
		if (involvedUser == null)
		{
		  throw new ActivitiIllegalArgumentException("Involved user is null");
		}

		if (inOrStatement)
		{
		  this.currentOrQueryObject.involvedUser_Renamed = involvedUser;
		}
		else
		{
		  this.involvedUser_Renamed = involvedUser;
		}
		return this;
	  }

	  public virtual ProcessInstanceQuery active()
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.suspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		}
		else
		{
		  this.suspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		}
		return this;
	  }

	  public virtual ProcessInstanceQuery suspended()
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.suspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		}
		else
		{
		  this.suspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		}
		return this;
	  }

	  public virtual ProcessInstanceQuery includeProcessVariables()
	  {
		this.includeProcessVariables_Renamed = true;
		return this;
	  }

	  public virtual ProcessInstanceQuery limitProcessInstanceVariables(int? processInstanceVariablesLimit)
	  {
		this.processInstanceVariablesLimit = processInstanceVariablesLimit;
		return this;
	  }

	  public virtual int? ProcessInstanceVariablesLimit
	  {
		  get
		  {
			return processInstanceVariablesLimit;
		  }
	  }

	  public virtual ProcessInstanceQuery withJobException()
	  {
		this.withJobException_Renamed = true;
		return this;
	  }

	  public override ProcessInstanceQuery processInstanceName(string name)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.name = name;
		}
		else
		{
		  this.name = name;
		}
		return this;
	  }

	  public override ProcessInstanceQuery processInstanceNameLike(string nameLike)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.nameLike = nameLike;
		}
		else
		{
		  this.nameLike = nameLike;
		}
		return this;
	  }

	  public virtual ProcessInstanceQuery processInstanceNameLikeIgnoreCase(string nameLikeIgnoreCase)
	  {
		  if (inOrStatement)
		  {
		  this.currentOrQueryObject.nameLikeIgnoreCase = nameLikeIgnoreCase.ToLower();
		  }
		else
		{
		  this.nameLikeIgnoreCase = nameLikeIgnoreCase.ToLower();
		}
		return this;
	  }

	  public virtual ProcessInstanceQuery or()
	  {
		if (inOrStatement)
		{
		  throw new ActivitiException("the query is already in an or statement");
		}

		inOrStatement = true;
		currentOrQueryObject = new ProcessInstanceQueryImpl();
		orQueryObjects.Add(currentOrQueryObject);
		return this;
	  }

	  public virtual ProcessInstanceQuery endOr()
	  {
		if (!inOrStatement)
		{
		  throw new ActivitiException("endOr() can only be called after calling or()");
		}

		inOrStatement = false;
		currentOrQueryObject = null;
		return this;
	  }

	  public override ProcessInstanceQuery variableValueEquals(string variableName, object variableValue)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueEquals(variableName, variableValue, false);
		  return this;
		}
		else
		{
		  return variableValueEquals(variableName, variableValue, false);
		}
	  }

	  public override ProcessInstanceQuery variableValueNotEquals(string variableName, object variableValue)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueNotEquals(variableName, variableValue, false);
		  return this;
		}
		else
		{
		  return variableValueNotEquals(variableName, variableValue, false);
		}
	  }

	  public override ProcessInstanceQuery variableValueEquals(object variableValue)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueEquals(variableValue, false);
		  return this;
		}
		else
		{
		  return variableValueEquals(variableValue, false);
		}
	  }

	  public override ProcessInstanceQuery variableValueEqualsIgnoreCase(string name, string value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueEqualsIgnoreCase(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueEqualsIgnoreCase(name, value, false);
		}
	  }

	  public override ProcessInstanceQuery variableValueNotEqualsIgnoreCase(string name, string value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueNotEqualsIgnoreCase(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueNotEqualsIgnoreCase(name, value, false);
		}
	  }

	  public override ProcessInstanceQuery variableValueGreaterThan(string name, object value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueGreaterThan(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueGreaterThan(name, value, false);
		}
	  }

	  public override ProcessInstanceQuery variableValueGreaterThanOrEqual(string name, object value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueGreaterThanOrEqual(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueGreaterThanOrEqual(name, value, false);
		}
	  }

	  public override ProcessInstanceQuery variableValueLessThan(string name, object value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueLessThan(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueLessThan(name, value, false);
		}
	  }

	  public override ProcessInstanceQuery variableValueLessThanOrEqual(string name, object value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueLessThanOrEqual(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueLessThanOrEqual(name, value, false);
		}
	  }

	  public override ProcessInstanceQuery variableValueLike(string name, string value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueLike(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueLike(name, value, false);
		}
	  }

	  public override ProcessInstanceQuery variableValueLikeIgnoreCase(string name, string value)
	  {
		if (inOrStatement)
		{
			currentOrQueryObject.variableValueLikeIgnoreCase(name, value, false);
			return this;
		}
		else
		{
			return variableValueLikeIgnoreCase(name, value, false);
		}
	  }

	  public virtual ProcessInstanceQuery locale(string locale)
	  {
		this.locale_Renamed = locale;
		return this;
	  }

	  public virtual ProcessInstanceQuery withLocalizationFallback()
	  {
		withLocalizationFallback_Renamed = true;
		return this;
	  }

	  public virtual ProcessInstanceQuery orderByProcessInstanceId()
	  {
		this.orderProperty = ProcessInstanceQueryProperty.PROCESS_INSTANCE_ID;
		return this;
	  }

	  public virtual ProcessInstanceQuery orderByProcessDefinitionId()
	  {
		this.orderProperty = ProcessInstanceQueryProperty.PROCESS_DEFINITION_ID;
		return this;
	  }

	  public virtual ProcessInstanceQuery orderByProcessDefinitionKey()
	  {
		this.orderProperty = ProcessInstanceQueryProperty.PROCESS_DEFINITION_KEY;
		return this;
	  }

	  public virtual ProcessInstanceQuery orderByTenantId()
	  {
		this.orderProperty = ProcessInstanceQueryProperty.TENANT_ID;
		return this;
	  }

	  public virtual string MssqlOrDB2OrderBy
	  {
		  get
		  {
			string specialOrderBy = base.OrderBy;
			if (specialOrderBy != null && specialOrderBy.Length > 0)
			{
			  specialOrderBy = specialOrderBy.Replace("RES.", "TEMPRES_");
			  specialOrderBy = specialOrderBy.Replace("ProcessDefinitionKey", "TEMPP_KEY_");
			  specialOrderBy = specialOrderBy.Replace("ProcessDefinitionId", "TEMPP_ID_");
			}
			return specialOrderBy;
		  }
	  }

	  //results /////////////////////////////////////////////////////////////////

	  public virtual long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.ExecutionEntityManager.findProcessInstanceCountByQueryCriteria(this);
	  }

	  public virtual IList<ProcessInstance> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		IList<ProcessInstance> processInstances = null;
		if (includeProcessVariables_Renamed)
		{
		  processInstances = commandContext.ExecutionEntityManager.findProcessInstanceAndVariablesByQueryCriteria(this);
		}
		else
		{
		  processInstances = commandContext.ExecutionEntityManager.findProcessInstanceByQueryCriteria(this);
		}

		foreach (ProcessInstance processInstance in processInstances)
		{
		  localize(processInstance);
		}

		return processInstances;
	  }

	  protected internal override void ensureVariablesInitialized()
	  {
		base.ensureVariablesInitialized();

		foreach (ProcessInstanceQueryImpl orQueryObject in orQueryObjects)
		{
		  orQueryObject.ensureVariablesInitialized();
		}
	  }

	  protected internal virtual void localize(ProcessInstance processInstance)
	  {
		ExecutionEntity processInstanceExecution = (ExecutionEntity) processInstance;
		processInstanceExecution.LocalizedName = null;
		processInstanceExecution.LocalizedDescription = null;

		if (locale_Renamed != null)
		{
		  string processDefinitionId = processInstanceExecution.ProcessDefinitionId;
		  if (processDefinitionId != null)
		  {
			ObjectNode languageNode = Context.getLocalizationElementProperties(locale_Renamed, processInstanceExecution.ProcessDefinitionKey, processDefinitionId, withLocalizationFallback_Renamed);
			if (languageNode != null)
			{
			  JsonNode languageNameNode = languageNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_NAME);
			  if (languageNameNode != null && languageNameNode.Null == false)
			  {
				processInstanceExecution.LocalizedName = languageNameNode.asText();
			  }

			  JsonNode languageDescriptionNode = languageNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_DESCRIPTION);
			  if (languageDescriptionNode != null && languageDescriptionNode.Null == false)
			  {
				processInstanceExecution.LocalizedDescription = languageDescriptionNode.asText();
			  }
			}
		  }
		}
	  }

	  //getters /////////////////////////////////////////////////////////////////

	  public virtual bool OnlyProcessInstances
	  {
		  get
		  {
			return true; // See dynamic query in runtime.mapping.xml
		  }
	  }
	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return executionId;
		  }
	  }
	  public virtual Set<string> ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds_Renamed;
		  }
	  }
	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
	  }
	  public virtual bool IncludeChildExecutionsWithBusinessKeyQuery
	  {
		  get
		  {
			return includeChildExecutionsWithBusinessKeyQuery;
		  }
	  }
	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Renamed;
		  }
	  }
	  public virtual Set<string> ProcessDefinitionIds
	  {
		  get
		  {
			return processDefinitionIds_Renamed;
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
	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Renamed;
		  }
	  }
	  public virtual Set<string> ProcessDefinitionKeys
	  {
		  get
		  {
			return processDefinitionKeys_Renamed;
		  }
	  }
	  public virtual string ActivityId
	  {
		  get
		  {
			return null; // Unused, see dynamic query
		  }
	  }
	  public virtual string SuperProcessInstanceId
	  {
		  get
		  {
			return superProcessInstanceId_Renamed;
		  }
	  }
	  public virtual string SubProcessInstanceId
	  {
		  get
		  {
			return subProcessInstanceId_Renamed;
		  }
	  }
	  public virtual bool ExcludeSubprocesses
	  {
		  get
		  {
			return excludeSubprocesses_Renamed;
		  }
	  }
	  public virtual string InvolvedUser
	  {
		  get
		  {
			return involvedUser_Renamed;
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



	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
	  }

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId_Renamed;
		  }
	  }

	  public virtual IList<string> DeploymentIds
	  {
		  get
		  {
			return deploymentIds;
		  }
	  }

	  public virtual bool IncludeProcessVariables
	  {
		  get
		  {
			return includeProcessVariables_Renamed;
		  }
	  }

	  public virtual bool iswithException()
	  {
		return withJobException_Renamed;
	  }

	  public virtual string NameLikeIgnoreCase
	  {
		  get
		  {
			  return nameLikeIgnoreCase;
		  }
	  }

	  public virtual IList<ProcessInstanceQueryImpl> OrQueryObjects
	  {
		  get
		  {
			return orQueryObjects;
		  }
	  }

	  /// <summary>
	  /// Method needed for ibatis because of re-use of query-xml for executions. ExecutionQuery contains
	  /// a parentId property.
	  /// </summary>
	  public virtual string ParentId
	  {
		  get
		  {
			return null;
		  }
	  }
	}

}