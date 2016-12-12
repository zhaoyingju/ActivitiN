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
	using ReflectUtil = org.activiti.engine.impl.util.ReflectUtil;

	/// <summary>
	/// An <seealso cref="ActivitiEventListener"/> implementation which uses a classname to
	/// create a delegate <seealso cref="ActivitiEventListener"/> instance to use for event notification.
	/// <br><br>
	/// 
	/// In case an entityClass was passed in the constructor, only events that are <seealso cref="ActivitiEntityEvent"/>'s
	/// that target an entity of the given type, are dispatched to the delegate.
	///  
	/// @author Frederik Heremans
	/// </summary>
	public class DelegateActivitiEventListener : BaseDelegateEventListener
	{

		protected internal string className;
		protected internal ActivitiEventListener delegateInstance;
		protected internal bool failOnException = true;

		public DelegateActivitiEventListener(string className, Type entityClass)
		{
			this.className = className;
			EntityClass = entityClass;
		}

		public override void onEvent(ActivitiEvent @event)
		{
			if (isValidEvent(@event))
			{
				DelegateInstance.onEvent(@event);
			}
		}

		public override bool FailOnException
		{
			get
			{
				if (delegateInstance != null)
				{
					return delegateInstance.FailOnException;
				}
				return failOnException;
			}
		}

		protected internal virtual ActivitiEventListener DelegateInstance
		{
			get
			{
				if (delegateInstance == null)
				{
					object instance = ReflectUtil.instantiate(className);
					if (instance is ActivitiEventListener)
					{
						delegateInstance = (ActivitiEventListener) instance;
					}
					else
					{
						// Force failing of the listener invocation, since the delegate cannot be created
						failOnException = true;
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
						throw new ActivitiIllegalArgumentException("Class " + className + " does not implement " + typeof(ActivitiEventListener).FullName);
					}
				}
				return delegateInstance;
			}
		}
	}

}