namespace org.activiti.engine.impl.pvm.@delegate
{

    using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;

    public interface ExecutionListenerExecution : DelegateExecution
    {

        string EventName { get; }

        PvmProcessElement EventSource { get; }

        string DeleteReason { get; }
    }

}