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
namespace org.activiti.engine.impl
{



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class ProcessEngineInfoImpl : ProcessEngineInfo
	{

	  private const long serialVersionUID = 1L;

	  internal string name;
	  internal string resourceUrl;
	  internal string exception;

	  public ProcessEngineInfoImpl(string name, string resourceUrl, string exception)
	  {
		this.name = name;
		this.resourceUrl = resourceUrl;
		this.exception = exception;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }
	  public virtual string ResourceUrl
	  {
		  get
		  {
			return resourceUrl;
		  }
	  }
	  public virtual string Exception
	  {
		  get
		  {
			return exception;
		  }
	  }
	}

}