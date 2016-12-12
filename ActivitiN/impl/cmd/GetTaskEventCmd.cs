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

namespace org.activiti.engine.impl.cmd
{

	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using Event = org.activiti.engine.task.Event;


	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	[Serializable]
	public class GetTaskEventCmd : Command<Event>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string eventId;

	  public GetTaskEventCmd(string eventId)
	  {
		this.eventId = eventId;

		if (eventId == null)
		{
		  throw new ActivitiIllegalArgumentException("eventId is null");
		}
	  }

	  public virtual Event execute(CommandContext commandContext)
	  {
		return commandContext.CommentEntityManager.findEvent(eventId);
	  }
	}

}