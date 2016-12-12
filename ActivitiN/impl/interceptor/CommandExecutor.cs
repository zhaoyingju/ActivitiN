namespace org.activiti.engine.impl.interceptor
{

    /// <summary>
    ///�����ڲ�ʹ�õ�����ִ�г���
    /// </summary>
    public interface CommandExecutor
    {

        /// <returns> the default <seealso cref="CommandConfig"/>, used if none is provided. </returns>
        CommandConfig DefaultConfig { get; }

        /// <summary>
        /// Execute a command with the specified <seealso cref="CommandConfig"/>.
        /// </summary>
        T execute<T>(CommandConfig config, Command<T> command);

        /// <summary>
        /// Execute a command with the default <seealso cref="CommandConfig"/>.
        /// </summary>
        T execute<T>(Command<T> command);

    }

}