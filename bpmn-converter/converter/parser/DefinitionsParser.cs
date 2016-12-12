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
using bpmn_converter.converter.util;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.parser
{












/**
 * //@author Tijs Rademakers

 */

    public class DefinitionsParser : BpmnXMLConstants
    {

        protected static List<ExtensionAttribute> defaultAttributes = new List<ExtensionAttribute>()
        {
            new ExtensionAttribute(TYPE_LANGUAGE_ATTRIBUTE),
            new ExtensionAttribute(EXPRESSION_LANGUAGE_ATTRIBUTE),
            new ExtensionAttribute(TARGET_NAMESPACE_ATTRIBUTE)
        };

        //@SuppressWarnings("unchecked")

        public void parse(XMLStreamReader xtr, BpmnModel model)
        {
            model.setTargetNamespace(xtr.getAttributeValue(null, TARGET_NAMESPACE_ATTRIBUTE));
            for (int i = 0; i < xtr.getNamespaceCount(); i++)
            {
                String prefix = xtr.getNamespacePrefix(i);
                if (!String.IsNullOrWhiteSpace(prefix))
                {
                    model.addNamespace(prefix, xtr.getNamespaceURI(i));
                }
            }

            for (int i = 0; i < xtr.getAttributeCount(); i++)
            {
                ExtensionAttribute extensionAttribute = new ExtensionAttribute();
                extensionAttribute.setName(xtr.getAttributeLocalName(i));
                extensionAttribute.setValue(xtr.getAttributeValue(i));
                if (!String.IsNullOrWhiteSpace(xtr.getAttributeNamespace(i)))
                {
                    extensionAttribute.setNamespace(xtr.getAttributeNamespace(i));
                }
                if (!String.IsNullOrWhiteSpace(xtr.getAttributePrefix(i)))
                {
                    extensionAttribute.setNamespacePrefix(xtr.getAttributePrefix(i));
                }
                if (!BpmnXMLUtil.isBlacklisted(extensionAttribute, defaultAttributes))
                {
                    model.addDefinitionsAttribute(extensionAttribute);
                }
            }
        }
    }
}