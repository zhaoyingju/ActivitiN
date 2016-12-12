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
namespace org.activiti.engine.impl.bpmn.behavior
{


	using Expression = org.activiti.engine.@delegate.Expression;
	using TaskListener = org.activiti.engine.@delegate.TaskListener;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using SkipExpressionUtil = org.activiti.engine.impl.bpmn.helper.SkipExpressionUtil;
	using BusinessCalendar = org.activiti.engine.impl.calendar.BusinessCalendar;
	using DueDateBusinessCalendar = org.activiti.engine.impl.calendar.DueDateBusinessCalendar;
	using Context = org.activiti.engine.impl.context.Context;
	using ExpressionManager = org.activiti.engine.impl.el.ExpressionManager;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using TaskDefinition = org.activiti.engine.impl.task.TaskDefinition;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;

	/// <summary>
	/// activity implementation for the user task.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class UserTaskActivityBehavior : TaskActivityBehavior
	{

	  private const long serialVersionUID = 1L;

	  private static readonly Logger LOGGER = LoggerFactory.getLogger(typeof(UserTaskActivityBehavior));

	  protected internal string userTaskId;
	  protected internal TaskDefinition taskDefinition;

	  public UserTaskActivityBehavior(string userTaskId, TaskDefinition taskDefinition)
	  {
		this.userTaskId = userTaskId;
		this.taskDefinition = taskDefinition;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		TaskEntity task = TaskEntity.createAndInsert(execution);
		task.setExecution(execution);

		Expression activeNameExpression = null;
		Expression activeDescriptionExpression = null;
		Expression activeDueDateExpression = null;
		Expression activePriorityExpression = null;
		Expression activeCategoryExpression = null;
		Expression activeFormKeyExpression = null;
		Expression activeSkipExpression = null;
		Expression activeAssigneeExpression = null;
		Expression activeOwnerExpression = null;
		Set<Expression> activeCandidateUserExpressions = null;
		Set<Expression> activeCandidateGroupExpressions = null;

		if (Context.ProcessEngineConfiguration.EnableProcessDefinitionInfoCache)
		{
		  ObjectNode taskElementProperties = Context.getBpmnOverrideElementProperties(userTaskId, execution.ProcessDefinitionId);
		  activeNameExpression = getActiveValue(taskDefinition.NameExpression, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_NAME, taskElementProperties);
		  taskDefinition.NameExpression = activeNameExpression;
		  activeDescriptionExpression = getActiveValue(taskDefinition.DescriptionExpression, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_DESCRIPTION, taskElementProperties);
		  taskDefinition.DescriptionExpression = activeDescriptionExpression;
		  activeDueDateExpression = getActiveValue(taskDefinition.DueDateExpression, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_DUEDATE, taskElementProperties);
		  taskDefinition.DueDateExpression = activeDueDateExpression;
		  activePriorityExpression = getActiveValue(taskDefinition.PriorityExpression, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_PRIORITY, taskElementProperties);
		  taskDefinition.PriorityExpression = activePriorityExpression;
		  activeCategoryExpression = getActiveValue(taskDefinition.CategoryExpression, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_CATEGORY, taskElementProperties);
		  taskDefinition.CategoryExpression = activeCategoryExpression;
		  activeFormKeyExpression = getActiveValue(taskDefinition.FormKeyExpression, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_FORM_KEY, taskElementProperties);
		  taskDefinition.FormKeyExpression = activeFormKeyExpression;
		  activeSkipExpression = getActiveValue(taskDefinition.SkipExpression, org.activiti.engine.DynamicBpmnConstants_Fields.TASK_SKIP_EXPRESSION, taskElementProperties);
		  taskDefinition.SkipExpression = activeSkipExpression;
		  activeAssigneeExpression = getActiveValue(taskDefinition.AssigneeExpression, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_ASSIGNEE, taskElementProperties);
		  taskDefinition.AssigneeExpression = activeAssigneeExpression;
		  activeOwnerExpression = getActiveValue(taskDefinition.OwnerExpression, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_OWNER, taskElementProperties);
		  taskDefinition.OwnerExpression = activeOwnerExpression;
		  activeCandidateUserExpressions = getActiveValueSet(taskDefinition.CandidateUserIdExpressions, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_CANDIDATE_USERS, taskElementProperties);
		  taskDefinition.CandidateUserIdExpressions = activeCandidateUserExpressions;
		  activeCandidateGroupExpressions = getActiveValueSet(taskDefinition.CandidateGroupIdExpressions, org.activiti.engine.DynamicBpmnConstants_Fields.USER_TASK_CANDIDATE_GROUPS, taskElementProperties);
		  taskDefinition.CandidateGroupIdExpressions = activeCandidateGroupExpressions;

		}
		else
		{
		  activeNameExpression = taskDefinition.NameExpression;
		  activeDescriptionExpression = taskDefinition.DescriptionExpression;
		  activeDueDateExpression = taskDefinition.DueDateExpression;
		  activePriorityExpression = taskDefinition.PriorityExpression;
		  activeCategoryExpression = taskDefinition.CategoryExpression;
		  activeFormKeyExpression = taskDefinition.FormKeyExpression;
		  activeSkipExpression = taskDefinition.SkipExpression;
		  activeAssigneeExpression = taskDefinition.AssigneeExpression;
		  activeOwnerExpression = taskDefinition.OwnerExpression;
		  activeCandidateUserExpressions = taskDefinition.CandidateUserIdExpressions;
		  activeCandidateGroupExpressions = taskDefinition.CandidateGroupIdExpressions;
		}

		task.TaskDefinition = taskDefinition;

		if (activeNameExpression != null)
		{
		  string name = null;
		  try
		  {
			name = (string) activeNameExpression.getValue(execution);
		  }
		  catch (ActivitiException e)
		  {
			name = activeNameExpression.ExpressionText;
			LOGGER.warn("property not found in task name expression " + e.Message);
		  }
		  task.Name = name;
		}

		if (activeDescriptionExpression != null)
		{
		  string description = null;
		  try
		  {
			description = (string) activeDescriptionExpression.getValue(execution);
		  }
		  catch (ActivitiException e)
		  {
			description = activeDescriptionExpression.ExpressionText;
			LOGGER.warn("property not found in task description expression " + e.Message);
		  }
		  task.Description = description;
		}

		if (activeDueDateExpression != null)
		{
		  object dueDate = activeDueDateExpression.getValue(execution);
		  if (dueDate != null)
		  {
			if (dueDate is DateTime)
			{
			  task.DueDate = (DateTime) dueDate;
			}
			else if (dueDate is string)
			{
			  BusinessCalendar businessCalendar = Context.ProcessEngineConfiguration.BusinessCalendarManager.getBusinessCalendar(taskDefinition.BusinessCalendarNameExpression.getValue(execution).ToString());
			  task.DueDate = businessCalendar.resolveDuedate((string) dueDate);
			}
			else
			{
			  throw new ActivitiIllegalArgumentException("Due date expression does not resolve to a Date or Date string: " + activeDueDateExpression.ExpressionText);
			}
		  }
		}

		if (activePriorityExpression != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object priority = activePriorityExpression.getValue(execution);
		  object priority = activePriorityExpression.getValue(execution);
		  if (priority != null)
		  {
			if (priority is string)
			{
			  try
			  {
				task.Priority = Convert.ToInt32((string) priority);
			  }
			  catch (NumberFormatException e)
			  {
				throw new ActivitiIllegalArgumentException("Priority does not resolve to a number: " + priority, e);
			  }
			}
			else if (priority is Number)
			{
			  task.Priority = (int)((Number) priority);
			}
			else
			{
			  throw new ActivitiIllegalArgumentException("Priority expression does not resolve to a number: " + activePriorityExpression.ExpressionText);
			}
		  }
		}

		if (activeCategoryExpression != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object category = activeCategoryExpression.getValue(execution);
			object category = activeCategoryExpression.getValue(execution);
			if (category != null)
			{
				if (category is string)
				{
					task.Category = (string) category;
				}
				else
				{
					 throw new ActivitiIllegalArgumentException("Category expression does not resolve to a string: " + activeCategoryExpression.ExpressionText);
				}
			}
		}

		if (activeFormKeyExpression != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object formKey = activeFormKeyExpression.getValue(execution);
			object formKey = activeFormKeyExpression.getValue(execution);
			if (formKey != null)
			{
				if (formKey is string)
				{
					task.FormKey = (string) formKey;
				}
				else
				{
				  throw new ActivitiIllegalArgumentException("FormKey expression does not resolve to a string: " + activeFormKeyExpression.ExpressionText);
				}
			}
		}

		bool skipUserTask = SkipExpressionUtil.isSkipExpressionEnabled(execution, activeSkipExpression) && SkipExpressionUtil.shouldSkipFlowElement(execution, activeSkipExpression);

		if (!skipUserTask)
		{
		  handleAssignments(activeAssigneeExpression, activeOwnerExpression, activeCandidateUserExpressions, activeCandidateGroupExpressions, task, execution);
		}

		task.fireEvent(org.activiti.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE);

		// All properties set, now firing 'create' events
		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
		  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.TASK_CREATED, task));
		}

		if (skipUserTask)
		{
		  task.complete(null, false);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void signal(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		if (((ExecutionEntity) execution).Tasks.Count > 0)
		{
		  throw new ActivitiException("UserTask should not be signalled before complete");
		}
		leave(execution);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) protected void handleAssignments(org.activiti.engine.delegate.Expression assigneeExpression, org.activiti.engine.delegate.Expression ownerExpression, java.util.Set<org.activiti.engine.delegate.Expression> candidateUserExpressions, java.util.Set<org.activiti.engine.delegate.Expression> candidateGroupExpressions, org.activiti.engine.impl.persistence.entity.TaskEntity task, org.activiti.engine.impl.pvm.delegate.ActivityExecution execution)
	  protected internal virtual void handleAssignments(Expression assigneeExpression, Expression ownerExpression, Set<Expression> candidateUserExpressions, Set<Expression> candidateGroupExpressions, TaskEntity task, ActivityExecution execution)
	  {

		if (assigneeExpression != null)
		{
		  object assigneeExpressionValue = assigneeExpression.getValue(execution);
		  string assigneeValue = null;
		  if (assigneeExpressionValue != null)
		  {
			assigneeValue = assigneeExpressionValue.ToString();
		  }
		  task.setAssignee(assigneeValue, true, false);
		}

		if (ownerExpression != null)
		{
		  object ownerExpressionValue = ownerExpression.getValue(execution);
		  string ownerValue = null;
		  if (ownerExpressionValue != null)
		  {
			ownerValue = ownerExpressionValue.ToString();
		  }
		  task.Owner = ownerValue;
		}

		if (candidateGroupExpressions != null && !candidateGroupExpressions.Empty)
		{
		  foreach (Expression groupIdExpr in candidateGroupExpressions)
		  {
			object value = groupIdExpr.getValue(execution);
			if (value is string)
			{
			  IList<string> candidates = extractCandidates((string) value);
			  task.addCandidateGroups(candidates);
			}
			else if (value is ICollection)
			{
			  task.addCandidateGroups((ICollection) value);
			}
			else
			{
			  throw new ActivitiIllegalArgumentException("Expression did not resolve to a string or collection of strings");
			}
		  }
		}

		if (candidateUserExpressions != null && !candidateUserExpressions.Empty)
		{
		  foreach (Expression userIdExpr in candidateUserExpressions)
		  {
			object value = userIdExpr.getValue(execution);
			if (value is string)
			{
			  IList<string> candiates = extractCandidates((string) value);
			  task.addCandidateUsers(candiates);
			}
			else if (value is ICollection)
			{
			  task.addCandidateUsers((ICollection) value);
			}
			else
			{
			  throw new ActivitiException("Expression did not resolve to a string or collection of strings");
			}
		  }
		}

		if (taskDefinition.CustomUserIdentityLinkExpressions.Count > 0)
		{
		  IDictionary<string, Set<Expression>> identityLinks = taskDefinition.CustomUserIdentityLinkExpressions;
		  foreach (string identityLinkType in identityLinks.Keys)
		  {
			foreach (Expression idExpression in identityLinks[identityLinkType])
			{
			  object value = idExpression.getValue(execution);
			  if (value is string)
			  {
				IList<string> userIds = extractCandidates((string) value);
				foreach (string userId in userIds)
				{
				  task.addUserIdentityLink(userId, identityLinkType);
				}
			  }
			  else if (value is ICollection)
			  {
				IEnumerator userIdSet = ((ICollection) value).GetEnumerator();
				while (userIdSet.hasNext())
				{
				  task.addUserIdentityLink((string)userIdSet.next(), identityLinkType);
				}
			  }
			  else
			  {
				throw new ActivitiException("Expression did not resolve to a string or collection of strings");
			  }
			}
		  }
		}

		if (taskDefinition.CustomGroupIdentityLinkExpressions.Count > 0)
		{
		  IDictionary<string, Set<Expression>> identityLinks = taskDefinition.CustomGroupIdentityLinkExpressions;
		  foreach (string identityLinkType in identityLinks.Keys)
		  {
			foreach (Expression idExpression in identityLinks[identityLinkType])
			{
			  object value = idExpression.getValue(execution);
			  if (value is string)
			  {
				IList<string> groupIds = extractCandidates((string) value);
				foreach (string groupId in groupIds)
				{
				  task.addGroupIdentityLink(groupId, identityLinkType);
				}
			  }
			  else if (value is ICollection)
			  {
				IEnumerator groupIdSet = ((ICollection) value).GetEnumerator();
				while (groupIdSet.hasNext())
				{
				  task.addGroupIdentityLink((string)groupIdSet.next(), identityLinkType);
				}
			  }
			  else
			  {
				throw new ActivitiException("Expression did not resolve to a string or collection of strings");
			  }
			}
		  }
		}
	  }

	  /// <summary>
	  /// Extract a candidate list from a string. 
	  /// </summary>
	  /// <param name="str"> </param>
	  /// <returns>  </returns>
	  protected internal virtual IList<string> extractCandidates(string str)
	  {
		return Arrays.asList(str.Split("[\\s]*,[\\s]*", true));
	  }

	  protected internal virtual Expression getActiveValue(Expression originalValue, string propertyName, ObjectNode taskElementProperties)
	  {
		Expression activeValue = originalValue;
		if (taskElementProperties != null)
		{
		  JsonNode overrideValueNode = taskElementProperties.get(propertyName);
		  if (overrideValueNode != null)
		  {
			if (overrideValueNode.Null)
			{
			  activeValue = null;
			}
			else
			{
			  activeValue = Context.ProcessEngineConfiguration.ExpressionManager.createExpression(overrideValueNode.asText());
			}
		  }
		}
		return activeValue;
	  }

	  protected internal virtual Set<Expression> getActiveValueSet(Set<Expression> originalValues, string propertyName, ObjectNode taskElementProperties)
	  {
		Set<Expression> activeValues = originalValues;
		if (taskElementProperties != null)
		{
		  JsonNode overrideValuesNode = taskElementProperties.get(propertyName);
		  if (overrideValuesNode != null)
		  {
			if (overrideValuesNode.Null || overrideValuesNode.Array == false || overrideValuesNode.size() == 0)
			{
			  activeValues = null;
			}
			else
			{
			  ExpressionManager expressionManager = Context.ProcessEngineConfiguration.ExpressionManager;
			  activeValues = new HashSet<Expression>();
			  foreach (JsonNode valueNode in overrideValuesNode)
			  {
				activeValues.add(expressionManager.createExpression(valueNode.asText()));
			  }
			}
		  }
		}
		return activeValues;
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual TaskDefinition TaskDefinition
	  {
		  get
		  {
			return taskDefinition;
		  }
	  }

	}

}