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

    public class Lane : BaseElement
    {

        protected String name;
        protected Process parentProcess;
        protected List<String> flowReferences = new List<String>();

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        //@JsonBackReference
        public Process getParentProcess()
        {
            return parentProcess;
        }

        public void setParentProcess(Process parentProcess)
        {
            this.parentProcess = parentProcess;
        }

        public List<String> getFlowReferences()
        {
            return flowReferences;
        }

        public void setFlowReferences(List<String> flowReferences)
        {
            this.flowReferences = flowReferences;
        }

        public override Object clone()
        {
            Lane clone = new Lane();
            clone.setValues(this);
            return clone;
        }

        public void setValues(Lane otherElement)
        {
            base.setValues(otherElement);
            setName(otherElement.getName());
            setParentProcess(otherElement.getParentProcess());

            flowReferences = new List<String>();
            if (otherElement.getFlowReferences() != null && otherElement.getFlowReferences().Any())
            {
                flowReferences.AddRange(otherElement.getFlowReferences());
            }
        }
    }
}