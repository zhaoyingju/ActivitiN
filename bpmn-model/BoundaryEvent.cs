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

    public class BoundaryEvent : Event
    {

        //@JsonIgnore
        protected Activity attachedToRef;
        protected String attachedToRefId;
        protected Boolean cancelActivity = true;

        public Activity getAttachedToRef()
        {
            return attachedToRef;
        }

        public void setAttachedToRef(Activity attachedToRef)
        {
            this.attachedToRef = attachedToRef;
        }

        public String getAttachedToRefId()
        {
            return attachedToRefId;
        }

        public void setAttachedToRefId(String attachedToRefId)
        {
            this.attachedToRefId = attachedToRefId;
        }

        public Boolean isCancelActivity()
        {
            return cancelActivity;
        }

        public void setCancelActivity(Boolean cancelActivity)
        {
            this.cancelActivity = cancelActivity;
        }

        public override Object clone()
        {
            BoundaryEvent clone = new BoundaryEvent();
            clone.setValues(this);
            return clone;
        }

        public void setValues(BoundaryEvent otherEvent)
        {
            base.setValues(otherEvent);
            setAttachedToRefId(otherEvent.getAttachedToRefId());
            setAttachedToRef(otherEvent.getAttachedToRef());
            setCancelActivity(otherEvent.isCancelActivity());
        }
    }
}