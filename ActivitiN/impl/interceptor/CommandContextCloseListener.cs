namespace org.activiti.engine.impl.interceptor
{

    /// <summary>
    /// ������֪ͨ<seealso cref =��CommandContext��/>�رյ���������
    /// </summary>
    public interface CommandContextCloseListener
	{
		void closing(CommandContext commandContext);

		void closed(CommandContext commandContext);

	}
}