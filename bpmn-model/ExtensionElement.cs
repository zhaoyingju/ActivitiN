using System;
using System.Collections.Generic;
using System.Linq;

namespace org.activiti.bpmn.model
{








    public class ExtensionElement : BaseElement
    {

        protected String name;
        protected String NamespacePrefix;
        protected String Namespace;
        protected String elementText;

        protected Dictionary<String, List<ExtensionElement>> childElements =
            new Dictionary<String, List<ExtensionElement>>();

        public String getElementText()
        {
            return elementText;
        }

        public void setElementText(String elementText)
        {
            this.elementText = elementText;
        }

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
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

        public Dictionary<String, List<ExtensionElement>> getChildElements()
        {
            return childElements;
        }

        public void addChildElement(ExtensionElement childElement)
        {
            if (childElement != null && !String.IsNullOrWhiteSpace(childElement.getName()))
            {
                List<ExtensionElement> elementList = null;
                if (this.childElements.ContainsKey(childElement.getName()) == false)
                {
                    elementList = new List<ExtensionElement>();
                    this.childElements.Add(childElement.getName(), elementList);
                }
                this.childElements[childElement.getName()].Add(childElement);
            }
        }

        public void setChildElements(Dictionary<String, List<ExtensionElement>> childElements)
        {
            this.childElements = childElements;
        }

        public override Object clone()
        {
            ExtensionElement clone = new ExtensionElement();
            clone.setValues(this);
            return clone;
        }

        public void setValues(ExtensionElement otherElement)
        {
            setName(otherElement.getName());
            setNamespacePrefix(otherElement.getNamespacePrefix());
            setNamespace(otherElement.getNamespace());
            setElementText(otherElement.getElementText());
            setAttributes(otherElement.getAttributes());

            childElements = new Dictionary<String, List<ExtensionElement>>();
            if (otherElement.getChildElements() != null && otherElement.getChildElements().Any())
            {
                foreach (String key in otherElement.getChildElements().Keys)
                {
                    List<ExtensionElement> otherElementList = otherElement.getChildElements()[key];
                    if (otherElementList != null && otherElementList.Any())
                    {
                        List<ExtensionElement> elementList = new List<ExtensionElement>();
                        foreach (ExtensionElement extensionElement in otherElementList)
                        {
                            elementList.Add((ExtensionElement) extensionElement.clone());
                        }
                        childElements.Add(key, elementList);
                    }
                }
            }
        }
    }
}