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
	using ExclusiveGateway = org.activiti.bpmn.model.ExclusiveGateway;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ExclusiveGatewayParseHandler : AbstractActivityBpmnParseHandler<ExclusiveGateway>
	{

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(ExclusiveGateway);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, ExclusiveGateway gateway)
	  {
		ActivityImpl activity = createActivityOnCurrentScope(bpmnParse, gateway, BpmnXMLConstants.ELEMENT_GATEWAY_EXCLUSIVE);

		activity.Async = gateway.Asynchronous;
		activity.Exclusive = !gateway.NotExclusive;

		activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createExclusiveGatewayActivityBehavior(gateway);
	  }

	}

}