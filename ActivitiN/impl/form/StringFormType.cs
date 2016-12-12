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
	public class StringFormType : AbstractFormType
	{

	  private const long serialVersionUID = 1L;

	  public override string Name
	  {
		  get
		  {
			return "string";
		  }
	  }

	  public virtual string MimeType
	  {
		  get
		  {
			return "text/plain";
		  }
	  }

	  public override object convertFormValueToModelValue(string propertyValue)
	  {
		return propertyValue;
	  }

	  public override string convertModelValueToFormValue(object modelValue)
	  {
		return (string) modelValue;
	  }

	}

}