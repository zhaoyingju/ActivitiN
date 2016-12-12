using System;

namespace org.activiti.bpmn.model
{


    public class DataSpec : BaseElement
    {

        protected String name;
        protected String itemSubjectRef;
        protected Boolean _isCollection;

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public String getItemSubjectRef()
        {
            return itemSubjectRef;
        }

        public void setItemSubjectRef(String itemSubjectRef)
        {
            this.itemSubjectRef = itemSubjectRef;
        }

        public Boolean isCollection()
        {
            return _isCollection;
        }

        public void setCollection(Boolean isCollection)
        {
            this._isCollection = isCollection;
        }

        public override Object clone()
        {
            DataSpec clone = new DataSpec();
            clone.setValues(this);
            return clone;
        }

        public void setValues(DataSpec otherDataSpec)
        {
            setName(otherDataSpec.getName());
            setItemSubjectRef(otherDataSpec.getItemSubjectRef());
            setCollection(otherDataSpec.isCollection());
        }
    }
}