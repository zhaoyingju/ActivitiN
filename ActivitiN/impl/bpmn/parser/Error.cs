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
namespace org.activiti.engine.impl.bpmn.parser
{

	using BpmnError = org.activiti.engine.@delegate.BpmnError;


	/// <summary>
	/// Represents a BPMN Error definition, whereas <seealso cref="BpmnError"/> represents an
	/// actual instance of an Error.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class Error
	{

	  protected internal string id;
	  protected internal string errorCode;

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


	  public virtual string ErrorCode
	  {
		  get
		  {
			return errorCode;
		  }
		  set
		  {
			this.errorCode = value;
		  }
	  }


	}

}