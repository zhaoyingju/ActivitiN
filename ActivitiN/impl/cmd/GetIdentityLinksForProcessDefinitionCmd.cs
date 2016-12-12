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
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using IdentityLink = org.activiti.engine.task.IdentityLink;


	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	[Serializable]
	public class GetIdentityLinksForProcessDefinitionCmd : Command<IList<IdentityLink>>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string processDefinitionId;

	  public GetIdentityLinksForProcessDefinitionCmd(string processDefinitionId)
	  {
		this.processDefinitionId = processDefinitionId;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "rawtypes"}) public java.util.List<org.activiti.engine.task.IdentityLink> execute(org.activiti.engine.impl.interceptor.CommandContext commandContext)
	  public virtual IList<IdentityLink> execute(CommandContext commandContext)
	  {
		ProcessDefinitionEntity processDefinition = commandContext.ProcessDefinitionEntityManager.findProcessDefinitionById(processDefinitionId);

		if (processDefinition == null)
		{
		  throw new ActivitiObjectNotFoundException("Cannot find process definition with id " + processDefinitionId, typeof(ProcessDefinition));
		}

		IList<IdentityLink> identityLinks = (IList) processDefinition.IdentityLinks;
		return identityLinks;
	  }

	}

}