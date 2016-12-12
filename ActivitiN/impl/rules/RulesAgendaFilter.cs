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

namespace org.activiti.engine.impl.rules
{


	using Activation = org.drools.runtime.rule.Activation;
	using AgendaFilter = org.drools.runtime.rule.AgendaFilter;

	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	public class RulesAgendaFilter : AgendaFilter
	{

	  protected internal IList<string> suffixList = new List<string>();
	  protected internal bool accept_Renamed;

	  public RulesAgendaFilter()
	  {
	  }

	  public virtual bool accept(Activation activation)
	  {
		string ruleName = activation.Rule.Name;
		foreach (string suffix in suffixList)
		{
		  if (ruleName.EndsWith(suffix))
		  {
			return this.accept_Renamed;
		  }
		}
		return !this.accept_Renamed;
	  }

	  public virtual void addSuffic(string suffix)
	  {
		this.suffixList.Add(suffix);
	  }

	  public virtual bool Accept
	  {
		  set
		  {
			this.accept_Renamed = value;
		  }
	  }
	}

}