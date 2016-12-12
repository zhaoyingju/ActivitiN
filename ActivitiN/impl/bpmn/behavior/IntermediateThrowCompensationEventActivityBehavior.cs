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
	using CompensateEventDefinition = org.activiti.engine.impl.bpmn.parser.CompensateEventDefinition;
	using CompensateEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.CompensateEventSubscriptionEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class IntermediateThrowCompensationEventActivityBehavior : FlowNodeActivityBehavior
	{

	  protected internal readonly CompensateEventDefinition compensateEventDefinition;

	  public IntermediateThrowCompensationEventActivityBehavior(CompensateEventDefinition compensateEventDefinition)
	  {
		this.compensateEventDefinition = compensateEventDefinition;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public override void execute(ActivityExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String activityRef = compensateEventDefinition.getActivityRef();
		string activityRef = compensateEventDefinition.ActivityRef;

		ExecutionEntity scopeExecution = ScopeUtil.findScopeExecutionForScope((ExecutionEntity)execution, (ActivityImpl)execution.Activity);

		IList<CompensateEventSubscriptionEntity> eventSubscriptions;

		if (activityRef != null)
		{
		  eventSubscriptions = scopeExecution.getCompensateEventSubscriptions(activityRef);
		}
		else
		{
		  eventSubscriptions = scopeExecution.CompensateEventSubscriptions;
		}

		if (eventSubscriptions.Count == 0)
		{
		  leave(execution);
		}
		else
		{
		  // TODO: implement async (waitForCompletion=false in bpmn)
		  ScopeUtil.throwCompensationEvent(eventSubscriptions, execution, false);
		}

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void signal(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
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