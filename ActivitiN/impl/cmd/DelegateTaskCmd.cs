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
	/// </summary>
	public class DelegateTaskCmd : NeedsActiveTaskCmd<object>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string userId;

	  public DelegateTaskCmd(string taskId, string userId) : base(taskId)
	  {
		this.userId = userId;
	  }

	  protected internal virtual object execute(CommandContext commandContext, TaskEntity task)
	  {
		task.@delegate(userId);
		return null;
	  }

	  protected internal override string SuspendedTaskException
	  {
		  get
		  {
			return "Cannot delegate a suspended task";
		  }
	  }

	}

}