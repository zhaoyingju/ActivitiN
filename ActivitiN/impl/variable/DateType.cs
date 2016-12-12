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
	/// @author Tom Baeyens
	/// </summary>
	public class DateType : VariableType
	{

	  public virtual string TypeName
	  {
		  get
		  {
			return "date";
		  }
	  }

	  public virtual bool Cachable
	  {
		  get
		  {
			return true;
		  }
	  }

	  public virtual bool isAbleToStore(object value)
	  {
		if (value == null)
		{
		  return true;
		}
		return value.GetType().IsSubclassOf(typeof(DateTime));
	  }

	  public virtual object getValue(ValueFields valueFields)
	  {
		long? longValue = valueFields.LongValue;
		if (longValue != null)
		{
		  return new DateTime(longValue);
		}
		return null;
	  }

	  public virtual void setValue(object value, ValueFields valueFields)
	  {
		if (value != null)
		{
		  valueFields.LongValue = ((DateTime)value).Ticks;
		}
		else
		{
		  valueFields.LongValue = null;
		}
	  }
	}

}