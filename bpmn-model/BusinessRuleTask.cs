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

namespace org.activiti.bpmn.model
{




/**
 * //@author Tijs Rademakers
 */

    public class BusinessRuleTask : TaskActivity
    {

        protected String resultVariableName;
        protected Boolean exclude;
        protected List<String> ruleNames = new List<String>();
        protected List<String> inputVariables = new List<String>();
        protected String className;

        public Boolean isExclude()
        {
            return exclude;
        }

        public void setExclude(Boolean exclude)
        {
            this.exclude = exclude;
        }

        public String getResultVariableName()
        {
            return resultVariableName;
        }

        public void setResultVariableName(String resultVariableName)
        {
            this.resultVariableName = resultVariableName;
        }

        public List<String> getRuleNames()
        {
            return ruleNames;
        }

        public void setRuleNames(List<String> ruleNames)
        {
            this.ruleNames = ruleNames;
        }

        public List<String> getInputVariables()
        {
            return inputVariables;
        }

        public void setInputVariables(List<String> inputVariables)
        {
            this.inputVariables = inputVariables;
        }

        public String getClassName()
        {
            return className;
        }

        public void setClassName(String className)
        {
            this.className = className;
        }

        public override Object clone()
        {
            BusinessRuleTask clone = new BusinessRuleTask();
            clone.setValues(this);
            return clone;
        }

        public void setValues(BusinessRuleTask otherElement)
        {
            base.setValues(otherElement);
            setResultVariableName(otherElement.getResultVariableName());
            setExclude(otherElement.isExclude());
            setClassName(otherElement.getClassName());
            ruleNames = new List<String>(otherElement.getRuleNames());
            inputVariables = new List<String>(otherElement.getInputVariables());
        }
    }
}