namespace org.activiti.engine.impl.interceptor
{
    public class LogInterceptor : AbstractCommandInterceptor
    {

        public override T execute<T>(CommandConfig config, Command<T> command)
        {
            //if (!log.DebugEnabled)
            //{
            //  // do nothing here if we cannot log
            //  return next.execute(config, command);
            //}
            //log.debug("\n");
            //log.debug("--- starting {} --------------------------------------------------------", command.GetType().Name);
            //try
            //{

            //  return next.execute(config, command);

            //}
            //finally
            //{
            //  log.debug("--- {} finished --------------------------------------------------------", command.GetType().Name);
            //  log.debug("\n");
            //}
        }
    }
}