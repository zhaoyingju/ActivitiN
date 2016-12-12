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
	/// @author Joram Barrez
	/// </summary>
	public class CompleteTaskCmd : NeedsActiveTaskCmd<Void>
	{

	  private const long serialVersionUID = 1L;
	  protected internal IDictionary<string, object> variables;
	  protected internal bool localScope;

	  public CompleteTaskCmd(string taskId, IDictionary<string, object> variables) : base(taskId)
	  {
		this.variables = variables;
	  }

	  public CompleteTaskCmd(string taskId, IDictionary<string, object> variables, bool localScope) : base(taskId)
	  {
		this.variables = variables;
		this.localScope = localScope;
	  }

	  protected internal virtual Void execute(CommandContext commandContext, TaskEntity task)
	  {
		if (variables != null)
		{
			if (localScope)
			{
				task.VariablesLocal = variables;
			}
			else if (task.ExecutionId != null)
			{
				task.ExecutionVariables = variables;
			}
			else
			{
				task.Variables = variables;
			}
		}

		task.complete(variables, localScope);
		return null;
	  }

	  protected internal override string SuspendedTaskException
	  {
		  get
		  {
			return "Cannot complete a suspended task";
		  }
	  }

	}

}