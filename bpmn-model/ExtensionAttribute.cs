using System;
using System.Text;

namespace org.activiti.bpmn.model
{



    public class ExtensionAttribute
    {

        protected String name;
        protected String value;
        protected String NamespacePrefix;
        protected String Namespace;

        public ExtensionAttribute()
        {
        }

        public ExtensionAttribute(String name)
        {
            this.name = name;
        }

        public ExtensionAttribute(String Namespace, String name)
        {
            this.Namespace = Namespace;
            this.name = name;
        }

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public String getValue()
        {
            return value;
        }

        public void setValue(String value)
        {
            this.value = value;
        }

        public String getNamespacePrefix()
        {
            return NamespacePrefix;
        }

        public void setNamespacePrefix(String NamespacePrefix)
        {
            this.NamespacePrefix = NamespacePrefix;
        }

        public String getNamespace()
        {
            return Namespace;
        }

        public void setNamespace(String Namespace)
        {
            this.Namespace = Namespace;
        }

        public String ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (NamespacePrefix != null)
            {
                sb.Append(NamespacePrefix);
                if (name != null)
                    sb.Append(":").Append(name);
            }
            else
                sb.Append(name);
            if (value != null)
                sb.Append("=").Append(value);
            return sb.ToString();
        }

        public Object clone()
        {
            ExtensionAttribute clone = new ExtensionAttribute();
            clone.setValues(this);
            return clone;
        }

        public void setValues(ExtensionAttribute otherAttribute)
        {
            setName(otherAttribute.getName());
            setValue(otherAttribute.getValue());
            setNamespacePrefix(otherAttribute.getNamespacePrefix());
            setNamespace(otherAttribute.getNamespace());
        }
    }
}