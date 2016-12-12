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


	/// <summary>
	/// An <seealso cref="ActivitiSignalEvent"/> implementation.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class ActivitiMessageEventImpl : ActivitiActivityEventImpl, ActivitiMessageEvent
	{

		protected internal string messageName;
		protected internal object messageData;

		public ActivitiMessageEventImpl(ActivitiEventType type) : base(type)
		{
		}

		public virtual string MessageName
		{
			set
			{
			  this.messageName = value;
			}
			get
			{
			  return messageName;
			}
		}


		public virtual object MessageData
		{
			set
			{
			  this.messageData = value;
			}
			get
			{
			  return messageData;
			}
		}

	}

}