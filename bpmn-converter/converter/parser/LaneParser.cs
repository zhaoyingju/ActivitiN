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

using bpmn_converter.converter.util;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.parser
{









/**
 * //@author Tijs Rademakers

 */

    public class LaneParser : BpmnXMLConstants
    {

        public void parse(XMLStreamReader xtr, Process activeProcess, BpmnModel model)
        { Lane  lane  = new Lane  (); 
            BpmnXMLUtil.addXMLLocation( lane  , xtr  ); 
            lane.setId( xtr.getAttributeValue  (null, ATTRIBUTE_ID  )); 
            lane.setName( xtr.getAttributeValue  (null, ATTRIBUTE_NAME  )); 
            lane.setParentProcess  ( activeProcess  ); 
            activeProcess.getLanes().Add( lane  ); 
            BpmnXMLUtil.parseChildElements( ELEMENT_LANE  , lane  , xtr  , model  );
        }
    }
}