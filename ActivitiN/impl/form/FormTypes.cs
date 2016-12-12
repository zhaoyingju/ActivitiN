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


	using FormProperty = org.activiti.bpmn.model.FormProperty;
	using FormValue = org.activiti.bpmn.model.FormValue;
	using AbstractFormType = org.activiti.engine.form.AbstractFormType;
	using StringUtils = org.apache.commons.lang3.StringUtils;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class FormTypes
	{

	  protected internal IDictionary<string, AbstractFormType> formTypes = new Dictionary<string, AbstractFormType>();

	  public virtual void addFormType(AbstractFormType formType)
	  {
		formTypes[formType.Name] = formType;
	  }

	  public virtual AbstractFormType parseFormPropertyType(FormProperty formProperty)
	  {
		AbstractFormType formType = null;

		if ("date".Equals(formProperty.Type) && StringUtils.isNotEmpty(formProperty.DatePattern))
		{
		  formType = new DateFormType(formProperty.DatePattern);

		}
		else if ("enum".Equals(formProperty.Type))
		{
		  // ACT-1023: Using linked hashmap to preserve the order in which the entries are defined
		  IDictionary<string, string> values = new LinkedHashMap<string, string>();
		  foreach (FormValue formValue in formProperty.FormValues)
		  {
			values[formValue.Id] = formValue.Name;
		  }
		  formType = new EnumFormType(values);

		}
		else if (StringUtils.isNotEmpty(formProperty.Type))
		{
		  formType = formTypes[formProperty.Type];
		  if (formType == null)
		  {
			throw new ActivitiIllegalArgumentException("unknown type '" + formProperty.Type + "' " + formProperty.Id);
		  }
		}
		return formType;
	  }
	}

}