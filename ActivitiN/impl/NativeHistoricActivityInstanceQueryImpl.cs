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
namespace org.activiti.engine.impl
{


	using HistoricActivityInstance = org.activiti.engine.history.HistoricActivityInstance;
	using NativeHistoricActivityInstanceQuery = org.activiti.engine.history.NativeHistoricActivityInstanceQuery;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;


	public class NativeHistoricActivityInstanceQueryImpl : AbstractNativeQuery<NativeHistoricActivityInstanceQuery, HistoricActivityInstance>, NativeHistoricActivityInstanceQuery
	{

	  private const long serialVersionUID = 1L;

	  public NativeHistoricActivityInstanceQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public NativeHistoricActivityInstanceQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }


	 //results ////////////////////////////////////////////////////////////////

	  public virtual IList<HistoricActivityInstance> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return commandContext.HistoricActivityInstanceEntityManager.findHistoricActivityInstancesByNativeQuery(parameterMap, firstResult, maxResults);
	  }

	  public virtual long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap)
	  {
		return commandContext.HistoricActivityInstanceEntityManager.findHistoricActivityInstanceCountByNativeQuery(parameterMap);
	  }

	}

}