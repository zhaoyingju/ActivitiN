using System;
using System.Collections.Generic;
using bpmn_converter.converter;
using bpmn_converter.converter.util;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter{






















/**
 * //@author Lori Small

 * //@author Tijs Rademakers

 */
public class ValuedDataObjectXMLConverter : BaseBpmnXMLConverter {
  
  private  Pattern xmlChars = Pattern.compile("[<>&]");
  private SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
  protected bool didWriteExtensionStartElement = false;
  
  public Type getBpmnElementType() {
    return typeof(ValuedDataObject);
  }
  
  //@Override

  protected String getXMLElementName() {
    return ELEMENT_DATA_OBJECT;
  }
  
  //@Override

  protected BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model) {
    ValuedDataObject dataObject = null;
    ItemDefinition itemSubjectRef = new ItemDefinition();

    String structureRef = xtr.getAttributeValue(null, ATTRIBUTE_DATA_ITEM_REF);
    if (!String.IsNullOrWhiteSpace(structureRef) && structureRef.contains(":")) {
      String dataType = structureRef.Substring(structureRef.indexOf(':') + 1);
      
      if (dataType.Equals("string")) {
        dataObject = new StringDataObject();
      } else if (dataType.Equals("int")) {
        dataObject = new IntegerDataObject();
      } else if (dataType.Equals("long")) {
        dataObject = new LongDataObject();
      } else if (dataType.Equals("double")) {
        dataObject = new DoubleDataObject();
      } else if (dataType.Equals("bool")) {
        dataObject = new BooleanDataObject();
      } else if (dataType.Equals("datetime")) {
        dataObject = new DateDataObject();
      } else {
        LOGGER.error("Error converting {}, invalid data type: " + dataType, xtr.getAttributeValue(null, ATTRIBUTE_DATA_NAME));
      }
    
    } else {
      // use String as default type
      dataObject = new StringDataObject();
      structureRef = "xsd:string";
    }
    
    if (dataObject != null) {
      dataObject.setId(xtr.getAttributeValue(null, ATTRIBUTE_DATA_ID)); 
      dataObject.setName(xtr.getAttributeValue(null, ATTRIBUTE_DATA_NAME)); 
      
      BpmnXMLUtil.addXMLLocation(dataObject, xtr);

      itemSubjectRef.setStructureRef(structureRef);
      dataObject.setItemSubjectRef(itemSubjectRef); 

      parseChildElements(getXMLElementName(), dataObject, model, xtr);
      
      List<ExtensionElement> valuesElement = dataObject.getExtensionElements().get("value");
      if (valuesElement != null && !valuesElement.isEmpty()) {
        ExtensionElement valueElement = valuesElement.get(0);
        if (!String.IsNullOrWhiteSpace(valueElement.getElementText())) {
          if (dataObject as DateDataObject !=null) {
            try {
              dataObject.setValue(sdf.parse(valueElement.getElementText()));
            } catch (Exception e) {
              LOGGER.error("Error converting {}", dataObject.getName(), e.getMessage());
            }
          } else {
            dataObject.setValue(valueElement.getElementText());
          }
        }
        
        // remove value element
        dataObject.getExtensionElements().remove("value");
      }
    }

    return dataObject;
  }

  //@Override

  protected void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    ValuedDataObject dataObject = (ValuedDataObject) element;
    if (dataObject.getItemSubjectRef() != null && !String.IsNullOrWhiteSpace(dataObject.getItemSubjectRef().getStructureRef())) {
      writeDefaultAttribute(ATTRIBUTE_DATA_ITEM_REF, dataObject.getItemSubjectRef().getStructureRef(), xtw);
    }
  }

  //@Override

  protected bool writeExtensionChildElements(BaseElement element, bool didWriteExtensionStartElement, XMLStreamWriter xtw) {
    ValuedDataObject dataObject = (ValuedDataObject) element;

    if (!String.IsNullOrWhiteSpace(dataObject.getId()) && dataObject.getValue() != null) {

      if (didWriteExtensionStartElement == false) { 
        xtw.writeStartElement(ELEMENT_EXTENSIONS);
        didWriteExtensionStartElement = true;
      }

      xtw.writeStartElement(ACTIVITI_EXTENSIONS_PREFIX, ELEMENT_DATA_VALUE, ACTIVITI_EXTENSIONS_NAMESPACE);
      if (dataObject.getValue() != null) {
        String value = null;
        if (dataObject as DateDataObject !=null) {
          value = sdf.format(dataObject.getValue());
        } else {
          value = dataObject.getValue().ToString();
        }

        if (dataObject as StringDataObject !=null && xmlChars.matcher(value).find())
        {
          xtw.writeCData(value);
        }
        else
        {
          xtw.writeCharacters(value);
        }
      }
      xtw.writeEndElement();
    }
    
    return didWriteExtensionStartElement;
  }

  //@Override

  protected void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
  }
}
