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
	using Model = org.activiti.engine.repository.Model;
	using ModelQuery = org.activiti.engine.repository.ModelQuery;


	/// <summary>
	/// @author Tijs Rademakers
	/// @author Joram Barrez
	/// </summary>
	public class ModelQueryImpl : AbstractQuery<ModelQuery, Model>, ModelQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string id;
	  protected internal string category;
	  protected internal string categoryLike;
	  protected internal string categoryNotEquals;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string key;
	  protected internal int? version;
	  protected internal bool latest;
	  protected internal string deploymentId_Renamed;
	  protected internal bool notDeployed_Renamed;
	  protected internal bool deployed_Renamed;
	  protected internal string tenantId;
	  protected internal string tenantIdLike;
	  protected internal bool withoutTenantId;

	  public ModelQueryImpl()
	  {
	  }

	  public ModelQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public ModelQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual ModelQueryImpl modelId(string modelId)
	  {
		this.id = modelId;
		return this;
	  }

	  public virtual ModelQueryImpl modelCategory(string category)
	  {
		if (category == null)
		{
		  throw new ActivitiIllegalArgumentException("category is null");
		}
		this.category = category;
		return this;
	  }

	  public virtual ModelQueryImpl modelCategoryLike(string categoryLike)
	  {
		if (categoryLike == null)
		{
		  throw new ActivitiIllegalArgumentException("categoryLike is null");
		}
		this.categoryLike = categoryLike;
		return this;
	  }

	  public virtual ModelQueryImpl modelCategoryNotEquals(string categoryNotEquals)
	  {
		if (categoryNotEquals == null)
		{
		  throw new ActivitiIllegalArgumentException("categoryNotEquals is null");
		}
		this.categoryNotEquals = categoryNotEquals;
		return this;
	  }

	  public virtual ModelQueryImpl modelName(string name)
	  {
		if (name == null)
		{
		  throw new ActivitiIllegalArgumentException("name is null");
		}
		this.name = name;
		return this;
	  }

	  public virtual ModelQueryImpl modelNameLike(string nameLike)
	  {
		if (nameLike == null)
		{
		  throw new ActivitiIllegalArgumentException("nameLike is null");
		}
		this.nameLike = nameLike;
		return this;
	  }

	  public virtual ModelQuery modelKey(string key)
	  {
		if (key == null)
		{
		  throw new ActivitiIllegalArgumentException("key is null");
		}
		this.key = key;
		return this;
	  }

	  public virtual ModelQueryImpl modelVersion(int? version)
	  {
		if (version == null)
		{
		  throw new ActivitiIllegalArgumentException("version is null");
		}
		else if (version <= 0)
		{
		  throw new ActivitiIllegalArgumentException("version must be positive");
		}
		this.version = version;
		return this;
	  }

	  public virtual ModelQuery latestVersion()
	  {
		this.latest = true;
		return this;
	  }

	  public virtual ModelQuery deploymentId(string deploymentId)
	  {
		if (deploymentId == null)
		{
		  throw new ActivitiIllegalArgumentException("DeploymentId is null");
		}
		this.deploymentId_Renamed = deploymentId;
		return this;
	  }

	  public virtual ModelQuery notDeployed()
	  {
		if (deployed_Renamed == true)
		{
		  throw new ActivitiIllegalArgumentException("Invalid usage: cannot use deployed() and notDeployed() in the same query");
		}
		this.notDeployed_Renamed = true;
		return this;
	  }

	  public virtual ModelQuery deployed()
	  {
		if (notDeployed_Renamed == true)
		{
		  throw new ActivitiIllegalArgumentException("Invalid usage: cannot use deployed() and notDeployed() in the same query");
		}
		this.deployed_Renamed = true;
		return this;
	  }

	  public virtual ModelQuery modelTenantId(string tenantId)
	  {
		  if (tenantId == null)
		  {
			  throw new ActivitiIllegalArgumentException("Model tenant id is null");
		  }
		  this.tenantId = tenantId;
		  return this;
	  }

	  public virtual ModelQuery modelTenantIdLike(string tenantIdLike)
	  {
		  if (tenantIdLike == null)
		  {
			  throw new ActivitiIllegalArgumentException("Model tenant id is null");
		  }
		  this.tenantIdLike = tenantIdLike;
		  return this;
	  }

	  public virtual ModelQuery modelWithoutTenantId()
	  {
		  this.withoutTenantId = true;
		  return this;
	  }

	  //sorting ////////////////////////////////////////////

	  public virtual ModelQuery orderByModelCategory()
	  {
		return orderBy(ModelQueryProperty.MODEL_CATEGORY);
	  }

	  public virtual ModelQuery orderByModelId()
	  {
		return orderBy(ModelQueryProperty.MODEL_ID);
	  }

	  public virtual ModelQuery orderByModelKey()
	  {
		return orderBy(ModelQueryProperty.MODEL_KEY);
	  }

	  public virtual ModelQuery orderByModelVersion()
	  {
		return orderBy(ModelQueryProperty.MODEL_VERSION);
	  }

	  public virtual ModelQuery orderByModelName()
	  {
		return orderBy(ModelQueryProperty.MODEL_NAME);
	  }

	  public virtual ModelQuery orderByCreateTime()
	  {
		return orderBy(ModelQueryProperty.MODEL_CREATE_TIME);
	  }

	  public virtual ModelQuery orderByLastUpdateTime()
	  {
		return orderBy(ModelQueryProperty.MODEL_LAST_UPDATE_TIME);
	  }

	  public virtual ModelQuery orderByTenantId()
	  {
		  return orderBy(ModelQueryProperty.MODEL_TENANT_ID);
	  }

	  //results ////////////////////////////////////////////

	  public virtual long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.ModelEntityManager.findModelCountByQueryCriteria(this);
	  }

	  public virtual IList<Model> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.ModelEntityManager.findModelsByQueryCriteria(this, page);
	  }

	  //getters ////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
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
	  public virtual int? Version
	  {
		  get
		  {
			return version;
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
	  public virtual string CategoryNotEquals
	  {
		  get
		  {
			return categoryNotEquals;
		  }
	  }
	}

}