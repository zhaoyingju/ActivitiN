namespace org.activiti.engine.impl.pvm
{

    /// <summary>
    /// ֻ�����̶���
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