using System;

namespace org.activiti.bpmn.model
{


/**
 * //@author Lori Small
 */

    public class BooleanDataObject : ValuedDataObject
    {

        public override void setValue(Object value)
        {
            this.value = Boolean.Parse(value.ToString());
        }

        public override Object clone()
        {
            BooleanDataObject clone = new BooleanDataObject();
            clone.setValues(this);
            return clone;
        }
    }
}