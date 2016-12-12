namespace org.activiti.engine.impl.context
{

    using DeploymentEntity = org.activiti.engine.impl.persistence.entity.DeploymentEntity;
    using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
    using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
    using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;

    public class ExecutionContext
    {

        protected internal ExecutionEntity execution;

        public ExecutionContext(InterpretableExecution execution)
        {
            this.execution = (ExecutionEntity)execution;
        }

        public virtual ExecutionEntity Execution
        {
            get
            {
                return execution;
            }
        }

        public virtual ExecutionEntity ProcessInstance
        {
            get
            {
                return execution.getProcessInstance();
            }
        }

        public virtual ProcessDefinitionEntity ProcessDefinition
        {
            get
            {
                return (ProcessDefinitionEntity)execution.ProcessDefinition;
            }
        }

        public virtual DeploymentEntity Deployment
        {
            get
            {
                string deploymentId = ProcessDefinition.DeploymentId;
                DeploymentEntity deployment = Context.CommandContext.DeploymentEntityManager.findDeploymentById(deploymentId);
                return deployment;
            }
        }
    }

}