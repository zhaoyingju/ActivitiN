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

	using User = org.activiti.engine.identity.User;
	using UserQuery = org.activiti.engine.identity.UserQuery;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class UserQueryImpl : AbstractQuery<UserQuery, User>, UserQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string id;
	  protected internal string firstName;
	  protected internal string firstNameLike;
	  protected internal string lastName;
	  protected internal string lastNameLike;
	  protected internal string fullNameLike;
	  protected internal string email;
	  protected internal string emailLike;
	  protected internal string groupId;
	  protected internal string procDefId;

	  public UserQueryImpl()
	  {
	  }

	  public UserQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public UserQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual UserQuery userId(string id)
	  {
		if (id == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided id is null");
		}
		this.id = id;
		return this;
	  }

	  public virtual UserQuery userFirstName(string firstName)
	  {
		if (firstName == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided firstName is null");
		}
		this.firstName = firstName;
		return this;
	  }

	  public virtual UserQuery userFirstNameLike(string firstNameLike)
	  {
		if (firstNameLike == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided firstNameLike is null");
		}
		this.firstNameLike = firstNameLike;
		return this;
	  }

	  public virtual UserQuery userLastName(string lastName)
	  {
		if (lastName == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided lastName is null");
		}
		this.lastName = lastName;
		return this;
	  }

	  public virtual UserQuery userLastNameLike(string lastNameLike)
	  {
		if (lastNameLike == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided lastNameLike is null");
		}
		this.lastNameLike = lastNameLike;
		return this;
	  }

	  public virtual UserQuery userFullNameLike(string fullNameLike)
	  {
		if (fullNameLike == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided full name is null");
		}
		this.fullNameLike = fullNameLike;
		return this;
	  }

	  public virtual UserQuery userEmail(string email)
	  {
		if (email == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided email is null");
		}
		this.email = email;
		return this;
	  }

	  public virtual UserQuery userEmailLike(string emailLike)
	  {
		if (emailLike == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided emailLike is null");
		}
		this.emailLike = emailLike;
		return this;
	  }

	  public virtual UserQuery memberOfGroup(string groupId)
	  {
		if (groupId == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided groupIds is null or empty");
		}
		this.groupId = groupId;
		return this;
	  }

	  public virtual UserQuery potentialStarter(string procDefId)
	  {
		if (procDefId == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided processDefinitionId is null or empty");
		}
		this.procDefId = procDefId;
		return this;

	  }

	  //sorting //////////////////////////////////////////////////////////

	  public virtual UserQuery orderByUserId()
	  {
		return orderBy(UserQueryProperty.USER_ID);
	  }

	  public virtual UserQuery orderByUserEmail()
	  {
		return orderBy(UserQueryProperty.EMAIL);
	  }

	  public virtual UserQuery orderByUserFirstName()
	  {
		return orderBy(UserQueryProperty.FIRST_NAME);
	  }

	  public virtual UserQuery orderByUserLastName()
	  {
		return orderBy(UserQueryProperty.LAST_NAME);
	  }

	  //results //////////////////////////////////////////////////////////

	  public virtual long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.UserIdentityManager.findUserCountByQueryCriteria(this);
	  }

	  public virtual IList<User> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.UserIdentityManager.findUserByQueryCriteria(this, page);
	  }

	  //getters //////////////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }
	  public virtual string FirstName
	  {
		  get
		  {
			return firstName;
		  }
	  }
	  public virtual string FirstNameLike
	  {
		  get
		  {
			return firstNameLike;
		  }
	  }
	  public virtual string LastName
	  {
		  get
		  {
			return lastName;
		  }
	  }
	  public virtual string LastNameLike
	  {
		  get
		  {
			return lastNameLike;
		  }
	  }
	  public virtual string Email
	  {
		  get
		  {
			return email;
		  }
	  }
	  public virtual string EmailLike
	  {
		  get
		  {
			return emailLike;
		  }
	  }
	  public virtual string GroupId
	  {
		  get
		  {
			return groupId;
		  }
	  }
	  public virtual string FullNameLike
	  {
		  get
		  {
			return fullNameLike;
		  }
	  }

	}

}