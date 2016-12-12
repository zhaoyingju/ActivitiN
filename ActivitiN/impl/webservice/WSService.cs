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
namespace org.activiti.engine.impl.webservice
{


	using BpmnInterface = org.activiti.engine.impl.bpmn.webservice.BpmnInterface;
	using BpmnInterfaceImplementation = org.activiti.engine.impl.bpmn.webservice.BpmnInterfaceImplementation;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ReflectUtil = org.activiti.engine.impl.util.ReflectUtil;

	/// <summary>
	/// Represents a WS implementation of a <seealso cref="BpmnInterface"/>
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public class WSService : BpmnInterfaceImplementation
	{

	  protected internal string name;

	  protected internal string location;

	  protected internal IDictionary<string, WSOperation> operations;

	  protected internal string wsdlLocation;

	  protected internal SyncWebServiceClient client;

	  public WSService(string name, string location, string wsdlLocation)
	  {
		this.name = name;
		this.location = location;
		this.operations = new Dictionary<string, WSOperation>();
		this.wsdlLocation = wsdlLocation;
	  }

	  public WSService(string name, string location, SyncWebServiceClient client)
	  {
		this.name = name;
		this.location = location;
		this.operations = new Dictionary<string, WSOperation>();
		this.client = client;
	  }

	  public virtual void addOperation(WSOperation operation)
	  {
		this.operations[operation.Name] = operation;
	  }

	  internal virtual SyncWebServiceClient Client
	  {
		  get
		  {
			if (this.client == null)
			{
			  //TODO refactor to use configuration
			  SyncWebServiceClientFactory factory = (SyncWebServiceClientFactory) ReflectUtil.instantiate(ProcessEngineConfigurationImpl.DEFAULT_WS_SYNC_FACTORY);
			  this.client = factory.create(this.wsdlLocation);
			}
			return this.client;
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

	  public virtual string Location
	  {
		  get
		  {
			return this.location;
		  }
	  }
	}

}