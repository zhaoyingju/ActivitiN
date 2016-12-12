using System;
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
namespace org.activiti.engine.impl.persistence.entity
{


	using Expression = org.activiti.engine.@delegate.Expression;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using ActivitiEventSupport = org.activiti.engine.@delegate.@event.impl.ActivitiEventSupport;
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using Context = org.activiti.engine.impl.context.Context;
	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using StartFormHandler = org.activiti.engine.impl.form.StartFormHandler;
	using Authentication = org.activiti.engine.impl.identity.Authentication;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.activiti.engine.impl.pvm.process.ProcessDefinitionImpl;
	using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;
	using TaskDefinition = org.activiti.engine.impl.task.TaskDefinition;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using IdentityLinkType = org.activiti.engine.task.IdentityLinkType;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class ProcessDefinitionEntity : ProcessDefinitionImpl, ProcessDefinition, PersistentObject, HasRevision
	{

	  private const long serialVersionUID = 1L;

	  protected internal new string key;
	  protected internal int revision = 1;
	  protected internal int version;
	  protected internal string category;
	  protected internal string deploymentId;
	  protected internal string resourceName;
	  protected internal string tenantId = ProcessEngineConfiguration.NO_TENANT_ID;
	  protected internal int? historyLevel;
	  protected internal StartFormHandler startFormHandler;
	  protected internal string diagramResourceName;
	  protected internal bool isGraphicalNotationDefined;
	  protected internal IDictionary<string, TaskDefinition> taskDefinitions;
	  protected internal IDictionary<string, object> variables;
	  protected internal bool hasStartFormKey_Renamed;
	  protected internal int suspensionState = SuspensionState_Fields.ACTIVE.StateCode;
	  protected internal bool isIdentityLinksInitialized = false;
	  protected internal IList<IdentityLinkEntity> definitionIdentityLinkEntities = new List<IdentityLinkEntity>();
	  protected internal Set<Expression> candidateStarterUserIdExpressions = new HashSet<Expression>();
	  protected internal Set<Expression> candidateStarterGroupIdExpressions = new HashSet<Expression>();
	  [NonSerialized]
	  protected internal ActivitiEventSupport eventSupport;

	  public ProcessDefinitionEntity() : base(null)
	  {
		eventSupport = new ActivitiEventSupport();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
	  private void readObject(ObjectInputStream @in)
	  {
		@in.defaultReadObject();
		eventSupport = new ActivitiEventSupport();

	  }

	  public virtual ExecutionEntity createProcessInstance(string businessKey, ActivityImpl initial)
	  {
		ExecutionEntity processInstance = null;

		if (initial == null)
		{
		  processInstance = (ExecutionEntity) base.createProcessInstance();
		}
		else
		{
		  processInstance = (ExecutionEntity) base.createProcessInstanceForInitial(initial);
		}

		processInstance.Executions = new List<ExecutionEntity>();
		processInstance.ProcessDefinition = processDefinition;
		// Do not initialize variable map (let it happen lazily)

		// Set business key (if any)
		if (businessKey != null)
		{
			processInstance.BusinessKey = businessKey;
		}

		// Inherit tenant id (if any)
		if (TenantId != null)
		{
			processInstance.TenantId = TenantId;
		}

		// Reset the process instance in order to have the db-generated process instance id available
		processInstance.setProcessInstance(processInstance);

		// initialize the template-defined data objects as variables first
		IDictionary<string, object> dataObjectVars = Variables;
		if (dataObjectVars != null)
		{
		  processInstance.Variables = dataObjectVars;
		}

		string authenticatedUserId = Authentication.AuthenticatedUserId;
		string initiatorVariableName = (string) getProperty(BpmnParse.PROPERTYNAME_INITIATOR_VARIABLE_NAME);
		if (initiatorVariableName != null)
		{
		  processInstance.setVariable(initiatorVariableName, authenticatedUserId);
		}
		if (authenticatedUserId != null)
		{
		  processInstance.addIdentityLink(authenticatedUserId, null, IdentityLinkType.STARTER);
		}

		Context.CommandContext.HistoryManager.recordProcessInstanceStart(processInstance);

		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, processInstance));
		}

		return processInstance;
	  }
	  public virtual ExecutionEntity createProcessInstance(string businessKey)
	  {
		return createProcessInstance(businessKey, null);
	  }

	  public override ExecutionEntity createProcessInstance()
	  {
		return createProcessInstance(null);
	  }


	  protected internal override InterpretableExecution newProcessInstance(ActivityImpl activityImpl)
	  {
		ExecutionEntity processInstance = new ExecutionEntity(activityImpl);
		processInstance.insert();
		return processInstance;
	  }

	  public virtual IdentityLinkEntity addIdentityLink(string userId, string groupId)
	  {
		IdentityLinkEntity identityLinkEntity = new IdentityLinkEntity();
		IdentityLinks.Add(identityLinkEntity);
		identityLinkEntity.setProcessDef(this);
		identityLinkEntity.UserId = userId;
		identityLinkEntity.GroupId = groupId;
		identityLinkEntity.Type = IdentityLinkType.CANDIDATE;
		identityLinkEntity.insert();
		return identityLinkEntity;
	  }

	  public virtual void deleteIdentityLink(string userId, string groupId)
	  {
		IList<IdentityLinkEntity> identityLinks = Context.CommandContext.IdentityLinkEntityManager.findIdentityLinkByProcessDefinitionUserAndGroup(id, userId, groupId);

		foreach (IdentityLinkEntity identityLink in identityLinks)
		{
		  Context.CommandContext.IdentityLinkEntityManager.deleteIdentityLink(identityLink, false);
		}
	  }

	  public virtual IList<IdentityLinkEntity> IdentityLinks
	  {
		  get
		  {
			if (!isIdentityLinksInitialized)
			{
			  definitionIdentityLinkEntities = Context.CommandContext.IdentityLinkEntityManager.findIdentityLinksByProcessDefinitionId(id);
			  isIdentityLinksInitialized = true;
			}
    
			return definitionIdentityLinkEntities;
		  }
	  }

	  public override string ToString()
	  {
		return "ProcessDefinitionEntity[" + id + "]";
	  }


	  // getters and setters //////////////////////////////////////////////////////

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["suspensionState"] = this.suspensionState;
			persistentState["category"] = this.category;
			return persistentState;
		  }
	  }

	  public override string Key
	  {
		  get
		  {
			return key;
		  }
		  set
		  {
			this.key = value;
		  }
	  }


	  public virtual string Description
	  {
		  set
		  {
			this.description = value;
		  }
		  get
		  {
			return description;
		  }
	  }


	  public override string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
		  }
	  }


	  public virtual int Version
	  {
		  get
		  {
			return version;
		  }
		  set
		  {
			this.version = value;
		  }
	  }


	  public virtual string Id
	  {
		  set
		  {
			this.id = value;
		  }
	  }

	  public virtual string ResourceName
	  {
		  get
		  {
			return resourceName;
		  }
		  set
		  {
			this.resourceName = value;
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


		public virtual int? HistoryLevel
		{
			get
			{
			return historyLevel;
			}
			set
			{
			this.historyLevel = value;
			}
		}


	  public virtual StartFormHandler StartFormHandler
	  {
		  get
		  {
			return startFormHandler;
		  }
		  set
		  {
			this.startFormHandler = value;
		  }
	  }


	  public virtual IDictionary<string, TaskDefinition> TaskDefinitions
	  {
		  get
		  {
			return taskDefinitions;
		  }
		  set
		  {
			this.taskDefinitions = value;
		  }
	  }


	  public virtual IDictionary<string, object> Variables
	  {
		  get
		  {
			return variables;
		  }
		  set
		  {
			this.variables = value;
		  }
	  }


	  public virtual string Category
	  {
		  get
		  {
			return category;
		  }
		  set
		  {
			this.category = value;
		  }
	  }


	  public override string DiagramResourceName
	  {
		  get
		  {
			return diagramResourceName;
		  }
		  set
		  {
			this.diagramResourceName = value;
		  }
	  }


	  public virtual bool hasStartFormKey()
	  {
		return hasStartFormKey_Renamed;
	  }

	  public virtual bool HasStartFormKey
	  {
		  get
		  {
			return hasStartFormKey_Renamed;
		  }
		  set
		  {
			this.hasStartFormKey_Renamed = value;
		  }
	  }

	  public virtual bool StartFormKey
	  {
		  set
		  {
			this.hasStartFormKey_Renamed = value;
		  }
	  }


	  public virtual bool GraphicalNotationDefined
	  {
		  get
		  {
			return isGraphicalNotationDefined;
		  }
		  set
		  {
			this.isGraphicalNotationDefined = value;
		  }
	  }

	  public virtual bool hasGraphicalNotation()
	  {
		  return isGraphicalNotationDefined;
	  }


	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public virtual int SuspensionState
	  {
		  get
		  {
			return suspensionState;
		  }
		  set
		  {
			this.suspensionState = value;
		  }
	  }


	  public virtual bool Suspended
	  {
		  get
		  {
			return suspensionState == SuspensionState_Fields.SUSPENDED.StateCode;
		  }
	  }

	  public virtual Set<Expression> CandidateStarterUserIdExpressions
	  {
		  get
		  {
			return candidateStarterUserIdExpressions;
		  }
	  }

	  public virtual void addCandidateStarterUserIdExpression(Expression userId)
	  {
		candidateStarterUserIdExpressions.add(userId);
	  }

	  public virtual Set<Expression> CandidateStarterGroupIdExpressions
	  {
		  get
		  {
			return candidateStarterGroupIdExpressions;
		  }
	  }

	  public virtual void addCandidateStarterGroupIdExpression(Expression groupId)
	  {
		candidateStarterGroupIdExpressions.add(groupId);
	  }

	  public virtual ActivitiEventSupport EventSupport
	  {
		  get
		  {
			  return eventSupport;
		  }
	  }
	}

}