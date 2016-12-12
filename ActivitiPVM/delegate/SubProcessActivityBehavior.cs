namespace org.activiti.engine.impl.pvm.@delegate
{

    using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;

    public interface SubProcessActivityBehavior : ActivityBehavior
    {

        /// <summary>
        /// called before the process instance is destroyed to allow 
        /// this activity to extract data from the sub process instance.
        /// No control flow should be done on the execution yet. 
        /// </summary>
        void completing(DelegateExecution execution, DelegateExecution subProcessInstance);

        /// <summary>
        /// called after the process instance is destroyed for  
        /// this activity to perform its outgoing control flow logic. 
        /// </summary>
        void completed(ActivityExecution execution);
    }

}