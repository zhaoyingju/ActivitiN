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
using bpmn_converter.converter;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.export
{

    public class MultiInstanceExport : BpmnXMLConstants
    {

        public static void writeMultiInstance(Activity activity, XMLStreamWriter xtw)
        {
            if (activity.getLoopCharacteristics() != null)
            {
                MultiInstanceLoopCharacteristics multiInstanceObject = activity.getLoopCharacteristics();
                if (!String.IsNullOrWhiteSpace(multiInstanceObject.getLoopCardinality()) ||
                    !String.IsNullOrWhiteSpace(multiInstanceObject.getInputDataItem()) ||
                    !String.IsNullOrWhiteSpace(multiInstanceObject.getCompletionCondition()))
                {

                    xtw.writeStartElement(ELEMENT_MULTIINSTANCE);
                    BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_MULTIINSTANCE_SEQUENTIAL,
                        multiInstanceObject.isSequential().ToString().ToLower(), xtw);
                    if (!String.IsNullOrWhiteSpace(multiInstanceObject.getInputDataItem()))
                    {
                        BpmnXMLUtil.writeQualifiedAttribute(ATTRIBUTE_MULTIINSTANCE_COLLECTION,
                            multiInstanceObject.getInputDataItem(), xtw);
                    }
                    if (!String.IsNullOrWhiteSpace(multiInstanceObject.getElementVariable()))
                    {
                        BpmnXMLUtil.writeQualifiedAttribute(ATTRIBUTE_MULTIINSTANCE_VARIABLE,
                            multiInstanceObject.getElementVariable(), xtw);
                    }
                    if (!String.IsNullOrWhiteSpace(multiInstanceObject.getLoopCardinality()))
                    {
                        xtw.writeStartElement(ELEMENT_MULTIINSTANCE_CARDINALITY);
                        xtw.writeCharacters(multiInstanceObject.getLoopCardinality());
                        xtw.writeEndElement();
                    }
                    if (!String.IsNullOrWhiteSpace(multiInstanceObject.getCompletionCondition()))
                    {
                        xtw.writeStartElement(ELEMENT_MULTIINSTANCE_CONDITION);
                        xtw.writeCharacters(multiInstanceObject.getCompletionCondition());
                        xtw.writeEndElement();
                    }
                    xtw.writeEndElement();
                }
            }
        }
    }
}