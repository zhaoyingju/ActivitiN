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
	public class ScriptBindings : Bindings
	{

	  /// <summary>
	  /// The script engine implementations put some key/value pairs into the binding.
	  /// This list contains those keys, such that they wouldn't be stored as process variable.
	  /// 
	  /// This list contains the keywords for JUEL, Javascript and Groovy.
	  /// </summary>
	  protected internal static readonly Set<string> UNSTORED_KEYS = new HashSet<string>(Arrays.asList("out", "out:print", "lang:import", "context", "elcontext", "print", "println", "nashorn.global"));

	  protected internal IList<Resolver> scriptResolvers;
	  protected internal VariableScope variableScope;
	  protected internal Bindings defaultBindings;
	  protected internal bool storeScriptVariables = true; // By default everything is stored (backwards compatibility)

	  public ScriptBindings(IList<Resolver> scriptResolvers, VariableScope variableScope)
	  {
		this.scriptResolvers = scriptResolvers;
		this.variableScope = variableScope;
		this.defaultBindings = (new SimpleScriptContext()).getBindings(SimpleScriptContext.ENGINE_SCOPE);
	  }

	  public ScriptBindings(IList<Resolver> scriptResolvers, VariableScope variableScope, bool storeScriptVariables) : this(scriptResolvers, variableScope)
	  {
		this.storeScriptVariables = storeScriptVariables;
	  }

	  public virtual bool containsKey(object key)
	  {
		foreach (Resolver scriptResolver in scriptResolvers)
		{
		  if (scriptResolver.containsKey(key))
		  {
			return true;
		  }
		}
		return defaultBindings.containsKey(key);
	  }

	  public virtual object get(object key)
	  {
		foreach (Resolver scriptResolver in scriptResolvers)
		{
		  if (scriptResolver.containsKey(key))
		  {
			return scriptResolver.get(key);
		  }
		}
		return defaultBindings.get(key);
	  }

	  public virtual object put(string name, object value)
	  {
		if (storeScriptVariables)
		{
		  object oldValue = null;
		  if (!UNSTORED_KEYS.contains(name))
		  {
			oldValue = variableScope.getVariable(name);
			variableScope.setVariable(name, value);
			return oldValue;
		  }
		}
		return defaultBindings.put(name, value);
	  }

	  public virtual Set<KeyValuePair<string, object>> entrySet()
	  {
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
		return variableScope.Variables.entrySet();
	  }

	  public virtual Set<string> keySet()
	  {
		return variableScope.Variables.Keys;
	  }

	  public virtual int size()
	  {
		return variableScope.Variables.Count;
	  }

	  public virtual ICollection<object> values()
	  {
		return variableScope.Variables.Values;
	  }

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public void putAll(java.util.Map< ? extends String, ? extends Object> toMerge)
	  public virtual void putAll<T1>(IDictionary<T1> toMerge) where T1 : String where ? : Object
	  {
		throw new System.NotSupportedException();
	  }

	  public virtual object remove(object key)
	  {
		if (UNSTORED_KEYS.contains(key))
		{
		  return null;
		}
		return defaultBindings.remove(key);
	  }

	  public virtual void clear()
	  {
		throw new System.NotSupportedException();
	  }

	  public virtual bool containsValue(object value)
	  {
		throw new System.NotSupportedException();
	  }

	  public virtual bool Empty
	  {
		  get
		  {
			throw new System.NotSupportedException();
		  }
	  }

	  public virtual void addUnstoredKey(string unstoredKey)
	  {
		  UNSTORED_KEYS.add(unstoredKey);
	  }

	}

}