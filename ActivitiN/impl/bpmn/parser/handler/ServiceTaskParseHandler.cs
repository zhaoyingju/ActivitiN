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
	using ServiceTask = org.activiti.bpmn.model.ServiceTask;
	using WebServiceActivityBehavior = org.activiti.engine.impl.bpmn.behavior.WebServiceActivityBehavior;
	using AbstractDataAssociation = org.activiti.engine.impl.bpmn.data.AbstractDataAssociation;
	using IOSpecification = org.activiti.engine.impl.bpmn.data.IOSpecification;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using StringUtils = org.apache.commons.lang3.StringUtils;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ServiceTaskParseHandler : AbstractExternalInvocationBpmnParseHandler<ServiceTask>
	{

		private static Logger logger = LoggerFactory.getLogger(typeof(ServiceTaskParseHandler));

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(ServiceTask);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, ServiceTask serviceTask)
	  {

		  ActivityImpl activity = createActivityOnCurrentScope(bpmnParse, serviceTask, BpmnXMLConstants.ELEMENT_TASK_SERVICE);
		  activity.Async = serviceTask.Asynchronous;
		  activity.FailedJobRetryTimeCycleValue = serviceTask.FailedJobRetryTimeCycleValue;
		  activity.Exclusive = !serviceTask.NotExclusive;

		  // Email, Mule and Shell service tasks
		  if (StringUtils.isNotEmpty(serviceTask.Type))
		  {

			if (serviceTask.Type.equalsIgnoreCase("mail"))
			{
			  activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createMailActivityBehavior(serviceTask);

			}
			else if (serviceTask.Type.equalsIgnoreCase("mule"))
			{
			  activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createMuleActivityBehavior(serviceTask, bpmnParse.BpmnModel);

			}
			else if (serviceTask.Type.equalsIgnoreCase("camel"))
			{
			  activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createCamelActivityBehavior(serviceTask, bpmnParse.BpmnModel);

			}
			else if (serviceTask.Type.equalsIgnoreCase("shell"))
			{
			  activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createShellActivityBehavior(serviceTask);

			}
			else
			{
				logger.warn("Invalid service task type: '" + serviceTask.Type + "' " + " for service task " + serviceTask.Id);
			}

		  // activiti:class
		  }
		  else if (ImplementationType.IMPLEMENTATION_TYPE_CLASS.equalsIgnoreCase(serviceTask.ImplementationType))
		  {
			activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createClassDelegateServiceTask(serviceTask);

		  // activiti:delegateExpression
		  }
		  else if (ImplementationType.IMPLEMENTATION_TYPE_DELEGATEEXPRESSION.equalsIgnoreCase(serviceTask.ImplementationType))
		  {
			activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createServiceTaskDelegateExpressionActivityBehavior(serviceTask);

		  // activiti:expression      
		  }
		  else if (ImplementationType.IMPLEMENTATION_TYPE_EXPRESSION.equalsIgnoreCase(serviceTask.ImplementationType))
		  {
			activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createServiceTaskExpressionActivityBehavior(serviceTask);

		  // Webservice   
		  }
		  else if (ImplementationType.IMPLEMENTATION_TYPE_WEBSERVICE.equalsIgnoreCase(serviceTask.ImplementationType) && StringUtils.isNotEmpty(serviceTask.OperationRef))
		  {

			if (!bpmnParse.Operations.ContainsKey(serviceTask.OperationRef))
			{
				logger.warn(serviceTask.OperationRef + " does not exist for service task " + serviceTask.Id);
			}
			else
			{

			  WebServiceActivityBehavior webServiceActivityBehavior = bpmnParse.ActivityBehaviorFactory.createWebServiceActivityBehavior(serviceTask);
			  webServiceActivityBehavior.Operation = bpmnParse.Operations[serviceTask.OperationRef];

			  if (serviceTask.IoSpecification != null)
			  {
				IOSpecification ioSpecification = createIOSpecification(bpmnParse, serviceTask.IoSpecification);
				webServiceActivityBehavior.IoSpecification = ioSpecification;
			  }

			  foreach (DataAssociation dataAssociationElement in serviceTask.DataInputAssociations)
			  {
				AbstractDataAssociation dataAssociation = createDataInputAssociation(bpmnParse, dataAssociationElement);
				webServiceActivityBehavior.addDataInputAssociation(dataAssociation);
			  }

			  foreach (DataAssociation dataAssociationElement in serviceTask.DataOutputAssociations)
			  {
				AbstractDataAssociation dataAssociation = createDataOutputAssociation(bpmnParse, dataAssociationElement);
				webServiceActivityBehavior.addDataOutputAssociation(dataAssociation);
			  }

			  activity.ActivityBehavior = webServiceActivityBehavior;
			}
		  }
		  else
		  {
			logger.warn("One of the attributes 'class', 'delegateExpression', 'type', 'operation', or 'expression' is mandatory on serviceTask " + serviceTask.Id);
		  }

	  }
	}


}