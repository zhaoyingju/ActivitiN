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
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.child
{










/**
 * //@author Tijs Rademakers

 */

    public abstract class BaseChildElementParser : BpmnXMLConstants
    {

        protected static ILog LOGGER = LogManager.GetLogger(typeof(BaseChildElementParser));

        public abstract String getElementName();

        public abstract void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model);

        protected void parseChildElements(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model,
            BaseChildElementParser parser)
        {
            bool readyWithChildElements = false;
            while (readyWithChildElements == false && xtr.hasNext())
            {
                xtr.next();
                if (xtr.isStartElement())
                {
                    if (parser.getElementName().Equals(xtr.getLocalName()))
                    {
                        parser.parseChildElement(xtr, parentElement, model);
                    }

                }
                else if (xtr.isEndElement() && getElementName().equalsIgnoreCase(xtr.getLocalName()))
                {
                    readyWithChildElements = true;
                }
            }
        }
    }
}