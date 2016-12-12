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

namespace org.activiti.engine.impl.persistence.entity
{


	using Context = org.activiti.engine.impl.context.Context;
	using CacheableVariable = org.activiti.engine.impl.variable.CacheableVariable;
	using JPAEntityListVariableType = org.activiti.engine.impl.variable.JPAEntityListVariableType;
	using JPAEntityVariableType = org.activiti.engine.impl.variable.JPAEntityVariableType;


	/// <summary>
	/// List that initialises binary variable values if command-context is active.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class HistoricVariableInitializingList : List<HistoricVariableInstanceEntity>
	{

	  private const long serialVersionUID = 1L;

	  public override void add(int index, HistoricVariableInstanceEntity e)
	  {
		base.Insert(index, e);
		initializeVariable(e);
	  }

	  public override bool add(HistoricVariableInstanceEntity e)
	  {
		initializeVariable(e);
		return base.Add(e);
	  }
	  public override bool addAll<T1>(ICollection<T1> c) where T1 : HistoricVariableInstanceEntity
	  {
		foreach (HistoricVariableInstanceEntity e in c)
		{
		  initializeVariable(e);
		}
		return base.AddRange(c);
	  }
	  public override bool addAll<T1>(int index, ICollection<T1> c) where T1 : HistoricVariableInstanceEntity
	  {
		foreach (HistoricVariableInstanceEntity e in c)
		{
		  initializeVariable(e);
		}
		return base.AddRange(index, c);
	  }

	  /// <summary>
	  /// If the passed <seealso cref="HistoricVariableInstanceEntity"/> is a binary variable and the command-context is active,
	  /// the variable value is fetched to ensure the byte-array is populated.
	  /// </summary>
	  protected internal virtual void initializeVariable(HistoricVariableInstanceEntity e)
	  {
		if (Context.CommandContext != null && e != null && e.VariableType != null)
		{
		  e.Value;

		  // make sure JPA entities are cached for later retrieval
		  if (JPAEntityVariableType.TYPE_NAME.Equals(e.VariableType.TypeName) || JPAEntityListVariableType.TYPE_NAME.Equals(e.VariableType.TypeName))
		  {
			((CacheableVariable) e.VariableType).ForceCacheable = true;
		  }
		}
	  }
	}
}