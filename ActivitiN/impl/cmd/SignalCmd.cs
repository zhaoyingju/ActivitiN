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

	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class SignalCmd : NeedsActiveExecutionCmd<object>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string signalName;
	  protected internal object signalData;
	  protected internal readonly IDictionary<string, object> processVariables;

	  public SignalCmd(string executionId, string signalName, object signalData, IDictionary<string, object> processVariables) : base(executionId)
	  {
		this.signalName = signalName;
		this.signalData = signalData;
		this.processVariables = processVariables;
	  }

	  protected internal virtual object execute(CommandContext commandContext, ExecutionEntity execution)
	  {
		if (processVariables != null)
		{
		  execution.Variables = processVariables;
		}
		execution.signal(signalName, signalData);
		return null;
	  }

	  protected internal override string SuspendedExceptionMessage
	  {
		  get
		  {
			return "Cannot signal an execution that is suspended";
		  }
	  }

	}

}