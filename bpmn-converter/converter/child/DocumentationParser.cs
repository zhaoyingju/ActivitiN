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
using bpmn_converter.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.child
{









/**
 * //@author Tijs Rademakers

 */

    public class DocumentationParser : BaseChildElementParser
    {

        public override String getElementName()
        {
            return "documentation";
        }

        public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model)
        {
            String docText = xtr.getElementText();
            if (!String.IsNullOrWhiteSpace(docText))
            {
                if (parentElement as FlowElement != null)
                {
                    ((FlowElement) parentElement).setDocumentation(docText.Trim());
                }
                else if (parentElement as Process != null)
                {
                    ((Process) parentElement).setDocumentation(docText.Trim());
                }
            }
        }
    }
}