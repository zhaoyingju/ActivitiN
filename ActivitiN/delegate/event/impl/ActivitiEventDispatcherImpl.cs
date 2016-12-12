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
namespace org.activiti.engine.@delegate.@event.impl
{

	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;

	/// <summary>
	/// Class capable of dispatching events.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class ActivitiEventDispatcherImpl : ActivitiEventDispatcher
	{

		protected internal ActivitiEventSupport eventSupport;
		protected internal bool enabled = true;

		public ActivitiEventDispatcherImpl()
		{
			eventSupport = new ActivitiEventSupport();
		}

		public virtual bool Enabled
		{
			set
			{
				this.enabled = value;
			}
			get
			{
				return enabled;
			}
		}


		public override void addEventListener(ActivitiEventListener listenerToAdd)
		{
			eventSupport.addEventListener(listenerToAdd);
		}

		public override void addEventListener(ActivitiEventListener listenerToAdd, params ActivitiEventType[] types)
		{
			eventSupport.addEventListener(listenerToAdd, types);
		}

		public override void removeEventListener(ActivitiEventListener listenerToRemove)
		{
			eventSupport.removeEventListener(listenerToRemove);
		}

		public override void dispatchEvent(ActivitiEvent @event)
		{
			if (enabled)
			{
				eventSupport.dispatchEvent(@event);
			}

			// Check if a process context is active. If so, we also call the
			// process-definition specific listeners (if any).
			if (Context.ExecutionContextActive)
			{
				ProcessDefinitionEntity definition = Context.ExecutionContext.ProcessDefinition;
				if (definition != null)
				{
					definition.EventSupport.dispatchEvent(@event);
				}
			}
			else
			{
				// Try getting hold of the Process definition, based on the process
				// definition-key, if a context is active
				CommandContext commandContext = Context.CommandContext;
				if (commandContext != null)
				{
					ProcessDefinitionEntity processDefinition = extractProcessDefinitionEntityFromEvent(@event);
					if (processDefinition != null)
					{
						processDefinition.EventSupport.dispatchEvent(@event);
					}
				}
			}
		}

		/// <summary>
		/// In case no process-context is active, this method attempts to extract a
		/// process-definition based on the event. In case it's an event related to an
		/// entity, this can be deducted by inspecting the entity, without additional
		/// queries to the database.
		/// 
		/// If not an entity-related event, the process-definition will be retrieved
		/// based on the processDefinitionId (if filled in). This requires an
		/// additional query to the database in case not already cached. However,
		/// queries will only occur when the definition is not yet in the cache, which
		/// is very unlikely to happen, unless evicted.
		/// </summary>
		/// <param name="event">
		/// @return </param>
		protected internal virtual ProcessDefinitionEntity extractProcessDefinitionEntityFromEvent(ActivitiEvent @event)
		{
			ProcessDefinitionEntity result = null;

			if (@event.ProcessDefinitionId != null)
			{
				result = Context.ProcessEngineConfiguration.DeploymentManager.ProcessDefinitionCache.get(@event.ProcessDefinitionId);
				if (result != null)
				{
					result = Context.ProcessEngineConfiguration.DeploymentManager.resolveProcessDefinition(result);
				}
			}

			if (result == null && @event is ActivitiEntityEvent)
			{
				object entity = ((ActivitiEntityEvent) @event).Entity;
				if (entity is ProcessDefinition)
				{
					result = (ProcessDefinitionEntity) entity;
				}
			}
			return result;
		}

	}

}