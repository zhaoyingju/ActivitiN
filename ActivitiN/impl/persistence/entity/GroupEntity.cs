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


	using Group = org.activiti.engine.identity.Group;
	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class GroupEntity : Group, PersistentObject, HasRevision
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;
	  protected internal string name;
	  protected internal string type;

	  public GroupEntity()
	  {
	  }

	  public GroupEntity(string id)
	  {
		this.id = id;
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["name"] = name;
			persistentState["type"] = type;
			return persistentState;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

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
	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
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
	}

}