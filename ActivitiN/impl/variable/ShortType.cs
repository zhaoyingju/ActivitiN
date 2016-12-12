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
	/// @author Joram Barrez
	/// </summary>
	public class ShortType : VariableType
	{

	  private const long serialVersionUID = 1L;

	  public virtual string TypeName
	  {
		  get
		  {
			return "short";
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
		if (valueFields.LongValue != null)
		{
		  return new short?((short)valueFields.LongValue);
		}
		return null;
	  }

	  public virtual void setValue(object value, ValueFields valueFields)
	  {
		if (value != null)
		{
		  valueFields.LongValue = (long)((short?) value);
		  valueFields.TextValue = value.ToString();
		}
		else
		{
		  valueFields.LongValue = null;
		  valueFields.TextValue = null;
		}
	  }

	  public virtual bool isAbleToStore(object value)
	  {
		if (value == null)
		{
		  return true;
		}
		return value.GetType().IsSubclassOf(typeof(short?)) || value.GetType().IsSubclassOf(typeof(short));
	  }
	}

}