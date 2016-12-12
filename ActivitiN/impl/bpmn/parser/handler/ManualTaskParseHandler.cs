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
	using ManualTask = org.activiti.bpmn.model.ManualTask;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ManualTaskParseHandler : AbstractActivityBpmnParseHandler<ManualTask>
	{

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(ManualTask);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, ManualTask manualTask)
	  {
		ActivityImpl activity = createActivityOnCurrentScope(bpmnParse, manualTask, BpmnXMLConstants.ELEMENT_TASK_MANUAL);
		activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createManualTaskActivityBehavior(manualTask);

		activity.Async = manualTask.Asynchronous;
		activity.Exclusive = !manualTask.NotExclusive;
	  }

	}

}