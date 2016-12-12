namespace org.activiti.engine.@delegate
{




    /// <summary>
    /// Callback interface to be notified of execution events like starting a process instance,
    /// ending an activity instance or taking a transition.
    /// </summary>
    public interface ExecutionListener
    {
        void notify(DelegateExecution execution);
    }

    public static class ExecutionListener_Fields
    {
        public const string EVENTNAME_START = "start";
        public const string EVENTNAME_END = "end";
        public const string EVENTNAME_TAKE = "take";
    }

}