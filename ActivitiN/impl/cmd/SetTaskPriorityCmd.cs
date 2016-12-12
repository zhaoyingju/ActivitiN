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
	public class SetTaskPriorityCmd : NeedsActiveTaskCmd<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal int priority;

	  public SetTaskPriorityCmd(string taskId, int priority) : base(taskId)
	  {
		this.priority = priority;
	  }

	  protected internal virtual Void execute(CommandContext commandContext, TaskEntity task)
	  {
		task.setPriority(priority, true);
		return null;
	  }

	}

}