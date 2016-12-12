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


	using HistoricVariableInstance = org.activiti.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.activiti.engine.history.HistoricVariableInstanceQuery;
	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using HistoricVariableInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using CacheableVariable = org.activiti.engine.impl.variable.CacheableVariable;
	using JPAEntityListVariableType = org.activiti.engine.impl.variable.JPAEntityListVariableType;
	using JPAEntityVariableType = org.activiti.engine.impl.variable.JPAEntityVariableType;
	using VariableTypes = org.activiti.engine.impl.variable.VariableTypes;

	/// <summary>
	/// @author Christian Lipphardt (camunda)
	/// </summary>
	public class HistoricVariableInstanceQueryImpl : AbstractQuery<HistoricVariableInstanceQuery, HistoricVariableInstance>, HistoricVariableInstanceQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string id_Renamed;
	  protected internal string taskId_Renamed;
	  protected internal Set<string> taskIds_Renamed;
	  protected internal string executionId_Renamed;
	  protected internal Set<string> executionIds_Renamed;
	  protected internal string processInstanceId_Renamed;
	  protected internal string activityInstanceId_Renamed;
	  protected internal string variableName_Renamed;
	  protected internal string variableNameLike_Renamed;
	  protected internal bool excludeTaskRelated = false;
	  protected internal bool excludeVariableInitialization_Renamed = false;
	  protected internal QueryVariableValue queryVariableValue;

	  public HistoricVariableInstanceQueryImpl()
	  {
	  }

	  public HistoricVariableInstanceQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public HistoricVariableInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual HistoricVariableInstanceQuery id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual HistoricVariableInstanceQueryImpl processInstanceId(string processInstanceId)
	  {
		if (processInstanceId == null)
		{
		  throw new ActivitiIllegalArgumentException("processInstanceId is null");
		}
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual HistoricVariableInstanceQueryImpl executionId(string executionId)
	  {
		if (executionId == null)
		{
		  throw new ActivitiIllegalArgumentException("Execution id is null");
		}
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual HistoricVariableInstanceQueryImpl executionIds(Set<string> executionIds)
	  {
		if (executionIds == null)
		{
		  throw new ActivitiIllegalArgumentException("executionIds is null");
		}
		if (executionIds.Empty)
		{
			throw new ActivitiIllegalArgumentException("Set of executionIds is empty");
		}
		this.executionIds_Renamed = executionIds;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Renamed = activityInstanceId;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery taskId(string taskId)
	  {
		if (taskId == null)
		{
		  throw new ActivitiIllegalArgumentException("taskId is null");
		}
		if (excludeTaskRelated)
		{
		  throw new ActivitiIllegalArgumentException("Cannot use taskId together with excludeTaskVariables");
		}
		this.taskId_Renamed = taskId;
		return this;
	  }

	  public virtual HistoricVariableInstanceQueryImpl taskIds(Set<string> taskIds)
	  {
		if (taskIds == null)
		{
		  throw new ActivitiIllegalArgumentException("taskIds is null");
		}
		if (taskIds.Empty)
		{
			throw new ActivitiIllegalArgumentException("Set of taskIds is empty");
		}
		if (excludeTaskRelated)
		{
			throw new ActivitiIllegalArgumentException("Cannot use taskIds together with excludeTaskVariables");
		}
		this.taskIds_Renamed = taskIds;
		return this;
	  }

	  public override HistoricVariableInstanceQuery excludeTaskVariables()
	  {
		if (taskId_Renamed != null)
		{
		  throw new ActivitiIllegalArgumentException("Cannot use taskId together with excludeTaskVariables");
		}
		if (taskIds_Renamed != null)
		{
		  throw new ActivitiIllegalArgumentException("Cannot use taskIds together with excludeTaskVariables");
		}
		excludeTaskRelated = true;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery excludeVariableInitialization()
	  {
		excludeVariableInitialization_Renamed = true;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery variableName(string variableName)
	  {
		if (variableName == null)
		{
		  throw new ActivitiIllegalArgumentException("variableName is null");
		}
		this.variableName_Renamed = variableName;
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery variableValueEquals(string variableName, object variableValue)
	  {
		if (variableName == null)
		{
		  throw new ActivitiIllegalArgumentException("variableName is null");
		}
		if (variableValue == null)
		{
		  throw new ActivitiIllegalArgumentException("variableValue is null");
		}
		this.variableName_Renamed = variableName;
		queryVariableValue = new QueryVariableValue(variableName, variableValue, QueryOperator.EQUALS, true);
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery variableValueNotEquals(string variableName, object variableValue)
	  {
		if (variableName == null)
		{
		  throw new ActivitiIllegalArgumentException("variableName is null");
		}
		if (variableValue == null)
		{
		  throw new ActivitiIllegalArgumentException("variableValue is null");
		}
		this.variableName_Renamed = variableName;
		queryVariableValue = new QueryVariableValue(variableName, variableValue, QueryOperator.NOT_EQUALS, true);
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery variableValueLike(string variableName, string variableValue)
	  {
		if (variableName == null)
		{
		  throw new ActivitiIllegalArgumentException("variableName is null");
		}
		if (variableValue == null)
		{
		  throw new ActivitiIllegalArgumentException("variableValue is null");
		}
		this.variableName_Renamed = variableName;
		queryVariableValue = new QueryVariableValue(variableName, variableValue, QueryOperator.LIKE, true);
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery variableValueLikeIgnoreCase(string variableName, string variableValue)
	  {
		if (variableName == null)
		{
		  throw new ActivitiIllegalArgumentException("variableName is null");
		}
		if (variableValue == null)
		{
		  throw new ActivitiIllegalArgumentException("variableValue is null");
		}
		this.variableName_Renamed = variableName;
		queryVariableValue = new QueryVariableValue(variableName, variableValue.ToLower(), QueryOperator.LIKE_IGNORE_CASE, true);
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery variableNameLike(string variableNameLike)
	  {
		if (variableNameLike == null)
		{
		  throw new ActivitiIllegalArgumentException("variableNameLike is null");
		}
		this.variableNameLike_Renamed = variableNameLike;
		return this;
	  }

	  protected internal virtual void ensureVariablesInitialized()
	  {
		if (this.queryVariableValue != null)
		{
		  VariableTypes variableTypes = Context.ProcessEngineConfiguration.VariableTypes;
		  queryVariableValue.initialize(variableTypes);
		}
	  }

	  public virtual long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		ensureVariablesInitialized();
		return commandContext.HistoricVariableInstanceEntityManager.findHistoricVariableInstanceCountByQueryCriteria(this);
	  }

	  public virtual IList<HistoricVariableInstance> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		ensureVariablesInitialized();

		IList<HistoricVariableInstance> historicVariableInstances = commandContext.HistoricVariableInstanceEntityManager.findHistoricVariableInstancesByQueryCriteria(this, page);

		if (excludeVariableInitialization_Renamed == false)
		{
		  foreach (HistoricVariableInstance historicVariableInstance in historicVariableInstances)
		  {
			if (historicVariableInstance is HistoricVariableInstanceEntity)
			{
			  HistoricVariableInstanceEntity variableEntity = (HistoricVariableInstanceEntity) historicVariableInstance;
			  if (variableEntity != null && variableEntity.VariableType != null)
			  {
				variableEntity.Value;

				// make sure JPA entities are cached for later retrieval
				if (JPAEntityVariableType.TYPE_NAME.Equals(variableEntity.VariableType.TypeName) || JPAEntityListVariableType.TYPE_NAME.Equals(variableEntity.VariableType.TypeName))
				{
				  ((CacheableVariable) variableEntity.VariableType).ForceCacheable = true;
				}
			  }
			}
		  }
		}
		return historicVariableInstances;
	  }

	  // order by /////////////////////////////////////////////////////////////////

	  public virtual HistoricVariableInstanceQuery orderByProcessInstanceId()
	  {
		orderBy(HistoricVariableInstanceQueryProperty.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricVariableInstanceQuery orderByVariableName()
	  {
		orderBy(HistoricVariableInstanceQueryProperty.VARIABLE_NAME);
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

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId_Renamed;
		  }
	  }

	  public virtual bool ExcludeTaskRelated
	  {
		  get
		  {
			return excludeTaskRelated;
		  }
	  }

	  public virtual string VariableName
	  {
		  get
		  {
			return variableName_Renamed;
		  }
	  }

	  public virtual string VariableNameLike
	  {
		  get
		  {
			return variableNameLike_Renamed;
		  }
	  }

	  public virtual QueryVariableValue QueryVariableValue
	  {
		  get
		  {
			return queryVariableValue;
		  }
	  }

	}

}