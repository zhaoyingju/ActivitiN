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
namespace org.activiti.engine.impl.cmd
{


	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommentEntity = org.activiti.engine.impl.persistence.entity.CommentEntity;
	using CommentEntityManager = org.activiti.engine.impl.persistence.entity.CommentEntityManager;
	using Comment = org.activiti.engine.task.Comment;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class DeleteCommentCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string taskId;
	  protected internal string processInstanceId;
	  protected internal string commentId;

	  public DeleteCommentCmd(string taskId, string processInstanceId, string commentId)
	  {
		this.taskId = taskId;
		this.processInstanceId = processInstanceId;
		this.commentId = commentId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		CommentEntityManager commentManager = commandContext.CommentEntityManager;

		if (commentId != null)
		{
		  // Delete for an individual comment
		  Comment comment = commentManager.findComment(commentId);
		  if (comment == null)
		  {
			throw new ActivitiObjectNotFoundException("Comment with id '" + commentId + "' doesn't exists.", typeof(Comment));
		  }
		  commentManager.delete((CommentEntity) comment);

		}
		else
		{
		  // Delete all comments on a task of process
		  List<Comment> comments = new List<Comment>();
		  if (processInstanceId != null)
		  {
			comments.AddRange(commentManager.findCommentsByProcessInstanceId(processInstanceId));
		  }
		  if (taskId != null)
		  {
			comments.AddRange(commentManager.findCommentsByTaskId(taskId));
		  }

		  foreach (Comment comment in comments)
		  {
			commentManager.delete((CommentEntity)comment);
		  }
		}
		return null;
	  }
	}

}