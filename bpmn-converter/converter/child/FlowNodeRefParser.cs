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


    public class FlowNodeRefParser : BaseChildElementParser
    {

        public override String getElementName()
        {
            return ELEMENT_FLOWNODE_REF;
        }

        public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model)
        {
            if (parentElement as Lane == null) return;

            Lane lane = (Lane) parentElement;
            lane.getFlowReferences().Add(xtr.getElementText());
        }
    }
}