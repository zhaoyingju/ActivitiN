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
namespace org.activiti.engine.impl.bpmn.data
{

	/// <summary>
	/// Implementation of the BPMN 2.0 'dataInputRef' and 'dataOutputRef'
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public class DataRef
	{

	  protected internal string idRef;

	  public DataRef(string idRef)
	  {
		this.idRef = idRef;
	  }

	  public virtual string IdRef
	  {
		  get
		  {
			return this.idRef;
		  }
	  }
	}

}