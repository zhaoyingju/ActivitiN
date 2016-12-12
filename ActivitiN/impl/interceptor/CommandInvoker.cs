namespace org.activiti.engine.impl.interceptor
{

    using Context = org.activiti.engine.impl.context.Context;

    public class CommandInvoker : AbstractCommandInterceptor
    {

        public override T execute<T>(CommandConfig config, Command<T> command)
        {
            return command.execute(Context.CommandContext);
        }

        public override CommandInterceptor Next
        {
            get
            {
                return null;
            }

            set
            {
                throw new System.NotSupportedException("CommandInvoker must be the last interceptor in the chain");
            }
        }

    }
}