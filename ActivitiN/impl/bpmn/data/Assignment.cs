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
namespace org.activiti.engine.impl.bpmn.data
{

	using Expression = org.activiti.engine.@delegate.Expression;
	using VariableScope = org.activiti.engine.@delegate.VariableScope;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;

	/// <summary>
	/// Implementation of the BPMN 2.0 'assignment'
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public class Assignment
	{

	  protected internal Expression fromExpression;

	  protected internal Expression toExpression;

	  public Assignment(Expression fromExpression, Expression toExpression)
	  {
		this.fromExpression = fromExpression;
		this.toExpression = toExpression;
	  }

	  public virtual void evaluate(ActivityExecution execution)
	  {
		VariableScope variableScope = (VariableScope) execution;
		object value = this.fromExpression.getValue(variableScope);
		this.toExpression.setValue(value, variableScope);
	  }
	}

}