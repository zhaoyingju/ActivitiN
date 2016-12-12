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
using org.activiti.bpmn.constants;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.parser
{









/**
 * //@author Tijs Rademakers

 */

    public class MessageParser : BpmnXMLConstants
    {

        public void parse(XMLStreamReader xtr, BpmnModel model)
        {
            if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_ID)))
            {
                String messageId = model.getTargetNamespace() + ":" + xtr.getAttributeValue(null, ATTRIBUTE_ID);
                String messageName = xtr.getAttributeValue(null, ATTRIBUTE_NAME);
                String itemRef = parseItemRef(xtr.getAttributeValue(null, ATTRIBUTE_ITEM_REF), model);
                Message message = new Message(messageId, messageName, itemRef);
                BpmnXMLUtil.addXMLLocation(message, xtr);
                BpmnXMLUtil.parseChildElements(ELEMENT_MESSAGE, message, xtr, model);
                model.addMessage(message);
            }
        }

        protected String parseItemRef(String itemRef, BpmnModel model)
        {
            String result = null;
            if (!String.IsNullOrWhiteSpace(itemRef))
            {
                int indexOfP = itemRef.IndexOf(':');
                if (indexOfP != -1)
                {
                    String prefix = itemRef.Substring(0, indexOfP);
                    String resolvedNamespace = model.getNamespace(prefix);
                    result = resolvedNamespace + ":" + itemRef.Substring(indexOfP + 1);
                }
                else
                {
                    result = model.getTargetNamespace() + ":" + itemRef;
                }
            }
            return result;
        }
    }
}