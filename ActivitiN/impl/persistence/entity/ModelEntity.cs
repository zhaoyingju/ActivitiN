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


	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using Model = org.activiti.engine.repository.Model;


	/// <summary>
	/// @author Tijs Rademakers
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class ModelEntity : Model, HasRevision, PersistentObject
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision = 1;
	  protected internal string name;
	  protected internal string key;
	  protected internal string category;
	  protected internal DateTime createTime;
	  protected internal DateTime lastUpdateTime;
	  protected internal int? version = 1;
	  protected internal string metaInfo;
	  protected internal string deploymentId;
	  protected internal string editorSourceValueId;
	  protected internal string editorSourceExtraValueId;
	  protected internal string tenantId = ProcessEngineConfiguration.NO_TENANT_ID;

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["name"] = this.name;
			persistentState["key"] = key;
			persistentState["category"] = this.category;
			persistentState["createTime"] = this.createTime;
			persistentState["lastUpdateTime"] = lastUpdateTime;
			persistentState["version"] = this.version;
			persistentState["metaInfo"] = this.metaInfo;
			persistentState["deploymentId"] = deploymentId;
			persistentState["editorSourceValueId"] = this.editorSourceValueId;
			persistentState["editorSourceExtraValueId"] = this.editorSourceExtraValueId;
			return persistentState;
		  }
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


	  public virtual string Key
	  {
		  get
		  {
			return key;
		  }
		  set
		  {
			this.key = value;
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


	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime;
		  }
		  set
		  {
			this.createTime = value;
		  }
	  }


	  public virtual DateTime LastUpdateTime
	  {
		  get
		  {
			return lastUpdateTime;
		  }
		  set
		  {
			this.lastUpdateTime = value;
		  }
	  }


	  public virtual int? Version
	  {
		  get
		  {
			return version;
		  }
		  set
		  {
			this.version = value;
		  }
	  }


	  public virtual string MetaInfo
	  {
		  get
		  {
			return metaInfo;
		  }
		  set
		  {
			this.metaInfo = value;
		  }
	  }


	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
		  }
	  }


	  public virtual string EditorSourceValueId
	  {
		  get
		  {
			return editorSourceValueId;
		  }
		  set
		  {
			this.editorSourceValueId = value;
		  }
	  }


	  public virtual string EditorSourceExtraValueId
	  {
		  get
		  {
			return editorSourceExtraValueId;
		  }
		  set
		  {
			this.editorSourceExtraValueId = value;
		  }
	  }


	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
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


		public virtual bool hasEditorSource()
		{
		  return this.editorSourceValueId != null;
		}

		public virtual bool hasEditorSourceExtra()
		{
		  return this.editorSourceExtraValueId != null;
		}

	}

}