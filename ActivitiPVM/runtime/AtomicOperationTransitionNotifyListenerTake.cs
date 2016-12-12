using System;
using System.Collections.Generic;

namespace org.activiti.engine.impl.pvm.runtime
{

    using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;
    using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
    using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
    using Context = org.activiti.engine.impl.context.Context;
    using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
    using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
    using TransitionImpl = org.activiti.engine.impl.pvm.process.TransitionImpl;


    /// <summary>
    /// @author Tom Baeyens
    /// </summary>
    public class AtomicOperationTransitionNotifyListenerTake : AtomicOperation
    {
        public virtual bool isAsync(InterpretableExecution execution)
        {
            return false;
        }

        public virtual void execute(InterpretableExecution execution)
        {
            TransitionImpl transition = execution.Transition;

            IList<ExecutionListener> executionListeners = transition.ExecutionListeners;
            int executionListenerIndex = execution.ExecutionListenerIndex.Value;

            if (executionListeners.Count > executionListenerIndex)
            {
                execution.EventName = org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_TAKE;
                execution.EventSource = transition;
                ExecutionListener listener = executionListeners[executionListenerIndex];
                try
                {
                    listener.notify(execution);
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
                //if (log.DebugEnabled)
                //{
                //    log.debug("{} takes transition {}", execution, transition);
                //}
                execution.ExecutionListenerIndex = 0;
                execution.EventName = null;
                execution.EventSource = null;

                ActivityImpl activity = (ActivityImpl)execution.Activity;
                ActivityImpl nextScope = findNextScope(activity.setParent, transition.Destination);
                execution.Activity = nextScope;

                // Firing event that transition is being taken     	
                if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
                {
                    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
                    Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createSequenceFlowTakenEvent(ActivitiEventType.SEQUENCEFLOW_TAKEN, transition.Id, activity.Id, (string)activity.Properties["name"], (string)activity.Properties["type"], activity.ActivityBehavior.GetType().FullName, nextScope.Id, (string)nextScope.Properties["name"], (string)nextScope.Properties["type"], nextScope.ActivityBehavior.GetType().FullName));
                }

                execution.performOperation(AtomicOperation_Fields.TRANSITION_CREATE_SCOPE);
            }
        }

        /// <summary>
        /// finds the next scope to enter.  the most outer scope is found first </summary>
        public static ActivityImpl findNextScope(ScopeImpl outerScopeElement, ActivityImpl destination)
        {
            ActivityImpl nextScope = destination;
            while ((nextScope.setParent is ActivityImpl) && (nextScope.setParent != outerScopeElement))
            {
                nextScope = (ActivityImpl)nextScope.setParent;
            }
            return nextScope;
        }
    }

}