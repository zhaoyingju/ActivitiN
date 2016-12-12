using System;

namespace org.activiti.bpmn.model
{


/**
 * //@author Lori Small
 */

    public abstract class ValuedDataObject : DataObject
    {

        protected Object value;

        public Object getValue()
        {
            return value;
        }

        public abstract void setValue(Object value);

        public abstract override Object clone();

        public void setValues(ValuedDataObject otherElement)
        {
            base.setValues(otherElement);
            if (otherElement.getValue() != null)
            {
                setValue(otherElement.getValue());
            }
        }

        public Boolean Equals(ValuedDataObject otherObject)
        {

            if (!otherObject.getItemSubjectRef().getStructureRef().Equals(this.itemSubjectRef.getStructureRef()))
                return false;
            if (!otherObject.getId().Equals(this.id)) return false;
            if (!otherObject.getName().Equals(this.name)) return false;
            if (!otherObject.getValue().Equals(this.value.ToString())) return false;

            return true;
        }
    }
}