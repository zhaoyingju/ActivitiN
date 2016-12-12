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

	using DelegateTask = org.activiti.engine.@delegate.DelegateTask;
	using Expression = org.activiti.engine.@delegate.Expression;
	using TaskListener = org.activiti.engine.@delegate.TaskListener;
	using ClassDelegate = org.activiti.engine.impl.bpmn.helper.ClassDelegate;
	using FieldDeclaration = org.activiti.engine.impl.bpmn.parser.FieldDeclaration;
	using Context = org.activiti.engine.impl.context.Context;
	using TaskListenerInvocation = org.activiti.engine.impl.@delegate.TaskListenerInvocation;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class DelegateExpressionTaskListener : TaskListener
	{

	  protected internal Expression expression;
	  private readonly IList<FieldDeclaration> fieldDeclarations;

	  public DelegateExpressionTaskListener(Expression expression, IList<FieldDeclaration> fieldDeclarations)
	  {
		this.expression = expression;
		this.fieldDeclarations = fieldDeclarations;
	  }

	  public virtual void notify(DelegateTask delegateTask)
	  {
		// Note: we can't cache the result of the expression, because the
		// execution can change: eg. delegateExpression='${mySpringBeanFactory.randomSpringBean()}'
		object @delegate = expression.getValue(delegateTask.Execution);
		ClassDelegate.applyFieldDeclaration(fieldDeclarations, @delegate);

		if (@delegate is TaskListener)
		{
		  try
		  {
			Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new TaskListenerInvocation((TaskListener)@delegate, delegateTask));
		  }
		  catch (Exception e)
		  {
			throw new ActivitiException("Exception while invoking TaskListener: " + e.Message, e);
		  }
		}
		else
		{
		  throw new ActivitiIllegalArgumentException("Delegate expression " + expression + " did not resolve to an implementation of " + typeof(TaskListener));
		}
	  }

	  /// <summary>
	  /// returns the expression text for this task listener. Comes in handy if you want to
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