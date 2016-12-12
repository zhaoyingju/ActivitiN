using System.Collections.Generic;

namespace org.activiti.engine.impl.runtime
{


    using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;
    using ProcessInstanceBuilder = org.activiti.engine.runtime.ProcessInstanceBuilder;

    public class ProcessInstanceBuilderImpl : ProcessInstanceBuilder
    {

        protected internal RuntimeServiceImpl runtimeService;

        protected internal string processDefinitionId_Renamed;
        protected internal string processDefinitionKey_Renamed;
        protected internal string processInstanceName_Renamed;
        protected internal string businessKey_Renamed;
        protected internal string tenantId_Renamed;
        protected internal IDictionary<string, object> variables = new Dictionary<string, object>();

        public ProcessInstanceBuilderImpl(RuntimeServiceImpl runtimeService)
        {
            this.runtimeService = runtimeService;
        }

        public virtual ProcessInstanceBuilder processDefinitionId(string processDefinitionId)
        {
            this.processDefinitionId_Renamed = processDefinitionId;
            return this;
        }

        public virtual ProcessInstanceBuilder processDefinitionKey(string processDefinitionKey)
        {
            this.processDefinitionKey_Renamed = processDefinitionKey;
            return this;
        }

        public virtual ProcessInstanceBuilder processInstanceName(string processInstanceName)
        {
            this.processInstanceName_Renamed = processInstanceName;
            return this;
        }

        public virtual ProcessInstanceBuilder businessKey(string businessKey)
        {
            this.businessKey_Renamed = businessKey;
            return this;
        }

        public virtual ProcessInstanceBuilder tenantId(string tenantId)
        {
            this.tenantId_Renamed = tenantId;
            return this;
        }

        public virtual ProcessInstanceBuilder addVariable(string variableName, object value)
        {
            this.variables[variableName] = value;
            return this;
        }

        public virtual ProcessInstance start()
        {
            if (processDefinitionId_Renamed == null && processDefinitionKey_Renamed == null)
            {
                throw new ActivitiIllegalArgumentException("processDefinitionKey and processDefinitionId are null");
            }
            return runtimeService.startProcessInstance(this);
        }

        public virtual string ProcessDefinitionId
        {
            get
            {
                return processDefinitionId_Renamed;
            }
        }

        public virtual string ProcessDefinitionKey
        {
            get
            {
                return processDefinitionKey_Renamed;
            }
        }

        public virtual string ProcessInstanceName
        {
            get
            {
                return processInstanceName_Renamed;
            }
        }

        public virtual string BusinessKey
        {
            get
            {
                return businessKey_Renamed;
            }
        }

        public virtual string TenantId
        {
            get
            {
                return tenantId_Renamed;
            }
        }

        public virtual IDictionary<string, object> Variables
        {
            get
            {
                return variables;
            }
        }

    }

}