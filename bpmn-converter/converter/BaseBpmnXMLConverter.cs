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
using bpmn_converter.converter;
using bpmn_converter.converter.util;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.converter.child;
using org.activiti.bpmn.converter.export;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter
{
    public abstract class BaseBpmnXMLConverter : BpmnXMLConstants
    {

        protected static List<ExtensionAttribute> defaultElementAttributes = new List<ExtensionAttribute>() { new ExtensionAttribute(ATTRIBUTE_ID), new ExtensionAttribute(ATTRIBUTE_NAME) };

        protected static List<ExtensionAttribute> defaultActivityAttributes = new List<ExtensionAttribute>() {
            new ExtensionAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_ACTIVITY_ASYNCHRONOUS),
            new ExtensionAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_ACTIVITY_EXCLUSIVE),
            new ExtensionAttribute(ATTRIBUTE_DEFAULT),
            new ExtensionAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_ACTIVITY_ISFORCOMPENSATION)
        };

        public void convertToBpmnModel(XMLStreamReader xtr, BpmnModel model, Process activeProcess,
            List<SubProcess> activeSubProcessList)
        {

            string elementId = xtr.getAttributeValue(null, ATTRIBUTE_ID);
            string elementName = xtr.getAttributeValue(null, ATTRIBUTE_NAME);
            bool async = parseAsync(xtr);
            bool notExclusive = parseNotExclusive(xtr);
            string defaultFlow = xtr.getAttributeValue(null, ATTRIBUTE_DEFAULT);
            bool isForCompensation = parseForCompensation(xtr);

            BaseElement parsedElement = convertXMLToElement(xtr, model);

            if (parsedElement as Artifact != null)
            {
                Artifact currentArtifact = (Artifact)parsedElement;
                currentArtifact.setId(elementId);

                if (isInSubProcess(activeSubProcessList))
                {
                    SubProcess currentSubProcess = activeSubProcessList[activeSubProcessList.Count - 2];
                    currentSubProcess.addArtifact(currentArtifact);

                }
                else
                {
                    activeProcess.addArtifact(currentArtifact);
                }
            }

            if (parsedElement as FlowElement != null)
            {

                FlowElement currentFlowElement = (FlowElement)parsedElement;
                currentFlowElement.setId(elementId);
                currentFlowElement.setName(elementName);

                if (currentFlowElement as Activity != null)
                {

                    Activity activity = (Activity)currentFlowElement;
                    activity.setAsynchronous(async);
                    activity.setNotExclusive(notExclusive);
                    activity.setForCompensation(isForCompensation);
                    if (!string.IsNullOrWhiteSpace(defaultFlow))
                    {
                        activity.setDefaultFlow(defaultFlow);
                    }
                }

                if (currentFlowElement as Gateway != null)
                {
                    Gateway gateway = (Gateway)currentFlowElement;
                    if (!string.IsNullOrWhiteSpace(defaultFlow))
                    {
                        gateway.setDefaultFlow(defaultFlow);
                    }

                    gateway.setAsynchronous(async);
                    gateway.setNotExclusive(notExclusive);
                }

                if (currentFlowElement as DataObject != null)
                {
                    if (!(activeSubProcessList.Count < 0))
                    {
                        activeSubProcessList[activeSubProcessList.Count - 1].getDataObjects().Add((ValuedDataObject)parsedElement);
                    }
                    else
                    {
                        activeProcess.getDataObjects().Add((ValuedDataObject)parsedElement);
                    }
                }

                if (!(activeSubProcessList.Count < 0))
                {
                    activeSubProcessList[activeSubProcessList.Count - 1].addFlowElement(currentFlowElement);
                }
                else
                {
                    activeProcess.addFlowElement(currentFlowElement);
                }
            }
        }

        public void convertToXML(XMLStreamWriter xtw, BaseElement baseElement, BpmnModel model)
        {
            xtw.writeStartElement(getXMLElementName());
            bool didWriteExtensionStartElement = false;
            writeDefaultAttribute(ATTRIBUTE_ID, baseElement.getId(), xtw);
            if (baseElement as FlowElement != null)
            {
                writeDefaultAttribute(ATTRIBUTE_NAME, ((FlowElement)baseElement).getName(), xtw);
            }

            if (baseElement as Activity != null)
            {
                Activity activity = (Activity)baseElement;
                if (activity.isAsynchronous())
                {
                    writeQualifiedAttribute(ATTRIBUTE_ACTIVITY_ASYNCHRONOUS, ATTRIBUTE_VALUE_TRUE, xtw);
                    if (activity.isNotExclusive())
                    {
                        writeQualifiedAttribute(ATTRIBUTE_ACTIVITY_EXCLUSIVE, ATTRIBUTE_VALUE_FALSE, xtw);
                    }
                }
                if (activity.isForCompensation())
                {
                    writeDefaultAttribute(ATTRIBUTE_ACTIVITY_ISFORCOMPENSATION, ATTRIBUTE_VALUE_TRUE, xtw);
                }
                if (!string.IsNullOrWhiteSpace(activity.getDefaultFlow()))
                {
                    FlowElement defaultFlowElement = model.getFlowElement(activity.getDefaultFlow());
                    if (defaultFlowElement as SequenceFlow != null)
                    {
                        writeDefaultAttribute(ATTRIBUTE_DEFAULT, activity.getDefaultFlow(), xtw);
                    }
                }
            }

            if (baseElement as Gateway != null)
            {
                Gateway gateway = (Gateway)baseElement;
                if (gateway.isAsynchronous())
                {
                    writeQualifiedAttribute(ATTRIBUTE_ACTIVITY_ASYNCHRONOUS, ATTRIBUTE_VALUE_TRUE, xtw);
                    if (gateway.isNotExclusive())
                    {
                        writeQualifiedAttribute(ATTRIBUTE_ACTIVITY_EXCLUSIVE, ATTRIBUTE_VALUE_FALSE, xtw);
                    }
                }
                if (!string.IsNullOrWhiteSpace(gateway.getDefaultFlow()))
                {
                    FlowElement defaultFlowElement = model.getFlowElement(gateway.getDefaultFlow());
                    if (defaultFlowElement as SequenceFlow != null)
                    {
                        writeDefaultAttribute(ATTRIBUTE_DEFAULT, gateway.getDefaultFlow(), xtw);
                    }
                }
            }

            writeAdditionalAttributes(baseElement, model, xtw);

            if (baseElement as FlowElement != null)
            {
                FlowElement flowElement = (FlowElement)baseElement;
                if (!string.IsNullOrWhiteSpace(flowElement.getDocumentation()))
                {

                    xtw.writeStartElement(ELEMENT_DOCUMENTATION);
                    xtw.writeCharacters(flowElement.getDocumentation());
                    xtw.writeEndElement();
                }
            }

            didWriteExtensionStartElement = writeExtensionChildElements(baseElement, didWriteExtensionStartElement, xtw);
            didWriteExtensionStartElement = writeListeners(baseElement, didWriteExtensionStartElement, xtw);
            didWriteExtensionStartElement = BpmnXMLUtil.writeExtensionElements(baseElement, didWriteExtensionStartElement, model.getNamespaces(), xtw);
            if (baseElement as Activity != null)
            {
                Activity activity = (Activity)baseElement;
                FailedJobRetryCountExport.writeFailedJobRetryCount(activity, xtw);

            }

            if (didWriteExtensionStartElement)
            {
                xtw.writeEndElement();
            }

            if (baseElement as Activity != null)
            {
                Activity activity = (Activity)baseElement;
                MultiInstanceExport.writeMultiInstance(activity, xtw);

            }

            writeAdditionalChildElements(baseElement, model, xtw);

            xtw.writeEndElement();
        }

        protected abstract Type getBpmnElementType();

        protected abstract BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model) throws Exception;

        protected abstract string getXMLElementName();

        protected abstract void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw) throws Exception;

        protected bool writeExtensionChildElements(BaseElement element, bool didWriteExtensionStartElement, XMLStreamWriter xtw)
        {
            return didWriteExtensionStartElement;
        }

        protected abstract void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw) throws Exception;

        // To BpmnModel converter convenience methods

        protected void parseChildElements(string elementName, BaseElement parentElement, BpmnModel model, XMLStreamReader xtr)
        {
            parseChildElements(elementName, parentElement, null, model, xtr);
        }

        protected void parseChildElements(string elementName, BaseElement parentElement, Dictionary<string, BaseChildElementParser> additionalParsers,
            BpmnModel model, XMLStreamReader xtr)
        {

            Map<string, BaseChildElementParser> childParsers = new Dictionary<string, BaseChildElementParser>();
            if (additionalParsers != null)
            {
                childParsers.putAll(additionalParsers);
            }
            BpmnXMLUtil.parseChildElements(elementName, parentElement, xtr, childParsers, model);
        }

        //@SuppressWarnings("unchecked")

        protected ExtensionElement parseExtensionElement(XMLStreamReader xtr)
        {
            ExtensionElement extensionElement = new ExtensionElement();
            extensionElement.setName(xtr.getLocalName());
            if (!string.IsNullOrWhiteSpace(xtr.getNamespaceURI()))
            {
                extensionElement.setNamespace(xtr.getNamespaceURI());
            }
            if (!string.IsNullOrWhiteSpace(xtr.getPrefix()))
            {
                extensionElement.setNamespacePrefix(xtr.getPrefix());
            }

            BpmnXMLUtil.addCustomAttributes(xtr, extensionElement, defaultElementAttributes);

            bool readyWithExtensionElement = false;
            while (readyWithExtensionElement == false && xtr.hasNext())
            {
                xtr.next();
                if (xtr.isCharacters() || XMLStreamReader.CDATA == xtr.getEventType())
                {
                    if (!string.IsNullOrWhiteSpace(xtr.getText().Trim()))
                    {
                        extensionElement.setElementText(xtr.getText().Trim());
                    }
                }
                else if (xtr.isStartElement())
                {
                    ExtensionElement childExtensionElement = parseExtensionElement(xtr);
                    extensionElement.addChildElement(childExtensionElement);
                }
                else if (xtr.isEndElement() && extensionElement.getName().equalsIgnoreCase(xtr.getLocalName()))
                {
                    readyWithExtensionElement = true;
                }
            }
            return extensionElement;
        }

        protected bool parseAsync(XMLStreamReader xtr)
        {
            bool async = false;
            string asyncstring = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_ACTIVITY_ASYNCHRONOUS);
            if (ATTRIBUTE_VALUE_TRUE.equalsIgnoreCase(asyncstring))
            {
                async = true;
            }
            return async;
        }

        protected bool parseNotExclusive(XMLStreamReader xtr)
        {
            bool notExclusive = false;
            string exclusivestring = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_ACTIVITY_EXCLUSIVE);
            if (ATTRIBUTE_VALUE_FALSE.equalsIgnoreCase(exclusivestring))
            {
                notExclusive = true;
            }
            return notExclusive;
        }

        protected bool parseForCompensation(XMLStreamReader xtr)
        {
            bool isForCompensation = false;
            string compensationstring = xtr.getAttributeValue(null, ATTRIBUTE_ACTIVITY_ISFORCOMPENSATION);
            if (ATTRIBUTE_VALUE_TRUE.equalsIgnoreCase(compensationstring))
            {
                isForCompensation = true;
            }
            return isForCompensation;
        }

        protected List<string> parseDelimitedList(string expression)
        {
            return BpmnXMLUtil.parseDelimitedList(expression);
        }

        private bool isInSubProcess(List<SubProcess> subProcessList)
        {
            if (subProcessList.size() > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // To XML converter convenience methods

        protected string convertToDelimitedstring(List<string> stringList)
        {
            return BpmnXMLUtil.convertToDelimitedstring(stringList);
        }

        protected bool writeFormProperties(FlowElement flowElement, bool didWriteExtensionStartElement, XMLStreamWriter xtw)
        {

            List<FormProperty> propertyList = null;
            if (flowElement as UserTask != null)
            {
                propertyList = ((UserTask)flowElement).getFormProperties();
            }
            else if (flowElement as StartEvent != null)
            {
                propertyList = ((StartEvent)flowElement).getFormProperties();
            }

            if (propertyList != null)
            {

                foreach (FormProperty property in propertyList)
                {

                    if (!string.IsNullOrWhiteSpace(property.getId()))
                    {

                        if (didWriteExtensionStartElement == false)
                        {
                            xtw.writeStartElement(ELEMENT_EXTENSIONS);
                            didWriteExtensionStartElement = true;
                        }

                        xtw.writeStartElement(ACTIVITI_EXTENSIONS_PREFIX, ELEMENT_FORMPROPERTY, ACTIVITI_EXTENSIONS_NAMESPACE);
                        writeDefaultAttribute(ATTRIBUTE_FORM_ID, property.getId(), xtw);

                        writeDefaultAttribute(ATTRIBUTE_FORM_NAME, property.getName(), xtw);
                        writeDefaultAttribute(ATTRIBUTE_FORM_TYPE, property.getType(), xtw);
                        writeDefaultAttribute(ATTRIBUTE_FORM_EXPRESSION, property.getExpression(), xtw);
                        writeDefaultAttribute(ATTRIBUTE_FORM_VARIABLE, property.getVariable(), xtw);
                        writeDefaultAttribute(ATTRIBUTE_FORM_DEFAULT, property.getDefaultExpression(), xtw);
                        writeDefaultAttribute(ATTRIBUTE_FORM_DATEPATTERN, property.getDatePattern(), xtw);
                        if (property.isReadable() == false)
                        {
                            writeDefaultAttribute(ATTRIBUTE_FORM_READABLE, ATTRIBUTE_VALUE_FALSE, xtw);
                        }
                        if (property.isWriteable() == false)
                        {
                            writeDefaultAttribute(ATTRIBUTE_FORM_WRITABLE, ATTRIBUTE_VALUE_FALSE, xtw);
                        }
                        if (property.isRequired())
                        {
                            writeDefaultAttribute(ATTRIBUTE_FORM_REQUIRED, ATTRIBUTE_VALUE_TRUE, xtw);
                        }

                        foreach (FormValue formValue in property.getFormValues())
                        {
                            if (!string.IsNullOrWhiteSpace(formValue.getId()))
                            {
                                xtw.writeStartElement(ACTIVITI_EXTENSIONS_PREFIX, ELEMENT_VALUE, ACTIVITI_EXTENSIONS_NAMESPACE);
                                xtw.writeAttribute(ATTRIBUTE_ID, formValue.getId());
                                xtw.writeAttribute(ATTRIBUTE_NAME, formValue.getName());
                                xtw.writeEndElement();
                            }
                        }

                        xtw.writeEndElement();
                    }
                }
            }

            return didWriteExtensionStartElement;
        }

        protected bool writeListeners(BaseElement element, bool didWriteExtensionStartElement, XMLStreamWriter xtw)
        {
            return ActivitiListenerExport.writeListeners(element, didWriteExtensionStartElement, xtw);
        }

        protected void writeEventDefinitions(Event parentEvent, List<EventDefinition> eventDefinitions, BpmnModel model, XMLStreamWriter xtw)
        {
            foreach (EventDefinition eventDefinition in eventDefinitions)
            {
                if (eventDefinition as TimerEventDefinition != null)
                {
                    writeTimerDefinition(parentEvent, (TimerEventDefinition)eventDefinition, xtw);
                }
                else if (eventDefinition as SignalEventDefinition != null)
                {
                    writeSignalDefinition(parentEvent, (SignalEventDefinition)eventDefinition, xtw);
                }
                else if (eventDefinition as MessageEventDefinition != null)
                {
                    writeMessageDefinition(parentEvent, (MessageEventDefinition)eventDefinition, model, xtw);
                }
                else if (eventDefinition as ErrorEventDefinition != null)
                {
                    writeErrorDefinition(parentEvent, (ErrorEventDefinition)eventDefinition, xtw);
                }
                else if (eventDefinition as TerminateEventDefinition != null)
                {
                    writeTerminateDefinition(parentEvent, (TerminateEventDefinition)eventDefinition, xtw);
                }
                else if (eventDefinition as CompensateEventDefinition != null)
                {
                    writeCompensateDefinition(parentEvent, (CompensateEventDefinition)eventDefinition, xtw);
                }
            }
        }

        protected void writeTimerDefinition(Event parentEvent, TimerEventDefinition timerDefinition, XMLStreamWriter xtw)
        {
            xtw.writeStartElement(ELEMENT_EVENT_TIMERDEFINITION);
            bool didWriteExtensionStartElement = BpmnXMLUtil.writeExtensionElements(timerDefinition, false, xtw);
            if (didWriteExtensionStartElement)
            {
                xtw.writeEndElement();
            }
            if (!string.IsNullOrWhiteSpace(timerDefinition.getTimeDate()))
            {
                xtw.writeStartElement(ATTRIBUTE_TIMER_DATE);
                xtw.writeCharacters(timerDefinition.getTimeDate());
                xtw.writeEndElement();

            }
            else if (!string.IsNullOrWhiteSpace(timerDefinition.getTimeCycle()))
            {
                xtw.writeStartElement(ATTRIBUTE_TIMER_CYCLE);
                xtw.writeCharacters(timerDefinition.getTimeCycle());
                xtw.writeEndElement();

            }
            else if (!string.IsNullOrWhiteSpace(timerDefinition.getTimeDuration()))
            {
                xtw.writeStartElement(ATTRIBUTE_TIMER_DURATION);
                xtw.writeCharacters(timerDefinition.getTimeDuration());
                xtw.writeEndElement();
            }

            xtw.writeEndElement();
        }

        protected void writeSignalDefinition(Event parentEvent, SignalEventDefinition signalDefinition, XMLStreamWriter xtw)
        {
            xtw.writeStartElement(ELEMENT_EVENT_SIGNALDEFINITION);
            writeDefaultAttribute(ATTRIBUTE_SIGNAL_REF, signalDefinition.getSignalRef(), xtw);
            if (parentEvent as ThrowEvent != null && signalDefinition.isAsync())
            {
                BpmnXMLUtil.writeQualifiedAttribute(ATTRIBUTE_ACTIVITY_ASYNCHRONOUS, "true", xtw);
            }
            bool didWriteExtensionStartElement = BpmnXMLUtil.writeExtensionElements(signalDefinition, false, xtw);
            if (didWriteExtensionStartElement)
            {
                xtw.writeEndElement();
            }
            xtw.writeEndElement();
        }

        protected void writeCompensateDefinition(Event parentEvent, CompensateEventDefinition compensateEventDefinition, XMLStreamWriter xtw)
        {
            xtw.writeStartElement(ELEMENT_EVENT_COMPENSATEDEFINITION);
            writeDefaultAttribute(ATTRIBUTE_COMPENSATE_ACTIVITYREF, compensateEventDefinition.getActivityRef(), xtw);
            bool didWriteExtensionStartElement = BpmnXMLUtil.writeExtensionElements(compensateEventDefinition, false, xtw);
            if (didWriteExtensionStartElement)
            {
                xtw.writeEndElement();
            }
            xtw.writeEndElement();
        }

        protected void writeMessageDefinition(Event parentEvent, MessageEventDefinition messageDefinition, BpmnModel model, XMLStreamWriter xtw)
        {
            xtw.writeStartElement(ELEMENT_EVENT_MESSAGEDEFINITION);

            string messageRef = messageDefinition.getMessageRef();
            if (!string.IsNullOrWhiteSpace(messageRef))
            {
                // remove the namespace from the message id if set
                if (messageRef.StartsWith(model.getTargetNamespace()))
                {
                    messageRef = messageRef.Replace(model.getTargetNamespace(), "");
                    messageRef = messageRef.ReplaceFirst(":", "");
                }
                else
                {
                    foreach (string prefix in model.getNamespaces().Keys)
                    {
                        string Namespace = model.getNamespace(prefix);
                        if (messageRef.StartsWith(Namespace))
                        {
                            messageRef = messageRef.Replace(model.getTargetNamespace(), "");
                            messageRef = prefix + messageRef;
                        }
                    }
                }
            }
            writeDefaultAttribute(ATTRIBUTE_MESSAGE_REF, messageRef, xtw);
            bool didWriteExtensionStartElement = BpmnXMLUtil.writeExtensionElements(messageDefinition, false, xtw);
            if (didWriteExtensionStartElement)
            {
                xtw.writeEndElement();
            }
            xtw.writeEndElement();
        }

        protected void writeErrorDefinition(Event parentEvent, ErrorEventDefinition errorDefinition, XMLStreamWriter xtw)
        {
            xtw.writeStartElement(ELEMENT_EVENT_ERRORDEFINITION);
            writeDefaultAttribute(ATTRIBUTE_ERROR_REF, errorDefinition.getErrorCode(), xtw);
            bool didWriteExtensionStartElement = BpmnXMLUtil.writeExtensionElements(errorDefinition, false, xtw);
            if (didWriteExtensionStartElement)
            {
                xtw.writeEndElement();
            }
            xtw.writeEndElement();
        }

        protected void writeTerminateDefinition(Event parentEvent, TerminateEventDefinition terminateDefinition, XMLStreamWriter xtw)
        {
            xtw.writeStartElement(ELEMENT_EVENT_TERMINATEDEFINITION);
            bool didWriteExtensionStartElement = BpmnXMLUtil.writeExtensionElements(terminateDefinition, false, xtw);
            if (didWriteExtensionStartElement)
            {
                xtw.writeEndElement();
            }
            xtw.writeEndElement();
        }

        protected void writeDefaultAttribute(string attributeName, string value, XMLStreamWriter xtw)
        {
            BpmnXMLUtil.writeDefaultAttribute(attributeName, value, xtw);
        }

        protected void writeQualifiedAttribute(string attributeName, string value, XMLStreamWriter xtw)
        {
            BpmnXMLUtil.writeQualifiedAttribute(attributeName, value, xtw);
        }
    }
