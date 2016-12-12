using System;
using System.Collections.Generic;
using System.Linq;

namespace org.activiti.bpmn.model
{




    public class DataAssociation : BaseElement
    {

        protected String sourceRef;
        protected String targetRef;
        protected String transformation;
        protected List<Assignment> assignments = new List<Assignment>();

        public String getSourceRef()
        {
            return sourceRef;
        }

        public void setSourceRef(String sourceRef)
        {
            this.sourceRef = sourceRef;
        }

        public String getTargetRef()
        {
            return targetRef;
        }

        public void setTargetRef(String targetRef)
        {
            this.targetRef = targetRef;
        }

        public String getTransformation()
        {
            return transformation;
        }

        public void setTransformation(String transformation)
        {
            this.transformation = transformation;
        }

        public List<Assignment> getAssignments()
        {
            return assignments;
        }

        public void setAssignments(List<Assignment> assignments)
        {
            this.assignments = assignments;
        }

        public override Object clone()
        {
            DataAssociation clone = new DataAssociation();
            clone.setValues(this);
            return clone;
        }

        public void setValues(DataAssociation otherAssociation)
        {
            setSourceRef(otherAssociation.getSourceRef());
            setTargetRef(otherAssociation.getTargetRef());
            setTransformation(otherAssociation.getTransformation());

            assignments = new List<Assignment>();
            if (otherAssociation.getAssignments() != null && otherAssociation.getAssignments().Any())
            {
                foreach (Assignment assignment in otherAssociation.getAssignments())
                {
                    assignments.Add((Assignment)assignment.clone());
                }
            }
        }
    }
}