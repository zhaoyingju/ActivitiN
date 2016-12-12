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

    public abstract class Activity : FlowNode
    {

        protected Boolean asynchronous;
        protected Boolean notExclusive;
        protected String defaultFlow;
        protected Boolean forCompensation;
        protected MultiInstanceLoopCharacteristics loopCharacteristics;
        protected IOSpecification ioSpecification;
        protected List<DataAssociation> dataInputAssociations = new List<DataAssociation>();
        protected List<DataAssociation> dataOutputAssociations = new List<DataAssociation>();
        protected List<BoundaryEvent> boundaryEvents = new List<BoundaryEvent>();
        protected String failedJobRetryTimeCycleValue;

        public Boolean isAsynchronous()
        {
            return asynchronous;
        }

        public void setAsynchronous(Boolean asynchronous)
        {
            this.asynchronous = asynchronous;
        }

        public String getFailedJobRetryTimeCycleValue()
        {
            return failedJobRetryTimeCycleValue;
        }

        public void setFailedJobRetryTimeCycleValue(String failedJobRetryTimeCycleValue)
        {
            this.failedJobRetryTimeCycleValue = failedJobRetryTimeCycleValue;
        }

        public Boolean isNotExclusive()
        {
            return notExclusive;
        }

        public void setNotExclusive(Boolean notExclusive)
        {
            this.notExclusive = notExclusive;
        }

        public Boolean isForCompensation()
        {
            return forCompensation;
        }

        public void setForCompensation(Boolean forCompensation)
        {
            this.forCompensation = forCompensation;
        }

        public List<BoundaryEvent> getBoundaryEvents()
        {
            return boundaryEvents;
        }

        public void setBoundaryEvents(List<BoundaryEvent> boundaryEvents)
        {
            this.boundaryEvents = boundaryEvents;
        }

        public String getDefaultFlow()
        {
            return defaultFlow;
        }

        public void setDefaultFlow(String defaultFlow)
        {
            this.defaultFlow = defaultFlow;
        }

        public MultiInstanceLoopCharacteristics getLoopCharacteristics()
        {
            return loopCharacteristics;
        }

        public void setLoopCharacteristics(MultiInstanceLoopCharacteristics loopCharacteristics)
        {
            this.loopCharacteristics = loopCharacteristics;
        }

        public IOSpecification getIoSpecification()
        {
            return ioSpecification;
        }

        public void setIoSpecification(IOSpecification ioSpecification)
        {
            this.ioSpecification = ioSpecification;
        }

        public List<DataAssociation> getDataInputAssociations()
        {
            return dataInputAssociations;
        }

        public void setDataInputAssociations(List<DataAssociation> dataInputAssociations)
        {
            this.dataInputAssociations = dataInputAssociations;
        }

        public List<DataAssociation> getDataOutputAssociations()
        {
            return dataOutputAssociations;
        }

        public void setDataOutputAssociations(List<DataAssociation> dataOutputAssociations)
        {
            this.dataOutputAssociations = dataOutputAssociations;
        }

        public void setValues(Activity otherActivity)
        {
            base.setValues(otherActivity);
            setAsynchronous(otherActivity.isAsynchronous());
            setFailedJobRetryTimeCycleValue(otherActivity.getFailedJobRetryTimeCycleValue());
            setNotExclusive(otherActivity.isNotExclusive());
            setDefaultFlow(otherActivity.getDefaultFlow());
            setForCompensation(otherActivity.isForCompensation());
            if (otherActivity.getLoopCharacteristics() != null)
            {
                setLoopCharacteristics((MultiInstanceLoopCharacteristics) otherActivity.getLoopCharacteristics().clone());
            }
            if (otherActivity.getIoSpecification() != null)
            {
                setIoSpecification((IOSpecification) otherActivity.getIoSpecification().clone());
            }

            dataInputAssociations = new List<DataAssociation>();
            if (otherActivity.getDataInputAssociations() != null && otherActivity.getDataInputAssociations().Any())
            {
                foreach (DataAssociation association in otherActivity.getDataInputAssociations())
                {
                    dataInputAssociations.Add((DataAssociation) association.clone());
                }
            }

            dataOutputAssociations = new List<DataAssociation>();
            if (otherActivity.getDataOutputAssociations() != null && otherActivity.getDataOutputAssociations().Any())
            {
                foreach (DataAssociation association in otherActivity.getDataOutputAssociations())
                {
                    dataOutputAssociations.Add((DataAssociation) association.clone());
                }
            }

            boundaryEvents = new List<BoundaryEvent>();
            if (otherActivity.getBoundaryEvents() != null && otherActivity.getBoundaryEvents().Any())
            {
                foreach (BoundaryEvent Event in otherActivity.getBoundaryEvents())
                {
                    boundaryEvents.Add((BoundaryEvent) Event.clone());
                }
            }
        }
    }
}