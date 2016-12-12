using System;

namespace org.activiti.engine.impl.pvm.runtime
{

    using SubProcessActivityBehavior = org.activiti.engine.impl.pvm.@delegate.SubProcessActivityBehavior;
    using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
    using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;

    public class AtomicOperationProcessEnd : AbstractEventAtomicOperation
    {

        protected internal override ScopeImpl getScope(InterpretableExecution execution)
        {
            return execution.ProcessDefinition;
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
            InterpretableExecution superExecution = execution.SuperExecution;
            SubProcessActivityBehavior subProcessActivityBehavior = null;

            // copy variables before destroying the ended sub process instance
            if (superExecution != null)
            {
                ActivityImpl activity = (ActivityImpl)superExecution.Activity;
                subProcessActivityBehavior = (SubProcessActivityBehavior)activity.ActivityBehavior;
                try
                {
                    subProcessActivityBehavior.completing(superExecution, execution);
                }
                catch (Exception e)
                {
                    log.error("Error while completing sub process of execution {}", execution, e);
                    throw e;
                }
                catch (Exception e)
                {
                    log.error("Error while completing sub process of execution {}", execution, e);
                    throw new ActivitiException("Error while completing sub process of execution " + execution, e);
                }
            }

            execution.destroy();
            execution.remove();

            // and trigger execution afterwards
            if (superExecution != null)
            {
                superExecution.setSubProcessInstance(null);
                try
                {
                    subProcessActivityBehavior.completed(superExecution);
                }
                catch (Exception e)
                {
                    log.error("Error while completing sub process of execution {}", execution, e);
                    throw e;
                }
                catch (Exception e)
                {
                    log.error("Error while completing sub process of execution {}", execution, e);
                    throw new ActivitiException("Error while completing sub process of execution " + execution, e);
                }
            }
        }
    }

}