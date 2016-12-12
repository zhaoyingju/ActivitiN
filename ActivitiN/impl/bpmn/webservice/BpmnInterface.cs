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
namespace org.activiti.engine.impl.bpmn.webservice
{


	/// <summary>
	/// An Interface defines a set of operations that are implemented by services
	/// external to the process.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class BpmnInterface
	{

	  protected internal string id;

	  protected internal string name;

	  protected internal BpmnInterfaceImplementation implementation;

	  /// <summary>
	  /// Mapping of the operations of this interface.
	  /// The key of the map is the id of the operation, for easy retrieval.
	  /// </summary>
	  protected internal IDictionary<string, Operation> operations = new Dictionary<string, Operation>();

	  public BpmnInterface()
	  {

	  }

	  public BpmnInterface(string id, string name)
	  {
		Id = id;
		Name = name;
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


	  public virtual void addOperation(Operation operation)
	  {
		operations[operation.Id] = operation;
	  }

	  public virtual Operation getOperation(string operationId)
	  {
		return operations[operationId];
	  }

	  public virtual ICollection<Operation> Operations
	  {
		  get
		  {
			return operations.Values;
		  }
	  }

	  public virtual BpmnInterfaceImplementation getImplementation()
	  {
		return implementation;
	  }

	  public virtual void setImplementation(BpmnInterfaceImplementation implementation)
	  {
		this.implementation = implementation;
	  }
	}

}