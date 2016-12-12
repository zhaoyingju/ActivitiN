using System.Collections.Generic;

namespace org.activiti.engine.impl.pvm.@delegate
{

    using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;


    public interface ActivityExecution : DelegateExecution
    {


        PvmActivity Activity { get; }

        /// <summary>
        /// leaves the current activity by taking the given transition.
        /// </summary>
        void take(PvmTransition transition);


        /* Execution management */

        /// <summary>
        /// creates a new execution. This execution will be the parent of the newly created execution.
        /// properties processDefinition, processInstance and activity will be initialized.
        /// </summary>
        ActivityExecution createExecution();

        /// <summary>
        /// creates a new sub process instance.
        /// The current execution will be the super execution of the created execution.
        /// </summary>
        /// <param name="processDefinition"> The <seealso cref="PvmProcessDefinition"/> of the subprocess. </param>
        PvmProcessInstance createSubProcessInstance(PvmProcessDefinition processDefinition);

        /// <summary>
        /// returns the parent of this execution, or null if there no parent.
        /// </summary>
        ActivityExecution Parent { get; }

        ActivityExecution ProcessInstance { get; }

        /// <summary>
        /// returns the list of execution of which this execution the parent of.
        /// </summary>
        IList<ActivityExecution> Executions { get; }

        /// <summary>
        /// ends this execution.
        /// </summary>
        void end();


        /* State management */

        /// <summary>
        /// makes this execution active or inactive.
        /// </summary>
        bool Active { set; get; }


        /// <summary>
        /// returns whether this execution has ended or not.
        /// </summary>
        bool Ended { get; set; }


        /// <summary>
        /// changes the concurrent indicator on this execution.
        /// </summary>
        bool Concurrent { set; get; }


        /// <summary>
        /// returns whether this execution is a process instance or not.
        /// </summary>
        bool ProcessInstanceType { get; }

        /// <summary>
        /// Inactivates this execution.
        /// This is useful for example in a join: the execution
        /// still exists, but it is not longer active.
        /// </summary>
        void inactivate();

        /// <summary>
        /// Returns whether this execution is a scope.
        /// </summary>
        bool Scope { get; set; }


        /// <summary>
        /// Retrieves all executions which are concurrent and inactive at the given activity.
        /// </summary>
        IList<ActivityExecution> findInactiveConcurrentExecutions(PvmActivity activity);

        /// <summary>
        /// Takes the given outgoing transitions, and potentially reusing
        /// the given list of executions that were previously joined.
        /// </summary>
        void takeAll(IList<PvmTransition> outgoingTransitions, IList<ActivityExecution> joinedExecutions);

        /// <summary>
        /// Executes the <seealso cref="ActivityBehavior"/> associated with the given activity.
        /// </summary>
        void executeActivity(PvmActivity activity);

        /// <summary>
        /// Called when an execution is interrupted. 
        /// 
        /// Performs destroy scope behavior: all child executions and sub-process instances and other related
        /// resources are removed. The execution itself can continue execution. 
        /// </summary>
        void destroyScope(string reason);
    }

}