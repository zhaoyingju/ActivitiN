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

namespace org.activiti.engine.form
{



	/// <summary>
	/// Custom form types should extend this abstract class.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public abstract class AbstractFormType : FormType
	{
		public abstract string Name {get;}

	  private const long serialVersionUID = 1L;

	  public abstract object convertFormValueToModelValue(string propertyValue);

	  public abstract string convertModelValueToFormValue(object modelValue);

	  public virtual object getInformation(string key)
	  {
		return null;
	  }

	}

}