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
namespace org.activiti.engine.impl.bpmn.parser.handler
{

	using BpmnXMLConstants = org.activiti.bpmn.constants.BpmnXMLConstants;
	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using EventSubProcess = org.activiti.bpmn.model.EventSubProcess;
	using SubProcess = org.activiti.bpmn.model.SubProcess;
	using IOSpecification = org.activiti.engine.impl.bpmn.data.IOSpecification;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class SubProcessParseHandler : AbstractActivityBpmnParseHandler<SubProcess>
	{

	  protected internal virtual Type HandledType
	  {
		  get
		  {
			return typeof(SubProcess);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, SubProcess subProcess)
	  {

		ActivityImpl activity = createActivityOnScope(bpmnParse, subProcess, BpmnXMLConstants.ELEMENT_SUBPROCESS, bpmnParse.CurrentScope);

		activity.Async = subProcess.Asynchronous;
		activity.Exclusive = !subProcess.NotExclusive;

		bool triggeredByEvent = false;
		if (subProcess is EventSubProcess)
		{
		  triggeredByEvent = true;
		}
		activity.setProperty("triggeredByEvent", triggeredByEvent);

		// event subprocesses are not scopes
		activity.Scope = !triggeredByEvent;
		activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createSubprocActivityBehavior(subProcess);

		bpmnParse.CurrentScope = activity;
		bpmnParse.CurrentSubProcess = subProcess;

		bpmnParse.processFlowElements(subProcess.FlowElements);
		processArtifacts(bpmnParse, subProcess.Artifacts, activity);

		// no data objects for event subprocesses
		if (!(subProcess is EventSubProcess))
		{
		  // parse out any data objects from the template in order to set up the necessary process variables
		  IDictionary<string, object> variables = processDataObjects(bpmnParse, subProcess.DataObjects, activity);
		  activity.Variables = variables;
		}

		bpmnParse.removeCurrentScope();
		bpmnParse.removeCurrentSubProcess();

		if (subProcess.IoSpecification != null)
		{
		  IOSpecification ioSpecification = createIOSpecification(bpmnParse, subProcess.IoSpecification);
		  activity.IoSpecification = ioSpecification;
		}

	  }

	}

}