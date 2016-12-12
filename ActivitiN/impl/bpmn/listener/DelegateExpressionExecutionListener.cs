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
namespace org.activiti.engine.impl.bpmn.listener
{

	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;
	using Expression = org.activiti.engine.@delegate.Expression;
	using JavaDelegate = org.activiti.engine.@delegate.JavaDelegate;
	using ClassDelegate = org.activiti.engine.impl.bpmn.helper.ClassDelegate;
	using FieldDeclaration = org.activiti.engine.impl.bpmn.parser.FieldDeclaration;
	using Context = org.activiti.engine.impl.context.Context;
	using ExecutionListenerInvocation = org.activiti.engine.impl.@delegate.ExecutionListenerInvocation;
	using JavaDelegateInvocation = org.activiti.engine.impl.@delegate.JavaDelegateInvocation;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class DelegateExpressionExecutionListener : ExecutionListener
	{

	  protected internal Expression expression;
	  private readonly IList<FieldDeclaration> fieldDeclarations;

	  public DelegateExpressionExecutionListener(Expression expression, IList<FieldDeclaration> fieldDeclarations)
	  {
		this.expression = expression;
		this.fieldDeclarations = fieldDeclarations;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.activiti.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		// Note: we can't cache the result of the expression, because the
		// execution can change: eg. delegateExpression='${mySpringBeanFactory.randomSpringBean()}'
		object @delegate = expression.getValue(execution);
		ClassDelegate.applyFieldDeclaration(fieldDeclarations, @delegate);

		if (@delegate is ExecutionListener)
		{
		  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new ExecutionListenerInvocation((ExecutionListener) @delegate, execution));
		}
		else if (@delegate is JavaDelegate)
		{
		  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new JavaDelegateInvocation((JavaDelegate) @delegate, execution));
		}
		else
		{
		  throw new ActivitiIllegalArgumentException("Delegate expression " + expression + " did not resolve to an implementation of " + typeof(ExecutionListener) + " nor " + typeof(JavaDelegate));
		}
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