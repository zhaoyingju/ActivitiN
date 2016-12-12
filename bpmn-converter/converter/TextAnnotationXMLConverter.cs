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
public class TextAnnotationXMLConverter : BaseBpmnXMLConverter {
  
  protected Dictionary<String, BaseChildElementParser> childParserMap = new Dictionary<String, BaseChildElementParser>();
  
  public TextAnnotationXMLConverter() {
    TextAnnotationTextParser annotationTextParser = new TextAnnotationTextParser();
    childParserMap.Add(annotationTextParser.getElementName(), annotationTextParser);
  }
  
  public Type getBpmnElementType() {
    return typeof(TextAnnotation);
  }
  
  //@Override

  protected String getXMLElementName() {
    return ELEMENT_TEXT_ANNOTATION;
  }
  
  //@Override

  protected BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model) {
    TextAnnotation textAnnotation = new TextAnnotation();
    BpmnXMLUtil.addXMLLocation(textAnnotation, xtr);
    textAnnotation.setTextFormat(xtr.getAttributeValue(null, ATTRIBUTE_TEXTFORMAT));
    parseChildElements(getXMLElementName(), textAnnotation, childParserMap, model, xtr);
    return textAnnotation;
  }

  //@Override

  protected void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    TextAnnotation textAnnotation = (TextAnnotation) element;
    writeDefaultAttribute(ATTRIBUTE_TEXTFORMAT, textAnnotation.getTextFormat(), xtw);
  }
  
  //@Override

  protected void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    TextAnnotation textAnnotation = (TextAnnotation) element;
    if (!String.IsNullOrWhiteSpace(textAnnotation.getText())) {
      xtw.writeStartElement(ELEMENT_TEXT_ANNOTATION_TEXT);
      xtw.writeCharacters(textAnnotation.getText());
      xtw.writeEndElement();
    }
  }
}
