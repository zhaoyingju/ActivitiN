using System;

namespace org.activiti.bpmn.model
{


/**
 * //@author Lori Small
 */

    public class IntegerDataObject : ValuedDataObject
    {

        public override void setValue(Object value)
        {
            this.value = int.Parse(value.ToString());
        }

        public override Object clone()
        {
            IntegerDataObject clone = new IntegerDataObject();
            clone.setValues(this);
            return clone;
        }
    }
}