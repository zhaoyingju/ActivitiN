using System;

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

namespace org.activiti.engine.impl.history.handler
{

	using DelegateTask = org.activiti.engine.@delegate.DelegateTask;
	using TaskListener = org.activiti.engine.@delegate.TaskListener;
	using Context = org.activiti.engine.impl.context.Context;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class UserTaskAssignmentHandler : TaskListener
	{

	  public virtual void notify(DelegateTask task)
	  {
	   Context.CommandContext.HistoryManager.recordTaskAssignment((TaskEntity) task);
	  }

	}

}