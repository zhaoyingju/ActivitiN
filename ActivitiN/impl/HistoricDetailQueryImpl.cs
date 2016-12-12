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

namespace org.activiti.engine.impl
{

	using HistoricDetail = org.activiti.engine.history.HistoricDetail;
	using HistoricDetailQuery = org.activiti.engine.history.HistoricDetailQuery;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using HistoricDetailVariableInstanceUpdateEntity = org.activiti.engine.impl.persistence.entity.HistoricDetailVariableInstanceUpdateEntity;
	using HistoricJPAEntityListVariableType = org.activiti.engine.impl.variable.HistoricJPAEntityListVariableType;
	using HistoricJPAEntityVariableType = org.activiti.engine.impl.variable.HistoricJPAEntityVariableType;
	using JPAEntityListVariableType = org.activiti.engine.impl.variable.JPAEntityListVariableType;
	using JPAEntityVariableType = org.activiti.engine.impl.variable.JPAEntityVariableType;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class HistoricDetailQueryImpl : AbstractQuery<HistoricDetailQuery, HistoricDetail>, HistoricDetailQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string id_Renamed;
	  protected internal string taskId_Renamed;
	  protected internal string processInstanceId_Renamed;
	  protected internal string executionId_Renamed;
	  protected internal string activityId_Renamed;
	  protected internal string activityInstanceId_Renamed;
	  protected internal string type;
	  protected internal bool excludeTaskRelated = false;

	  public HistoricDetailQueryImpl()
	  {
	  }

	  public HistoricDetailQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public HistoricDetailQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual HistoricDetailQueryImpl id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual HistoricDetailQueryImpl processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual HistoricDetailQueryImpl executionId(string executionId)
	  {
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual HistoricDetailQueryImpl activityId(string activityId)
	  {
		this.activityId_Renamed = activityId;
		return this;
	  }

	  public virtual HistoricDetailQueryImpl activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Renamed = activityInstanceId;
		return this;
	  }

	  public virtual HistoricDetailQueryImpl taskId(string taskId)
	  {
		this.taskId_Renamed = taskId;
		return this;
	  }

	  public virtual HistoricDetailQueryImpl formProperties()
	  {
		this.type = "FormProperty";
		return this;
	  }

	  public virtual HistoricDetailQueryImpl variableUpdates()
	  {
		this.type = "VariableUpdate";
		return this;
	  }

	  public virtual HistoricDetailQueryImpl excludeTaskDetails()
	  {
		this.excludeTaskRelated = true;
		return this;
	  }

	  public virtual long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricDetailEntityManager.findHistoricDetailCountByQueryCriteria(this);
	  }

	  public virtual IList<HistoricDetail> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		IList<HistoricDetail> historicDetails = commandContext.HistoricDetailEntityManager.findHistoricDetailsByQueryCriteria(this, page);

		HistoricDetailVariableInstanceUpdateEntity varUpdate = null;
		if (historicDetails != null)
		{
		  foreach (HistoricDetail historicDetail in historicDetails)
		  {
			if (historicDetail is HistoricDetailVariableInstanceUpdateEntity)
			{
			  varUpdate = (HistoricDetailVariableInstanceUpdateEntity)historicDetail;

			  // Touch byte-array to ensure initialized inside context
			  // TODO there should be a generic way to initialize variable values
			  varUpdate.Bytes;

			  // ACT-863: EntityManagerFactorySession instance needed for fetching value, touch while inside context to store
			  // cached value
			  if (varUpdate.VariableType is JPAEntityVariableType)
			  {
				// Use HistoricJPAEntityVariableType to force caching of value to return from query
				varUpdate.VariableType = HistoricJPAEntityVariableType.SharedInstance;
				varUpdate.Value;
			  }
			  else if (varUpdate.VariableType is JPAEntityListVariableType)
			  {
				// Use HistoricJPAEntityListVariableType to force caching of list to return from query
				varUpdate.VariableType = HistoricJPAEntityListVariableType.SharedInstance;
				varUpdate.Value;
			  }
			}
		  }
		}
		return historicDetails;
	  }

	  // order by /////////////////////////////////////////////////////////////////

	  public virtual HistoricDetailQueryImpl orderByProcessInstanceId()
	  {
		orderBy(HistoricDetailQueryProperty.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricDetailQueryImpl orderByTime()
	  {
		orderBy(HistoricDetailQueryProperty.TIME);
		return this;
	  }

	  public virtual HistoricDetailQueryImpl orderByVariableName()
	  {
		orderBy(HistoricDetailQueryProperty.VARIABLE_NAME);
		return this;
	  }

	  public virtual HistoricDetailQueryImpl orderByFormPropertyId()
	  {
		orderBy(HistoricDetailQueryProperty.VARIABLE_NAME);
		return this;
	  }

	  public virtual HistoricDetailQueryImpl orderByVariableRevision()
	  {
		orderBy(HistoricDetailQueryProperty.VARIABLE_REVISION);
		return this;
	  }

	  public virtual HistoricDetailQueryImpl orderByVariableType()
	  {
		orderBy(HistoricDetailQueryProperty.VARIABLE_TYPE);
		return this;
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Renamed;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId_Renamed;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId_Renamed;
		  }
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
	  }

	  public virtual bool ExcludeTaskRelated
	  {
		  get
		  {
			return excludeTaskRelated;
		  }
	  }
	}

}