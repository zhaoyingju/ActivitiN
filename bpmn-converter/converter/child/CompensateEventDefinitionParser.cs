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










/**
 * //@author Tijs Rademakers

 */

    public class CompensateEventDefinitionParser : BaseChildElementParser
    {

        public override String getElementName()
        {
            return ELEMENT_EVENT_COMPENSATEDEFINITION;
        }

        public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model)
        {
            if (parentElement as Event == null) return;

            CompensateEventDefinition eventDefinition = new CompensateEventDefinition();
            BpmnXMLUtil.addXMLLocation(eventDefinition, xtr);
            eventDefinition.setActivityRef(xtr.getAttributeValue(null, ATTRIBUTE_COMPENSATE_ACTIVITYREF));
            if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_COMPENSATE_WAITFORCOMPLETION)))
            {
                eventDefinition.setWaitForCompletion(
                    bool.Parse(xtr.getAttributeValue(null, ATTRIBUTE_COMPENSATE_WAITFORCOMPLETION)));
            }

            BpmnXMLUtil.parseChildElements(ELEMENT_EVENT_COMPENSATEDEFINITION, eventDefinition, xtr, model);

            ((Event) parentElement).getEventDefinitions().Add(eventDefinition);
        }
    }
}