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

	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using Authentication = org.activiti.engine.impl.identity.Authentication;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using AttachmentEntity = org.activiti.engine.impl.persistence.entity.AttachmentEntity;
	using ByteArrayEntity = org.activiti.engine.impl.persistence.entity.ByteArrayEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;
	using IoUtil = org.activiti.engine.impl.util.IoUtil;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;
	using Attachment = org.activiti.engine.task.Attachment;
	using Task = org.activiti.engine.task.Task;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	// Not Serializable
	public class CreateAttachmentCmd : Command<Attachment>
	{

	  protected internal string attachmentType;
	  protected internal string taskId;
	  protected internal string processInstanceId;
	  protected internal string attachmentName;
	  protected internal string attachmentDescription;
	  protected internal InputStream content;
	  protected internal string url;

	  public CreateAttachmentCmd(string attachmentType, string taskId, string processInstanceId, string attachmentName, string attachmentDescription, InputStream content, string url)
	  {
		this.attachmentType = attachmentType;
		this.taskId = taskId;
		this.processInstanceId = processInstanceId;
		this.attachmentName = attachmentName;
		this.attachmentDescription = attachmentDescription;
		this.content = content;
		this.url = url;
	  }

	  public virtual Attachment execute(CommandContext commandContext)
	  {

		verifyParameters(commandContext);

		AttachmentEntity attachment = new AttachmentEntity();
		attachment.Name = attachmentName;
		attachment.Description = attachmentDescription;
		attachment.Type = attachmentType;
		attachment.TaskId = taskId;
		attachment.ProcessInstanceId = processInstanceId;
		attachment.Url = url;
		attachment.UserId = Authentication.AuthenticatedUserId;
		attachment.Time = commandContext.ProcessEngineConfiguration.Clock.CurrentTime;

		DbSqlSession dbSqlSession = commandContext.DbSqlSession;
		dbSqlSession.insert(attachment);

		if (content != null)
		{
		  sbyte[] bytes = IoUtil.readInputStream(content, attachmentName);
		  ByteArrayEntity byteArray = ByteArrayEntity.createAndInsert(bytes);
		  attachment.ContentId = byteArray.Id;
		  attachment.setContent(byteArray);
		}

		commandContext.HistoryManager.createAttachmentComment(taskId, processInstanceId, attachmentName, true);

		if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			// Forced to fetch the process-instance to associate the right process definition
			string processDefinitionId = null;
			if (attachment.ProcessInstanceId != null)
			{
				ExecutionEntity process = commandContext.ExecutionEntityManager.findExecutionById(processInstanceId);
				if (process != null)
				{
					processDefinitionId = process.ProcessDefinitionId;
				}
			}

			commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, attachment, processInstanceId, processInstanceId, processDefinitionId));
			commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_INITIALIZED, attachment, processInstanceId, processInstanceId, processDefinitionId));
		}

		return attachment;
	  }

	  private void verifyParameters(CommandContext commandContext)
	  {
		if (taskId != null)
		{
		  TaskEntity task = commandContext.TaskEntityManager.findTaskById(taskId);

		  if (task == null)
		  {
			throw new ActivitiObjectNotFoundException("Cannot find task with id " + taskId, typeof(Task));
		  }

		  if (task.Suspended)
		  {
			throw new ActivitiException("It is not allowed to add an attachment to a suspended task");
		  }
		}

		if (processInstanceId != null)
		{
		  ExecutionEntity execution = commandContext.ExecutionEntityManager.findExecutionById(processInstanceId);

		  if (execution == null)
		  {
			throw new ActivitiObjectNotFoundException("Process instance " + processInstanceId + " doesn't exist", typeof(ProcessInstance));
		  }

		  if (execution.Suspended)
		  {
			throw new ActivitiException("It is not allowed to add an attachment to a suspended process instance");
		  }
		}
	  }

	}

}