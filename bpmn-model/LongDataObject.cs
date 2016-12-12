using System;

namespace org.activiti.bpmn.model
{


/**
 * //@author Lori Small
 */

    public class LongDataObject : ValuedDataObject
    {

        public override void setValue(Object value)
        {
            this.value = long.Parse(value.ToString());
        }

        public override object clone()
        {
            LongDataObject clone = new LongDataObject();
            clone.setValues(this);
            return clone;
        }
    }
}