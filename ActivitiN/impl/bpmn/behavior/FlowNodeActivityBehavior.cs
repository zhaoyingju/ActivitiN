using System;

namespace org.activiti.engine.impl.bpmn.behavior
{

    using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
    using SignallableActivityBehavior = org.activiti.engine.impl.pvm.@delegate.SignallableActivityBehavior;


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class FlowNodeActivityBehavior : SignallableActivityBehavior
    {

        protected internal BpmnActivityBehavior bpmnActivityBehavior = new BpmnActivityBehavior();

        /// <summary>
        /// 默认行为：只是离开活动没有额外的功能。
        /// </summary>
        public virtual void execute(ActivityExecution execution)
        {
            leave(execution);
        }


        protected internal virtual void leave(ActivityExecution execution)
        {
            bpmnActivityBehavior.performDefaultOutgoingBehavior(execution);
        }

        protected internal virtual void leaveIgnoreConditions(ActivityExecution activityContext)
        {
            bpmnActivityBehavior.performIgnoreConditionsOutgoingBehavior(activityContext);
        }


        public virtual void signal(ActivityExecution execution, string signalName, object signalData)
        {
            // concrete activity behaviours that do accept signals should override this method;
            throw new ActivitiException("this activity doesn't accept signals");
        }

    }
}