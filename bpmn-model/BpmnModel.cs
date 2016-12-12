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
 * //@author Tijs Rademakers
 */

    public class BpmnModel
    {

        protected Dictionary<String, List<ExtensionAttribute>> definitionsAttributes =
            new Dictionary<String, List<ExtensionAttribute>>();

        protected List<Process> processes = new List<Process>();
        protected Dictionary<String, GraphicInfo> locationMap = new Dictionary<String, GraphicInfo>();
        protected Dictionary<String, GraphicInfo> labelLocationMap = new Dictionary<String, GraphicInfo>();
        protected Dictionary<String, List<GraphicInfo>> flowLocationMap = new Dictionary<String, List<GraphicInfo>>();
        protected List<Signal> signals = new List<Signal>();
        protected Dictionary<String, MessageFlow> messageFlowMap = new Dictionary<String, MessageFlow>();
        protected Dictionary<String, Message> messageMap = new Dictionary<String, Message>();
        protected Dictionary<String, String> errorMap = new Dictionary<String, String>();
        protected Dictionary<String, ItemDefinition> itemDefinitionMap = new Dictionary<String, ItemDefinition>();
        protected Dictionary<String, DataStore> dataStoreMap = new Dictionary<String, DataStore>();
        protected List<Pool> pools = new List<Pool>();
        protected List<Import> imports = new List<Import>();
        protected List<Interface> interfaces = new List<Interface>();
        protected List<Artifact> globalArtifacts = new List<Artifact>();
        protected Dictionary<String, String> NamespaceMap = new Dictionary<String, String>();
        protected String targetNamespace;
        protected List<String> userTaskFormTypes;
        protected List<String> startEventFormTypes;
        protected int nextFlowIdCounter = 1;


        public Dictionary<String, List<ExtensionAttribute>> getDefinitionsAttributes()
        {
            return definitionsAttributes;
        }

        public String getDefinitionsAttributeValue(String Namespace, String name)
        {
            List<ExtensionAttribute> attributes = getDefinitionsAttributes()[name];
            if (attributes != null && attributes.Any())
            {
                foreach (ExtensionAttribute attribute in attributes)
                {
                    if (Namespace.Equals(attribute.getNamespace()))
                        return attribute.getValue();
                }
            }
            return null;
        }

        public void addDefinitionsAttribute(ExtensionAttribute attribute)
        {
            if (attribute != null && !String.IsNullOrWhiteSpace(attribute.getName()))
            {
                List<ExtensionAttribute> attributeList = null;
                if (this.definitionsAttributes.ContainsKey(attribute.getName()) == false)
                {
                    attributeList = new List<ExtensionAttribute>();
                    this.definitionsAttributes.Add(attribute.getName(), attributeList);
                }
                this.definitionsAttributes[attribute.getName()].Add(attribute);
            }
        }

        public void setDefinitionsAttributes(Dictionary<String, List<ExtensionAttribute>> attributes)
        {
            this.definitionsAttributes = attributes;
        }

        public Process getMainProcess()
        {
            if (getPools().Any())
            {
                return getProcess(getPools()[0].getId());
            }
            else
            {
                return getProcess(null);
            }
        }

        public Process getProcess(String poolRef)
        {
            foreach (Process process in processes)
            {
                Boolean foundPool = false;
                foreach (Pool pool in pools)
                {
                    if (!String.IsNullOrWhiteSpace(pool.getProcessRef()) &&
                        pool.getProcessRef().Equals(process.getId(), StringComparison.InvariantCultureIgnoreCase))
                    {

                        if (poolRef != null)
                        {
                            if (pool.getId().Equals(poolRef, StringComparison.InvariantCultureIgnoreCase))
                            {
                                foundPool = true;
                            }
                        }
                        else
                        {
                            foundPool = true;
                        }
                    }
                }

                if (poolRef == null && foundPool == false)
                {
                    return process;
                }
                else if (poolRef != null && foundPool == true)
                {
                    return process;
                }
            }

            return null;
        }

        public List<Process> getProcesses()
        {
            return processes;
        }

        public void addProcess(Process process)
        {
            processes.Add(process);
        }

        public Pool getPool(String id)
        {
            Pool foundPool = null;
            if (!String.IsNullOrWhiteSpace(id))
            {
                foreach (Pool pool in pools)
                {
                    if (id.Equals(pool.getId()))
                    {
                        foundPool = pool;
                        break;
                    }
                }
            }
            return foundPool;
        }

        public Lane getLane(String id)
        {
            Lane foundLane = null;
            if (!String.IsNullOrWhiteSpace(id))
            {
                foreach (Process process in processes)
                {
                    foreach (Lane lane in process.getLanes())
                    {
                        if (id.Equals(lane.getId()))
                        {
                            foundLane = lane;
                            break;
                        }
                    }
                    if (foundLane != null)
                    {
                        break;
                    }
                }
            }
            return foundLane;
        }

        public FlowElement getFlowElement(String id)
        {
            FlowElement foundFlowElement = null;
            foreach (Process process in processes)
            {
                foundFlowElement = process.getFlowElement(id);
                if (foundFlowElement != null)
                {
                    break;
                }
            }

            if (foundFlowElement == null)
            {
                foreach (Process process in processes)
                {
                    foreach (FlowElement flowElement in process.findFlowElementsOfType(typeof (SubProcess)))
                    {
                        foundFlowElement = getFlowElementInSubProcess(id, (SubProcess) flowElement);
                        if (foundFlowElement != null)
                        {
                            break;
                        }
                    }
                    if (foundFlowElement != null)
                    {
                        break;
                    }
                }
            }

            return foundFlowElement;
        }

        protected FlowElement getFlowElementInSubProcess(String id, SubProcess subProcess)
        {
            FlowElement foundFlowElement = subProcess.getFlowElement(id);
            if (foundFlowElement == null)
            {
                foreach (FlowElement flowElement in subProcess.getFlowElements())
                {
                    if (flowElement as SubProcess != null)
                    {
                        foundFlowElement = getFlowElementInSubProcess(id, (SubProcess) flowElement);
                        if (foundFlowElement != null)
                        {
                            break;
                        }
                    }
                }
            }
            return foundFlowElement;
        }

        public Artifact getArtifact(String id)
        {
            Artifact foundArtifact = null;
            foreach (Process process in processes)
            {
                foundArtifact = process.getArtifact(id);
                if (foundArtifact != null)
                {
                    break;
                }
            }

            if (foundArtifact == null)
            {
                foreach (Process process in processes)
                {
                    foreach (FlowElement flowElement in process.findFlowElementsOfType(typeof (SubProcess)))
                    {
                        foundArtifact = getArtifactInSubProcess(id, (SubProcess) flowElement);
                        if (foundArtifact != null)
                        {
                            break;
                        }
                    }
                    if (foundArtifact != null)
                    {
                        break;
                    }
                }
            }

            return foundArtifact;
        }

        protected Artifact getArtifactInSubProcess(String id, SubProcess subProcess)
        {
            Artifact foundArtifact = subProcess.getArtifact(id);
            if (foundArtifact == null)
            {
                foreach (FlowElement flowElement in subProcess.getFlowElements())
                {
                    if (flowElement
                    as SubProcess != null)
                    {
                        foundArtifact = getArtifactInSubProcess(id, (SubProcess) flowElement);
                        if (foundArtifact != null)
                        {
                            break;
                        }
                    }
                }
            }
            return foundArtifact;
        }

        public void addGraphicInfo(String key, GraphicInfo graphicInfo)
        {
            locationMap.Add(key, graphicInfo);
        }

        public GraphicInfo getGraphicInfo(String key)
        {
            return locationMap[key];
        }

        public void removeGraphicInfo(String key)
        {
            locationMap.Remove(key);
        }

        public List<GraphicInfo> getFlowLocationGraphicInfo(String key)
        {
            return flowLocationMap[key];
        }

        public void removeFlowGraphicInfoList(String key)
        {
            flowLocationMap.Remove(key);
        }

        public Dictionary<String, GraphicInfo> getLocationMap()
        {
            return locationMap;
        }

        public Dictionary<String, List<GraphicInfo>> getFlowLocationMap()
        {
            return flowLocationMap;
        }

        public GraphicInfo getLabelGraphicInfo(String key)
        {
            return labelLocationMap[key];
        }

        public void addLabelGraphicInfo(String key, GraphicInfo graphicInfo)
        {
            labelLocationMap.Add(key, graphicInfo);
        }

        public void removeLabelGraphicInfo(String key)
        {
            labelLocationMap.Remove(key);
        }

        public Dictionary<String, GraphicInfo> getLabelLocationMap()
        {
            return labelLocationMap;
        }

        public void addFlowGraphicInfoList(String key, List<GraphicInfo> graphicInfoList)
        {
            flowLocationMap.Add(key, graphicInfoList);
        }

        public IEnumerable<Signal> getSignals()
        {
            return signals;
        }

        public void setSignals(IEnumerable<Signal> signalList)
        {
            if (signalList != null)
            {
                signals.Clear();
                signals.AddRange(signalList);
            }
        }

        public void addSignal(Signal signal)
        {
            if (signal != null)
            {
                signals.Add(signal);
            }
        }

        public Boolean containsSignalId(String signalId)
        {
            return getSignal(signalId) != null;
        }

        public Signal getSignal(String id)
        {
            foreach (Signal signal in signals)
            {
                if (id.Equals(signal.getId()))
                {
                    return signal;
                }
            }
            return null;
        }

        public Dictionary<String, MessageFlow> getMessageFlows()
        {
            return messageFlowMap;
        }

        public void setMessageFlows(Dictionary<String, MessageFlow> messageFlows)
        {
            this.messageFlowMap = messageFlows;
        }

        public void addMessageFlow(MessageFlow messageFlow)
        {
            if (messageFlow != null && !String.IsNullOrWhiteSpace(messageFlow.getId()))
            {
                messageFlowMap.Add(messageFlow.getId(), messageFlow);
            }
        }

        public MessageFlow getMessageFlow(String id)
        {
            return messageFlowMap[id];
        }

        public Boolean containsMessageFlowId(String messageFlowId)
        {
            return messageFlowMap.ContainsKey(messageFlowId);
        }

        public IEnumerable<Message> getMessages()
        {
            return messageMap.Values;
        }

        public void setMessages(IEnumerable<Message> messageList)
        {
            if (messageList != null)
            {
                messageMap.Clear();
                foreach (Message message in messageList)
                {
                    addMessage(message);
                }
            }
        }

        public void addMessage(Message message)
        {
            if (message != null && !String.IsNullOrWhiteSpace(message.getId()))
            {
                messageMap.Add(message.getId(), message);
            }
        }

        public Message getMessage(String id)
        {
            return messageMap[id];
        }

        public Boolean containsMessageId(String messageId)
        {
            return messageMap.ContainsKey(messageId);
        }

        public Dictionary<String, String> getErrors()
        {
            return errorMap;
        }

        public void setErrors(Dictionary<String, String> errorMap)
        {
            this.errorMap = errorMap;
        }

        public void addError(String errorRef, String errorCode)
        {
            if (!String.IsNullOrWhiteSpace(errorRef))
            {
                errorMap.Add(errorRef, errorCode);
            }
        }

        public Boolean containsErrorRef(String errorRef)
        {
            return errorMap.ContainsKey(errorRef);
        }

        public Dictionary<String, ItemDefinition> getItemDefinitions()
        {
            return itemDefinitionMap;
        }

        public void setItemDefinitions(Dictionary<String, ItemDefinition> itemDefinitionMap)
        {
            this.itemDefinitionMap = itemDefinitionMap;
        }

        public void addItemDefinition(String id, ItemDefinition item)
        {
            if (!String.IsNullOrWhiteSpace(id))
            {
                itemDefinitionMap.Add(id, item);
            }
        }

        public Boolean containsItemDefinitionId(String id)
        {
            return itemDefinitionMap.ContainsKey(id);
        }

        public Dictionary<String, DataStore> getDataStores()
        {
            return dataStoreMap;
        }

        public void setDataStores(Dictionary<String, DataStore> dataStoreMap)
        {
            this.dataStoreMap = dataStoreMap;
        }

        public DataStore getDataStore(String id)
        {
            DataStore dataStore = null;
            if (dataStoreMap.ContainsKey(id))
            {
                dataStore = dataStoreMap[id];
            }
            return dataStore;
        }

        public void addDataStore(String id, DataStore dataStore)
        {
            if (!String.IsNullOrWhiteSpace(id))
            {
                dataStoreMap.Add(id, dataStore);
            }
        }

        public Boolean containsDataStore(String id)
        {
            return dataStoreMap.ContainsKey(id);
        }

        public List<Pool> getPools()
        {
            return pools;
        }

        public void setPools(List<Pool> pools)
        {
            this.pools = pools;
        }

        public List<Import> getImports()
        {
            return imports;
        }

        public void setImports(List<Import> imports)
        {
            this.imports = imports;
        }

        public List<Interface> getInterfaces()
        {
            return interfaces;
        }

        public void setInterfaces(List<Interface> interfaces)
        {
            this.interfaces = interfaces;
        }

        public List<Artifact> getGlobalArtifacts()
        {
            return globalArtifacts;
        }

        public void setGlobalArtifacts(List<Artifact> globalArtifacts)
        {
            this.globalArtifacts = globalArtifacts;
        }

        public void addNamespace(String prefix, String uri)
        {
            NamespaceMap.Add(prefix, uri);
        }

        public Boolean containsNamespacePrefix(String prefix)
        {
            return NamespaceMap.ContainsKey(prefix);
        }

        public String getNamespace(String prefix)
        {
            return NamespaceMap[prefix];
        }

        public Dictionary<String, String> getNamespaces()
        {
            return NamespaceMap;
        }

        public String getTargetNamespace()
        {
            return targetNamespace;
        }

        public void setTargetNamespace(String targetNamespace)
        {
            this.targetNamespace = targetNamespace;
        }

        public List<String> getUserTaskFormTypes()
        {
            return userTaskFormTypes;
        }

        public void setUserTaskFormTypes(List<String> userTaskFormTypes)
        {
            this.userTaskFormTypes = userTaskFormTypes;
        }

        public List<String> getStartEventFormTypes()
        {
            return startEventFormTypes;
        }

        public void setStartEventFormTypes(List<String> startEventFormTypes)
        {
            this.startEventFormTypes = startEventFormTypes;
        }
    }
}