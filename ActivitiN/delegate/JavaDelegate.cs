namespace org.activiti.engine.@delegate
{


    /// <summary>
    /// Convience class that should be used when a Java delegation in a BPMN 2.0
    /// process is required (for example, to call custom business logic).
    /// 
    /// This class can be used for both service tasks and event listeners.
    /// 
    /// This class does not allow to influence the control flow. It follows the
    /// default BPMN 2.0 behavior of taking every outgoing sequence flow (which has a
    /// condition that evaluates to true if there is a condition defined) If you are
    /// in need of influencing the flow in your process, use the
    /// class 'org.activiti.engine.impl.pvm.delegate.ActivityBehavior' instead.
    /// 
    /// @author Joram Barrez
    /// </summary>
    public interface JavaDelegate
    {
        void execute(DelegateExecution execution);
    }
}