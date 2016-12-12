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

namespace org.activiti.engine.impl.persistence.entity
{

	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using Attachment = org.activiti.engine.task.Attachment;
	using Task = org.activiti.engine.task.Task;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class AttachmentEntityManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.task.Attachment> findAttachmentsByProcessInstanceId(String processInstanceId)
	  public virtual IList<Attachment> findAttachmentsByProcessInstanceId(string processInstanceId)
	  {
		checkHistoryEnabled();
		return DbSqlSession.selectList("selectAttachmentsByProcessInstanceId", processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.task.Attachment> findAttachmentsByTaskId(String taskId)
	  public virtual IList<Attachment> findAttachmentsByTaskId(string taskId)
	  {
		checkHistoryEnabled();
		return DbSqlSession.selectList("selectAttachmentsByTaskId", taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void deleteAttachmentsByTaskId(String taskId)
	  public virtual void deleteAttachmentsByTaskId(string taskId)
	  {
		checkHistoryEnabled();
		IList<AttachmentEntity> attachments = DbSqlSession.selectList("selectAttachmentsByTaskId", taskId);
		bool dispatchEvents = ProcessEngineConfiguration.EventDispatcher.Enabled;

		string processInstanceId = null;
		string processDefinitionId = null;
		string executionId = null;

		if (dispatchEvents && attachments != null && attachments.Count > 0)
		{
			// Forced to fetch the task to get hold of the process definition for event-dispatching, if available
			Task task = TaskManager.findTaskById(taskId);
			if (task != null)
			{
				processDefinitionId = task.ProcessDefinitionId;
				processInstanceId = task.ProcessInstanceId;
				executionId = task.ExecutionId;
			}
		}

		foreach (AttachmentEntity attachment in attachments)
		{
		  string contentId = attachment.ContentId;
		  if (contentId != null)
		  {
			ByteArrayManager.deleteByteArrayById(contentId);
		  }
		  DbSqlSession.delete(attachment);
		  if (dispatchEvents)
		  {
			  ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_DELETED, attachment, executionId, processInstanceId, processDefinitionId));
		  }
		}
	  }

	  protected internal virtual void checkHistoryEnabled()
	  {
		if (!HistoryManager.HistoryEnabled)
		{
		  throw new ActivitiException("In order to use attachments, history should be enabled");
		}
	  }
	}


}