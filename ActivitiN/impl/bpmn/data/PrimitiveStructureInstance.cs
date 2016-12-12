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
namespace org.activiti.engine.impl.bpmn.data
{

	/// <summary>
	/// An instance of <seealso cref="PrimitiveStructureDefinition"/>
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public class PrimitiveStructureInstance : StructureInstance
	{

	  protected internal object primitive;

	  protected internal PrimitiveStructureDefinition definition;

	  public PrimitiveStructureInstance(PrimitiveStructureDefinition definition) : this(definition, null)
	  {
	  }

	  public PrimitiveStructureInstance(PrimitiveStructureDefinition definition, object primitive)
	  {
		this.definition = definition;
		this.primitive = primitive;
	  }

	  public virtual object Primitive
	  {
		  get
		  {
			return this.primitive;
		  }
	  }

	  public virtual object[] toArray()
	  {
		return new object[] {this.primitive};
	  }

	  public virtual void loadFrom(object[] array)
	  {
		for (int i = 0; i < array.Length; i++)
		{
		  object @object = array[i];
		  if (this.definition.PrimitiveClass.IsInstanceOfType(@object))
		  {
			this.primitive = @object;
			return;
		  }
		}
	  }
	}

}