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
namespace org.activiti.engine.impl.bpmn.parser
{


	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using DataObject = org.activiti.bpmn.model.DataObject;
	using FlowElement = org.activiti.bpmn.model.FlowElement;
	using BpmnParseHandler = org.activiti.engine.parse.BpmnParseHandler;
	using Logger = org.slf4j.Logger;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class BpmnParseHandlers
	{

	  private static readonly Logger LOGGER = org.slf4j.LoggerFactory.getLogger(typeof(BpmnParseHandlers));

	  protected internal IDictionary<Type, IList<BpmnParseHandler>> parseHandlers;

	  public BpmnParseHandlers()
	  {
		this.parseHandlers = new Dictionary<Type, IList<BpmnParseHandler>>();
	  }

	  public virtual IList<BpmnParseHandler> getHandlersFor(Type clazz)
	  {
		return parseHandlers[clazz];
	  }

	  public virtual void addHandlers(IList<BpmnParseHandler> bpmnParseHandlers)
	  {
		foreach (BpmnParseHandler bpmnParseHandler in bpmnParseHandlers)
		{
		  addHandler(bpmnParseHandler);
		}
	  }

	  public virtual void addHandler(BpmnParseHandler bpmnParseHandler)
	  {
		foreach (Type type in bpmnParseHandler.HandledTypes)
		{
		  IList<BpmnParseHandler> handlers = parseHandlers[type];
		  if (handlers == null)
		  {
			handlers = new List<BpmnParseHandler>();
			parseHandlers[type] = handlers;
		  }
		  handlers.Add(bpmnParseHandler);
		}
	  }

	  public virtual void parseElement(BpmnParse bpmnParse, BaseElement element)
	  {

		if (element is DataObject)
		{
		  // ignore DataObject elements because they are processed on Process and Sub process level
		  return;
		}

		if (element is FlowElement)
		{
		  bpmnParse.CurrentFlowElement = (FlowElement) element;
		}

		// Execute parse handlers
		IList<BpmnParseHandler> handlers = parseHandlers[element.GetType()];

		if (handlers == null)
		{
		  LOGGER.warn("Could not find matching parse handler for + " + element.Id + " this is likely a bug.");
		}
		else
		{
		  foreach (BpmnParseHandler handler in handlers)
		  {
			handler.parse(bpmnParse, element);
		  }
		}
	  }

	}

}