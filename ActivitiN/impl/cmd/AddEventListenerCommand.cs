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

	using ActivitiEventListener = org.activiti.engine.@delegate.@event.ActivitiEventListener;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// Command that adds an event-listener to the Activiti engine.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class AddEventListenerCommand : Command<Void>
	{

		protected internal ActivitiEventListener listener;
		protected internal ActivitiEventType[] types;

		public AddEventListenerCommand(ActivitiEventListener listener, ActivitiEventType[] types)
		{
		  this.listener = listener;
		  this.types = types;
		}

		public AddEventListenerCommand(ActivitiEventListener listener) : base()
		{
		  this.listener = listener;
		}

		public override Void execute(CommandContext commandContext)
		{
			if (listener == null)
			{
				throw new ActivitiIllegalArgumentException("listener is null.");
			}

			if (types != null)
			{
				commandContext.ProcessEngineConfiguration.EventDispatcher.addEventListener(listener, types);
			}
			else
			{
				commandContext.ProcessEngineConfiguration.EventDispatcher.addEventListener(listener);
			}

		  return null;
		}

	}

}