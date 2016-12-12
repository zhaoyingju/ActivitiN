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

	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class PropertyEntity : PersistentObject, HasRevision
	{

	  private const long serialVersionUID = 1L;

	  internal string name;
	  internal int revision;
	  internal string value;

	  public PropertyEntity()
	  {
	  }

	  public PropertyEntity(string name, string value)
	  {
		this.name = name;
		this.value = value;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
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
	  public virtual string Value
	  {
		  get
		  {
			return value;
		  }
		  set
		  {
			this.value = value;
		  }
	  }

	  // persistent object methods ////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			throw new ActivitiException("only provided id generation allowed for properties");
		  }
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			return value;
		  }
	  }


	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  // common methods  //////////////////////////////////////////////////////////

	  public override string ToString()
	  {
		return "PropertyEntity[name=" + name + ", value=" + value + "]";
	  }

	}

}