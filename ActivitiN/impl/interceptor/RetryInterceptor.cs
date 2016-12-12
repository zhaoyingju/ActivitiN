using System.Threading;

namespace org.activiti.engine.impl.interceptor
{

    /// <summary>
    /// Intercepts <seealso cref="ActivitiOptimisticLockingException"/> and tries to run the
    /// same command again. The number of retries and the time waited between retries
    /// is configurable.
    /// 
    /// @author Daniel Meyer
    /// </summary>
    public class RetryInterceptor : AbstractCommandInterceptor
    {
        protected internal int numOfRetries = 3;
        protected internal int waitTimeInMs = 50;
        protected internal int waitIncreaseFactor = 5;

        public override T execute<T>(CommandConfig config, Command<T> command)
        {
            long waitTime = waitTimeInMs;
            int failedAttempts = 0;

            do
            {
                if (failedAttempts > 0)
                {
                    //log.info("Waiting for {}ms before retrying the command.", waitTime);
                    waitBeforeRetry(waitTime);
                    waitTime *= waitIncreaseFactor;
                }

                try
                {

                    // try to execute the command
                    return next.execute(config, command);

                }
                catch (ActivitiOptimisticLockingException e)
                {
                    //log.info("Caught optimistic locking exception: " + e);
                }

                failedAttempts++;
            } while (failedAttempts <= numOfRetries);

            throw new ActivitiException(numOfRetries + " retries failed with ActivitiOptimisticLockingException. Giving up.");
        }

        protected internal virtual void waitBeforeRetry(long waitTime)
        {
            try
            {
                Thread.Sleep(waitTime);
            }
            catch (InterruptedException)
            {
                //log.debug("I am interrupted while waiting for a retry.");
            }
        }

        public virtual int NumOfRetries
        {
            set
            {
                this.numOfRetries = value;
            }
            get
            {
                return numOfRetries;
            }
        }

        public virtual int WaitIncreaseFactor
        {
            set
            {
                this.waitIncreaseFactor = value;
            }
            get
            {
                return waitIncreaseFactor;
            }
        }

        public virtual int WaitTimeInMs
        {
            set
            {
                this.waitTimeInMs = value;
            }
            get
            {
                return waitTimeInMs;
            }
        }
    }
}