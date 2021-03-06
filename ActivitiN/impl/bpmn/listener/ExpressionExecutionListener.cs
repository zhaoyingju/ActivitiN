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

namespace org.activiti.engine.impl.bpmn.listener
{

	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;
	using Expression = org.activiti.engine.@delegate.Expression;

	/// <summary>
	/// An <seealso cref="ExecutionListener"/> that evaluates a <seealso cref="Expression"/> when notified.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	[Serializable]
	public class ExpressionExecutionListener : ExecutionListener
	{

	  protected internal Expression expression;

	  public ExpressionExecutionListener(Expression expression)
	  {
		this.expression = expression;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.activiti.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		// Return value of expression is ignored
		expression.getValue(execution);
	  }

	  /// <summary>
	  /// returns the expression text for this execution listener. Comes in handy if you want to
	  /// check which listeners you already have.
	  /// </summary>
	  public virtual string ExpressionText
	  {
		  get
		  {
			return expression.ExpressionText;
		  }
	  }
	}

}