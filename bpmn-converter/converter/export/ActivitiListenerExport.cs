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

    public class ActivitiListenerExport : BpmnXMLConstants
    {

        public static bool writeListeners(BaseElement element, bool didWriteExtensionStartElement, XMLStreamWriter xtw)
        {
            if (element as HasExecutionListeners != null)
            {
                didWriteExtensionStartElement = writeListeners(ELEMENT_EXECUTION_LISTENER,
                    ((HasExecutionListeners) element).getExecutionListeners(), didWriteExtensionStartElement, xtw);
            }
            // In case of a usertaks, also add task-listeners
            if (element as UserTask != null)
            {
                didWriteExtensionStartElement = writeListeners(ELEMENT_TASK_LISTENER,
                    ((UserTask) element).getTaskListeners(), didWriteExtensionStartElement, xtw);
            }

            // In case of a process-element, write the event-listeners
            if (element as Process != null)
            {
                didWriteExtensionStartElement = writeEventListeners(((Process) element).getEventListeners(),
                    didWriteExtensionStartElement, xtw);
            }

            return didWriteExtensionStartElement;
        }

        protected static bool writeEventListeners(List<EventListener> eventListeners,
            bool didWriteExtensionStartElement, XMLStreamWriter xtw)
        {

            if (eventListeners != null && eventListeners.Any())
            {
                foreach (EventListener eventListener in eventListeners)
                {
                    if (!didWriteExtensionStartElement)
                    {
                        xtw.writeStartElement(ELEMENT_EXTENSIONS);
                        didWriteExtensionStartElement = true;
                    }

                    xtw.writeStartElement(ACTIVITI_EXTENSIONS_PREFIX, ELEMENT_EVENT_LISTENER,
                        ACTIVITI_EXTENSIONS_NAMESPACE);
                    BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_EVENTS, eventListener.getEvents(), xtw);
                    BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_ENTITY_TYPE, eventListener.getEntityType(), xtw);

                    if (ImplementationType.IMPLEMENTATION_TYPE_CLASS.Equals(eventListener.getImplementationType()))
                    {
                        BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_CLASS, eventListener.getImplementation(),
                            xtw);
                    }
                    else if (
                        ImplementationType.IMPLEMENTATION_TYPE_DELEGATEEXPRESSION.Equals(
                            eventListener.getImplementationType()))
                    {
                        BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_DELEGATEEXPRESSION,
                            eventListener.getImplementation(), xtw);
                    }
                    else if (
                        ImplementationType.IMPLEMENTATION_TYPE_THROW_SIGNAL_EVENT.Equals(
                            eventListener.getImplementationType()))
                    {
                        BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_THROW_SIGNAL_EVENT_NAME,
                            eventListener.getImplementation(), xtw);
                        BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_THROW_EVENT_TYPE,
                            ATTRIBUTE_LISTENER_THROW_EVENT_TYPE_SIGNAL, xtw);
                    }
                    else if (
                        ImplementationType.IMPLEMENTATION_TYPE_THROW_SIGNAL_EVENT.Equals(
                            eventListener.getImplementationType()))
                    {
                        BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_THROW_SIGNAL_EVENT_NAME,
                            eventListener.getImplementation(), xtw);
                        BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_THROW_EVENT_TYPE,
                            ATTRIBUTE_LISTENER_THROW_EVENT_TYPE_GLOBAL_SIGNAL, xtw);
                    }
                    else if (
                        ImplementationType.IMPLEMENTATION_TYPE_THROW_MESSAGE_EVENT.Equals(
                            eventListener.getImplementationType()))
                    {
                        BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_THROW_MESSAGE_EVENT_NAME,
                            eventListener.getImplementation(), xtw);
                        BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_THROW_EVENT_TYPE,
                            ATTRIBUTE_LISTENER_THROW_EVENT_TYPE_MESSAGE, xtw);
                    }
                    else if (
                        ImplementationType.IMPLEMENTATION_TYPE_THROW_ERROR_EVENT.Equals(
                            eventListener.getImplementationType()))
                    {
                        BpmnXMLUtil.writeDefaultAttribute(
                            ATTRIBUTE_LISTENER_THROW_ERROR_EVENT_CODE,
                            eventListener.getImplementation(), xtw);
                        BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_THROW_EVENT_TYPE,
                            ATTRIBUTE_LISTENER_THROW_EVENT_TYPE_ERROR, xtw);
                    }

                    xtw.writeEndElement();
                }
            }

            return didWriteExtensionStartElement;
        }

        private static bool writeListeners(String xmlElementName, List<ActivitiListener> listenerList,
            bool didWriteExtensionStartElement, XMLStreamWriter xtw)
        {
            if (listenerList != null)
            {

                foreach (ActivitiListener listener  in listenerList)
                {

                    if (!String.IsNullOrWhiteSpace(listener.getEvent()))
                    {

                        if (!didWriteExtensionStartElement)
                        {
                            xtw.writeStartElement(ELEMENT_EXTENSIONS);
                            didWriteExtensionStartElement = true;
                        }

                        xtw.writeStartElement(ACTIVITI_EXTENSIONS_PREFIX, xmlElementName, ACTIVITI_EXTENSIONS_NAMESPACE);
                        BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_EVENT, listener.getEvent(), xtw);

                        if (ImplementationType.IMPLEMENTATION_TYPE_CLASS.Equals(listener.getImplementationType()))
                        {
                            BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_CLASS, listener.getImplementation(),
                                xtw);
                        }
                        else if (
                            ImplementationType.IMPLEMENTATION_TYPE_EXPRESSION.Equals(
                                listener.getImplementationType()))
                        {
                            BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_EXPRESSION,
                                listener.getImplementation(), xtw);
                        }
                        else if (
                            ImplementationType.IMPLEMENTATION_TYPE_DELEGATEEXPRESSION.Equals(
                                listener.getImplementationType()))
                        {
                            BpmnXMLUtil.writeDefaultAttribute(ATTRIBUTE_LISTENER_DELEGATEEXPRESSION,
                                listener.getImplementation(), xtw);
                        }

                        FieldExtensionExport.writeFieldExtensions(listener.getFieldExtensions(), true, xtw);

                        xtw.writeEndElement();
                    }
                }
            }
            return didWriteExtensionStartElement;
        }

    }
}