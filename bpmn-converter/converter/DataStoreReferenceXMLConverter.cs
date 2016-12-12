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
using bpmn_converter.converter;
using bpmn_converter.converter.util;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter{










/**
 * //@author Tijs Rademakers

 */
public class DataStoreReferenceXMLConverter : BaseBpmnXMLConverter {

  public Type getBpmnElementType() {
    return typeof(DataStoreReference);
  }
  
  //@Override

  protected String getXMLElementName() {
    return ELEMENT_DATA_STORE_REFERENCE;
  }
  
  //@Override

  protected BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model) {
    DataStoreReference dataStoreRef = new DataStoreReference();
    BpmnXMLUtil.addXMLLocation(dataStoreRef, xtr);
    parseChildElements(getXMLElementName(), dataStoreRef, model, xtr);
    return dataStoreRef;
  }

  //@Override

  protected void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    DataStoreReference dataStoreRef = (DataStoreReference) element;
    if (!String.IsNullOrWhiteSpace(dataStoreRef.getDataStoreRef())) {
      xtw.writeAttribute(ATTRIBUTE_DATA_STORE_REF, dataStoreRef.getDataStoreRef());
    }
    
    if (!String.IsNullOrWhiteSpace(dataStoreRef.getItemSubjectRef())) {
      xtw.writeAttribute(ATTRIBUTE_ITEM_SUBJECT_REF, dataStoreRef.getItemSubjectRef());
    }
  }
  
  //@Override

  protected void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    DataStoreReference dataStoreRef = (DataStoreReference) element;
    if (!String.IsNullOrWhiteSpace(dataStoreRef.getDataState())) {
      xtw.writeStartElement(ELEMENT_DATA_STATE);
      xtw.writeCharacters(dataStoreRef.getDataState());
      xtw.writeEndElement();
    }
  }
}
