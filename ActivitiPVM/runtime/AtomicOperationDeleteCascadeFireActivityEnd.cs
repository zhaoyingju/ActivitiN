namespace org.activiti.engine.impl.pvm.runtime
{

    using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
    using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;

    public class AtomicOperationDeleteCascadeFireActivityEnd : AbstractEventAtomicOperation
    {

        protected internal override ScopeImpl getScope(InterpretableExecution execution)
        {
            ActivityImpl activity = (ActivityImpl)execution.Activity;

            if (activity != null)
            {
                return activity;
            }
            else
            {
                InterpretableExecution parent = (InterpretableExecution)execution.Parent;
                if (parent != null)
                {
                    return getScope((InterpretableExecution)execution.Parent);
                }
                return execution.ProcessDefinition;
            }
        }

        protected internal override string EventName
        {
            get
            {
                return org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_END;
            }
        }

        protected internal override void eventNotificationsCompleted(InterpretableExecution execution)
        {
            ActivityImpl activity = (ActivityImpl)execution.Activity;
            if ((execution.Scope) && (activity != null))
            {
                execution.Activity = activity.ParentActivity;
                execution.performOperation(AtomicOperation_Fields.DELETE_CASCADE_FIRE_ACTIVITY_END);

            }
            else
            {
                if (execution.Scope)
                {
                    execution.destroy();
                }

                execution.remove();

                if (!execution.DeleteRoot)
                {
                    InterpretableExecution parent = (InterpretableExecution)execution.Parent;
                    if (parent != null)
                    {
                        parent.performOperation(AtomicOperation_Fields.DELETE_CASCADE);
                    }
                }
            }
        }
    }
}