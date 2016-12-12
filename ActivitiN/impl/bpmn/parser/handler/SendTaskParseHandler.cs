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
	using DataAssociation = org.activiti.bpmn.model.DataAssociation;
	using ImplementationType = org.activiti.bpmn.model.ImplementationType;
	using SendTask = org.activiti.bpmn.model.SendTask;
	using WebServiceActivityBehavior = org.activiti.engine.impl.bpmn.behavior.WebServiceActivityBehavior;
	using AbstractDataAssociation = org.activiti.engine.impl.bpmn.data.AbstractDataAssociation;
	using IOSpecification = org.activiti.engine.impl.bpmn.data.IOSpecification;
	using Operation = org.activiti.engine.impl.bpmn.webservice.Operation;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using StringUtils = org.apache.commons.lang3.StringUtils;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class SendTaskParseHandler : AbstractExternalInvocationBpmnParseHandler<SendTask>
	{

		private static readonly Logger logger = LoggerFactory.getLogger(typeof(SendTaskParseHandler));

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(SendTask);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, SendTask sendTask)
	  {

		ActivityImpl activity = createActivityOnCurrentScope(bpmnParse, sendTask, BpmnXMLConstants.ELEMENT_TASK_SEND);

		activity.Async = sendTask.Asynchronous;
		activity.Exclusive = !sendTask.NotExclusive;

		if (StringUtils.isNotEmpty(sendTask.Type))
		{
		  if (sendTask.Type.equalsIgnoreCase("mail"))
		  {
			activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createMailActivityBehavior(sendTask);
		  }
		  else if (sendTask.Type.equalsIgnoreCase("mule"))
		  {
			activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createMuleActivityBehavior(sendTask, bpmnParse.BpmnModel);
		  }
		  else if (sendTask.Type.equalsIgnoreCase("camel"))
		  {
			activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createCamelActivityBehavior(sendTask, bpmnParse.BpmnModel);
		  }

		  // for web service
		}
		else if (ImplementationType.IMPLEMENTATION_TYPE_WEBSERVICE.equalsIgnoreCase(sendTask.ImplementationType) && StringUtils.isNotEmpty(sendTask.OperationRef))
		{

		  if (!bpmnParse.Operations.ContainsKey(sendTask.OperationRef))
		  {
			logger.warn(sendTask.OperationRef + " does not exist for sendTask " + sendTask.Id);
		  }
		  else
		  {
			WebServiceActivityBehavior webServiceActivityBehavior = bpmnParse.ActivityBehaviorFactory.createWebServiceActivityBehavior(sendTask);
			Operation operation = bpmnParse.Operations[sendTask.OperationRef];
			webServiceActivityBehavior.Operation = operation;

			if (sendTask.IoSpecification != null)
			{
			  IOSpecification ioSpecification = createIOSpecification(bpmnParse, sendTask.IoSpecification);
			  webServiceActivityBehavior.IoSpecification = ioSpecification;
			}

			foreach (DataAssociation dataAssociationElement in sendTask.DataInputAssociations)
			{
			  AbstractDataAssociation dataAssociation = createDataInputAssociation(bpmnParse, dataAssociationElement);
			  webServiceActivityBehavior.addDataInputAssociation(dataAssociation);
			}

			foreach (DataAssociation dataAssociationElement in sendTask.DataOutputAssociations)
			{
			  AbstractDataAssociation dataAssociation = createDataOutputAssociation(bpmnParse, dataAssociationElement);
			  webServiceActivityBehavior.addDataOutputAssociation(dataAssociation);
			}

			activity.ActivityBehavior = webServiceActivityBehavior;
		  }
		}
		else
		{
		  logger.warn("One of the attributes 'type' or 'operation' is mandatory on sendTask " + sendTask.Id);
		}
	  }

	}

}