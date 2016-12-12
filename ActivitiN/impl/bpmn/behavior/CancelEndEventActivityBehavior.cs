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
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// </summary>
	public class CancelEndEventActivityBehavior : FlowNodeActivityBehavior
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public override void execute(ActivityExecution execution)
	  {

		// find cancel boundary event:
		ActivityImpl cancelBoundaryEvent = ScopeUtil.findInParentScopesByBehaviorType((ActivityImpl) execution.Activity, typeof(CancelBoundaryEventActivityBehavior));

		if (cancelBoundaryEvent == null)
		{
		  throw new ActivitiException("Could not find cancel boundary event for cancel end event " + execution.Activity);
		}

		ActivityExecution scopeExecution = ScopeUtil.findScopeExecutionForScope((ExecutionEntity)execution, cancelBoundaryEvent.ParentActivity);

		// end all executions and process instances in the scope of the transaction
		scopeExecution.destroyScope("cancel end event fired");

		// the scope execution executes the boundary event
		InterpretableExecution outgoingExecution = (InterpretableExecution)scopeExecution;
		outgoingExecution.Activity = cancelBoundaryEvent;
		outgoingExecution.Active = true;

		// execute the boundary
		cancelBoundaryEvent.ActivityBehavior.execute(outgoingExecution);
	  }

	}

}