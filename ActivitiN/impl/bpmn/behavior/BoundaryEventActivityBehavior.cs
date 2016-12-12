using System.Collections;
using System.Collections.Generic;

namespace org.activiti.engine.impl.bpmn.behavior
{


    using Context = org.activiti.engine.impl.context.Context;
    using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
    using PvmTransition = org.activiti.engine.impl.pvm.PvmTransition;
    using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
    using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;

    public class BoundaryEventActivityBehavior : FlowNodeActivityBehavior
    {

        protected internal bool interrupting;
        protected internal string activityId;

        public BoundaryEventActivityBehavior()
        {

        }

        public BoundaryEventActivityBehavior(bool interrupting, string activityId)
        {
            this.interrupting = interrupting;
            this.activityId = activityId;
        }

        public override void execute(ActivityExecution execution)
        {
            ExecutionEntity executionEntity = (ExecutionEntity)execution;
            ActivityImpl boundaryActivity = executionEntity.ProcessDefinition.findActivity(activityId);
            ActivityImpl interruptedActivity = executionEntity.Activity;

            IList<PvmTransition> outgoingTransitions = boundaryActivity.setOutgoingTransitions;
            IList<ExecutionEntity> interruptedExecutions = null;

            if (interrupting)
            {

                // Call activity
                if (executionEntity.getSubProcessInstance() != null)
                {
                    executionEntity.getSubProcessInstance().deleteCascade(executionEntity.DeleteReason);
                }
                else
                {
                    Context.CommandContext.HistoryManager.recordActivityEnd(executionEntity);
                }

                executionEntity.Activity = boundaryActivity;

                interruptedExecutions = new List<ExecutionEntity>(executionEntity.Executions);
                foreach (ExecutionEntity interruptedExecution in interruptedExecutions)
                {
                    interruptedExecution.deleteCascade("interrupting boundary event '" + execution.Activity.Id + "' fired");
                }

                execution.takeAll(outgoingTransitions, (IList)interruptedExecutions);
            }
            else
            {
                // non interrupting event, introduced with BPMN 2.0, we need to create a new execution in this case

                // create a new execution and move it out from the timer activity
                ExecutionEntity concurrentRoot = executionEntity.getParent().Concurrent ? executionEntity.getParent() : executionEntity;
                ExecutionEntity outgoingExecution = concurrentRoot.createExecution();

                outgoingExecution.Active = true;
                outgoingExecution.Scope = false;
                outgoingExecution.Concurrent = true;

                outgoingExecution.takeAll(outgoingTransitions, Collections.EMPTY_LIST);
                outgoingExecution.remove();
                // now we have to move the execution back to the real activity
                // since the execution stays there (non interrupting) and it was
                // set to the boundary event before
                executionEntity.Activity = interruptedActivity;
            }
        }

        public virtual bool Interrupting
        {
            get
            {
                return interrupting;
            }
            set
            {
                this.interrupting = value;
            }
        }


    }

}