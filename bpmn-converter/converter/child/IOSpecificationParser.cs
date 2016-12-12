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

    public class IOSpecificationParser : BaseChildElementParser
    {

        public override String getElementName()
        {
            return ELEMENT_IOSPECIFICATION;
        }

        public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model)
        {

            if (parentElement as Activity == null && parentElement as Process == null) return;

            IOSpecification ioSpecification = new IOSpecification();
            BpmnXMLUtil.addXMLLocation(ioSpecification, xtr);
            bool readyWithIOSpecification = false;
            try
            {
                while (readyWithIOSpecification == false && xtr.hasNext())
                {
                    xtr.next();
                    if (xtr.isStartElement() && ELEMENT_DATA_INPUT.equalsIgnoreCase(xtr.getLocalName()))
                    {
                        DataSpec dataSpec = new DataSpec();
                        BpmnXMLUtil.addXMLLocation(dataSpec, xtr);
                        dataSpec.setId(xtr.getAttributeValue(null, ATTRIBUTE_ID));
                        dataSpec.setName(xtr.getAttributeValue(null, ATTRIBUTE_NAME));
                        dataSpec.setItemSubjectRef(
                            parseItemSubjectRef(xtr.getAttributeValue(null, ATTRIBUTE_ITEM_SUBJECT_REF), model));
                        ioSpecification.getDataInputs().Add(dataSpec);

                    }
                    else if (xtr.isStartElement() && ELEMENT_DATA_OUTPUT.equalsIgnoreCase(xtr.getLocalName()))
                    {
                        DataSpec dataSpec = new DataSpec();
                        BpmnXMLUtil.addXMLLocation(dataSpec, xtr);
                        dataSpec.setId(xtr.getAttributeValue(null, ATTRIBUTE_ID));
                        dataSpec.setName(xtr.getAttributeValue(null, ATTRIBUTE_NAME));
                        dataSpec.setItemSubjectRef(
                            parseItemSubjectRef(xtr.getAttributeValue(null, ATTRIBUTE_ITEM_SUBJECT_REF), model));
                        ioSpecification.getDataOutputs().Add(dataSpec);

                    }
                    else if (xtr.isStartElement() && ELEMENT_DATA_INPUT_REFS.equalsIgnoreCase(xtr.getLocalName()))
                    {
                        String dataInputRefs = xtr.getElementText();
                        if (!String.IsNullOrWhiteSpace(dataInputRefs))
                        {
                            ioSpecification.getDataInputRefs().Add(dataInputRefs.Trim());
                        }

                    }
                    else if (xtr.isStartElement() && ELEMENT_DATA_OUTPUT_REFS.equalsIgnoreCase(xtr.getLocalName()))
                    {
                        String dataOutputRefs = xtr.getElementText();
                        if (!String.IsNullOrWhiteSpace(dataOutputRefs))
                        {
                            ioSpecification.getDataOutputRefs().Add(dataOutputRefs.Trim());
                        }

                    }
                    else if (xtr.isEndElement() && getElementName().equalsIgnoreCase(xtr.getLocalName()))
                    {
                        readyWithIOSpecification = true;
                    }
                }
            }
            catch (Exception e)
            {
                LOGGER.Warn("Error parsing ioSpecification child elements", e);
            }

            if (parentElement as Process != null)
            {
                ((Process) parentElement).setIoSpecification(ioSpecification);
            }
            else
            {
                ((Activity) parentElement).setIoSpecification(ioSpecification);
            }
        }

        protected String parseItemSubjectRef(String itemSubjectRef, BpmnModel model)
        {
            String result = null;
            if (!String.IsNullOrWhiteSpace(itemSubjectRef))
            {
                int indexOfP = itemSubjectRef.IndexOf(':');
                if (indexOfP != -1)
                {
                    String prefix = itemSubjectRef.Substring(0, indexOfP);
                    String resolvedNamespace = model.getNamespace(prefix);
                    result = resolvedNamespace + ":" + itemSubjectRef.Substring(indexOfP + 1);
                }
                else
                {
                    result = model.getTargetNamespace() + ":" + itemSubjectRef;
                }
            }
            return result;
        }
    }
}