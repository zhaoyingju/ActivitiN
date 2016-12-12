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

namespace org.activiti.engine.impl.form
{

	using AbstractFormType = org.activiti.engine.form.AbstractFormType;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class EnumFormType : AbstractFormType
	{

	  private const long serialVersionUID = 1L;

	  protected internal IDictionary<string, string> values;

	  public EnumFormType(IDictionary<string, string> values)
	  {
		this.values = values;
	  }

	  public override string Name
	  {
		  get
		  {
			return "enum";
		  }
	  }

	  public override object getInformation(string key)
	  {
		if (key.Equals("values"))
		{
		  return values;
		}
		return null;
	  }

	  public override object convertFormValueToModelValue(string propertyValue)
	  {
		validateValue(propertyValue);
		return propertyValue;
	  }

	  public override string convertModelValueToFormValue(object modelValue)
	  {
		if (modelValue != null)
		{
		  if (!(modelValue is string))
		  {
			throw new ActivitiIllegalArgumentException("Model value should be a String");
		  }
		  validateValue((string) modelValue);
		}
		return (string) modelValue;
	  }

	  protected internal virtual void validateValue(string value)
	  {
		if (value != null)
		{
		  if (values != null && !values.ContainsKey(value))
		  {
			throw new ActivitiIllegalArgumentException("Invalid value for enum form property: " + value);
		  }
		}
	  }

	}

}