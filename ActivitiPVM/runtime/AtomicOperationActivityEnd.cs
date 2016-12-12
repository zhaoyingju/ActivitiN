using System.Collections;
using System.Collections.Generic;

namespace org.activiti.engine.impl.pvm.runtime
{

    using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
    using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
    using Context = org.activiti.engine.impl.context.Context;
    using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
    using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
    using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
    using CompositeActivityBehavior = org.activiti.engine.impl.pvm.@delegate.CompositeActivityBehavior;
    using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
    using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;


    public class AtomicOperationActivityEnd : AbstractEventAtomicOperation
    {

        protected internal override ScopeImpl getScope(InterpretableExecution execution)
        {
            return (ScopeImpl)execution.Activity;
        }

        protected internal override string EventName
        {
            get
            {
                return PvmEvent.EVENTNAME_END;
            }
        }

        
        protected internal override void eventNotificationsCompleted(InterpretableExecution execution)
        {

            ActivityImpl activity = (ActivityImpl)execution.Activity;
            ActivityImpl parentActivity = activity.ParentActivity;

            if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
            {
                if (execution is ExecutionEntity)
                {
                    ExecutionEntity executionEntity = (ExecutionEntity)execution;
                    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
                    Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createActivityEvent(ActivitiEventType.ACTIVITY_COMPLETED, execution.Activity.Id, (string)executionEntity.Activity.Properties["name"], execution.Id, execution.ProcessInstanceId, execution.ProcessDefinitionId, (string)executionEntity.Activity.Properties["type"], executionEntity.Activity.ActivityBehavior.GetType().FullName));
                }
            }

            // if the execution is a single path of execution inside the process definition scope
            if ((parentActivity != null) && (!parentActivity.Scope))
            {
                execution.Activity = parentActivity;
                execution.performOperation(AtomicOperation_Fields.ACTIVITY_END);

            }
            else if (execution.ProcessInstanceType)
            {
                // dispatch process completed event
                if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
                {
                    Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.PROCESS_COMPLETED, execution));
                }

                execution.performOperation(AtomicOperation_Fields.PROCESS_END);

            }
            else if (execution.Scope)
            {

                ActivityBehavior parentActivityBehavior = (parentActivity != null ? parentActivity.ActivityBehavior : null);
                if (parentActivityBehavior is CompositeActivityBehavior)
                {
                    CompositeActivityBehavior compositeActivityBehavior = (CompositeActivityBehavior)parentActivity.ActivityBehavior;

                    if (activity.Scope && activity.setOutgoingTransitions.Count == 0)
                    {
                        // there is no transition destroying the scope
                        InterpretableExecution parentScopeExecution = (InterpretableExecution)execution.Parent;
                        execution.destroy();
                        execution.remove();
                        parentScopeExecution.Activity = parentActivity;
                        compositeActivityBehavior.lastExecutionEnded(parentScopeExecution);
                    }
                    else
                    {
                        execution.Activity = parentActivity;
                        compositeActivityBehavior.lastExecutionEnded(execution);
                    }
                }
                else
                {
                    // default destroy scope behavior
                    InterpretableExecution parentScopeExecution = (InterpretableExecution)execution.Parent;
                    execution.destroy();
                    execution.remove();
                    // if we are a scope under the process instance 
                    // and have no outgoing transitions: end the process instance here
                    if (activity.setParent == activity.ProcessDefinition)
                    {
                        parentScopeExecution.Activity = activity;
                        if (activity.setOutgoingTransitions.Count == 0)
                        {
                            // we call end() because it sets isEnded on the execution
                            parentScopeExecution.end();
                        }
                        else
                        {
                            // dispatch process completed event
                            if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
                            {
                                Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.PROCESS_COMPLETED, execution));
                            }

                            parentScopeExecution.performOperation(AtomicOperation_Fields.PROCESS_END);
                        }
                    }
                    else
                    {
                        parentScopeExecution.Activity = parentActivity;
                        parentScopeExecution.performOperation(AtomicOperation_Fields.ACTIVITY_END);
                    }
                }

            } // execution.isConcurrent() && !execution.isScope()
            else
            {

                execution.remove();

                // prune if necessary
                InterpretableExecution concurrentRoot = (InterpretableExecution)execution.Parent;
                if (concurrentRoot.Executions.Count == 1)
                {
                    InterpretableExecution lastConcurrent = (InterpretableExecution)concurrentRoot.Executions[0];
                    if (!lastConcurrent.Scope)
                    {
                        concurrentRoot.Activity = (ActivityImpl)lastConcurrent.Activity;
                        lastConcurrent.setReplacedBy(concurrentRoot);

                        // Move children of lastConcurrent one level up
                        if (lastConcurrent.Executions.Count > 0)
                        {
                            concurrentRoot.Executions.Clear();
                            foreach (ActivityExecution childExecution in lastConcurrent.Executions)
                            {
                                InterpretableExecution childInterpretableExecution = (InterpretableExecution)childExecution;
                                ((IList)concurrentRoot.Executions).Add(childExecution); // casting ... damn generics
                                childInterpretableExecution.Parent = concurrentRoot;
                            }
                            lastConcurrent.Executions.Clear();
                        }

                        // Copy execution-local variables of lastConcurrent
                        concurrentRoot.VariablesLocal = lastConcurrent.VariablesLocal;

                        // Make sure parent execution is re-activated when the last concurrent child execution is active
                        if (!concurrentRoot.Active && lastConcurrent.Active)
                        {
                            concurrentRoot.Active = true;
                        }

                        lastConcurrent.remove();
                    }
                    else
                    {
                        lastConcurrent.Concurrent = false;
                    }
                }
            }
        }

        
        protected internal virtual bool isExecutionAloneInParent(InterpretableExecution execution)
        {
            ScopeImpl parentScope = (ScopeImpl)execution.Activity.setParent;
            foreach (InterpretableExecution other in (IList<InterpretableExecution>)execution.Parent.Executions)
            {
                if (other != execution && parentScope.contains((ActivityImpl)other.Activity))
                {
                    return false;
                }
            }
            return true;
        }
    }
}