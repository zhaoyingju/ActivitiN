using System.Collections.Generic;

namespace org.activiti.engine.impl.pvm.runtime
{
    public class AtomicOperationDeleteCascade : AtomicOperation
    {

        public virtual bool isAsync(InterpretableExecution execution)
        {
            return false;
        }

        public virtual void execute(InterpretableExecution execution)
        {
            InterpretableExecution firstLeaf = findFirstLeaf(execution);

            if (firstLeaf.getSubProcessInstance() != null)
            {
                firstLeaf.getSubProcessInstance().deleteCascade(execution.DeleteReason);
            }

            firstLeaf.performOperation(AtomicOperation_Fields.DELETE_CASCADE_FIRE_ACTIVITY_END);
        }

        protected internal virtual InterpretableExecution findFirstLeaf(InterpretableExecution execution)
        {
            IList<InterpretableExecution> executions = (IList<InterpretableExecution>)execution.Executions;
            if (executions.Count > 0)
            {
                return findFirstLeaf(executions[0]);
            }
            return execution;
        }
    }

}