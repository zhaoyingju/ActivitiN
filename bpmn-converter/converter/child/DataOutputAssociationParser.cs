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
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.child
{

    public class DataOutputAssociationParser : BaseChildElementParser
    {

        public override String getElementName()
        {
            return ELEMENT_OUTPUT_ASSOCIATION;
        }

        public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model)
        {

            if (parentElement as Activity == null) return;

            DataAssociation dataAssociation = new DataAssociation();
            BpmnXMLUtil.addXMLLocation(dataAssociation, xtr);
            DataAssociationParser.parseDataAssociation(dataAssociation, getElementName(), xtr);

            ((Activity) parentElement).getDataOutputAssociations().Add(dataAssociation);
        }
    }
}