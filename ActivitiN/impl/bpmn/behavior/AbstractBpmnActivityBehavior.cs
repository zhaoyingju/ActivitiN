namespace org.activiti.engine.impl.bpmn.behavior
{

    using ScopeUtil = org.activiti.engine.impl.bpmn.helper.ScopeUtil;
    using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
    using CompensateEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.CompensateEventSubscriptionEntity;
    using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
    using PvmScope = org.activiti.engine.impl.pvm.PvmScope;
    using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
    using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
    using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;

    /// <summary>
    /// 
    /// </summary>
    public class AbstractBpmnActivityBehavior : FlowNodeActivityBehavior
    {

        protected internal MultiInstanceActivityBehavior multiInstanceActivityBehavior;


        protected internal override void leave(ActivityExecution execution)
        {
            if (hasCompensationHandler(execution))
            {
                createCompensateEventSubscription(execution);
            }
            if (!hasLoopCharacteristics())
            {
                base.leave(execution);
            }
            else if (hasMultiInstanceCharacteristics())
            {
                multiInstanceActivityBehavior.leave(execution);
            }
        }

        protected internal virtual bool hasCompensationHandler(ActivityExecution execution)
        {
            return execution.Activity.getProperty(BpmnParse.PROPERTYNAME_COMPENSATION_HANDLER_ID) != null;
        }

        protected internal virtual void createCompensateEventSubscription(ActivityExecution execution)
        {
            string compensationHandlerId = (string)execution.Activity.getProperty(BpmnParse.PROPERTYNAME_COMPENSATION_HANDLER_ID);

            ExecutionEntity executionEntity = (ExecutionEntity)execution;
            ActivityImpl compensationHandlder = executionEntity.ProcessDefinition.findActivity(compensationHandlerId);
            PvmScope scopeActivitiy = compensationHandlder.setParent;
            ExecutionEntity scopeExecution = ScopeUtil.findScopeExecutionForScope(executionEntity, scopeActivitiy);

            CompensateEventSubscriptionEntity compensateEventSubscriptionEntity = CompensateEventSubscriptionEntity.createAndInsert(scopeExecution);
            compensateEventSubscriptionEntity.Activity = compensationHandlder;
        }

        protected internal virtual bool hasLoopCharacteristics()
        {
            return hasMultiInstanceCharacteristics();
        }

        protected internal virtual bool hasMultiInstanceCharacteristics()
        {
            return multiInstanceActivityBehavior != null;
        }

        public virtual MultiInstanceActivityBehavior getMultiInstanceActivityBehavior()
        {
            return multiInstanceActivityBehavior;
        }

        public virtual void setMultiInstanceActivityBehavior(MultiInstanceActivityBehavior multiInstanceActivityBehavior)
        {
            this.multiInstanceActivityBehavior = multiInstanceActivityBehavior;
        }

        public override void signal(ActivityExecution execution, string signalName, object signalData)
        {
            if ("compensationDone".Equals(signalName))
            {
                signalCompensationDone(execution, signalData);
            }
            else
            {
                base.signal(execution, signalName, signalData);
            }
        }

        protected internal virtual void signalCompensationDone(ActivityExecution execution, object signalData)
        {
            // default behavior is to join compensating executions and propagate the signal if all executions 
            // have compensated

            // join compensating executions    
            if (execution.Executions.Count == 0)
            {
                if (execution.Parent != null)
                {
                    ActivityExecution parent = execution.Parent;
                    ((InterpretableExecution)execution).remove();
                    ((InterpretableExecution)parent).signal("compensationDone", signalData);
                }
            }
            else
            {
                ((ExecutionEntity)execution).forceUpdate();
            }

        }

    }

}