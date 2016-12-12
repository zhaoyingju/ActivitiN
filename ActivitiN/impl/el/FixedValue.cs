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

namespace org.activiti.engine.impl.el
{

	using VariableScope = org.activiti.engine.@delegate.VariableScope;

	/// <summary>
	/// Expression that always returns the same value when <code>getValue</code> is
	/// called. Setting of the value is not supported.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	[Serializable]
	public class FixedValue : Expression
	{

	  private const long serialVersionUID = 1L;
	  private object value;

	  public FixedValue(object value)
	  {
		this.value = value;
	  }

	  public virtual object getValue(VariableScope variableScope)
	  {
		return value;
	  }

	  public virtual void setValue(object value, VariableScope variableScope)
	  {
		throw new ActivitiException("Cannot change fixed value");
	  }

	  public virtual string ExpressionText
	  {
		  get
		  {
			return value.ToString();
		  }
	  }

	}

}