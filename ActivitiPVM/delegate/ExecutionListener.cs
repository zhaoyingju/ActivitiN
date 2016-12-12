using System;

namespace org.activiti.engine.impl.pvm.@delegate
{



    /// @deprecated use <seealso cref="org.activiti.delegate.ExecutionListener"/> instead.
    /// 
    /// @author Tom Baeyens
    /// @author Joram Barrez 
    //[Obsolete("use <seealso cref="org.activiti.@delegate.ExecutionListener"/> instead.")]
    public interface ExecutionListener
    {
        void notify(ExecutionListenerExecution execution);
    }

    public static class ExecutionListener_Fields
    {
        public const string EVENTNAME_START = "start";
        public const string EVENTNAME_END = "end";
        public const string EVENTNAME_TAKE = "take";
    }

}