using System;

namespace org.activiti.bpmn.model
{



/**
 * //@author Lori Small
 */

    public class DateDataObject : ValuedDataObject
    {

        public override void setValue(Object value)
        {
            this.value = (DateTime) value;
        }

        public override Object clone()
        {
            DateDataObject clone = new DateDataObject();
            clone.setValues(this);
            return clone;
        }
    }
}