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
namespace org.activiti.engine.impl.bpmn.webservice
{


	/// <summary>
	/// An Operation is part of an <seealso cref="BpmnInterface"/> and it defines Messages that are consumed and
	/// (optionally) produced when the Operation is called.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class Operation
	{

	  protected internal string id;

	  protected internal string name;

	  protected internal MessageDefinition inMessage;

	  protected internal MessageDefinition outMessage;

	  protected internal OperationImplementation implementation;

	  /// <summary>
	  /// The interface to which this operations belongs
	  /// </summary>
	  protected internal BpmnInterface bpmnInterface;

	  public Operation()
	  {

	  }

	  public Operation(string id, string name, BpmnInterface bpmnInterface, MessageDefinition inMessage)
	  {
		Id = id;
		Name = name;
		setInterface(bpmnInterface);
		setInMessage(inMessage);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MessageInstance sendMessage(MessageInstance message, final java.util.concurrent.ConcurrentMap<javax.xml.namespace.QName, java.net.URL> overridenEndpointAddresses) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public virtual MessageInstance sendMessage(MessageInstance message, ConcurrentMap<QName, URL> overridenEndpointAddresses)
	  {
		return this.implementation.sendFor(message, this, overridenEndpointAddresses);
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual BpmnInterface getInterface()
	  {
		return bpmnInterface;
	  }

	  public virtual void setInterface(BpmnInterface bpmnInterface)
	  {
		this.bpmnInterface = bpmnInterface;
	  }

	  public virtual MessageDefinition getInMessage()
	  {
		return inMessage;
	  }

	  public virtual void setInMessage(MessageDefinition inMessage)
	  {
		this.inMessage = inMessage;
	  }

	  public virtual MessageDefinition getOutMessage()
	  {
		return outMessage;
	  }

	  public virtual void setOutMessage(MessageDefinition outMessage)
	  {
		this.outMessage = outMessage;
	  }

	  public virtual OperationImplementation getImplementation()
	  {
		return implementation;
	  }

	  public virtual void setImplementation(OperationImplementation implementation)
	  {
		this.implementation = implementation;
	  }
	}

}