using System.Collections.Generic;

/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace org.activiti.engine.impl.persistence.entity
{


	using HistoricProcessInstance = org.activiti.engine.history.HistoricProcessInstance;
	using Context = org.activiti.engine.impl.context.Context;
	using BulkDeleteable = org.activiti.engine.impl.db.BulkDeleteable;
	using Authentication = org.activiti.engine.impl.identity.Authentication;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Christian Stettler
	/// @author Joram Barrez
	/// </summary>
	public class HistoricProcessInstanceEntity : HistoricScopeInstanceEntity, HistoricProcessInstance, BulkDeleteable
	{

	  private const long serialVersionUID = 1L;

	  protected internal string endActivityId;
	  protected internal string businessKey;
	  protected internal string startUserId;
	  protected internal string startActivityId;
	  protected internal string superProcessInstanceId;
	  protected internal string tenantId = ProcessEngineConfiguration.NO_TENANT_ID;
	  protected internal string name;
	  protected internal string localizedName;
	  protected internal string description;
	  protected internal string localizedDescription;
	  protected internal IList<HistoricVariableInstanceEntity> queryVariables;

	  public HistoricProcessInstanceEntity()
	  {
	  }

	  public HistoricProcessInstanceEntity(ExecutionEntity processInstance)
	  {
		id = processInstance.Id;
		processInstanceId = processInstance.Id;
		businessKey = processInstance.BusinessKey;
		processDefinitionId = processInstance.ProcessDefinitionId;
		processDefinitionKey = processInstance.ProcessDefinitionKey;
		processDefinitionName = processInstance.ProcessDefinitionName;
		processDefinitionVersion = processInstance.ProcessDefinitionVersion;
		deploymentId = processInstance.DeploymentId;
		startTime = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		startUserId = Authentication.AuthenticatedUserId;
		startActivityId = processInstance.ActivityId;
		superProcessInstanceId = processInstance.getSuperExecution() != null ? processInstance.getSuperExecution().ProcessInstanceId : null;

		// Inherit tenant id (if applicable)
		if (processInstance.TenantId != null)
		{
			tenantId = processInstance.TenantId;
		}
	  }


	  public override object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = (IDictionary<string, object>) new Dictionary<string, object>();
			persistentState["endTime"] = endTime;
			persistentState["businessKey"] = businessKey;
			persistentState["name"] = name;
			persistentState["durationInMillis"] = durationInMillis;
			persistentState["deleteReason"] = deleteReason;
			persistentState["endStateName"] = endActivityId;
			persistentState["superProcessInstanceId"] = superProcessInstanceId;
			persistentState["processDefinitionId"] = processDefinitionId;
			persistentState["processDefinitionKey"] = processDefinitionKey;
			persistentState["processDefinitionName"] = processDefinitionName;
			persistentState["processDefinitionVersion"] = processDefinitionVersion;
			persistentState["deploymentId"] = deploymentId;
			return persistentState;
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////


	  public virtual string EndActivityId
	  {
		  get
		  {
			return endActivityId;
		  }
		  set
		  {
			this.endActivityId = value;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
		  set
		  {
			this.businessKey = value;
		  }
	  }

	  public virtual string StartUserId
	  {
		  get
		  {
			return startUserId;
		  }
		  set
		  {
			this.startUserId = value;
		  }
	  }

	  public virtual string StartActivityId
	  {
		  get
		  {
			return startActivityId;
		  }
		  set
		  {
			this.startActivityId = value;
		  }
	  }

	  public virtual string SuperProcessInstanceId
	  {
		  get
		  {
			return superProcessInstanceId;
		  }
		  set
		  {
			this.superProcessInstanceId = value;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
				return tenantId;
		  }
		  set
		  {
				this.tenantId = value;
		  }
	  }


		public virtual string Name
		{
			get
			{
			if (localizedName != null && localizedName.Length > 0)
			{
			  return localizedName;
			}
			else
			{
			  return name;
			}
			}
			set
			{
			this.name = value;
			}
		}


	  public virtual string LocalizedName
	  {
		  get
		  {
			return localizedName;
		  }
		  set
		  {
			this.localizedName = value;
		  }
	  }


	  public virtual string Description
	  {
		  get
		  {
			if (localizedDescription != null && localizedDescription.Length > 0)
			{
			  return localizedDescription;
			}
			else
			{
			  return description;
			}
		  }
		  set
		  {
			this.description = value;
		  }
	  }


	  public virtual string LocalizedDescription
	  {
		  get
		  {
			return localizedDescription;
		  }
		  set
		  {
			this.localizedDescription = value;
		  }
	  }


		public virtual IDictionary<string, object> ProcessVariables
		{
			get
			{
			IDictionary<string, object> variables = new Dictionary<string, object>();
			if (queryVariables != null)
			{
			  foreach (HistoricVariableInstanceEntity variableInstance in queryVariables)
			  {
				if (variableInstance.Id != null && variableInstance.TaskId == null)
				{
				  variables[variableInstance.Name] = variableInstance.Value;
				}
			  }
			}
			return variables;
			}
		}

	  public virtual IList<HistoricVariableInstanceEntity> QueryVariables
	  {
		  get
		  {
			if (queryVariables == null && Context.CommandContext != null)
			{
			  queryVariables = new HistoricVariableInitializingList();
			}
			return queryVariables;
		  }
		  set
		  {
			this.queryVariables = value;
		  }
	  }


	  // common methods  //////////////////////////////////////////////////////////

	  public override string ToString()
	  {
		return "HistoricProcessInstanceEntity[superProcessInstanceId=" + superProcessInstanceId + "]";
	  }
	}

}