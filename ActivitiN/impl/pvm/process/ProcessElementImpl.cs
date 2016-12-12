using System;
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

namespace org.activiti.engine.impl.pvm.process
{



	/// <summary>
	/// common properties for process definition, activity and transition 
	/// including event listeners.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class ProcessElementImpl : PvmProcessElement
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal ProcessDefinitionImpl processDefinition;
	  protected internal IDictionary<string, object> properties;

	  public ProcessElementImpl(string id, ProcessDefinitionImpl processDefinition)
	  {
		this.id = id;
		this.processDefinition = processDefinition;
	  }

	  public virtual void setProperty(string name, object value)
	  {
		if (properties == null)
		{
		  properties = new Dictionary<string, object>();
		}
		properties[name] = value;
	  }

	  public virtual object getProperty(string name)
	  {
		if (properties == null)
		{
		  return null;
		}
		return properties[name];
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Map<String, Object> getProperties()
	  public virtual IDictionary<string, object> Properties
	  {
		  get
		  {
			if (properties == null)
			{
			  return Collections.EMPTY_MAP;
			}
			return properties;
		  }
		  set
		  {
			this.properties = value;
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }


	  public virtual ProcessDefinitionImpl ProcessDefinition
	  {
		  get
		  {
			return processDefinition;
		  }
	  }
	}

}