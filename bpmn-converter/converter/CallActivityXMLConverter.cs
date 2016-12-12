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
using org.activiti.bpmn.converter.child;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter{
















/**
 * //@author Tijs Rademakers

 */
public class CallActivityXMLConverter : BaseBpmnXMLConverter {
  
  protected Dictionary<String, BaseChildElementParser> childParserMap = new Dictionary<String, BaseChildElementParser>();
  
  public CallActivityXMLConverter() {
  	InParameterParser inParameterParser = new InParameterParser();
  	childParserMap.Add(inParameterParser.getElementName(), inParameterParser);
    OutParameterParser outParameterParser = new OutParameterParser();
    childParserMap.Add(outParameterParser.getElementName(), outParameterParser);
  }

  public Type getBpmnElementType() {
    return typeof(CallActivity);
  }
  
  //@Override

  protected String getXMLElementName() {
    return ELEMENT_CALL_ACTIVITY;
  }
  
  //@Override

  protected BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model) {
    CallActivity callActivity = new CallActivity();
    BpmnXMLUtil.addXMLLocation(callActivity, xtr);
    callActivity.setCalledElement(xtr.getAttributeValue(null, ATTRIBUTE_CALL_ACTIVITY_CALLEDELEMENT));
    parseChildElements(getXMLElementName(), callActivity, childParserMap, model, xtr);
    return callActivity;
  }

  //@Override

  protected void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    CallActivity callActivity = (CallActivity) element;
    if (!String.IsNullOrWhiteSpace(callActivity.getCalledElement())) {
      xtw.writeAttribute(ATTRIBUTE_CALL_ACTIVITY_CALLEDELEMENT, callActivity.getCalledElement());
    }
  }
  
  //@Override

  protected bool writeExtensionChildElements(BaseElement element, bool didWriteExtensionStartElement, XMLStreamWriter xtw) {
    CallActivity callActivity = (CallActivity) element;
    didWriteExtensionStartElement = writeIOParameters(ELEMENT_CALL_ACTIVITY_IN_PARAMETERS, callActivity.getInParameters(), didWriteExtensionStartElement, xtw);
    didWriteExtensionStartElement = writeIOParameters(ELEMENT_CALL_ACTIVITY_OUT_PARAMETERS, callActivity.getOutParameters(), didWriteExtensionStartElement, xtw);
    return didWriteExtensionStartElement;
  }

  //@Override

  protected void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
  }
  
  private bool writeIOParameters(String elementName, List<IOParameter> parameterList, bool didWriteExtensionStartElement, XMLStreamWriter xtw) {
    if (parameterList.isEmpty()) return didWriteExtensionStartElement;
    
    foreach (IOParameter ioParameter  in parameterList) {
      if (didWriteExtensionStartElement == false) { 
        xtw.writeStartElement(ELEMENT_EXTENSIONS);
        didWriteExtensionStartElement = true;
      }
      
      xtw.writeStartElement(ACTIVITI_EXTENSIONS_PREFIX, elementName, ACTIVITI_EXTENSIONS_NAMESPACE);
      if (!String.IsNullOrWhiteSpace(ioParameter.getSource())) {
        writeDefaultAttribute(ATTRIBUTE_IOPARAMETER_SOURCE, ioParameter.getSource(), xtw);
      }
      if (!String.IsNullOrWhiteSpace(ioParameter.getSourceExpression())) {
        writeDefaultAttribute(ATTRIBUTE_IOPARAMETER_SOURCE_EXPRESSION, ioParameter.getSourceExpression(), xtw);
      }
      if (!String.IsNullOrWhiteSpace(ioParameter.getTarget())) {
        writeDefaultAttribute(ATTRIBUTE_IOPARAMETER_TARGET, ioParameter.getTarget(), xtw);
      }
      
      xtw.writeEndElement();
    }
    
    return didWriteExtensionStartElement;
  }
  
  public class InParameterParser : BaseChildElementParser {

    public override  String getElementName() {
      return ELEMENT_CALL_ACTIVITY_IN_PARAMETERS;
    }

    public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model) {
      String source = xtr.getAttributeValue(null, ATTRIBUTE_IOPARAMETER_SOURCE);
      String sourceExpression = xtr.getAttributeValue(null, ATTRIBUTE_IOPARAMETER_SOURCE_EXPRESSION);
      String target = xtr.getAttributeValue(null, ATTRIBUTE_IOPARAMETER_TARGET);
      if((!String.IsNullOrWhiteSpace(source) || !String.IsNullOrWhiteSpace(sourceExpression)) && !String.IsNullOrWhiteSpace(target)) {
        
        IOParameter parameter = new IOParameter();
        if(!String.IsNullOrWhiteSpace(sourceExpression)) {
          parameter.setSourceExpression(sourceExpression);
        } else {
          parameter.setSource(source);
        }
        
        parameter.setTarget(target);
        
        ((CallActivity) parentElement).getInParameters().Add(parameter);
      }
    }
  }
  
  public class OutParameterParser : BaseChildElementParser {

    public override  String getElementName() {
      return ELEMENT_CALL_ACTIVITY_OUT_PARAMETERS;
    }

    public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model) {
      String source = xtr.getAttributeValue(null, ATTRIBUTE_IOPARAMETER_SOURCE);
      String sourceExpression = xtr.getAttributeValue(null, ATTRIBUTE_IOPARAMETER_SOURCE_EXPRESSION);
      String target = xtr.getAttributeValue(null, ATTRIBUTE_IOPARAMETER_TARGET);
      if((!String.IsNullOrWhiteSpace(source) || !String.IsNullOrWhiteSpace(sourceExpression)) && !String.IsNullOrWhiteSpace(target)) {
        
        IOParameter parameter = new IOParameter();
        if(!String.IsNullOrWhiteSpace(sourceExpression)) {
          parameter.setSourceExpression(sourceExpression);
        } else {
          parameter.setSource(source);
        }
        
        parameter.setTarget(target);
        
        ((CallActivity) parentElement).getOutParameters().Add(parameter);
      }
    }
  }
}
