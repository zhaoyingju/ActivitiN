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
namespace org.activiti.engine.impl.webservice
{


	using MessageDefinition = org.activiti.engine.impl.bpmn.webservice.MessageDefinition;
	using MessageInstance = org.activiti.engine.impl.bpmn.webservice.MessageInstance;
	using Operation = org.activiti.engine.impl.bpmn.webservice.Operation;
	using OperationImplementation = org.activiti.engine.impl.bpmn.webservice.OperationImplementation;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// Represents a WS implementation of a <seealso cref="Operation"/>
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public class WSOperation : OperationImplementation
	{

	  private static readonly Logger LOGGER = LoggerFactory.getLogger(typeof(WSOperation));

	  protected internal string id;

	  protected internal string name;

	  protected internal WSService service;

		public WSOperation(string id, string operationName, WSService service)
		{
		this.id = id;
		this.name = operationName;
		this.service = service;
		}

	  /// <summary>
	  /// {@inheritDoc}
	  /// </summary>
	  public virtual string Id
	  {
		  get
		  {
			return this.id;
		  }
	  }

	  /// <summary>
	  /// {@inheritDoc}
	  /// </summary>
	  public virtual string Name
	  {
		  get
		  {
			return this.name;
		  }
	  }

	  /// <summary>
	  /// {@inheritDoc}
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.activiti.engine.impl.bpmn.webservice.MessageInstance sendFor(org.activiti.engine.impl.bpmn.webservice.MessageInstance message, org.activiti.engine.impl.bpmn.webservice.Operation operation, final java.util.concurrent.ConcurrentMap<javax.xml.namespace.QName, java.net.URL> overridenEndpointAddresses) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public virtual MessageInstance sendFor(MessageInstance message, Operation operation, ConcurrentMap<QName, URL> overridenEndpointAddresses)
	  {
		object[] arguments = this.getArguments(message);
		object[] results = this.safeSend(arguments, overridenEndpointAddresses);
		return this.createResponseMessage(results, operation);
	  }

	  private object[] getArguments(MessageInstance message)
	  {
		return message.StructureInstance.toArray();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object[] safeSend(Object[] arguments, final java.util.concurrent.ConcurrentMap<javax.xml.namespace.QName, java.net.URL> overridenEndpointAddresses) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  private object[] safeSend(object[] arguments, ConcurrentMap<QName, URL> overridenEndpointAddresses)
	  {
		object[] results = this.service.Client.send(this.name, arguments, overridenEndpointAddresses);
		if (results == null)
		{
		  results = new object[] {};
		}
		return results;
	  }

	  private MessageInstance createResponseMessage(object[] results, Operation operation)
	  {
		MessageInstance message = null;
		MessageDefinition outMessage = operation.getOutMessage();
		if (outMessage != null)
		{
		  message = outMessage.createInstance();
		  message.StructureInstance.loadFrom(results);
		}
		return message;
	  }

	  public virtual WSService Service
	  {
		  get
		  {
			return this.service;
		  }
	  }
	}

}