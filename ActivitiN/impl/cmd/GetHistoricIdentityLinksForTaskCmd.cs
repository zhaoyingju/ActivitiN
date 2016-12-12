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
namespace org.activiti.engine.impl.cmd
{


	using HistoricIdentityLink = org.activiti.engine.history.HistoricIdentityLink;
	using HistoricTaskInstance = org.activiti.engine.history.HistoricTaskInstance;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using HistoricIdentityLinkEntity = org.activiti.engine.impl.persistence.entity.HistoricIdentityLinkEntity;
	using HistoricTaskInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricTaskInstanceEntity;
	using IdentityLinkType = org.activiti.engine.task.IdentityLinkType;


	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	[Serializable]
	public class GetHistoricIdentityLinksForTaskCmd : Command<IList<HistoricIdentityLink>>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string taskId;
	  protected internal string processInstanceId;

	  public GetHistoricIdentityLinksForTaskCmd(string taskId, string processInstanceId)
	  {
		if (taskId == null && processInstanceId == null)
		{
		  throw new ActivitiIllegalArgumentException("taskId or processInstanceId is required");
		}
		this.taskId = taskId;
		this.processInstanceId = processInstanceId;
	  }

	  public virtual IList<HistoricIdentityLink> execute(CommandContext commandContext)
	  {
		if (taskId != null)
		{
		  return getLinksForTask(commandContext);
		}
		else
		{
		  return getLinksForProcessInstance(commandContext);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes" }) protected java.util.List<org.activiti.engine.history.HistoricIdentityLink> getLinksForTask(org.activiti.engine.impl.interceptor.CommandContext commandContext)
	  protected internal virtual IList<HistoricIdentityLink> getLinksForTask(CommandContext commandContext)
	  {
		HistoricTaskInstanceEntity task = commandContext.HistoricTaskInstanceEntityManager.findHistoricTaskInstanceById(taskId);

		if (task == null)
		{
		  throw new ActivitiObjectNotFoundException("No historic task exists with the given id: " + taskId, typeof(HistoricTaskInstance));
		}

		IList<HistoricIdentityLink> identityLinks = (IList) commandContext.HistoricIdentityLinkEntityManager.findHistoricIdentityLinksByTaskId(taskId);

		// Similar to GetIdentityLinksForTask, return assignee and owner as identity link
		if (task.Assignee != null)
		{
		  HistoricIdentityLinkEntity identityLink = new HistoricIdentityLinkEntity();
		  identityLink.UserId = task.Assignee;
		  identityLink.TaskId = task.Id;
		  identityLink.Type = IdentityLinkType.ASSIGNEE;
		  identityLinks.Add(identityLink);
		}
		if (task.Owner != null)
		{
		  HistoricIdentityLinkEntity identityLink = new HistoricIdentityLinkEntity();
		  identityLink.TaskId = task.Id;
		  identityLink.UserId = task.Owner;
		  identityLink.Type = IdentityLinkType.OWNER;
		  identityLinks.Add(identityLink);
		}

		return identityLinks;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes" }) protected java.util.List<org.activiti.engine.history.HistoricIdentityLink> getLinksForProcessInstance(org.activiti.engine.impl.interceptor.CommandContext commandContext)
	  protected internal virtual IList<HistoricIdentityLink> getLinksForProcessInstance(CommandContext commandContext)
	  {
	   return (IList) commandContext.HistoricIdentityLinkEntityManager.findHistoricIdentityLinksByProcessInstanceId(processInstanceId);
	  }

	}

}