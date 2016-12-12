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


    public class MessageEventDefinitionParser : BaseChildElementParser
    {

        public override String getElementName()
        {
            return ELEMENT_EVENT_MESSAGEDEFINITION;
        }

        public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model)
        {
            if (parentElement as Event == null) return;

            MessageEventDefinition eventDefinition = new MessageEventDefinition();
            BpmnXMLUtil.addXMLLocation(eventDefinition, xtr);
            eventDefinition.setMessageRef(xtr.getAttributeValue(null, ATTRIBUTE_MESSAGE_REF));

            if (String.IsNullOrWhiteSpace(eventDefinition.getMessageRef()))
            {

                int indexOfP = eventDefinition.getMessageRef().IndexOf(':');
                if (indexOfP != -1)
                {
                    String prefix = eventDefinition.getMessageRef().Substring(0, indexOfP);
                    String resolvedNamespace = model.getNamespace(prefix);
                    eventDefinition.setMessageRef(resolvedNamespace + ":" +
                                                  eventDefinition.getMessageRef().Substring(indexOfP + 1));
                }
                else
                {
                    eventDefinition.setMessageRef(model.getTargetNamespace() + ":" + eventDefinition.getMessageRef());
                }

            }

            BpmnXMLUtil.parseChildElements(ELEMENT_EVENT_MESSAGEDEFINITION, eventDefinition, xtr, model);

            ((Event) parentElement).getEventDefinitions().Add(eventDefinition);
        }
    }
}