namespace org.activiti.engine.impl.interceptor
{

    /// <summary>
    /// 可用于通知<seealso cref =“CommandContext”/>关闭的侦听器。
    /// </summary>
    public interface CommandContextCloseListener
	{
		void closing(CommandContext commandContext);

		void closed(CommandContext commandContext);

	}
}