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

	using DataAssociation = org.activiti.bpmn.model.DataAssociation;
	using FlowNode = org.activiti.bpmn.model.FlowNode;
	using Expression = org.activiti.engine.@delegate.Expression;
	using AbstractDataAssociation = org.activiti.engine.impl.bpmn.data.AbstractDataAssociation;
	using Assignment = org.activiti.engine.impl.bpmn.data.Assignment;
	using SimpleDataInputAssociation = org.activiti.engine.impl.bpmn.data.SimpleDataInputAssociation;
	using TransformationDataOutputAssociation = org.activiti.engine.impl.bpmn.data.TransformationDataOutputAssociation;
	using MessageImplicitDataInputAssociation = org.activiti.engine.impl.bpmn.webservice.MessageImplicitDataInputAssociation;
	using MessageImplicitDataOutputAssociation = org.activiti.engine.impl.bpmn.webservice.MessageImplicitDataOutputAssociation;
	using StringUtils = org.apache.commons.lang3.StringUtils;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public abstract class AbstractExternalInvocationBpmnParseHandler<T> : AbstractActivityBpmnParseHandler<T> where T : org.activiti.bpmn.model.FlowNode
	{

	  public virtual AbstractDataAssociation createDataInputAssociation(BpmnParse bpmnParse, DataAssociation dataAssociationElement)
	  {
		if (dataAssociationElement.Assignments.Empty)
		{
		  return new MessageImplicitDataInputAssociation(dataAssociationElement.SourceRef, dataAssociationElement.TargetRef);
		}
		else
		{
		  SimpleDataInputAssociation dataAssociation = new SimpleDataInputAssociation(dataAssociationElement.SourceRef, dataAssociationElement.TargetRef);

		  foreach (org.activiti.bpmn.model.Assignment assigmentElement in dataAssociationElement.Assignments)
		  {
			if (StringUtils.isNotEmpty(assigmentElement.From) && StringUtils.isNotEmpty(assigmentElement.To))
			{
			  Expression from = bpmnParse.ExpressionManager.createExpression(assigmentElement.From);
			  Expression to = bpmnParse.ExpressionManager.createExpression(assigmentElement.To);
			  Assignment assignment = new Assignment(from, to);
			  dataAssociation.addAssignment(assignment);
			}
		  }
		  return dataAssociation;
		}
	  }

	  public virtual AbstractDataAssociation createDataOutputAssociation(BpmnParse bpmnParse, DataAssociation dataAssociationElement)
	  {
		if (StringUtils.isNotEmpty(dataAssociationElement.SourceRef))
		{
		  return new MessageImplicitDataOutputAssociation(dataAssociationElement.TargetRef, dataAssociationElement.SourceRef);
		}
		else
		{
		  Expression transformation = bpmnParse.ExpressionManager.createExpression(dataAssociationElement.Transformation);
		  AbstractDataAssociation dataOutputAssociation = new TransformationDataOutputAssociation(null, dataAssociationElement.TargetRef, transformation);
		  return dataOutputAssociation;
		}
	  }


	}

}