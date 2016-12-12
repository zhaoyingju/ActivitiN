using System;

namespace org.activiti.bpmn.model
{

    public class ItemDefinition : BaseElement
    {

        protected String structureRef;
        protected String itemKind;

        public String getStructureRef()
        {
            return structureRef;
        }

        public void setStructureRef(String structureRef)
        {
            this.structureRef = structureRef;
        }

        public String getItemKind()
        {
            return itemKind;
        }

        public void setItemKind(String itemKind)
        {
            this.itemKind = itemKind;
        }

        public override Object clone()
        {
            ItemDefinition clone = new ItemDefinition();
            clone.setValues(this);
            return clone;
        }

        public void setValues(ItemDefinition otherElement)
        {
            base.setValues(otherElement);
            setStructureRef(otherElement.getStructureRef());
            setItemKind(otherElement.getItemKind());
        }
    }
}