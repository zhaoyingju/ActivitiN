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
	public class ActivitiSignalEventImpl : ActivitiActivityEventImpl, ActivitiSignalEvent
	{

		protected internal string signalName;
		protected internal object signalData;

		public ActivitiSignalEventImpl(ActivitiEventType type) : base(type)
		{
		}

		public override string SignalName
		{
			get
			{
				return signalName;
			}
			set
			{
			  this.signalName = value;
			}
		}


		public override object SignalData
		{
			get
			{
				return signalData;
			}
			set
			{
			  this.signalData = value;
			}
		}

	}

}