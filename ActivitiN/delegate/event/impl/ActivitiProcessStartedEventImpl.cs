using System.Collections;

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

	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// An <seealso cref="org.activiti.engine.delegate.event.ActivitiCancelledEvent"/> implementation.
	/// 
	/// @author martin.grofcik
	/// </summary>
	public class ActivitiProcessStartedEventImpl : ActivitiEntityWithVariablesEventImpl, ActivitiProcessStartedEvent
	{

	  protected internal readonly string nestedProcessInstanceId;

	  protected internal readonly string nestedProcessDefinitionId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public ActivitiProcessStartedEventImpl(final Object entity, final java.util.Map variables, final boolean localScope)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public ActivitiProcessStartedEventImpl(object entity, IDictionary variables, bool localScope) : base(entity, variables, localScope, ActivitiEventType.PROCESS_STARTED)
	  {
		if (entity is ExecutionEntity)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.activiti.engine.impl.persistence.entity.ExecutionEntity superExecution = ((org.activiti.engine.impl.persistence.entity.ExecutionEntity) entity).getSuperExecution();
		  ExecutionEntity superExecution = ((ExecutionEntity) entity).getSuperExecution();
		  if (superExecution != null)
		  {
			this.nestedProcessDefinitionId = superExecution.ProcessDefinitionId;
			this.nestedProcessInstanceId = superExecution.ProcessInstanceId;
		  }
		  else
		  {
			this.nestedProcessDefinitionId = null;
			this.nestedProcessInstanceId = null;
		  }
		}
		else
		{
		  this.nestedProcessDefinitionId = null;
		  this.nestedProcessInstanceId = null;
		}
	  }

	  public override string NestedProcessInstanceId
	  {
		  get
		  {
			return this.nestedProcessInstanceId;
		  }
	  }

	  public override string NestedProcessDefinitionId
	  {
		  get
		  {
			return this.nestedProcessDefinitionId;
		  }
	  }

	}

}