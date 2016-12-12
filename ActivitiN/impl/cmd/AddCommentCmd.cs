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

namespace org.activiti.engine.impl.cmd
{

	using Authentication = org.activiti.engine.impl.identity.Authentication;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommentEntity = org.activiti.engine.impl.persistence.entity.CommentEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;
	using Execution = org.activiti.engine.runtime.Execution;
	using Comment = org.activiti.engine.task.Comment;
	using Event = org.activiti.engine.task.Event;
	using Task = org.activiti.engine.task.Task;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class AddCommentCmd : Command<Comment>
	{

	  protected internal string taskId;
	  protected internal string processInstanceId;
	  protected internal string type;
	  protected internal string message;

	  public AddCommentCmd(string taskId, string processInstanceId, string message)
	  {
		this.taskId = taskId;
		this.processInstanceId = processInstanceId;
		this.message = message;
	  }

	  public AddCommentCmd(string taskId, string processInstanceId, string type, string message)
	  {
		this.taskId = taskId;
		this.processInstanceId = processInstanceId;
		this.type = type;
		this.message = message;
	  }

	  public virtual Comment execute(CommandContext commandContext)
	  {

		// Validate task
		if (taskId != null)
		{
		  TaskEntity task = commandContext.TaskEntityManager.findTaskById(taskId);

		  if (task == null)
		  {
			throw new ActivitiObjectNotFoundException("Cannot find task with id " + taskId, typeof(Task));
		  }

		  if (task.Suspended)
		  {
			throw new ActivitiException(SuspendedTaskException);
		  }
		}

		if (processInstanceId != null)
		{
		  ExecutionEntity execution = commandContext.ExecutionEntityManager.findExecutionById(processInstanceId);

		  if (execution == null)
		  {
			throw new ActivitiObjectNotFoundException("execution " + processInstanceId + " doesn't exist", typeof(Execution));
		  }

		  if (execution.Suspended)
		  {
			throw new ActivitiException(SuspendedExceptionMessage);
		  }
		}

		string userId = Authentication.AuthenticatedUserId;
		CommentEntity comment = new CommentEntity();
		comment.UserId = userId;
		comment.Type = (type == null)? CommentEntity.TYPE_COMMENT : type;
		comment.Time = commandContext.ProcessEngineConfiguration.Clock.CurrentTime;
		comment.TaskId = taskId;
		comment.ProcessInstanceId = processInstanceId;
		comment.Action = org.activiti.engine.task.Event_Fields.ACTION_ADD_COMMENT;

		string eventMessage = message.replaceAll("\\s+", " ");
		if (eventMessage.Length > 163)
		{
		  eventMessage = eventMessage.Substring(0, 160) + "...";
		}
		comment.Message = eventMessage;

		comment.FullMessage = message;

		commandContext.CommentEntityManager.insert(comment);

		return comment;
	  }

	  protected internal virtual string SuspendedTaskException
	  {
		  get
		  {
			return "Cannot add a comment to a suspended task";
		  }
	  }

	  protected internal virtual string SuspendedExceptionMessage
	  {
		  get
		  {
			return "Cannot add a comment to a suspended execution";
		  }
	  }
	}

}