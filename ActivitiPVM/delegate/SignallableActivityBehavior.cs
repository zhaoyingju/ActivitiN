namespace org.activiti.engine.impl.pvm.@delegate
{

    public interface SignallableActivityBehavior : ActivityBehavior
    {
        void signal(ActivityExecution execution, string signalEvent, object signalData);
    }
}