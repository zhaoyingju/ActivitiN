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

	using ItemInstance = org.activiti.engine.impl.bpmn.data.ItemInstance;
	using StructureInstance = org.activiti.engine.impl.bpmn.data.StructureInstance;


	/// <summary>
	/// An instance of a <seealso cref="MessageDefinition"/>
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public class MessageInstance
	{

	  protected internal MessageDefinition message;

	  protected internal ItemInstance item;

	  public MessageInstance(MessageDefinition message, ItemInstance item)
	  {
		this.message = message;
		this.item = item;
	  }

	  public virtual StructureInstance StructureInstance
	  {
		  get
		  {
			return this.item.StructureInstance;
		  }
	  }

	  public virtual MessageDefinition Message
	  {
		  get
		  {
			return this.message;
		  }
	  }
	}

}