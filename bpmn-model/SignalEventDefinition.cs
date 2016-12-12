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
 */

    public class SignalEventDefinition : EventDefinition
    {

        protected String signalRef;
        protected Boolean async;

        public String getSignalRef()
        {
            return signalRef;
        }

        public void setSignalRef(String signalRef)
        {
            this.signalRef = signalRef;
        }

        public Boolean isAsync()
        {
            return async;
        }

        public void setAsync(Boolean async)
        {
            this.async = async;
        }

        public override Object clone()
        {
            SignalEventDefinition clone = new SignalEventDefinition();
            clone.setValues(this);
            return clone;
        }

        public void setValues(SignalEventDefinition otherDefinition)
        {
            base.setValues(otherDefinition);
            setSignalRef(otherDefinition.getSignalRef());
            setAsync(otherDefinition.isAsync());
        }
    }
}