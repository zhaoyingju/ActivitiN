using System.Collections.Generic;

/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace org.activiti.engine.impl
{

	using HistoricActivityInstanceQuery = org.activiti.engine.history.HistoricActivityInstanceQuery;
	using HistoricDetailQuery = org.activiti.engine.history.HistoricDetailQuery;
	using HistoricIdentityLink = org.activiti.engine.history.HistoricIdentityLink;
	using HistoricProcessInstanceQuery = org.activiti.engine.history.HistoricProcessInstanceQuery;
	using HistoricTaskInstanceQuery = org.activiti.engine.history.HistoricTaskInstanceQuery;
	using HistoricVariableInstanceQuery = org.activiti.engine.history.HistoricVariableInstanceQuery;
	using NativeHistoricActivityInstanceQuery = org.activiti.engine.history.NativeHistoricActivityInstanceQuery;
	using NativeHistoricDetailQuery = org.activiti.engine.history.NativeHistoricDetailQuery;
	using NativeHistoricProcessInstanceQuery = org.activiti.engine.history.NativeHistoricProcessInstanceQuery;
	using NativeHistoricTaskInstanceQuery = org.activiti.engine.history.NativeHistoricTaskInstanceQuery;
	using NativeHistoricVariableInstanceQuery = org.activiti.engine.history.NativeHistoricVariableInstanceQuery;
	using ProcessInstanceHistoryLogQuery = org.activiti.engine.history.ProcessInstanceHistoryLogQuery;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DeleteHistoricProcessInstanceCmd = org.activiti.engine.impl.cmd.DeleteHistoricProcessInstanceCmd;
	using DeleteHistoricTaskInstanceCmd = org.activiti.engine.impl.cmd.DeleteHistoricTaskInstanceCmd;
	using GetHistoricIdentityLinksForTaskCmd = org.activiti.engine.impl.cmd.GetHistoricIdentityLinksForTaskCmd;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Bernd Ruecker (camunda)
	/// @author Christian Stettler
	/// </summary>
	public class HistoryServiceImpl : ServiceImpl, HistoryService
	{

		public HistoryServiceImpl()
		{

		}

		public HistoryServiceImpl(ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
		{
		}

	  public virtual HistoricProcessInstanceQuery createHistoricProcessInstanceQuery()
	  {
		return new HistoricProcessInstanceQueryImpl(commandExecutor);
	  }

	  public virtual HistoricActivityInstanceQuery createHistoricActivityInstanceQuery()
	  {
		return new HistoricActivityInstanceQueryImpl(commandExecutor);
	  }

	  public virtual HistoricTaskInstanceQuery createHistoricTaskInstanceQuery()
	  {
		return new HistoricTaskInstanceQueryImpl(commandExecutor, processEngineConfiguration.DatabaseType);
	  }

	  public virtual HistoricDetailQuery createHistoricDetailQuery()
	  {
		return new HistoricDetailQueryImpl(commandExecutor);
	  }

	  public override NativeHistoricDetailQuery createNativeHistoricDetailQuery()
	  {
		return new NativeHistoricDetailQueryImpl(commandExecutor);
	  }

	  public virtual HistoricVariableInstanceQuery createHistoricVariableInstanceQuery()
	  {
		return new HistoricVariableInstanceQueryImpl(commandExecutor);
	  }

	  public override NativeHistoricVariableInstanceQuery createNativeHistoricVariableInstanceQuery()
	  {
		return new NativeHistoricVariableInstanceQueryImpl(commandExecutor);
	  }

	  public virtual void deleteHistoricTaskInstance(string taskId)
	  {
		commandExecutor.execute(new DeleteHistoricTaskInstanceCmd(taskId));
	  }

	  public virtual void deleteHistoricProcessInstance(string processInstanceId)
	  {
		commandExecutor.execute(new DeleteHistoricProcessInstanceCmd(processInstanceId));
	  }

	  public virtual NativeHistoricProcessInstanceQuery createNativeHistoricProcessInstanceQuery()
	  {
		return new NativeHistoricProcessInstanceQueryImpl(commandExecutor);
	  }

	  public virtual NativeHistoricTaskInstanceQuery createNativeHistoricTaskInstanceQuery()
	  {
		return new NativeHistoricTaskInstanceQueryImpl(commandExecutor);
	  }

	  public virtual NativeHistoricActivityInstanceQuery createNativeHistoricActivityInstanceQuery()
	  {
		return new NativeHistoricActivityInstanceQueryImpl(commandExecutor);
	  }

	  public override IList<HistoricIdentityLink> getHistoricIdentityLinksForProcessInstance(string processInstanceId)
	  {
		return commandExecutor.execute(new GetHistoricIdentityLinksForTaskCmd(null, processInstanceId));
	  }

	  public override IList<HistoricIdentityLink> getHistoricIdentityLinksForTask(string taskId)
	  {
		return commandExecutor.execute(new GetHistoricIdentityLinksForTaskCmd(taskId, null));
	  }

	  public override ProcessInstanceHistoryLogQuery createProcessInstanceHistoryLogQuery(string processInstanceId)
	  {
		  return new ProcessInstanceHistoryLogQueryImpl(commandExecutor, processInstanceId);
	  }

	}

}