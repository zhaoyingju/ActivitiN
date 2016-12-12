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

namespace org.activiti.engine.impl
{


	using GroupQuery = org.activiti.engine.identity.GroupQuery;
	using QueryProperty = org.activiti.engine.query.QueryProperty;



	/// <summary>
	/// Contains the possible properties that can be used by the <seealso cref="GroupQuery"/>.
	/// 
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class GroupQueryProperty : QueryProperty
	{

	  private const long serialVersionUID = 1L;

	  private static readonly IDictionary<string, GroupQueryProperty> properties = new Dictionary<string, GroupQueryProperty>();

	  public static readonly GroupQueryProperty GROUP_ID = new GroupQueryProperty("RES.ID_");
	  public static readonly GroupQueryProperty NAME = new GroupQueryProperty("RES.NAME_");
	  public static readonly GroupQueryProperty TYPE = new GroupQueryProperty("RES.TYPE_");

	  private string name;

	  public GroupQueryProperty(string name)
	  {
		this.name = name;
		properties[name] = this;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public static GroupQueryProperty findByName(string propertyName)
	  {
		return properties[propertyName];
	  }

	}

}