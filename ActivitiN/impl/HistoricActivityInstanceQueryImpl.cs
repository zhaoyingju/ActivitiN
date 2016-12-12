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

	using HistoricActivityInstance = org.activiti.engine.history.HistoricActivityInstance;
	using HistoricActivityInstanceQuery = org.activiti.engine.history.HistoricActivityInstanceQuery;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class HistoricActivityInstanceQueryImpl : AbstractQuery<HistoricActivityInstanceQuery, HistoricActivityInstance>, HistoricActivityInstanceQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string activityInstanceId_Renamed;
	  protected internal string processInstanceId_Renamed;
	  protected internal string executionId_Renamed;
	  protected internal string processDefinitionId_Renamed;
	  protected internal string activityId_Renamed;
	  protected internal string activityName_Renamed;
	  protected internal string activityType_Renamed;
	  protected internal string assignee;
	  protected internal string tenantId;
	  protected internal string tenantIdLike;
	  protected internal bool withoutTenantId;
	  protected internal bool finished_Renamed;
	  protected internal bool unfinished_Renamed;

	  public HistoricActivityInstanceQueryImpl()
	  {
	  }

	  public HistoricActivityInstanceQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public HistoricActivityInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.HistoricActivityInstanceEntityManager.findHistoricActivityInstanceCountByQueryCriteria(this);
	  }

	  public override IList<HistoricActivityInstance> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.HistoricActivityInstanceEntityManager.findHistoricActivityInstancesByQueryCriteria(this, page);
	  }

	  public virtual HistoricActivityInstanceQueryImpl processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl executionId(string executionId)
	  {
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl activityId(string activityId)
	  {
		this.activityId_Renamed = activityId;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl activityName(string activityName)
	  {
		this.activityName_Renamed = activityName;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl activityType(string activityType)
	  {
		this.activityType_Renamed = activityType;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl taskAssignee(string assignee)
	  {
		this.assignee = assignee;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl finished()
	  {
		this.finished_Renamed = true;
		this.unfinished_Renamed = false;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl unfinished()
	  {
		this.unfinished_Renamed = true;
		this.finished_Renamed = false;
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl activityTenantId(string tenantId)
	  {
		  if (tenantId == null)
		  {
			  throw new ActivitiIllegalArgumentException("activity tenant id is null");
		  }
		  this.tenantId = tenantId;
		  return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl activityTenantIdLike(string tenantIdLike)
	  {
		  if (tenantIdLike == null)
		  {
			  throw new ActivitiIllegalArgumentException("activity tenant id is null");
		  }
		  this.tenantIdLike = tenantIdLike;
		  return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl activityWithoutTenantId()
	  {
		  this.withoutTenantId = true;
		  return this;
	  }


	  // ordering /////////////////////////////////////////////////////////////////

	  public virtual HistoricActivityInstanceQueryImpl orderByHistoricActivityInstanceDuration()
	  {
		orderBy(HistoricActivityInstanceQueryProperty.DURATION);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByHistoricActivityInstanceEndTime()
	  {
		orderBy(HistoricActivityInstanceQueryProperty.END);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByExecutionId()
	  {
		orderBy(HistoricActivityInstanceQueryProperty.EXECUTION_ID);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByHistoricActivityInstanceId()
	  {
		orderBy(HistoricActivityInstanceQueryProperty.HISTORIC_ACTIVITY_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByProcessDefinitionId()
	  {
		orderBy(HistoricActivityInstanceQueryProperty.PROCESS_DEFINITION_ID);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByProcessInstanceId()
	  {
		orderBy(HistoricActivityInstanceQueryProperty.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByHistoricActivityInstanceStartTime()
	  {
		orderBy(HistoricActivityInstanceQueryProperty.START);
		return this;
	  }

	  public virtual HistoricActivityInstanceQuery orderByActivityId()
	  {
		orderBy(HistoricActivityInstanceQueryProperty.ACTIVITY_ID);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByActivityName()
	  {
		orderBy(HistoricActivityInstanceQueryProperty.ACTIVITY_NAME);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByActivityType()
	  {
		orderBy(HistoricActivityInstanceQueryProperty.ACTIVITY_TYPE);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl orderByTenantId()
	  {
		  orderBy(HistoricActivityInstanceQueryProperty.TENANT_ID);
		return this;
	  }

	  public virtual HistoricActivityInstanceQueryImpl activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Renamed = activityInstanceId;
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
	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Renamed;
		  }
	  }
	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Renamed;
		  }
	  }
	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId_Renamed;
		  }
	  }
	  public virtual string ActivityName
	  {
		  get
		  {
			return activityName_Renamed;
		  }
	  }
	  public virtual string ActivityType
	  {
		  get
		  {
			return activityType_Renamed;
		  }
	  }
	  public virtual string Assignee
	  {
		  get
		  {
			return assignee;
		  }
	  }
	  public virtual bool Finished
	  {
		  get
		  {
			return finished_Renamed;
		  }
	  }
	  public virtual bool Unfinished
	  {
		  get
		  {
			return unfinished_Renamed;
		  }
	  }
	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId_Renamed;
		  }
	  }
	}

}