namespace org.activiti.engine.impl.cfg
{

	using org.activiti.engine.impl.interceptor;
	using CommandConfig = org.activiti.engine.impl.interceptor.CommandConfig;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using CommandInterceptor = org.activiti.engine.impl.interceptor.CommandInterceptor;

	/// <summary>
	/// Command executor that passes commands to the first interceptor in the chain.
	/// If no <seealso cref="CommandConfig"/> is passed, the default configuration will be used.  
	/// 
	/// @author Marcus Klimstra (CGI)
	/// </summary>
	public class CommandExecutorImpl : CommandExecutor
	{

	  private readonly CommandConfig defaultConfig;
	  private readonly CommandInterceptor first;

	  public CommandExecutorImpl(CommandConfig defaultConfig, CommandInterceptor first)
	  {
		this.defaultConfig = defaultConfig;
		this.first = first;
	  }

	  public virtual CommandInterceptor First
	  {
		  get
		  {
			return first;
		  }
	  }

	  public override CommandConfig DefaultConfig
	  {
		  get
		  {
			return defaultConfig;
		  }
	  }

	  public override T execute<T>(Command<T> command)
	  {
		return execute(defaultConfig, command);
	  }

	  public override T execute<T>(CommandConfig config, Command<T> command)
	  {
		return first.execute(config, command);
	  }

	}

}