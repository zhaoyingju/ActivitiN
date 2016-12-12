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

namespace org.activiti.engine.impl.bpmn.behavior
{

	using BpmnError = org.activiti.engine.@delegate.BpmnError;
	using Expression = org.activiti.engine.@delegate.Expression;
	using ErrorPropagation = org.activiti.engine.impl.bpmn.helper.ErrorPropagation;
	using SkipExpressionUtil = org.activiti.engine.impl.bpmn.helper.SkipExpressionUtil;
	using Context = org.activiti.engine.impl.context.Context;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;

	/// <summary>
	/// ActivityBehavior that evaluates an expression when executed. Optionally, it
	/// sets the result of the expression as a variable on the execution.
	/// 
	/// @author Tom Baeyens
	/// @author Christian Stettler
	/// @author Frederik Heremans
	/// @author Slawomir Wojtasiak (Patch for ACT-1159)
	/// @author Falko Menge
	/// </summary>
	public class ServiceTaskExpressionActivityBehavior : TaskActivityBehavior
	{

	  protected internal string serviceTaskId;
	  protected internal Expression expression;
	  protected internal Expression skipExpression;
	  protected internal string resultVariable;

	  public ServiceTaskExpressionActivityBehavior(string serviceTaskId, Expression expression, Expression skipExpression, string resultVariable)
	  {
		this.serviceTaskId = serviceTaskId;
		this.expression = expression;
		this.skipExpression = skipExpression;
		this.resultVariable = resultVariable;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		object value = null;
		try
		{
		  bool isSkipExpressionEnabled = SkipExpressionUtil.isSkipExpressionEnabled(execution, skipExpression);
		  if (!isSkipExpressionEnabled || (isSkipExpressionEnabled && !SkipExpressionUtil.shouldSkipFlowElement(execution, skipExpression)))
		  {

			if (Context.ProcessEngineConfiguration.EnableProcessDefinitionInfoCache)
			{
			  ObjectNode taskElementProperties = Context.getBpmnOverrideElementProperties(serviceTaskId, execution.ProcessDefinitionId);
			  if (taskElementProperties != null && taskElementProperties.has(org.activiti.engine.DynamicBpmnConstants_Fields.SERVICE_TASK_EXPRESSION))
			  {
				string overrideExpression = taskElementProperties.get(org.activiti.engine.DynamicBpmnConstants_Fields.SERVICE_TASK_EXPRESSION).asText();
				if (StringUtils.isNotEmpty(overrideExpression) && overrideExpression.Equals(expression.ExpressionText) == false)
				{
				  expression = Context.ProcessEngineConfiguration.ExpressionManager.createExpression(overrideExpression);
				}
			  }
			}

			value = expression.getValue(execution);
			if (resultVariable != null)
			{
			  execution.setVariable(resultVariable, value);
			}
		  }

		  leave(execution);
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