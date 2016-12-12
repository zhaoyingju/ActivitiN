namespace org.activiti.engine.cfg
{

	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;

	/// <summary>
	/// @author jbarrez
	/// </summary>
	public abstract class AbstractProcessEngineConfigurator : ProcessEngineConfigurator
	{

		public static int DEFAULT_CONFIGURATOR_PRIORITY = 10000;

		public override int Priority
		{
			get
			{
				return DEFAULT_CONFIGURATOR_PRIORITY;
			}
		}

		public virtual void beforeInit(ProcessEngineConfigurationImpl processEngineConfiguration)
		{

		}

		public virtual void configure(ProcessEngineConfigurationImpl processEngineConfiguration)
		{

		}

	}

}