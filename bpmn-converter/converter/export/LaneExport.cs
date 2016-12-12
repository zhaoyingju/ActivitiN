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
using System.Linq;
using bpmn_converter.converter;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.export
{








    public class LaneExport : BpmnXMLConstants
    {

        public static void writeLanes(Process process, XMLStreamWriter xtw)
        {
            if (process.getLanes().Any())
            {
                xtw.writeStartElement(ELEMENT_LANESET);
                xtw.writeAttribute(ATTRIBUTE_ID, "laneSet_" + process.getId());
                foreach (Lane lane  in process.getLanes())
                {
                    xtw.writeStartElement(ELEMENT_LANE);
                    xtw.writeAttribute(ATTRIBUTE_ID, lane.getId());
                    if (!String.IsNullOrWhiteSpace(lane.getName()))
                    {
                        xtw.writeAttribute(ATTRIBUTE_NAME, lane.getName());
                    }

                    foreach (String flowNodeRef  in lane.getFlowReferences())
                    {
                        xtw.writeStartElement(ELEMENT_FLOWNODE_REF);
                        xtw.writeCharacters(flowNodeRef);
                        xtw.writeEndElement();
                    }

                    xtw.writeEndElement();
                }
                xtw.writeEndElement();
            }
        }
    }
}