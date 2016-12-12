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
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using SignalEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.SignalEventSubscriptionEntity;
	using Execution = org.activiti.engine.runtime.Execution;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public class SignalEventReceivedCmd : Command<Void>
	{

	  protected internal readonly string eventName;
	  protected internal readonly string executionId;
	  protected internal readonly Serializable payload;
	  protected internal readonly bool @async;
	  protected internal string tenantId;

	  public SignalEventReceivedCmd(string eventName, string executionId, IDictionary<string, object> processVariables, string tenantId)
	  {
		this.eventName = eventName;
		this.executionId = executionId;
		if (processVariables != null)
		{
			if (processVariables is Serializable)
			{
				this.payload = (Serializable) processVariables;
			}
			else
			{
				this.payload = new Dictionary<string, object>(processVariables);
			}
		}
		else
		{
			this.payload = null;
		}
		this.@async = false;
		this.tenantId = tenantId;
	  }

	  public SignalEventReceivedCmd(string eventName, string executionId, bool @async, string tenantId)
	  {
		  this.eventName = eventName;
		  this.executionId = executionId;
		  this.@async = @async;
		  this.payload = null;
		  this.tenantId = tenantId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {

		IList<SignalEventSubscriptionEntity> signalEvents = null;

		if (executionId == null)
		{
		   signalEvents = commandContext.EventSubscriptionEntityManager.findSignalEventSubscriptionsByEventName(eventName, tenantId);
		}
		else
		{

		  ExecutionEntity execution = commandContext.ExecutionEntityManager.findExecutionById(executionId);

		  if (execution == null)
		  {
			throw new ActivitiObjectNotFoundException("Cannot find execution with id '" + executionId + "'", typeof(Execution));
		  }

		  if (execution.Suspended)
		  {
			throw new ActivitiException("Cannot throw signal event '" + eventName + "' because execution '" + executionId + "' is suspended");
		  }

		  signalEvents = commandContext.EventSubscriptionEntityManager.findSignalEventSubscriptionsByNameAndExecution(eventName, executionId);

		  if (signalEvents.Count == 0)
		  {
			throw new ActivitiException("Execution '" + executionId + "' has not subscribed to a signal event with name '" + eventName + "'.");
		  }
		}


		foreach (SignalEventSubscriptionEntity signalEventSubscriptionEntity in signalEvents)
		{
		  // We only throw the event to globally scoped signals. 
		  // Process instance scoped signals must be thrown within the process itself 
		  if (signalEventSubscriptionEntity.GlobalScoped)
		  {
			signalEventSubscriptionEntity.eventReceived(payload, @async);
		  }
		}

		return null;
	  }

	}

}