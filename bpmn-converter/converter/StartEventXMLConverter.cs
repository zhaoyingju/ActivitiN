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
public class StartEventXMLConverter : BaseBpmnXMLConverter {
  
  public Type getBpmnElementType() {
    return typeof(StartEvent);
  }
  
  //@Override

  protected String getXMLElementName() {
    return ELEMENT_EVENT_START;
  }
  
  //@Override

  protected BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model) {
    String formKey = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_FORM_FORMKEY);
    StartEvent startEvent = null;
    if (!String.IsNullOrWhiteSpace(formKey)) {
      if (model.getStartEventFormTypes() != null && model.getStartEventFormTypes().contains(formKey)) {
        startEvent = new AlfrescoStartEvent();
      }
    }
    if (startEvent == null) {
      startEvent = new StartEvent();
    }
    BpmnXMLUtil.addXMLLocation(startEvent, xtr);
    startEvent.setInitiator(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_EVENT_START_INITIATOR));
    startEvent.setFormKey(formKey);
    
    parseChildElements(getXMLElementName(), startEvent, model, xtr);
    
    return startEvent;
  }
  
  //@Override

  protected void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    StartEvent startEvent = (StartEvent) element;
    writeQualifiedAttribute(ATTRIBUTE_EVENT_START_INITIATOR, startEvent.getInitiator(), xtw);
    writeQualifiedAttribute(ATTRIBUTE_FORM_FORMKEY, startEvent.getFormKey(), xtw);
  }
  
  //@Override

  protected bool writeExtensionChildElements(BaseElement element, bool didWriteExtensionStartElement, XMLStreamWriter xtw) {
    StartEvent startEvent = (StartEvent) element;
    didWriteExtensionStartElement = writeFormProperties(startEvent, didWriteExtensionStartElement, xtw);
    return didWriteExtensionStartElement;
  }
  
  //@Override

  protected void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    StartEvent startEvent = (StartEvent) element;
    writeEventDefinitions(startEvent, startEvent.getEventDefinitions(), model, xtw);
  }
}
