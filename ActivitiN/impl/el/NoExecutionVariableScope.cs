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

namespace org.activiti.engine.impl.el
{


	using VariableScope = org.activiti.engine.@delegate.VariableScope;
	using VariableInstance = org.activiti.engine.impl.persistence.entity.VariableInstance;


	/// <summary>
	/// Variable-scope only used to resolve variables when NO execution is active but
	/// expression-resolving is needed. This occurs eg. when start-form properties have default's
	/// defined. Even though variables are not available yet, expressions should be resolved 
	/// anyway.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class NoExecutionVariableScope : VariableScope
	{

	  private static readonly NoExecutionVariableScope INSTANCE = new NoExecutionVariableScope();

	  /// <summary>
	  /// Since a <seealso cref="NoExecutionVariableScope"/> has no state, it's safe to use the same
	  /// instance to prevent too many useless instances created.
	  /// </summary>
	  public static NoExecutionVariableScope SharedInstance
	  {
		  get
		  {
			return INSTANCE;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Map<String, Object> getVariables()
	  public virtual IDictionary<string, object> Variables
	  {
		  get
		  {
			return Collections.EMPTY_MAP;
		  }
		  set
		  {
			throw new System.NotSupportedException("No execution active, no variables can be set");
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Map<String, Object> getVariablesLocal()
	  public virtual IDictionary<string, object> VariablesLocal
	  {
		  get
		  {
			return Collections.EMPTY_MAP;
		  }
		  set
		  {
			throw new System.NotSupportedException("No execution active, no variables can be set");
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public java.util.Map<String, Object> getVariables(java.util.Collection<String> variableNames)
	  public override IDictionary<string, object> getVariables(ICollection<string> variableNames)
	  {
		  return Collections.EMPTY_MAP;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public java.util.Map<String, Object> getVariables(java.util.Collection<String> variableNames, boolean fetchAllVariables)
	  public override IDictionary<string, object> getVariables(ICollection<string> variableNames, bool fetchAllVariables)
	  {
		  return Collections.EMPTY_MAP;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public java.util.Map<String, Object> getVariablesLocal(java.util.Collection<String> variableNames)
	  public override IDictionary<string, object> getVariablesLocal(ICollection<string> variableNames)
	  {
		  return Collections.EMPTY_MAP;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public java.util.Map<String, Object> getVariablesLocal(java.util.Collection<String> variableNames, boolean fetchAllVariables)
	  public override IDictionary<string, object> getVariablesLocal(ICollection<string> variableNames, bool fetchAllVariables)
	  {
		  return Collections.EMPTY_MAP;
	  }

	  public virtual object getVariable(string variableName)
	  {
		return null;
	  }

	  public override object getVariable(string variableName, bool fetchAllVariables)
	  {
		return null;
	  }

	  public virtual object getVariableLocal(string variableName)
	  {
		return null;
	  }

	  public override object getVariableLocal(string variableName, bool fetchAllVariables)
	  {
		return null;
	  }

		public override T getVariable<T>(string variableName, Type variableClass)
		{
			return null;
		}

		public override T getVariableLocal<T>(string variableName, Type variableClass)
		{
			return null;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Set<String> getVariableNames()
	  public virtual Set<string> VariableNames
	  {
		  get
		  {
			return Collections.EMPTY_SET;
		  }
	  }

	  public virtual Set<string> VariableNamesLocal
	  {
		  get
		  {
			return null;
		  }
	  }

	  public override IDictionary<string, VariableInstance> VariableInstances
	  {
		  get
		  {
			return null;
		  }
	  }

	  public override IDictionary<string, VariableInstance> getVariableInstances(ICollection<string> variableNames)
	  {
		return null;
	  }

	  public override IDictionary<string, VariableInstance> getVariableInstances(ICollection<string> variableNames, bool fetchAllVariables)
	  {
		return null;
	  }

	  public override IDictionary<string, VariableInstance> VariableInstancesLocal
	  {
		  get
		  {
			return null;
		  }
	  }

	  public override IDictionary<string, VariableInstance> getVariableInstancesLocal(ICollection<string> variableNames)
	  {
		return null;
	  }

	  public override IDictionary<string, VariableInstance> getVariableInstancesLocal(ICollection<string> variableNames, bool fetchAllVariables)
	  {
		return null;
	  }

	  public override VariableInstance getVariableInstance(string variableName)
	  {
		return null;
	  }

	  public override VariableInstance getVariableInstance(string variableName, bool fetchAllVariables)
	  {
		return null;
	  }

	  public override VariableInstance getVariableInstanceLocal(string variableName)
	  {
		return null;
	  }

	  public override VariableInstance getVariableInstanceLocal(string variableName, bool fetchAllVariables)
	  {
		return null;
	  }

	  public virtual void setVariable(string variableName, object value)
	  {
		throw new System.NotSupportedException("No execution active, no variables can be set");
	  }

	  public override void setVariable(string variableName, object value, bool fetchAllVariables)
	  {
		  throw new System.NotSupportedException("No execution active, no variables can be set");
	  }

	  public virtual object setVariableLocal(string variableName, object value)
	  {
		throw new System.NotSupportedException("No execution active, no variables can be set");
	  }

	  public override object setVariableLocal(string variableName, object value, bool fetchAllVariables)
	  {
		  throw new System.NotSupportedException("No execution active, no variables can be set");
	  }



	  public virtual bool hasVariables()
	  {
		return false;
	  }

	  public virtual bool hasVariablesLocal()
	  {
		return false;
	  }

	  public virtual bool hasVariable(string variableName)
	  {
		return false;
	  }

	  public virtual bool hasVariableLocal(string variableName)
	  {
		return false;
	  }

	  public virtual void createVariableLocal(string variableName, object value)
	  {
		throw new System.NotSupportedException("No execution active, no variables can be created");
	  }

	  public virtual void createVariablesLocal<T1>(IDictionary<T1> variables) where T1 : Object
	  {
		throw new System.NotSupportedException("No execution active, no variables can be created");
	  }

	  public virtual void removeVariable(string variableName)
	  {
		throw new System.NotSupportedException("No execution active, no variables can be removed");
	  }

	  public virtual void removeVariableLocal(string variableName)
	  {
		throw new System.NotSupportedException("No execution active, no variables can be removed");
	  }

	  public virtual void removeVariables()
	  {
		throw new System.NotSupportedException("No execution active, no variables can be removed");
	  }

	  public virtual void removeVariablesLocal()
	  {
		throw new System.NotSupportedException("No execution active, no variables can be removed");
	  }

	  public virtual void removeVariables(ICollection<string> variableNames)
	  {
		throw new System.NotSupportedException("No execution active, no variables can be removed");
	  }

	  public virtual void removeVariablesLocal(ICollection<string> variableNames)
	  {
		throw new System.NotSupportedException("No execution active, no variables can be removed");
	  }

	}

}