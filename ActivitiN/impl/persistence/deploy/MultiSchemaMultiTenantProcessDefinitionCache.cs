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
namespace org.activiti.engine.impl.persistence.deploy
{


	using TenantInfoHolder = org.activiti.engine.impl.cfg.multitenant.TenantInfoHolder;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;

	/// <summary>
	/// @author jbarrez
	/// </summary>
	public class MultiSchemaMultiTenantProcessDefinitionCache : DeploymentCache<ProcessDefinitionEntity>
	{

	  protected internal TenantInfoHolder tenantInfoHolder;
	  protected internal int cacheLimit;
	  protected internal IDictionary<string, DeploymentCache<ProcessDefinitionEntity>> caches = new Dictionary<string, DeploymentCache<ProcessDefinitionEntity>>();

	  public MultiSchemaMultiTenantProcessDefinitionCache(TenantInfoHolder tenantInfoHolder, int cacheLimit)
	  {
		this.tenantInfoHolder = tenantInfoHolder;
		this.cacheLimit = cacheLimit;
	  }

	  protected internal virtual DeploymentCache<ProcessDefinitionEntity> DeploymentCacheForCurrentTenant
	  {
		  get
		  {
			string currentTenantId = tenantInfoHolder.CurrentTenantId;
			DeploymentCache<ProcessDefinitionEntity> tenantDeploymentCache = caches[currentTenantId];
			if (tenantDeploymentCache == null)
			{
			  tenantDeploymentCache = createTenantDeploymentCache(currentTenantId);
			}
			return tenantDeploymentCache;
		  }
	  }

	  protected internal virtual DeploymentCache<ProcessDefinitionEntity> createTenantDeploymentCache(string currentTenantId)
	  {
		  lock (this)
		  {
			DeploymentCache<ProcessDefinitionEntity> tenantDeploymentCache = caches[currentTenantId];
			if (tenantDeploymentCache == null)
			{
			  tenantDeploymentCache = new DefaultDeploymentCache<ProcessDefinitionEntity>(cacheLimit);
			  caches[currentTenantId] = tenantDeploymentCache;
			}
			return tenantDeploymentCache;
		  }
	  }

	  public override ProcessDefinitionEntity get(string id)
	  {
		return DeploymentCacheForCurrentTenant.get(id);
	  }

	  public override void add(string id, ProcessDefinitionEntity @object)
	  {
		DeploymentCache<ProcessDefinitionEntity> tenantDeploymentCache = DeploymentCacheForCurrentTenant;
		tenantDeploymentCache.add(id, @object);
		Console.WriteLine("AAP");
	  }

	  public override void remove(string id)
	  {
		DeploymentCacheForCurrentTenant.remove(id);
	  }

	  public override void clear()
	  {
		DeploymentCacheForCurrentTenant.clear();
	  }

	}
}