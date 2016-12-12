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
using System.Linq;
using bpmn_converter.converter;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.export
{









    public class CollaborationExport : BpmnXMLConstants
    {

        public static void writePools(BpmnModel model, XMLStreamWriter xtw)
        {
            if (model.getPools().Any())
            {
                xtw.writeStartElement(ELEMENT_COLLABORATION);
                xtw.writeAttribute(ATTRIBUTE_ID, "Collaboration");
                foreach (Pool pool  in model.getPools())
                {
                    xtw.writeStartElement(ELEMENT_PARTICIPANT);
                    xtw.writeAttribute(ATTRIBUTE_ID, pool.getId());
                    if (!String.IsNullOrWhiteSpace(pool.getName()))
                    {
                        xtw.writeAttribute(ATTRIBUTE_NAME, pool.getName());
                    }
                    if (!String.IsNullOrWhiteSpace(pool.getProcessRef()))
                    {
                        xtw.writeAttribute(ATTRIBUTE_PROCESS_REF, pool.getProcessRef());
                    }
                    xtw.writeEndElement();
                }

                foreach (MessageFlow messageFlow  in model.getMessageFlows().Values)
                {
                    xtw.writeStartElement(ELEMENT_MESSAGE_FLOW);
                    xtw.writeAttribute(ATTRIBUTE_ID, messageFlow.getId());
                    if (!String.IsNullOrWhiteSpace(messageFlow.getName()))
                    {
                        xtw.writeAttribute(ATTRIBUTE_NAME, messageFlow.getName());
                    }
                    if (!String.IsNullOrWhiteSpace(messageFlow.getSourceRef()))
                    {
                        xtw.writeAttribute(ATTRIBUTE_FLOW_SOURCE_REF, messageFlow.getSourceRef());
                    }
                    if (!String.IsNullOrWhiteSpace(messageFlow.getTargetRef()))
                    {
                        xtw.writeAttribute(ATTRIBUTE_FLOW_TARGET_REF, messageFlow.getTargetRef());
                    }
                    if (!String.IsNullOrWhiteSpace(messageFlow.getMessageRef()))
                    {
                        xtw.writeAttribute(ATTRIBUTE_MESSAGE_REF, messageFlow.getMessageRef());
                    }
                    xtw.writeEndElement();
                }

                xtw.writeEndElement();
            }
        }
    }
}