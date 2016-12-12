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

using System.Collections.Generic;
using System.Linq;

namespace org.activiti.bpmn.model
{




/**
 * //@author Tijs Rademakers
 */

    public abstract class Event : FlowNode
    {

        protected List<EventDefinition> eventDefinitions = new List<EventDefinition>();

        public List<EventDefinition> getEventDefinitions()
        {
            return eventDefinitions;
        }

        public void setEventDefinitions(List<EventDefinition> eventDefinitions)
        {
            this.eventDefinitions = eventDefinitions;
        }

        public void addEventDefinition(EventDefinition eventDefinition)
        {
            eventDefinitions.Add(eventDefinition);
        }

        public void setValues(Event otherEvent)
        {
            base.setValues(otherEvent);

            eventDefinitions = new List<EventDefinition>();
            if (otherEvent.getEventDefinitions() != null && otherEvent.getEventDefinitions().Any())
            {
                foreach (EventDefinition eventDef in otherEvent.getEventDefinitions())
                {
                    eventDefinitions.Add((EventDefinition)eventDef.clone());
                }
            }
        }
    }
}