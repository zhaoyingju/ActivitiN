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
public class ScriptTaskXMLConverter : BaseBpmnXMLConverter {
  
  protected Dictionary<String, BaseChildElementParser> childParserMap = new Dictionary<String, BaseChildElementParser>();
  
	public ScriptTaskXMLConverter() {
		ScriptTextParser scriptTextParser = new ScriptTextParser();
		childParserMap.Add(scriptTextParser.getElementName(), scriptTextParser);
	}
	
  public Type getBpmnElementType() {
    return typeof(ScriptTask);
  }
  
  //@Override

  protected String getXMLElementName() {
    return ELEMENT_TASK_SCRIPT;
  }
  
  //@Override

  protected BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model) {
    ScriptTask scriptTask = new ScriptTask();
    BpmnXMLUtil.addXMLLocation(scriptTask, xtr);
    scriptTask.setScriptFormat(xtr.getAttributeValue(null, ATTRIBUTE_TASK_SCRIPT_FORMAT));
    scriptTask.setResultVariable(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_SCRIPT_RESULTVARIABLE));
    if (StringUtils.isEmpty(scriptTask.getResultVariable())) {
      scriptTask.setResultVariable(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_SERVICE_RESULTVARIABLE));
    }
    String autoStoreVariables = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_SCRIPT_AUTO_STORE_VARIABLE);
    if (!String.IsNullOrWhiteSpace(autoStoreVariables)) {
      scriptTask.setAutoStoreVariables(bool.Parse(autoStoreVariables));
    }
    parseChildElements(getXMLElementName(), scriptTask, childParserMap, model, xtr);
    return scriptTask;
  }

  //@Override

  protected void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    ScriptTask scriptTask = (ScriptTask) element;
    writeDefaultAttribute(ATTRIBUTE_TASK_SCRIPT_FORMAT, scriptTask.getScriptFormat(), xtw);
    writeQualifiedAttribute(ATTRIBUTE_TASK_SCRIPT_RESULTVARIABLE, scriptTask.getResultVariable(), xtw);
    writeQualifiedAttribute(ATTRIBUTE_TASK_SCRIPT_AUTO_STORE_VARIABLE, String.Parse(scriptTask.isAutoStoreVariables()), xtw);
  }
  
  //@Override

  protected void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw) {
    ScriptTask scriptTask = (ScriptTask) element;
    if (!String.IsNullOrWhiteSpace(scriptTask.getScript())) {
      xtw.writeStartElement(ATTRIBUTE_TASK_SCRIPT_TEXT);
      xtw.writeCharacters(scriptTask.getScript());
      xtw.writeEndElement();
    }
  }
}
