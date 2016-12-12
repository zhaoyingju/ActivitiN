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

namespace org.activiti.engine.impl.persistence.entity
{

	using HistoricFormProperty = org.activiti.engine.history.HistoricFormProperty;
	using Context = org.activiti.engine.impl.context.Context;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class HistoricFormPropertyEntity : HistoricDetailEntity, HistoricFormProperty
	{

	  private const long serialVersionUID = 1L;

	  protected internal string propertyId;
	  protected internal string propertyValue;

	  public HistoricFormPropertyEntity()
	  {
		this.detailType = "FormProperty";
	  }

	  public HistoricFormPropertyEntity(ExecutionEntity execution, string propertyId, string propertyValue) : this(execution, propertyId, propertyValue, null)
	  {
	  }

	  public HistoricFormPropertyEntity(ExecutionEntity execution, string propertyId, string propertyValue, string taskId)
	  {
		this.processInstanceId = execution.ProcessInstanceId;
		this.executionId = execution.Id;
		this.taskId = taskId;
		this.propertyId = propertyId;
		this.propertyValue = propertyValue;
		this.time = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		this.detailType = "FormProperty";

		HistoricActivityInstanceEntity historicActivityInstance = Context.CommandContext.HistoryManager.findActivityInstance(execution);
		if (historicActivityInstance != null)
		{
		  this.activityInstanceId = historicActivityInstance.Id;
		}
	  }

	  public virtual string PropertyId
	  {
		  get
		  {
			return propertyId;
		  }
		  set
		  {
			this.propertyId = value;
		  }
	  }


	  public virtual string PropertyValue
	  {
		  get
		  {
			return propertyValue;
		  }
		  set
		  {
			this.propertyValue = value;
		  }
	  }

	}

}