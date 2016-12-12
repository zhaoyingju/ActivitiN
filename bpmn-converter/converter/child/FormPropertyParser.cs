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

    public class FormPropertyParser : BaseChildElementParser
    {

        public override String getElementName()
        {
            return ELEMENT_FORMPROPERTY;
        }

        public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model)
        {

            if (parentElement as UserTask == null && parentElement as StartEvent == null) return;

            FormProperty property = new FormProperty();
            BpmnXMLUtil.addXMLLocation(property, xtr);
            property.setId(xtr.getAttributeValue(null, ATTRIBUTE_FORM_ID));
            property.setName(xtr.getAttributeValue(null, ATTRIBUTE_FORM_NAME));
            property.setType(xtr.getAttributeValue(null, ATTRIBUTE_FORM_TYPE));
            property.setVariable(xtr.getAttributeValue(null, ATTRIBUTE_FORM_VARIABLE));
            property.setExpression(xtr.getAttributeValue(null, ATTRIBUTE_FORM_EXPRESSION));
            property.setDefaultExpression(xtr.getAttributeValue(null, ATTRIBUTE_FORM_DEFAULT));
            property.setDatePattern(xtr.getAttributeValue(null, ATTRIBUTE_FORM_DATEPATTERN));
            if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_FORM_REQUIRED)))
            {
                property.setRequired(bool.Parse(xtr.getAttributeValue(null, ATTRIBUTE_FORM_REQUIRED)));
            }
            if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_FORM_READABLE)))
            {
                property.setReadable(bool.Parse(xtr.getAttributeValue(null, ATTRIBUTE_FORM_READABLE)));
            }
            if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_FORM_WRITABLE)))
            {
                property.setWriteable(bool.Parse(xtr.getAttributeValue(null, ATTRIBUTE_FORM_WRITABLE)));
            }

            bool readyWithFormProperty = false;
            try
            {
                while (readyWithFormProperty == false && xtr.hasNext())
                {
                    xtr.next();
                    if (xtr.isStartElement() && ELEMENT_VALUE.equalsIgnoreCase(xtr.getLocalName()))
                    {
                        FormValue value = new FormValue();
                        BpmnXMLUtil.addXMLLocation(value, xtr);
                        value.setId(xtr.getAttributeValue(null, ATTRIBUTE_ID));
                        value.setName(xtr.getAttributeValue(null, ATTRIBUTE_NAME));
                        property.getFormValues().Add(value);

                    }
                    else if (xtr.isEndElement() && getElementName().equalsIgnoreCase(xtr.getLocalName()))
                    {
                        readyWithFormProperty = true;
                    }
                }
            }
            catch (Exception e)
            {
                LOGGER.Warn("Error parsing form properties child elements", e);
            }

            if (parentElement as UserTask != null)
            {
                ((UserTask) parentElement).getFormProperties().Add(property);
            }
            else
            {
                ((StartEvent) parentElement).getFormProperties().Add(property);
            }
        }
    }
}