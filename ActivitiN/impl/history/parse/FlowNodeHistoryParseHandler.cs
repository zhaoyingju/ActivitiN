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
namespace org.activiti.engine.impl.history.parse
{


	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using BoundaryEvent = org.activiti.bpmn.model.BoundaryEvent;
	using BusinessRuleTask = org.activiti.bpmn.model.BusinessRuleTask;
	using CallActivity = org.activiti.bpmn.model.CallActivity;
	using EndEvent = org.activiti.bpmn.model.EndEvent;
	using EventGateway = org.activiti.bpmn.model.EventGateway;
	using ExclusiveGateway = org.activiti.bpmn.model.ExclusiveGateway;
	using InclusiveGateway = org.activiti.bpmn.model.InclusiveGateway;
	using IntermediateCatchEvent = org.activiti.bpmn.model.IntermediateCatchEvent;
	using ManualTask = org.activiti.bpmn.model.ManualTask;
	using ParallelGateway = org.activiti.bpmn.model.ParallelGateway;
	using ReceiveTask = org.activiti.bpmn.model.ReceiveTask;
	using ScriptTask = org.activiti.bpmn.model.ScriptTask;
	using SendTask = org.activiti.bpmn.model.SendTask;
	using ServiceTask = org.activiti.bpmn.model.ServiceTask;
	using SubProcess = org.activiti.bpmn.model.SubProcess;
	using Task = org.activiti.bpmn.model.Task;
	using ThrowEvent = org.activiti.bpmn.model.ThrowEvent;
	using UserTask = org.activiti.bpmn.model.UserTask;
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using ActivityInstanceEndHandler = org.activiti.engine.impl.history.handler.ActivityInstanceEndHandler;
	using ActivityInstanceStartHandler = org.activiti.engine.impl.history.handler.ActivityInstanceStartHandler;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using BpmnParseHandler = org.activiti.engine.parse.BpmnParseHandler;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class FlowNodeHistoryParseHandler : BpmnParseHandler
	{

	  protected internal static readonly ActivityInstanceEndHandler ACTIVITI_INSTANCE_END_LISTENER = new ActivityInstanceEndHandler();

	  protected internal static readonly ActivityInstanceStartHandler ACTIVITY_INSTANCE_START_LISTENER = new ActivityInstanceStartHandler();

	  protected internal static Set<Type> supportedElementClasses = new HashSet<Type>();

	  static FlowNodeHistoryParseHandler()
	  {
		supportedElementClasses.add(typeof(EndEvent));
		supportedElementClasses.add(typeof(ThrowEvent));
		supportedElementClasses.add(typeof(BoundaryEvent));
		supportedElementClasses.add(typeof(IntermediateCatchEvent));

		supportedElementClasses.add(typeof(ExclusiveGateway));
		supportedElementClasses.add(typeof(InclusiveGateway));
		supportedElementClasses.add(typeof(ParallelGateway));
		supportedElementClasses.add(typeof(EventGateway));

		supportedElementClasses.add(typeof(Task));
		supportedElementClasses.add(typeof(ManualTask));
		supportedElementClasses.add(typeof(ReceiveTask));
		supportedElementClasses.add(typeof(ScriptTask));
		supportedElementClasses.add(typeof(ServiceTask));
		supportedElementClasses.add(typeof(BusinessRuleTask));
		supportedElementClasses.add(typeof(SendTask));
		supportedElementClasses.add(typeof(UserTask));

		supportedElementClasses.add(typeof(CallActivity));
		supportedElementClasses.add(typeof(SubProcess));
	  }

	  public virtual Set<Type> HandledTypes
	  {
		  get
		  {
			return supportedElementClasses;
		  }
	  }

	  public virtual void parse(BpmnParse bpmnParse, BaseElement element)
	  {
		ActivityImpl activity = bpmnParse.CurrentScope.findActivity(element.Id);
		if (element is BoundaryEvent)
		{
			// A boundary-event never receives an activity start-event
			activity.addExecutionListener(org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_END, ACTIVITY_INSTANCE_START_LISTENER, 0);
			activity.addExecutionListener(org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_END, ACTIVITI_INSTANCE_END_LISTENER, 1);
		}
		else
		{
			activity.addExecutionListener(org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_START, ACTIVITY_INSTANCE_START_LISTENER, 0);
			activity.addExecutionListener(org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_END, ACTIVITI_INSTANCE_END_LISTENER);
		}
	  }

	}

}