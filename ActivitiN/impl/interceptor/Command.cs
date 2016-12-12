namespace org.activiti.engine.impl.interceptor
{
    public interface Command<T>
    {
        T execute(CommandContext commandContext);
    }
}