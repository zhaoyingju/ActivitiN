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
namespace org.activiti.engine.impl.bpmn.parser.handler
{


	using BpmnXMLConstants = org.activiti.bpmn.constants.BpmnXMLConstants;
	using ActivitiListener = org.activiti.bpmn.model.ActivitiListener;
	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using ImplementationType = org.activiti.bpmn.model.ImplementationType;
	using UserTask = org.activiti.bpmn.model.UserTask;
	using Expression = org.activiti.engine.@delegate.Expression;
	using TaskListener = org.activiti.engine.@delegate.TaskListener;
	using DueDateBusinessCalendar = org.activiti.engine.impl.calendar.DueDateBusinessCalendar;
	using ExpressionManager = org.activiti.engine.impl.el.ExpressionManager;
	using DefaultTaskFormHandler = org.activiti.engine.impl.form.DefaultTaskFormHandler;
	using TaskFormHandler = org.activiti.engine.impl.form.TaskFormHandler;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using TaskDefinition = org.activiti.engine.impl.task.TaskDefinition;
	using StringUtils = org.apache.commons.lang3.StringUtils;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class UserTaskParseHandler : AbstractActivityBpmnParseHandler<UserTask>
	{

	  public const string PROPERTY_TASK_DEFINITION = "taskDefinition";

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(UserTask);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, UserTask userTask)
	  {
		ActivityImpl activity = createActivityOnCurrentScope(bpmnParse, userTask, BpmnXMLConstants.ELEMENT_TASK_USER);

		activity.Async = userTask.Asynchronous;
		activity.Exclusive = !userTask.NotExclusive;

		TaskDefinition taskDefinition = parseTaskDefinition(bpmnParse, userTask, userTask.Id, (ProcessDefinitionEntity) bpmnParse.CurrentScope.ProcessDefinition);
		activity.setProperty(PROPERTY_TASK_DEFINITION, taskDefinition);
		activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createUserTaskActivityBehavior(userTask, taskDefinition);
	  }

	  public virtual TaskDefinition parseTaskDefinition(BpmnParse bpmnParse, UserTask userTask, string taskDefinitionKey, ProcessDefinitionEntity processDefinition)
	  {
		TaskFormHandler taskFormHandler = new DefaultTaskFormHandler();
		taskFormHandler.parseConfiguration(userTask.FormProperties, userTask.FormKey, bpmnParse.Deployment, processDefinition);

		TaskDefinition taskDefinition = new TaskDefinition(taskFormHandler);

		taskDefinition.Key = taskDefinitionKey;
		processDefinition.TaskDefinitions[taskDefinitionKey] = taskDefinition;
		ExpressionManager expressionManager = bpmnParse.ExpressionManager;

		if (StringUtils.isNotEmpty(userTask.Name))
		{
		  taskDefinition.NameExpression = expressionManager.createExpression(userTask.Name);
		}

		if (StringUtils.isNotEmpty(userTask.Documentation))
		{
		  taskDefinition.DescriptionExpression = expressionManager.createExpression(userTask.Documentation);
		}

		if (StringUtils.isNotEmpty(userTask.Assignee))
		{
		  taskDefinition.AssigneeExpression = expressionManager.createExpression(userTask.Assignee);
		}
		if (StringUtils.isNotEmpty(userTask.Owner))
		{
		  taskDefinition.OwnerExpression = expressionManager.createExpression(userTask.Owner);
		}
		foreach (string candidateUser in userTask.CandidateUsers)
		{
		  taskDefinition.addCandidateUserIdExpression(expressionManager.createExpression(candidateUser));
		}
		foreach (string candidateGroup in userTask.CandidateGroups)
		{
		  taskDefinition.addCandidateGroupIdExpression(expressionManager.createExpression(candidateGroup));
		}

		// Activiti custom extension

		// Task listeners
		foreach (ActivitiListener taskListener in userTask.TaskListeners)
		{
		  taskDefinition.addTaskListener(taskListener.Event, createTaskListener(bpmnParse, taskListener, userTask.Id));
		}

		// Due date
		if (StringUtils.isNotEmpty(userTask.DueDate))
		{
		  taskDefinition.DueDateExpression = expressionManager.createExpression(userTask.DueDate);
		}

		// Business calendar name
		if (StringUtils.isNotEmpty(userTask.BusinessCalendarName))
		{
		  taskDefinition.BusinessCalendarNameExpression = expressionManager.createExpression(userTask.BusinessCalendarName);
		}
		else
		{
		  taskDefinition.BusinessCalendarNameExpression = expressionManager.createExpression(DueDateBusinessCalendar.NAME);
		}

		// Category
		if (StringUtils.isNotEmpty(userTask.Category))
		{
			taskDefinition.CategoryExpression = expressionManager.createExpression(userTask.Category);
		}

		// Priority
		if (StringUtils.isNotEmpty(userTask.Priority))
		{
		  taskDefinition.PriorityExpression = expressionManager.createExpression(userTask.Priority);
		}

		if (StringUtils.isNotEmpty(userTask.FormKey))
		{
			taskDefinition.FormKeyExpression = expressionManager.createExpression(userTask.FormKey);
		}

		// CustomUserIdentityLinks
		foreach (string customUserIdentityLinkType in userTask.CustomUserIdentityLinks.Keys)
		{
			Set<Expression> userIdentityLinkExpression = new HashSet<Expression>();
			foreach (string userIdentityLink in userTask.CustomUserIdentityLinks.get(customUserIdentityLinkType))
			{
				userIdentityLinkExpression.add(expressionManager.createExpression(userIdentityLink));
			}
			taskDefinition.addCustomUserIdentityLinkExpression(customUserIdentityLinkType, userIdentityLinkExpression);
		}

		// CustomGroupIdentityLinks
		foreach (string customGroupIdentityLinkType in userTask.CustomGroupIdentityLinks.Keys)
		{
			Set<Expression> groupIdentityLinkExpression = new HashSet<Expression>();
			foreach (string groupIdentityLink in userTask.CustomGroupIdentityLinks.get(customGroupIdentityLinkType))
			{
				groupIdentityLinkExpression.add(expressionManager.createExpression(groupIdentityLink));
			}
			taskDefinition.addCustomGroupIdentityLinkExpression(customGroupIdentityLinkType, groupIdentityLinkExpression);
		}

		if (StringUtils.isNotEmpty(userTask.SkipExpression))
		{
		  taskDefinition.SkipExpression = expressionManager.createExpression(userTask.SkipExpression);
		}

		return taskDefinition;
	  }

	  protected internal virtual TaskListener createTaskListener(BpmnParse bpmnParse, ActivitiListener activitiListener, string taskId)
	  {
		TaskListener taskListener = null;

		if (ImplementationType.IMPLEMENTATION_TYPE_CLASS.equalsIgnoreCase(activitiListener.ImplementationType))
		{
		  taskListener = bpmnParse.ListenerFactory.createClassDelegateTaskListener(activitiListener);
		}
		else if (ImplementationType.IMPLEMENTATION_TYPE_EXPRESSION.equalsIgnoreCase(activitiListener.ImplementationType))
		{
		  taskListener = bpmnParse.ListenerFactory.createExpressionTaskListener(activitiListener);
		}
		else if (ImplementationType.IMPLEMENTATION_TYPE_DELEGATEEXPRESSION.equalsIgnoreCase(activitiListener.ImplementationType))
		{
		  taskListener = bpmnParse.ListenerFactory.createDelegateExpressionTaskListener(activitiListener);
		}
		return taskListener;
	  }
	}

}