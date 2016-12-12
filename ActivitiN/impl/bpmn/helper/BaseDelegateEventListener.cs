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
namespace org.activiti.engine.impl.bpmn.helper
{

	using ActivitiEntityEvent = org.activiti.engine.@delegate.@event.ActivitiEntityEvent;
	using ActivitiEvent = org.activiti.engine.@delegate.@event.ActivitiEvent;
	using ActivitiEventListener = org.activiti.engine.@delegate.@event.ActivitiEventListener;

	/// <summary>
	/// Base implementation of a <seealso cref="ActivitiEventListener"/>, used when creating event-listeners
	/// that are part of a BPMN definition.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public abstract class BaseDelegateEventListener : ActivitiEventListener
	{
		public abstract bool FailOnException {get;}
		public abstract void onEvent(ActivitiEvent @event);

		protected internal Type entityClass;

		public virtual Type EntityClass
		{
			set
			{
			  this.entityClass = value;
			}
		}

		protected internal virtual bool isValidEvent(ActivitiEvent @event)
		{
			bool valid = false;
		  if (entityClass != null)
		  {
			  if (@event is ActivitiEntityEvent)
			  {
				  object entity = ((ActivitiEntityEvent) @event).Entity;
				  if (entity != null)
				  {
					  valid = entityClass.IsAssignableFrom(entity.GetType());
				  }
			  }
		  }
		  else
		  {
			  // If no class is specified, all events are valid
			  valid = true;
		  }
		  return valid;
		}

	}

}