namespace org.activiti.engine.impl.pvm.runtime
{

    using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
    using ExecutionListenerExecution = org.activiti.engine.impl.pvm.@delegate.ExecutionListenerExecution;
    using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
    using ProcessDefinitionImpl = org.activiti.engine.impl.pvm.process.ProcessDefinitionImpl;
    using TransitionImpl = org.activiti.engine.impl.pvm.process.TransitionImpl;

    public interface InterpretableExecution : ActivityExecution, ExecutionListenerExecution, PvmProcessInstance
    {

        void take(PvmTransition transition);

        void take(PvmTransition transition, bool fireActivityCompletedEvent);

        string EventName { set; }

        PvmProcessElement EventSource { set; }

        int? ExecutionListenerIndex { get; set; }

        ProcessDefinitionImpl ProcessDefinition { get; set; }

        ActivityImpl Activity { set; }

        void performOperation(AtomicOperation etomicOperation);

        bool Scope { get; }

        void destroy();

        void remove();

        InterpretableExecution ReplacedBy { get; set; }

        InterpretableExecution SubProcessInstance { get; set; }

        InterpretableExecution SuperExecution { get; }

        void deleteCascade(string deleteReason);

        bool DeleteRoot { get; }

        TransitionImpl Transition { get; set; }

        void initialize();

        InterpretableExecution Parent { set; }


        InterpretableExecution ProcessInstance { set; }

        bool EventScope { get; set; }


        StartingExecution StartingExecution { get; }

        void disposeStartingExecution();
    }

}