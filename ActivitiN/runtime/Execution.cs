namespace org.activiti.engine.runtime
{
    /// <summary>
    /// 表示流程实例中的“执行路径”。
    /// 注意，ProcessInstance也是一个执行。
    /// </summary>
    public interface Execution
    {

        /// <summary>
        /// The unique identifier of the execution.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Indicates if the execution is suspended.
        /// </summary>
        bool Suspended { get; }

        /// <summary>
        /// Indicates if the execution is ended.
        /// </summary>
        bool Ended { get; }

        /// <summary>
        /// Returns the id of the activity where the execution currently is at.
        /// Returns null if the execution is not a 'leaf' execution (eg concurrent parent).
        /// </summary>
        string ActivityId { get; }

        /// <summary>
        /// Id of the root of the execution tree representing the process instance.
        /// It is the same as <seealso cref="#getId()"/> if this execution is the process instance. 
        /// </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        /// Gets the id of the parent of this execution. If null, the execution represents a process-instance.
        /// </summary>
        string ParentId { get; }

        /// <summary>
        /// Gets the id of the super execution of this execution.
        /// </summary>
        string SuperExecutionId { get; }

        /// <summary>
        /// The tenant identifier of this process instance 
        /// </summary>
        string TenantId { get; }

        /// <summary>
        /// Returns the name of this execution.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the description of this execution.
        /// </summary>
        string Description { get; }

    }
}