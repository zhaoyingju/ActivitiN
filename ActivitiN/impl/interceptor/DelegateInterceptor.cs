namespace org.activiti.engine.impl.interceptor
{

    using DelegateInvocation = org.activiti.engine.impl.@delegate.DelegateInvocation;

    /// <summary>
    /// 拦截器负责处理对“用户代码”的调用。 用户代码表示由其调用的外部Java代码（例如服务和侦听器）活动
    /// </summary>
    public interface DelegateInterceptor
    {
        void handleInvocation(DelegateInvocation invocation);
    }
}