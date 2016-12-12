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
using System.Linq;
using bpmn_converter.converter;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.export
{

    public class BPMNDIExport : BpmnXMLConstants
    {

        public static void writeBPMNDI(BpmnModel model, XMLStreamWriter xtw)
        {
            // BPMN DI information
            xtw.writeStartElement(BPMNDI_PREFIX, ELEMENT_DI_DIAGRAM, BPMNDI_NAMESPACE);

            String processId = null;
            if (model.getPools().Any())
            {
                processId = "Collaboration";
            }
            else
            {
                processId = model.getMainProcess().getId();
            }

            xtw.writeAttribute(ATTRIBUTE_ID, "BPMNDiagram_" + processId);

            xtw.writeStartElement(BPMNDI_PREFIX, ELEMENT_DI_PLANE, BPMNDI_NAMESPACE);
            xtw.writeAttribute(ATTRIBUTE_DI_BPMNELEMENT, processId);
            xtw.writeAttribute(ATTRIBUTE_ID, "BPMNPlane_" + processId);

            foreach (String elementId  in model.getLocationMap().Keys)
            {

                if (model.getFlowElement(elementId) != null || model.getArtifact(elementId) != null ||
                    model.getPool(elementId) != null || model.getLane(elementId) != null)
                {

                    xtw.writeStartElement(BPMNDI_PREFIX, ELEMENT_DI_SHAPE, BPMNDI_NAMESPACE);
                    xtw.writeAttribute(ATTRIBUTE_DI_BPMNELEMENT, elementId);
                    xtw.writeAttribute(ATTRIBUTE_ID, "BPMNShape_" + elementId);

                    GraphicInfo graphicInfo = model.getGraphicInfo(elementId);
                    FlowElement flowElement = model.getFlowElement(elementId);
                    if (flowElement as SubProcess != null && graphicInfo.getExpanded() != null)
                    {
                        xtw.writeAttribute(ATTRIBUTE_DI_IS_EXPANDED, graphicInfo.getExpanded().ToString());
                    }

                    xtw.writeStartElement(OMGDC_PREFIX, ELEMENT_DI_BOUNDS, OMGDC_NAMESPACE);
                    xtw.writeAttribute(ATTRIBUTE_DI_HEIGHT, "" + graphicInfo.getHeight());
                    xtw.writeAttribute(ATTRIBUTE_DI_WIDTH, "" + graphicInfo.getWidth());
                    xtw.writeAttribute(ATTRIBUTE_DI_X, "" + graphicInfo.getX());
                    xtw.writeAttribute(ATTRIBUTE_DI_Y, "" + graphicInfo.getY());
                    xtw.writeEndElement();

                    xtw.writeEndElement();
                }
            }

            foreach (String elementId  in model.getFlowLocationMap().Keys)
            {

                if (model.getFlowElement(elementId) != null || model.getArtifact(elementId) != null ||
                    model.getMessageFlow(elementId) != null)
                {

                    xtw.writeStartElement(BPMNDI_PREFIX, ELEMENT_DI_EDGE, BPMNDI_NAMESPACE);
                    xtw.writeAttribute(ATTRIBUTE_DI_BPMNELEMENT, elementId);
                    xtw.writeAttribute(ATTRIBUTE_ID, "BPMNEdge_" + elementId);

                    List<GraphicInfo> graphicInfoList = model.getFlowLocationGraphicInfo(elementId);
                    foreach (GraphicInfo graphicInfo  in graphicInfoList)
                    {
                        xtw.writeStartElement(OMGDI_PREFIX, ELEMENT_DI_WAYPOINT, OMGDI_NAMESPACE);
                        xtw.writeAttribute(ATTRIBUTE_DI_X, "" + graphicInfo.getX());
                        xtw.writeAttribute(ATTRIBUTE_DI_Y, "" + graphicInfo.getY());
                        xtw.writeEndElement();
                    }

                    GraphicInfo labelGraphicInfo = model.getLabelGraphicInfo(elementId);
                    FlowElement flowElement = model.getFlowElement(elementId);
                    MessageFlow messageFlow = null;
                    if (flowElement == null)
                    {
                        messageFlow = model.getMessageFlow(elementId);
                    }

                    bool hasName = false;
                    if (flowElement != null && !String.IsNullOrWhiteSpace(flowElement.getName()))
                    {
                        hasName = true;

                    }
                    else if (messageFlow != null && !String.IsNullOrWhiteSpace(messageFlow.getName()))
                    {
                        hasName = true;
                    }

                    if (labelGraphicInfo != null && hasName)
                    {
                        xtw.writeStartElement(BPMNDI_PREFIX, ELEMENT_DI_LABEL, BPMNDI_NAMESPACE);
                        xtw.writeStartElement(OMGDC_PREFIX, ELEMENT_DI_BOUNDS, OMGDC_NAMESPACE);
                        xtw.writeAttribute(ATTRIBUTE_DI_HEIGHT, "" + labelGraphicInfo.getHeight());
                        xtw.writeAttribute(ATTRIBUTE_DI_WIDTH, "" + labelGraphicInfo.getWidth());
                        xtw.writeAttribute(ATTRIBUTE_DI_X, "" + labelGraphicInfo.getX());
                        xtw.writeAttribute(ATTRIBUTE_DI_Y, "" + labelGraphicInfo.getY());
                        xtw.writeEndElement();
                        xtw.writeEndElement();
                    }

                    xtw.writeEndElement();
                }
            }

            // end BPMN DI elements
            xtw.writeEndElement();
            xtw.writeEndElement();
        }
    }
}