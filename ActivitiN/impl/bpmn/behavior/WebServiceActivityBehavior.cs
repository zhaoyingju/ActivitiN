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
namespace org.activiti.engine.impl.bpmn.behavior
{


	using BpmnError = org.activiti.engine.@delegate.BpmnError;
	using AbstractDataAssociation = org.activiti.engine.impl.bpmn.data.AbstractDataAssociation;
	using IOSpecification = org.activiti.engine.impl.bpmn.data.IOSpecification;
	using ItemInstance = org.activiti.engine.impl.bpmn.data.ItemInstance;
	using ErrorPropagation = org.activiti.engine.impl.bpmn.helper.ErrorPropagation;
	using MessageInstance = org.activiti.engine.impl.bpmn.webservice.MessageInstance;
	using Operation = org.activiti.engine.impl.bpmn.webservice.Operation;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.activiti.engine.impl.context.Context;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;

	/// <summary>
	/// An activity behavior that allows calling Web services
	/// 
	/// @author Esteban Robles Luna
	/// @author Falko Menge
	/// @author Joram Barrez
	/// </summary>
	public class WebServiceActivityBehavior : AbstractBpmnActivityBehavior
	{

	  public const string CURRENT_MESSAGE = "org.activiti.engine.impl.bpmn.CURRENT_MESSAGE";

	  protected internal Operation operation;

	  protected internal IOSpecification ioSpecification;

	  protected internal IList<AbstractDataAssociation> dataInputAssociations;

	  protected internal IList<AbstractDataAssociation> dataOutputAssociations;

	  public WebServiceActivityBehavior()
	  {
		this.dataInputAssociations = new List<AbstractDataAssociation>();
		this.dataOutputAssociations = new List<AbstractDataAssociation>();
	  }

	  public virtual void addDataInputAssociation(AbstractDataAssociation dataAssociation)
	  {
		this.dataInputAssociations.Add(dataAssociation);
	  }

	  public virtual void addDataOutputAssociation(AbstractDataAssociation dataAssociation)
	  {
		this.dataOutputAssociations.Add(dataAssociation);
	  }

	  /// <summary>
	  /// {@inheritDoc}
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		MessageInstance message;

		try
		{
		  if (ioSpecification != null)
		  {
			this.ioSpecification.initialize(execution);
			ItemInstance inputItem = (ItemInstance) execution.getVariable(this.ioSpecification.FirstDataInputName);
			message = new MessageInstance(this.operation.getInMessage(), inputItem);
		  }
		  else
		  {
			message = this.operation.getInMessage().createInstance();
		  }

		  execution.setVariable(CURRENT_MESSAGE, message);

		  this.fillMessage(message, execution);

		  ProcessEngineConfigurationImpl processEngineConfig = Context.ProcessEngineConfiguration;
		  MessageInstance receivedMessage = this.operation.sendMessage(message, processEngineConfig.WsOverridenEndpointAddresses);

		  execution.setVariable(CURRENT_MESSAGE, receivedMessage);

		  if (ioSpecification != null)
		  {
			string firstDataOutputName = this.ioSpecification.FirstDataOutputName;
			if (firstDataOutputName != null)
			{
			  ItemInstance outputItem = (ItemInstance) execution.getVariable(firstDataOutputName);
			  outputItem.StructureInstance.loadFrom(receivedMessage.StructureInstance.toArray());
			}
		  }

		  this.returnMessage(receivedMessage, execution);

		  execution.setVariable(CURRENT_MESSAGE, null);
		  leave(execution);
		}
		catch (Exception exc)
		{

		  Exception cause = exc;
		  BpmnError error = null;
		  while (cause != null)
		  {
			if (cause is BpmnError)
			{
			  error = (BpmnError) cause;
			  break;
			}
			cause = cause.InnerException;
		  }

		  if (error != null)
		  {
			ErrorPropagation.propagateError(error, execution);
		  }
		  else
		  {
			throw exc;
		  }
		}
	  }

	  private void returnMessage(MessageInstance message, ActivityExecution execution)
	  {
		foreach (AbstractDataAssociation dataAssociation in this.dataOutputAssociations)
		{
		  dataAssociation.evaluate(execution);
		}
	  }

	  private void fillMessage(MessageInstance message, ActivityExecution execution)
	  {
		foreach (AbstractDataAssociation dataAssociation in this.dataInputAssociations)
		{
		  dataAssociation.evaluate(execution);
		}
	  }

	  public virtual IOSpecification IoSpecification
	  {
		  set
		  {
			this.ioSpecification = value;
		  }
	  }

	  public virtual Operation Operation
	  {
		  set
		  {
			this.operation = value;
		  }
	  }

	}

}