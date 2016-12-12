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


	using Context = org.activiti.engine.impl.context.Context;
	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using EventHandler = org.activiti.engine.impl.@event.EventHandler;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ProcessEventJobHandler = org.activiti.engine.impl.jobexecutor.ProcessEventJobHandler;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.activiti.engine.impl.pvm.process.ProcessDefinitionImpl;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public abstract class EventSubscriptionEntity : PersistentObject, HasRevision
	{

	  private const long serialVersionUID = 1L;

	  // persistent state ///////////////////////////
	  protected internal string id;
	  protected internal int revision = 1;
	  protected internal string eventType;
	  protected internal string eventName;
	  protected internal string executionId;
	  protected internal string processInstanceId;
	  protected internal string activityId;
	  protected internal string configuration;
	  protected internal DateTime created;
	  protected internal string processDefinitionId;
	  protected internal string tenantId;

	  // runtime state /////////////////////////////
	  protected internal ExecutionEntity execution;
	  protected internal ActivityImpl activity;

	  /////////////////////////////////////////////

	  public EventSubscriptionEntity()
	  {
		this.created = Context.ProcessEngineConfiguration.Clock.CurrentTime;
	  }

	  public EventSubscriptionEntity(ExecutionEntity executionEntity) : this()
	  {
		setExecution(executionEntity);
		Activity = execution.Activity;
		this.processInstanceId = executionEntity.ProcessInstanceId;
		this.processDefinitionId = executionEntity.ProcessDefinitionId;
	  }

	  // processing /////////////////////////////

	  public virtual void eventReceived(Serializable payload, bool processASync)
	  {
		if (processASync)
		{
		  scheduleEventAsync(payload);
		}
		else
		{
		  processEventSync(payload);
		}
	  }

	  protected internal virtual void processEventSync(object payload)
	  {
		EventHandler eventHandler = Context.ProcessEngineConfiguration.getEventHandler(eventType);
		if (eventHandler == null)
		{
		  throw new ActivitiException("Could not find eventhandler for event of type '" + eventType + "'.");
		}
		eventHandler.handleEvent(this, payload, Context.CommandContext);
	  }

	  protected internal virtual void scheduleEventAsync(Serializable payload)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.activiti.engine.impl.interceptor.CommandContext commandContext = org.activiti.engine.impl.context.Context.getCommandContext();
		CommandContext commandContext = Context.CommandContext;

		MessageEntity message = new MessageEntity();
		message.JobHandlerType = ProcessEventJobHandler.TYPE;
		message.JobHandlerConfiguration = id;
		message.TenantId = TenantId;

		GregorianCalendar expireCal = new GregorianCalendar();
		ProcessEngineConfiguration processEngineConfig = Context.CommandContext.ProcessEngineConfiguration;
		expireCal.Time = processEngineConfig.Clock.CurrentTime;
		expireCal.add(DateTime.SECOND, processEngineConfig.LockTimeAsyncJobWaitTime);
		message.LockExpirationTime = expireCal.Time;

		// TODO: support payload
	//    if(payload != null) {
	//      message.setEventPayload(payload);
	//    }

		commandContext.JobEntityManager.send(message);
	  }

	  // persistence behavior /////////////////////

	  public virtual void delete()
	  {
		Context.CommandContext.EventSubscriptionEntityManager.deleteEventSubscription(this);
		removeFromExecution();
	  }

	  public virtual void insert()
	  {
		Context.CommandContext.EventSubscriptionEntityManager.insert(this);
		addToExecution();
	  }

	 // referential integrity -> ExecutionEntity ////////////////////////////////////

	  protected internal virtual void addToExecution()
	  {
		// add reference in execution
		ExecutionEntity execution = getExecution();
		if (execution != null)
		{
		  execution.addEventSubscription(this);
		}
	  }

	  protected internal virtual void removeFromExecution()
	  {
		// remove reference in execution
		ExecutionEntity execution = getExecution();
		if (execution != null)
		{
		  execution.removeEventSubscription(this);
		}
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			Dictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["executionId"] = executionId;
			persistentState["configuration"] = configuration;
			persistentState["processDefinitionId"] = processDefinitionId;
			return persistentState;
		  }
	  }

	  // getters & setters ////////////////////////////

	  public virtual ExecutionEntity getExecution()
	  {
		if (execution == null && executionId != null)
		{
		  execution = Context.CommandContext.ExecutionEntityManager.findExecutionById(executionId);
		}
		return execution;
	  }

	  public virtual void setExecution(ExecutionEntity execution)
	  {
		this.execution = execution;
		if (execution != null)
		{
		  this.executionId = execution.Id;
		}
	  }

	  public virtual ActivityImpl Activity
	  {
		  get
		  {
			if (activity == null && activityId != null)
			{
			  ExecutionEntity execution = getExecution();
			  if (execution != null)
			  {
				ProcessDefinitionImpl processDefinition = execution.ProcessDefinition;
				activity = processDefinition.findActivity(activityId);
			  }
			}
			return activity;
		  }
		  set
		  {
			this.activity = value;
			if (value != null)
			{
			  this.activityId = value.Id;
			}
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


	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }


	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public virtual string EventType
	  {
		  get
		  {
			return eventType;
		  }
		  set
		  {
			this.eventType = value;
		  }
	  }


	  public virtual string EventName
	  {
		  get
		  {
			return eventName;
		  }
		  set
		  {
			this.eventName = value;
		  }
	  }


	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
		  set
		  {
			this.executionId = value;
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


	  public virtual string Configuration
	  {
		  get
		  {
			return configuration;
		  }
		  set
		  {
			this.configuration = value;
		  }
	  }


	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
		  set
		  {
			this.activityId = value;
		  }
	  }


	  public virtual DateTime Created
	  {
		  get
		  {
			return created;
		  }
		  set
		  {
			this.created = value;
		  }
	  }


	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
				return processDefinitionId;
		  }
		  set
		  {
				this.processDefinitionId = value;
		  }
	  }


	  public virtual string TenantId
	  {
		  get
		  {
				return tenantId;
		  }
		  set
		  {
				this.tenantId = value;
		  }
	  }


		public override int GetHashCode()
		{
		const int prime = 31;
		int result = 1;
		result = prime * result + ((id == null) ? 0 : id.GetHashCode());
		return result;
		}

	  public override bool Equals(object obj)
	  {
		if (this == obj)
		{
		  return true;
		}
		if (obj == null)
		{
		  return false;
		}
		if (this.GetType() != obj.GetType())
		{
		  return false;
		}
		EventSubscriptionEntity other = (EventSubscriptionEntity) obj;
		if (id == null)
		{
		  if (other.id != null)
		  {
			return false;
		  }
		}
		else if (!id.Equals(other.id))
		{
		  return false;
		}
		return true;
	  }

	}

}