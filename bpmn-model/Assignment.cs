using System;

namespace org.activiti.bpmn.model
{


    public class Assignment : BaseElement
    {

        protected String from;
        protected String to;

        public String getFrom()
        {
            return from;
        }

        public void setFrom(String from)
        {
            this.from = from;
        }

        public String getTo()
        {
            return to;
        }

        public void setTo(String to)
        {
            this.to = to;
        }

        public override Object clone()
        {
            Assignment clone = new Assignment();
            clone.setValues(this);
            return clone;
        }

        public void setValues(Assignment otherAssignment)
        {
            setFrom(otherAssignment.getFrom());
            setTo(otherAssignment.getTo());
        }
    }
}