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


    public class SubProcessParser : BpmnXMLConstants
    {

        public void parse(XMLStreamReader xtr, List<SubProcess> activeSubProcessList, Process activeProcess)
        {
            SubProcess subProcess = null;
            if (ELEMENT_TRANSACTION.equalsIgnoreCase(xtr.getLocalName()))
            {
                subProcess = new Transaction();
            }
            else if (ATTRIBUTE_VALUE_TRUE.equalsIgnoreCase(xtr.getAttributeValue(null, ATTRIBUTE_TRIGGERED_BY)))
            {
                subProcess = new EventSubProcess();
            }
            else
            {
                subProcess = new SubProcess();
            }
            BpmnXMLUtil.addXMLLocation(subProcess, xtr);
            activeSubProcessList.Add(subProcess);

            subProcess.setId(xtr.getAttributeValue(null, ATTRIBUTE_ID));
            subProcess.setName(xtr.getAttributeValue(null, ATTRIBUTE_NAME));

            bool async = false;
            String asyncString = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_ACTIVITY_ASYNCHRONOUS);
            if (ATTRIBUTE_VALUE_TRUE.equalsIgnoreCase(asyncString))
            {
                async = true;
            }

            bool notExclusive = false;
            String exclusiveString = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_ACTIVITY_EXCLUSIVE);
            if (ATTRIBUTE_VALUE_FALSE.equalsIgnoreCase(exclusiveString))
            {
                notExclusive = true;
            }

            bool forCompensation = false;
            String compensationString = xtr.getAttributeValue(null, ATTRIBUTE_ACTIVITY_ISFORCOMPENSATION);
            if (ATTRIBUTE_VALUE_TRUE.equalsIgnoreCase(compensationString))
            {
                forCompensation = true;
            }

            subProcess.setAsynchronous(async);
            subProcess.setNotExclusive(notExclusive);
            subProcess.setForCompensation(forCompensation);
            if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_DEFAULT)))
            {
                subProcess.setDefaultFlow(xtr.getAttributeValue(null, ATTRIBUTE_DEFAULT));
            }

            if (activeSubProcessList.Count > 1)
            {
                activeSubProcessList[activeSubProcessList.Count - 2].addFlowElement(subProcess);

            }
            else
            {
                activeProcess.addFlowElement(subProcess);
            }
        }
    }
}