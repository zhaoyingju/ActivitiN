using System;
using System.Collections.Generic;

namespace org.activiti.engine.impl.pvm.process
{
    [Serializable]
    public class ProcessElementImpl : PvmProcessElement
    {
        protected internal string id;
        protected internal ProcessDefinitionImpl processDefinition;
        protected internal IDictionary<string, object> properties;

        public ProcessElementImpl(string id, ProcessDefinitionImpl processDefinition)
        {
            this.id = id;
            this.processDefinition = processDefinition;
        }

        public virtual void setProperty(string name, object value)
        {
            if (properties == null)
            {
                properties = new Dictionary<string, object>();
            }
            properties[name] = value;
        }

        public virtual object getProperty(string name)
        {
            if (properties == null)
            {
                return null;
            }
            return properties[name];
        }

        public virtual IDictionary<string, object> Properties
        {
            get
            {
                if (properties == null)
                {
                    return new Dictionary<string, object>();
                }
                return properties;
            }
            set
            {
                this.properties = value;
            }
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual string Id
        {
            get
            {
                return id;
            }
        }


        public virtual PvmProcessDefinition ProcessDefinition
        {
            get
            {
                return processDefinition;
            }
        }
    }

}