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
namespace org.activiti.engine.impl.bpmn.behavior
{

	using BpmnError = org.activiti.engine.@delegate.BpmnError;
	using Expression = org.activiti.engine.@delegate.Expression;
	using JavaDelegate = org.activiti.engine.@delegate.JavaDelegate;
	using DelegateExpressionUtil = org.activiti.engine.impl.bpmn.helper.DelegateExpressionUtil;
	using ErrorPropagation = org.activiti.engine.impl.bpmn.helper.ErrorPropagation;
	using SkipExpressionUtil = org.activiti.engine.impl.bpmn.helper.SkipExpressionUtil;
	using FieldDeclaration = org.activiti.engine.impl.bpmn.parser.FieldDeclaration;
	using Context = org.activiti.engine.impl.context.Context;
	using ActivityBehaviorInvocation = org.activiti.engine.impl.@delegate.ActivityBehaviorInvocation;
	using JavaDelegateInvocation = org.activiti.engine.impl.@delegate.JavaDelegateInvocation;
	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using SignallableActivityBehavior = org.activiti.engine.impl.pvm.@delegate.SignallableActivityBehavior;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;


	/// <summary>
	/// <seealso cref="ActivityBehavior"/> used when 'delegateExpression' is used
	/// for a serviceTask.
	/// 
	/// @author Joram Barrez
	/// @author Josh Long
	/// @author Slawomir Wojtasiak (Patch for ACT-1159)
	/// @author Falko Menge
	/// </summary>
	public class ServiceTaskDelegateExpressionActivityBehavior : TaskActivityBehavior
	{

	  protected internal string serviceTaskId;
	  protected internal Expression expression;
	  protected internal Expression skipExpression;
	  private readonly IList<FieldDeclaration> fieldDeclarations;

	  public ServiceTaskDelegateExpressionActivityBehavior(string serviceTaskId, Expression expression, Expression skipExpression, IList<FieldDeclaration> fieldDeclarations)
	  {
		this.serviceTaskId = serviceTaskId;
		this.expression = expression;
		this.skipExpression = skipExpression;
		this.fieldDeclarations = fieldDeclarations;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void signal(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public override void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		object @delegate = DelegateExpressionUtil.resolveDelegateExpression(expression, execution, fieldDeclarations);
		if (@delegate is SignallableActivityBehavior)
		{
		  ((SignallableActivityBehavior) @delegate).signal(execution, signalName, signalData);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
		public virtual void execute(ActivityExecution execution)
		{

		try
		{
		  bool isSkipExpressionEnabled = SkipExpressionUtil.isSkipExpressionEnabled(execution, skipExpression);
		  if (!isSkipExpressionEnabled || (isSkipExpressionEnabled && !SkipExpressionUtil.shouldSkipFlowElement(execution, skipExpression)))
		  {

			if (Context.ProcessEngineConfiguration.EnableProcessDefinitionInfoCache)
			{
			  ObjectNode taskElementProperties = Context.getBpmnOverrideElementProperties(serviceTaskId, execution.ProcessDefinitionId);
			  if (taskElementProperties != null && taskElementProperties.has(org.activiti.engine.DynamicBpmnConstants_Fields.SERVICE_TASK_DELEGATE_EXPRESSION))
			  {
				string overrideExpression = taskElementProperties.get(org.activiti.engine.DynamicBpmnConstants_Fields.SERVICE_TASK_DELEGATE_EXPRESSION).asText();
				if (StringUtils.isNotEmpty(overrideExpression) && overrideExpression.Equals(expression.ExpressionText) == false)
				{
				  expression = Context.ProcessEngineConfiguration.ExpressionManager.createExpression(overrideExpression);
				}
			  }
			}

			object @delegate = DelegateExpressionUtil.resolveDelegateExpression(expression, execution, fieldDeclarations);
			if (@delegate is ActivityBehavior)
			{

			  if (@delegate is AbstractBpmnActivityBehavior)
			  {
				((AbstractBpmnActivityBehavior) @delegate).setMultiInstanceActivityBehavior(MultiInstanceActivityBehavior);
			  }

			  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new ActivityBehaviorInvocation((ActivityBehavior) @delegate, execution));

			}
			else if (@delegate is JavaDelegate)
			{
			  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new JavaDelegateInvocation((JavaDelegate) @delegate, execution));
			  leave(execution);

			}
			else
			{
			  throw new ActivitiIllegalArgumentException("Delegate expression " + expression + " did neither resolve to an implementation of " + typeof(ActivityBehavior) + " nor " + typeof(JavaDelegate));
			}
		  }
		  else
		  {
			leave(execution);
		  }
		}
		catch (Exception exc)
		{

		  Exception cause = exc;
		  BpmnError error = null;
		  while (cause != null)
		  {
			if (cause is BpmnError)
			{
			  error = (BpmnError) cause;
			  break;
			}
			cause = cause.InnerException;
		  }

		  if (error != null)
		  {
			ErrorPropagation.propagateError(error, execution);
		  }
		  else
		  {
			throw exc;
		  }

		}
		}

	}

}