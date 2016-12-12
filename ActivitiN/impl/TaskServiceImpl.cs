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


	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using AddCommentCmd = org.activiti.engine.impl.cmd.AddCommentCmd;
	using AddIdentityLinkCmd = org.activiti.engine.impl.cmd.AddIdentityLinkCmd;
	using ClaimTaskCmd = org.activiti.engine.impl.cmd.ClaimTaskCmd;
	using CompleteTaskCmd = org.activiti.engine.impl.cmd.CompleteTaskCmd;
	using CreateAttachmentCmd = org.activiti.engine.impl.cmd.CreateAttachmentCmd;
	using DelegateTaskCmd = org.activiti.engine.impl.cmd.DelegateTaskCmd;
	using DeleteAttachmentCmd = org.activiti.engine.impl.cmd.DeleteAttachmentCmd;
	using DeleteCommentCmd = org.activiti.engine.impl.cmd.DeleteCommentCmd;
	using DeleteIdentityLinkCmd = org.activiti.engine.impl.cmd.DeleteIdentityLinkCmd;
	using DeleteTaskCmd = org.activiti.engine.impl.cmd.DeleteTaskCmd;
	using GetAttachmentCmd = org.activiti.engine.impl.cmd.GetAttachmentCmd;
	using GetAttachmentContentCmd = org.activiti.engine.impl.cmd.GetAttachmentContentCmd;
	using GetCommentCmd = org.activiti.engine.impl.cmd.GetCommentCmd;
	using GetIdentityLinksForTaskCmd = org.activiti.engine.impl.cmd.GetIdentityLinksForTaskCmd;
	using GetProcessInstanceAttachmentsCmd = org.activiti.engine.impl.cmd.GetProcessInstanceAttachmentsCmd;
	using GetProcessInstanceCommentsCmd = org.activiti.engine.impl.cmd.GetProcessInstanceCommentsCmd;
	using GetSubTasksCmd = org.activiti.engine.impl.cmd.GetSubTasksCmd;
	using GetTaskAttachmentsCmd = org.activiti.engine.impl.cmd.GetTaskAttachmentsCmd;
	using GetTaskCommentsByTypeCmd = org.activiti.engine.impl.cmd.GetTaskCommentsByTypeCmd;
	using GetTaskCommentsCmd = org.activiti.engine.impl.cmd.GetTaskCommentsCmd;
	using GetTaskEventCmd = org.activiti.engine.impl.cmd.GetTaskEventCmd;
	using GetTaskEventsCmd = org.activiti.engine.impl.cmd.GetTaskEventsCmd;
	using GetTaskVariableCmd = org.activiti.engine.impl.cmd.GetTaskVariableCmd;
	using GetTaskVariablesCmd = org.activiti.engine.impl.cmd.GetTaskVariablesCmd;
	using GetTasksLocalVariablesCmd = org.activiti.engine.impl.cmd.GetTasksLocalVariablesCmd;
	using GetTypeCommentsCmd = org.activiti.engine.impl.cmd.GetTypeCommentsCmd;
	using HasTaskVariableCmd = org.activiti.engine.impl.cmd.HasTaskVariableCmd;
	using NewTaskCmd = org.activiti.engine.impl.cmd.NewTaskCmd;
	using RemoveTaskVariablesCmd = org.activiti.engine.impl.cmd.RemoveTaskVariablesCmd;
	using ResolveTaskCmd = org.activiti.engine.impl.cmd.ResolveTaskCmd;
	using SaveAttachmentCmd = org.activiti.engine.impl.cmd.SaveAttachmentCmd;
	using SaveTaskCmd = org.activiti.engine.impl.cmd.SaveTaskCmd;
	using SetTaskDueDateCmd = org.activiti.engine.impl.cmd.SetTaskDueDateCmd;
	using SetTaskPriorityCmd = org.activiti.engine.impl.cmd.SetTaskPriorityCmd;
	using SetTaskVariablesCmd = org.activiti.engine.impl.cmd.SetTaskVariablesCmd;
	using VariableInstance = org.activiti.engine.impl.persistence.entity.VariableInstance;
	using Attachment = org.activiti.engine.task.Attachment;
	using Comment = org.activiti.engine.task.Comment;
	using Event = org.activiti.engine.task.Event;
	using IdentityLink = org.activiti.engine.task.IdentityLink;
	using IdentityLinkType = org.activiti.engine.task.IdentityLinkType;
	using NativeTaskQuery = org.activiti.engine.task.NativeTaskQuery;
	using Task = org.activiti.engine.task.Task;
	using TaskQuery = org.activiti.engine.task.TaskQuery;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class TaskServiceImpl : ServiceImpl, TaskService
	{

		public TaskServiceImpl()
		{

		}

		public TaskServiceImpl(ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
		{
		}

	  public virtual Task newTask()
	  {
		return newTask(null);
	  }

	  public virtual Task newTask(string taskId)
	  {
		return commandExecutor.execute(new NewTaskCmd(taskId));
	  }

	  public virtual void saveTask(Task task)
	  {
		commandExecutor.execute(new SaveTaskCmd(task));
	  }

	  public virtual void deleteTask(string taskId)
	  {
		commandExecutor.execute(new DeleteTaskCmd(taskId, null, false));
	  }

	  public virtual void deleteTasks(ICollection<string> taskIds)
	  {
		commandExecutor.execute(new DeleteTaskCmd(taskIds, null, false));
	  }

	  public virtual void deleteTask(string taskId, bool cascade)
	  {
		commandExecutor.execute(new DeleteTaskCmd(taskId, null, cascade));
	  }

	  public virtual void deleteTasks(ICollection<string> taskIds, bool cascade)
	  {
		commandExecutor.execute(new DeleteTaskCmd(taskIds, null, cascade));
	  }

	  public override void deleteTask(string taskId, string deleteReason)
	  {
		commandExecutor.execute(new DeleteTaskCmd(taskId, deleteReason, false));
	  }

	  public override void deleteTasks(ICollection<string> taskIds, string deleteReason)
	  {
		commandExecutor.execute(new DeleteTaskCmd(taskIds, deleteReason, false));
	  }

	  public virtual void setAssignee(string taskId, string userId)
	  {
		commandExecutor.execute(new AddIdentityLinkCmd(taskId, userId, null, IdentityLinkType.ASSIGNEE));
	  }

	  public virtual void setOwner(string taskId, string userId)
	  {
		commandExecutor.execute(new AddIdentityLinkCmd(taskId, userId, null, IdentityLinkType.OWNER));
	  }

	  public virtual void addCandidateUser(string taskId, string userId)
	  {
		commandExecutor.execute(new AddIdentityLinkCmd(taskId, userId, null, IdentityLinkType.CANDIDATE));
	  }

	  public virtual void addCandidateGroup(string taskId, string groupId)
	  {
		commandExecutor.execute(new AddIdentityLinkCmd(taskId, null, groupId, IdentityLinkType.CANDIDATE));
	  }

	  public virtual void addUserIdentityLink(string taskId, string userId, string identityLinkType)
	  {
		commandExecutor.execute(new AddIdentityLinkCmd(taskId, userId, null, identityLinkType));
	  }

	  public virtual void addGroupIdentityLink(string taskId, string groupId, string identityLinkType)
	  {
		commandExecutor.execute(new AddIdentityLinkCmd(taskId, null, groupId, identityLinkType));
	  }

	  public virtual void deleteCandidateGroup(string taskId, string groupId)
	  {
		commandExecutor.execute(new DeleteIdentityLinkCmd(taskId, null, groupId, IdentityLinkType.CANDIDATE));
	  }

	  public virtual void deleteCandidateUser(string taskId, string userId)
	  {
		commandExecutor.execute(new DeleteIdentityLinkCmd(taskId, userId, null, IdentityLinkType.CANDIDATE));
	  }

	  public virtual void deleteGroupIdentityLink(string taskId, string groupId, string identityLinkType)
	  {
		commandExecutor.execute(new DeleteIdentityLinkCmd(taskId, null, groupId, identityLinkType));
	  }

	  public virtual void deleteUserIdentityLink(string taskId, string userId, string identityLinkType)
	  {
		commandExecutor.execute(new DeleteIdentityLinkCmd(taskId, userId, null, identityLinkType));
	  }

	  public virtual IList<IdentityLink> getIdentityLinksForTask(string taskId)
	  {
		return commandExecutor.execute(new GetIdentityLinksForTaskCmd(taskId));
	  }

	  public virtual void claim(string taskId, string userId)
	  {
		commandExecutor.execute(new ClaimTaskCmd(taskId, userId));
	  }

	  public virtual void unclaim(string taskId)
	  {
		commandExecutor.execute(new ClaimTaskCmd(taskId, null));
	  }

	  public virtual void complete(string taskId)
	  {
		commandExecutor.execute(new CompleteTaskCmd(taskId, null));
	  }

	  public virtual void complete(string taskId, IDictionary<string, object> variables)
	  {
		commandExecutor.execute(new CompleteTaskCmd(taskId, variables));
	  }

	  public virtual void complete(string taskId, IDictionary<string, object> variables, bool localScope)
	  {
		  commandExecutor.execute(new CompleteTaskCmd(taskId, variables, localScope));
	  }

	  public virtual void delegateTask(string taskId, string userId)
	  {
		commandExecutor.execute(new DelegateTaskCmd(taskId, userId));
	  }

	  public virtual void resolveTask(string taskId)
	  {
		commandExecutor.execute(new ResolveTaskCmd(taskId, null));
	  }

	  public virtual void resolveTask(string taskId, IDictionary<string, object> variables)
	  {
		commandExecutor.execute(new ResolveTaskCmd(taskId, variables));
	  }

	  public virtual void setPriority(string taskId, int priority)
	  {
		commandExecutor.execute(new SetTaskPriorityCmd(taskId, priority));
	  }

	  public virtual void setDueDate(string taskId, DateTime dueDate)
	  {
		commandExecutor.execute(new SetTaskDueDateCmd(taskId, dueDate));
	  }

	  public virtual TaskQuery createTaskQuery()
	  {
		return new TaskQueryImpl(commandExecutor, processEngineConfiguration.DatabaseType);
	  }

	  public virtual NativeTaskQuery createNativeTaskQuery()
	  {
		return new NativeTaskQueryImpl(commandExecutor);
	  }

	  public virtual IDictionary<string, object> getVariables(string taskId)
	  {
		return commandExecutor.execute(new GetTaskVariablesCmd(taskId, null, false));
	  }

	  public virtual IDictionary<string, object> getVariablesLocal(string taskId)
	  {
		return commandExecutor.execute(new GetTaskVariablesCmd(taskId, null, true));
	  }

	  public virtual IDictionary<string, object> getVariables(string taskId, ICollection<string> variableNames)
	  {
		return commandExecutor.execute(new GetTaskVariablesCmd(taskId, variableNames, false));
	  }

	  public virtual IDictionary<string, object> getVariablesLocal(string taskId, ICollection<string> variableNames)
	  {
		return commandExecutor.execute(new GetTaskVariablesCmd(taskId, variableNames, true));
	  }

	  public virtual object getVariable(string taskId, string variableName)
	  {
		return commandExecutor.execute(new GetTaskVariableCmd(taskId, variableName, false));
	  }

	  public override T getVariable<T>(string taskId, string variableName, Type variableClass)
	  {
		  return variableClass.cast(getVariable(taskId, variableName));
	  }

	  public virtual bool hasVariable(string taskId, string variableName)
	  {
		return commandExecutor.execute(new HasTaskVariableCmd(taskId, variableName, false));
	  }

	  public virtual object getVariableLocal(string taskId, string variableName)
	  {
		return commandExecutor.execute(new GetTaskVariableCmd(taskId, variableName, true));
	  }

	  public override T getVariableLocal<T>(string taskId, string variableName, Type variableClass)
	  {
		  return variableClass.cast(getVariableLocal(taskId, variableName));
	  }

	  public virtual IList<VariableInstance> getVariableInstancesLocalByTaskIds(Set<string> taskIds)
	  {
		return commandExecutor.execute(new GetTasksLocalVariablesCmd(taskIds));
	  }

	  public virtual bool hasVariableLocal(string taskId, string variableName)
	  {
		return commandExecutor.execute(new HasTaskVariableCmd(taskId, variableName, true));
	  }

	  public virtual void setVariable(string taskId, string variableName, object value)
	  {
		if (variableName == null)
		{
		  throw new ActivitiIllegalArgumentException("variableName is null");
		}
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[variableName] = value;
		commandExecutor.execute(new SetTaskVariablesCmd(taskId, variables, false));
	  }

	  public virtual void setVariableLocal(string taskId, string variableName, object value)
	  {
		if (variableName == null)
		{
		  throw new ActivitiIllegalArgumentException("variableName is null");
		}
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[variableName] = value;
		commandExecutor.execute(new SetTaskVariablesCmd(taskId, variables, true));
	  }

	  public virtual void setVariables<T1>(string taskId, IDictionary<T1> variables) where T1 : Object
	  {
		commandExecutor.execute(new SetTaskVariablesCmd(taskId, variables, false));
	  }

	  public virtual void setVariablesLocal<T1>(string taskId, IDictionary<T1> variables) where T1 : Object
	  {
		commandExecutor.execute(new SetTaskVariablesCmd(taskId, variables, true));
	  }

	  public virtual void removeVariable(string taskId, string variableName)
	  {
		ICollection<string> variableNames = new List<string>();
		variableNames.Add(variableName);
		commandExecutor.execute(new RemoveTaskVariablesCmd(taskId, variableNames, false));
	  }

	  public virtual void removeVariableLocal(string taskId, string variableName)
	  {
		ICollection<string> variableNames = new List<string>(1);
		variableNames.Add(variableName);
		commandExecutor.execute(new RemoveTaskVariablesCmd(taskId, variableNames, true));
	  }

	  public virtual void removeVariables(string taskId, ICollection<string> variableNames)
	  {
		commandExecutor.execute(new RemoveTaskVariablesCmd(taskId, variableNames, false));
	  }

	  public virtual void removeVariablesLocal(string taskId, ICollection<string> variableNames)
	  {
		commandExecutor.execute(new RemoveTaskVariablesCmd(taskId, variableNames, true));
	  }

	  public virtual Comment addComment(string taskId, string processInstance, string message)
	  {
		return commandExecutor.execute(new AddCommentCmd(taskId, processInstance, message));
	  }

	  public virtual Comment addComment(string taskId, string processInstance, string type, string message)
	  {
		return commandExecutor.execute(new AddCommentCmd(taskId, processInstance, type, message));
	  }

	  public override Comment getComment(string commentId)
	  {
		return commandExecutor.execute(new GetCommentCmd(commentId));
	  }

	  public override Event getEvent(string eventId)
	  {
		return commandExecutor.execute(new GetTaskEventCmd(eventId));
	  }

	  public virtual IList<Comment> getTaskComments(string taskId)
	  {
		return commandExecutor.execute(new GetTaskCommentsCmd(taskId));
	  }

	  public virtual IList<Comment> getTaskComments(string taskId, string type)
	  {
		return commandExecutor.execute(new GetTaskCommentsByTypeCmd(taskId, type));
	  }

	  public virtual IList<Comment> getCommentsByType(string type)
	  {
		return commandExecutor.execute(new GetTypeCommentsCmd(type));
	  }

	  public virtual IList<Event> getTaskEvents(string taskId)
	  {
		return commandExecutor.execute(new GetTaskEventsCmd(taskId));
	  }

	  public virtual IList<Comment> getProcessInstanceComments(string processInstanceId)
	  {
		return commandExecutor.execute(new GetProcessInstanceCommentsCmd(processInstanceId));
	  }

	  public virtual IList<Comment> getProcessInstanceComments(string processInstanceId, string type)
	  {
		return commandExecutor.execute(new GetProcessInstanceCommentsCmd(processInstanceId, type));
	  }

	  public virtual Attachment createAttachment(string attachmentType, string taskId, string processInstanceId, string attachmentName, string attachmentDescription, InputStream content)
	  {
		return commandExecutor.execute(new CreateAttachmentCmd(attachmentType, taskId, processInstanceId, attachmentName, attachmentDescription, content, null));
	  }

	  public virtual Attachment createAttachment(string attachmentType, string taskId, string processInstanceId, string attachmentName, string attachmentDescription, string url)
	  {
		return commandExecutor.execute(new CreateAttachmentCmd(attachmentType, taskId, processInstanceId, attachmentName, attachmentDescription, null, url));
	  }

	  public virtual InputStream getAttachmentContent(string attachmentId)
	  {
		return commandExecutor.execute(new GetAttachmentContentCmd(attachmentId));
	  }

	  public virtual void deleteAttachment(string attachmentId)
	  {
		commandExecutor.execute(new DeleteAttachmentCmd(attachmentId));
	  }

	  public virtual void deleteComments(string taskId, string processInstanceId)
	  {
		commandExecutor.execute(new DeleteCommentCmd(taskId, processInstanceId, null));
	  }

	  public override void deleteComment(string commentId)
	  {
		commandExecutor.execute(new DeleteCommentCmd(null, null, commentId));
	  }

	  public virtual Attachment getAttachment(string attachmentId)
	  {
		return commandExecutor.execute(new GetAttachmentCmd(attachmentId));
	  }

	  public virtual IList<Attachment> getTaskAttachments(string taskId)
	  {
		return commandExecutor.execute(new GetTaskAttachmentsCmd(taskId));
	  }

	  public virtual IList<Attachment> getProcessInstanceAttachments(string processInstanceId)
	  {
		return commandExecutor.execute(new GetProcessInstanceAttachmentsCmd(processInstanceId));
	  }

	  public virtual void saveAttachment(Attachment attachment)
	  {
		commandExecutor.execute(new SaveAttachmentCmd(attachment));
	  }

	  public virtual IList<Task> getSubTasks(string parentTaskId)
	  {
		return commandExecutor.execute(new GetSubTasksCmd(parentTaskId));
	  }

	}

}