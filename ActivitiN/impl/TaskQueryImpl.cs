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
namespace org.activiti.engine.impl
{


	using Group = org.activiti.engine.identity.Group;
	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using SuspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState;
	using VariableTypes = org.activiti.engine.impl.variable.VariableTypes;
	using DelegationState = org.activiti.engine.task.DelegationState;
	using Task = org.activiti.engine.task.Task;
	using TaskQuery = org.activiti.engine.task.TaskQuery;

	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;

	/// <summary>
	/// @author Joram Barrez
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// @author Tijs Rademakers
	/// </summary>
	public class TaskQueryImpl : AbstractVariableQueryImpl<TaskQuery, Task>, TaskQuery
	{

	  private const long serialVersionUID = 1L;

	  protected internal string taskId_Renamed;
	  protected internal string name;
	  protected internal string nameLike;
	  protected internal string nameLikeIgnoreCase;
	  protected internal IList<string> nameList;
	  protected internal IList<string> nameListIgnoreCase;
	  protected internal string description;
	  protected internal string descriptionLike;
	  protected internal string descriptionLikeIgnoreCase;
	  protected internal int? priority;
	  protected internal int? minPriority;
	  protected internal int? maxPriority;
	  protected internal string assignee;
	  protected internal string assigneeLike;
	  protected internal string assigneeLikeIgnoreCase;
	  protected internal string involvedUser;
	  protected internal string owner;
	  protected internal string ownerLike;
	  protected internal string ownerLikeIgnoreCase;
	  protected internal bool unassigned = false;
	  protected internal bool noDelegationState = false;
	  protected internal DelegationState delegationState;
	  protected internal string candidateUser;
	  protected internal string candidateGroup;
	  protected internal IList<string> candidateGroups;
	  protected internal string tenantId;
	  protected internal string tenantIdLike;
	  protected internal bool withoutTenantId;
	  protected internal string processInstanceId_Renamed;
	  protected internal IList<string> processInstanceIds;
	  protected internal string executionId_Renamed;
	  protected internal DateTime createTime;
	  protected internal DateTime createTimeBefore;
	  protected internal DateTime createTimeAfter;
	  protected internal string category;
	  protected internal string key;
	  protected internal string keyLike;
	  protected internal string processDefinitionKey_Renamed;
	  protected internal string processDefinitionKeyLike_Renamed;
	  protected internal string processDefinitionKeyLikeIgnoreCase_Renamed;
	  protected internal IList<string> processDefinitionKeys;
	  protected internal string processDefinitionId_Renamed;
	  protected internal string processDefinitionName_Renamed;
	  protected internal string processDefinitionNameLike_Renamed;
	  protected internal IList<string> processCategoryInList;
	  protected internal IList<string> processCategoryNotInList;
	  protected internal string deploymentId_Renamed;
	  protected internal IList<string> deploymentIds;
	  protected internal string processInstanceBusinessKey_Renamed;
	  protected internal string processInstanceBusinessKeyLike_Renamed;
	  protected internal string processInstanceBusinessKeyLikeIgnoreCase_Renamed;
	  protected internal DateTime dueDate_Renamed;
	  protected internal DateTime dueBefore_Renamed;
	  protected internal DateTime dueAfter_Renamed;
	  protected internal bool withoutDueDate_Renamed = false;
	  protected internal SuspensionState suspensionState;
	  protected internal bool excludeSubtasks_Renamed = false;
	  protected internal bool includeTaskLocalVariables_Renamed = false;
	  protected internal bool includeProcessVariables_Renamed = false;
	  protected internal int? taskVariablesLimit;
	  protected internal string userIdForCandidateAndAssignee;
	  protected internal bool bothCandidateAndAssigned = false;
	  protected internal string locale_Renamed;
	  protected internal bool withLocalizationFallback_Renamed;
	  protected internal bool orActive;
	  protected internal IList<TaskQueryImpl> orQueryObjects = new List<TaskQueryImpl>();
	  protected internal TaskQueryImpl currentOrQueryObject = null;

	  public TaskQueryImpl()
	  {
	  }

	  public TaskQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public TaskQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public TaskQueryImpl(CommandExecutor commandExecutor, string databaseType) : base(commandExecutor)
	  {
		this.databaseType = databaseType;
	  }

	  public virtual TaskQueryImpl taskId(string taskId)
	  {
		if (taskId == null)
		{
		  throw new ActivitiIllegalArgumentException("Task id is null");
		}

		if (orActive)
		{
		  currentOrQueryObject.taskId_Renamed = taskId;
		}
		else
		{
		  this.taskId_Renamed = taskId;
		}
		return this;
	  }

	  public virtual TaskQueryImpl taskName(string name)
	  {
		if (name == null)
		{
		  throw new ActivitiIllegalArgumentException("Task name is null");
		}

		if (orActive)
		{
		  currentOrQueryObject.name = name;
		}
		else
		{
		  this.name = name;
		}
		return this;
	  }

	  public override TaskQuery taskNameIn(IList<string> nameList)
	  {
		if (nameList == null)
		{
		  throw new ActivitiIllegalArgumentException("Task name list is null");
		}
		if (nameList.Count == 0)
		{
		  throw new ActivitiIllegalArgumentException("Task name list is empty");
		}
		foreach (string name in nameList)
		{
		  if (name == null)
		  {
			throw new ActivitiIllegalArgumentException("None of the given task names can be null");
		  }
		}

		if (name != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both taskNameIn and name");
		}
		if (nameLike != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both taskNameIn and nameLike");
		}
		if (nameLikeIgnoreCase != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both taskNameIn and nameLikeIgnoreCase");
		}

		if (orActive)
		{
		  currentOrQueryObject.nameList = nameList;
		}
		else
		{
		  this.nameList = nameList;
		}
		return this;
	  }

	  public override TaskQuery taskNameInIgnoreCase(IList<string> nameList)
	  {
		if (nameList == null)
		{
		  throw new ActivitiIllegalArgumentException("Task name list is null");
		}
		if (nameList.Count == 0)
		{
		  throw new ActivitiIllegalArgumentException("Task name list is empty");
		}
		foreach (string name in nameList)
		{
		  if (name == null)
		  {
			throw new ActivitiIllegalArgumentException("None of the given task names can be null");
		  }
		}

		if (name != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both taskNameInIgnoreCase and name");
		}
		if (nameLike != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both taskNameInIgnoreCase and nameLike");
		}
		if (nameLikeIgnoreCase != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both taskNameInIgnoreCase and nameLikeIgnoreCase");
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nameListSize = nameList.size();
		int nameListSize = nameList.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> caseIgnoredNameList = new java.util.ArrayList<String>(nameListSize);
		IList<string> caseIgnoredNameList = new List<string>(nameListSize);
		foreach (string name in nameList)
		{
		  caseIgnoredNameList.Add(name.ToLower());
		}

		if (orActive)
		{
		  this.currentOrQueryObject.nameListIgnoreCase = caseIgnoredNameList;
		}
		else
		{
		  this.nameListIgnoreCase = caseIgnoredNameList;
		}
		return this;
	  }

	  public virtual TaskQueryImpl taskNameLike(string nameLike)
	  {
		if (nameLike == null)
		{
		  throw new ActivitiIllegalArgumentException("Task namelike is null");
		}

		if (orActive)
		{
		  currentOrQueryObject.nameLike = nameLike;
		}
		else
		{
		  this.nameLike = nameLike;
		}
		return this;
	  }

	  public virtual TaskQuery taskNameLikeIgnoreCase(string nameLikeIgnoreCase)
	  {
		   if (nameLikeIgnoreCase == null)
		   {
		   throw new ActivitiIllegalArgumentException("Task nameLikeIgnoreCase is null");
		   }

		 if (orActive)
		 {
		   currentOrQueryObject.nameLikeIgnoreCase = nameLikeIgnoreCase.ToLower();
		 }
		 else
		 {
		   this.nameLikeIgnoreCase = nameLikeIgnoreCase.ToLower();
		 }
		 return this;
	  }

	  public virtual TaskQueryImpl taskDescription(string description)
	  {
		if (description == null)
		{
		  throw new ActivitiIllegalArgumentException("Description is null");
		}

		if (orActive)
		{
		  currentOrQueryObject.description = description;
		}
		else
		{
		  this.description = description;
		}
		return this;
	  }

	  public virtual TaskQuery taskDescriptionLike(string descriptionLike)
	  {
		if (descriptionLike == null)
		{
		  throw new ActivitiIllegalArgumentException("Task descriptionlike is null");
		}
		if (orActive)
		{
		  currentOrQueryObject.descriptionLike = descriptionLike;
		}
		else
		{
		  this.descriptionLike = descriptionLike;
		}
		return this;
	  }

	  public virtual TaskQuery taskDescriptionLikeIgnoreCase(string descriptionLikeIgnoreCase)
	  {
		  if (descriptionLikeIgnoreCase == null)
		  {
		  throw new ActivitiIllegalArgumentException("Task descriptionLikeIgnoreCase is null");
		  }
		if (orActive)
		{
		  currentOrQueryObject.descriptionLikeIgnoreCase = descriptionLikeIgnoreCase.ToLower();
		}
		else
		{
		  this.descriptionLikeIgnoreCase = descriptionLikeIgnoreCase.ToLower();
		}
		return this;
	  }

	  public virtual TaskQuery taskPriority(int? priority)
	  {
		if (priority == null)
		{
		  throw new ActivitiIllegalArgumentException("Priority is null");
		}
		if (orActive)
		{
		  currentOrQueryObject.priority = priority;
		}
		else
		{
		  this.priority = priority;
		}
		return this;
	  }

	  public virtual TaskQuery taskMinPriority(int? minPriority)
	  {
		if (minPriority == null)
		{
		  throw new ActivitiIllegalArgumentException("Min Priority is null");
		}
		if (orActive)
		{
		  currentOrQueryObject.minPriority = minPriority;
		}
		else
		{
		  this.minPriority = minPriority;
		}
		return this;
	  }

	  public virtual TaskQuery taskMaxPriority(int? maxPriority)
	  {
		if (maxPriority == null)
		{
		  throw new ActivitiIllegalArgumentException("Max Priority is null");
		}
		if (orActive)
		{
		  currentOrQueryObject.maxPriority = maxPriority;
		}
		else
		{
		  this.maxPriority = maxPriority;
		}
		return this;
	  }

	  public virtual TaskQueryImpl taskAssignee(string assignee)
	  {
		if (assignee == null)
		{
		  throw new ActivitiIllegalArgumentException("Assignee is null");
		}
		if (orActive)
		{
		  currentOrQueryObject.assignee = assignee;
		}
		else
		{
		  this.assignee = assignee;
		}
		return this;
	  }

	  public virtual TaskQueryImpl taskAssigneeLike(string assigneeLike)
	  {
		if (assigneeLike == null)
		{
		  throw new ActivitiIllegalArgumentException("AssigneeLike is null");
		}
		if (orActive)
		{
		  currentOrQueryObject.assigneeLike = assignee;
		}
		else
		{
		  this.assigneeLike = assigneeLike;
		}
		return this;
	  }

	  public virtual TaskQuery taskAssigneeLikeIgnoreCase(string assigneeLikeIgnoreCase)
	  {
		   if (assigneeLikeIgnoreCase == null)
		   {
		   throw new ActivitiIllegalArgumentException("assigneeLikeIgnoreCase is null");
		   }
		 if (orActive)
		 {
		   currentOrQueryObject.assigneeLikeIgnoreCase = assigneeLikeIgnoreCase.ToLower();
		 }
		 else
		 {
		   this.assigneeLikeIgnoreCase = assigneeLikeIgnoreCase.ToLower();
		 }
		 return this;
	  }

	  public virtual TaskQueryImpl taskOwner(string owner)
	  {
		if (owner == null)
		{
		  throw new ActivitiIllegalArgumentException("Owner is null");
		}
		if (orActive)
		{
		  currentOrQueryObject.owner = owner;
		}
		else
		{
		  this.owner = owner;
		}
		return this;
	  }

	  public virtual TaskQueryImpl taskOwnerLike(string ownerLike)
	  {
		if (ownerLike == null)
		{
		  throw new ActivitiIllegalArgumentException("Owner is null");
		}
		if (orActive)
		{
		  currentOrQueryObject.ownerLike = ownerLike;
		}
		else
		{
		  this.ownerLike = ownerLike;
		}
		return this;
	  }

	  public virtual TaskQuery taskOwnerLikeIgnoreCase(string ownerLikeIgnoreCase)
	  {
		  if (ownerLikeIgnoreCase == null)
		  {
		  throw new ActivitiIllegalArgumentException("OwnerLikeIgnoreCase");
		  }
		if (orActive)
		{
		  currentOrQueryObject.ownerLikeIgnoreCase = ownerLikeIgnoreCase.ToLower();
		}
		else
		{
		  this.ownerLikeIgnoreCase = ownerLikeIgnoreCase.ToLower();
		}
		return this;
	  }

	  /// <seealso cref= <seealso cref="#taskUnassigned"/> </seealso>
	  [Obsolete]
	  public virtual TaskQuery taskUnnassigned()
	  {
		return taskUnassigned();
	  }

	  public virtual TaskQuery taskUnassigned()
	  {
		if (orActive)
		{
		  currentOrQueryObject.unassigned = true;
		}
		else
		{
		  this.unassigned = true;
		}
		return this;
	  }

	  public virtual TaskQuery taskDelegationState(DelegationState delegationState)
	  {
		if (orActive)
		{
		  if (delegationState == null)
		  {
			currentOrQueryObject.noDelegationState = true;
		  }
		  else
		  {
			currentOrQueryObject.delegationState = delegationState;
		  }
		}
		else
		{
		  if (delegationState == null)
		  {
			this.noDelegationState = true;
		  }
		  else
		  {
			this.delegationState = delegationState;
		  }
		}
		return this;
	  }

	  public virtual TaskQueryImpl taskCandidateUser(string candidateUser)
	  {
		if (candidateUser == null)
		{
		  throw new ActivitiIllegalArgumentException("Candidate user is null");
		}

		if (orActive)
		{
		  currentOrQueryObject.candidateUser = candidateUser;
		}
		else
		{
		  this.candidateUser = candidateUser;
		}

		return this;
	  }

	  public virtual TaskQueryImpl taskInvolvedUser(string involvedUser)
	  {
		if (involvedUser == null)
		{
		  throw new ActivitiIllegalArgumentException("Involved user is null");
		}
		if (orActive)
		{
		  currentOrQueryObject.involvedUser = involvedUser;
		}
		else
		{
		  this.involvedUser = involvedUser;
		}
		return this;
	  }

	  public virtual TaskQueryImpl taskCandidateGroup(string candidateGroup)
	  {
		if (candidateGroup == null)
		{
		  throw new ActivitiIllegalArgumentException("Candidate group is null");
		}

		if (candidateGroups != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both candidateGroup and candidateGroupIn");
		}

		if (orActive)
		{
		  currentOrQueryObject.candidateGroup = candidateGroup;
		}
		else
		{
		  this.candidateGroup = candidateGroup;
		}
		return this;
	  }

	  public override TaskQuery taskCandidateOrAssigned(string userIdForCandidateAndAssignee)
	  {
		if (candidateGroup != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set candidateGroup");
		}
		if (candidateUser != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both candidateGroup and candidateUser");
		}

		if (orActive)
		{
		  currentOrQueryObject.bothCandidateAndAssigned = true;
		  currentOrQueryObject.userIdForCandidateAndAssignee = userIdForCandidateAndAssignee;
		}
		else
		{
		  this.bothCandidateAndAssigned = true;
		  this.userIdForCandidateAndAssignee = userIdForCandidateAndAssignee;
		}

		return this;
	  }

	  public virtual TaskQuery taskCandidateGroupIn(IList<string> candidateGroups)
	  {
		if (candidateGroups == null)
		{
		  throw new ActivitiIllegalArgumentException("Candidate group list is null");
		}

		if (candidateGroups.Count == 0)
		{
		  throw new ActivitiIllegalArgumentException("Candidate group list is empty");
		}

		if (candidateGroup != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both candidateGroupIn and candidateGroup");
		}

		if (orActive)
		{
		  currentOrQueryObject.candidateGroups = candidateGroups;
		}
		else
		{
		  this.candidateGroups = candidateGroups;
		}
		return this;
	  }

	  public virtual TaskQuery taskTenantId(string tenantId)
	  {
		  if (tenantId == null)
		  {
			  throw new ActivitiIllegalArgumentException("task tenant id is null");
		  }
		   if (orActive)
		   {
		   currentOrQueryObject.tenantId = tenantId;
		   }
		 else
		 {
		   this.tenantId = tenantId;
		 }
		  return this;
	  }

	  public virtual TaskQuery taskTenantIdLike(string tenantIdLike)
	  {
		  if (tenantIdLike == null)
		  {
			  throw new ActivitiIllegalArgumentException("task tenant id is null");
		  }
		  if (orActive)
		  {
		  currentOrQueryObject.tenantIdLike = tenantIdLike;
		  }
		else
		{
		  this.tenantIdLike = tenantIdLike;
		}
		  return this;
	  }

	  public virtual TaskQuery taskWithoutTenantId()
	  {
		if (orActive)
		{
		  currentOrQueryObject.withoutTenantId = true;
		}
		else
		{
		  this.withoutTenantId = true;
		}
		  return this;
	  }

	  public virtual TaskQueryImpl processInstanceId(string processInstanceId)
	  {
		if (orActive)
		{
		  currentOrQueryObject.processInstanceId_Renamed = processInstanceId;
		}
		else
		{
		  this.processInstanceId_Renamed = processInstanceId;
		}
		return this;
	  }

	  public override TaskQuery processInstanceIdIn(IList<string> processInstanceIds)
	  {
		if (processInstanceIds == null)
		{
		  throw new ActivitiIllegalArgumentException("Process instance id list is null");
		}
		if (processInstanceIds.Count == 0)
		{
		  throw new ActivitiIllegalArgumentException("Process instance id list is empty");
		}
		foreach (string processInstanceId in processInstanceIds)
		{
		  if (processInstanceId == null)
		  {
			throw new ActivitiIllegalArgumentException("None of the given process instance ids can be null");
		  }
		}

		if (orActive)
		{
		  currentOrQueryObject.processInstanceIds = processInstanceIds;
		}
		else
		{
		  this.processInstanceIds = processInstanceIds;
		}
		return this;
	  }

	  public virtual TaskQueryImpl processInstanceBusinessKey(string processInstanceBusinessKey)
	  {
		if (orActive)
		{
		  currentOrQueryObject.processInstanceBusinessKey_Renamed = processInstanceBusinessKey;
		}
		else
		{
		  this.processInstanceBusinessKey_Renamed = processInstanceBusinessKey;
		}
		return this;
	  }

	  public virtual TaskQueryImpl processInstanceBusinessKeyLike(string processInstanceBusinessKeyLike)
	  {
		if (orActive)
		{
		  currentOrQueryObject.processInstanceBusinessKeyLike_Renamed = processInstanceBusinessKeyLike;
		}
		else
		{
		  this.processInstanceBusinessKeyLike_Renamed = processInstanceBusinessKeyLike;
		}
		return this;
	  }

	  public virtual TaskQuery processInstanceBusinessKeyLikeIgnoreCase(string processInstanceBusinessKeyLikeIgnoreCase)
	  {
		   if (orActive)
		   {
		   currentOrQueryObject.processInstanceBusinessKeyLikeIgnoreCase_Renamed = processInstanceBusinessKeyLikeIgnoreCase.ToLower();
		   }
		 else
		 {
		   this.processInstanceBusinessKeyLikeIgnoreCase_Renamed = processInstanceBusinessKeyLikeIgnoreCase.ToLower();
		 }
		 return this;
	  }

	  public virtual TaskQueryImpl executionId(string executionId)
	  {
		if (orActive)
		{
		  currentOrQueryObject.executionId_Renamed = executionId;
		}
		else
		{
		  this.executionId_Renamed = executionId;
		}
		return this;
	  }

	  public virtual TaskQueryImpl taskCreatedOn(DateTime createTime)
	  {
		if (orActive)
		{
		  currentOrQueryObject.createTime = createTime;
		}
		else
		{
		  this.createTime = createTime;
		}
		return this;
	  }

	  public virtual TaskQuery taskCreatedBefore(DateTime before)
	  {
		if (orActive)
		{
		  currentOrQueryObject.createTimeBefore = before;
		}
		else
		{
		  this.createTimeBefore = before;
		}
		return this;
	  }

	  public virtual TaskQuery taskCreatedAfter(DateTime after)
	  {
		if (orActive)
		{
		  currentOrQueryObject.createTimeAfter = after;
		}
		else
		{
		  this.createTimeAfter = after;
		}
		return this;
	  }

	  public virtual TaskQuery taskCategory(string category)
	  {
		if (orActive)
		{
		  currentOrQueryObject.category = category;
		}
		else
		{
		  this.category = category;
		}
		  return this;
	  }

	  public virtual TaskQuery taskDefinitionKey(string key)
	  {
		if (orActive)
		{
		  currentOrQueryObject.key = key;
		}
		else
		{
		  this.key = key;
		}
		return this;
	  }

	  public virtual TaskQuery taskDefinitionKeyLike(string keyLike)
	  {
		if (orActive)
		{
		  currentOrQueryObject.keyLike = keyLike;
		}
		else
		{
		  this.keyLike = keyLike;
		}
		return this;
	  }

	  public virtual TaskQuery taskVariableValueEquals(string variableName, object variableValue)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueEquals(variableName, variableValue);
		}
		else
		{
		  this.variableValueEquals(variableName, variableValue);
		}
		return this;
	  }

	  public virtual TaskQuery taskVariableValueEquals(object variableValue)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueEquals(variableValue);
		}
		else
		{
		  this.variableValueEquals(variableValue);
		}
		return this;
	  }

	  public virtual TaskQuery taskVariableValueEqualsIgnoreCase(string name, string value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueEqualsIgnoreCase(name, value);
		}
		else
		{
		  this.variableValueEqualsIgnoreCase(name, value);
		}
		return this;
	  }

	  public virtual TaskQuery taskVariableValueNotEqualsIgnoreCase(string name, string value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueNotEqualsIgnoreCase(name, value);
		}
		else
		{
		  this.variableValueNotEqualsIgnoreCase(name, value);
		}
		return this;
	  }

	  public virtual TaskQuery taskVariableValueNotEquals(string variableName, object variableValue)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueNotEquals(variableName, variableValue);
		}
		else
		{
		  this.variableValueNotEquals(variableName, variableValue);
		}
		return this;
	  }

	  public virtual TaskQuery taskVariableValueGreaterThan(string name, object value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueGreaterThan(name, value);
		}
		else
		{
		  this.variableValueGreaterThan(name, value);
		}
		return this;
	  }

	  public virtual TaskQuery taskVariableValueGreaterThanOrEqual(string name, object value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueGreaterThanOrEqual(name, value);
		}
		else
		{
		  this.variableValueGreaterThanOrEqual(name, value);
		}
		return this;
	  }

	  public virtual TaskQuery taskVariableValueLessThan(string name, object value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueLessThan(name, value);
		}
		else
		{
		  this.variableValueLessThan(name, value);
		}
		return this;
	  }

	  public virtual TaskQuery taskVariableValueLessThanOrEqual(string name, object value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueLessThanOrEqual(name, value);
		}
		else
		{
		  this.variableValueLessThanOrEqual(name, value);
		}
		return this;
	  }

	  public virtual TaskQuery taskVariableValueLike(string name, string value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueLike(name, value);
		}
		else
		{
		  this.variableValueLike(name, value);
		}
		return this;
	  }

	  public virtual TaskQuery taskVariableValueLikeIgnoreCase(string name, string value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueLikeIgnoreCase(name, value);
		}
		else
		{
		  this.variableValueLikeIgnoreCase(name, value);
		}
		return this;
	  }

	  public virtual TaskQuery processVariableValueEquals(string variableName, object variableValue)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueEquals(variableName, variableValue, false);
		}
		else
		{
		  this.variableValueEquals(variableName, variableValue, false);
		}
		return this;
	  }

	  public virtual TaskQuery processVariableValueNotEquals(string variableName, object variableValue)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueNotEquals(variableName, variableValue, false);
		}
		else
		{
		  this.variableValueNotEquals(variableName, variableValue, false);
		}
		return this;
	  }

	  public virtual TaskQuery processVariableValueEquals(object variableValue)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueEquals(variableValue, false);
		}
		else
		{
		  this.variableValueEquals(variableValue, false);
		}
		return this;
	  }

	  public virtual TaskQuery processVariableValueEqualsIgnoreCase(string name, string value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueEqualsIgnoreCase(name, value, false);
		}
		else
		{
		  this.variableValueEqualsIgnoreCase(name, value, false);
		}
		return this;
	  }

	  public virtual TaskQuery processVariableValueNotEqualsIgnoreCase(string name, string value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueNotEqualsIgnoreCase(name, value, false);
		}
		else
		{
		  this.variableValueNotEqualsIgnoreCase(name, value, false);
		}
		return this;
	  }

	  public virtual TaskQuery processVariableValueGreaterThan(string name, object value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueGreaterThan(name, value, false);
		}
		else
		{
		  this.variableValueGreaterThan(name, value, false);
		}
		return this;
	  }

	  public virtual TaskQuery processVariableValueGreaterThanOrEqual(string name, object value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueGreaterThanOrEqual(name, value, false);
		}
		else
		{
		  this.variableValueGreaterThanOrEqual(name, value, false);
		}
		return this;
	  }

	  public virtual TaskQuery processVariableValueLessThan(string name, object value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueLessThan(name, value, false);
		}
		else
		{
		  this.variableValueLessThan(name, value, false);
		}
		return this;
	  }

	  public virtual TaskQuery processVariableValueLessThanOrEqual(string name, object value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueLessThanOrEqual(name, value, false);
		}
		else
		{
		  this.variableValueLessThanOrEqual(name, value, false);
		}
		return this;
	  }

	  public virtual TaskQuery processVariableValueLike(string name, string value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueLike(name, value, false);
		}
		else
		{
		  this.variableValueLike(name, value, false);
		}
		return this;
	  }

	  public virtual TaskQuery processVariableValueLikeIgnoreCase(string name, string value)
	  {
		if (orActive)
		{
		  currentOrQueryObject.variableValueLikeIgnoreCase(name, value, false);
		}
		else
		{
		  this.variableValueLikeIgnoreCase(name, value, false);
		}
		return this;
	  }

	  public virtual TaskQuery processDefinitionKey(string processDefinitionKey)
	  {
		if (orActive)
		{
		  currentOrQueryObject.processDefinitionKey_Renamed = processDefinitionKey;
		}
		else
		{
		  this.processDefinitionKey_Renamed = processDefinitionKey;
		}
		return this;
	  }

	  public virtual TaskQuery processDefinitionKeyLike(string processDefinitionKeyLike)
	  {
		if (orActive)
		{
		  currentOrQueryObject.processDefinitionKeyLike_Renamed = processDefinitionKeyLike;
		}
		else
		{
		  this.processDefinitionKeyLike_Renamed = processDefinitionKeyLike;
		}
		return this;
	  }

	  public virtual TaskQuery processDefinitionKeyLikeIgnoreCase(string processDefinitionKeyLikeIgnoreCase)
	  {
		  if (orActive)
		  {
		  currentOrQueryObject.processDefinitionKeyLikeIgnoreCase_Renamed = processDefinitionKeyLikeIgnoreCase.ToLower();
		  }
		else
		{
		  this.processDefinitionKeyLikeIgnoreCase_Renamed = processDefinitionKeyLikeIgnoreCase.ToLower();
		}
		return this;
	  }

	  public virtual TaskQuery processDefinitionKeyIn(IList<string> processDefinitionKeys)
	  {
		if (orActive)
		{
		  this.currentOrQueryObject.processDefinitionKeys = processDefinitionKeys;
		}
		else
		{
		  this.processDefinitionKeys = processDefinitionKeys;
		}
		return this;
	  }

	  public virtual TaskQuery processDefinitionId(string processDefinitionId)
	  {
		if (orActive)
		{
		  currentOrQueryObject.processDefinitionId_Renamed = processDefinitionId;
		}
		else
		{
		  this.processDefinitionId_Renamed = processDefinitionId;
		}
		return this;
	  }

	  public virtual TaskQuery processDefinitionName(string processDefinitionName)
	  {
		if (orActive)
		{
		  currentOrQueryObject.processDefinitionName_Renamed = processDefinitionName;
		}
		else
		{
		  this.processDefinitionName_Renamed = processDefinitionName;
		}
		return this;
	  }

	  public virtual TaskQuery processDefinitionNameLike(string processDefinitionNameLike)
	  {
		if (orActive)
		{
		  currentOrQueryObject.processDefinitionNameLike_Renamed = processDefinitionNameLike;
		}
		else
		{
		  this.processDefinitionNameLike_Renamed = processDefinitionNameLike;
		}
		return this;
	  }

	  public override TaskQuery processCategoryIn(IList<string> processCategoryInList)
	  {
		if (processCategoryInList == null)
		{
		  throw new ActivitiIllegalArgumentException("Process category list is null");
		}
		if (processCategoryInList.Count == 0)
		{
		  throw new ActivitiIllegalArgumentException("Process category list is empty");
		}
		foreach (string processCategory in processCategoryInList)
		{
		  if (processCategory == null)
		  {
			throw new ActivitiIllegalArgumentException("None of the given process categories can be null");
		  }
		}

		if (orActive)
		{
		  currentOrQueryObject.processCategoryInList = processCategoryInList;
		}
		else
		{
		  this.processCategoryInList = processCategoryInList;
		}
		return this;
	  }

	  public override TaskQuery processCategoryNotIn(IList<string> processCategoryNotInList)
	  {
		if (processCategoryNotInList == null)
		{
		  throw new ActivitiIllegalArgumentException("Process category list is null");
		}
		if (processCategoryNotInList.Count == 0)
		{
		  throw new ActivitiIllegalArgumentException("Process category list is empty");
		}
		foreach (string processCategory in processCategoryNotInList)
		{
		  if (processCategory == null)
		  {
			throw new ActivitiIllegalArgumentException("None of the given process categories can be null");
		  }
		}

		if (orActive)
		{
		  currentOrQueryObject.processCategoryNotInList = processCategoryNotInList;
		}
		else
		{
		  this.processCategoryNotInList = processCategoryNotInList;
		}
		return this;
	  }

	  public virtual TaskQuery deploymentId(string deploymentId)
	  {
		if (orActive)
		{
		  currentOrQueryObject.deploymentId_Renamed = deploymentId;
		}
		else
		{
		  this.deploymentId_Renamed = deploymentId;
		}
		return this;
	  }

	  public virtual TaskQuery deploymentIdIn(IList<string> deploymentIds)
	  {
		if (orActive)
		{
		  currentOrQueryObject.deploymentIds = deploymentIds;
		}
		else
		{
		  this.deploymentIds = deploymentIds;
		}
		return this;
	  }

	  public virtual TaskQuery dueDate(DateTime dueDate)
	  {
		if (orActive)
		{
		  currentOrQueryObject.dueDate_Renamed = dueDate;
		  currentOrQueryObject.withoutDueDate_Renamed = false;
		}
		else
		{
		  this.dueDate_Renamed = dueDate;
		  this.withoutDueDate_Renamed = false;
		}
		return this;
	  }

	  public override TaskQuery taskDueDate(DateTime dueDate)
	  {
		  return dueDate(dueDate);
	  }

	  public virtual TaskQuery dueBefore(DateTime dueBefore)
	  {
		if (orActive)
		{
		  currentOrQueryObject.dueBefore_Renamed = dueBefore;
		  currentOrQueryObject.withoutDueDate_Renamed = false;
		}
		else
		{
		  this.dueBefore_Renamed = dueBefore;
		  this.withoutDueDate_Renamed = false;
		}
		return this;
	  }

	  public override TaskQuery taskDueBefore(DateTime dueDate)
	  {
		  return dueBefore(dueDate);
	  }

	  public virtual TaskQuery dueAfter(DateTime dueAfter)
	  {
		if (orActive)
		{
		  currentOrQueryObject.dueAfter_Renamed = dueAfter;
		  currentOrQueryObject.withoutDueDate_Renamed = false;
		}
		else
		{
		  this.dueAfter_Renamed = dueAfter;
		  this.withoutDueDate_Renamed = false;
		}
		return this;
	  }

	  public virtual TaskQuery taskDueAfter(DateTime dueDate)
	  {
		return dueAfter(dueDate);
	  }

	  public virtual TaskQuery withoutDueDate()
	  {
		if (orActive)
		{
		  currentOrQueryObject.withoutDueDate_Renamed = true;
		}
		else
		{
		  this.withoutDueDate_Renamed = true;
		}
		return this;
	  }

	  public override TaskQuery withoutTaskDueDate()
	  {
		  return withoutDueDate();
	  }

	  public virtual TaskQuery excludeSubtasks()
	  {
		if (orActive)
		{
		  currentOrQueryObject.excludeSubtasks_Renamed = true;
		}
		else
		{
		  this.excludeSubtasks_Renamed = true;
		}
		return this;
	  }

	  public virtual TaskQuery suspended()
	  {
		if (orActive)
		{
		  currentOrQueryObject.suspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		}
		else
		{
		  this.suspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		}
		return this;
	  }

	  public virtual TaskQuery active()
	  {
		if (orActive)
		{
		  currentOrQueryObject.suspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		}
		else
		{
		  this.suspensionState = org.activiti.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE;
		}
		return this;
	  }

	  public virtual TaskQuery locale(string locale)
	  {
		this.locale_Renamed = locale;
		return this;
	  }

	  public virtual TaskQuery withLocalizationFallback()
	  {
		withLocalizationFallback_Renamed = true;
		return this;
	  }

	  public virtual TaskQuery includeTaskLocalVariables()
	  {
		this.includeTaskLocalVariables_Renamed = true;
		return this;
	  }

	  public virtual TaskQuery includeProcessVariables()
	  {
		this.includeProcessVariables_Renamed = true;
		return this;
	  }

	  public virtual TaskQuery limitTaskVariables(int? taskVariablesLimit)
	  {
		this.taskVariablesLimit = taskVariablesLimit;
		return this;
	  }

	  public virtual int? TaskVariablesLimit
	  {
		  get
		  {
			return taskVariablesLimit;
		  }
	  }

	  public virtual IList<string> CandidateGroups
	  {
		  get
		  {
			if (candidateGroup != null)
			{
			  IList<string> candidateGroupList = new List<string>(1);
			  candidateGroupList.Add(candidateGroup);
			  return candidateGroupList;
    
			}
			else if (candidateGroups != null)
			{
			  return candidateGroups;
    
			}
			else if (candidateUser != null)
			{
			  return getGroupsForCandidateUser(candidateUser);
    
			}
			else if (userIdForCandidateAndAssignee != null)
			{
			  return getGroupsForCandidateUser(userIdForCandidateAndAssignee);
			}
			return null;
		  }
	  }

	  protected internal virtual IList<string> getGroupsForCandidateUser(string candidateUser)
	  {
		// TODO: Discuss about removing this feature? Or document it properly and maybe recommend to not use it
		// and explain alternatives
		IList<Group> groups = Context.CommandContext.GroupIdentityManager.findGroupsByUser(candidateUser);
		IList<string> groupIds = new List<string>();
		foreach (Group group in groups)
		{
		  groupIds.Add(group.Id);
		}
		return groupIds;
	  }

	  protected internal virtual void ensureVariablesInitialized()
	  {
		VariableTypes types = Context.ProcessEngineConfiguration.VariableTypes;
		foreach (QueryVariableValue @var in queryVariableValues)
		{
		  @var.initialize(types);
		}

		foreach (TaskQueryImpl orQueryObject in orQueryObjects)
		{
		  orQueryObject.ensureVariablesInitialized();
		}
	  }

	  //or query ////////////////////////////////////////////////////////////////

	  public override TaskQuery or()
	  {
		if (orActive)
		{
			throw new ActivitiException("the query is already in an or statement");
		}

		// Create instance of the orQuery
		orActive = true;
		currentOrQueryObject = new TaskQueryImpl();
		orQueryObjects.Add(currentOrQueryObject);
		return this;
	  }

	  public override TaskQuery endOr()
	  {
		if (!orActive)
		{
		  throw new ActivitiException("endOr() can only be called after calling or()");
		}

		orActive = false;
		currentOrQueryObject = null;
		return this;
	  }


	  //ordering ////////////////////////////////////////////////////////////////

	  public virtual TaskQuery orderByTaskId()
	  {
		return orderBy(TaskQueryProperty.TASK_ID);
	  }

	  public virtual TaskQuery orderByTaskName()
	  {
		return orderBy(TaskQueryProperty.NAME);
	  }

	  public virtual TaskQuery orderByTaskDescription()
	  {
		return orderBy(TaskQueryProperty.DESCRIPTION);
	  }

	  public virtual TaskQuery orderByTaskPriority()
	  {
		return orderBy(TaskQueryProperty.PRIORITY);
	  }

	  public virtual TaskQuery orderByProcessInstanceId()
	  {
		return orderBy(TaskQueryProperty.PROCESS_INSTANCE_ID);
	  }

	  public virtual TaskQuery orderByExecutionId()
	  {
		return orderBy(TaskQueryProperty.EXECUTION_ID);
	  }

	  public override TaskQuery orderByProcessDefinitionId()
	  {
		  return orderBy(TaskQueryProperty.PROCESS_DEFINITION_ID);
	  }

	  public virtual TaskQuery orderByTaskAssignee()
	  {
		return orderBy(TaskQueryProperty.ASSIGNEE);
	  }

	  public override TaskQuery orderByTaskOwner()
	  {
		  return orderBy(TaskQueryProperty.OWNER);
	  }

	  public virtual TaskQuery orderByTaskCreateTime()
	  {
		return orderBy(TaskQueryProperty.CREATE_TIME);
	  }

	  public virtual TaskQuery orderByDueDate()
	  {
		return orderBy(TaskQueryProperty.DUE_DATE);
	  }

	  public override TaskQuery orderByTaskDueDate()
	  {
		  return orderByDueDate();
	  }

	  public override TaskQuery orderByTaskDefinitionKey()
	  {
		  return orderBy(TaskQueryProperty.TASK_DEFINITION_KEY);
	  }

	  public virtual TaskQuery orderByDueDateNullsFirst()
	  {
		  return orderBy(TaskQueryProperty.DUE_DATE, NullHandlingOnOrder.NULLS_FIRST);
	  }

	  public override TaskQuery orderByDueDateNullsLast()
	  {
		  return orderBy(TaskQueryProperty.DUE_DATE, NullHandlingOnOrder.NULLS_LAST);
	  }

	  public override TaskQuery orderByTenantId()
	  {
		  return orderBy(TaskQueryProperty.TENANT_ID);
	  }

	  public virtual string MssqlOrDB2OrderBy
	  {
		  get
		  {
			string specialOrderBy = base.OrderBy;
			if (specialOrderBy != null && specialOrderBy.Length > 0)
			{
			  specialOrderBy = specialOrderBy.Replace("RES.", "TEMPRES_");
			}
			return specialOrderBy;
		  }
	  }

	  //results ////////////////////////////////////////////////////////////////

	  public virtual IList<Task> executeList(CommandContext commandContext, Page page)
	  {
		ensureVariablesInitialized();
		checkQueryOk();
		IList<Task> tasks = null;
		if (includeTaskLocalVariables_Renamed || includeProcessVariables_Renamed)
		{
		  tasks = commandContext.TaskEntityManager.findTasksAndVariablesByQueryCriteria(this);
		}
		else
		{
		  tasks = commandContext.TaskEntityManager.findTasksByQueryCriteria(this);
		}

		if (tasks != null)
		{
		  foreach (Task task in tasks)
		  {
			localize(task);
		  }
		}

		return tasks;
	  }

	  public virtual long executeCount(CommandContext commandContext)
	  {
		ensureVariablesInitialized();
		checkQueryOk();
		return commandContext.TaskEntityManager.findTaskCountByQueryCriteria(this);
	  }

	  protected internal virtual void localize(Task task)
	  {
		task.LocalizedName = null;
		task.LocalizedDescription = null;

		if (locale_Renamed != null)
		{
		  string processDefinitionId = task.ProcessDefinitionId;
		  if (processDefinitionId != null)
		  {
			ObjectNode languageNode = Context.getLocalizationElementProperties(locale_Renamed, task.TaskDefinitionKey, processDefinitionId, withLocalizationFallback_Renamed);
			if (languageNode != null)
			{
			  JsonNode languageNameNode = languageNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_NAME);
			  if (languageNameNode != null && languageNameNode.Null == false)
			  {
				task.LocalizedName = languageNameNode.asText();
			  }

			  JsonNode languageDescriptionNode = languageNode.get(org.activiti.engine.DynamicBpmnConstants_Fields.LOCALIZATION_DESCRIPTION);
			  if (languageDescriptionNode != null && languageDescriptionNode.Null == false)
			  {
				task.LocalizedDescription = languageDescriptionNode.asText();
			  }
			}
		  }
		}
	  }

	  //getters ////////////////////////////////////////////////////////////////

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }
	  public virtual string NameLike
	  {
		  get
		  {
			return nameLike;
		  }
	  }
	  public virtual IList<string> NameList
	  {
		  get
		  {
			return nameList;
		  }
	  }
	  public virtual IList<string> NameListIgnoreCase
	  {
		  get
		  {
			return nameListIgnoreCase;
		  }
	  }
	  public virtual string Assignee
	  {
		  get
		  {
			return assignee;
		  }
	  }
	  public virtual bool Unassigned
	  {
		  get
		  {
			return unassigned;
		  }
	  }
	  public virtual DelegationState DelegationState
	  {
		  get
		  {
			return delegationState;
		  }
	  }
	  public virtual bool NoDelegationState
	  {
		  get
		  {
			return noDelegationState;
		  }
	  }
	  public virtual string DelegationStateString
	  {
		  get
		  {
			return (delegationState != null ? delegationState.ToString() : null);
		  }
	  }
	  public virtual string CandidateUser
	  {
		  get
		  {
			return candidateUser;
		  }
	  }
	  public virtual string CandidateGroup
	  {
		  get
		  {
			return candidateGroup;
		  }
	  }
	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Renamed;
		  }
	  }
	  public virtual IList<string> ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds;
		  }
	  }
	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Renamed;
		  }
	  }
	  public virtual string TaskId
	  {
		  get
		  {
			return taskId_Renamed;
		  }
	  }
	  public virtual string Description
	  {
		  get
		  {
			return description;
		  }
	  }
	  public virtual string DescriptionLike
	  {
		  get
		  {
			return descriptionLike;
		  }
	  }
	  public virtual int? Priority
	  {
		  get
		  {
			return priority;
		  }
	  }
	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime;
		  }
	  }
	  public virtual DateTime CreateTimeBefore
	  {
		  get
		  {
			return createTimeBefore;
		  }
	  }
	  public virtual DateTime CreateTimeAfter
	  {
		  get
		  {
			return createTimeAfter;
		  }
	  }
	  public virtual string Key
	  {
		  get
		  {
			return key;
		  }
	  }
	  public virtual string KeyLike
	  {
		  get
		  {
			return keyLike;
		  }
	  }
	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Renamed;
		  }
	  }
	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Renamed;
		  }
	  }
	  public virtual string ProcessDefinitionName
	  {
		  get
		  {
			return processDefinitionName_Renamed;
		  }
	  }
	  public virtual string ProcessInstanceBusinessKey
	  {
		  get
		  {
			return processInstanceBusinessKey_Renamed;
		  }
	  }
	  public virtual bool ExcludeSubtasks
	  {
		  get
		  {
			return excludeSubtasks_Renamed;
		  }
	  }
	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }
	  public virtual string TenantIdLike
	  {
		  get
		  {
			return tenantIdLike;
		  }
	  }
	  public virtual bool WithoutTenantId
	  {
		  get
		  {
			return withoutTenantId;
		  }
	  }
	  public virtual string UserIdForCandidateAndAssignee
	  {
		  get
		  {
			return userIdForCandidateAndAssignee;
		  }
	  }

	  public virtual IList<TaskQueryImpl> OrQueryObjects
	  {
		  get
		  {
			return orQueryObjects;
		  }
		  set
		  {
			this.orQueryObjects = value;
		  }
	  }


	  public virtual int? MinPriority
	  {
		  get
		  {
			return minPriority;
		  }
	  }

	  public virtual int? MaxPriority
	  {
		  get
		  {
			return maxPriority;
		  }
	  }

	  public virtual string AssigneeLike
	  {
		  get
		  {
			return assigneeLike;
		  }
	  }

	  public virtual string InvolvedUser
	  {
		  get
		  {
			return involvedUser;
		  }
	  }

	  public virtual string Owner
	  {
		  get
		  {
			return owner;
		  }
	  }

	  public virtual string OwnerLike
	  {
		  get
		  {
			return ownerLike;
		  }
	  }

	  public virtual string Category
	  {
		  get
		  {
			return category;
		  }
	  }

	  public virtual string ProcessDefinitionKeyLike
	  {
		  get
		  {
			return processDefinitionKeyLike_Renamed;
		  }
	  }

	  public virtual IList<string> ProcessDefinitionKeys
	  {
		  get
		  {
			return processDefinitionKeys;
		  }
	  }

	  public virtual string ProcessDefinitionNameLike
	  {
		  get
		  {
			return processDefinitionNameLike_Renamed;
		  }
	  }

	  public virtual IList<string> ProcessCategoryInList
	  {
		  get
		  {
			return processCategoryInList;
		  }
	  }

	  public virtual IList<string> ProcessCategoryNotInList
	  {
		  get
		  {
			return processCategoryNotInList;
		  }
	  }

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId_Renamed;
		  }
	  }

	  public virtual IList<string> DeploymentIds
	  {
		  get
		  {
			return deploymentIds;
		  }
	  }

	  public virtual string ProcessInstanceBusinessKeyLike
	  {
		  get
		  {
			return processInstanceBusinessKeyLike_Renamed;
		  }
	  }

	  public virtual DateTime DueDate
	  {
		  get
		  {
			return dueDate_Renamed;
		  }
	  }

	  public virtual DateTime DueBefore
	  {
		  get
		  {
			return dueBefore_Renamed;
		  }
	  }

	  public virtual DateTime DueAfter
	  {
		  get
		  {
			return dueAfter_Renamed;
		  }
	  }

	  public virtual bool WithoutDueDate
	  {
		  get
		  {
			return withoutDueDate_Renamed;
		  }
	  }

	  public virtual SuspensionState SuspensionState
	  {
		  get
		  {
			return suspensionState;
		  }
	  }

	  public virtual bool IncludeTaskLocalVariables
	  {
		  get
		  {
			return includeTaskLocalVariables_Renamed;
		  }
	  }

	  public virtual bool IncludeProcessVariables
	  {
		  get
		  {
			return includeProcessVariables_Renamed;
		  }
	  }

	  public virtual bool BothCandidateAndAssigned
	  {
		  get
		  {
			return bothCandidateAndAssigned;
		  }
	  }

	  public virtual string NameLikeIgnoreCase
	  {
		  get
		  {
				return nameLikeIgnoreCase;
		  }
	  }

		public virtual string DescriptionLikeIgnoreCase
		{
			get
			{
				return descriptionLikeIgnoreCase;
			}
		}

		public virtual string AssigneeLikeIgnoreCase
		{
			get
			{
				return assigneeLikeIgnoreCase;
			}
		}

		public virtual string OwnerLikeIgnoreCase
		{
			get
			{
				return ownerLikeIgnoreCase;
			}
		}

		public virtual string ProcessInstanceBusinessKeyLikeIgnoreCase
		{
			get
			{
				return processInstanceBusinessKeyLikeIgnoreCase_Renamed;
			}
		}

		public virtual string ProcessDefinitionKeyLikeIgnoreCase
		{
			get
			{
				return processDefinitionKeyLikeIgnoreCase_Renamed;
			}
		}

		public virtual bool OrActive
		{
			get
			{
			return orActive;
			}
		}

	}

}