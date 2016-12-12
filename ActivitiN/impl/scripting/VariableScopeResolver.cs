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
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;
	using ExecutionImpl = org.activiti.engine.impl.pvm.runtime.ExecutionImpl;


	/// <summary>
	/// Bindings implementation using an <seealso cref="ExecutionImpl"/> as 'back-end'.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class VariableScopeResolver : Resolver
	{

	  protected internal VariableScope variableScope;
	  protected internal string variableScopeKey = "execution";

	  public VariableScopeResolver(VariableScope variableScope)
	  {
		if (variableScope == null)
		{
		  throw new ActivitiIllegalArgumentException("variableScope cannot be null");
		}
		if (variableScope is ExecutionEntity)
		{
		  variableScopeKey = "execution";
		}
		else if (variableScope is TaskEntity)
		{
		  variableScopeKey = "task";
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ActivitiException("unsupported variable scope type: " + variableScope.GetType().FullName);
		}
		this.variableScope = variableScope;
	  }

	  public virtual bool containsKey(object key)
	  {
		return variableScopeKey.Equals(key) || variableScope.hasVariable((string) key);
	  }

	  public virtual object get(object key)
	  {
		if (variableScopeKey.Equals(key))
		{
		  return variableScope;
		}

		return variableScope.getVariable((string) key);
	  }
	}

}