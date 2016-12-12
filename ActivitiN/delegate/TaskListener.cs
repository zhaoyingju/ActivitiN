
namespace org.activiti.engine.@delegate
{
    public interface TaskListener
    {
        void notify(DelegateTask delegateTask);
    }

    public static class TaskListener_Fields
    {
        public const string EVENTNAME_CREATE = "create";
        public const string EVENTNAME_ASSIGNMENT = "assignment";
        public const string EVENTNAME_COMPLETE = "complete";
        public const string EVENTNAME_DELETE = "delete";
        public const string EVENTNAME_ALL_EVENTS = "all";
    }

}