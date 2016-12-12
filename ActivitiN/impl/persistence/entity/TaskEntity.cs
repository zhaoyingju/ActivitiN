using System;
using System.Collections;
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


	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using DelegateTask = org.activiti.engine.@delegate.DelegateTask;
	using TaskListener = org.activiti.engine.@delegate.TaskListener;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using Context = org.activiti.engine.impl.context.Context;
	using BulkDeleteable = org.activiti.engine.impl.db.BulkDeleteable;
	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using TaskListenerInvocation = org.activiti.engine.impl.@delegate.TaskListenerInvocation;
	using Authentication = org.activiti.engine.impl.identity.Authentication;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using TaskDefinition = org.activiti.engine.impl.task.TaskDefinition;
	using DelegationState = org.activiti.engine.task.DelegationState;
	using IdentityLink = org.activiti.engine.task.IdentityLink;
	using IdentityLinkType = org.activiti.engine.task.IdentityLinkType;
	using Task = org.activiti.engine.task.Task;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Falko Menge
	/// @author Tijs Rademakers
	/// </summary>
	[Serializable]
	public class TaskEntity : VariableScopeImpl, Task, DelegateTask, PersistentObject, HasRevision, BulkDeleteable
	{

	  public const string DELETE_REASON_COMPLETED = "completed";
	  public const string DELETE_REASON_DELETED = "deleted";

	  private const long serialVersionUID = 1L;

	  protected internal int revision;

	  protected internal string owner;
	  protected internal string assignee;
	  protected internal string initialAssignee;
	  protected internal DelegationState delegationState;

	  protected internal string parentTaskId;

	  protected internal string name;
	  protected internal string localizedName;
	  protected internal string description;
	  protected internal string localizedDescription;
	  protected internal int priority = org.activiti.engine.task.Task_Fields.DEFAULT_PRIORITY;
	  protected internal DateTime createTime; // The time when the task has been created
	  protected internal DateTime dueDate;
	  protected internal int suspensionState = SuspensionState_Fields.ACTIVE.StateCode;
	  protected internal string category;

	  protected internal bool isIdentityLinksInitialized = false;
	  protected internal IList<IdentityLinkEntity> taskIdentityLinkEntities = new List<IdentityLinkEntity>();

	  protected internal string executionId;
	  protected internal ExecutionEntity execution;

	  protected internal string processInstanceId;
	  protected internal ExecutionEntity processInstance;

	  protected internal string processDefinitionId;

	  protected internal TaskDefinition taskDefinition;
	  protected internal string taskDefinitionKey;
	  protected internal string formKey;

	  protected internal bool isDeleted;

	  protected internal string eventName;

	  protected internal string tenantId = ProcessEngineConfiguration.NO_TENANT_ID;

	  protected internal IList<VariableInstanceEntity> queryVariables;

	  protected internal bool forcedUpdate;

	  public TaskEntity()
	  {
	  }

	  public TaskEntity(string taskId)
	  {
		this.id = taskId;
	  }

	  /// <summary>
	  /// creates and initializes a new persistent task. </summary>
	  public static TaskEntity createAndInsert(ActivityExecution execution)
	  {
		TaskEntity task = create(Context.ProcessEngineConfiguration.Clock.CurrentTime);
		task.insert((ExecutionEntity) execution);
		return task;
	  }

	  public virtual void insert(ExecutionEntity execution)
	  {
		CommandContext commandContext = Context.CommandContext;
		DbSqlSession dbSqlSession = commandContext.DbSqlSession;
		dbSqlSession.insert(this);

		// Inherit tenant id (if applicable)
		if (execution != null && execution.TenantId != null)
		{
			TenantId = execution.TenantId;
		}

		if (execution != null)
		{
		  execution.addTask(this);
		}

		commandContext.HistoryManager.recordTaskCreated(this, execution);

		if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, this));
			commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_INITIALIZED, this));
		}
	  }

	  public virtual void update()
	  {
		// Needed to make history work: the setter will also update the historic task
		Owner = this.Owner;
		setAssignee(this.Assignee, true, false);
		DelegationState = this.DelegationState;
		Name = this.Name;
		Description = this.Description;
		Priority = this.Priority;
		Category = this.Category;
		CreateTime = this.CreateTime;
		DueDate = this.DueDate;
		ParentTaskId = this.ParentTaskId;
		FormKey = formKey;

		CommandContext commandContext = Context.CommandContext;
		DbSqlSession dbSqlSession = commandContext.DbSqlSession;
		dbSqlSession.update(this);

		if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, this));
		}
	  }

	  /// <summary>
	  ///  Creates a new task.  Embedded state and create time will be initialized.
	  /// But this task still will have to be persisted. See <seealso cref="#insert(ExecutionEntity))"/>. 
	  /// </summary>
	  public static TaskEntity create(DateTime createTime)
	  {
		TaskEntity task = new TaskEntity();
		task.isIdentityLinksInitialized = true;
		task.createTime = createTime;
		return task;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public void complete(java.util.Map variablesMap, boolean localScope)
	  public virtual void complete(IDictionary variablesMap, bool localScope)
	  {

		  if (DelegationState != null && DelegationState.Equals(DelegationState.PENDING))
		  {
			  throw new ActivitiException("A delegated task cannot be completed, but should be resolved instead.");
		  }

		fireEvent(org.activiti.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE);

		if (Authentication.AuthenticatedUserId != null && processInstanceId != null)
		{
		  getProcessInstance().involveUser(Authentication.AuthenticatedUserId, IdentityLinkType.PARTICIPANT);
		}

		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityWithVariablesEvent(ActivitiEventType.TASK_COMPLETED, this, variablesMap, localScope));
		}

		Context.CommandContext.TaskEntityManager.deleteTask(this, TaskEntity.DELETE_REASON_COMPLETED, false);

		if (executionId != null)
		{
		  ExecutionEntity execution = getExecution();
		  execution.removeTask(this);
		  execution.signal(null, null);
		}
	  }

	  public virtual void @delegate(string userId)
	  {
		DelegationState = DelegationState.PENDING;
		if (Owner == null)
		{
		  Owner = Assignee;
		}
		setAssignee(userId, true, true);
	  }

	  public virtual void resolve()
	  {
		DelegationState = DelegationState.RESOLVED;
		setAssignee(this.owner, true, true);
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["assignee"] = this.assignee;
			persistentState["owner"] = this.owner;
			persistentState["name"] = this.name;
			persistentState["priority"] = this.priority;
			if (executionId != null)
			{
			  persistentState["executionId"] = this.executionId;
			}
			if (processDefinitionId != null)
			{
			  persistentState["processDefinitionId"] = this.processDefinitionId;
			}
			if (createTime != null)
			{
			  persistentState["createTime"] = this.createTime;
			}
			if (description != null)
			{
			  persistentState["description"] = this.description;
			}
			if (dueDate != null)
			{
			  persistentState["dueDate"] = this.dueDate;
			}
			if (parentTaskId != null)
			{
			  persistentState["parentTaskId"] = this.parentTaskId;
			}
			if (delegationState != null)
			{
			  persistentState["delegationState"] = this.delegationState;
			}
    
			persistentState["suspensionState"] = this.suspensionState;
    
			if (forcedUpdate)
			{
			  persistentState["forcedUpdate"] = true;
			}
    
			return persistentState;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public virtual void forceUpdate()
	  {
		this.forcedUpdate = true;
	  }

	  // variables ////////////////////////////////////////////////////////////////

	  protected internal override VariableScopeImpl ParentVariableScope
	  {
		  get
		  {
			if (getExecution() != null)
			{
			  return execution;
			}
			return null;
		  }
	  }

	  protected internal override void initializeVariableInstanceBackPointer(VariableInstanceEntity variableInstance)
	  {
		variableInstance.TaskId = id;
		variableInstance.ExecutionId = executionId;
		variableInstance.ProcessInstanceId = processInstanceId;
	  }

	  protected internal override IList<VariableInstanceEntity> loadVariableInstances()
	  {
		return Context.CommandContext.VariableInstanceEntityManager.findVariableInstancesByTaskId(id);
	  }

	  protected internal override VariableInstanceEntity createVariableInstance(string variableName, object value, ExecutionEntity sourceActivityExecution)
	  {
		VariableInstanceEntity result = base.createVariableInstance(variableName, value, sourceActivityExecution);

		// Dispatch event, if needed
		if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createVariableEvent(ActivitiEventType.VARIABLE_CREATED, variableName, value, result.Type, result.TaskId, result.ExecutionId, ProcessInstanceId, ProcessDefinitionId));
		}
		return result;
	  }

	  protected internal override void updateVariableInstance(VariableInstanceEntity variableInstance, object value, ExecutionEntity sourceActivityExecution)
	  {
		base.updateVariableInstance(variableInstance, value, sourceActivityExecution);

		// Dispatch event, if needed
		if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createVariableEvent(ActivitiEventType.VARIABLE_UPDATED, variableInstance.Name, value, variableInstance.Type, variableInstance.TaskId, variableInstance.ExecutionId, ProcessInstanceId, ProcessDefinitionId));
		}
	  }

	  // execution ////////////////////////////////////////////////////////////////

	  public virtual ExecutionEntity getExecution()
	  {
		if ((execution == null) && (executionId != null))
		{
		  this.execution = Context.CommandContext.ExecutionEntityManager.findExecutionById(executionId);
		}
		return execution;
	  }

	  public virtual void setExecution(DelegateExecution execution)
	  {
		if (execution != null)
		{
		  this.execution = (ExecutionEntity) execution;
		  this.executionId = this.execution.Id;
		  this.processInstanceId = this.execution.ProcessInstanceId;
		  this.processDefinitionId = this.execution.ProcessDefinitionId;

		  Context.CommandContext.HistoryManager.recordTaskExecutionIdChange(this.id, executionId);

		}
		else
		{
		  this.execution = null;
		  this.executionId = null;
		  this.processInstanceId = null;
		  this.processDefinitionId = null;
		}
	  }

	  // task assignment //////////////////////////////////////////////////////////

	  public virtual IdentityLinkEntity addIdentityLink(string userId, string groupId, string type)
	  {
		IdentityLinkEntity identityLinkEntity = new IdentityLinkEntity();
		IdentityLinks.Add(identityLinkEntity);
		identityLinkEntity.setTask(this);
		identityLinkEntity.UserId = userId;
		identityLinkEntity.GroupId = groupId;
		identityLinkEntity.Type = type;
		identityLinkEntity.insert();
		if (userId != null && processInstanceId != null)
		{
		  getProcessInstance().involveUser(userId, IdentityLinkType.PARTICIPANT);
		}
		return identityLinkEntity;
	  }

	  public virtual void deleteIdentityLink(string userId, string groupId, string type)
	  {
		IList<IdentityLinkEntity> identityLinks = Context.CommandContext.IdentityLinkEntityManager.findIdentityLinkByTaskUserGroupAndType(id, userId, groupId, type);

		IList<string> identityLinkIds = new List<string>();
		foreach (IdentityLinkEntity identityLink in identityLinks)
		{
		  Context.CommandContext.IdentityLinkEntityManager.deleteIdentityLink(identityLink, true);
		  identityLinkIds.Add(identityLink.Id);
		}

		// fix deleteCandidate() in create TaskListener
		IList<IdentityLinkEntity> removedIdentityLinkEntities = new List<IdentityLinkEntity>();
		foreach (IdentityLinkEntity identityLinkEntity in this.IdentityLinks)
		{
		  if (IdentityLinkType.CANDIDATE.Equals(identityLinkEntity.Type) && identityLinkIds.Contains(identityLinkEntity.Id) == false)
		  {

			if ((userId != null && userId.Equals(identityLinkEntity.UserId)) || (groupId != null && groupId.Equals(identityLinkEntity.GroupId)))
			{

			  Context.CommandContext.IdentityLinkEntityManager.deleteIdentityLink(identityLinkEntity, true);
			  removedIdentityLinkEntities.Add(identityLinkEntity);
			}
		  }
		}
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		IdentityLinks.removeAll(removedIdentityLinkEntities);
	  }

	  public virtual Set<IdentityLink> Candidates
	  {
		  get
		  {
			Set<IdentityLink> potentialOwners = new HashSet<IdentityLink>();
			foreach (IdentityLinkEntity identityLinkEntity in IdentityLinks)
			{
			  if (IdentityLinkType.CANDIDATE.Equals(identityLinkEntity.Type))
			  {
				potentialOwners.add(identityLinkEntity);
			  }
			}
			return potentialOwners;
		  }
	  }

	  public virtual void addCandidateUser(string userId)
	  {
		addIdentityLink(userId, null, IdentityLinkType.CANDIDATE);
	  }

	  public virtual void addCandidateUsers(ICollection<string> candidateUsers)
	  {
		foreach (string candidateUser in candidateUsers)
		{
		  addCandidateUser(candidateUser);
		}
	  }

	  public virtual void addCandidateGroup(string groupId)
	  {
		addIdentityLink(null, groupId, IdentityLinkType.CANDIDATE);
	  }

	  public virtual void addCandidateGroups(ICollection<string> candidateGroups)
	  {
		foreach (string candidateGroup in candidateGroups)
		{
		  addCandidateGroup(candidateGroup);
		}
	  }

	  public virtual void addGroupIdentityLink(string groupId, string identityLinkType)
	  {
		addIdentityLink(null, groupId, identityLinkType);
	  }

	  public virtual void addUserIdentityLink(string userId, string identityLinkType)
	  {
		addIdentityLink(userId, null, identityLinkType);
	  }

	  public virtual void deleteCandidateGroup(string groupId)
	  {
		deleteGroupIdentityLink(groupId, IdentityLinkType.CANDIDATE);
	  }

	  public virtual void deleteCandidateUser(string userId)
	  {
		deleteUserIdentityLink(userId, IdentityLinkType.CANDIDATE);
	  }

	  public virtual void deleteGroupIdentityLink(string groupId, string identityLinkType)
	  {
		if (groupId != null)
		{
		  deleteIdentityLink(null, groupId, identityLinkType);
		}
	  }

	  public virtual void deleteUserIdentityLink(string userId, string identityLinkType)
	  {
		if (userId != null)
		{
		  deleteIdentityLink(userId, null, identityLinkType);
		}
	  }

	  public virtual IList<IdentityLinkEntity> IdentityLinks
	  {
		  get
		  {
			if (!isIdentityLinksInitialized)
			{
			  taskIdentityLinkEntities = Context.CommandContext.IdentityLinkEntityManager.findIdentityLinksByTaskId(id);
			  isIdentityLinksInitialized = true;
			}
    
			return taskIdentityLinkEntities;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Map<String, Object> getActivityInstanceVariables()
	  public virtual IDictionary<string, object> ActivityInstanceVariables
	  {
		  get
		  {
			if (execution != null)
			{
			  return execution.Variables;
			}
			return Collections.EMPTY_MAP;
		  }
	  }

	  public virtual IDictionary<string, object> ExecutionVariables
	  {
		  set
		  {
			if (getExecution() != null)
			{
			  execution.Variables = value;
			}
		  }
	  }

	  public override string ToString()
	  {
		return "Task[id=" + id + ", name=" + name + "]";
	  }

	  // special setters //////////////////////////////////////////////////////////

	  public virtual string Name
	  {
		  set
		  {
			this.name = value;
    
			CommandContext commandContext = Context.CommandContext;
			if (commandContext != null)
			{
			  commandContext.HistoryManager.recordTaskNameChange(id, value);
			}
		  }
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
	  }

	  /* plain setter for persistence */
	  public virtual string NameWithoutCascade
	  {
		  set
		  {
			this.name = value;
		  }
	  }

	  public virtual string LocalizedName
	  {
		  set
		  {
			this.localizedName = value;
		  }
	  }

	  public virtual string Description
	  {
		  set
		  {
			this.description = value;
    
			CommandContext commandContext = Context.CommandContext;
			if (commandContext != null)
			{
			  commandContext.HistoryManager.recordTaskDescriptionChange(id, value);
			}
		  }
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
	  }

	  /* plain setter for persistence */
	  public virtual string DescriptionWithoutCascade
	  {
		  set
		  {
			this.description = value;
		  }
	  }

	  public virtual string LocalizedDescription
	  {
		  set
		  {
			this.localizedDescription = value;
		  }
	  }

	  public virtual string Assignee
	  {
		  set
		  {
			setAssignee(value, false, false);
		  }
		  get
		  {
			return assignee;
		  }
	  }

	  public virtual void setAssignee(string assignee, bool dispatchAssignmentEvent, bool dispatchUpdateEvent)
	  {
		  CommandContext commandContext = Context.CommandContext;

		  if (assignee == null && this.assignee == null)
		  {

			  // ACT-1923: even if assignee is unmodified and null, this should be stored in history.
			  if (commandContext != null)
			  {
			commandContext.HistoryManager.recordTaskAssigneeChange(id, assignee);
			  }

		  return;
		  }
		this.assignee = assignee;

		// if there is no command context, then it means that the user is calling the 
		// setAssignee outside a service method.  E.g. while creating a new task.
		if (commandContext != null)
		{
		  commandContext.HistoryManager.recordTaskAssigneeChange(id, assignee);

		  if (assignee != null && processInstanceId != null)
		  {
			getProcessInstance().involveUser(assignee, IdentityLinkType.PARTICIPANT);
		  }

		  if (!StringUtils.Equals(initialAssignee, assignee))
		  {
			  fireEvent(org.activiti.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT);
			  initialAssignee = assignee;
		  }

		  if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
			  if (dispatchAssignmentEvent)
			  {
				  commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.TASK_ASSIGNED, this));
			  }

			  if (dispatchUpdateEvent)
			  {
				  commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, this));
			  }
		  }
		}
	  }

	  /* plain setter for persistence */
	  public virtual string AssigneeWithoutCascade
	  {
		  set
		  {
			this.assignee = value;
    
			// Assign the value that was persisted before
			this.initialAssignee = value;
		  }
	  }

	  public virtual string Owner
	  {
		  set
		  {
			setOwner(value, false);
		  }
		  get
		  {
			return owner;
		  }
	  }

	  public virtual void setOwner(string owner, bool dispatchUpdateEvent)
	  {
		  if (owner == null && this.owner == null)
		  {
		  return;
		  }
	//    if (owner!=null && owner.equals(this.owner)) {
	//      return;
	//    }
		this.owner = owner;

		CommandContext commandContext = Context.CommandContext;
		if (commandContext != null)
		{
		  commandContext.HistoryManager.recordTaskOwnerChange(id, owner);

		  if (owner != null && processInstanceId != null)
		  {
			getProcessInstance().involveUser(owner, IdentityLinkType.PARTICIPANT);
		  }

		  if (dispatchUpdateEvent && commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
			  if (dispatchUpdateEvent)
			  {
				  commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, this));
			  }
		  }
		}
	  }

	  /* plain setter for persistence */
	  public virtual string OwnerWithoutCascade
	  {
		  set
		  {
			this.owner = value;
		  }
	  }

	  public virtual DateTime DueDate
	  {
		  set
		  {
		   setDueDate(value, false);
		  }
		  get
		  {
			return dueDate;
		  }
	  }

	  public virtual void setDueDate(DateTime dueDate, bool dispatchUpdateEvent)
	  {
		   this.dueDate = dueDate;

		 CommandContext commandContext = Context.CommandContext;
		 if (commandContext != null)
		 {
		   commandContext.HistoryManager.recordTaskDueDateChange(id, dueDate);

		   if (dispatchUpdateEvent && commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
		   {
			   if (dispatchUpdateEvent)
			   {
				   commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, this));
			   }
		   }
		 }
	  }

	  public virtual DateTime DueDateWithoutCascade
	  {
		  set
		  {
			this.dueDate = value;
		  }
	  }

	  public virtual int Priority
	  {
		  set
		  {
			  setPriority(value, false);
		  }
		  get
		  {
			return priority;
		  }
	  }

	  public virtual void setPriority(int priority, bool dispatchUpdateEvent)
	  {
		this.priority = priority;

		CommandContext commandContext = Context.CommandContext;
		if (commandContext != null)
		{
		  commandContext.HistoryManager.recordTaskPriorityChange(id, priority);

		  if (dispatchUpdateEvent && commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
			  if (dispatchUpdateEvent)
			  {
				  commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, this));
			  }
		  }
		}
	  }

	  public virtual string CategoryWithoutCascade
	  {
		  set
		  {
			  this.category = value;
		  }
	  }

	  public virtual string Category
	  {
		  set
		  {
			  this.category = value;
    
			CommandContext commandContext = Context.CommandContext;
			if (commandContext != null)
			{
			  commandContext.HistoryManager.recordTaskCategoryChange(id, value);
			}
		  }
		  get
		  {
				return category;
		  }
	  }

	  public virtual int PriorityWithoutCascade
	  {
		  set
		  {
			this.priority = value;
		  }
	  }

	  public virtual string ParentTaskId
	  {
		  set
		  {
			this.parentTaskId = value;
    
			CommandContext commandContext = Context.CommandContext;
			if (commandContext != null)
			{
			  commandContext.HistoryManager.recordTaskParentTaskIdChange(id, value);
			}
		  }
		  get
		  {
			return parentTaskId;
		  }
	  }

	  public virtual string ParentTaskIdWithoutCascade
	  {
		  set
		  {
			this.parentTaskId = value;
		  }
	  }

	  public virtual string TaskDefinitionKeyWithoutCascade
	  {
		  set
		  {
			   this.taskDefinitionKey = value;
		  }
	  }

	  public virtual string FormKey
	  {
		  get
		  {
				return formKey;
		  }
		  set
		  {
				this.formKey = value;
    
			  CommandContext commandContext = Context.CommandContext;
			if (commandContext != null)
			{
			  commandContext.HistoryManager.recordTaskFormKeyChange(id, value);
			}
		  }
	  }


		public virtual string FormKeyWithoutCascade
		{
			set
			{
				this.formKey = value;
			}
		}

		public virtual void fireEvent(string taskEventName)
		{
		TaskDefinition taskDefinition = TaskDefinition;
		if (taskDefinition != null)
		{
		  IList<TaskListener> taskEventListeners = TaskDefinition.getTaskListener(taskEventName);
		  if (taskEventListeners != null)
		  {
			foreach (TaskListener taskListener in taskEventListeners)
			{
			  ExecutionEntity execution = getExecution();
			  if (execution != null)
			  {
				EventName = taskEventName;
			  }
			  try
			  {
				Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new TaskListenerInvocation(taskListener, (DelegateTask)this));
			  }
			  catch (Exception e)
			  {
				throw new ActivitiException("Exception while invoking TaskListener: " + e.Message, e);
			  }
			}
		  }
		}
		}

	  protected internal override bool ActivityIdUsedForDetails
	  {
		  get
		  {
			return false;
		  }
	  }

	  // Override from VariableScopeImpl

	  // Overriden to avoid fetching *all* variables (as is the case in the super call)
	  protected internal override VariableInstanceEntity getSpecificVariable(string variableName)
	  {
			CommandContext commandContext = Context.CommandContext;
			if (commandContext == null)
			{
				throw new ActivitiException("lazy loading outside command context");
			}
			VariableInstanceEntity variableInstance = commandContext.VariableInstanceEntityManager.findVariableInstanceByTaskAndName(id, variableName);

			return variableInstance;
	  }

	  protected internal override IList<VariableInstanceEntity> getSpecificVariables(ICollection<string> variableNames)
	  {
		  CommandContext commandContext = Context.CommandContext;
			if (commandContext == null)
			{
				throw new ActivitiException("lazy loading outside command context");
			}
			return commandContext.VariableInstanceEntityManager.findVariableInstancesByTaskAndNames(id, variableNames);
	  }

	  // modified getters and setters /////////////////////////////////////////////

	  public virtual TaskDefinition TaskDefinition
	  {
		  set
		  {
			this.taskDefinition = value;
			this.taskDefinitionKey = value.Key;
    
			CommandContext commandContext = Context.CommandContext;
			if (commandContext != null)
			{
			  commandContext.HistoryManager.recordTaskDefinitionKeyChange(this, taskDefinitionKey);
			}
		  }
		  get
		  {
			if (taskDefinition == null && taskDefinitionKey != null)
			{
			  ProcessDefinitionEntity processDefinition = Context.ProcessEngineConfiguration.DeploymentManager.findDeployedProcessDefinitionById(processDefinitionId);
			  taskDefinition = processDefinition.TaskDefinitions[taskDefinitionKey];
			}
			return taskDefinition;
		  }
	  }


	  // getters and setters //////////////////////////////////////////////////////

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






	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime;
		  }
		  set
		  {
			this.createTime = value;
		  }
	  }


	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
		  set
		  {
			this.executionId = value;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }



	  public virtual string TaskDefinitionKey
	  {
		  get
		  {
			return taskDefinitionKey;
		  }
		  set
		  {
			this.taskDefinitionKey = value;
    
			CommandContext commandContext = Context.CommandContext;
			if (commandContext != null)
			{
			  commandContext.HistoryManager.recordTaskDefinitionKeyChange(this, value);
			}
		  }
	  }


	  public virtual string EventName
	  {
		  get
		  {
			return eventName;
		  }
		  set
		  {
			this.eventName = value;
		  }
	  }
	  public virtual ExecutionEntity getProcessInstance()
	  {
		if (processInstance == null && processInstanceId != null)
		{
		  processInstance = Context.CommandContext.ExecutionEntityManager.findExecutionById(processInstanceId);
		}
		return processInstance;
	  }
	  public virtual void setProcessInstance(ExecutionEntity processInstance)
	  {
		this.processInstance = processInstance;
	  }
	  public virtual void setExecution(ExecutionEntity execution)
	  {
		this.execution = execution;
	  }
	  public virtual DelegationState DelegationState
	  {
		  get
		  {
			return delegationState;
		  }
		  set
		  {
			this.delegationState = value;
		  }
	  }
	  public virtual string DelegationStateString
	  {
		  get
		  {
			return (delegationState != null ? delegationState.ToString() : null);
		  }
		  set
		  {
			this.delegationState = (value != null ? DelegationState.valueOf(typeof(DelegationState), value) : null);
		  }
	  }
	  public virtual bool Deleted
	  {
		  get
		  {
			return isDeleted;
		  }
		  set
		  {
			this.isDeleted = value;
		  }
	  }
	  public override IDictionary<string, VariableInstanceEntity> VariableInstanceEntities
	  {
		  get
		  {
			ensureVariableInstancesInitialized();
			return variableInstances;
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
	  public virtual IDictionary<string, object> TaskLocalVariables
	  {
		  get
		  {
			IDictionary<string, object> variables = new Dictionary<string, object>();
			if (queryVariables != null)
			{
			  foreach (VariableInstanceEntity variableInstance in queryVariables)
			  {
				if (variableInstance.Id != null && variableInstance.TaskId != null)
				{
				  variables[variableInstance.Name] = variableInstance.Value;
				}
			  }
			}
			return variables;
		  }
	  }
	  public virtual IDictionary<string, object> ProcessVariables
	  {
		  get
		  {
			IDictionary<string, object> variables = new Dictionary<string, object>();
			if (queryVariables != null)
			{
			  foreach (VariableInstanceEntity variableInstance in queryVariables)
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
		public virtual IList<VariableInstanceEntity> QueryVariables
		{
			get
			{
			if (queryVariables == null && Context.CommandContext != null)
			{
			  queryVariables = new VariableInitializingList();
			}
			return queryVariables;
			}
			set
			{
			this.queryVariables = value;
			}
		}


	}

}