using System;
using System.Collections.Generic;
using System.Text;

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


	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using Context = org.activiti.engine.impl.context.Context;
	using BulkDeleteable = org.activiti.engine.impl.db.BulkDeleteable;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using IdentityLink = org.activiti.engine.task.IdentityLink;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class IdentityLinkEntity : IdentityLink, BulkDeleteable, PersistentObject
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;

	  protected internal string type;

	  protected internal string userId;

	  protected internal string groupId;

	  protected internal string taskId;

	  protected internal string processInstanceId;

	  protected internal string processDefId;

	  protected internal TaskEntity task;

	  protected internal ExecutionEntity processInstance;

	  protected internal ProcessDefinitionEntity processDef;

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
    
			if (this.processDefId != null)
			{
			  persistentState["processDefId"] = this.processDefId;
			}
    
			return persistentState;
		  }
	  }

	  public virtual void insert()
	  {
		Context.CommandContext.DbSqlSession.insert(this);


		Context.CommandContext.HistoryManager.recordIdentityLinkCreated(this);

		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, this));
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_INITIALIZED, this));
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


	  public virtual string ProcessDefId
	  {
		  get
		  {
			return processDefId;
		  }
		  set
		  {
			this.processDefId = value;
		  }
	  }


	  public virtual TaskEntity getTask()
	  {
		if ((task == null) && (taskId != null))
		{
		  this.task = Context.CommandContext.TaskEntityManager.findTaskById(taskId);
		}
		return task;
	  }

	  public virtual void setTask(TaskEntity task)
	  {
		this.task = task;
		this.taskId = task.Id;
	  }

	  public virtual ExecutionEntity getProcessInstance()
	  {
		if ((processInstance == null) && (processInstanceId != null))
		{
		  this.processInstance = Context.CommandContext.ExecutionEntityManager.findExecutionById(processInstanceId);
		}
		return processInstance;
	  }

	  public virtual void setProcessInstance(ExecutionEntity processInstance)
	  {
		this.processInstance = processInstance;
		this.processInstanceId = processInstance.Id;
	  }

	  public virtual ProcessDefinitionEntity getProcessDef()
	  {
		if ((processDef == null) && (processDefId != null))
		{
		  this.processDef = Context.CommandContext.ProcessDefinitionEntityManager.findProcessDefinitionById(processDefId);
		}
		return processDef;
	  }

	  public virtual void setProcessDef(ProcessDefinitionEntity processDef)
	  {
		this.processDef = processDef;
		this.processDefId = processDef.Id;
	  }

	  public override string ProcessDefinitionId
	  {
		  get
		  {
			return this.processDefId;
		  }
	  }

	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append("IdentityLinkEntity[id=").Append(id);
		sb.Append(", type=").Append(type);
		if (userId != null)
		{
		  sb.Append(", userId=").Append(userId);
		}
		if (groupId != null)
		{
		  sb.Append(", groupId=").Append(groupId);
		}
		if (taskId != null)
		{
		  sb.Append(", taskId=").Append(taskId);
		}
		if (processInstanceId != null)
		{
		  sb.Append(", processInstanceId=").Append(processInstanceId);
		}
		if (processDefId != null)
		{
		  sb.Append(", processDefId=").Append(processDefId);
		}
		sb.Append("]");
		return sb.ToString();
	  }
	}

}