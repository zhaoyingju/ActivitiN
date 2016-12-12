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

namespace org.activiti.engine.impl.form
{

	using AbstractFormType = org.activiti.engine.form.AbstractFormType;



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class LongFormType : AbstractFormType
	{

	  private const long serialVersionUID = 1L;

	  public override string Name
	  {
		  get
		  {
			return "long";
		  }
	  }

	  public virtual string MimeType
	  {
		  get
		  {
			return "plain/text";
		  }
	  }

	  public override object convertFormValueToModelValue(string propertyValue)
	  {
		if (propertyValue == null || "".Equals(propertyValue))
		{
		  return null;
		}
		return Convert.ToInt64(propertyValue);
	  }

	  public override string convertModelValueToFormValue(object modelValue)
	  {
		if (modelValue == null)
		{
		  return null;
		}
		return modelValue.ToString();
	  }
	}

}