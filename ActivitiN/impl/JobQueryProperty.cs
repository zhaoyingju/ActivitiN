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

namespace org.activiti.engine.impl
{


	using QueryProperty = org.activiti.engine.query.QueryProperty;
	using JobQuery = org.activiti.engine.runtime.JobQuery;

	/// <summary>
	/// Contains the possible properties that can be used in a <seealso cref="JobQuery"/>.
	/// 
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class JobQueryProperty : QueryProperty
	{

	  private const long serialVersionUID = 1L;

	  private static readonly IDictionary<string, JobQueryProperty> properties = new Dictionary<string, JobQueryProperty>();

	  public static readonly JobQueryProperty JOB_ID = new JobQueryProperty("ID_");
	  public static readonly JobQueryProperty PROCESS_INSTANCE_ID = new JobQueryProperty("RES.PROCESS_INSTANCE_ID_");
	  public static readonly JobQueryProperty EXECUTION_ID = new JobQueryProperty("RES.EXECUTION_ID_");
	  public static readonly JobQueryProperty DUEDATE = new JobQueryProperty("RES.DUEDATE_");
	  public static readonly JobQueryProperty RETRIES = new JobQueryProperty("RES.RETRIES_");
	  public static readonly JobQueryProperty TENANT_ID = new JobQueryProperty("RES.TENANT_ID_");

	  private string name;

	  public JobQueryProperty(string name)
	  {
		this.name = name;
		properties[name] = this;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public static JobQueryProperty findByName(string propertyName)
	  {
		return properties[propertyName];
	  }

	}

}