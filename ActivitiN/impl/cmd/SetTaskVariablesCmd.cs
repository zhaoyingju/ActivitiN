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
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class SetTaskVariablesCmd : NeedsActiveTaskCmd<object>
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: protected java.util.Map<String, ? extends Object> variables;
	  protected internal IDictionary<string, ?> variables;
	  protected internal bool isLocal;

	  public SetTaskVariablesCmd<T1>(string taskId, IDictionary<T1> variables, bool isLocal) where T1 : Object : base(taskId)
	  {
		this.taskId = taskId;
		this.variables = variables;
		this.isLocal = isLocal;
	  }

	  protected internal virtual object execute(CommandContext commandContext, TaskEntity task)
	  {

		if (isLocal)
		{
			if (variables != null)
			{
				foreach (string variableName in variables.Keys)
				{
					task.setVariableLocal(variableName, variables[variableName], false);
				}
			}

		}
		else
		{
			if (variables != null)
			{
				foreach (string variableName in variables.Keys)
				{
					task.setVariable(variableName, variables[variableName], false);
				}
			}
		}

		// ACT-1887: Force an update of the task's revision to prevent simultaneous inserts of the same
		// variable. If not, duplicate variables may occur since optimistic locking doesn't work on inserts
		task.forceUpdate();
		return null;
	  }

	  protected internal override string SuspendedTaskException
	  {
		  get
		  {
			return "Cannot add variables to a suspended task";
		  }
	  }

	}

}