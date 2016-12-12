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
	using VariableInstance = org.activiti.engine.impl.persistence.entity.VariableInstance;
	using VariableInstanceEntity = org.activiti.engine.impl.persistence.entity.VariableInstanceEntity;

	/// <summary>
	/// @author Daisuke Yoshimoto
	/// </summary>
	[Serializable]
	public class GetTasksLocalVariablesCmd : Command<IList<VariableInstance>>
	{


	  private const long serialVersionUID = 4326522873059188196L;
	  protected internal Set<string> taskIds;

	  public GetTasksLocalVariablesCmd(Set<string> taskIds)
	  {
		this.taskIds = taskIds;
	  }

		public override IList<VariableInstance> execute(CommandContext commandContext)
		{
		  if (taskIds == null)
		  {
			throw new ActivitiIllegalArgumentException("taskIds is null");
		  }
		if (taskIds.Empty)
		{
		  throw new ActivitiIllegalArgumentException("Set of taskIds is empty");
		}

		IList<VariableInstance> instances = new List<VariableInstance>();
		IList<VariableInstanceEntity> entities = commandContext.VariableInstanceEntityManager.findVariableInstancesByTaskIds(taskIds);
		foreach (VariableInstanceEntity entity in entities)
		{
		  entity.Value;
		  instances.Add(entity);
		}

		return instances;
		}

	}
}