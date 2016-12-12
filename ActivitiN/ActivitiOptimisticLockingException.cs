namespace org.activiti.engine
{
    public class ActivitiOptimisticLockingException : ActivitiException
    {
        public ActivitiOptimisticLockingException(string message) : base(message)
        {
        }
    }
}