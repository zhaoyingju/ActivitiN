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
namespace org.activiti.engine.impl.bpmn.helper
{

	using Expression = org.activiti.engine.@delegate.Expression;
	using ActivitiEntityEvent = org.activiti.engine.@delegate.@event.ActivitiEntityEvent;
	using ActivitiEvent = org.activiti.engine.@delegate.@event.ActivitiEvent;
	using ActivitiEventListener = org.activiti.engine.@delegate.@event.ActivitiEventListener;
	using NoExecutionVariableScope = org.activiti.engine.impl.el.NoExecutionVariableScope;

	/// <summary>
	/// An <seealso cref="ActivitiEventListener"/> implementation which resolves an expression
	/// to a delegate <seealso cref="ActivitiEventListener"/> instance and uses this for event notification.
	/// <br><br>
	/// In case an entityClass was passed in the constructor, only events that are <seealso cref="ActivitiEntityEvent"/>'s
	/// that target an entity of the given type, are dispatched to the delegate.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class DelegateExpressionActivitiEventListener : BaseDelegateEventListener
	{

		protected internal Expression expression;
		protected internal bool failOnException = true;

		public DelegateExpressionActivitiEventListener(Expression expression, Type entityClass)
		{
			this.expression = expression;
			EntityClass = entityClass;
		}

		public override void onEvent(ActivitiEvent @event)
		{
			if (isValidEvent(@event))
			{

			  object @delegate = DelegateExpressionUtil.resolveDelegateExpression(expression, new NoExecutionVariableScope());
				if (@delegate is ActivitiEventListener)
				{
					// Cache result of isFailOnException() from delegate-instance until next
					// event is received. This prevents us from having to resolve the expression twice when
					// an error occurs.
					failOnException = ((ActivitiEventListener) @delegate).FailOnException;

					// Call the delegate
					((ActivitiEventListener) @delegate).onEvent(@event);
				}
				else
				{

					// Force failing, since the exception we're about to throw cannot be ignored, because it
					// did not originate from the listener itself
					failOnException = true;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					throw new ActivitiIllegalArgumentException("Delegate expression " + expression + " did not resolve to an implementation of " + typeof(ActivitiEventListener).FullName);
				}
			}
		}

		public override bool FailOnException
		{
			get
			{
				return failOnException;
			}
		}

	}

}