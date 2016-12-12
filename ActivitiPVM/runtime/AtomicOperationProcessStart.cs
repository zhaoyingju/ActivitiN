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

namespace org.activiti.engine.impl.pvm.runtime
{


	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using Context = org.activiti.engine.impl.context.Context;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.activiti.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class AtomicOperationProcessStart : AbstractEventAtomicOperation
	{

	  protected internal override ScopeImpl getScope(InterpretableExecution execution)
	  {
		return execution.ProcessDefinition;
	  }

	  protected internal override string EventName
	  {
		  get
		  {
			return org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_START;
		  }
	  }

	  protected internal override void eventNotificationsCompleted(InterpretableExecution execution)
	  {
		  if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
			IDictionary<string, object> variablesMap = null;
			try
			{
			  variablesMap = execution.Variables;
			}
			catch (Exception)
			{
			  // In some rare cases getting the execution variables can fail (JPA entity load failure for example)
			  // We ignore the exception here, because it's only meant to include variables in the initialized event.
			}
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityWithVariablesEvent(ActivitiEventType.ENTITY_INITIALIZED, execution, variablesMap, false));
		  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createProcessStartedEvent(execution, variablesMap, false));
		  }

		ProcessDefinitionImpl processDefinition = execution.ProcessDefinition;
		StartingExecution startingExecution = execution.StartingExecution;
		IList<ActivityImpl> initialActivityStack = processDefinition.getInitialActivityStack(startingExecution.Initial);
		execution.Activity = initialActivityStack[0];
		execution.performOperation(AtomicOperation_Fields.PROCESS_START_INITIAL);
	  }
	}

}