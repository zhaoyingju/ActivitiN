namespace org.activiti.engine.impl.interceptor
{
    public interface CommandInterceptor
    {
        T execute<T>(CommandConfig config, Command<T> command) where T : class;

        CommandInterceptor Next { get; set; }
    }
}