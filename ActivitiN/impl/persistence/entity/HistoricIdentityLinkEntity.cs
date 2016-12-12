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
namespace org.activiti.engine.impl.persistence.entity
{


	using HistoricIdentityLink = org.activiti.engine.history.HistoricIdentityLink;
	using BulkDeleteable = org.activiti.engine.impl.db.BulkDeleteable;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;


	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	[Serializable]
	public class HistoricIdentityLinkEntity : HistoricIdentityLink, BulkDeleteable, PersistentObject
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;

	  protected internal string type;

	  protected internal string userId;

	  protected internal string groupId;

	  protected internal string taskId;

	  protected internal string processInstanceId;

	  public HistoricIdentityLinkEntity(IdentityLinkEntity identityLink)
	  {
		this.id = identityLink.Id;
		this.groupId = identityLink.GroupId;
		this.processInstanceId = identityLink.ProcessInstanceId;
		this.taskId = identityLink.TaskId;
		this.type = identityLink.Type;
		this.userId = identityLink.UserId;
	  }


	  public HistoricIdentityLinkEntity()
	  {

	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["id"] = this.id;
			persistentState["type"] = this.type;
    
			if (this.userId != null)
			{
			  persistentState["userId"] = this.userId;
			}
    
			if (this.groupId != null)
			{
			  persistentState["groupId"] = this.groupId;
			}
    
			if (this.taskId != null)
			{
			  persistentState["taskId"] = this.taskId;
			}
    
			if (this.processInstanceId != null)
			{
			  persistentState["processInstanceId"] = this.processInstanceId;
			}
    
			return persistentState;
		  }
	  }

	  public virtual bool User
	  {
		  get
		  {
			return userId != null;
		  }
	  }

	  public virtual bool Group
	  {
		  get
		  {
			return groupId != null;
		  }
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }


	  public virtual string UserId
	  {
		  get
		  {
			return userId;
		  }
		  set
		  {
			if (this.groupId != null && value != null)
			{
			  throw new ActivitiException("Cannot assign a userId to a task assignment that already has a groupId");
			}
			this.userId = value;
		  }
	  }


	  public virtual string GroupId
	  {
		  get
		  {
			return groupId;
		  }
		  set
		  {
			if (this.userId != null && value != null)
			{
			  throw new ActivitiException("Cannot assign a groupId to a task assignment that already has a userId");
			}
			this.groupId = value;
		  }
	  }


	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
		  set
		  {
			this.taskId = value;
		  }
	  }


	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }

	}

}