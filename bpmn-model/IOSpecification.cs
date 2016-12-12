using System;
using System.Collections.Generic;
using System.Linq;

namespace org.activiti.bpmn.model
{




    public class IOSpecification : BaseElement
    {

        protected List<DataSpec> dataInputs = new List<DataSpec>();
        protected List<DataSpec> dataOutputs = new List<DataSpec>();
        protected List<String> dataInputRefs = new List<String>();
        protected List<String> dataOutputRefs = new List<String>();

        public List<DataSpec> getDataInputs()
        {
            return dataInputs;
        }

        public void setDataInputs(List<DataSpec> dataInputs)
        {
            this.dataInputs = dataInputs;
        }

        public List<DataSpec> getDataOutputs()
        {
            return dataOutputs;
        }

        public void setDataOutputs(List<DataSpec> dataOutputs)
        {
            this.dataOutputs = dataOutputs;
        }

        public List<String> getDataInputRefs()
        {
            return dataInputRefs;
        }

        public void setDataInputRefs(List<String> dataInputRefs)
        {
            this.dataInputRefs = dataInputRefs;
        }

        public List<String> getDataOutputRefs()
        {
            return dataOutputRefs;
        }

        public void setDataOutputRefs(List<String> dataOutputRefs)
        {
            this.dataOutputRefs = dataOutputRefs;
        }

        public override Object clone()
        {
            IOSpecification clone = new IOSpecification();
            clone.setValues(this);
            return clone;
        }

        public void setValues(IOSpecification otherSpec)
        {
            dataInputs = new List<DataSpec>();
            if (otherSpec.getDataInputs() != null && otherSpec.getDataInputs().Any())
            {
                foreach (DataSpec dataSpec in otherSpec.getDataInputs())
                {
                    dataInputs.Add((DataSpec)dataSpec.clone());
                }
            }

            dataOutputs = new List<DataSpec>();
            if (otherSpec.getDataOutputs() != null && otherSpec.getDataOutputs().Any())
            {
                foreach (DataSpec dataSpec in otherSpec.getDataOutputs())
                {
                    dataOutputs.Add((DataSpec)dataSpec.clone());
                }
            }

            dataInputRefs = new List<String>(otherSpec.getDataInputRefs());
            dataOutputRefs = new List<String>(otherSpec.getDataOutputRefs());
        }
    }
}