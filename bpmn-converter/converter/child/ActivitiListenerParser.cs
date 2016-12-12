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










/**
 * //@author Tijs Rademakers

 */

    public abstract class ActivitiListenerParser : BaseChildElementParser
    {

        public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model)
        {

            ActivitiListener listener = new ActivitiListener();
            BpmnXMLUtil.addXMLLocation(listener, xtr);
            if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_LISTENER_CLASS)))
            {
                listener.setImplementation(xtr.getAttributeValue(null, ATTRIBUTE_LISTENER_CLASS));
                listener.setImplementationType(ImplementationType.IMPLEMENTATION_TYPE_CLASS);
            }
            else if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_LISTENER_EXPRESSION)))
            {
                listener.setImplementation(xtr.getAttributeValue(null, ATTRIBUTE_LISTENER_EXPRESSION));
                listener.setImplementationType(ImplementationType.IMPLEMENTATION_TYPE_EXPRESSION);
            }
            else if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_LISTENER_DELEGATEEXPRESSION)))
            {
                listener.setImplementation(xtr.getAttributeValue(null, ATTRIBUTE_LISTENER_DELEGATEEXPRESSION));
                listener.setImplementationType(ImplementationType.IMPLEMENTATION_TYPE_DELEGATEEXPRESSION);
            }
            listener.setEvent(xtr.getAttributeValue(null, ATTRIBUTE_LISTENER_EVENT));
            addListenerToParent(listener, parentElement);
            parseChildElements(xtr, listener, model, new FieldExtensionParser());
        }

        public abstract void addListenerToParent(ActivitiListener listener, BaseElement parentElement);
    }
}