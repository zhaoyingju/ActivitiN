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
	public class GetExecutionsVariablesCmd : Command<IList<VariableInstance>>
	{

	  private const long serialVersionUID = 1L;
	  protected internal Set<string> executionIds;

	  public GetExecutionsVariablesCmd(Set<string> executionIds)
	  {
		this.executionIds = executionIds;
	  }

	  public override IList<VariableInstance> execute(CommandContext commandContext)
	  {
		// Verify existance of executions
		if (executionIds == null)
		{
		  throw new ActivitiIllegalArgumentException("executionIds is null");
		}
		if (executionIds.Empty)
		{
			throw new ActivitiIllegalArgumentException("Set of executionIds is empty");
		}

		IList<VariableInstance> instances = new List<VariableInstance>();
		IList<VariableInstanceEntity> entities = commandContext.VariableInstanceEntityManager.findVariableInstancesByExecutionIds(executionIds);
		foreach (VariableInstanceEntity entity in entities)
		{
			entity.Value;
			instances.Add(entity);
		}
		return instances;
	  }

	}

}