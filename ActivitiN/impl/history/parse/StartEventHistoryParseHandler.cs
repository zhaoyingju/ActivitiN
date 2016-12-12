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
namespace org.activiti.engine.impl.history.parse
{

	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using StartEvent = org.activiti.bpmn.model.StartEvent;
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using org.activiti.engine.impl.bpmn.parser.handler;
	using StartEventEndHandler = org.activiti.engine.impl.history.handler.StartEventEndHandler;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class StartEventHistoryParseHandler : AbstractBpmnParseHandler<StartEvent>
	{

	  protected internal static readonly StartEventEndHandler START_EVENT_END_HANDLER = new StartEventEndHandler();

	  protected internal virtual Type HandledType
	  {
		  get
		  {
			return typeof(StartEvent);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, StartEvent element)
	  {
		bpmnParse.CurrentActivity.addExecutionListener(org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_END, START_EVENT_END_HANDLER);
	  }

	}

}