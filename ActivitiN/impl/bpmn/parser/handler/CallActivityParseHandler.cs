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
namespace org.activiti.engine.impl.bpmn.parser.handler
{

	using BpmnXMLConstants = org.activiti.bpmn.constants.BpmnXMLConstants;
	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using CallActivity = org.activiti.bpmn.model.CallActivity;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class CallActivityParseHandler : AbstractActivityBpmnParseHandler<CallActivity>
	{

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(CallActivity);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, CallActivity callActivity)
	  {

		ActivityImpl activity = createActivityOnCurrentScope(bpmnParse, callActivity, BpmnXMLConstants.ELEMENT_CALL_ACTIVITY);
		activity.Scope = true;
		activity.Async = callActivity.Asynchronous;
		activity.Exclusive = !callActivity.NotExclusive;
		activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createCallActivityBehavior(callActivity);
	  }

	}

}