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
public class ServiceTaskXMLConverter : BaseBpmnXMLConverter {
  
  public Type getBpmnElementType() {
    return typeof(ServiceTask);
  }
  
  //@Override

  protected String getXMLElementName() {
    return ELEMENT_TASK_SERVICE;
  }

  //@Override

  protected BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model) {
		ServiceTask serviceTask = new ServiceTask();
		BpmnXMLUtil.addXMLLocation(serviceTask, xtr);
		if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_SERVICE_CLASS))) {
			serviceTask.setImplementationType(ImplementationType.IMPLEMENTATION_TYPE_CLASS);
			serviceTask.setImplementation(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_SERVICE_CLASS));
			
		} else if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_SERVICE_EXPRESSION))) {
			serviceTask.setImplementationType(ImplementationType.IMPLEMENTATION_TYPE_EXPRESSION);
			serviceTask.setImplementation(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_SERVICE_EXPRESSION));
			
		} else if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_SERVICE_DELEGATEEXPRESSION))) {
			serviceTask.setImplementationType(ImplementationType.IMPLEMENTATION_TYPE_DELEGATEEXPRESSION);
			serviceTask.setImplementation(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_SERVICE_DELEGATEEXPRESSION));
		
		} else if ("##WebService".Equals(xtr.getAttributeValue(null, ATTRIBUTE_TASK_IMPLEMENTATION))) {
		  serviceTask.setImplementationType(ImplementationType.IMPLEMENTATION_TYPE_WEBSERVICE);
		  serviceTask.setOperationRef(parseOperationRef(xtr.getAttributeValue(null, ATTRIBUTE_TASK_OPERATION_REF), model));
		}
	
		serviceTask.setResultVariableName(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_SERVICE_RESULTVARIABLE));
		if (StringUtils.isEmpty(serviceTask.getResultVariableName())) {
		  serviceTask.setResultVariableName(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, "resultVariable"));
		}
		
		serviceTask.setType(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TYPE));
		serviceTask.setExtensionId(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_SERVICE_EXTENSIONID));
	
		if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_SERVICE_SKIP_EXPRESSION))) {
		  serviceTask.setSkipExpression(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_SERVICE_SKIP_EXPRESSION));
		}
		parseChildElements(getXMLElementName(), serviceTask, model, xtr);
		
		return serviceTask;
  }
  
  //@Override

  protected void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    
    ServiceTask serviceTask = (ServiceTask) element;
    
    if (ImplementationType.IMPLEMENTATION_TYPE_CLASS.Equals(serviceTask.getImplementationType())) {
      writeQualifiedAttribute(ATTRIBUTE_TASK_SERVICE_CLASS, serviceTask.getImplementation(), xtw);
    } else if (ImplementationType.IMPLEMENTATION_TYPE_EXPRESSION.Equals(serviceTask.getImplementationType())) {
      writeQualifiedAttribute(ATTRIBUTE_TASK_SERVICE_EXPRESSION, serviceTask.getImplementation(), xtw);
    } else if (ImplementationType.IMPLEMENTATION_TYPE_DELEGATEEXPRESSION.Equals(serviceTask.getImplementationType())) {
      writeQualifiedAttribute(ATTRIBUTE_TASK_SERVICE_DELEGATEEXPRESSION, serviceTask.getImplementation(), xtw);
    }
    
    if (!String.IsNullOrWhiteSpace(serviceTask.getResultVariableName())) {
      writeQualifiedAttribute(ATTRIBUTE_TASK_SERVICE_RESULTVARIABLE, serviceTask.getResultVariableName(), xtw);
    }
    if (!String.IsNullOrWhiteSpace(serviceTask.getType())) {
      writeQualifiedAttribute(ATTRIBUTE_TYPE, serviceTask.getType(), xtw);
    }
    if (!String.IsNullOrWhiteSpace(serviceTask.getExtensionId())) {
      writeQualifiedAttribute(ATTRIBUTE_TASK_SERVICE_EXTENSIONID, serviceTask.getExtensionId(), xtw);
    }
    if (!String.IsNullOrWhiteSpace(serviceTask.getSkipExpression())) {
      writeQualifiedAttribute(ATTRIBUTE_TASK_SERVICE_SKIP_EXPRESSION, serviceTask.getSkipExpression(), xtw);
    }
  }
  
  //@Override

  protected bool writeExtensionChildElements(BaseElement element, bool didWriteExtensionStartElement, XMLStreamWriter xtw) {
    ServiceTask serviceTask = (ServiceTask) element;
    
    if (!serviceTask.getCustomProperties().isEmpty()) {
      foreach (CustomProperty customProperty  in serviceTask.getCustomProperties()) {
        
        if (StringUtils.isEmpty(customProperty.getSimpleValue())) {
          continue;
        }
        
        if (didWriteExtensionStartElement == false) {
          xtw.writeStartElement(ELEMENT_EXTENSIONS);
          didWriteExtensionStartElement = true;
        }
        xtw.writeStartElement(ACTIVITI_EXTENSIONS_PREFIX, ELEMENT_FIELD, ACTIVITI_EXTENSIONS_NAMESPACE);
        xtw.writeAttribute(ATTRIBUTE_FIELD_NAME, customProperty.getName());
        if ((customProperty.getSimpleValue().contains("${") || customProperty.getSimpleValue().contains("#{")) &&
            customProperty.getSimpleValue().contains("}")) {
          
          xtw.writeStartElement(ACTIVITI_EXTENSIONS_PREFIX, ATTRIBUTE_FIELD_EXPRESSION, ACTIVITI_EXTENSIONS_NAMESPACE);
        } else {
          xtw.writeStartElement(ACTIVITI_EXTENSIONS_PREFIX, ELEMENT_FIELD_STRING, ACTIVITI_EXTENSIONS_NAMESPACE);
        }
        xtw.writeCharacters(customProperty.getSimpleValue());
        xtw.writeEndElement();
        xtw.writeEndElement();
      }
    } else {
      didWriteExtensionStartElement = FieldExtensionExport.writeFieldExtensions(serviceTask.getFieldExtensions(), didWriteExtensionStartElement, xtw);
    }
    
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
