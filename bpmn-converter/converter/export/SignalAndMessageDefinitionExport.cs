using System;
using System.Linq;
using bpmn_converter.converter;
using bpmn_converter.converter.util;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.export
{















    public class SignalAndMessageDefinitionExport : BpmnXMLConstants
    {

        public static void writeSignalsAndMessages(BpmnModel model, XMLStreamWriter xtw)
        {

            foreach (Process process  in model.getProcesses())
            {
                foreach (FlowElement flowElement  in process.findFlowElementsOfType(typeof (Event)))
                {
                    Event Event = (Event) flowElement;
                    if (Event.getEventDefinitions().Any())
                    {
                        EventDefinition eventDefinition = Event.getEventDefinitions()[0];
                        if (eventDefinition as SignalEventDefinition != null)
                        {
                            SignalEventDefinition signalEvent = (SignalEventDefinition) eventDefinition;
                            if (model.containsSignalId(signalEvent.getSignalRef()) == false)
                            {
                                Signal signal = new Signal(signalEvent.getSignalRef(), signalEvent.getSignalRef());
                                model.addSignal(signal);
                            }

                        }
                        else if (eventDefinition as MessageEventDefinition != null)
                        {
                            MessageEventDefinition messageEvent = (MessageEventDefinition) eventDefinition;
                            if (model.containsMessageId(messageEvent.getMessageRef()) == false)
                            {
                                Message message = new Message(messageEvent.getMessageRef(), messageEvent.getMessageRef(),
                                    null);
                                model.addMessage(message);
                            }
                        }
                    }
                }
            }

            foreach (Signal signal  in model.getSignals())
            {
                xtw.writeStartElement(ELEMENT_SIGNAL);
                xtw.writeAttribute(ATTRIBUTE_ID, signal.getId());
                xtw.writeAttribute(ATTRIBUTE_NAME, signal.getName());
                if (signal.getScope() != null)
                {
                    xtw.writeAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_SCOPE, signal.getScope());
                }
                xtw.writeEndElement();
            }

            foreach (Message message  in model.getMessages())
            {
                xtw.writeStartElement(ELEMENT_MESSAGE);
                String messageId = message.getId();
                // remove the namespace from the message id if set
                if (model.getTargetNamespace() != null && messageId.StartsWith(model.getTargetNamespace()))
                {
                    messageId = messageId.Replace(model.getTargetNamespace(), "");
                    messageId = messageId.ReplaceFirst(":", "");
                }
                else
                {
                    foreach (String prefix  in model.getNamespaces().Keys)
                    {
                        String Namespace = model.getNamespace(prefix);
                        if (messageId.StartsWith(Namespace))
                        {
                            messageId = messageId.Replace(model.getTargetNamespace(), "");
                            messageId = prefix + messageId;
                        }
                    }
                }
                xtw.writeAttribute(ATTRIBUTE_ID, messageId);
                if (!String.IsNullOrWhiteSpace(message.getName()))
                {
                    xtw.writeAttribute(ATTRIBUTE_NAME, message.getName());
                }
                if (!String.IsNullOrWhiteSpace(message.getItemRef()))
                {
                    // replace the namespace by the right prefix
                    String itemRef = message.getItemRef();
                    foreach (String prefix  in model.getNamespaces().Keys)
                    {
                        String Namespace = model.getNamespace(prefix);
                        if (itemRef.StartsWith(Namespace))
                        {
                            if (!prefix.Any())
                            {
                                itemRef = itemRef.Replace(Namespace + ":", "");
                            }
                            else
                            {
                                itemRef = itemRef.Replace(Namespace, prefix);
                            }
                            break;
                        }
                    }
                    xtw.writeAttribute(ATTRIBUTE_ITEM_REF, itemRef);
                }
                xtw.writeEndElement();
            }
        }
    }
}