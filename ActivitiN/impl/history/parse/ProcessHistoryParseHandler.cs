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
	using Process = org.activiti.bpmn.model.Process;
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using org.activiti.engine.impl.bpmn.parser.handler;
	using ProcessInstanceEndHandler = org.activiti.engine.impl.history.handler.ProcessInstanceEndHandler;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ProcessHistoryParseHandler : AbstractBpmnParseHandler<Process>
	{

	  protected internal static readonly ProcessInstanceEndHandler PROCESS_INSTANCE_END_HANDLER = new ProcessInstanceEndHandler();

	  protected internal virtual Type HandledType
	  {
		  get
		  {
			return typeof(Process);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, Process element)
	  {
		bpmnParse.CurrentProcessDefinition.addExecutionListener(org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_END, PROCESS_INSTANCE_END_HANDLER);
	  }

	}

}