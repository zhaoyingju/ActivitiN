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

namespace org.activiti.engine.impl.cmd
{


	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using IdentityInfoEntity = org.activiti.engine.impl.persistence.entity.IdentityInfoEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class SetUserInfoCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string userId;
	  protected internal string userPassword;
	  protected internal string type;
	  protected internal string key;
	  protected internal string value;
	  protected internal string accountPassword;
	  protected internal IDictionary<string, string> accountDetails;

	  public SetUserInfoCmd(string userId, string key, string value)
	  {
		this.userId = userId;
		this.type = IdentityInfoEntity.TYPE_USERINFO;
		this.key = key;
		this.value = value;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		commandContext.IdentityInfoEntityManager.setUserInfo(userId, userPassword, type, key, value, accountPassword, accountDetails);
		return null;
	  }
	}

}