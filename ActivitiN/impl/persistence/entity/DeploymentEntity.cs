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

namespace org.activiti.engine.impl.persistence.entity
{


	using Context = org.activiti.engine.impl.context.Context;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using Deployment = org.activiti.engine.repository.Deployment;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class DeploymentEntity : Deployment, PersistentObject
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal string name;
	  protected internal string category;
	  protected internal string tenantId = ProcessEngineConfiguration.NO_TENANT_ID;
	  protected internal IDictionary<string, ResourceEntity> resources;
	  protected internal DateTime deploymentTime;
	  protected internal bool isNew;

	  /// <summary>
	  /// Will only be used during actual deployment to pass deployed artifacts (eg process definitions).
	  /// Will be null otherwise.
	  /// </summary>
	  protected internal IDictionary<Type, IList<object>> deployedArtifacts;

	  public virtual ResourceEntity getResource(string resourceName)
	  {
		return Resources[resourceName];
	  }

	  public virtual void addResource(ResourceEntity resource)
	  {
		if (resources == null)
		{
		  resources = new Dictionary<string, ResourceEntity>();
		}
		resources[resource.Name] = resource;
	  }

	  // lazy loading /////////////////////////////////////////////////////////////
	  public virtual IDictionary<string, ResourceEntity> Resources
	  {
		  get
		  {
			if (resources == null && id != null)
			{
			  IList<ResourceEntity> resourcesList = Context.CommandContext.ResourceEntityManager.findResourcesByDeploymentId(id);
			  resources = new Dictionary<string, ResourceEntity>();
			  foreach (ResourceEntity resource in resourcesList)
			  {
				resources[resource.Name] = resource;
			  }
			}
			return resources;
		  }
		  set
		  {
			this.resources = value;
		  }
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["category"] = this.category;
			persistentState["tenantId"] = tenantId;
			return persistentState;
		  }
	  }

	  // Deployed artifacts manipulation //////////////////////////////////////////
	  public virtual void addDeployedArtifact(object deployedArtifact)
	  {
		if (deployedArtifacts == null)
		{
		  deployedArtifacts = new Dictionary<Type, IList<object>>();
		}

		Type clazz = deployedArtifact.GetType();
		IList<object> artifacts = deployedArtifacts[clazz];
		if (artifacts == null)
		{
		  artifacts = new List<object>();
		  deployedArtifacts[clazz] = artifacts;
		}

		artifacts.Add(deployedArtifact);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> java.util.List<T> getDeployedArtifacts(Class clazz)
	  public virtual IList<T> getDeployedArtifacts<T>(Type clazz)
	  {
		return (IList<T>) deployedArtifacts[clazz];
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
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


	  public virtual string Category
	  {
		  get
		  {
			return category;
		  }
		  set
		  {
			this.category = value;
		  }
	  }


	  public virtual string TenantId
	  {
		  get
		  {
			  return tenantId;
		  }
		  set
		  {
			  this.tenantId = value;
		  }
	  }



	  public virtual DateTime DeploymentTime
	  {
		  get
		  {
			return deploymentTime;
		  }
		  set
		  {
			this.deploymentTime = value;
		  }
	  }


	  public virtual bool New
	  {
		  get
		  {
			return isNew;
		  }
		  set
		  {
			this.isNew = value;
		  }
	  }



	  // common methods  //////////////////////////////////////////////////////////

	  public override string ToString()
	  {
		return "DeploymentEntity[id=" + id + ", name=" + name + "]";
	  }

	}

}