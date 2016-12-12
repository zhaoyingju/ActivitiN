/* Licensed under the Apache License, Version 2.0 (the "License");ILog LOGGER = LogManager.GetLogger
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

namespace org.activiti.bpmn.converter.child
{

    public class DataAssociationParser : BpmnXMLConstants
    {

        protected static ILog LOGGER = LogManager.GetLogger(typeof (DataAssociationParser));

        public static void parseDataAssociation(DataAssociation dataAssociation, String elementName, XMLStreamReader xtr)
        {
            bool readyWithDataAssociation = false;
            Assignment assignment = null;
            try
            {

                dataAssociation.setId(xtr.getAttributeValue(null, "id"));

                while (readyWithDataAssociation == false && xtr.hasNext())
                {
                    xtr.next();
                    if (xtr.isStartElement() && ELEMENT_SOURCE_REF.Equals(xtr.getLocalName()))
                    {
                        String sourceRef = xtr.getElementText();
                        if (!String.IsNullOrWhiteSpace(sourceRef))
                        {
                            dataAssociation.setSourceRef(sourceRef.Trim());
                        }

                    }
                    else if (xtr.isStartElement() && ELEMENT_TARGET_REF.Equals(xtr.getLocalName()))
                    {
                        String targetRef = xtr.getElementText();
                        if (!String.IsNullOrWhiteSpace(targetRef))
                        {
                            dataAssociation.setTargetRef(targetRef.Trim());
                        }

                    }
                    else if (xtr.isStartElement() && ELEMENT_TRANSFORMATION.Equals(xtr.getLocalName()))
                    {
                        String transformation = xtr.getElementText();
                        if (!String.IsNullOrWhiteSpace(transformation))
                        {
                            dataAssociation.setTransformation(transformation.Trim());
                        }

                    }
                    else if (xtr.isStartElement() && ELEMENT_ASSIGNMENT.Equals(xtr.getLocalName()))
                    {
                        assignment = new Assignment();
                        BpmnXMLUtil.addXMLLocation(assignment, xtr);

                    }
                    else if (xtr.isStartElement() && ELEMENT_FROM.Equals(xtr.getLocalName()))
                    {
                        String from = xtr.getElementText();
                        if (assignment != null && !String.IsNullOrWhiteSpace(from))
                        {
                            assignment.setFrom(from.Trim());
                        }

                    }
                    else if (xtr.isStartElement() && ELEMENT_TO.Equals(xtr.getLocalName()))
                    {
                        String to = xtr.getElementText();
                        if (assignment != null && !String.IsNullOrWhiteSpace(to))
                        {
                            assignment.setTo(to.Trim());
                        }

                    }
                    else if (xtr.isEndElement() && ELEMENT_ASSIGNMENT.Equals(xtr.getLocalName()))
                    {
                        if (!String.IsNullOrWhiteSpace(assignment.getFrom()) &&
                            !String.IsNullOrWhiteSpace(assignment.getTo()))
                        {
                            dataAssociation.getAssignments().Add(assignment);
                        }

                    }
                    else if (xtr.isEndElement() && elementName.Equals(xtr.getLocalName()))
                    {
                        readyWithDataAssociation = true;
                    }
                }
            }
            catch (Exception e)
            {
                LOGGER.Warn("Error parsing data association child elements", e);
            }
        }
    }
}