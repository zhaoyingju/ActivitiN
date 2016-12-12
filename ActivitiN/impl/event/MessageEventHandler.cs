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

namespace org.activiti.engine.impl.@event
{

	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using EventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.EventSubscriptionEntity;



	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class MessageEventHandler : AbstractEventHandler
	{

	  public const string EVENT_HANDLER_TYPE = "message";

	  public override string EventHandlerType
	  {
		  get
		  {
			return EVENT_HANDLER_TYPE;
		  }
	  }

	  public override void handleEvent(EventSubscriptionEntity eventSubscription, object payload, CommandContext commandContext)
	  {
		  // As stated in the ActivitiEventType java-doc, the message-event is thrown before the actual message has been sent
		  if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
			commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createMessageEvent(ActivitiEventType.ACTIVITY_MESSAGE_RECEIVED, eventSubscription.ActivityId, eventSubscription.EventName, payload, eventSubscription.ExecutionId, eventSubscription.ProcessInstanceId, eventSubscription.getExecution().ProcessDefinitionId));
		  }

		base.handleEvent(eventSubscription, payload, commandContext);
	  }

	}

}