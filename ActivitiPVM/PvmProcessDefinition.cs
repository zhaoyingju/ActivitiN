namespace org.activiti.engine.impl.pvm
{
    /// <summary>
    /// ���̶���
    /// </summary>
    public interface PvmProcessDefinition : ReadOnlyProcessDefinition
    {

        string DeploymentId { get; }

        PvmProcessInstance createProcessInstance();

    }
}