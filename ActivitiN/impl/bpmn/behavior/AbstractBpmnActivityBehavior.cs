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
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using CompensateEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.CompensateEventSubscriptionEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using PvmScope = org.activiti.engine.impl.pvm.PvmScope;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;


	/// <summary>
	/// Denotes an 'activity' in the sense of BPMN 2.0: 
	/// a parent class for all tasks, subprocess and callActivity. 
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class AbstractBpmnActivityBehavior : FlowNodeActivityBehavior
	{

	  protected internal MultiInstanceActivityBehavior multiInstanceActivityBehavior;

	  /// <summary>
	  /// Subclasses that call leave() will first pass through this method, before
	  /// the regular <seealso cref="FlowNodeActivityBehavior#leave(ActivityExecution)"/> is
	  /// called. This way, we can check if the activity has loop characteristics,
	  /// and delegate to the behavior if this is the case.
	  /// </summary>
	  protected internal override void leave(ActivityExecution execution)
	  {
		if (hasCompensationHandler(execution))
		{
		  createCompensateEventSubscription(execution);
		}
		if (!hasLoopCharacteristics())
		{
		  base.leave(execution);
		}
		else if (hasMultiInstanceCharacteristics())
		{
		  multiInstanceActivityBehavior.leave(execution);
		}
	  }

	  protected internal virtual bool hasCompensationHandler(ActivityExecution execution)
	  {
		return execution.Activity.getProperty(BpmnParse.PROPERTYNAME_COMPENSATION_HANDLER_ID) != null;
	  }

	  protected internal virtual void createCompensateEventSubscription(ActivityExecution execution)
	  {
		string compensationHandlerId = (string) execution.Activity.getProperty(BpmnParse.PROPERTYNAME_COMPENSATION_HANDLER_ID);

		ExecutionEntity executionEntity = (ExecutionEntity) execution;
		ActivityImpl compensationHandlder = executionEntity.ProcessDefinition.findActivity(compensationHandlerId);
		PvmScope scopeActivitiy = compensationHandlder.Parent;
		ExecutionEntity scopeExecution = ScopeUtil.findScopeExecutionForScope(executionEntity, scopeActivitiy);

		CompensateEventSubscriptionEntity compensateEventSubscriptionEntity = CompensateEventSubscriptionEntity.createAndInsert(scopeExecution);
		compensateEventSubscriptionEntity.Activity = compensationHandlder;
	  }

	  protected internal virtual bool hasLoopCharacteristics()
	  {
		return hasMultiInstanceCharacteristics();
	  }

	  protected internal virtual bool hasMultiInstanceCharacteristics()
	  {
		return multiInstanceActivityBehavior != null;
	  }

	  public virtual MultiInstanceActivityBehavior getMultiInstanceActivityBehavior()
	  {
		return multiInstanceActivityBehavior;
	  }

	  public virtual void setMultiInstanceActivityBehavior(MultiInstanceActivityBehavior multiInstanceActivityBehavior)
	  {
		this.multiInstanceActivityBehavior = multiInstanceActivityBehavior;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void signal(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public override void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		if ("compensationDone".Equals(signalName))
		{
		  signalCompensationDone(execution, signalData);
		}
		else
		{
		  base.signal(execution, signalName, signalData);
		}
	  }

	  protected internal virtual void signalCompensationDone(ActivityExecution execution, object signalData)
	  {
		// default behavior is to join compensating executions and propagate the signal if all executions 
		// have compensated

		// join compensating executions    
		if (execution.Executions.Count == 0)
		{
		  if (execution.Parent != null)
		  {
			ActivityExecution parent = execution.Parent;
			((InterpretableExecution)execution).remove();
			((InterpretableExecution)parent).signal("compensationDone", signalData);
		  }
		}
		else
		{
		  ((ExecutionEntity)execution).forceUpdate();
		}

	  }

	}

}