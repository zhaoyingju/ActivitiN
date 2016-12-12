using System;
using System.Collections.Generic;


namespace org.activiti.engine.impl.pvm.runtime
{

    using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;
    using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;

    public abstract class AbstractEventAtomicOperation : AtomicOperation
    {
        public virtual bool isAsync(InterpretableExecution execution)
        {
            return false;
        }

        public virtual void execute(InterpretableExecution execution)
        {
            ScopeImpl scope = getScope(execution);
            IList<ExecutionListener> exectionListeners = scope.getExecutionListeners(EventName);
            int executionListenerIndex = execution.ExecutionListenerIndex.Value;

            if (exectionListeners.Count > executionListenerIndex)
            {
                execution.EventName = EventName;
                execution.EventSource = scope;
                ExecutionListener listener = exectionListeners[executionListenerIndex];
                try
                {
                    listener.notify(execution);
                }
                catch (Exception e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw new PvmException("couldn't execute event listener : " + e.Message, e);
                }
                execution.ExecutionListenerIndex = executionListenerIndex + 1;
                execution.performOperation(this);

            }
            else
            {
                execution.ExecutionListenerIndex = 0;
                execution.EventName = null;
                execution.EventSource = null;

                eventNotificationsCompleted(execution);
            }
        }

        protected internal abstract ScopeImpl getScope(InterpretableExecution execution);
        protected internal abstract string EventName { get; }
        protected internal abstract void eventNotificationsCompleted(InterpretableExecution execution);
    }
}