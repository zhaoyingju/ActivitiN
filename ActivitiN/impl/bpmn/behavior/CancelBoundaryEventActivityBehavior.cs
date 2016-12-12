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

namespace org.activiti.engine.impl.bpmn.behavior
{

	using ScopeUtil = org.activiti.engine.impl.bpmn.helper.ScopeUtil;
	using CompensateEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.CompensateEventSubscriptionEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class CancelBoundaryEventActivityBehavior : FlowNodeActivityBehavior
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public override void execute(ActivityExecution execution)
	  {

		  IList<CompensateEventSubscriptionEntity> eventSubscriptions = ((ExecutionEntity)execution).CompensateEventSubscriptions;

		  if (eventSubscriptions.Count == 0)
		  {
			leave(execution);
		  }
		  else
		  {
			// cancel boundary is always sync
			ScopeUtil.throwCompensationEvent(eventSubscriptions, execution, false);
		  }


	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void signal(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public override void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		// join compensating executions    
		if (execution.Executions.Count == 0)
		{
		  leave(execution);
		}
		else
		{
		  ((ExecutionEntity)execution).forceUpdate();
		}
	  }


	}

}