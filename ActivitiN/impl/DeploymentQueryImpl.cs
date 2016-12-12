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


	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using Deployment = org.activiti.engine.repository.Deployment;
	using DeploymentQuery = org.activiti.engine.repository.DeploymentQuery;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class DeploymentQueryImpl : AbstractQuery<DeploymentQuery, Deployment>, DeploymentQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string deploymentId_Renamed;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string category;
	  protected internal string categoryNotEquals;
	  protected internal string tenantId;
	  protected internal string tenantIdLike;
	  protected internal bool withoutTenantId;
	  protected internal string processDefinitionKey_Renamed;
	  protected internal string processDefinitionKeyLike_Renamed;

	  public DeploymentQueryImpl()
	  {
	  }

	  public DeploymentQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public DeploymentQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual DeploymentQueryImpl deploymentId(string deploymentId)
	  {
		if (deploymentId == null)
		{
		  throw new ActivitiIllegalArgumentException("Deployment id is null");
		}
		this.deploymentId_Renamed = deploymentId;
		return this;
	  }

	  public virtual DeploymentQueryImpl deploymentName(string deploymentName)
	  {
		if (deploymentName == null)
		{
		  throw new ActivitiIllegalArgumentException("deploymentName is null");
		}
		this.name = deploymentName;
		return this;
	  }

	  public virtual DeploymentQueryImpl deploymentNameLike(string nameLike)
	  {
		if (nameLike == null)
		{
		  throw new ActivitiIllegalArgumentException("deploymentNameLike is null");
		}
		this.nameLike = nameLike;
		return this;
	  }

	  public virtual DeploymentQueryImpl deploymentCategory(string deploymentCategory)
	  {
		if (deploymentCategory == null)
		{
		  throw new ActivitiIllegalArgumentException("deploymentCategory is null");
		}
		this.category = deploymentCategory;
		return this;
	  }

	  public virtual DeploymentQueryImpl deploymentCategoryNotEquals(string deploymentCategoryNotEquals)
	  {
		if (deploymentCategoryNotEquals == null)
		{
		  throw new ActivitiIllegalArgumentException("deploymentCategoryExclude is null");
		}
		this.categoryNotEquals = deploymentCategoryNotEquals;
		return this;
	  }

	  public virtual DeploymentQueryImpl deploymentTenantId(string tenantId)
	  {
		  if (tenantId == null)
		  {
			  throw new ActivitiIllegalArgumentException("deploymentTenantId is null");
		  }
		  this.tenantId = tenantId;
		  return this;
	  }

	  public virtual DeploymentQueryImpl deploymentTenantIdLike(string tenantIdLike)
	  {
		  if (tenantIdLike == null)
		  {
			  throw new ActivitiIllegalArgumentException("deploymentTenantIdLike is null");
		  }
		  this.tenantIdLike = tenantIdLike;
		  return this;
	  }

	  public virtual DeploymentQueryImpl deploymentWithoutTenantId()
	  {
		  this.withoutTenantId = true;
		  return this;
	  }

	  public virtual DeploymentQueryImpl processDefinitionKey(string key)
	  {
		  if (key == null)
		  {
			  throw new ActivitiIllegalArgumentException("key is null");
		  }
		  this.processDefinitionKey_Renamed = key;
		  return this;
	  }

	  public virtual DeploymentQueryImpl processDefinitionKeyLike(string keyLike)
	  {
		if (keyLike == null)
		{
		  throw new ActivitiIllegalArgumentException("keyLike is null");
		}
		this.processDefinitionKeyLike_Renamed = keyLike;
		return this;
	  }

	  //sorting ////////////////////////////////////////////////////////


	  public virtual DeploymentQuery orderByDeploymentId()
	  {
		return orderBy(DeploymentQueryProperty.DEPLOYMENT_ID);
	  }

	  public virtual DeploymentQuery orderByDeploymenTime()
	  {
		return orderBy(DeploymentQueryProperty.DEPLOY_TIME);
	  }

	  public virtual DeploymentQuery orderByDeploymentName()
	  {
		return orderBy(DeploymentQueryProperty.DEPLOYMENT_NAME);
	  }

	  public virtual DeploymentQuery orderByTenantId()
	  {
		  return orderBy(DeploymentQueryProperty.DEPLOYMENT_TENANT_ID);
	  }

	  //results ////////////////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.DeploymentEntityManager.findDeploymentCountByQueryCriteria(this);
	  }

	  public override IList<Deployment> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.DeploymentEntityManager.findDeploymentsByQueryCriteria(this, page);
	  }

	  //getters ////////////////////////////////////////////////////////

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId_Renamed;
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

	  public virtual string Category
	  {
		  get
		  {
			return category;
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

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Renamed;
		  }
	  }

	  public virtual string ProcessDefinitionKeyLike
	  {
		  get
		  {
			return processDefinitionKeyLike_Renamed;
		  }
	  }
	}

}