namespace org.activiti.engine.impl.pvm
{
    /// <summary>
    /// 流程定义
    /// </summary>
    public interface PvmProcessDefinition : ReadOnlyProcessDefinition
    {

        string DeploymentId { get; }

        PvmProcessInstance createProcessInstance();

    }
}