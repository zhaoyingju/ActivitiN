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
namespace org.activiti.engine.@delegate.@event.impl
{


	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// Class that allows adding and removing event listeners and dispatching events
	/// to the appropriate listeners.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class ActivitiEventSupport
	{

		private static readonly Logger LOG = LoggerFactory.getLogger(typeof(ActivitiEventSupport));

		protected internal IList<ActivitiEventListener> eventListeners;
		protected internal IDictionary<ActivitiEventType, IList<ActivitiEventListener>> typedListeners;

		public ActivitiEventSupport()
		{
			eventListeners = new CopyOnWriteArrayList<ActivitiEventListener>();
			typedListeners = new Dictionary<ActivitiEventType, IList<ActivitiEventListener>>();
		}

		public virtual void addEventListener(ActivitiEventListener listenerToAdd)
		{
			lock (this)
			{
				if (listenerToAdd == null)
				{
					throw new ActivitiIllegalArgumentException("Listener cannot be null.");
				}
				if (!eventListeners.Contains(listenerToAdd))
				{
					eventListeners.Add(listenerToAdd);
				}
			}
		}

		public virtual void addEventListener(ActivitiEventListener listenerToAdd, params ActivitiEventType[] types)
		{
			lock (this)
			{
				if (listenerToAdd == null)
				{
					throw new ActivitiIllegalArgumentException("Listener cannot be null.");
				}
        
				if (types == null || types.Length == 0)
				{
					addEventListener(listenerToAdd);
        
				}
				else
				{
				  foreach (ActivitiEventType type in types)
				  {
					  addTypedEventListener(listenerToAdd, type);
				  }
				}
			}
		}

		public virtual void removeEventListener(ActivitiEventListener listenerToRemove)
		{
			eventListeners.Remove(listenerToRemove);

			foreach (IList<ActivitiEventListener> listeners in typedListeners.Values)
			{
				listeners.Remove(listenerToRemove);
			}
		}

		public virtual void dispatchEvent(ActivitiEvent @event)
		{
			if (@event == null)
			{
				throw new ActivitiIllegalArgumentException("Event cannot be null.");
			}

			if (@event.Type == null)
			{
				throw new ActivitiIllegalArgumentException("Event type cannot be null.");
			}

			// Call global listeners
			if (eventListeners.Count > 0)
			{
				foreach (ActivitiEventListener listener in eventListeners)
				{
					dispatchEvent(@event, listener);
				}
			}

			// Call typed listeners, if any
			IList<ActivitiEventListener> typed = typedListeners[@event.Type];
			if (typed != null && typed.Count > 0)
			{
				foreach (ActivitiEventListener listener in typed)
				{
					dispatchEvent(@event, listener);
				}
			}
		}

		protected internal virtual void dispatchEvent(ActivitiEvent @event, ActivitiEventListener listener)
		{
			try
			{
				listener.onEvent(@event);
			}
			catch (Exception t)
			{
				if (listener.FailOnException)
				{
					throw new ActivitiException("Exception while executing event-listener", t);
				}
				else
				{
					// Ignore the exception and continue notifying remaining listeners. The
					// listener
					// explicitly states that the exception should not bubble up
					LOG.warn("Exception while executing event-listener, which was ignored", t);
				}
			}
		}

		protected internal virtual void addTypedEventListener(ActivitiEventListener listener, ActivitiEventType type)
		{
			lock (this)
			{
				IList<ActivitiEventListener> listeners = typedListeners[type];
				if (listeners == null)
				{
					// Add an empty list of listeners for this type
					listeners = new CopyOnWriteArrayList<ActivitiEventListener>();
					typedListeners[type] = listeners;
				}
        
				if (!listeners.Contains(listener))
				{
					listeners.Add(listener);
				}
			}
		}
	}

}