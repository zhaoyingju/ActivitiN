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
	using Transaction = org.activiti.bpmn.model.Transaction;
	using IOSpecification = org.activiti.engine.impl.bpmn.data.IOSpecification;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class TransactionParseHandler : AbstractActivityBpmnParseHandler<Transaction>
	{

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(Transaction);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, Transaction transaction)
	  {

		ActivityImpl activity = createActivityOnCurrentScope(bpmnParse, transaction, BpmnXMLConstants.ELEMENT_TRANSACTION);

		activity.Async = transaction.Asynchronous;
		activity.Exclusive = !transaction.NotExclusive;

		activity.Scope = true;
		activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createTransactionActivityBehavior(transaction);


		bpmnParse.CurrentScope = activity;

		bpmnParse.processFlowElements(transaction.FlowElements);
		processArtifacts(bpmnParse, transaction.Artifacts, activity);

		bpmnParse.removeCurrentScope();

		if (transaction.IoSpecification != null)
		{
		  IOSpecification ioSpecification = createIOSpecification(bpmnParse, transaction.IoSpecification);
		  activity.IoSpecification = ioSpecification;
		}

	  }

	}

}