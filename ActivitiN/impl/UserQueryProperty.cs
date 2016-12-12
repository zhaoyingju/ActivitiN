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


	using UserQuery = org.activiti.engine.identity.UserQuery;
	using QueryProperty = org.activiti.engine.query.QueryProperty;



	/// <summary>
	/// Contains the possible properties that can be used by the <seealso cref="UserQuery"/>.
	/// 
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class UserQueryProperty : QueryProperty
	{

	  private const long serialVersionUID = 1L;

	  private static readonly IDictionary<string, UserQueryProperty> properties = new Dictionary<string, UserQueryProperty>();

	  public static readonly UserQueryProperty USER_ID = new UserQueryProperty("RES.ID_");
	  public static readonly UserQueryProperty FIRST_NAME = new UserQueryProperty("RES.FIRST_");
	  public static readonly UserQueryProperty LAST_NAME = new UserQueryProperty("RES.LAST_");
	  public static readonly UserQueryProperty EMAIL = new UserQueryProperty("RES.EMAIL_");

	  private string name;

	  public UserQueryProperty(string name)
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

	  public static UserQueryProperty findByName(string propertyName)
	  {
		return properties[propertyName];
	  }

	}

}