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

namespace org.activiti.engine.impl.bpmn.parser
{

	using EventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using MessageEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.MessageEventSubscriptionEntity;
	using SignalEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.SignalEventSubscriptionEntity;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class EventSubscriptionDeclaration
	{

	  private const long serialVersionUID = 1L;

	  protected internal readonly string eventName;
	  protected internal readonly string eventType;

	  protected internal bool @async;
	  protected internal string activityId;
	  protected internal bool isStartEvent;
	  protected internal string configuration;

	  public EventSubscriptionDeclaration(string eventName, string eventType)
	  {
		this.eventName = eventName;
		this.eventType = eventType;
	  }

	  public virtual string EventName
	  {
		  get
		  {
			return eventName;
		  }
	  }

	  public virtual bool Async
	  {
		  get
		  {
			return @async;
		  }
		  set
		  {
			this.@async = value;
		  }
	  }


	  public virtual string ActivityId
	  {
		  set
		  {
			this.activityId = value;
		  }
		  get
		  {
			return activityId;
		  }
	  }


	  public virtual bool StartEvent
	  {
		  get
		  {
			return isStartEvent;
		  }
		  set
		  {
			this.isStartEvent = value;
		  }
	  }


	  public virtual string EventType
	  {
		  get
		  {
			return eventType;
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


	  public virtual EventSubscriptionEntity prepareEventSubscriptionEntity(ExecutionEntity execution)
	  {
		EventSubscriptionEntity eventSubscriptionEntity = null;
		if (eventType.Equals("message"))
		{
		  eventSubscriptionEntity = new MessageEventSubscriptionEntity(execution);
		}
		else if (eventType.Equals("signal"))
		{
		  eventSubscriptionEntity = new SignalEventSubscriptionEntity(execution);
		}
		else
		{
		  throw new ActivitiIllegalArgumentException("Found event definition of unknown type: " + eventType);
		}

		eventSubscriptionEntity.EventName = eventName;
		if (activityId != null)
		{
		  ActivityImpl activity = execution.ProcessDefinition.findActivity(activityId);
		  eventSubscriptionEntity.Activity = activity;
		}

		if (configuration != null)
		{
		  eventSubscriptionEntity.Configuration = configuration;
		}

		return eventSubscriptionEntity;
	  }

	}

}