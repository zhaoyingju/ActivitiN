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


	using HistoricTaskInstance = org.activiti.engine.history.HistoricTaskInstance;
	using HistoricTaskInstanceQuery = org.activiti.engine.history.HistoricTaskInstanceQuery;
	using Group = org.activiti.engine.identity.Group;
	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using VariableTypes = org.activiti.engine.impl.variable.VariableTypes;

	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class HistoricTaskInstanceQueryImpl : AbstractVariableQueryImpl<HistoricTaskInstanceQuery, HistoricTaskInstance>, HistoricTaskInstanceQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string processDefinitionId_Renamed;
	  protected internal string processDefinitionKey_Renamed;
	  protected internal string processDefinitionKeyLike_Renamed;
	  protected internal string processDefinitionKeyLikeIgnoreCase_Renamed;
	  protected internal IList<string> processDefinitionKeys;
	  protected internal string processDefinitionName_Renamed;
	  protected internal string processDefinitionNameLike_Renamed;
	  protected internal IList<string> processCategoryInList;
	  protected internal IList<string> processCategoryNotInList;
	  protected internal string deploymentId_Renamed;
	  protected internal IList<string> deploymentIds;
	  protected internal string processInstanceId_Renamed;
	  protected internal IList<string> processInstanceIds;
	  protected internal string processInstanceBusinessKey_Renamed;
	  protected internal string processInstanceBusinessKeyLike_Renamed;
	  protected internal string processInstanceBusinessKeyLikeIgnoreCase_Renamed;
	  protected internal string executionId_Renamed;
	  protected internal string taskId_Renamed;
	  protected internal string taskName_Renamed;
	  protected internal string taskNameLike_Renamed;
	  protected internal string taskNameLikeIgnoreCase_Renamed;
	  protected internal IList<string> taskNameList;
	  protected internal IList<string> taskNameListIgnoreCase;
	  protected internal string taskParentTaskId_Renamed;
	  protected internal string taskDescription_Renamed;
	  protected internal string taskDescriptionLike_Renamed;
	  protected internal string taskDescriptionLikeIgnoreCase_Renamed;
	  protected internal string taskDeleteReason_Renamed;
	  protected internal string taskDeleteReasonLike_Renamed;
	  protected internal string taskOwner_Renamed;
	  protected internal string taskOwnerLike_Renamed;
	  protected internal string taskOwnerLikeIgnoreCase_Renamed;
	  protected internal string taskAssignee_Renamed;
	  protected internal string taskAssigneeLike_Renamed;
	  protected internal string taskAssigneeLikeIgnoreCase_Renamed;
	  protected internal string taskDefinitionKey_Renamed;
	  protected internal string taskDefinitionKeyLike_Renamed;
	  protected internal string candidateUser;
	  protected internal string candidateGroup;
	  private IList<string> candidateGroups;
	  protected internal string involvedUser;
	  protected internal int? taskPriority_Renamed;
	  protected internal int? taskMinPriority_Renamed;
	  protected internal int? taskMaxPriority_Renamed;
	  protected internal bool finished_Renamed;
	  protected internal bool unfinished_Renamed;
	  protected internal bool processFinished_Renamed;
	  protected internal bool processUnfinished_Renamed;
	  protected internal DateTime dueDate_Renamed;
	  protected internal DateTime dueAfter_Renamed;
	  protected internal DateTime dueBefore_Renamed;
	  protected internal bool withoutDueDate_Renamed = false;
	  protected internal DateTime creationDate;
	  protected internal DateTime creationAfterDate;
	  protected internal DateTime creationBeforeDate;
	  protected internal DateTime completedDate;
	  protected internal DateTime completedAfterDate;
	  protected internal DateTime completedBeforeDate;
	  protected internal string category;
	  protected internal string tenantId;
	  protected internal string tenantIdLike;
	  protected internal bool withoutTenantId;
	  protected internal string locale_Renamed;
	  protected internal bool withLocalizationFallback_Renamed;
	  protected internal bool includeTaskLocalVariables_Renamed = false;
	  protected internal bool includeProcessVariables_Renamed = false;
	  protected internal int? taskVariablesLimit;
	  protected internal IList<HistoricTaskInstanceQueryImpl> orQueryObjects = new List<HistoricTaskInstanceQueryImpl>();
	  protected internal HistoricTaskInstanceQueryImpl currentOrQueryObject = null;
	  protected internal bool inOrStatement = false;

	  public HistoricTaskInstanceQueryImpl()
	  {
	  }

	  public HistoricTaskInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public HistoricTaskInstanceQueryImpl(CommandExecutor commandExecutor, string databaseType) : base(commandExecutor)
	  {
		this.databaseType = databaseType;
	  }

	  public override long executeCount(CommandContext commandContext)
	  {
		ensureVariablesInitialized();
		checkQueryOk();
		return commandContext.HistoricTaskInstanceEntityManager.findHistoricTaskInstanceCountByQueryCriteria(this);
	  }

	  public override IList<HistoricTaskInstance> executeList(CommandContext commandContext, Page page)
	  {
		ensureVariablesInitialized();
		checkQueryOk();
		IList<HistoricTaskInstance> tasks = null;
		if (includeTaskLocalVariables_Renamed || includeProcessVariables_Renamed)
		{
		  tasks = commandContext.HistoricTaskInstanceEntityManager.findHistoricTaskInstancesAndVariablesByQueryCriteria(this);
		}
		else
		{
		  tasks = commandContext.HistoricTaskInstanceEntityManager.findHistoricTaskInstancesByQueryCriteria(this);
		}

		if (tasks != null)
		{
		  foreach (HistoricTaskInstance task in tasks)
		  {
			localize(task);
		  }
		}

		return tasks;
	  }

	  protected internal virtual void localize(HistoricTaskInstance task)
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

	  public virtual HistoricTaskInstanceQueryImpl processInstanceId(string processInstanceId)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.processInstanceId_Renamed = processInstanceId;
		}
		else
		{
		  this.processInstanceId_Renamed = processInstanceId;
		}
		return this;
	  }

	  public override HistoricTaskInstanceQueryImpl processInstanceIdIn(IList<string> processInstanceIds)
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

		if (inOrStatement)
		{
		  this.currentOrQueryObject.processInstanceIds = processInstanceIds;
		}
		else
		{
		  this.processInstanceIds = processInstanceIds;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl processInstanceBusinessKey(string processInstanceBusinessKey)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.processInstanceBusinessKey_Renamed = processInstanceBusinessKey;
		}
		else
		{
		  this.processInstanceBusinessKey_Renamed = processInstanceBusinessKey;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl processInstanceBusinessKeyLike(string processInstanceBusinessKeyLike)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.processInstanceBusinessKeyLike_Renamed = processInstanceBusinessKeyLike;
		}
		else
		{
		  this.processInstanceBusinessKeyLike_Renamed = processInstanceBusinessKeyLike;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processInstanceBusinessKeyLikeIgnoreCase(string processInstanceBusinessKeyLikeIgnoreCase)
	  {
		  if (inOrStatement)
		  {
		  this.currentOrQueryObject.processInstanceBusinessKeyLikeIgnoreCase_Renamed = processInstanceBusinessKeyLikeIgnoreCase.ToLower();
		  }
		else
		{
		  this.processInstanceBusinessKeyLikeIgnoreCase_Renamed = processInstanceBusinessKeyLikeIgnoreCase.ToLower();
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl executionId(string executionId)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.executionId_Renamed = executionId;
		}
		else
		{
		  this.executionId_Renamed = executionId;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl processDefinitionId(string processDefinitionId)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.processDefinitionId_Renamed = processDefinitionId;
		}
		else
		{
		  this.processDefinitionId_Renamed = processDefinitionId;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processDefinitionKey(string processDefinitionKey)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.processDefinitionKey_Renamed = processDefinitionKey;
		}
		else
		{
		  this.processDefinitionKey_Renamed = processDefinitionKey;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processDefinitionKeyLike(string processDefinitionKeyLike)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.processDefinitionKeyLike_Renamed = processDefinitionKeyLike;
		}
		else
		{
		  this.processDefinitionKeyLike_Renamed = processDefinitionKeyLike;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processDefinitionKeyLikeIgnoreCase(string processDefinitionKeyLikeIgnoreCase)
	  {
		   if (inOrStatement)
		   {
		   this.currentOrQueryObject.processDefinitionKeyLikeIgnoreCase_Renamed = processDefinitionKeyLikeIgnoreCase.ToLower();
		   }
		 else
		 {
		   this.processDefinitionKeyLikeIgnoreCase_Renamed = processDefinitionKeyLikeIgnoreCase.ToLower();
		 }
		 return this;
	  }

	  public virtual HistoricTaskInstanceQuery processDefinitionKeyIn(IList<string> processDefinitionKeys)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.processDefinitionKeys = processDefinitionKeys;
		}
		else
		{
		  this.processDefinitionKeys = processDefinitionKeys;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processDefinitionName(string processDefinitionName)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.processDefinitionName_Renamed = processDefinitionName;
		}
		else
		{
		  this.processDefinitionName_Renamed = processDefinitionName;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processDefinitionNameLike(string processDefinitionNameLike)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.processDefinitionNameLike_Renamed = processDefinitionNameLike;
		}
		else
		{
		  this.processDefinitionNameLike_Renamed = processDefinitionNameLike;
		}
		return this;
	  }

	  public override HistoricTaskInstanceQuery processCategoryIn(IList<string> processCategoryInList)
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

		if (inOrStatement)
		{
		  currentOrQueryObject.processCategoryInList = processCategoryInList;
		}
		else
		{
		  this.processCategoryInList = processCategoryInList;
		}
		return this;
	  }

	  public override HistoricTaskInstanceQuery processCategoryNotIn(IList<string> processCategoryNotInList)
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

		if (inOrStatement)
		{
		  currentOrQueryObject.processCategoryNotInList = processCategoryNotInList;
		}
		else
		{
		  this.processCategoryNotInList = processCategoryNotInList;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery deploymentId(string deploymentId)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.deploymentId_Renamed = deploymentId;
		}
		else
		{
		  this.deploymentId_Renamed = deploymentId;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery deploymentIdIn(IList<string> deploymentIds)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.deploymentIds = deploymentIds;
		}
		else
		{
		  this.deploymentIds = deploymentIds;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskId(string taskId)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskId_Renamed = taskId;
		}
		else
		{
		  this.taskId_Renamed = taskId;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskName(string taskName)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskName_Renamed = taskName;
		}
		else
		{
		  this.taskName_Renamed = taskName;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskNameIn(IList<string> taskNameList)
	  {
		if (taskNameList == null)
		{
		  throw new ActivitiIllegalArgumentException("Task name list is null");
		}
		if (taskNameList.Count == 0)
		{
		  throw new ActivitiIllegalArgumentException("Task name list is empty");
		}

		if (taskName_Renamed != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both taskNameIn and taskName");
		}
		if (taskNameLike_Renamed != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both taskNameIn and taskNameLike");
		}
		if (taskNameLikeIgnoreCase_Renamed != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both taskNameIn and taskNameLikeIgnoreCase");
		}

		if (inOrStatement)
		{
		  currentOrQueryObject.taskNameList = taskNameList;
		}
		else
		{
		  this.taskNameList = taskNameList;
		}
		return this;
	  }

	  public override HistoricTaskInstanceQuery taskNameInIgnoreCase(IList<string> taskNameList)
	  {
		if (taskNameList == null)
		{
		  throw new ActivitiIllegalArgumentException("Task name list is null");
		}
		if (taskNameList.Count == 0)
		{
		  throw new ActivitiIllegalArgumentException("Task name list is empty");
		}
		foreach (string taskName in taskNameList)
		{
		  if (taskName == null)
		  {
			throw new ActivitiIllegalArgumentException("None of the given task names can be null");
		  }
		}

		if (taskName_Renamed != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both taskNameInIgnoreCase and name");
		}
		if (taskNameLike_Renamed != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both taskNameInIgnoreCase and nameLike");
		}
		if (taskNameLikeIgnoreCase_Renamed != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both taskNameInIgnoreCase and nameLikeIgnoreCase");
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nameListSize = taskNameList.size();
		int nameListSize = taskNameList.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> caseIgnoredTaskNameList = new java.util.ArrayList<String>(nameListSize);
		IList<string> caseIgnoredTaskNameList = new List<string>(nameListSize);
		foreach (string taskName in taskNameList)
		{
		  caseIgnoredTaskNameList.Add(taskName.ToLower());
		}

		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskNameListIgnoreCase = caseIgnoredTaskNameList;
		}
		else
		{
		  this.taskNameListIgnoreCase = caseIgnoredTaskNameList;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskNameLike(string taskNameLike)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskNameLike_Renamed = taskNameLike;
		}
		else
		{
		  this.taskNameLike_Renamed = taskNameLike;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskNameLikeIgnoreCase(string taskNameLikeIgnoreCase)
	  {
		  if (inOrStatement)
		  {
		  this.currentOrQueryObject.taskNameLikeIgnoreCase_Renamed = taskNameLikeIgnoreCase.ToLower();
		  }
		else
		{
		  this.taskNameLikeIgnoreCase_Renamed = taskNameLikeIgnoreCase.ToLower();
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskParentTaskId(string parentTaskId)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskParentTaskId_Renamed = parentTaskId;
		}
		else
		{
		  this.taskParentTaskId_Renamed = parentTaskId;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskDescription(string taskDescription)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskDescription_Renamed = taskDescription;
		}
		else
		{
		  this.taskDescription_Renamed = taskDescription;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskDescriptionLike(string taskDescriptionLike)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskDescriptionLike_Renamed = taskDescriptionLike;
		}
		else
		{
		  this.taskDescriptionLike_Renamed = taskDescriptionLike;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskDescriptionLikeIgnoreCase(string taskDescriptionLikeIgnoreCase)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskDescriptionLikeIgnoreCase_Renamed = taskDescriptionLikeIgnoreCase.ToLower();
		}
		else
		{
		  this.taskDescriptionLikeIgnoreCase_Renamed = taskDescriptionLikeIgnoreCase.ToLower();
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskDeleteReason(string taskDeleteReason)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskDeleteReason_Renamed = taskDeleteReason;
		}
		else
		{
		  this.taskDeleteReason_Renamed = taskDeleteReason;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskDeleteReasonLike(string taskDeleteReasonLike)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskDeleteReasonLike_Renamed = taskDeleteReasonLike;
		}
		else
		{
		  this.taskDeleteReasonLike_Renamed = taskDeleteReasonLike;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskAssignee(string taskAssignee)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskAssignee_Renamed = taskAssignee;
		}
		else
		{
		  this.taskAssignee_Renamed = taskAssignee;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskAssigneeLike(string taskAssigneeLike)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskAssigneeLike_Renamed = taskAssigneeLike;
		}
		else
		{
		  this.taskAssigneeLike_Renamed = taskAssigneeLike;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskAssigneeLikeIgnoreCase(string taskAssigneeLikeIgnoreCase)
	  {
		   if (inOrStatement)
		   {
		   this.currentOrQueryObject.taskAssigneeLikeIgnoreCase_Renamed = taskAssigneeLikeIgnoreCase.ToLower();
		   }
		 else
		 {
		   this.taskAssigneeLikeIgnoreCase_Renamed = taskAssigneeLikeIgnoreCase.ToLower();
		 }
		 return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskOwner(string taskOwner)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskOwner_Renamed = taskOwner;
		}
		else
		{
		  this.taskOwner_Renamed = taskOwner;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskOwnerLike(string taskOwnerLike)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskOwnerLike_Renamed = taskOwnerLike;
		}
		else
		{
		  this.taskOwnerLike_Renamed = taskOwnerLike;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskOwnerLikeIgnoreCase(string taskOwnerLikeIgnoreCase)
	  {
		  if (inOrStatement)
		  {
		  this.currentOrQueryObject.taskOwnerLikeIgnoreCase_Renamed = taskOwnerLikeIgnoreCase.ToLower();
		  }
		else
		{
		  this.taskOwnerLikeIgnoreCase_Renamed = taskOwnerLikeIgnoreCase.ToLower();
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery finished()
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.finished_Renamed = true;
		}
		else
		{
		  this.finished_Renamed = true;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery unfinished()
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.unfinished_Renamed = true;
		}
		else
		{
		  this.unfinished_Renamed = true;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskVariableValueEquals(string variableName, object variableValue)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueEquals(variableName, variableValue);
		  return this;
		}
		else
		{
		  return variableValueEquals(variableName, variableValue);
		}
	  }

	  public virtual HistoricTaskInstanceQuery taskVariableValueEquals(object variableValue)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueEquals(variableValue);
		  return this;
		}
		else
		{
		  return variableValueEquals(variableValue);
		}
	  }

	  public virtual HistoricTaskInstanceQuery taskVariableValueEqualsIgnoreCase(string name, string value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueEqualsIgnoreCase(name, value);
		  return this;
		}
		else
		{
		  return variableValueEqualsIgnoreCase(name, value);
		}
	  }

	  public virtual HistoricTaskInstanceQuery taskVariableValueNotEqualsIgnoreCase(string name, string value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueNotEqualsIgnoreCase(name, value);
		  return this;
		}
		else
		{
		  return variableValueNotEqualsIgnoreCase(name, value);
		}
	  }

	  public virtual HistoricTaskInstanceQuery taskVariableValueNotEquals(string variableName, object variableValue)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueNotEquals(variableName, variableValue);
		  return this;
		}
		else
		{
		  return variableValueNotEquals(variableName, variableValue);
		}
	  }

	  public virtual HistoricTaskInstanceQuery taskVariableValueGreaterThan(string name, object value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueGreaterThan(name, value);
		  return this;
		}
		else
		{
		  return variableValueGreaterThan(name, value);
		}
	  }

	  public virtual HistoricTaskInstanceQuery taskVariableValueGreaterThanOrEqual(string name, object value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueGreaterThanOrEqual(name, value);
		  return this;
		}
		else
		{
		  return variableValueGreaterThanOrEqual(name, value);
		}
	  }

	  public virtual HistoricTaskInstanceQuery taskVariableValueLessThan(string name, object value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueLessThan(name, value);
		  return this;
		}
		else
		{
		  return variableValueLessThan(name, value);
		}
	  }

	  public virtual HistoricTaskInstanceQuery taskVariableValueLessThanOrEqual(string name, object value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueLessThanOrEqual(name, value);
		  return this;
		}
		else
		{
		  return variableValueLessThanOrEqual(name, value);
		}
	  }

	  public virtual HistoricTaskInstanceQuery taskVariableValueLike(string name, string value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueLike(name, value);
		  return this;
		}
		else
		{
		  return variableValueLike(name, value);
		}
	  }

	  public virtual HistoricTaskInstanceQuery taskVariableValueLikeIgnoreCase(string name, string value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueLikeIgnoreCase(name, value, true);
		  return this;
		}
		else
		{
		  return variableValueLikeIgnoreCase(name, value, true);
		}
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueEquals(string variableName, object variableValue)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueEquals(variableName, variableValue, false);
		  return this;
		}
		else
		{
		  return variableValueEquals(variableName, variableValue, false);
		}
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueNotEquals(string variableName, object variableValue)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueNotEquals(variableName, variableValue, false);
		  return this;
		}
		else
		{
		  return variableValueNotEquals(variableName, variableValue, false);
		}
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueEquals(object variableValue)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueEquals(variableValue, false);
		  return this;
		}
		else
		{
		  return variableValueEquals(variableValue, false);
		}
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueEqualsIgnoreCase(string name, string value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueEqualsIgnoreCase(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueEqualsIgnoreCase(name, value, false);
		}
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueNotEqualsIgnoreCase(string name, string value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueNotEqualsIgnoreCase(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueNotEqualsIgnoreCase(name, value, false);
		}
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueGreaterThan(string name, object value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueGreaterThan(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueGreaterThan(name, value, false);
		}
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueGreaterThanOrEqual(string name, object value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueGreaterThanOrEqual(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueGreaterThanOrEqual(name, value, false);
		}
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueLessThan(string name, object value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueLessThan(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueLessThan(name, value, false);
		}
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueLessThanOrEqual(string name, object value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueLessThanOrEqual(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueLessThanOrEqual(name, value, false);
		}
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueLike(string name, string value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueLike(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueLike(name, value, false);
		}
	  }

	  public virtual HistoricTaskInstanceQuery processVariableValueLikeIgnoreCase(string name, string value)
	  {
		if (inOrStatement)
		{
		  currentOrQueryObject.variableValueLikeIgnoreCase(name, value, false);
		  return this;
		}
		else
		{
		  return variableValueLikeIgnoreCase(name, value, false);
		}
	  }

	  public virtual HistoricTaskInstanceQuery taskDefinitionKey(string taskDefinitionKey)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskDefinitionKey_Renamed = taskDefinitionKey;
		}
		else
		{
		  this.taskDefinitionKey_Renamed = taskDefinitionKey;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskDefinitionKeyLike(string taskDefinitionKeyLike)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskDefinitionKeyLike_Renamed = taskDefinitionKeyLike;
		}
		else
		{
		  this.taskDefinitionKeyLike_Renamed = taskDefinitionKeyLike;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskPriority(int? taskPriority)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskPriority_Renamed = taskPriority;
		}
		else
		{
		  this.taskPriority_Renamed = taskPriority;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskMinPriority(int? taskMinPriority)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskMinPriority_Renamed = taskMinPriority;
		}
		else
		{
		  this.taskMinPriority_Renamed = taskMinPriority;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskMaxPriority(int? taskMaxPriority)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.taskMaxPriority_Renamed = taskMaxPriority;
		}
		else
		{
		  this.taskMaxPriority_Renamed = taskMaxPriority;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processFinished()
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.processFinished_Renamed = true;
		}
		else
		{
		  this.processFinished_Renamed = true;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery processUnfinished()
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.processUnfinished_Renamed = true;
		}
		else
		{
		  this.processUnfinished_Renamed = true;
		}
		return this;
	  }

	  protected internal virtual void ensureVariablesInitialized()
	  {
		VariableTypes types = Context.ProcessEngineConfiguration.VariableTypes;
		foreach (QueryVariableValue @var in queryVariableValues)
		{
		  @var.initialize(types);
		}

		foreach (HistoricTaskInstanceQueryImpl orQueryObject in orQueryObjects)
		{
		  orQueryObject.ensureVariablesInitialized();
		}
	  }

	  public virtual HistoricTaskInstanceQuery taskDueDate(DateTime dueDate)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.dueDate_Renamed = dueDate;
		}
		else
		{
		  this.dueDate_Renamed = dueDate;
		}
		return this;
	  }

	  public override HistoricTaskInstanceQuery dueDate(DateTime dueDate)
	  {
		  return taskDueDate(dueDate);
	  }

	  public virtual HistoricTaskInstanceQuery taskDueAfter(DateTime dueAfter)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.dueAfter_Renamed = dueAfter;
		}
		else
		{
		  this.dueAfter_Renamed = dueAfter;
		}
		return this;
	  }

	  public override HistoricTaskInstanceQuery dueAfter(DateTime dueDate)
	  {
		  return taskDueAfter(dueDate);
	  }

	  public virtual HistoricTaskInstanceQuery taskDueBefore(DateTime dueBefore)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.dueBefore_Renamed = dueBefore;
		}
		else
		{
		  this.dueBefore_Renamed = dueBefore;
		}
		return this;
	  }

	  public override HistoricTaskInstanceQuery dueBefore(DateTime dueDate)
	  {
		  return taskDueBefore(dueDate);
	  }

	  public virtual HistoricTaskInstanceQuery taskCreatedOn(DateTime creationDate)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.creationDate = creationDate;
		}
		else
		{
		  this.creationDate = creationDate;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskCreatedBefore(DateTime creationBeforeDate)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.creationBeforeDate = creationBeforeDate;
		}
		else
		{
		  this.creationBeforeDate = creationBeforeDate;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskCreatedAfter(DateTime creationAfterDate)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.creationAfterDate = creationAfterDate;
		}
		else
		{
		  this.creationAfterDate = creationAfterDate;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskCompletedOn(DateTime completedDate)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.completedDate = completedDate;
		}
		else
		{
		  this.completedDate = completedDate;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskCompletedBefore(DateTime completedBeforeDate)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.completedBeforeDate = completedBeforeDate;
		}
		else
		{
		  this.completedBeforeDate = completedBeforeDate;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskCompletedAfter(DateTime completedAfterDate)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.completedAfterDate = completedAfterDate;
		}
		else
		{
		  this.completedAfterDate = completedAfterDate;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery withoutTaskDueDate()
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.withoutDueDate_Renamed = true;
		}
		else
		{
		  this.withoutDueDate_Renamed = true;
		}
		return this;
	  }

	  public override HistoricTaskInstanceQuery withoutDueDate()
	  {
		  return withoutTaskDueDate();
	  }

	  public virtual HistoricTaskInstanceQuery taskCategory(string category)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.category = category;
		}
		else
		{
		  this.category = category;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskCandidateUser(string candidateUser)
	  {
		if (candidateUser == null)
		{
		  throw new ActivitiIllegalArgumentException("Candidate user is null");
		}

		if (inOrStatement)
		{
		  this.currentOrQueryObject.candidateUser = candidateUser;
		}
		else
		{
		  this.candidateUser = candidateUser;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskCandidateGroup(string candidateGroup)
	  {
		if (candidateGroup == null)
		{
		  throw new ActivitiIllegalArgumentException("Candidate group is null");
		}

		if (candidateGroups != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query usage: cannot set both candidateGroup and candidateGroupIn");
		}

		if (inOrStatement)
		{
		  this.currentOrQueryObject.candidateGroup = candidateGroup;
		}
		else
		{
		  this.candidateGroup = candidateGroup;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskCandidateGroupIn(IList<string> candidateGroups)
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

		if (inOrStatement)
		{
		  this.currentOrQueryObject.candidateGroups = candidateGroups;
		}
		else
		{
		  this.candidateGroups = candidateGroups;
		}
		return this;
	  }

	  public override HistoricTaskInstanceQuery taskInvolvedUser(string involvedUser)
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.involvedUser = involvedUser;
		}
		else
		{
		  this.involvedUser = involvedUser;
		}
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskTenantId(string tenantId)
	  {
		  if (tenantId == null)
		  {
			  throw new ActivitiIllegalArgumentException("task tenant id is null");
		  }
		  if (inOrStatement)
		  {
		  this.currentOrQueryObject.tenantId = tenantId;
		  }
		else
		{
		  this.tenantId = tenantId;
		}
		  return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskTenantIdLike(string tenantIdLike)
	  {
		  if (tenantIdLike == null)
		  {
			  throw new ActivitiIllegalArgumentException("task tenant id is null");
		  }
		  if (inOrStatement)
		  {
		  this.currentOrQueryObject.tenantIdLike = tenantIdLike;
		  }
		else
		{
		  this.tenantIdLike = tenantIdLike;
		}
		  return this;
	  }

	  public virtual HistoricTaskInstanceQuery taskWithoutTenantId()
	  {
		if (inOrStatement)
		{
		  this.currentOrQueryObject.withoutTenantId = true;
		}
		else
		{
		  this.withoutTenantId = true;
		}
		  return this;
	  }

	  public virtual HistoricTaskInstanceQuery locale(string locale)
	  {
		this.locale_Renamed = locale;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery withLocalizationFallback()
	  {
		withLocalizationFallback_Renamed = true;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery includeTaskLocalVariables()
	  {
		this.includeTaskLocalVariables_Renamed = true;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery includeProcessVariables()
	  {
		this.includeProcessVariables_Renamed = true;
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery limitTaskVariables(int? taskVariablesLimit)
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

	  public virtual HistoricTaskInstanceQuery or()
	  {
		if (inOrStatement)
		{
		  throw new ActivitiException("the query is already in an or statement");
		}

		inOrStatement = true;
		currentOrQueryObject = new HistoricTaskInstanceQueryImpl();
		orQueryObjects.Add(currentOrQueryObject);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery endOr()
	  {
		if (!inOrStatement)
		{
		  throw new ActivitiException("endOr() can only be called after calling or()");
		}

		inOrStatement = false;
		currentOrQueryObject = null;
		return this;
	  }

	  // ordering /////////////////////////////////////////////////////////////////

	  public virtual HistoricTaskInstanceQueryImpl orderByTaskId()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.HISTORIC_TASK_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByHistoricActivityInstanceId()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.PROCESS_DEFINITION_ID);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByProcessDefinitionId()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.PROCESS_DEFINITION_ID);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByProcessInstanceId()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.PROCESS_INSTANCE_ID);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByExecutionId()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.EXECUTION_ID);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByHistoricTaskInstanceDuration()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.DURATION);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByHistoricTaskInstanceEndTime()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.END);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByHistoricActivityInstanceStartTime()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.START);
		return this;
	  }

	  public override HistoricTaskInstanceQuery orderByHistoricTaskInstanceStartTime()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.START);
		return this;
	  }

	  public override HistoricTaskInstanceQuery orderByTaskCreateTime()
	  {
		  return orderByHistoricTaskInstanceStartTime();
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByTaskName()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.TASK_NAME);
		return this;
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByTaskDescription()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.TASK_DESCRIPTION);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByTaskAssignee()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.TASK_ASSIGNEE);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByTaskOwner()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.TASK_OWNER);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByTaskDueDate()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.TASK_DUE_DATE);
		return this;
	  }

	  public override HistoricTaskInstanceQuery orderByDueDateNullsFirst()
	  {
		  return orderBy(HistoricTaskInstanceQueryProperty.TASK_DUE_DATE, NullHandlingOnOrder.NULLS_FIRST);
	  }

	  public override HistoricTaskInstanceQuery orderByDueDateNullsLast()
	  {
		  return orderBy(HistoricTaskInstanceQueryProperty.TASK_DUE_DATE, NullHandlingOnOrder.NULLS_LAST);
	  }

	  public virtual HistoricTaskInstanceQueryImpl orderByDeleteReason()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.DELETE_REASON);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByTaskDefinitionKey()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.TASK_DEFINITION_KEY);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByTaskPriority()
	  {
		orderBy(HistoricTaskInstanceQueryProperty.TASK_PRIORITY);
		return this;
	  }

	  public virtual HistoricTaskInstanceQuery orderByTenantId()
	  {
		  orderBy(HistoricTaskInstanceQueryProperty.TENANT_ID_);
		  return this;
	  }

	  protected internal override void checkQueryOk()
	  {
		base.checkQueryOk();
		// In case historic query variables are included, an additional order-by clause should be added
		// to ensure the last value of a variable is used
		if (includeProcessVariables_Renamed || includeTaskLocalVariables_Renamed)
		{
			this.orderBy(HistoricTaskInstanceQueryProperty.INCLUDED_VARIABLE_TIME).asc();
		}
	  }

	  public virtual string MssqlOrDB2OrderBy
	  {
		  get
		  {
			string specialOrderBy = base.OrderBy;
			if (specialOrderBy != null && specialOrderBy.Length > 0)
			{
			  specialOrderBy = specialOrderBy.Replace("RES.", "TEMPRES_");
			  specialOrderBy = specialOrderBy.Replace("VAR.", "TEMPVAR_");
			}
			return specialOrderBy;
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

	  // getters and setters //////////////////////////////////////////////////////

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
	  public virtual string ProcessInstanceBusinessKey
	  {
		  get
		  {
			return processInstanceBusinessKey_Renamed;
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
	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Renamed;
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
	  public virtual string ProcessDefinitionName
	  {
		  get
		  {
			return processDefinitionName_Renamed;
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
	  public virtual string TaskDefinitionKeyLike
	  {
		  get
		  {
			return taskDefinitionKeyLike_Renamed;
		  }
	  }
	  public virtual int? TaskPriority
	  {
		  get
		  {
			return taskPriority_Renamed;
		  }
	  }
	  public virtual int? TaskMinPriority
	  {
		  get
		  {
			return taskMinPriority_Renamed;
		  }
	  }
	  public virtual int? TaskMaxPriority
	  {
		  get
		  {
			return taskMaxPriority_Renamed;
		  }
	  }
	  public virtual bool ProcessFinished
	  {
		  get
		  {
			return processFinished_Renamed;
		  }
	  }
	  public virtual bool ProcessUnfinished
	  {
		  get
		  {
			return processUnfinished_Renamed;
		  }
	  }
	  public virtual DateTime DueDate
	  {
		  get
		  {
			return dueDate_Renamed;
		  }
	  }
	  public virtual DateTime DueAfter
	  {
		  get
		  {
			return dueAfter_Renamed;
		  }
	  }
	  public virtual DateTime DueBefore
	  {
		  get
		  {
			return dueBefore_Renamed;
		  }
	  }
	  public virtual bool WithoutDueDate
	  {
		  get
		  {
			return withoutDueDate_Renamed;
		  }
	  }
	  public virtual DateTime CreationAfterDate
	  {
		  get
		  {
			return creationAfterDate;
		  }
	  }
	  public virtual DateTime CreationBeforeDate
	  {
		  get
		  {
			return creationBeforeDate;
		  }
	  }
	  public virtual DateTime CompletedDate
	  {
		  get
		  {
			return completedDate;
		  }
	  }
	  public virtual DateTime CompletedAfterDate
	  {
		  get
		  {
			return completedAfterDate;
		  }
	  }
	  public virtual DateTime CompletedBeforeDate
	  {
		  get
		  {
			return completedBeforeDate;
		  }
	  }
	  public virtual string Category
	  {
		  get
		  {
			return category;
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
	  public virtual bool InOrStatement
	  {
		  get
		  {
			return inOrStatement;
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
	  public virtual string TaskName
	  {
		  get
		  {
			return taskName_Renamed;
		  }
	  }
	  public virtual string TaskNameLike
	  {
		  get
		  {
			return taskNameLike_Renamed;
		  }
	  }
	  public virtual IList<string> TaskNameList
	  {
		  get
		  {
			return taskNameList;
		  }
	  }
	  public virtual IList<string> TaskNameListIgnoreCase
	  {
		  get
		  {
			return taskNameListIgnoreCase;
		  }
	  }
	  public virtual string TaskDescription
	  {
		  get
		  {
			return taskDescription_Renamed;
		  }
	  }
	  public virtual string TaskDescriptionLike
	  {
		  get
		  {
			return taskDescriptionLike_Renamed;
		  }
	  }
	  public virtual string TaskDeleteReason
	  {
		  get
		  {
			return taskDeleteReason_Renamed;
		  }
	  }
	  public virtual string TaskDeleteReasonLike
	  {
		  get
		  {
			return taskDeleteReasonLike_Renamed;
		  }
	  }
	  public virtual string TaskAssignee
	  {
		  get
		  {
			return taskAssignee_Renamed;
		  }
	  }
	  public virtual string TaskAssigneeLike
	  {
		  get
		  {
			return taskAssigneeLike_Renamed;
		  }
	  }
	  public virtual string TaskId
	  {
		  get
		  {
			return taskId_Renamed;
		  }
	  }
	  public virtual string TaskDefinitionKey
	  {
		  get
		  {
			return taskDefinitionKey_Renamed;
		  }
	  }
	  public virtual string TaskOwnerLike
	  {
		  get
		  {
			return taskOwnerLike_Renamed;
		  }
	  }
	  public virtual string TaskOwner
	  {
		  get
		  {
			return taskOwner_Renamed;
		  }
	  }
	  public virtual string TaskParentTaskId
	  {
		  get
		  {
			return taskParentTaskId_Renamed;
		  }
	  }
	  public virtual DateTime CreationDate
	  {
		  get
		  {
			return creationDate;
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
	  public virtual string InvolvedUser
	  {
		  get
		  {
			return involvedUser;
		  }
	  }
		public virtual string ProcessDefinitionKeyLikeIgnoreCase
		{
			get
			{
				return processDefinitionKeyLikeIgnoreCase_Renamed;
			}
		}
		public virtual string ProcessInstanceBusinessKeyLikeIgnoreCase
		{
			get
			{
				return processInstanceBusinessKeyLikeIgnoreCase_Renamed;
			}
		}
		public virtual string TaskNameLikeIgnoreCase
		{
			get
			{
				return taskNameLikeIgnoreCase_Renamed;
			}
		}
		public virtual string TaskDescriptionLikeIgnoreCase
		{
			get
			{
				return taskDescriptionLikeIgnoreCase_Renamed;
			}
		}
		public virtual string TaskOwnerLikeIgnoreCase
		{
			get
			{
				return taskOwnerLikeIgnoreCase_Renamed;
			}
		}
		public virtual string TaskAssigneeLikeIgnoreCase
		{
			get
			{
				return taskAssigneeLikeIgnoreCase_Renamed;
			}
		}
		public virtual IList<HistoricTaskInstanceQueryImpl> OrQueryObjects
		{
			get
			{
			return orQueryObjects;
			}
		}
	}

}