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
using bpmn_converter.converter;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.export
{

    public class ProcessExport : BpmnXMLConstants
    {
        /**
   * default attributes taken from process instance attributes
   */

        public static List<ExtensionAttribute> defaultProcessAttributes = new List<ExtensionAttribute>()
        {
            new ExtensionAttribute(ATTRIBUTE_ID),
            new ExtensionAttribute(ATTRIBUTE_NAME),
            new ExtensionAttribute(ATTRIBUTE_PROCESS_EXECUTABLE),
            new ExtensionAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_PROCESS_CANDIDATE_USERS),
            new ExtensionAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_PROCESS_CANDIDATE_GROUPS)
        };

        //@SuppressWarnings("unchecked")

        public static void writeProcess(Process process, XMLStreamWriter xtw)
        {
            // start process element
            xtw.writeStartElement(ELEMENT_PROCESS);
            xtw.writeAttribute(ATTRIBUTE_ID, process.getId());

            if (!String.IsNullOrWhiteSpace(process.getName()))
            {
                xtw.writeAttribute(ATTRIBUTE_NAME, process.getName());
            }

            xtw.writeAttribute(ATTRIBUTE_PROCESS_EXECUTABLE, process.isExecutable().ToString());

            if (process.getCandidateStarterUsers().Any())
            {
                xtw.writeAttribute(ACTIVITI_EXTENSIONS_PREFIX, ACTIVITI_EXTENSIONS_NAMESPACE,
                    ATTRIBUTE_PROCESS_CANDIDATE_USERS,
                    BpmnXMLUtil.convertToDelimitedString(process.getCandidateStarterUsers()));
            }

            if (process.getCandidateStarterGroups().Any())
            {
                xtw.writeAttribute(ACTIVITI_EXTENSIONS_PREFIX, ACTIVITI_EXTENSIONS_NAMESPACE,
                    ATTRIBUTE_PROCESS_CANDIDATE_GROUPS,
                    BpmnXMLUtil.convertToDelimitedString(process.getCandidateStarterGroups()));
            }

            // write custom attributes
            BpmnXMLUtil.writeCustomAttributes(process.getAttributes().Values, xtw, defaultProcessAttributes);

            if (!String.IsNullOrWhiteSpace(process.getDocumentation()))
            {

                xtw.writeStartElement(ELEMENT_DOCUMENTATION);
                xtw.writeCharacters(process.getDocumentation());
                xtw.writeEndElement();
            }

            bool didWriteExtensionStartElement = ActivitiListenerExport.writeListeners(process, false, xtw);
            didWriteExtensionStartElement = BpmnXMLUtil.writeExtensionElements(process, didWriteExtensionStartElement,
                xtw);

            if (didWriteExtensionStartElement)
            {
                // closing extensions element
                xtw.writeEndElement();
            }

            LaneExport.writeLanes(process, xtw);
        }
    }
}