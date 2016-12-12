namespace org.activiti.engine.impl.pvm
{

    /// <summary>
    /// 只读流程定义
    /// </summary>
    public interface ReadOnlyProcessDefinition : PvmScope
    {

        string Name { get; }

        string Key { get; }

        string Description { get; }

        PvmActivity Initial { get; }

        string DiagramResourceName { get; }
    }

}