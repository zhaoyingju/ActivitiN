using System;
using bpmn_converter.converter;
using bpmn_converter.converter.util;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter
{

    public class BoundaryEventXMLConverter : BaseBpmnXMLConverter
    {

        protected override Type getBpmnElementType()
        {
            return typeof(BoundaryEvent);
        }

        //@Override

        protected override String getXMLElementName()
        {
            return ELEMENT_EVENT_BOUNDARY;
        }

        //@Override

        protected override BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model)
        {
            BoundaryEvent boundaryEvent = new BoundaryEvent();
            BpmnXMLUtil.addXMLLocation(boundaryEvent, xtr);
            if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_BOUNDARY_CANCELACTIVITY)))
            {
                String cancelActivity = xtr.getAttributeValue(null, ATTRIBUTE_BOUNDARY_CANCELACTIVITY);
                if (ATTRIBUTE_VALUE_FALSE.equalsIgnoreCase(cancelActivity))
                {
                    boundaryEvent.setCancelActivity(false);
                }
            }
            boundaryEvent.setAttachedToRefId(xtr.getAttributeValue(null, ATTRIBUTE_BOUNDARY_ATTACHEDTOREF));
            parseChildElements(getXMLElementName(), boundaryEvent, model, xtr);

            // Explicitly set cancel activity to false for error boundary events
            if (boundaryEvent.getEventDefinitions().size() == 1)
            {
                EventDefinition eventDef = boundaryEvent.getEventDefinitions().get(0);

                if (eventDef as ErrorEventDefinition != null)
                {
                    boundaryEvent.setCancelActivity(false);
                }
            }

            return boundaryEvent;
        }

        //@Override

        protected override void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw)
        {
            BoundaryEvent boundaryEvent = (BoundaryEvent)element;
            if (boundaryEvent.getAttachedToRef() != null)
            {
                writeDefaultAttribute(ATTRIBUTE_BOUNDARY_ATTACHEDTOREF, boundaryEvent.getAttachedToRef().getId(), xtw);
            }

            if (boundaryEvent.getEventDefinitions().size() == 1)
            {
                EventDefinition eventDef = boundaryEvent.getEventDefinitions().get(0);

                if (eventDef as ErrorEventDefinition != null == false)
                {
                    writeDefaultAttribute(ATTRIBUTE_BOUNDARY_CANCELACTIVITY, String.Parse(boundaryEvent.isCancelActivity()).toLowerCase(), xtw);
                }
            }
        }

        //@Override

        protected override void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw)
        {
            BoundaryEvent boundaryEvent = (BoundaryEvent)element;
            writeEventDefinitions(boundaryEvent, boundaryEvent.getEventDefinitions(), model, xtw);
        }
    }
}
