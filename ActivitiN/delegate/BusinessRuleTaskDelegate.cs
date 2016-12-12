namespace org.activiti.engine.@delegate
{

	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;

	public interface BusinessRuleTaskDelegate : ActivityBehavior
	{
	  void addRuleVariableInputIdExpression(Expression inputId);

	  void addRuleIdExpression(Expression inputId);

	  bool Exclude {set;}

	  string ResultVariable {set;}
	}
}