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
using Common.Logging;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.parser
{


    public class ParticipantParser : BpmnXMLConstants
    {

        protected static ILog LOGGER = LogManager.GetLogger(typeof (ParticipantParser));

        public void parse(XMLStreamReader xtr, BpmnModel model)
        {

            if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_ID)))
            {
                Pool pool = new Pool();
                pool.setId(xtr.getAttributeValue(null, ATTRIBUTE_ID));
                pool.setName(xtr.getAttributeValue(null, ATTRIBUTE_NAME));
                pool.setProcessRef(xtr.getAttributeValue(null, ATTRIBUTE_PROCESS_REF));
                BpmnXMLUtil.parseChildElements(ELEMENT_PARTICIPANT, pool, xtr, model);
                model.getPools().Add(pool);
            }
        }
    }
}