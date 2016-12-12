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
using System.Collections.ObjectModel;
using System.Linq;

namespace org.activiti.bpmn.model
{





/**
 * ////@author Tijs Rademakers
 */

    public class Process : BaseElement , FlowElementsContainer, HasExecutionListeners
    {

        protected String name;
        protected Boolean executable = true;
        protected String documentation;
        protected IOSpecification ioSpecification;
        protected List<ActivitiListener> executionListeners = new List<ActivitiListener>();
        protected List<Lane> lanes = new List<Lane>();
        protected List<FlowElement> flowElementList = new List<FlowElement>();
        protected List<ValuedDataObject> dataObjects = new List<ValuedDataObject>();
        protected List<Artifact> artifactList = new List<Artifact>();
        protected List<String> candidateStarterUsers = new List<String>();
        protected List<String> candidateStarterGroups = new List<String>();
        protected List<EventListener> eventListeners = new List<EventListener>();

        public Process()
        {

        }

        public String getDocumentation()
        {
            return documentation;
        }

        public void setDocumentation(String documentation)
        {
            this.documentation = documentation;
        }

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public Boolean isExecutable()
        {
            return executable;
        }

        public void setExecutable(Boolean executable)
        {
            this.executable = executable;
        }

        public IOSpecification getIoSpecification()
        {
            return ioSpecification;
        }

        public void setIoSpecification(IOSpecification ioSpecification)
        {
            this.ioSpecification = ioSpecification;
        }

        public List<ActivitiListener> getExecutionListeners()
        {
            return executionListeners;
        }

        public void setExecutionListeners(List<ActivitiListener> executionListeners)
        {
            this.executionListeners = executionListeners;
        }

        public List<Lane> getLanes()
        {
            return lanes;
        }

        public void setLanes(List<Lane> lanes)
        {
            this.lanes = lanes;
        }

        public FlowElement getFlowElement(String flowElementId)
        {
            return findFlowElementInList(flowElementId);
        }

        /**
   * Searches the whole process, including subprocesses (unlike {////@link getFlowElements(String)}
   */

        public FlowElement getFlowElementRecursive(String flowElementId)
        {
            return getFlowElementRecursive(this, flowElementId);
        }

        protected FlowElement getFlowElementRecursive(FlowElementsContainer flowElementsContainer, String flowElementId)
        {
            foreach (FlowElement flowElement in flowElementsContainer.getFlowElements())
            {
                if (flowElement.getId() != null && flowElement.getId().Equals(flowElementId))
                {
                    return flowElement;
                }
                else if (flowElement
                as FlowElementsContainer != null)
                {
                    FlowElement result = getFlowElementRecursive((FlowElementsContainer) flowElement, flowElementId);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        /**
   * Searches the whole process, including subprocesses
   */

        public FlowElementsContainer getFlowElementsContainerRecursive(String flowElementId)
        {
            return getFlowElementsContainerRecursive(this, flowElementId);
        }

        protected FlowElementsContainer getFlowElementsContainerRecursive(FlowElementsContainer flowElementsContainer,
            String flowElementId)
        {
            foreach (FlowElement flowElement in flowElementsContainer.getFlowElements())
            {
                if (flowElement.getId() != null && flowElement.getId().Equals(flowElementId))
                {
                    return flowElementsContainer;
                }
                else if (flowElement as FlowElementsContainer != null)
                {
                    FlowElementsContainer result = getFlowElementsContainerRecursive(
                        (FlowElementsContainer) flowElement, flowElementId);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        protected FlowElement findFlowElementInList(String flowElementId)
        {
            foreach (FlowElement f in flowElementList)
            {
                if (f.getId() != null && f.getId().Equals(flowElementId))
                {
                    return f;
                }
            }
            return null;
        }

        public IEnumerable<FlowElement> getFlowElements()
        {
            return flowElementList;
        }

        public void addFlowElement(FlowElement element)
        {
            flowElementList.Add(element);
        }

        public void removeFlowElement(String elementId)
        {
            FlowElement element = findFlowElementInList(elementId);
            if (element != null)
            {
                flowElementList.Remove(element);
            }
        }

        public Artifact getArtifact(String id)
        {
            Artifact foundArtifact = null;
            foreach (Artifact artifact in artifactList)
            {
                if (id.Equals(artifact.getId()))
                {
                    foundArtifact = artifact;
                    break;
                }
            }
            return foundArtifact;
        }

        public IEnumerable<Artifact> getArtifacts()
        {
            return artifactList;
        }

        public void addArtifact(Artifact artifact)
        {
            artifactList.Add(artifact);
        }

        public void removeArtifact(String artifactId)
        {
            Artifact artifact = getArtifact(artifactId);
            if (artifact != null)
            {
                artifactList.Remove(artifact);
            }
        }

        public List<String> getCandidateStarterUsers()
        {
            return candidateStarterUsers;
        }

        public void setCandidateStarterUsers(List<String> candidateStarterUsers)
        {
            this.candidateStarterUsers = candidateStarterUsers;
        }

        public List<String> getCandidateStarterGroups()
        {
            return candidateStarterGroups;
        }

        public void setCandidateStarterGroups(List<String> candidateStarterGroups)
        {
            this.candidateStarterGroups = candidateStarterGroups;
        }

        public List<EventListener> getEventListeners()
        {
            return eventListeners;
        }

        public void setEventListeners(List<EventListener> eventListeners)
        {
            this.eventListeners = eventListeners;
        }


        public List<FlowElement> findFlowElementsOfType(Type type)
        {
            return findFlowElementsOfType(type, true);
        }

        ////@SuppressWarnings("unchecked")
        public List<FlowElement> findFlowElementsOfType(Type type, Boolean goIntoSubprocesses)
        {
            List<FlowElement> foundFlowElements = new List<FlowElement>();
            foreach (FlowElement flowElement in this.getFlowElements())
            {
                if (type== flowElement.GetType())
                {
                    foundFlowElements.Add((FlowElement) flowElement);
                }
                if (flowElement as SubProcess != null)
                {
                    if (goIntoSubprocesses)
                    {
                        foundFlowElements.AddRange(findFlowElementsInSubProcessOfType((SubProcess) flowElement, type));
                    }
                }
            }
            return foundFlowElements;
        }

        public List<FlowElement> findFlowElementsInSubProcessOfType(SubProcess subProcess, Type type)
        {
            return findFlowElementsInSubProcessOfType(subProcess, type, true);
        }

        ////@SuppressWarnings("unchecked")
        public List<FlowElement> findFlowElementsInSubProcessOfType(SubProcess subProcess, Type type,
            Boolean goIntoSubprocesses)
        {
            List<FlowElement> foundFlowElements = new List<FlowElement>();
            foreach (FlowElement flowElement in subProcess.getFlowElements())
            {
                if (type == flowElement.GetType())
                {
                    foundFlowElements.Add((FlowElement) flowElement);
                }
                if (flowElement as SubProcess != null)
                {
                    if (goIntoSubprocesses)
                    {
                        foundFlowElements.AddRange(findFlowElementsInSubProcessOfType((SubProcess) flowElement, type));
                    }
                }
            }
            return foundFlowElements;
        }

        public FlowElementsContainer findParent(FlowElement childElement)
        {
            return findParent(childElement, this);
        }

        public FlowElementsContainer findParent(FlowElement childElement, FlowElementsContainer flowElementsContainer)
        {
            foreach (FlowElement flowElement in flowElementsContainer.getFlowElements())
            {
                if (childElement.getId() != null && childElement.getId().Equals(flowElement.getId()))
                {
                    return flowElementsContainer;
                }
                if (flowElement as FlowElementsContainer != null)
                {
                    FlowElementsContainer result = findParent(childElement, (FlowElementsContainer) flowElement);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        public override object clone()
        {
            Process clone = new Process();
            clone.setValues(this);
            return clone;
        }

        public void setValues(Process otherElement)
        {
            base.setValues(otherElement);

            setName(otherElement.getName());
            setExecutable(otherElement.isExecutable());
            setDocumentation(otherElement.getDocumentation());
            if (otherElement.getIoSpecification() != null)
            {
                setIoSpecification((IOSpecification)otherElement.getIoSpecification().clone());
            }

            executionListeners = new List<ActivitiListener>();
            if (otherElement.getExecutionListeners() != null && otherElement.getExecutionListeners().Any())
            {
                foreach (ActivitiListener listener in otherElement.getExecutionListeners())
                {
                    executionListeners.Add((ActivitiListener)listener.clone());
                }
            }

            candidateStarterUsers = new List<String>();
            if (otherElement.getCandidateStarterUsers() != null && otherElement.getCandidateStarterUsers().Any())
            {
                candidateStarterUsers.AddRange(otherElement.getCandidateStarterUsers());
            }

            candidateStarterGroups = new List<String>();
            if (otherElement.getCandidateStarterGroups() != null && otherElement.getCandidateStarterGroups().Any())
            {
                candidateStarterGroups.AddRange(otherElement.getCandidateStarterGroups());
            }

            eventListeners = new List<EventListener>();
            if (otherElement.getEventListeners() != null && otherElement.getEventListeners().Any())
            {
                foreach (EventListener listener in otherElement.getEventListeners())
                {
                    eventListeners.Add((EventListener)listener.clone());
                }
            }

            /*
     * This is required because data objects in Designer have no DI info
     * and are added as properties, not flow elements
     *
     * Determine the differences between the 2 elements' data object
     */
            foreach (ValuedDataObject thisObject in getDataObjects())
            {
                Boolean exists = false;
                foreach (ValuedDataObject otherObject in otherElement.getDataObjects())
                {
                    if (thisObject.getId().Equals(otherObject.getId()))
                    {
                        exists = true;
                    }
                }
                if (!exists)
                {
                    // missing object
                    removeFlowElement(thisObject.getId());
                }
            }

            dataObjects = new List<ValuedDataObject>();
            if (otherElement.getDataObjects() != null && otherElement.getDataObjects().Any())
            {
                foreach (ValuedDataObject dataObject in otherElement.getDataObjects())
                {
                    ValuedDataObject clone = (ValuedDataObject)dataObject.clone();
                    dataObjects.Add(clone);
                    // Add it to the list of FlowElements
                    // if it is already there, remove it first so order is same as data object list
                    removeFlowElement(clone.getId());
                    addFlowElement(clone);
                }
            }
        }

        public List<ValuedDataObject> getDataObjects()
        {
            return dataObjects;
        }

        public void setDataObjects(List<ValuedDataObject> dataObjects)
        {
            this.dataObjects = dataObjects;
        }
    }
}