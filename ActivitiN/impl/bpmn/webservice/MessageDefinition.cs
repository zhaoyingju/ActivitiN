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
namespace org.activiti.engine.impl.bpmn.webservice
{

	using ItemDefinition = org.activiti.engine.impl.bpmn.data.ItemDefinition;
	using StructureDefinition = org.activiti.engine.impl.bpmn.data.StructureDefinition;

	/// <summary>
	/// Implementation of the BPMN 2.0 'message'
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public class MessageDefinition
	{

	  protected internal string id;

	  protected internal ItemDefinition itemDefinition;

	  protected internal string name;

	  public MessageDefinition(string id, string name)
	  {
		this.id = id;
		this.name = name;
	  }

	  public virtual MessageInstance createInstance()
	  {
		return new MessageInstance(this, this.itemDefinition.createInstance());
	  }

	  public virtual ItemDefinition ItemDefinition
	  {
		  get
		  {
			return this.itemDefinition;
		  }
		  set
		  {
			this.itemDefinition = value;
		  }
	  }

	  public virtual StructureDefinition StructureDefinition
	  {
		  get
		  {
			return this.itemDefinition.StructureDefinition;
		  }
	  }


	  public virtual string Id
	  {
		  get
		  {
			return this.id;
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

	}

}