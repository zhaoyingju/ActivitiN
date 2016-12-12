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
namespace org.activiti.engine.impl.variable
{


	/// <summary>
	/// Custom object type
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public class CustomObjectType : VariableType
	{

	  protected internal string typeName;
	  protected internal Type theClass;

	  public CustomObjectType(string typeName, Type theClass)
	  {
		this.theClass = theClass;
		this.typeName = typeName;
	  }

	  public virtual string TypeName
	  {
		  get
		  {
			return this.typeName;
		  }
	  }

	  public virtual object getValue(ValueFields valueFields)
	  {
		return valueFields.CachedValue;
	  }

	  public virtual bool isAbleToStore(object value)
	  {
		if (value == null)
		{
		  return true;
		}
		return this.theClass.IsAssignableFrom(value.GetType());
	  }

	  public virtual bool Cachable
	  {
		  get
		  {
			return true;
		  }
	  }

	  public virtual void setValue(object value, ValueFields valueFields)
	  {
		valueFields.CachedValue = value;
	  }
	}

}