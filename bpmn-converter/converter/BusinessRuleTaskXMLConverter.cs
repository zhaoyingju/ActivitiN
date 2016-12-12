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
public class BusinessRuleTaskXMLConverter : BaseBpmnXMLConverter {
  
  public Type getBpmnElementType() {
    return typeof(BusinessRuleTask);
  }
  
  //@Override

  protected String getXMLElementName() {
    return ELEMENT_TASK_BUSINESSRULE;
  }
  
  //@Override

  protected BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model) {
    BusinessRuleTask businessRuleTask = new BusinessRuleTask();
    BpmnXMLUtil.addXMLLocation(businessRuleTask, xtr);
    businessRuleTask.setInputVariables(parseDelimitedList(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_RULE_VARIABLES_INPUT)));
    businessRuleTask.setRuleNames(parseDelimitedList(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_RULE_RULES)));
    businessRuleTask.setResultVariableName(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_RULE_RESULT_VARIABLE));
    businessRuleTask.setClassName(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_RULE_CLASS));
    String exclude = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_RULE_EXCLUDE);
    if (ATTRIBUTE_VALUE_TRUE.equalsIgnoreCase(exclude)) {
      businessRuleTask.setExclude(true);
    }
    parseChildElements(getXMLElementName(), businessRuleTask, model, xtr);
    return businessRuleTask;
  }

  //@Override

  protected void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    BusinessRuleTask businessRuleTask = (BusinessRuleTask) element;
    String inputVariables = convertToDelimitedString(businessRuleTask.getInputVariables());
    if (!String.IsNullOrWhiteSpace(inputVariables)) {
      writeQualifiedAttribute(ATTRIBUTE_TASK_RULE_VARIABLES_INPUT, inputVariables, xtw);
    }
    String ruleNames = convertToDelimitedString(businessRuleTask.getRuleNames());
    if (!String.IsNullOrWhiteSpace(ruleNames)) {
      writeQualifiedAttribute(ATTRIBUTE_TASK_RULE_RULES, ruleNames, xtw);
    }
    if (!String.IsNullOrWhiteSpace(businessRuleTask.getResultVariableName())) {
      writeQualifiedAttribute(ATTRIBUTE_TASK_RULE_RESULT_VARIABLE, businessRuleTask.getResultVariableName(), xtw);
    }
    if (!String.IsNullOrWhiteSpace(businessRuleTask.getClassName())) {
      writeQualifiedAttribute(ATTRIBUTE_TASK_RULE_CLASS, businessRuleTask.getClassName(), xtw);
    }
    if (businessRuleTask.isExclude()) {
      writeQualifiedAttribute(ATTRIBUTE_TASK_RULE_EXCLUDE, ATTRIBUTE_VALUE_TRUE, xtw);
    }
  }
  
  //@Override

  protected void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
  }
}
