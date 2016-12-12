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

using System.Collections.Generic;
using System.Linq;
using bpmn_converter.converter.util;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.converter.child;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.parser
{

    public class ExtensionElementsParser : BpmnXMLConstants
    {

        public void parse(XMLStreamReader xtr, List<SubProcess> activeSubProcessList, Process activeProcess,
            BpmnModel model)
        {
            BaseElement parentElement = null;
            if (activeSubProcessList.Any())
            {
                parentElement = activeSubProcessList[activeSubProcessList.Count - 1];

            }
            else
            {
                parentElement = activeProcess;
            }

            bool readyWithChildElements = false;
            while (readyWithChildElements == false && xtr.hasNext())
            {
                xtr.next();
                if (xtr.isStartElement())
                {
                    if (ELEMENT_EXECUTION_LISTENER.Equals(xtr.getLocalName()))
                    {
                        new ExecutionListenerParser().parseChildElement(xtr, parentElement, model);
                    }
                    else if (ELEMENT_EVENT_LISTENER.Equals(xtr.getLocalName()))
                    {
                        new ActivitiEventListenerParser().parseChildElement(xtr, parentElement, model);
                    }
                    else if (ELEMENT_POTENTIAL_STARTER.Equals(xtr.getLocalName()))
                    {
                        new PotentialStarterParser().parse(xtr, activeProcess);
                    }
                    else
                    {
                        ExtensionElement extensionElement = BpmnXMLUtil.parseExtensionElement(xtr);
                        parentElement.addExtensionElement(extensionElement);
                    }

                }
                else if (xtr.isEndElement())
                {
                    if (ELEMENT_EXTENSIONS.Equals(xtr.getLocalName()))
                    {
                        readyWithChildElements = true;
                    }
                }
            }
        }
    }
}