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
using bpmn_converter.converter.util;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.child
{


    public class FieldExtensionParser : BaseChildElementParser
    {

        public override String getElementName()
        {
            return ELEMENT_FIELD;
        }

        public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model)
        {

            if (parentElement as ActivitiListener == null && parentElement as ServiceTask == null &&
                parentElement as SendTask == null) return;

            FieldExtension extension = new FieldExtension();
            BpmnXMLUtil.addXMLLocation(extension, xtr);
            extension.setFieldName(xtr.getAttributeValue(null, ATTRIBUTE_FIELD_NAME));

            if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_FIELD_STRING)))
            {
                extension.setStringValue(xtr.getAttributeValue(null, ATTRIBUTE_FIELD_STRING));

            }
            else if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_FIELD_EXPRESSION)))
            {
                extension.setExpression(xtr.getAttributeValue(null, ATTRIBUTE_FIELD_EXPRESSION));

            }
            else
            {
                bool readyWithFieldExtension = false;
                try
                {
                    while (readyWithFieldExtension == false && xtr.hasNext())
                    {
                        xtr.next();
                        if (xtr.isStartElement() && ELEMENT_FIELD_STRING.equalsIgnoreCase(xtr.getLocalName()))
                        {
                            extension.setStringValue(xtr.getElementText().Trim());

                        }
                        else if (xtr.isStartElement() && ATTRIBUTE_FIELD_EXPRESSION.equalsIgnoreCase(xtr.getLocalName()))
                        {
                            extension.setExpression(xtr.getElementText().Trim());

                        }
                        else if (xtr.isEndElement() && getElementName().equalsIgnoreCase(xtr.getLocalName()))
                        {
                            readyWithFieldExtension = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    LOGGER.Warn("Error parsing field extension child elements", e);
                }
            }

            if (parentElement as ActivitiListener != null)
            {
                ((ActivitiListener) parentElement).getFieldExtensions().Add(extension);
            }
            else if (parentElement as ServiceTask != null)
            {
                ((ServiceTask) parentElement).getFieldExtensions().Add(extension);
            }
            else
            {
                ((SendTask) parentElement).getFieldExtensions().Add(extension);
            }
        }
    }
}