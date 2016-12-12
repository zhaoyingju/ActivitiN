using System;

namespace org.activiti.bpmn.model
{


/**
 * //@author Lori Small
 */

    public class DoubleDataObject : ValuedDataObject
    {

        public override void setValue(Object value)
        {
            this.value = Double.Parse(value.ToString());
        }

        public override Object clone()
        {
            DoubleDataObject clone = new DoubleDataObject();
            clone.setValues(this);
            return clone;
        }
    }
}