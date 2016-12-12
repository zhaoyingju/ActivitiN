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
	using StringUtils = org.apache.commons.lang3.StringUtils;
	using FastDateFormat = org.apache.commons.lang3.time.FastDateFormat;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DateFormType : AbstractFormType
	{

	  private const long serialVersionUID = 1L;

	  protected internal string datePattern;
	  protected internal Format dateFormat;

	  public DateFormType(string datePattern)
	  {
		this.datePattern = datePattern;
		this.dateFormat = FastDateFormat.getInstance(datePattern);
	  }

	  public override string Name
	  {
		  get
		  {
			return "date";
		  }
	  }

	  public override object getInformation(string key)
	  {
		if ("datePattern".Equals(key))
		{
		  return datePattern;
		}
		return null;
	  }

	  public override object convertFormValueToModelValue(string propertyValue)
	  {
		if (StringUtils.isEmpty(propertyValue))
		{
		  return null;
		}
		try
		{
		  return dateFormat.parseObject(propertyValue);
		}
		catch (ParseException)
		{
		  throw new ActivitiIllegalArgumentException("invalid date value " + propertyValue);
		}
	  }

	  public override string convertModelValueToFormValue(object modelValue)
	  {
		if (modelValue == null)
		{
		  return null;
		}
		return dateFormat.format(modelValue);
	  }
	}

}