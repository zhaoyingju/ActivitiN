using System;


namespace org.activiti.engine.impl.interceptor
{

    using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
    using Context = org.activiti.engine.impl.context.Context;

    /// <summary>
    /// @author Tom Baeyens
    /// </summary>
    public class CommandContextInterceptor : AbstractCommandInterceptor
    {
        protected internal CommandContextFactory commandContextFactory;
        protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

        public CommandContextInterceptor()
        {
        }

        public CommandContextInterceptor(CommandContextFactory commandContextFactory, ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            this.commandContextFactory = commandContextFactory;
            this.processEngineConfiguration = processEngineConfiguration;
        }

        public override T execute<T>(CommandConfig config, Command<T> command)
        {
            CommandContext context = Context.CommandContext;

            bool contextReused = false;
            // We need to check the exception, because the transaction can be in a rollback state,
            // and some other command is being fired to compensate (eg. decrementing job retries)
            if (!config.ContextReusePossible || context == null || context.Exception != null)
            {
                context = commandContextFactory.createCommandContext(command);
            }
            else
            {
                //log.debug("Valid context found. Reusing it for the current command '{}'", command.GetType().FullName);
                contextReused = true;
            }

            try
            {
                // Push on stack
                Context.CommandContext = context;
                Context.ProcessEngineConfiguration = processEngineConfiguration;

                return next.execute(config, command);

            }
            catch (Exception e)
            {

                context.exception(e);

            }
            finally
            {
                try
                {
                    if (!contextReused)
                    {
                        context.close();
                    }
                }
                finally
                {
                    // Pop from stack
                    Context.removeCommandContext();
                    Context.removeProcessEngineConfiguration();
                    Context.removeBpmnOverrideContext();
                }
            }

            return null;
        }

        public virtual CommandContextFactory getCommandContextFactory()
        {
            return commandContextFactory;
        }

        public virtual void setCommandContextFactory(CommandContextFactory commandContextFactory)
        {
            this.commandContextFactory = commandContextFactory;
        }

        public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
        {
            get
            {
                return processEngineConfiguration;
            }
        }

        public virtual ProcessEngineConfigurationImpl ProcessEngineContext
        {
            set
            {
                this.processEngineConfiguration = value;
            }
        }
    }

}