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
using org.activiti.bpmn.converter.export;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.parser
{












/**
 * //@author Tijs Rademakers

 */

    public class ProcessParser : BpmnXMLConstants
    {

        public Process parse(XMLStreamReader xtr, BpmnModel model)
        {
            Process process = null;
            if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_ID)))
            {
                String processId = xtr.getAttributeValue(null, ATTRIBUTE_ID);
                process = new Process();
                process.setId(processId);
                BpmnXMLUtil.addXMLLocation(process, xtr);
                process.setName(xtr.getAttributeValue(null, ATTRIBUTE_NAME));
                if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_PROCESS_EXECUTABLE)))
                {
                    process.setExecutable(bool.Parse(xtr.getAttributeValue(null, ATTRIBUTE_PROCESS_EXECUTABLE)));
                }
                String candidateUsersString = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE,
                    ATTRIBUTE_PROCESS_CANDIDATE_USERS);
                if (!String.IsNullOrWhiteSpace(candidateUsersString))
                {
                    List<String> candidateUsers = BpmnXMLUtil.parseDelimitedList(candidateUsersString);
                    process.setCandidateStarterUsers(candidateUsers);
                }
                String candidateGroupsString = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE,
                    ATTRIBUTE_PROCESS_CANDIDATE_GROUPS);
                if (!String.IsNullOrWhiteSpace(candidateGroupsString))
                {
                    List<String> candidateGroups = BpmnXMLUtil.parseDelimitedList(candidateGroupsString);
                    process.setCandidateStarterGroups(candidateGroups);
                }

                BpmnXMLUtil.addCustomAttributes(xtr, process, ProcessExport.defaultProcessAttributes);

                model.getProcesses().Add(process);

            }
            return process;
        }
    }
}