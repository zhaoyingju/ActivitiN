/* Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace org.activiti.bpmn.model
{








/**
 * //@author Tijs Rademakers
 */

    public abstract class BaseElement : HasExtensionAttributes
    {

        protected String id;
        protected int xmlRowNumber;
        protected int xmlColumnNumber;

        protected Dictionary<String, List<ExtensionElement>> extensionElements =
            new Dictionary<String, List<ExtensionElement>>();

        /** extension attributes could be part of each element */

        protected Dictionary<String, List<ExtensionAttribute>> attributes =
            new Dictionary<String, List<ExtensionAttribute>>();

        public String getId()
        {
            return id;
        }

        public void setId(String id)
        {
            this.id = id;
        }

        public int getXmlRowNumber()
        {
            return xmlRowNumber;
        }

        public void setXmlRowNumber(int xmlRowNumber)
        {
            this.xmlRowNumber = xmlRowNumber;
        }

        public int getXmlColumnNumber()
        {
            return xmlColumnNumber;
        }

        public void setXmlColumnNumber(int xmlColumnNumber)
        {
            this.xmlColumnNumber = xmlColumnNumber;
        }

        public Dictionary<String, List<ExtensionElement>> getExtensionElements()
        {
            return extensionElements;
        }

        public void addExtensionElement(ExtensionElement extensionElement)
        {
            if (extensionElement != null && !String.IsNullOrWhiteSpace(extensionElement.getName()))
            {
                List<ExtensionElement> elementList = null;
                if (this.extensionElements.ContainsKey(extensionElement.getName()) == false)
                {
                    elementList = new List<ExtensionElement>();
                    this.extensionElements.Add(extensionElement.getName(), elementList);
                }
                this.extensionElements[extensionElement.getName()].Add(extensionElement);
            }
        }

        public void setExtensionElements(Dictionary<String, List<ExtensionElement>> extensionElements)
        {
            this.extensionElements = extensionElements;
        }


        public Dictionary<String, List<ExtensionAttribute>> getAttributes()
        {
            return attributes;
        }


        public String getAttributeValue(String Namespace, String name)
        {
            List<ExtensionAttribute> attributes = getAttributes()[name];
            if (attributes != null && attributes.Any())
            {
                foreach (ExtensionAttribute attribute in attributes)
                {
                    if ((Namespace == null && attribute.getNamespace() == null)
                        || Namespace.Equals(attribute.getNamespace()))
                        return attribute.getValue();
                }
            }
            return null;
        }


        public void addAttribute(ExtensionAttribute attribute)
        {
            if (attribute != null && !String.IsNullOrWhiteSpace(attribute.getName()))
            {
                List<ExtensionAttribute> attributeList = null;
                if (this.attributes.ContainsKey(attribute.getName()) == false)
                {
                    attributeList = new List<ExtensionAttribute>();
                    this.attributes.Add(attribute.getName(), attributeList);
                }
                this.attributes[attribute.getName()].Add(attribute);
            }
        }


        public void setAttributes(Dictionary<String, List<ExtensionAttribute>> attributes)
        {
            this.attributes = attributes;
        }

        public void setValues(BaseElement otherElement)
        {
            setId(otherElement.getId());

            extensionElements = new Dictionary<String, List<ExtensionElement>>();
            if (otherElement.getExtensionElements() != null && otherElement.getExtensionElements().Any())
            {
                foreach (String key in otherElement.getExtensionElements().Keys)
                {
                    List<ExtensionElement> otherElementList = otherElement.getExtensionElements()[key];
                    if (otherElementList != null && otherElementList.Any())
                    {
                        List<ExtensionElement> elementList = new List<ExtensionElement>();
                        foreach (ExtensionElement extensionElement in otherElementList)
                        {
                            elementList.Add((ExtensionElement) extensionElement.clone());
                        }
                        extensionElements.Add(key, elementList);
                    }
                }
            }

            attributes = new Dictionary<String, List<ExtensionAttribute>>();
            if (otherElement.getAttributes() != null && otherElement.getAttributes().Any())
            {
                foreach (String key in otherElement.getAttributes().Keys)
                {
                    List<ExtensionAttribute> otherAttributeList = otherElement.getAttributes()[key];
                    if (otherAttributeList != null && otherAttributeList.Any())
                    {
                        List<ExtensionAttribute> attributeList = new List<ExtensionAttribute>();
                        foreach (ExtensionAttribute extensionAttribute in otherAttributeList)
                        {
                            attributeList.Add((ExtensionAttribute) extensionAttribute.clone());
                        }
                        attributes.Add(key, attributeList);
                    }
                }
            }
        }

        public abstract Object clone();
    }
}