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
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.parser
{

    public class InterfaceParser : BpmnXMLConstants
    {

        protected static ILog LOGGER = LogManager.GetLogger(typeof (InterfaceParser));

        public void parse(XMLStreamReader xtr, BpmnModel model)
        {

            Interface interfaceObject = new Interface();
            BpmnXMLUtil.addXMLLocation(interfaceObject, xtr);
            interfaceObject.setId(model.getTargetNamespace() + ":" + xtr.getAttributeValue(null, ATTRIBUTE_ID));
            interfaceObject.setName(xtr.getAttributeValue(null, ATTRIBUTE_NAME));
            interfaceObject.setImplementationRef(
                parseMessageRef(xtr.getAttributeValue(null, ATTRIBUTE_IMPLEMENTATION_REF), model));

            bool readyWithInterface = false;
            Operation operation = null;
            try
            {
                while (readyWithInterface == false && xtr.hasNext())
                {
                    xtr.next();
                    if (xtr.isStartElement() && ELEMENT_OPERATION.Equals(xtr.getLocalName()))
                    {
                        operation = new Operation();
                        BpmnXMLUtil.addXMLLocation(operation, xtr);
                        operation.setId(model.getTargetNamespace() + ":" + xtr.getAttributeValue(null, ATTRIBUTE_ID));
                        operation.setName(xtr.getAttributeValue(null, ATTRIBUTE_NAME));
                        operation.setImplementationRef(
                            parseMessageRef(xtr.getAttributeValue(null, ATTRIBUTE_IMPLEMENTATION_REF), model));

                    }
                    else if (xtr.isStartElement() && ELEMENT_IN_MESSAGE.Equals(xtr.getLocalName()))
                    {
                        String inMessageRef = xtr.getElementText();
                        if (operation != null && !String.IsNullOrWhiteSpace(inMessageRef))
                        {
                            operation.setInMessageRef(parseMessageRef(inMessageRef.Trim(), model));
                        }

                    }
                    else if (xtr.isStartElement() && ELEMENT_OUT_MESSAGE.Equals(xtr.getLocalName()))
                    {
                        String outMessageRef = xtr.getElementText();
                        if (operation != null && !String.IsNullOrWhiteSpace(outMessageRef))
                        {
                            operation.setOutMessageRef(parseMessageRef(outMessageRef.Trim(), model));
                        }

                    }
                    else if (xtr.isEndElement() && ELEMENT_OPERATION.equalsIgnoreCase(xtr.getLocalName()))
                    {
                        if (operation != null && !String.IsNullOrWhiteSpace(operation.getImplementationRef()))
                        {
                            interfaceObject.getOperations().Add(operation);
                        }

                    }
                    else if (xtr.isEndElement() && ELEMENT_INTERFACE.Equals(xtr.getLocalName()))
                    {
                        readyWithInterface = true;
                    }
                }
            }
            catch (Exception e)
            {
                LOGGER.Warn("Error parsing interface child elements", e);
            }

            model.getInterfaces().Add(interfaceObject);
        }

        protected String parseMessageRef(String messageRef, BpmnModel model)
        {
            String result = null;
            if (!String.IsNullOrWhiteSpace(messageRef))
            {
                int indexOfP = messageRef.IndexOf(':');
                if (indexOfP != -1)
                {
                    String prefix = messageRef.Substring(0, indexOfP);
                    String resolvedNamespace = model.getNamespace(prefix);
                    result = resolvedNamespace + ":" + messageRef.Substring(indexOfP + 1);
                }
                else
                {
                    result = model.getTargetNamespace() + ":" + messageRef;
                }
            }
            return result;
        }
    }
}