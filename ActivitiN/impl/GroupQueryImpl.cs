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

	using Group = org.activiti.engine.identity.Group;
	using GroupQuery = org.activiti.engine.identity.GroupQuery;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class GroupQueryImpl : AbstractQuery<GroupQuery, Group>, GroupQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string id;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string type;
	  protected internal string userId;
	  protected internal string procDefId;


	  public GroupQueryImpl()
	  {
	  }

	  public GroupQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public GroupQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual GroupQuery groupId(string id)
	  {
		if (id == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided id is null");
		}
		this.id = id;
		return this;
	  }

	  public virtual GroupQuery groupName(string name)
	  {
		if (name == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided name is null");
		}
		this.name = name;
		return this;
	  }

	  public virtual GroupQuery groupNameLike(string nameLike)
	  {
		if (nameLike == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided nameLike is null");
		}
		this.nameLike = nameLike;
		return this;
	  }

	  public virtual GroupQuery groupType(string type)
	  {
		if (type == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided type is null");
		}
		this.type = type;
		return this;
	  }

	  public virtual GroupQuery groupMember(string userId)
	  {
		if (userId == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided userId is null");
		}
		this.userId = userId;
		return this;
	  }

	  public virtual GroupQuery potentialStarter(string procDefId)
	  {
		if (procDefId == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided processDefinitionId is null or empty");
		}
		this.procDefId = procDefId;
		return this;

	  }

	  //sorting ////////////////////////////////////////////////////////

	  public virtual GroupQuery orderByGroupId()
	  {
		return orderBy(GroupQueryProperty.GROUP_ID);
	  }

	  public virtual GroupQuery orderByGroupName()
	  {
		return orderBy(GroupQueryProperty.NAME);
	  }

	  public virtual GroupQuery orderByGroupType()
	  {
		return orderBy(GroupQueryProperty.TYPE);
	  }

	  //results ////////////////////////////////////////////////////////

	  public virtual long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.GroupIdentityManager.findGroupCountByQueryCriteria(this);
	  }

	  public virtual IList<Group> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.GroupIdentityManager.findGroupByQueryCriteria(this, page);
	  }

	  //getters ////////////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }
	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }
	  public virtual string NameLike
	  {
		  get
		  {
			return nameLike;
		  }
	  }
	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
	  }
	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
	  }


	}

}