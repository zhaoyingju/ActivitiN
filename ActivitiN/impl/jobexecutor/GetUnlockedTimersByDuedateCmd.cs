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
namespace org.activiti.engine.impl.jobexecutor
{


	using Context = org.activiti.engine.impl.context.Context;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using TimerEntity = org.activiti.engine.impl.persistence.entity.TimerEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class GetUnlockedTimersByDuedateCmd : Command<IList<TimerEntity>>
	{

	  protected internal DateTime duedate;
	  protected internal Page page;

	  public GetUnlockedTimersByDuedateCmd(DateTime duedate, Page page)
	  {
		  this.duedate = duedate;
		  this.page = page;
	  }

	  public virtual IList<TimerEntity> execute(CommandContext commandContext)
	  {
		return Context.CommandContext.JobEntityManager.findUnlockedTimersByDuedate(duedate, page);
	  }
	}

}