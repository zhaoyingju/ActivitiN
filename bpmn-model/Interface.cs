using System;
using System.Collections.Generic;
using System.Linq;

namespace org.activiti.bpmn.model
{




    public class Interface : BaseElement
    {

        protected String name;
        protected String implementationRef;
        protected List<Operation> operations = new List<Operation>();

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public String getImplementationRef()
        {
            return implementationRef;
        }

        public void setImplementationRef(String implementationRef)
        {
            this.implementationRef = implementationRef;
        }

        public List<Operation> getOperations()
        {
            return operations;
        }

        public void setOperations(List<Operation> operations)
        {
            this.operations = operations;
        }

        public override Object clone()
        {
            Interface clone = new Interface();
            clone.setValues(this);
            return clone;
        }

        public void setValues(Interface otherElement)
        {
            base.setValues(otherElement);
            setName(otherElement.getName());
            setImplementationRef(otherElement.getImplementationRef());

            operations = new List<Operation>();
            if (otherElement.getOperations() != null && otherElement.getOperations().Any())
            {
                foreach (Operation operation in otherElement.getOperations())
                {
                    operations.Add((Operation)operation.clone());
                }
            }
        }
    }
}