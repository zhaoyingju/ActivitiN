using System;

namespace org.activiti.engine.impl.interceptor
{
    public abstract class AbstractCommandInterceptor : CommandInterceptor
    {
        public abstract T execute<T>(CommandConfig config, Command<T> command) where T : class;


        protected internal CommandInterceptor next;

        public virtual CommandInterceptor Next
        {
            get
            {
                return next;
            }
            set
            {
                this.next = value;
            }
        }
    }
}