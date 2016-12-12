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

namespace org.activiti.bpmn.model
{


/**
 * //@author Tijs Rademakers
 * //@author Joram Barrez
 */

    public class ScriptTask : TaskActivity
    {

        protected String scriptFormat;
        protected String script;
        protected String resultVariable;
        protected Boolean autoStoreVariables = false; // see http://jira.codehaus.org/browse/ACT-1626

        public String getScriptFormat()
        {
            return scriptFormat;
        }

        public void setScriptFormat(String scriptFormat)
        {
            this.scriptFormat = scriptFormat;
        }

        public String getScript()
        {
            return script;
        }

        public void setScript(String script)
        {
            this.script = script;
        }

        public String getResultVariable()
        {
            return resultVariable;
        }

        public void setResultVariable(String resultVariable)
        {
            this.resultVariable = resultVariable;
        }

        public Boolean isAutoStoreVariables()
        {
            return autoStoreVariables;
        }

        public void setAutoStoreVariables(Boolean autoStoreVariables)
        {
            this.autoStoreVariables = autoStoreVariables;
        }

        public override Object clone()
        {
            ScriptTask clone = new ScriptTask();
            clone.setValues(this);
            return clone;
        }

        public void setValues(ScriptTask otherElement)
        {
            base.setValues(otherElement);
            setScriptFormat(otherElement.getScriptFormat());
            setScript(otherElement.getScript());
            setResultVariable(otherElement.getResultVariable());
            setAutoStoreVariables(otherElement.isAutoStoreVariables());
        }
    }
}