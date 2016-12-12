namespace org.activiti.engine
{

    public class ActivitiTaskAlreadyClaimedException : ActivitiException
    {
        /// <summary>
        /// the id of the task that is already claimed </summary>
        private string taskId;

        /// <summary>
        /// the assignee of the task that is already claimed </summary>
        private string taskAssignee;

        public ActivitiTaskAlreadyClaimedException(string taskId, string taskAssignee) : base("Task '" + taskId + "' is already claimed by someone else.")
        {
            this.taskId = taskId;
            this.taskAssignee = taskAssignee;
        }

        public virtual string TaskId
        {
            get
            {
                return this.taskId;
            }
        }

        public virtual string TaskAssignee
        {
            get
            {
                return this.taskAssignee;
            }
        }
    }
}