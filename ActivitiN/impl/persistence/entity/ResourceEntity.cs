using System;

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

	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class ResourceEntity : PersistentObject
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal string name;
	  protected internal sbyte[] bytes;
	  protected internal string deploymentId;
	  protected internal bool generated = false;

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


	  public virtual sbyte[] Bytes
	  {
		  get
		  {
			return bytes;
		  }
		  set
		  {
			this.bytes = value;
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


	  public virtual object PersistentState
	  {
		  get
		  {
			return typeof(ResourceEntity);
		  }
	  }

	  public virtual bool Generated
	  {
		  set
		  {
			this.generated = value;
		  }
		  get
		  {
			return generated;
		  }
	  }


	  // common methods  //////////////////////////////////////////////////////////

	  public override string ToString()
	  {
		return "ResourceEntity[id=" + id + ", name=" + name + "]";
	  }
	}

}