using System;

namespace org.activiti.bpmn.model
{


/**
 * //@author Lori Small
 */

    public class StringDataObject : ValuedDataObject
    {

        public override void setValue(Object value)
        {
            this.value = value.ToString();
        }

        public override Object clone()
        {
            StringDataObject clone = new StringDataObject();
            clone.setValues(this);
            return clone;
        }
    }
}