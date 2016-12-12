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
using org.activiti.bpmn.converter.export;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter{












/**
 * //@author Tijs Rademakers

 */
public class SendTaskXMLConverter : BaseBpmnXMLConverter {
  
  public Type getBpmnElementType() {
    return typeof(SendTask);
  }
  
  //@Override

  protected String getXMLElementName() {
    return ELEMENT_TASK_SEND;
  }

  //@Override

  protected BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model) {
		SendTask sendTask = new SendTask();
		BpmnXMLUtil.addXMLLocation(sendTask, xtr);
		sendTask.setType(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TYPE));
		
		if ("##WebService".Equals(xtr.getAttributeValue(null, ATTRIBUTE_TASK_IMPLEMENTATION))) {
		  sendTask.setImplementationType(ImplementationType.IMPLEMENTATION_TYPE_WEBSERVICE);
		  sendTask.setOperationRef(parseOperationRef(xtr.getAttributeValue(null, ATTRIBUTE_TASK_OPERATION_REF), model));
    }
		
		parseChildElements(getXMLElementName(), sendTask, model, xtr);
		
		return sendTask;
  }
  
  //@Override

  protected void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    
    SendTask sendTask = (SendTask) element;
    
    if (!String.IsNullOrWhiteSpace(sendTask.getType())) {
      writeQualifiedAttribute(ATTRIBUTE_TYPE, sendTask.getType(), xtw);
    }
  }
  
  //@Override

  protected bool writeExtensionChildElements(BaseElement element, bool didWriteExtensionStartElement, XMLStreamWriter xtw) {
    SendTask sendTask = (SendTask) element;
    didWriteExtensionStartElement = FieldExtensionExport.writeFieldExtensions(sendTask.getFieldExtensions(), didWriteExtensionStartElement, xtw);
    return didWriteExtensionStartElement;
  }
  
  //@Override

  protected void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
  }
  
  protected String parseOperationRef(String operationRef, BpmnModel model) {
    String result = null;
    if (!String.IsNullOrWhiteSpace(operationRef)) {
      int indexOfP = operationRef.indexOf(':');
      if (indexOfP != -1) {
        String prefix = operationRef.Substring(0, indexOfP);
        String resolvedNamespace = model.getNamespace(prefix);
        result = resolvedNamespace + ":" + operationRef.Substring(indexOfP + 1);
      } else {
        result = model.getTargetNamespace() + ":" + operationRef;
      }
    }
    return result;
  }
}
