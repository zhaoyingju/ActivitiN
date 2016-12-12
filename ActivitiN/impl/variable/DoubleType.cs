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
	/// @author Tom Baeyens
	/// </summary>
	public class DoubleType : VariableType
	{

	  private const long serialVersionUID = 1L;

	  public virtual string TypeName
	  {
		  get
		  {
			return "double";
		  }
	  }

	  public virtual bool Cachable
	  {
		  get
		  {
			return true;
		  }
	  }

	  public virtual object getValue(ValueFields valueFields)
	  {
		return valueFields.DoubleValue;
	  }

	  public virtual void setValue(object value, ValueFields valueFields)
	  {
		valueFields.DoubleValue = (double?) value;
	  }

	  public virtual bool isAbleToStore(object value)
	  {
		if (value == null)
		{
		  return true;
		}
		return value.GetType().IsSubclassOf(typeof(double?));
	  }
	}

}