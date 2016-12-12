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


	using Group = org.activiti.engine.identity.Group;
	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using SuspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.activiti.engine.repository.ProcessDefinitionQuery;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// @author Saeid Mirzaei
	/// </summary>
	public class ProcessDefinitionQueryImpl : AbstractQuery<ProcessDefinitionQuery, ProcessDefinition>, ProcessDefinitionQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string id;
	  protected internal Set<string> ids;
	  protected internal string category;
	  protected internal string categoryLike;
	  protected internal string categoryNotEquals;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string deploymentId_Renamed;
	  protected internal Set<string> deploymentIds_Renamed;
	  protected internal string key;
	  protected internal string keyLike;
	  protected internal string resourceName;
	  protected internal string resourceNameLike;
	  protected internal int? version;
	  protected internal int? versionGt;
	  protected internal int? versionGte;
	  protected internal int? versionLt;
	  protected internal int? versionLte;
	  protected internal bool latest = false;
	  protected internal SuspensionState suspensionState;
	  protected internal string authorizationUserId;
	  protected internal string procDefId;
	  protected internal string tenantId;
	  protected internal string tenantIdLike;
	  protected internal bool withoutTenantId;

	  protected internal string eventSubscriptionName;
	  protected internal string eventSubscriptionType;

	  public ProcessDefinitionQueryImpl()
	  {
	  }

	  public ProcessDefinitionQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public ProcessDefinitionQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionId(string processDefinitionId)
	  {
		this.id = processDefinitionId;
		return this;
	  }

	  public override ProcessDefinitionQuery processDefinitionIds(Set<string> processDefinitionIds)
	  {
		if (processDefinitionIds == null)
		{
		  throw new ActivitiIllegalArgumentException("processDefinitionIds is null");
		}
		if (processDefinitionIds.Empty)
		{
		  throw new ActivitiIllegalArgumentException("processDefinitionIds is empty");
		}
		  this.ids = processDefinitionIds;
		  return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionCategory(string category)
	  {
		if (category == null)
		{
		  throw new ActivitiIllegalArgumentException("category is null");
		}
		this.category = category;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionCategoryLike(string categoryLike)
	  {
		if (categoryLike == null)
		{
		  throw new ActivitiIllegalArgumentException("categoryLike is null");
		}
		this.categoryLike = categoryLike;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionCategoryNotEquals(string categoryNotEquals)
	  {
		if (categoryNotEquals == null)
		{
		  throw new ActivitiIllegalArgumentException("categoryNotEquals is null");
		}
		this.categoryNotEquals = categoryNotEquals;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionName(string name)
	  {
		if (name == null)
		{
		  throw new ActivitiIllegalArgumentException("name is null");
		}
		this.name = name;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionNameLike(string nameLike)
	  {
		if (nameLike == null)
		{
		  throw new ActivitiIllegalArgumentException("nameLike is null");
		}
		this.nameLike = nameLike;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl deploymentId(string deploymentId)
	  {
		if (deploymentId == null)
		{
		  throw new ActivitiIllegalArgumentException("id is null");
		}
		this.deploymentId_Renamed = deploymentId;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl deploymentIds(Set<string> deploymentIds)
	  {
		if (deploymentIds == null)
		{
		  throw new ActivitiIllegalArgumentException("deploymentIds is null");
		}
		if (deploymentIds.Empty)
		{
		  throw new ActivitiIllegalArgumentException("deploymentIds is empty");
		}
		this.deploymentIds_Renamed = deploymentIds;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionKey(string key)
	  {
		if (key == null)
		{
		  throw new ActivitiIllegalArgumentException("key is null");
		}
		this.key = key;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionKeyLike(string keyLike)
	  {
		if (keyLike == null)
		{
		  throw new ActivitiIllegalArgumentException("keyLike is null");
		}
		this.keyLike = keyLike;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionResourceName(string resourceName)
	  {
		if (resourceName == null)
		{
		  throw new ActivitiIllegalArgumentException("resourceName is null");
		}
		this.resourceName = resourceName;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionResourceNameLike(string resourceNameLike)
	  {
		if (resourceNameLike == null)
		{
		  throw new ActivitiIllegalArgumentException("resourceNameLike is null");
		}
		this.resourceNameLike = resourceNameLike;
		return this;
	  }

	  public virtual ProcessDefinitionQueryImpl processDefinitionVersion(int? version)
	  {
		checkVersion(version);
		this.version = version;
		return this;
	  }

	  public virtual ProcessDefinitionQuery processDefinitionVersionGreaterThan(int? processDefinitionVersion)
	  {
		checkVersion(processDefinitionVersion);
		this.versionGt = processDefinitionVersion;
		return this;
	  }

	  public virtual ProcessDefinitionQuery processDefinitionVersionGreaterThanOrEquals(int? processDefinitionVersion)
	  {
		checkVersion(processDefinitionVersion);
		this.versionGte = processDefinitionVersion;
		return this;
	  }

	  public virtual ProcessDefinitionQuery processDefinitionVersionLowerThan(int? processDefinitionVersion)
	  {
		checkVersion(processDefinitionVersion);
		this.versionLt = processDefinitionVersion;
		return this;
	  }

	  public virtual ProcessDefinitionQuery processDefinitionVersionLowerThanOrEquals(int? processDefinitionVersion)
	  {
		checkVersion(processDefinitionVersion);
		this.versionLte = processDefinitionVersion;
		return this;
	  }

	  protected internal virtual void checkVersion(int? version)
	  {
		if (version == null)
		{
		  throw new ActivitiIllegalArgumentException("version is null");
		}
		else if (version <= 0)
		{
		  throw new ActivitiIllegalArgumentException("version must be positive");
		}
	  }

	  public virtual ProcessDefinitionQueryImpl latestVersion()
	  {
		this.latest = true;
		return this;
	  }

	  public virtual ProcessDefinitionQuery active()
	  {
		this.suspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		return this;
	  }

	  public virtual ProcessDefinitionQuery suspended()
	  {
		this.suspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		return this;
	  }

	  public virtual ProcessDefinitionQuery processDefinitionTenantId(string tenantId)
	  {
		  if (tenantId == null)
		  {
			  throw new ActivitiIllegalArgumentException("processDefinition tenantId is null");
		  }
		  this.tenantId = tenantId;
		  return this;
	  }

	  public virtual ProcessDefinitionQuery processDefinitionTenantIdLike(string tenantIdLike)
	  {
		  if (tenantIdLike == null)
		  {
			  throw new ActivitiIllegalArgumentException("process definition tenantId is null");
		  }
		  this.tenantIdLike = tenantIdLike;
		  return this;
	  }

	  public virtual ProcessDefinitionQuery processDefinitionWithoutTenantId()
	  {
		  this.withoutTenantId = true;
		  return this;
	  }

	  public virtual ProcessDefinitionQuery messageEventSubscription(string messageName)
	  {
		return eventSubscription("message", messageName);
	  }

	  public virtual ProcessDefinitionQuery messageEventSubscriptionName(string messageName)
	  {
		return eventSubscription("message", messageName);
	  }

	  public virtual ProcessDefinitionQuery processDefinitionStarter(string procDefId)
	  {
		this.procDefId = procDefId;
		return this;
	  }

	  public virtual ProcessDefinitionQuery eventSubscription(string eventType, string eventName)
	  {
		if (eventName == null)
		{
		  throw new ActivitiIllegalArgumentException("event name is null");
		}
		if (eventType == null)
		{
		  throw new ActivitiException("event type is null");
		}
		this.eventSubscriptionType = eventType;
		this.eventSubscriptionName = eventName;
		return this;
	  }

	  public virtual IList<string> AuthorizationGroups
	  {
		  get
		  {
			// Simmilar behaviour as the TaskQuery.taskCandidateUser() which includes the groups the candidate
			// user is part of
			if (authorizationUserId != null)
			{
			  IList<Group> groups = Context.CommandContext.GroupIdentityManager.findGroupsByUser(authorizationUserId);
					IList<string> groupIds = new List<string>();
					foreach (Group group in groups)
					{
					  groupIds.Add(group.Id);
					}
					return groupIds;
			}
			return null;
		  }
	  }

	  //sorting ////////////////////////////////////////////

	  public virtual ProcessDefinitionQuery orderByDeploymentId()
	  {
		return orderBy(ProcessDefinitionQueryProperty.DEPLOYMENT_ID);
	  }

	  public virtual ProcessDefinitionQuery orderByProcessDefinitionKey()
	  {
		return orderBy(ProcessDefinitionQueryProperty.PROCESS_DEFINITION_KEY);
	  }

	  public virtual ProcessDefinitionQuery orderByProcessDefinitionCategory()
	  {
		return orderBy(ProcessDefinitionQueryProperty.PROCESS_DEFINITION_CATEGORY);
	  }

	  public virtual ProcessDefinitionQuery orderByProcessDefinitionId()
	  {
		return orderBy(ProcessDefinitionQueryProperty.PROCESS_DEFINITION_ID);
	  }

	  public virtual ProcessDefinitionQuery orderByProcessDefinitionVersion()
	  {
		return orderBy(ProcessDefinitionQueryProperty.PROCESS_DEFINITION_VERSION);
	  }

	  public virtual ProcessDefinitionQuery orderByProcessDefinitionName()
	  {
		return orderBy(ProcessDefinitionQueryProperty.PROCESS_DEFINITION_NAME);
	  }

	  public virtual ProcessDefinitionQuery orderByTenantId()
	  {
		return orderBy(ProcessDefinitionQueryProperty.PROCESS_DEFINITION_TENANT_ID);
	  }

	  //results ////////////////////////////////////////////

	  public virtual long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.ProcessDefinitionEntityManager.findProcessDefinitionCountByQueryCriteria(this);
	  }

	  public virtual IList<ProcessDefinition> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.ProcessDefinitionEntityManager.findProcessDefinitionsByQueryCriteria(this, page);
	  }

	  public virtual void checkQueryOk()
	  {
		base.checkQueryOk();
	  }

	  //getters ////////////////////////////////////////////

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId_Renamed;
		  }
	  }
	  public virtual Set<string> DeploymentIds
	  {
		  get
		  {
			return deploymentIds_Renamed;
		  }
	  }
	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }
	  public virtual Set<string> Ids
	  {
		  get
		  {
			return ids;
		  }
	  }
	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }
	  public virtual string NameLike
	  {
		  get
		  {
			return nameLike;
		  }
	  }
	  public virtual string Key
	  {
		  get
		  {
			return key;
		  }
	  }
	  public virtual string KeyLike
	  {
		  get
		  {
			return keyLike;
		  }
	  }
	  public virtual int? Version
	  {
		  get
		  {
			return version;
		  }
	  }
	  public virtual int? VersionGt
	  {
		  get
		  {
			return versionGt;
		  }
	  }
	  public virtual int? VersionGte
	  {
		  get
		  {
			return versionGte;
		  }
	  }
	  public virtual int? VersionLt
	  {
		  get
		  {
			return versionLt;
		  }
	  }
	  public virtual int? VersionLte
	  {
		  get
		  {
			return versionLte;
		  }
	  }
	  public virtual bool Latest
	  {
		  get
		  {
			return latest;
		  }
	  }
	  public virtual string Category
	  {
		  get
		  {
			return category;
		  }
	  }
	  public virtual string CategoryLike
	  {
		  get
		  {
			return categoryLike;
		  }
	  }
	  public virtual string ResourceName
	  {
		  get
		  {
			return resourceName;
		  }
	  }
	  public virtual string ResourceNameLike
	  {
		  get
		  {
			return resourceNameLike;
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
	  public virtual string CategoryNotEquals
	  {
		  get
		  {
			return categoryNotEquals;
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

		public virtual ProcessDefinitionQueryImpl startableByUser(string userId)
		{
		if (userId == null)
		{
		  throw new ActivitiIllegalArgumentException("userId is null");
		}
		this.authorizationUserId = userId;
		return this;
		}

	}

}