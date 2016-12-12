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
namespace org.activiti.engine.impl
{


	using ActivitiEvent = org.activiti.engine.@delegate.@event.ActivitiEvent;
	using ActivitiEventListener = org.activiti.engine.@delegate.@event.ActivitiEventListener;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using FormData = org.activiti.engine.form.FormData;
	using ActivateProcessInstanceCmd = org.activiti.engine.impl.cmd.ActivateProcessInstanceCmd;
	using AddEventListenerCommand = org.activiti.engine.impl.cmd.AddEventListenerCommand;
	using AddIdentityLinkForProcessInstanceCmd = org.activiti.engine.impl.cmd.AddIdentityLinkForProcessInstanceCmd;
	using DeleteIdentityLinkForProcessInstanceCmd = org.activiti.engine.impl.cmd.DeleteIdentityLinkForProcessInstanceCmd;
	using DeleteProcessInstanceCmd = org.activiti.engine.impl.cmd.DeleteProcessInstanceCmd;
	using DispatchEventCommand = org.activiti.engine.impl.cmd.DispatchEventCommand;
	using FindActiveActivityIdsCmd = org.activiti.engine.impl.cmd.FindActiveActivityIdsCmd;
	using GetExecutionVariableCmd = org.activiti.engine.impl.cmd.GetExecutionVariableCmd;
	using GetExecutionVariableInstanceCmd = org.activiti.engine.impl.cmd.GetExecutionVariableInstanceCmd;
	using GetExecutionVariableInstancesCmd = org.activiti.engine.impl.cmd.GetExecutionVariableInstancesCmd;
	using GetExecutionVariablesCmd = org.activiti.engine.impl.cmd.GetExecutionVariablesCmd;
	using GetExecutionsVariablesCmd = org.activiti.engine.impl.cmd.GetExecutionsVariablesCmd;
	using GetIdentityLinksForProcessInstanceCmd = org.activiti.engine.impl.cmd.GetIdentityLinksForProcessInstanceCmd;
	using GetProcessInstanceEventsCmd = org.activiti.engine.impl.cmd.GetProcessInstanceEventsCmd;
	using GetStartFormCmd = org.activiti.engine.impl.cmd.GetStartFormCmd;
	using HasExecutionVariableCmd = org.activiti.engine.impl.cmd.HasExecutionVariableCmd;
	using MessageEventReceivedCmd = org.activiti.engine.impl.cmd.MessageEventReceivedCmd;
	using RemoveEventListenerCommand = org.activiti.engine.impl.cmd.RemoveEventListenerCommand;
	using RemoveExecutionVariablesCmd = org.activiti.engine.impl.cmd.RemoveExecutionVariablesCmd;
	using SetExecutionVariablesCmd = org.activiti.engine.impl.cmd.SetExecutionVariablesCmd;
	using SetProcessInstanceBusinessKeyCmd = org.activiti.engine.impl.cmd.SetProcessInstanceBusinessKeyCmd;
	using SetProcessInstanceNameCmd = org.activiti.engine.impl.cmd.SetProcessInstanceNameCmd;
	using SignalCmd = org.activiti.engine.impl.cmd.SignalCmd;
	using SignalEventReceivedCmd = org.activiti.engine.impl.cmd.SignalEventReceivedCmd;
	using StartProcessInstanceByMessageCmd = org.activiti.engine.impl.cmd.StartProcessInstanceByMessageCmd;
	using org.activiti.engine.impl.cmd;
	using SuspendProcessInstanceCmd = org.activiti.engine.impl.cmd.SuspendProcessInstanceCmd;
	using VariableInstance = org.activiti.engine.impl.persistence.entity.VariableInstance;
	using ProcessInstanceBuilderImpl = org.activiti.engine.impl.runtime.ProcessInstanceBuilderImpl;
	using ExecutionQuery = org.activiti.engine.runtime.ExecutionQuery;
	using NativeExecutionQuery = org.activiti.engine.runtime.NativeExecutionQuery;
	using NativeProcessInstanceQuery = org.activiti.engine.runtime.NativeProcessInstanceQuery;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;
	using ProcessInstanceBuilder = org.activiti.engine.runtime.ProcessInstanceBuilder;
	using ProcessInstanceQuery = org.activiti.engine.runtime.ProcessInstanceQuery;
	using Event = org.activiti.engine.task.Event;
	using IdentityLink = org.activiti.engine.task.IdentityLink;
	using IdentityLinkType = org.activiti.engine.task.IdentityLinkType;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class RuntimeServiceImpl : ServiceImpl, RuntimeService
	{

	  public virtual ProcessInstance startProcessInstanceByKey(string processDefinitionKey)
	  {
		return commandExecutor.execute(new StartProcessInstanceCmd<ProcessInstance>(processDefinitionKey, null, null, null));
	  }

	  public virtual ProcessInstance startProcessInstanceByKey(string processDefinitionKey, string businessKey)
	  {
		return commandExecutor.execute(new StartProcessInstanceCmd<ProcessInstance>(processDefinitionKey, null, businessKey, null));
	  }

	  public virtual ProcessInstance startProcessInstanceByKey(string processDefinitionKey, IDictionary<string, object> variables)
	  {
		return commandExecutor.execute(new StartProcessInstanceCmd<ProcessInstance>(processDefinitionKey, null, null, variables));
	  }

	  public virtual ProcessInstance startProcessInstanceByKey(string processDefinitionKey, string businessKey, IDictionary<string, object> variables)
	  {
		return commandExecutor.execute(new StartProcessInstanceCmd<ProcessInstance>(processDefinitionKey, null, businessKey, variables));
	  }

	  public virtual ProcessInstance startProcessInstanceByKeyAndTenantId(string processDefinitionKey, string tenantId)
	  {
		  return commandExecutor.execute(new StartProcessInstanceCmd<ProcessInstance>(processDefinitionKey, null, null, null, tenantId));
	  }

	  public virtual ProcessInstance startProcessInstanceByKeyAndTenantId(string processDefinitionKey, string businessKey, string tenantId)
	  {
		  return commandExecutor.execute(new StartProcessInstanceCmd<ProcessInstance>(processDefinitionKey, null, businessKey, null, tenantId));
	  }

	  public virtual ProcessInstance startProcessInstanceByKeyAndTenantId(string processDefinitionKey, IDictionary<string, object> variables, string tenantId)
	  {
		  return commandExecutor.execute(new StartProcessInstanceCmd<ProcessInstance>(processDefinitionKey, null, null, variables, tenantId));
	  }

		public virtual ProcessInstance startProcessInstanceByKeyAndTenantId(string processDefinitionKey, string businessKey, IDictionary<string, object> variables, string tenantId)
		{
			return commandExecutor.execute(new StartProcessInstanceCmd<ProcessInstance>(processDefinitionKey, null, businessKey, variables, tenantId));
		}

	  public virtual ProcessInstance startProcessInstanceById(string processDefinitionId)
	  {
		return commandExecutor.execute(new StartProcessInstanceCmd<ProcessInstance>(null, processDefinitionId, null, null));
	  }

	  public virtual ProcessInstance startProcessInstanceById(string processDefinitionId, string businessKey)
	  {
		return commandExecutor.execute(new StartProcessInstanceCmd<ProcessInstance>(null, processDefinitionId, businessKey, null));
	  }

	  public virtual ProcessInstance startProcessInstanceById(string processDefinitionId, IDictionary<string, object> variables)
	  {
		return commandExecutor.execute(new StartProcessInstanceCmd<ProcessInstance>(null, processDefinitionId, null, variables));
	  }

	  public virtual ProcessInstance startProcessInstanceById(string processDefinitionId, string businessKey, IDictionary<string, object> variables)
	  {
		return commandExecutor.execute(new StartProcessInstanceCmd<ProcessInstance>(null, processDefinitionId, businessKey, variables));
	  }

	  public virtual void deleteProcessInstance(string processInstanceId, string deleteReason)
	  {
		commandExecutor.execute(new DeleteProcessInstanceCmd(processInstanceId, deleteReason));
	  }

	  public virtual ExecutionQuery createExecutionQuery()
	  {
		return new ExecutionQueryImpl(commandExecutor);
	  }

	  public virtual NativeExecutionQuery createNativeExecutionQuery()
	  {
		return new NativeExecutionQueryImpl(commandExecutor);
	  }

	  public virtual NativeProcessInstanceQuery createNativeProcessInstanceQuery()
	  {
		return new NativeProcessInstanceQueryImpl(commandExecutor);
	  }

	  public virtual void updateBusinessKey(string processInstanceId, string businessKey)
	  {
		commandExecutor.execute(new SetProcessInstanceBusinessKeyCmd(processInstanceId, businessKey));
	  }

	  public virtual IDictionary<string, object> getVariables(string executionId)
	  {
		return commandExecutor.execute(new GetExecutionVariablesCmd(executionId, null, false));
	  }

	  public virtual IDictionary<string, VariableInstance> getVariableInstances(string executionId)
	  {
		return commandExecutor.execute(new GetExecutionVariableInstancesCmd(executionId, null, false));
	  }

	  public virtual IList<VariableInstance> getVariableInstancesByExecutionIds(Set<string> executionIds)
	  {
		return commandExecutor.execute(new GetExecutionsVariablesCmd(executionIds));
	  }

	  public virtual IDictionary<string, VariableInstance> getVariableInstances(string executionId, string locale, bool withLocalizationFallback)
	  {
		return commandExecutor.execute(new GetExecutionVariableInstancesCmd(executionId, null, false, locale, withLocalizationFallback));
	  }

	  public virtual IDictionary<string, object> getVariablesLocal(string executionId)
	  {
		return commandExecutor.execute(new GetExecutionVariablesCmd(executionId, null, true));
	  }

	  public virtual IDictionary<string, VariableInstance> getVariableInstancesLocal(string executionId)
	  {
		return commandExecutor.execute(new GetExecutionVariableInstancesCmd(executionId, null, true));
	  }

	  public virtual IDictionary<string, VariableInstance> getVariableInstancesLocal(string executionId, string locale, bool withLocalizationFallback)
	  {
		return commandExecutor.execute(new GetExecutionVariableInstancesCmd(executionId, null, true, locale, withLocalizationFallback));
	  }

	  public virtual IDictionary<string, object> getVariables(string executionId, ICollection<string> variableNames)
	  {
		return commandExecutor.execute(new GetExecutionVariablesCmd(executionId, variableNames, false));
	  }

	  public virtual IDictionary<string, VariableInstance> getVariableInstances(string executionId, ICollection<string> variableNames)
	  {
		return commandExecutor.execute(new GetExecutionVariableInstancesCmd(executionId, variableNames, false));
	  }

	  public virtual IDictionary<string, VariableInstance> getVariableInstances(string executionId, ICollection<string> variableNames, string locale, bool withLocalizationFallback)
	  {
		return commandExecutor.execute(new GetExecutionVariableInstancesCmd(executionId, variableNames, false, locale, withLocalizationFallback));
	  }

	  public virtual IDictionary<string, object> getVariablesLocal(string executionId, ICollection<string> variableNames)
	  {
		return commandExecutor.execute(new GetExecutionVariablesCmd(executionId, variableNames, true));
	  }

	  public virtual IDictionary<string, VariableInstance> getVariableInstancesLocal(string executionId, ICollection<string> variableNames)
	  {
		return commandExecutor.execute(new GetExecutionVariableInstancesCmd(executionId, variableNames, false));
	  }

	  public virtual IDictionary<string, VariableInstance> getVariableInstancesLocal(string executionId, ICollection<string> variableNames, string locale, bool withLocalizationFallback)
	  {
		return commandExecutor.execute(new GetExecutionVariableInstancesCmd(executionId, variableNames, false, locale, withLocalizationFallback));
	  }

	  public virtual object getVariable(string executionId, string variableName)
	  {
		return commandExecutor.execute(new GetExecutionVariableCmd(executionId, variableName, false));
	  }

	  public virtual VariableInstance getVariableInstance(string executionId, string variableName)
	  {
		return commandExecutor.execute(new GetExecutionVariableInstanceCmd(executionId, variableName, false));
	  }

	  public virtual VariableInstance getVariableInstance(string executionId, string variableName, string locale, bool withLocalizationFallback)
	  {
		return commandExecutor.execute(new GetExecutionVariableInstanceCmd(executionId, variableName, false, locale, withLocalizationFallback));
	  }

	  public override T getVariable<T>(string executionId, string variableName, Type variableClass)
	  {
		  return variableClass.cast(getVariable(executionId, variableName));
	  }

	  public override bool hasVariable(string executionId, string variableName)
	  {
		return commandExecutor.execute(new HasExecutionVariableCmd(executionId, variableName, false));
	  }

	  public virtual object getVariableLocal(string executionId, string variableName)
	  {
		return commandExecutor.execute(new GetExecutionVariableCmd(executionId, variableName, true));
	  }

	  public virtual VariableInstance getVariableInstanceLocal(string executionId, string variableName)
	  {
		return commandExecutor.execute(new GetExecutionVariableInstanceCmd(executionId, variableName, true));
	  }

	  public virtual VariableInstance getVariableInstanceLocal(string executionId, string variableName, string locale, bool withLocalizationFallback)
	  {
		return commandExecutor.execute(new GetExecutionVariableInstanceCmd(executionId, variableName, true, locale, withLocalizationFallback));
	  }

	  public override T getVariableLocal<T>(string executionId, string variableName, Type variableClass)
	  {
		  return variableClass.cast(getVariableLocal(executionId, variableName));
	  }

	  public override bool hasVariableLocal(string executionId, string variableName)
	  {
		return commandExecutor.execute(new HasExecutionVariableCmd(executionId, variableName, true));
	  }

	  public virtual void setVariable(string executionId, string variableName, object value)
	  {
		if (variableName == null)
		{
		  throw new ActivitiIllegalArgumentException("variableName is null");
		}
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[variableName] = value;
		commandExecutor.execute(new SetExecutionVariablesCmd(executionId, variables, false));
	  }

	  public virtual void setVariableLocal(string executionId, string variableName, object value)
	  {
		if (variableName == null)
		{
		  throw new ActivitiIllegalArgumentException("variableName is null");
		}
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[variableName] = value;
		commandExecutor.execute(new SetExecutionVariablesCmd(executionId, variables, true));
	  }

	  public virtual void setVariables<T1>(string executionId, IDictionary<T1> variables) where T1 : Object
	  {
		commandExecutor.execute(new SetExecutionVariablesCmd(executionId, variables, false));
	  }

	  public virtual void setVariablesLocal<T1>(string executionId, IDictionary<T1> variables) where T1 : Object
	  {
		commandExecutor.execute(new SetExecutionVariablesCmd(executionId, variables, true));
	  }

	  public virtual void removeVariable(string executionId, string variableName)
	  {
		ICollection<string> variableNames = new List<string>();
		variableNames.Add(variableName);
		commandExecutor.execute(new RemoveExecutionVariablesCmd(executionId, variableNames, false));
	  }

	  public virtual void removeVariableLocal(string executionId, string variableName)
	  {
		ICollection<string> variableNames = new List<string>();
		variableNames.Add(variableName);
		commandExecutor.execute(new RemoveExecutionVariablesCmd(executionId, variableNames, true));
	  }

	  public virtual void removeVariables(string executionId, ICollection<string> variableNames)
	  {
		commandExecutor.execute(new RemoveExecutionVariablesCmd(executionId, variableNames, false));
	  }

	  public virtual void removeVariablesLocal(string executionId, ICollection<string> variableNames)
	  {
		commandExecutor.execute(new RemoveExecutionVariablesCmd(executionId, variableNames, true));
	  }

	  public virtual void signal(string executionId)
	  {
		commandExecutor.execute(new SignalCmd(executionId, null, null, null));
	  }

	  public virtual void signal(string executionId, IDictionary<string, object> processVariables)
	  {
		commandExecutor.execute(new SignalCmd(executionId, null, null, processVariables));
	  }

	  public virtual void addUserIdentityLink(string processInstanceId, string userId, string identityLinkType)
	  {
		commandExecutor.execute(new AddIdentityLinkForProcessInstanceCmd(processInstanceId, userId, null, identityLinkType));
	  }

	  public virtual void addGroupIdentityLink(string processInstanceId, string groupId, string identityLinkType)
	  {
		commandExecutor.execute(new AddIdentityLinkForProcessInstanceCmd(processInstanceId, null, groupId, identityLinkType));
	  }

	  public virtual void addParticipantUser(string processInstanceId, string userId)
	  {
		commandExecutor.execute(new AddIdentityLinkForProcessInstanceCmd(processInstanceId, userId, null, IdentityLinkType.PARTICIPANT));
	  }

	  public virtual void addParticipantGroup(string processInstanceId, string groupId)
	  {
		commandExecutor.execute(new AddIdentityLinkForProcessInstanceCmd(processInstanceId, null, groupId, IdentityLinkType.PARTICIPANT));
	  }

	  public virtual void deleteParticipantUser(string processInstanceId, string userId)
	  {
		commandExecutor.execute(new DeleteIdentityLinkForProcessInstanceCmd(processInstanceId, userId, null, IdentityLinkType.PARTICIPANT));
	  }

	  public virtual void deleteParticipantGroup(string processInstanceId, string groupId)
	  {
		commandExecutor.execute(new DeleteIdentityLinkForProcessInstanceCmd(processInstanceId, null, groupId, IdentityLinkType.PARTICIPANT));
	  }

	  public virtual void deleteUserIdentityLink(string processInstanceId, string userId, string identityLinkType)
	  {
		commandExecutor.execute(new DeleteIdentityLinkForProcessInstanceCmd(processInstanceId, userId, null, identityLinkType));
	  }

	  public virtual void deleteGroupIdentityLink(string processInstanceId, string groupId, string identityLinkType)
	  {
		commandExecutor.execute(new DeleteIdentityLinkForProcessInstanceCmd(processInstanceId, null, groupId, identityLinkType));
	  }

	  public virtual IList<IdentityLink> getIdentityLinksForProcessInstance(string processInstanceId)
	  {
		return commandExecutor.execute(new GetIdentityLinksForProcessInstanceCmd(processInstanceId));
	  }

	  public virtual ProcessInstanceQuery createProcessInstanceQuery()
	  {
		return new ProcessInstanceQueryImpl(commandExecutor);
	  }

	  public virtual IList<string> getActiveActivityIds(string executionId)
	  {
		return commandExecutor.execute(new FindActiveActivityIdsCmd(executionId));
	  }

	  public virtual FormData getFormInstanceById(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetStartFormCmd(processDefinitionId));
	  }

	  public virtual void suspendProcessInstanceById(string processInstanceId)
	  {
		commandExecutor.execute(new SuspendProcessInstanceCmd(processInstanceId));
	  }

	  public virtual void activateProcessInstanceById(string processInstanceId)
	  {
		commandExecutor.execute(new ActivateProcessInstanceCmd(processInstanceId));
	  }

	  public virtual ProcessInstance startProcessInstanceByMessage(string messageName)
	  {
		return commandExecutor.execute(new StartProcessInstanceByMessageCmd(messageName, null, null, null));
	  }

	  public virtual ProcessInstance startProcessInstanceByMessageAndTenantId(string messageName, string tenantId)
	  {
		  return commandExecutor.execute(new StartProcessInstanceByMessageCmd(messageName, null, null, tenantId));
	  }

	  public virtual ProcessInstance startProcessInstanceByMessage(string messageName, string businessKey)
	  {
		return commandExecutor.execute(new StartProcessInstanceByMessageCmd(messageName, businessKey, null, null));
	  }

	  public virtual ProcessInstance startProcessInstanceByMessageAndTenantId(string messageName, string businessKey, string tenantId)
	  {
		return commandExecutor.execute(new StartProcessInstanceByMessageCmd(messageName, businessKey, null, tenantId));
	  }

	  public virtual ProcessInstance startProcessInstanceByMessage(string messageName, IDictionary<string, object> processVariables)
	  {
		return commandExecutor.execute(new StartProcessInstanceByMessageCmd(messageName, null, processVariables, null));
	  }

	  public virtual ProcessInstance startProcessInstanceByMessageAndTenantId(string messageName, IDictionary<string, object> processVariables, string tenantId)
	  {
		  return commandExecutor.execute(new StartProcessInstanceByMessageCmd(messageName, null, processVariables, tenantId));
	  }

	  public virtual ProcessInstance startProcessInstanceByMessage(string messageName, string businessKey, IDictionary<string, object> processVariables)
	  {
		return commandExecutor.execute(new StartProcessInstanceByMessageCmd(messageName, businessKey, processVariables, null));
	  }

	  public override ProcessInstance startProcessInstanceByMessageAndTenantId(string messageName, string businessKey, IDictionary<string, object> processVariables, string tenantId)
	  {
		  return commandExecutor.execute(new StartProcessInstanceByMessageCmd(messageName, businessKey, processVariables, tenantId));
	  }

	  public virtual void signalEventReceived(string signalName)
	  {
		commandExecutor.execute(new SignalEventReceivedCmd(signalName, null, null, null));
	  }

	  public virtual void signalEventReceivedWithTenantId(string signalName, string tenantId)
	  {
		  commandExecutor.execute(new SignalEventReceivedCmd(signalName, null, null, tenantId));
	  }

	  public virtual void signalEventReceivedAsync(string signalName)
	  {
		commandExecutor.execute(new SignalEventReceivedCmd(signalName, null, true, null));
	  }

	  public virtual void signalEventReceivedAsyncWithTenantId(string signalName, string tenantId)
	  {
		  commandExecutor.execute(new SignalEventReceivedCmd(signalName, null, true, tenantId));
	  }

	  public virtual void signalEventReceived(string signalName, IDictionary<string, object> processVariables)
	  {
		commandExecutor.execute(new SignalEventReceivedCmd(signalName, null, processVariables, null));
	  }

	  public virtual void signalEventReceivedWithTenantId(string signalName, IDictionary<string, object> processVariables, string tenantId)
	  {
		  commandExecutor.execute(new SignalEventReceivedCmd(signalName, null, processVariables, tenantId));
	  }

	  public virtual void signalEventReceived(string signalName, string executionId)
	  {
		commandExecutor.execute(new SignalEventReceivedCmd(signalName, executionId, null, null));
	  }

	  public virtual void signalEventReceived(string signalName, string executionId, IDictionary<string, object> processVariables)
	  {
		commandExecutor.execute(new SignalEventReceivedCmd(signalName, executionId, processVariables, null));
	  }

	  public virtual void signalEventReceivedAsync(string signalName, string executionId)
	  {
		commandExecutor.execute(new SignalEventReceivedCmd(signalName, executionId, true, null));
	  }

	  public virtual void messageEventReceived(string messageName, string executionId)
	  {
		commandExecutor.execute(new MessageEventReceivedCmd(messageName, executionId, null));
	  }

	  public virtual void messageEventReceived(string messageName, string executionId, IDictionary<string, object> processVariables)
	  {
		commandExecutor.execute(new MessageEventReceivedCmd(messageName, executionId, processVariables));
	  }

	  public virtual void messageEventReceivedAsync(string messageName, string executionId)
	  {
		  commandExecutor.execute(new MessageEventReceivedCmd(messageName, executionId, true));
	  }

		public override void addEventListener(ActivitiEventListener listenerToAdd)
		{
			commandExecutor.execute(new AddEventListenerCommand(listenerToAdd));
		}

		public override void addEventListener(ActivitiEventListener listenerToAdd, params ActivitiEventType[] types)
		{
			commandExecutor.execute(new AddEventListenerCommand(listenerToAdd, types));
		}

		public override void removeEventListener(ActivitiEventListener listenerToRemove)
		{
			commandExecutor.execute(new RemoveEventListenerCommand(listenerToRemove));
		}

		public override void dispatchEvent(ActivitiEvent @event)
		{
			commandExecutor.execute(new DispatchEventCommand(@event));
		}

	  public override void setProcessInstanceName(string processInstanceId, string name)
	  {
		 commandExecutor.execute(new SetProcessInstanceNameCmd(processInstanceId, name));
	  }

	  public override IList<Event> getProcessInstanceEvents(string processInstanceId)
	  {
		return commandExecutor.execute(new GetProcessInstanceEventsCmd(processInstanceId));
	  }

	  public override ProcessInstanceBuilder createProcessInstanceBuilder()
	  {
		return new ProcessInstanceBuilderImpl(this);
	  }

	  public virtual ProcessInstance startProcessInstance(ProcessInstanceBuilderImpl processInstanceBuilder)
	  {
		return commandExecutor.execute(new StartProcessInstanceCmd<ProcessInstance>(processInstanceBuilder));
	  }
	}

}