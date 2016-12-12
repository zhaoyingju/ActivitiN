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
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using Execution = org.activiti.engine.runtime.Execution;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class FindActiveActivityIdsCmd : Command<IList<string>>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string executionId;

	  public FindActiveActivityIdsCmd(string executionId)
	  {
		this.executionId = executionId;
	  }

	  public virtual IList<string> execute(CommandContext commandContext)
	  {
		if (executionId == null)
		{
		  throw new ActivitiIllegalArgumentException("executionId is null");
		}

		ExecutionEntity execution = commandContext.ExecutionEntityManager.findExecutionById(executionId);

		if (execution == null)
		{
		  throw new ActivitiObjectNotFoundException("execution " + executionId + " doesn't exist", typeof(Execution));
		}

		return execution.findActiveActivityIds();
	  }
	}

}