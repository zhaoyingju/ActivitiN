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


	/// <summary>
	/// Contains the possible properties which can be used in a <seealso cref="HistoricProcessInstanceQueryProperty"/>.
	/// 
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class HistoricProcessInstanceQueryProperty : QueryProperty
	{

	  private const long serialVersionUID = 1L;

	  private static readonly IDictionary<string, HistoricProcessInstanceQueryProperty> properties = new Dictionary<string, HistoricProcessInstanceQueryProperty>();

	  public static readonly HistoricProcessInstanceQueryProperty PROCESS_INSTANCE_ID_ = new HistoricProcessInstanceQueryProperty("RES.PROC_INST_ID_");
	  public static readonly HistoricProcessInstanceQueryProperty PROCESS_DEFINITION_ID = new HistoricProcessInstanceQueryProperty("RES.PROC_DEF_ID_");
	  public static readonly HistoricProcessInstanceQueryProperty PROCESS_DEFINITION_KEY = new HistoricProcessInstanceQueryProperty("DEF.KEY_");
	  public static readonly HistoricProcessInstanceQueryProperty BUSINESS_KEY = new HistoricProcessInstanceQueryProperty("RES.BUSINESS_KEY_");
	  public static readonly HistoricProcessInstanceQueryProperty START_TIME = new HistoricProcessInstanceQueryProperty("RES.START_TIME_");
	  public static readonly HistoricProcessInstanceQueryProperty END_TIME = new HistoricProcessInstanceQueryProperty("RES.END_TIME_");
	  public static readonly HistoricProcessInstanceQueryProperty DURATION = new HistoricProcessInstanceQueryProperty("RES.DURATION_");
	  public static readonly HistoricProcessInstanceQueryProperty TENANT_ID = new HistoricProcessInstanceQueryProperty("RES.TENANT_ID_");

	  public static readonly HistoricProcessInstanceQueryProperty INCLUDED_VARIABLE_TIME = new HistoricProcessInstanceQueryProperty("VAR.LAST_UPDATED_TIME_");

	  private string name;

	  public HistoricProcessInstanceQueryProperty(string name)
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

	  public static HistoricProcessInstanceQueryProperty findByName(string propertyName)
	  {
		return properties[propertyName];
	  }

	}

}