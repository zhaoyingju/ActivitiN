using System;
using bpmn_converter.converter;
using bpmn_converter.converter.util;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter
{


    public class AssociationXMLConverter : BaseBpmnXMLConverter
    {

        protected override Type getBpmnElementType()
        {
            return typeof(Association);
        }

        protected override string getXMLElementName()
        {
            return ELEMENT_ASSOCIATION;
        }

        protected override BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model)
        {
            Association association = new Association();
            BpmnXMLUtil.addXMLLocation(association, xtr);
            association.setSourceRef(xtr.getAttributeValue(null, ATTRIBUTE_FLOW_SOURCE_REF));
            association.setTargetRef(xtr.getAttributeValue(null, ATTRIBUTE_FLOW_TARGET_REF));
            association.setId(xtr.getAttributeValue(null, ATTRIBUTE_ID));

            parseChildElements(getXMLElementName(), association, model, xtr);

            return association;
        }

        protected override void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw)
        {
            Association association = (Association)element;
            writeDefaultAttribute(ATTRIBUTE_FLOW_SOURCE_REF, association.getSourceRef(), xtw);
            writeDefaultAttribute(ATTRIBUTE_FLOW_TARGET_REF, association.getTargetRef(), xtw);
        }

        protected override void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw)
        {
        }
    }
}
