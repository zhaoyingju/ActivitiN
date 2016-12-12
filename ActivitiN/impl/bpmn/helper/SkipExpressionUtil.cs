namespace org.activiti.engine.impl.bpmn.helper
{

	using Expression = org.activiti.engine.@delegate.Expression;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;


	public class SkipExpressionUtil
	{

	  public static bool isSkipExpressionEnabled(ActivityExecution execution, Expression skipExpression)
	  {

		if (skipExpression == null)
		{
		  return false;
		}

		const string skipExpressionEnabledVariable = "_ACTIVITI_SKIP_EXPRESSION_ENABLED";
		object isSkipExpressionEnabled = execution.getVariable(skipExpressionEnabledVariable);

		if (isSkipExpressionEnabled == null)
		{
		  return false;

		}
		else if (isSkipExpressionEnabled is bool?)
		{
		  return (bool)((bool?) isSkipExpressionEnabled);

		}
		else
		{
		  throw new ActivitiIllegalArgumentException(skipExpressionEnabledVariable + " variable does not resolve to a boolean. " + isSkipExpressionEnabled);
		}
	  }

	  public static bool shouldSkipFlowElement(ActivityExecution execution, Expression skipExpression)
	  {
		object value = skipExpression.getValue(execution);

		if (value is bool?)
		{
		  return (bool)((bool?)value);

		}
		else
		{
		  throw new ActivitiIllegalArgumentException("Skip expression does not resolve to a boolean: " + skipExpression.ExpressionText);
		}
	  }
	}

}