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
namespace org.activiti.engine.impl.bpmn.helper
{

	using Expression = org.activiti.engine.@delegate.Expression;
	using VariableScope = org.activiti.engine.@delegate.VariableScope;
	using FieldDeclaration = org.activiti.engine.impl.bpmn.parser.FieldDeclaration;
	using DelegateExpressionFieldInjectionMode = org.activiti.engine.impl.cfg.DelegateExpressionFieldInjectionMode;
	using Context = org.activiti.engine.impl.context.Context;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class DelegateExpressionUtil
	{

	  public static object resolveDelegateExpression(Expression expression, VariableScope variableScope)
	  {
		return resolveDelegateExpression(expression, variableScope, null);
	  }

	  public static object resolveDelegateExpression(Expression expression, VariableScope variableScope, IList<FieldDeclaration> fieldDeclarations)
	  {

		// Note: we can't cache the result of the expression, because the
		// execution can change: eg. delegateExpression='${mySpringBeanFactory.randomSpringBean()}'
		object @delegate = expression.getValue(variableScope);

		if (fieldDeclarations != null && fieldDeclarations.Count > 0)
		{

		  DelegateExpressionFieldInjectionMode injectionMode = Context.ProcessEngineConfiguration.DelegateExpressionFieldInjectionMode;
		  if (injectionMode.Equals(DelegateExpressionFieldInjectionMode.COMPATIBILITY))
		  {
			ClassDelegate.applyFieldDeclaration(fieldDeclarations, @delegate, true);
		  }
		  else if (injectionMode.Equals(DelegateExpressionFieldInjectionMode.MIXED))
		  {
			ClassDelegate.applyFieldDeclaration(fieldDeclarations, @delegate, false);
		  }

		}

		return @delegate;
	  }

	}

}