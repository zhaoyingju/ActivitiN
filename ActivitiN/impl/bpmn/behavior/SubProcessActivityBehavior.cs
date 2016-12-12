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

namespace org.activiti.engine.impl.bpmn.behavior
{

	using ScopeUtil = org.activiti.engine.impl.bpmn.helper.ScopeUtil;
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using Context = org.activiti.engine.impl.context.Context;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.activiti.engine.impl.pvm.PvmActivity;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using CompositeActivityBehavior = org.activiti.engine.impl.pvm.@delegate.CompositeActivityBehavior;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// Implementation of the BPMN 2.0 subprocess (formally known as 'embedded' subprocess):
	/// a subprocess defined within another process definition.
	/// 
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class SubProcessActivityBehavior : AbstractBpmnActivityBehavior, CompositeActivityBehavior
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		PvmActivity activity = execution.Activity;
		ActivityImpl initialActivity = (ActivityImpl) activity.getProperty(BpmnParse.PROPERTYNAME_INITIAL);

		if (initialActivity == null)
		{
		  throw new ActivitiException("No initial activity found for subprocess " + execution.Activity.Id);
		}

		// initialize the template-defined data objects as variables
		initializeDataObjects(execution, activity);

		if (initialActivity.ActivityBehavior != null && initialActivity.ActivityBehavior is NoneStartEventActivityBehavior) // embedded subprocess: only none start allowed
		{
			((ExecutionEntity) execution).Activity = initialActivity;
			Context.CommandContext.HistoryManager.recordActivityStart((ExecutionEntity) execution);
		}

		execution.executeActivity(initialActivity);
	  }

	  public virtual void lastExecutionEnded(ActivityExecution execution)
	  {
		ScopeUtil.createEventScopeExecution((ExecutionEntity) execution);

		// remove the template-defined data object variables
		IDictionary<string, object> dataObjectVars = ((ActivityImpl) execution.Activity).Variables;
		if (dataObjectVars != null)
		{
		  execution.removeVariablesLocal(dataObjectVars.Keys);
		}

		bpmnActivityBehavior.performDefaultOutgoingBehavior(execution);
	  }

	  protected internal virtual void initializeDataObjects(ActivityExecution execution, PvmActivity activity)
	  {
		IDictionary<string, object> dataObjectVars = ((ActivityImpl) activity).Variables;
		if (dataObjectVars != null)
		{
		  execution.VariablesLocal = dataObjectVars;
		}
	  }
	}

}