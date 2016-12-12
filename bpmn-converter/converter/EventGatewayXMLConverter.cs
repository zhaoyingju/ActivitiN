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
public class EventGatewayXMLConverter : BaseBpmnXMLConverter {
  
  public Type getBpmnElementType() {
    return typeof(EventGateway);
  }
  
  //@Override

  protected String getXMLElementName() {
    return ELEMENT_GATEWAY_EVENT;
  }
  
  //@Override

  protected BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model) {
    EventGateway gateway = new EventGateway();
    BpmnXMLUtil.addXMLLocation(gateway, xtr);
    parseChildElements(getXMLElementName(), gateway, model, xtr);
    return gateway;
  }

  //@Override

  protected void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
  }
  
  //@Override

  protected void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    
  }
}
