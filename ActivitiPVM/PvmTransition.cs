namespace org.activiti.engine.impl.pvm
{

    using Expression = org.activiti.engine.@delegate.Expression;


    public interface PvmTransition : PvmProcessElement
    {

        PvmActivity Source { get; }

        PvmActivity Destination { get; }

        Expression SkipExpression { get; }
    }
}