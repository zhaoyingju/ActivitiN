namespace org.activiti.engine.impl.pvm
{
    /// <summary>
    /// ��������Ԫ��
    /// </summary>
    public interface PvmProcessElement
    {
        string Id { get; }

        PvmProcessDefinition ProcessDefinition { get; }

        object getProperty(string name);

    }
}