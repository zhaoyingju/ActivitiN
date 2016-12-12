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
namespace org.activiti.engine.impl.persistence.deploy
{

	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;

	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	public class ProcessDefinitionInfoCacheObject
	{

	  protected internal string id;
	  protected internal int revision;
	  protected internal ObjectNode infoNode;

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


	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }


	  public virtual ObjectNode InfoNode
	  {
		  get
		  {
			return infoNode;
		  }
		  set
		  {
			this.infoNode = value;
		  }
	  }

	}

}