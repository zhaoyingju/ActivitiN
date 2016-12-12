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

    public class SendTask : TaskActivity
    {

        protected String type;
        protected String implementationType;
        protected String operationRef;
        protected List<FieldExtension> fieldExtensions = new List<FieldExtension>();

        public String getType()
        {
            return type;
        }

        public void setType(String type)
        {
            this.type = type;
        }

        public String getImplementationType()
        {
            return implementationType;
        }

        public void setImplementationType(String implementationType)
        {
            this.implementationType = implementationType;
        }

        public String getOperationRef()
        {
            return operationRef;
        }

        public void setOperationRef(String operationRef)
        {
            this.operationRef = operationRef;
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
            SendTask clone = new SendTask();
            clone.setValues(this);
            return clone;
        }

        public void setValues(SendTask otherElement)
        {
            base.setValues(otherElement);
            setType(otherElement.getType());
            setImplementationType(otherElement.getImplementationType());
            setOperationRef(otherElement.getOperationRef());

            fieldExtensions = new List<FieldExtension>();
            if (otherElement.getFieldExtensions() != null && otherElement.getFieldExtensions().Any())
            {
                foreach (FieldExtension extension in otherElement.getFieldExtensions())
                {
                    fieldExtensions.Add((FieldExtension) extension.clone());
                }
            }
        }
    }
}