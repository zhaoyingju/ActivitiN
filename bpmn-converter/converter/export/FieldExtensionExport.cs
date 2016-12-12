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
using bpmn_converter.converter;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.export
{










    public class FieldExtensionExport : BpmnXMLConstants
    {

        public static bool writeFieldExtensions(List<FieldExtension> fieldExtensionList,
            bool didWriteExtensionStartElement, XMLStreamWriter xtw)
        {

            foreach (FieldExtension fieldExtension  in fieldExtensionList)
            {

                if (!String.IsNullOrWhiteSpace(fieldExtension.getFieldName()))
                {

                    if (!String.IsNullOrWhiteSpace(fieldExtension.getStringValue()) ||
                        !String.IsNullOrWhiteSpace(fieldExtension.getExpression()))
                    {

                        if (didWriteExtensionStartElement == false)
                        {
                            xtw.writeStartElement(ELEMENT_EXTENSIONS);
                            didWriteExtensionStartElement = true;
                        }

                        xtw.writeStartElement(ACTIVITI_EXTENSIONS_PREFIX, ELEMENT_FIELD, ACTIVITI_EXTENSIONS_NAMESPACE);
                        BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_FIELD_NAME, fieldExtension.getFieldName(), xtw);

                        if (!String.IsNullOrWhiteSpace(fieldExtension.getStringValue()))
                        {
                            xtw.writeStartElement(ACTIVITI_EXTENSIONS_PREFIX, ELEMENT_FIELD_STRING,
                                ACTIVITI_EXTENSIONS_NAMESPACE);
                            xtw.writeCData(fieldExtension.getStringValue());
                        }
                        else
                        {
                            xtw.writeStartElement(ACTIVITI_EXTENSIONS_PREFIX, ATTRIBUTE_FIELD_EXPRESSION,
                                ACTIVITI_EXTENSIONS_NAMESPACE);
                            xtw.writeCharacters(fieldExtension.getExpression());
                        }
                        xtw.writeEndElement();
                        xtw.writeEndElement();
                    }
                }
            }
            return didWriteExtensionStartElement;
        }
    }
}