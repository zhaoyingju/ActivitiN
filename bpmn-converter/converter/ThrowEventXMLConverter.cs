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
public class ThrowEventXMLConverter : BaseBpmnXMLConverter {
  
  public Type getBpmnElementType() {
    return typeof(ThrowEvent);
  }
  
  //@Override

  protected String getXMLElementName() {
    return ELEMENT_EVENT_THROW;
  }
  
  //@Override

  protected BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model) {
    ThrowEvent throwEvent = new ThrowEvent();
    BpmnXMLUtil.addXMLLocation(throwEvent, xtr);
    parseChildElements(getXMLElementName(), throwEvent, model, xtr);
    return throwEvent;
  }

  //@Override

  protected void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
  }
 
  //@Override

  protected void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    ThrowEvent throwEvent = (ThrowEvent) element;
    writeEventDefinitions(throwEvent, throwEvent.getEventDefinitions(), model, xtw);
  }
}
