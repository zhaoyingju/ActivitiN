using System;

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
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using AttachmentEntity = org.activiti.engine.impl.persistence.entity.AttachmentEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using Attachment = org.activiti.engine.task.Attachment;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class SaveAttachmentCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;
	  protected internal Attachment attachment;

	  public SaveAttachmentCmd(Attachment attachment)
	  {
		this.attachment = attachment;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		AttachmentEntity updateAttachment = commandContext.DbSqlSession.selectById(typeof(AttachmentEntity), attachment.Id);

		updateAttachment.Name = attachment.Name;
		updateAttachment.Description = attachment.Description;

		if (commandContext.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			// Forced to fetch the process-instance to associate the right process definition
			string processDefinitionId = null;
			string processInstanceId = updateAttachment.ProcessInstanceId;
			if (updateAttachment.ProcessInstanceId != null)
			{
				ExecutionEntity process = commandContext.ExecutionEntityManager.findExecutionById(processInstanceId);
				if (process != null)
				{
					processDefinitionId = process.ProcessDefinitionId;
				}
			}

			commandContext.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, attachment, processInstanceId, processInstanceId, processDefinitionId));
		}

		return null;
	  }
	}

}