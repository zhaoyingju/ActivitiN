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

	using VariableType = org.activiti.engine.impl.variable.VariableType;

	/// <summary>
	/// Implementation of <seealso cref="ActivitiVariableEvent"/>.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class ActivitiVariableEventImpl : ActivitiEventImpl, ActivitiVariableEvent
	{

		protected internal string variableName;
		protected internal object variableValue;
		protected internal VariableType variableType;
		protected internal string taskId;

		public ActivitiVariableEventImpl(ActivitiEventType type) : base(type)
		{
		}

		public override string VariableName
		{
			get
			{
				return variableName;
			}
			set
			{
			  this.variableName = value;
			}
		}


		public override object VariableValue
		{
			get
			{
				return variableValue;
			}
			set
			{
			  this.variableValue = value;
			}
		}


		public virtual VariableType VariableType
		{
			get
			{
				return variableType;
			}
			set
			{
				this.variableType = value;
			}
		}


		public override string TaskId
		{
			get
			{
				return taskId;
			}
			set
			{
			  this.taskId = value;
			}
		}


	}

}