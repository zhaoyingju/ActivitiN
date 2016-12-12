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
namespace org.activiti.engine.impl.bpmn.behavior
{

	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using SignallableActivityBehavior = org.activiti.engine.impl.pvm.@delegate.SignallableActivityBehavior;


	/// <summary>
	/// Superclass for all 'connectable' BPMN 2.0 process elements: tasks, gateways and events.
	/// This means that any subclass can be the source or target of a sequenceflow.
	/// 
	/// Corresponds with the notion of the 'flownode' in BPMN 2.0.
	/// 
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public abstract class FlowNodeActivityBehavior : SignallableActivityBehavior
	{

	  protected internal BpmnActivityBehavior bpmnActivityBehavior = new BpmnActivityBehavior();

	  /// <summary>
	  /// Default behaviour: just leave the activity with no extra functionality.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		leave(execution);
	  }

	  /// <summary>
	  /// Default way of leaving a BPMN 2.0 activity: evaluate the conditions on the
	  /// outgoing sequence flow and take those that evaluate to true.
	  /// </summary>
	  protected internal virtual void leave(ActivityExecution execution)
	  {
		bpmnActivityBehavior.performDefaultOutgoingBehavior(execution);
	  }

	  protected internal virtual void leaveIgnoreConditions(ActivityExecution activityContext)
	  {
		bpmnActivityBehavior.performIgnoreConditionsOutgoingBehavior(activityContext);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void signal(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		// concrete activity behaviours that do accept signals should override this method;
		throw new ActivitiException("this activity doesn't accept signals");
	  }

	}

}