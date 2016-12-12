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

namespace org.activiti.bpmn.model
{




/**
 * //@author Tijs Rademakers
 */

    public class ActivitiListener : BaseElement
    {

        protected String Event;
        protected String implementationType;
        protected String implementation;
        protected List<FieldExtension> fieldExtensions = new List<FieldExtension>();

        public String getEvent()
        {
            return Event;
        }

        public void setEvent(String Event)
        {
            this.Event = Event;
        }

        public String getImplementationType()
        {
            return implementationType;
        }

        public void setImplementationType(String implementationType)
        {
            this.implementationType = implementationType;
        }

        public String getImplementation()
        {
            return implementation;
        }

        public void setImplementation(String implementation)
        {
            this.implementation = implementation;
        }

        public List<FieldExtension> getFieldExtensions()
        {
            return fieldExtensions;
        }

        public void setFieldExtensions(List<FieldExtension> fieldExtensions)
        {
            this.fieldExtensions = fieldExtensions;
        }

        public override Object clone()
        {
            ActivitiListener clone = new ActivitiListener();
            clone.setValues(this);
            return clone;
        }

        public void setValues(ActivitiListener otherListener)
        {
            setEvent(otherListener.getEvent());
            setImplementation(otherListener.getImplementation());
            setImplementationType(otherListener.getImplementationType());

            fieldExtensions = new List<FieldExtension>();
            if (otherListener.getFieldExtensions() != null && otherListener.getFieldExtensions().Any())
            {
                foreach (FieldExtension extension in otherListener.getFieldExtensions())
                {
                    fieldExtensions.Add((FieldExtension) extension.clone());
                }
            }
        }
    }
}