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

    public class FormProperty : BaseElement
    {

        protected String name;
        protected String expression;
        protected String variable;
        protected String type;
        protected String defaultExpression;
        protected String datePattern;
        protected Boolean readable = true;
        protected Boolean writeable = true;
        protected Boolean required;
        protected List<FormValue> formValues = new List<FormValue>();

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public String getExpression()
        {
            return expression;
        }

        public void setExpression(String expression)
        {
            this.expression = expression;
        }

        public String getVariable()
        {
            return variable;
        }

        public void setVariable(String variable)
        {
            this.variable = variable;
        }

        public String getType()
        {
            return type;
        }

        public String getDefaultExpression()
        {
            return defaultExpression;
        }

        public void setDefaultExpression(String defaultExpression)
        {
            this.defaultExpression = defaultExpression;
        }

        public void setType(String type)
        {
            this.type = type;
        }

        public String getDatePattern()
        {
            return datePattern;
        }

        public void setDatePattern(String datePattern)
        {
            this.datePattern = datePattern;
        }

        public Boolean isReadable()
        {
            return readable;
        }

        public void setReadable(Boolean readable)
        {
            this.readable = readable;
        }

        public Boolean isWriteable()
        {
            return writeable;
        }

        public void setWriteable(Boolean writeable)
        {
            this.writeable = writeable;
        }

        public Boolean isRequired()
        {
            return required;
        }

        public void setRequired(Boolean required)
        {
            this.required = required;
        }

        public List<FormValue> getFormValues()
        {
            return formValues;
        }

        public void setFormValues(List<FormValue> formValues)
        {
            this.formValues = formValues;
        }

        public override Object clone()
        {
            FormProperty clone = new FormProperty();
            clone.setValues(this);
            return clone;
        }

        public void setValues(FormProperty otherProperty)
        {
            base.setValues(otherProperty);
            setName(otherProperty.getName());
            setExpression(otherProperty.getExpression());
            setVariable(otherProperty.getVariable());
            setType(otherProperty.getType());
            setDefaultExpression(otherProperty.getDefaultExpression());
            setDatePattern(otherProperty.getDatePattern());
            setReadable(otherProperty.isReadable());
            setWriteable(otherProperty.isWriteable());
            setRequired(otherProperty.isRequired());

            formValues = new List<FormValue>();
            if (otherProperty.getFormValues() != null && otherProperty.getFormValues().Any())
            {
                foreach (FormValue formValue in otherProperty.getFormValues())
                {
                    formValues.Add((FormValue) formValue.clone());
                }
            }
        }
    }
}