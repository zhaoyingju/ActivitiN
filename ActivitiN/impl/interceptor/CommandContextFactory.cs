namespace org.activiti.engine.impl.interceptor
{

    using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;

    public class CommandContextFactory
    {

        protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

        public virtual CommandContext<T1> createCommandContext<T1>(Command<T1> cmd)
        {
            return new CommandContext<T1>(cmd, processEngineConfiguration);
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
        {
            get
            {
                return processEngineConfiguration;
            }
            set
            {
                this.processEngineConfiguration = value;
            }
        }

    }
}