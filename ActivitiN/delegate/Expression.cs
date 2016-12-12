namespace org.activiti.engine.@delegate
{
    public interface Expression
    {
        object getValue(VariableScope variableScope);

        void setValue(object value, VariableScope variableScope);

        string ExpressionText { get; }
    }
}