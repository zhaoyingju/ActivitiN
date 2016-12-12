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
using bpmn_converter.converter.util;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.parser
{












/**
 * //@author Tijs Rademakers

 */

    public class BpmnEdgeParser : BpmnXMLConstants
    {

        public void parse(XMLStreamReader xtr, BpmnModel model)
        {

            String id = xtr.getAttributeValue(null, ATTRIBUTE_DI_BPMNELEMENT);
            List<GraphicInfo> wayPointList = new List<GraphicInfo>();
            while (xtr.hasNext())
            {
                xtr.next();
                if (xtr.isStartElement() && ELEMENT_DI_LABEL.equalsIgnoreCase(xtr.getLocalName()))
                {
                    while (xtr.hasNext())
                    {
                        xtr.next();
                        if (xtr.isStartElement() && ELEMENT_DI_BOUNDS.equalsIgnoreCase(xtr.getLocalName()))
                        {
                            GraphicInfo graphicInfo = new GraphicInfo();
                            BpmnXMLUtil.addXMLLocation(graphicInfo, xtr);
                            graphicInfo.setX(Double.Parse(xtr.getAttributeValue(null, ATTRIBUTE_DI_X)));
                            graphicInfo.setY(Double.Parse(xtr.getAttributeValue(null, ATTRIBUTE_DI_Y)));
                            graphicInfo.setWidth(Double.Parse(xtr.getAttributeValue(null, ATTRIBUTE_DI_WIDTH)));
                            graphicInfo.setHeight(Double.Parse(xtr.getAttributeValue(null, ATTRIBUTE_DI_HEIGHT)));
                            model.addLabelGraphicInfo(id, graphicInfo);
                            break;
                        }
                        else if (xtr.isEndElement() && ELEMENT_DI_LABEL.equalsIgnoreCase(xtr.getLocalName()))
                        {
                            break;
                        }
                    }

                }
                else if (xtr.isStartElement() && ELEMENT_DI_WAYPOINT.equalsIgnoreCase(xtr.getLocalName()))
                {
                    GraphicInfo graphicInfo = new GraphicInfo();
                    BpmnXMLUtil.addXMLLocation(graphicInfo, xtr);
                    graphicInfo.setX(Double.Parse(xtr.getAttributeValue(null, ATTRIBUTE_DI_X)));
                    graphicInfo.setY(Double.Parse(xtr.getAttributeValue(null, ATTRIBUTE_DI_Y)));
                    wayPointList.Add(graphicInfo);

                }
                else if (xtr.isEndElement() && ELEMENT_DI_EDGE.equalsIgnoreCase(xtr.getLocalName()))
                {
                    break;
                }
            }
            model.addFlowGraphicInfoList(id, wayPointList);
        }

        public BaseElement parseElement()
        {
            return null;
        }
    }
}