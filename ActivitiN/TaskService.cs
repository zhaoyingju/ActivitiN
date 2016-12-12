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
namespace org.activiti.engine
{


	using VariableInstance = org.activiti.engine.impl.persistence.entity.VariableInstance;
	using org.activiti.engine.query;
	using Attachment = org.activiti.engine.task.Attachment;
	using Comment = org.activiti.engine.task.Comment;
	using Event = org.activiti.engine.task.Event;
	using IdentityLink = org.activiti.engine.task.IdentityLink;
	using IdentityLinkType = org.activiti.engine.task.IdentityLinkType;
	using NativeTaskQuery = org.activiti.engine.task.NativeTaskQuery;
	using Task = org.activiti.engine.task.Task;
	using TaskQuery = org.activiti.engine.task.TaskQuery;

	/// <summary>
	/// Service which provides access to <seealso cref="Task"/> and form related operations.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public interface TaskService
	{

		/// <summary>
		/// Creates a new task that is not related to any process instance.
		/// 
		/// The returned task is transient and must be saved with <seealso cref="#saveTask(Task)"/> 'manually'.
		/// </summary>
	  Task newTask();

	  /// <summary>
	  /// create a new task with a user defined task id </summary>
	  Task newTask(string taskId);

		/// <summary>
		/// Saves the given task to the persistent data store. If the task is already
		/// present in the persistent store, it is updated.
		/// After a new task has been saved, the task instance passed into this method
		/// is updated with the id of the newly created task. </summary>
		/// <param name="task"> the task, cannot be null. </param>
		void saveTask(Task task);

		/// <summary>
		/// Deletes the given task, not deleting historic information that is related to this task. </summary>
		/// <param name="taskId"> The id of the task that will be deleted, cannot be null. If no task
		/// exists with the given taskId, the operation is ignored. </param>
		/// <exception cref="ActivitiObjectNotFoundException"> when the task with given id does not exist. </exception>
		/// <exception cref="ActivitiException"> when an error occurs while deleting the task or in case the task is part
		///   of a running process. </exception>
		void deleteTask(string taskId);

		/// <summary>
		/// Deletes all tasks of the given collection, not deleting historic information that is related 
		/// to these tasks. </summary>
		/// <param name="taskIds"> The id's of the tasks that will be deleted, cannot be null. All
		/// id's in the list that don't have an existing task will be ignored. </param>
		/// <exception cref="ActivitiObjectNotFoundException"> when one of the task does not exist. </exception>
		/// <exception cref="ActivitiException"> when an error occurs while deleting the tasks or in case one of the tasks
		///  is part of a running process. </exception>
		void deleteTasks(ICollection<string> taskIds);

	  /// <summary>
	  /// Deletes the given task. </summary>
	  /// <param name="taskId"> The id of the task that will be deleted, cannot be null. If no task
	  /// exists with the given taskId, the operation is ignored. </param>
	  /// <param name="cascade"> If cascade is true, also the historic information related to this task is deleted. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task with given id does not exist. </exception>
	  /// <exception cref="ActivitiException"> when an error occurs while deleting the task or in case the task is part
	  ///   of a running process. </exception>
	  void deleteTask(string taskId, bool cascade);

	  /// <summary>
	  /// Deletes all tasks of the given collection. </summary>
	  /// <param name="taskIds"> The id's of the tasks that will be deleted, cannot be null. All
	  /// id's in the list that don't have an existing task will be ignored. </param>
	  /// <param name="cascade"> If cascade is true, also the historic information related to this task is deleted. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when one of the tasks does not exist. </exception>
	  /// <exception cref="ActivitiException"> when an error occurs while deleting the tasks or in case one of the tasks
	  ///  is part of a running process. </exception>
	  void deleteTasks(ICollection<string> taskIds, bool cascade);

	  /// <summary>
	  /// Deletes the given task, not deleting historic information that is related to this task.. </summary>
	  /// <param name="taskId"> The id of the task that will be deleted, cannot be null. If no task
	  /// exists with the given taskId, the operation is ignored. </param>
	  /// <param name="deleteReason"> reason the task is deleted. Is recorded in history, if enabled. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task with given id does not exist. </exception>
	  /// <exception cref="ActivitiException"> when an error occurs while deleting the task or in case the task is part
	  ///  of a running process </exception>
	  void deleteTask(string taskId, string deleteReason);

	  /// <summary>
	  /// Deletes all tasks of the given collection, not deleting historic information that is related to these tasks. </summary>
	  /// <param name="taskIds"> The id's of the tasks that will be deleted, cannot be null. All
	  /// id's in the list that don't have an existing task will be ignored. </param>
	  /// <param name="deleteReason"> reason the task is deleted. Is recorded in history, if enabled. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when one of the tasks does not exist. </exception>
	  /// <exception cref="ActivitiException"> when an error occurs while deleting the tasks or in case one of the tasks
	  ///  is part of a running process. </exception>
	  void deleteTasks(ICollection<string> taskIds, string deleteReason);

		 /// <summary>
		 /// Claim responsibility for a task: the given user is made assignee for the task.
		 /// The difference with <seealso cref="#setAssignee(String, String)"/> is that here 
		 /// a check is done if the task already has a user assigned to it.
		 /// No check is done whether the user is known by the identity component. </summary>
		 /// <param name="taskId"> task to claim, cannot be null. </param>
		 /// <param name="userId"> user that claims the task. When userId is null the task is unclaimed,
		 /// assigned to no one. </param>
		 /// <exception cref="ActivitiObjectNotFoundException"> when the task doesn't exist. </exception>
		 /// <exception cref="ActivitiTaskAlreadyClaimedException"> when the task is already claimed by another user. </exception>
	  void claim(string taskId, string userId);

	  /// <summary>
	  /// A shortcut to <seealso cref="#claim"/> with null user in order to unclaim the task </summary>
	  /// <param name="taskId"> task to unclaim, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task doesn't exist.  </exception>
	  void unclaim(string taskId);

	  /// <summary>
	  /// Called when the task is successfully executed. </summary>
	  /// <param name="taskId"> the id of the task to complete, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when no task exists with the given id. </exception>
	  /// <exception cref="ActivitiException"> when this task is <seealso cref="DelegationState#PENDING"/> delegation. </exception>
	  void complete(string taskId);

	  /// <summary>
	  /// Delegates the task to another user. This means that the assignee is set 
	  /// and the delegation state is set to <seealso cref="DelegationState#PENDING"/>.
	  /// If no owner is set on the task, the owner is set to the current assignee
	  /// of the task. </summary>
	  /// <param name="taskId"> The id of the task that will be delegated. </param>
	  /// <param name="userId"> The id of the user that will be set as assignee. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when no task exists with the given id. </exception>
	  void delegateTask(string taskId, string userId);

	  /// <summary>
	  /// Marks that the assignee is done with this task and that it can be send back to the owner.  
	  /// Can only be called when this task is <seealso cref="DelegationState#PENDING"/> delegation.
	  /// After this method returns, the <seealso cref="Task#getDelegationState() delegationState"/> is set to <seealso cref="DelegationState#RESOLVED"/>. </summary>
	  /// <param name="taskId"> the id of the task to resolve, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when no task exists with the given id. </exception>
	  void resolveTask(string taskId);

	  /// <summary>
	  /// Marks that the assignee is done with this task providing the required
	  /// variables and that it can be sent back to the owner. Can only be called
	  /// when this task is <seealso cref="DelegationState#PENDING"/> delegation. After this
	  /// method returns, the <seealso cref="Task#getDelegationState() delegationState"/> is
	  /// set to <seealso cref="DelegationState#RESOLVED"/>.
	  /// </summary>
	  /// <param name="taskId"> </param>
	  /// <param name="variables"> </param>
	  /// <exception cref="ProcessEngineException"> When no task exists with the given id. </exception>
	  void resolveTask(string taskId, IDictionary<string, object> variables);

	  /// <summary>
	  /// Called when the task is successfully executed, 
	  /// and the required task parameters are given by the end-user. </summary>
	  /// <param name="taskId"> the id of the task to complete, cannot be null. </param>
	  /// <param name="variables"> task parameters. May be null or empty. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when no task exists with the given id. </exception>
	  void complete(string taskId, IDictionary<string, object> variables);

	  /// <summary>
	  /// Called when the task is successfully executed, 
	  /// and the required task paramaters are given by the end-user. </summary>
	  /// <param name="taskId"> the id of the task to complete, cannot be null. </param>
	  /// <param name="variables"> task parameters. May be null or empty. </param>
	  /// <param name="localScope"> If true, the provided variables will be stored task-local, 
	  /// 									 instead of process instance wide (which is the default for <seealso cref="#complete(String, Map)"/>). </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when no task exists with the given id. </exception>
	  void complete(string taskId, IDictionary<string, object> variables, bool localScope);

	  /// <summary>
	  /// Changes the assignee of the given task to the given userId.
	  /// No check is done whether the user is known by the identity component. </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="userId"> id of the user to use as assignee. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task or user doesn't exist. </exception>
	  void setAssignee(string taskId, string userId);

	  /// <summary>
	  /// Transfers ownership of this task to another user.
	  /// No check is done whether the user is known by the identity component. </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="userId"> of the person that is receiving ownership. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task or user doesn't exist. </exception>
	  void setOwner(string taskId, string userId);

	  /// <summary>
	  /// Retrieves the <seealso cref="IdentityLink"/>s associated with the given task.
	  /// Such an <seealso cref="IdentityLink"/> informs how a certain identity (eg. group or user)
	  /// is associated with a certain task (eg. as candidate, assignee, etc.)
	  /// </summary>
	  IList<IdentityLink> getIdentityLinksForTask(string taskId);

	  /// <summary>
	  /// Convenience shorthand for <seealso cref="#addUserIdentityLink(String, String, String)"/>; with type <seealso cref="IdentityLinkType#CANDIDATE"/> </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="userId"> id of the user to use as candidate, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task or user doesn't exist. </exception>
	  void addCandidateUser(string taskId, string userId);

	  /// <summary>
	  /// Convenience shorthand for <seealso cref="#addGroupIdentityLink(String, String, String)"/>; with type <seealso cref="IdentityLinkType#CANDIDATE"/> </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="groupId"> id of the group to use as candidate, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task or group doesn't exist. </exception>
	  void addCandidateGroup(string taskId, string groupId);

	  /// <summary>
	  /// Involves a user with a task. The type of identity link is defined by the
	  /// given identityLinkType. </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="userId"> id of the user involve, cannot be null. </param> </param>
	  /// <param name="identityLinkType"> type of identityLink, cannot be null (<seealso cref= <seealso cref="IdentityLinkType"/>). </seealso>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task or user doesn't exist. </exception>
	  void addUserIdentityLink(string taskId, string userId, string identityLinkType);

	  /// <summary>
	  /// Involves a group with a task. The type of identityLink is defined by the
	  /// given identityLink. </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="groupId"> id of the group to involve, cannot be null. </param> </param>
	  /// <param name="identityLinkType"> type of identity, cannot be null (<seealso cref= <seealso cref="IdentityLinkType"/>). </seealso>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task or group doesn't exist. </exception>
	  void addGroupIdentityLink(string taskId, string groupId, string identityLinkType);

	  /// <summary>
	  /// Convenience shorthand for <seealso cref="#deleteUserIdentityLink(String, String, String)"/>; with type <seealso cref="IdentityLinkType#CANDIDATE"/> </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="userId"> id of the user to use as candidate, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task or user doesn't exist. </exception>
	  void deleteCandidateUser(string taskId, string userId);

	  /// <summary>
	  /// Convenience shorthand for <seealso cref="#deleteGroupIdentityLink(String, String, String)"/>; with type <seealso cref="IdentityLinkType#CANDIDATE"/> </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="groupId"> id of the group to use as candidate, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task or group doesn't exist. </exception>
	  void deleteCandidateGroup(string taskId, string groupId);

	  /// <summary>
	  /// Removes the association between a user and a task for the given identityLinkType. </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="userId"> id of the user involve, cannot be null. </param> </param>
	  /// <param name="identityLinkType"> type of identityLink, cannot be null (<seealso cref= <seealso cref="IdentityLinkType"/>). </seealso>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task or user doesn't exist. </exception>
	  void deleteUserIdentityLink(string taskId, string userId, string identityLinkType);

	  /// <summary>
	  /// Removes the association between a group and a task for the given identityLinkType. </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="groupId"> id of the group to involve, cannot be null. </param> </param>
	  /// <param name="identityLinkType"> type of identity, cannot be null (<seealso cref= <seealso cref="IdentityLinkType"/>). </seealso>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task or group doesn't exist. </exception>
	  void deleteGroupIdentityLink(string taskId, string groupId, string identityLinkType);

	  /// <summary>
	  /// Changes the priority of the task.
	  /// 
	  /// Authorization: actual owner / business admin
	  /// </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="priority"> the new priority for the task. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the task doesn't exist. </exception>
	  void setPriority(string taskId, int priority);

	  /// <summary>
	  /// Changes the due date of the task
	  /// </summary>
	  /// <param name="taskId"> id of the task, cannot be null. </param>
	  /// <param name="dueDate"> the new due date for the task </param>
	  /// <exception cref="ActivitiException"> when the task doesn't exist. </exception>
	  void setDueDate(string taskId, DateTime dueDate);

	  /// <summary>
	  /// Returns a new <seealso cref="TaskQuery"/> that can be used to dynamically query tasks.
	  /// </summary>
	  TaskQuery createTaskQuery();

	  /// <summary>
	  /// Returns a new <seealso cref="NativeQuery"/> for tasks.
	  /// </summary>
	  NativeTaskQuery createNativeTaskQuery();

	  /// <summary>
	  /// set variable on a task.  If the variable is not already existing, it will be created in the 
	  /// most outer scope.  This means the process instance in case this task is related to an 
	  /// execution. 
	  /// </summary>
	  void setVariable(string taskId, string variableName, object value);

	  /// <summary>
	  /// set variables on a task.  If the variable is not already existing, it will be created in the 
	  /// most outer scope.  This means the process instance in case this task is related to an 
	  /// execution. 
	  /// </summary>
	  void setVariables<T1>(string taskId, IDictionary<T1> variables) where T1 : Object;

	  /// <summary>
	  /// set variable on a task.  If the variable is not already existing, it will be created in the 
	  /// task.  
	  /// </summary>
	  void setVariableLocal(string taskId, string variableName, object value);

	  /// <summary>
	  /// set variables on a task.  If the variable is not already existing, it will be created in the 
	  /// task.  
	  /// </summary>
	  void setVariablesLocal<T1>(string taskId, IDictionary<T1> variables) where T1 : Object;

	  /// <summary>
	  /// get a variables and search in the task scope and if available also the execution scopes. </summary>
	  object getVariable(string taskId, string variableName);

	  /// <summary>
	  /// get a variables and search in the task scope and if available also the execution scopes. </summary>
	  T getVariable<T>(string taskId, string variableName, Type variableClass);

	  /// <summary>
	  /// checks whether or not the task has a variable defined with the given name, in the task scope and if available also the execution scopes. </summary>
	  bool hasVariable(string taskId, string variableName);

	  /// <summary>
	  /// checks whether or not the task has a variable defined with the given name. </summary>
	  object getVariableLocal(string taskId, string variableName);

	  /// <summary>
	  /// checks whether or not the task has a variable defined with the given name. </summary>
	  T getVariableLocal<T>(string taskId, string variableName, Type variableClass);

	  /// <summary>
	  /// checks whether or not the task has a variable defined with the given name, local task scope only. </summary>
	  bool hasVariableLocal(string taskId, string variableName);

	  /// <summary>
	  /// get all variables and search in the task scope and if available also the execution scopes. 
	  /// If you have many variables and you only need a few, consider using <seealso cref="#getVariables(String, Collection)"/> 
	  /// for better performance.
	  /// </summary>
	  IDictionary<string, object> getVariables(string taskId);

	  /// <summary>
	  /// get all variables and search only in the task scope.
	  /// If you have many task local variables and you only need a few, consider using <seealso cref="#getVariablesLocal(String, Collection)"/> 
	  /// for better performance.
	  /// </summary>
	  IDictionary<string, object> getVariablesLocal(string taskId);

	  /// <summary>
	  /// get values for all given variableNames and search only in the task scope. </summary>
	  IDictionary<string, object> getVariables(string taskId, ICollection<string> variableNames);

	  /// <summary>
	  /// get a variable on a task </summary>
	  IDictionary<string, object> getVariablesLocal(string taskId, ICollection<string> variableNames);

	  /// <summary>
	  /// get all variables and search only in the task scope. </summary>
	  IList<VariableInstance> getVariableInstancesLocalByTaskIds(Set<string> taskIds);

	  /// <summary>
	  /// Removes the variable from the task.
	  /// When the variable does not exist, nothing happens.
	  /// </summary>
	  void removeVariable(string taskId, string variableName);

	  /// <summary>
	  /// Removes the variable from the task (not considering parent scopes).
	  /// When the variable does not exist, nothing happens.
	  /// </summary>
	  void removeVariableLocal(string taskId, string variableName);

	  /// <summary>
	  /// Removes all variables in the given collection from the task.
	  /// Non existing variable names are simply ignored.
	  /// </summary>
	  void removeVariables(string taskId, ICollection<string> variableNames);

	  /// <summary>
	  /// Removes all variables in the given collection from the task (not considering parent scopes).
	  /// Non existing variable names are simply ignored.
	  /// </summary>
	  void removeVariablesLocal(string taskId, ICollection<string> variableNames);

	  /// <summary>
	  /// Add a comment to a task and/or process instance. </summary>
	  Comment addComment(string taskId, string processInstanceId, string message);

	  /// <summary>
	  /// Add a comment to a task and/or process instance with a custom type. </summary>
	  Comment addComment(string taskId, string processInstanceId, string type, string message);

	  /// <summary>
	  /// Returns an individual comment with the given id. Returns null if no comment exists with the given id.
	  /// </summary>
	  Comment getComment(string commentId);

	  /// <summary>
	  /// Removes all comments from the provided task and/or process instance </summary>
	  void deleteComments(string taskId, string processInstanceId);

	  /// <summary>
	  /// Removes an individual comment with the given id. </summary>
	  /// <exception cref="ActivitiObjectNotFoundException"> when no comment exists with the given id.  </exception>
	  void deleteComment(string commentId);

	  /// <summary>
	  /// The comments related to the given task. </summary>
	  IList<Comment> getTaskComments(string taskId);

	  /// <summary>
	  /// The comments related to the given task of the given type. </summary>
	  IList<Comment> getTaskComments(string taskId, string type);

	  /// <summary>
	  /// All comments of a given type. </summary>
	  IList<Comment> getCommentsByType(string type);

	  /// <summary>
	  /// The all events related to the given task. </summary>
	  IList<Event> getTaskEvents(string taskId);

	  /// <summary>
	  /// Returns an individual event with the given id. Returns null if no event exists with the given id. </summary>
	  Event getEvent(string eventId);

	  /// <summary>
	  /// The comments related to the given process instance. </summary>
	  IList<Comment> getProcessInstanceComments(string processInstanceId);

	  /// <summary>
	  /// The comments related to the given process instance. </summary>
	  IList<Comment> getProcessInstanceComments(string processInstanceId, string type);

	  /// <summary>
	  /// Add a new attachment to a task and/or a process instance and use an input stream to provide the content </summary>
	  Attachment createAttachment(string attachmentType, string taskId, string processInstanceId, string attachmentName, string attachmentDescription, InputStream content);

	  /// <summary>
	  /// Add a new attachment to a task and/or a process instance and use an url as the content </summary>
	  Attachment createAttachment(string attachmentType, string taskId, string processInstanceId, string attachmentName, string attachmentDescription, string url);

	  /// <summary>
	  /// Update the name and decription of an attachment </summary>
	  void saveAttachment(Attachment attachment);

	  /// <summary>
	  /// Retrieve a particular attachment </summary>
	  Attachment getAttachment(string attachmentId);

	  /// <summary>
	  /// Retrieve stream content of a particular attachment </summary>
	  InputStream getAttachmentContent(string attachmentId);

	  /// <summary>
	  /// The list of attachments associated to a task </summary>
	  IList<Attachment> getTaskAttachments(string taskId);

	  /// <summary>
	  /// The list of attachments associated to a process instance </summary>
	  IList<Attachment> getProcessInstanceAttachments(string processInstanceId);

	  /// <summary>
	  /// Delete an attachment </summary>
	  void deleteAttachment(string attachmentId);

	  /// <summary>
	  /// The list of subtasks for this parent task </summary>
	  IList<Task> getSubTasks(string parentTaskId);
	}

}