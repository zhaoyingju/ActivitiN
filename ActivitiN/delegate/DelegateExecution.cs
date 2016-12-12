namespace org.activiti.engine.@delegate
{
    /// <summary>
    /// Execution used in <seealso cref="JavaDelegate"/>s and <seealso cref="ExecutionListener"/>s.
    /// </summary>
    public interface DelegateExecution : VariableScope
    {
        string Id { get; }

        string ProcessInstanceId { get; }

        string EventName { get; }

        string BusinessKey { get; }

        string ProcessBusinessKey { get; }

        string ProcessDefinitionId { get; }

        string ParentId { get; }

        string SuperExecutionId { get; }

        string CurrentActivityId { get; }

        string CurrentActivityName { get; }

        string TenantId { get; }

        EngineServices EngineServices { get; }
    }
}