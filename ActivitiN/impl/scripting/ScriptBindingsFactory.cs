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

namespace org.activiti.engine.impl.scripting
{


	using VariableScope = org.activiti.engine.@delegate.VariableScope;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class ScriptBindingsFactory
	{

	  protected internal IList<ResolverFactory> resolverFactories;

	  public ScriptBindingsFactory(IList<ResolverFactory> resolverFactories)
	  {
		this.resolverFactories = resolverFactories;
	  }

	  public virtual Bindings createBindings(VariableScope variableScope)
	  {
		return new ScriptBindings(createResolvers(variableScope), variableScope);
	  }

	  public virtual Bindings createBindings(VariableScope variableScope, bool storeScriptVariables)
	  {
		return new ScriptBindings(createResolvers(variableScope), variableScope, storeScriptVariables);
	  }

	  protected internal virtual IList<Resolver> createResolvers(VariableScope variableScope)
	  {
		IList<Resolver> scriptResolvers = new List<Resolver>();
		foreach (ResolverFactory scriptResolverFactory in resolverFactories)
		{
		  Resolver resolver = scriptResolverFactory.createResolver(variableScope);
		  if (resolver != null)
		  {
			scriptResolvers.Add(resolver);
		  }
		}
		return scriptResolvers;
	  }

	  public virtual IList<ResolverFactory> ResolverFactories
	  {
		  get
		  {
			return resolverFactories;
		  }
		  set
		  {
			this.resolverFactories = value;
		  }
	  }

	}

}