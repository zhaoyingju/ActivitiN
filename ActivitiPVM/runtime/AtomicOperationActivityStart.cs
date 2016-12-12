namespace org.activiti.engine.impl.pvm.runtime
{

    using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;

    public class AtomicOperationActivityStart : AbstractEventAtomicOperation
    {

        protected internal override void eventNotificationsCompleted(InterpretableExecution execution)
        {
            execution.performOperation(AtomicOperation_Fields.ACTIVITY_EXECUTE);
        }

        protected internal override string EventName
        {
            get
            {
                return org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_START;
            }
        }

        protected internal override ScopeImpl getScope(InterpretableExecution execution)
        {
            return (ScopeImpl)execution.Activity;
        }

    }
}