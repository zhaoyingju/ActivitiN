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
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// Command that removes an event-listener to the Activiti engine.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class RemoveEventListenerCommand : Command<Void>
	{

		protected internal ActivitiEventListener listener;

		public RemoveEventListenerCommand(ActivitiEventListener listener) : base()
		{
		  this.listener = listener;
		}

		public override Void execute(CommandContext commandContext)
		{
			if (listener == null)
			{
				throw new ActivitiIllegalArgumentException("listener is null.");
			}

			commandContext.ProcessEngineConfiguration.EventDispatcher.removeEventListener(listener);

		  return null;
		}

	}

}