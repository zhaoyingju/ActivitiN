using System;

namespace org.activiti.bpmn.model
{

/**
 * //@author Lori Small
 */

    public class DataObject : FlowElement
    {

        protected ItemDefinition itemSubjectRef;

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public ItemDefinition getItemSubjectRef()
        {
            return itemSubjectRef;
        }

        public void setItemSubjectRef(ItemDefinition itemSubjectRef)
        {
            this.itemSubjectRef = itemSubjectRef;
        }

        public override Object clone()
        {
            DataObject clone = new DataObject();
            clone.setValues(this);
            return clone;
        }

        public void setValues(DataObject otherElement)
        {
            base.setValues(otherElement);

            setId(otherElement.getId());
            setName(otherElement.getName());
            setItemSubjectRef(otherElement.getItemSubjectRef());
        }
    }
}