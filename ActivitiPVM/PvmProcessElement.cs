namespace org.activiti.engine.impl.pvm
{
    /// <summary>
    /// 流程流程元素
    /// </summary>
    public interface PvmProcessElement
    {
        string Id { get; }

        PvmProcessDefinition ProcessDefinition { get; }

        object getProperty(string name);

    }
}