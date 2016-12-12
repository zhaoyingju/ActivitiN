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
using Common.Logging;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.parser
{










/**
 * //@author Tijs Rademakers

 */

    public class MessageFlowParser : BpmnXMLConstants
    {

        protected static ILog LOGGER = LogManager.GetLogger(typeof (MessageFlowParser));

        public void parse(XMLStreamReader xtr, BpmnModel model)
        {
            String id = xtr.getAttributeValue(null, ATTRIBUTE_ID);
            if (!String.IsNullOrWhiteSpace(id))
            {
                MessageFlow messageFlow = new MessageFlow();
                messageFlow.setId(id);

                String name = xtr.getAttributeValue(null, ATTRIBUTE_NAME);
                if (!String.IsNullOrWhiteSpace(name))
                {
                    messageFlow.setName(name);
                }

                String sourceRef = xtr.getAttributeValue(null, ATTRIBUTE_FLOW_SOURCE_REF);
                if (!String.IsNullOrWhiteSpace(sourceRef))
                {
                    messageFlow.setSourceRef(sourceRef);
                }

                String targetRef = xtr.getAttributeValue(null, ATTRIBUTE_FLOW_TARGET_REF);
                if (!String.IsNullOrWhiteSpace(targetRef))
                {
                    messageFlow.setTargetRef(targetRef);
                }

                String messageRef = xtr.getAttributeValue(null, ATTRIBUTE_MESSAGE_REF);
                if (!String.IsNullOrWhiteSpace(messageRef))
                {
                    messageFlow.setMessageRef(messageRef);
                }

                model.addMessageFlow(messageFlow);
            }
        }
    }
}