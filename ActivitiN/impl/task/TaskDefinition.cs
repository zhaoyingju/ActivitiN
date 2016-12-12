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
namespace org.activiti.engine.impl.task
{


	using Expression = org.activiti.engine.@delegate.Expression;
	using TaskListener = org.activiti.engine.@delegate.TaskListener;
	using TaskFormHandler = org.activiti.engine.impl.form.TaskFormHandler;

	/// <summary>
	/// Container for task definition information gathered at parsing time.
	/// 
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class TaskDefinition
	{

	  private const long serialVersionUID = 1L;

	  protected internal string key;

	  // assignment fields
	  protected internal Expression nameExpression;
	  protected internal Expression ownerExpression;
	  protected internal Expression descriptionExpression;
	  protected internal Expression assigneeExpression;
	  protected internal Set<Expression> candidateUserIdExpressions = new HashSet<Expression>();
	  protected internal Set<Expression> candidateGroupIdExpressions = new HashSet<Expression>();
	  protected internal Expression dueDateExpression;
	  protected internal Expression businessCalendarNameExpression;
	  protected internal Expression priorityExpression;
	  protected internal Expression categoryExpression;
	  protected internal IDictionary<string, Set<Expression>> customUserIdentityLinkExpressions = new Dictionary<string, Set<Expression>>();
	  protected internal IDictionary<string, Set<Expression>> customGroupIdentityLinkExpressions = new Dictionary<string, Set<Expression>>();
	  protected internal Expression skipExpression;

	  // form fields
	  protected internal TaskFormHandler taskFormHandler;
	  protected internal Expression formKeyExpression;

	  // task listeners
	  protected internal IDictionary<string, IList<TaskListener>> taskListeners = new Dictionary<string, IList<TaskListener>>();

	  public TaskDefinition(TaskFormHandler taskFormHandler)
	  {
		this.taskFormHandler = taskFormHandler;
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual Expression NameExpression
	  {
		  get
		  {
			return nameExpression;
		  }
		  set
		  {
			this.nameExpression = value;
		  }
	  }


	  public virtual Expression OwnerExpression
	  {
		  get
		  {
			return ownerExpression;
		  }
		  set
		  {
			this.ownerExpression = value;
		  }
	  }


	  public virtual Expression DescriptionExpression
	  {
		  get
		  {
			return descriptionExpression;
		  }
		  set
		  {
			this.descriptionExpression = value;
		  }
	  }


	  public virtual Expression AssigneeExpression
	  {
		  get
		  {
			return assigneeExpression;
		  }
		  set
		  {
			this.assigneeExpression = value;
		  }
	  }


	  public virtual Set<Expression> CandidateUserIdExpressions
	  {
		  get
		  {
			return candidateUserIdExpressions;
		  }
		  set
		  {
			this.candidateUserIdExpressions = value;
		  }
	  }

	  public virtual void addCandidateUserIdExpression(Expression userId)
	  {
		candidateUserIdExpressions.add(userId);
	  }


	  public virtual Set<Expression> CandidateGroupIdExpressions
	  {
		  get
		  {
			return candidateGroupIdExpressions;
		  }
		  set
		  {
			this.candidateGroupIdExpressions = value;
		  }
	  }

	  public virtual void addCandidateGroupIdExpression(Expression groupId)
	  {
		candidateGroupIdExpressions.add(groupId);
	  }


	  public virtual IDictionary<string, Set<Expression>> CustomUserIdentityLinkExpressions
	  {
		  get
		  {
			return customUserIdentityLinkExpressions;
		  }
	  }

	  public virtual void addCustomUserIdentityLinkExpression(string identityLinkType, Set<Expression> idList)
	  {
		  customUserIdentityLinkExpressions[identityLinkType] = idList;
	  }

	  public virtual IDictionary<string, Set<Expression>> CustomGroupIdentityLinkExpressions
	  {
		  get
		  {
			return customGroupIdentityLinkExpressions;
		  }
	  }

	  public virtual void addCustomGroupIdentityLinkExpression(string identityLinkType, Set<Expression> idList)
	  {
		  customGroupIdentityLinkExpressions[identityLinkType] = idList;
	  }

	  public virtual Expression PriorityExpression
	  {
		  get
		  {
			return priorityExpression;
		  }
		  set
		  {
			this.priorityExpression = value;
		  }
	  }


	  public virtual TaskFormHandler TaskFormHandler
	  {
		  get
		  {
			return taskFormHandler;
		  }
		  set
		  {
			this.taskFormHandler = value;
		  }
	  }


	  public virtual Expression FormKeyExpression
	  {
		  get
		  {
				return formKeyExpression;
		  }
		  set
		  {
				this.formKeyExpression = value;
		  }
	  }


		public virtual string Key
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


	  public virtual Expression DueDateExpression
	  {
		  get
		  {
			return dueDateExpression;
		  }
		  set
		  {
			this.dueDateExpression = value;
		  }
	  }


	  public virtual Expression BusinessCalendarNameExpression
	  {
		  get
		  {
			return businessCalendarNameExpression;
		  }
		  set
		  {
			this.businessCalendarNameExpression = value;
		  }
	  }


	  public virtual Expression CategoryExpression
	  {
		  get
		  {
				return categoryExpression;
		  }
		  set
		  {
				this.categoryExpression = value;
		  }
	  }


		public virtual IDictionary<string, IList<TaskListener>> TaskListeners
		{
			get
			{
			return taskListeners;
			}
			set
			{
			this.taskListeners = value;
			}
		}


	  public virtual IList<TaskListener> getTaskListener(string eventName)
	  {
		return taskListeners[eventName];
	  }

	  public virtual void addTaskListener(string eventName, TaskListener taskListener)
	  {
		if (org.activiti.engine.@delegate.TaskListener_Fields.EVENTNAME_ALL_EVENTS.Equals(eventName))
		{
		  // In order to prevent having to merge the "all" tasklisteners with the ones for a specific eventName,
		  // every time "getTaskListener()" is called, we add the listener explicitally to the individual lists
		  this.addTaskListener(org.activiti.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE, taskListener);
		  this.addTaskListener(org.activiti.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT, taskListener);
		  this.addTaskListener(org.activiti.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE, taskListener);
		  this.addTaskListener(org.activiti.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE, taskListener);

		}
		else
		{
		  IList<TaskListener> taskEventListeners = taskListeners[eventName];
		  if (taskEventListeners == null)
		  {
			taskEventListeners = new List<TaskListener>();
			taskListeners[eventName] = taskEventListeners;
		  }
		  taskEventListeners.Add(taskListener);
		}
	  }

	  public virtual Expression SkipExpression
	  {
		  get
		  {
			return skipExpression;
		  }
		  set
		  {
			this.skipExpression = value;
		  }
	  }


	}

}