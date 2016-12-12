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
using System.IO;
using bpmn_converter.converter;
using bpmn_converter.converter.util;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.converter.child;
using org.activiti.bpmn.converter.export;
using org.activiti.bpmn.converter.parser;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.exceptions;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter{

/**
 * //@author Tijs Rademakers

 * //@author Joram Barrez

 */
public class BpmnXMLConverter : BpmnXMLConstants {

  protected static  ILog LOGGER = LogManager.GetLogger(typeof(BpmnXMLConverter));
	
  protected static  String BPMN_XSD = "org/activiti/impl/bpmn/parser/BPMN20.xsd";
  protected static  String DEFAULT_ENCODING = "UTF-8";
  
	protected static Dictionary<String, BaseBpmnXMLConverter> convertersToBpmnMap = new Dictionary<String, BaseBpmnXMLConverter>();
	protected static Dictionary<BaseElement>, BaseBpmnXMLConverter> convertersToXMLMap = 
	    new Dictionary<BaseElement>, BaseBpmnXMLConverter>();
	
	protected ClassLoader classloader;
	protected List<String> userTaskFormTypes;
	protected List<String> startEventFormTypes;
	
	protected BpmnEdgeParser bpmnEdgeParser = new BpmnEdgeParser();
	protected BpmnShapeParser bpmnShapeParser = new BpmnShapeParser();
	protected DefinitionsParser definitionsParser = new DefinitionsParser();
	protected DocumentationParser documentationParser = new DocumentationParser();
	protected ExtensionElementsParser extensionElementsParser = new ExtensionElementsParser();
	protected ImportParser importParser = new ImportParser();
	protected InterfaceParser interfaceParser = new InterfaceParser();
  protected ItemDefinitionParser itemDefinitionParser = new ItemDefinitionParser();
  protected IOSpecificationParser ioSpecificationParser = new IOSpecificationParser();
  protected DataStoreParser dataStoreParser = new DataStoreParser();
  protected LaneParser laneParser = new LaneParser();
  protected MessageParser messageParser = new MessageParser();
  protected MessageFlowParser messageFlowParser = new MessageFlowParser();
  protected MultiInstanceParser multiInstanceParser = new MultiInstanceParser();
  protected ParticipantParser participantParser = new ParticipantParser();
  protected PotentialStarterParser potentialStarterParser = new PotentialStarterParser();
  protected ProcessParser processParser = new ProcessParser();
  protected SignalParser signalParser = new SignalParser();
  protected SubProcessParser subProcessParser = new SubProcessParser();
	
	static {
		// events
	  addConverter(new EndEventXMLConverter());
	  addConverter(new StartEventXMLConverter());
    
    // tasks
	  addConverter(new BusinessRuleTaskXMLConverter());
    addConverter(new ManualTaskXMLConverter());
    addConverter(new ReceiveTaskXMLConverter());
    addConverter(new ScriptTaskXMLConverter());
    addConverter(new ServiceTaskXMLConverter());
    addConverter(new SendTaskXMLConverter());
    addConverter(new UserTaskXMLConverter());
    addConverter(new TaskXMLConverter());
    addConverter(new CallActivityXMLConverter());
    
    // gateways
    addConverter(new EventGatewayXMLConverter());
    addConverter(new ExclusiveGatewayXMLConverter());
    addConverter(new InclusiveGatewayXMLConverter());
    addConverter(new ParallelGatewayXMLConverter());
    addConverter(new ComplexGatewayXMLConverter());
    
    // connectors
    addConverter(new SequenceFlowXMLConverter());
    
    // catch, throw and boundary event
    addConverter(new CatchEventXMLConverter());
    addConverter(new ThrowEventXMLConverter());
    addConverter(new BoundaryEventXMLConverter());
    
    // artifacts
    addConverter(new TextAnnotationXMLConverter());
    addConverter(new AssociationXMLConverter());
    
    // data store reference
    addConverter(new DataStoreReferenceXMLConverter());
    
    // data objects
    addConverter(new ValuedDataObjectXMLConverter(), typeof(StringDataObject));
    addConverter(new ValuedDataObjectXMLConverter(), typeof(BooleanDataObject));
    addConverter(new ValuedDataObjectXMLConverter(), typeof(IntegerDataObject));
    addConverter(new ValuedDataObjectXMLConverter(), typeof(LongDataObject));
    addConverter(new ValuedDataObjectXMLConverter(), typeof(DoubleDataObject));
    addConverter(new ValuedDataObjectXMLConverter(), typeof(DateDataObject));
    
    // Alfresco types
    //addConverter(new AlfrescoStartEventXMLConverter());
    //addConverter(new AlfrescoUserTaskXMLConverter());
  }
  
  public static void addConverter(BaseBpmnXMLConverter converter) {
    addConverter(converter, converter.getBpmnElementType());
  }
  
  public static void addConverter(BaseBpmnXMLConverter converter, Type elementType) {
    convertersToBpmnMap.Add(converter.getXMLElementName(), converter);
    convertersToXMLMap.Add(elementType, converter);
  }
  
  public void setClassloader(ClassLoader classloader) {
    typeof(this)loader = classloader;
  }

  public void setUserTaskFormTypes(List<String> userTaskFormTypes) {
    this.userTaskFormTypes = userTaskFormTypes;
  }
  
  public void setStartEventFormTypes(List<String> startEventFormTypes) {
    this.startEventFormTypes = startEventFormTypes;
  }
  
  public void validateModel(InputStreamProvider inputStreamProvider) {
    Schema schema = createSchema();
    
    Validator validator = schema.newValidator();
    validator.validate(new StreamSource(inputStreamProvider.getInputStream()));
  }
  
  public void validateModel(XMLStreamReader xmlStreamReader) {
    Schema schema = createSchema();
    
    Validator validator = schema.newValidator();
    validator.validate(new StAXSource(xmlStreamReader));
  }

  protected Schema createSchema() {
    SchemaFactory factory = SchemaFactory.newInstance(XMLConstants.W3C_XML_SCHEMA_NS_URI);
    Schema schema = null;
    if (classloader != null) {
      schema = factory.newSchema(classloader.getResource(BPMN_XSD));
    }
    
    if (schema == null) {
      schema = factory.newSchema(typeof(BpmnXMLConverter).getClassLoader().getResource(BPMN_XSD));
    }
    
    if (schema == null) {
      throw new XMLException("BPMN XSD could not be found");
    }
    return schema;
  }
  
  public BpmnModel convertToBpmnModel(InputStreamProvider inputStreamProvider, bool validateSchema, bool enableSafeBpmnXml) {
    return convertToBpmnModel(inputStreamProvider, validateSchema, enableSafeBpmnXml, DEFAULT_ENCODING);
  }
  
  public BpmnModel convertToBpmnModel(InputStreamProvider inputStreamProvider, bool validateSchema, bool enableSafeBpmnXml, String encoding) {
    XMLInputFactory xif = XMLInputFactory.newInstance();

    if (xif.isPropertySupported(XMLInputFactory.IS_REPLACING_ENTITY_REFERENCES)) {
      xif.setProperty(XMLInputFactory.IS_REPLACING_ENTITY_REFERENCES, false);
    }

    if (xif.isPropertySupported(XMLInputFactory.IS_SUPPORTING_EXTERNAL_ENTITIES)) {
      xif.setProperty(XMLInputFactory.IS_SUPPORTING_EXTERNAL_ENTITIES, false);
    }

    if (xif.isPropertySupported(XMLInputFactory.SUPPORT_DTD)) {
      xif.setProperty(XMLInputFactory.SUPPORT_DTD, false);
    }

    InputStreamReader in = null;
    try {
      in = new InputStreamReader(inputStreamProvider.getInputStream(), encoding);
      XMLStreamReader xtr = xif.createXMLStreamReader(in);
  
      try {
        if (validateSchema) {
          
          if (!enableSafeBpmnXml) {
            validateModel(inputStreamProvider);
          } else {
            validateModel(xtr);
          }
  
          // The input stream is closed after schema validation
          in = new InputStreamReader(inputStreamProvider.getInputStream(), encoding);
          xtr = xif.createXMLStreamReader(in);
        }
  
      } catch (Exception e) {
        throw new XMLException(e.getMessage(), e);
      }
  
      // XML conversion
      return convertToBpmnModel(xtr);
    } catch (UnsupportedEncodingException e) {
      throw new XMLException("The bpmn 2.0 xml is not UTF8 encoded", e);
    } catch (XMLStreamException e) {
      throw new XMLException("Error while reading the BPMN 2.0 XML", e);
    } finally {
      if (in != null) {
        try {
          in.close();
        } catch (IOException e) {
          LOGGER.debug("Problem closing BPMN input stream", e);
        }
      }
    }
  }

	public BpmnModel convertToBpmnModel(XMLStreamReader xtr) { 
	  BpmnModel model = new BpmnModel();
	  model.setStartEventFormTypes(startEventFormTypes);
	  model.setUserTaskFormTypes(userTaskFormTypes);
		try {
			Process activeProcess = null;
			List<SubProcess> activeSubProcessList = new List<SubProcess>();
			while (xtr.hasNext()) {
				try {
					xtr.next();
				} catch(Exception e) {
					LOGGER.debug("Error reading XML document", e);
					throw new XMLException("Error reading XML", e);
				}

				if (xtr.isEndElement()  && ELEMENT_SUBPROCESS.Equals(xtr.getLocalName())) {
					activeSubProcessList.remove(activeSubProcessList.size() - 1);
				}
				
				if (xtr.isEndElement()  && ELEMENT_TRANSACTION.Equals(xtr.getLocalName())) {
          activeSubProcessList.remove(activeSubProcessList.size() - 1);
        }

				if (xtr.isStartElement() == false)
					continue;

				if (ELEMENT_DEFINITIONS.Equals(xtr.getLocalName())) {
				  definitionsParser.parse(xtr, model);
				
				} else if (ELEMENT_SIGNAL.Equals(xtr.getLocalName())) {
					signalParser.parse(xtr, model);
					
				} else if (ELEMENT_MESSAGE.Equals(xtr.getLocalName())) {
					messageParser.parse(xtr, model);
          
				} else if (ELEMENT_ERROR.Equals(xtr.getLocalName())) {
          
          if (!String.IsNullOrWhiteSpace(xtr.getAttributeValue(null, ATTRIBUTE_ID))) {
            model.addError(xtr.getAttributeValue(null, ATTRIBUTE_ID),
                xtr.getAttributeValue(null, ATTRIBUTE_ERROR_CODE));
          }
          
				} else if (ELEMENT_IMPORT.Equals(xtr.getLocalName())) {
				  importParser.parse(xtr, model);
          
				} else if (ELEMENT_ITEM_DEFINITION.Equals(xtr.getLocalName())) {
				  itemDefinitionParser.parse(xtr, model);
          
				} else if (ELEMENT_DATA_STORE.Equals(xtr.getLocalName())) {
				  dataStoreParser.parse(xtr, model);
				  
				} else if (ELEMENT_INTERFACE.Equals(xtr.getLocalName())) {
				  interfaceParser.parse(xtr, model);
				  
				} else if (ELEMENT_IOSPECIFICATION.Equals(xtr.getLocalName())) {
				  ioSpecificationParser.parseChildElement(xtr, activeProcess, model);
					
				} else if (ELEMENT_PARTICIPANT.Equals(xtr.getLocalName())) {
				  participantParser.parse(xtr, model);
				  
				} else if (ELEMENT_MESSAGE_FLOW.Equals(xtr.getLocalName())) {
					messageFlowParser.parse(xtr, model);

				} else if (ELEMENT_PROCESS.Equals(xtr.getLocalName())) {
					
				  Process process = processParser.parse(xtr, model);
				  if (process != null) {
					  activeProcess = process;	
				  }
				
				} else if (ELEMENT_POTENTIAL_STARTER.Equals(xtr.getLocalName())) {
				  potentialStarterParser.parse(xtr, activeProcess);
				  
				} else if (ELEMENT_LANE.Equals(xtr.getLocalName())) {
					laneParser.parse(xtr, activeProcess, model);
					
				} else if (ELEMENT_DOCUMENTATION.Equals(xtr.getLocalName())) {
				  
					BaseElement parentElement = null;
					if (!activeSubProcessList.isEmpty()) {
						parentElement = activeSubProcessList.get(activeSubProcessList.size() - 1);
					} else if (activeProcess != null) {
						parentElement = activeProcess;
					}
					documentationParser.parseChildElement(xtr, parentElement, model);
				
				} else if (activeProcess == null && ELEMENT_TEXT_ANNOTATION.Equals(xtr.getLocalName())) {
				  String elementId = xtr.getAttributeValue(null, ATTRIBUTE_ID);
          TextAnnotation textAnnotation = (TextAnnotation) new TextAnnotationXMLConverter().convertXMLToElement(xtr, model);
          textAnnotation.setId(elementId);
          model.getGlobalArtifacts().Add(textAnnotation);
          
				} else if (activeProcess == null && ELEMENT_ASSOCIATION.Equals(xtr.getLocalName())) {
          String elementId = xtr.getAttributeValue(null, ATTRIBUTE_ID);
          Association association = (Association) new AssociationXMLConverter().convertXMLToElement(xtr, model);
          association.setId(elementId);
          model.getGlobalArtifacts().Add(association);
				
				} else if (ELEMENT_EXTENSIONS.Equals(xtr.getLocalName())) {
				  extensionElementsParser.parse(xtr, activeSubProcessList, activeProcess, model);
				
				} else if (ELEMENT_SUBPROCESS.Equals(xtr.getLocalName())) {
					subProcessParser.parse(xtr, activeSubProcessList, activeProcess);
          
				} else if (ELEMENT_TRANSACTION.Equals(xtr.getLocalName())) {
					subProcessParser.parse(xtr, activeSubProcessList, activeProcess);
					
				} else if (ELEMENT_DI_SHAPE.Equals(xtr.getLocalName())) {
					bpmnShapeParser.parse(xtr, model);
				
				} else if (ELEMENT_DI_EDGE.Equals(xtr.getLocalName())) {
				  bpmnEdgeParser.parse(xtr, model);

				} else {

					if (!activeSubProcessList.isEmpty() && ELEMENT_MULTIINSTANCE.equalsIgnoreCase(xtr.getLocalName())) {
						
					  multiInstanceParser.parseChildElement(xtr, activeSubProcessList.get(activeSubProcessList.size() - 1), model);
					  
					} else if (convertersToBpmnMap.containsKey(xtr.getLocalName())) {
					  if (activeProcess != null) {
  					  BaseBpmnXMLConverter converter = convertersToBpmnMap.get(xtr.getLocalName());
  					  converter.convertToBpmnModel(xtr, model, activeProcess, activeSubProcessList);
					  }
					}
				}
			}

			foreach (Process process  in model.getProcesses()) {
			  foreach (Pool pool  in model.getPools()) {
			    if (process.getId().Equals(pool.getProcessRef())) {
			      pool.setExecutable(process.isExecutable());
			    }
			  }
			  processFlowElements(process.getFlowElements(), process);
			}
		
		} catch (XMLException e) {
		  throw e;
		  
		} catch (Exception e) {
			LOGGER.error("Error processing BPMN document", e);
			throw new XMLException("Error processing BPMN document", e);
		}
		return model;
	}
	
	private void processFlowElements(Collection<FlowElement> flowElementList, BaseElement parentScope) {
	  foreach (FlowElement flowElement  in flowElementList) {
  	  if (flowElement as SequenceFlow !=null) {
        SequenceFlow sequenceFlow = (SequenceFlow) flowElement;
        FlowNode sourceNode = getFlowNodeFromScope(sequenceFlow.getSourceRef(), parentScope);
        if (sourceNode != null) {
          sourceNode.getOutgoingFlows().Add(sequenceFlow);
        }
        FlowNode targetNode = getFlowNodeFromScope(sequenceFlow.getTargetRef(), parentScope);
        if (targetNode != null) {
          targetNode.getIncomingFlows().Add(sequenceFlow);
        }
      } else if (flowElement as BoundaryEvent !=null) {
        BoundaryEvent boundaryEvent = (BoundaryEvent) flowElement;
        FlowElement attachedToElement = getFlowNodeFromScope(boundaryEvent.getAttachedToRefId(), parentScope);
        if(attachedToElement != null) {
          boundaryEvent.setAttachedToRef((Activity) attachedToElement);
          ((Activity) attachedToElement).getBoundaryEvents().Add(boundaryEvent);
        }
      } else if(flowElement as SubProcess !=null) {
        SubProcess subProcess = (SubProcess) flowElement;
        processFlowElements(subProcess.getFlowElements(), subProcess);
      }
	  }
	}
	
	private FlowNode getFlowNodeFromScope(String elementId, BaseElement scope) {
	  FlowNode flowNode = null;
	  if (!String.IsNullOrWhiteSpace(elementId)) {
  	  if (scope as Process !=null) {
  	    flowNode = (FlowNode) ((Process) scope).getFlowElement(elementId);
  	  } else if (scope as SubProcess !=null) {
  	    flowNode = (FlowNode) ((SubProcess) scope).getFlowElement(elementId);
  	  }
	  }
	  return flowNode;
	}
	
	public byte[] convertToXML(BpmnModel model) {
	  return convertToXML(model, DEFAULT_ENCODING);
	}
	
	public byte[] convertToXML(BpmnModel model, String encoding) {
    try {

      ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
      
      XMLOutputFactory xof = XMLOutputFactory.newInstance();
      OutputStreamWriter out = new OutputStreamWriter(outputStream, encoding);

      XMLStreamWriter writer = xof.createXMLStreamWriter(out);
      XMLStreamWriter xtw = new IndentingXMLStreamWriter(writer);

      DefinitionsRootExport.writeRootElement(model, xtw, encoding);
      CollaborationExport.writePools(model, xtw);
      DataStoreExport.writeDataStores(model, xtw);
      SignalAndMessageDefinitionExport.writeSignalsAndMessages(model, xtw);
      
      foreach (Process process  in model.getProcesses()) {
        
        if (process.getFlowElements().isEmpty() && process.getLanes().isEmpty()) {
          // empty process, ignore it 
          continue;
        }
      
        ProcessExport.writeProcess(process, xtw);
        
        foreach (FlowElement flowElement  in process.getFlowElements()) {
          createXML(flowElement, model, xtw);
        }
        
        foreach (Artifact artifact  in process.getArtifacts()) {
          createXML(artifact, model, xtw);
        }
        
        // end process element
        xtw.writeEndElement();
      }

      BPMNDIExport.writeBPMNDI(model, xtw);

      // end definitions root element
      xtw.writeEndElement();
      xtw.writeEndDocument();

      xtw.flush();

      outputStream.close();

      xtw.close();
      
      return outputStream.toByteArray();
      
    } catch (Exception e) {
      LOGGER.error("Error writing BPMN XML", e);
      throw new XMLException("Error writing BPMN XML", e);
    }
  }

  private void createXML(FlowElement flowElement, BpmnModel model, XMLStreamWriter xtw) {
    
    if (flowElement as SubProcess !=null) {
      
      SubProcess subProcess = (SubProcess) flowElement;
      xtw.writeStartElement(ELEMENT_SUBPROCESS);
      xtw.writeAttribute(ATTRIBUTE_ID, subProcess.getId());
      if (!String.IsNullOrWhiteSpace(subProcess.getName())) {
        xtw.writeAttribute(ATTRIBUTE_NAME, subProcess.getName());
      } else {
        xtw.writeAttribute(ATTRIBUTE_NAME, "subProcess");
      }
      
      if (subProcess as EventSubProcess !=null) {
        xtw.writeAttribute(ATTRIBUTE_TRIGGERED_BY, ATTRIBUTE_VALUE_TRUE);
      }
      
      if (!String.IsNullOrWhiteSpace(subProcess.getDocumentation())) {

        xtw.writeStartElement(ELEMENT_DOCUMENTATION);
        xtw.writeCharacters(subProcess.getDocumentation());
        xtw.writeEndElement();
      }
      
      bool didWriteExtensionStartElement = ActivitiListenerExport.writeListeners(subProcess, false, xtw);
      
      didWriteExtensionStartElement = BpmnXMLUtil.writeExtensionElements(subProcess, didWriteExtensionStartElement, model.getNamespaces(), xtw);
      if (didWriteExtensionStartElement) {
        // closing extensions element
        xtw.writeEndElement();
      }
      
      MultiInstanceExport.writeMultiInstance(subProcess, xtw);
      
      foreach (FlowElement subElement  in subProcess.getFlowElements()) {
        createXML(subElement, model, xtw);
      }
      
      foreach (Artifact artifact  in subProcess.getArtifacts()) {
        createXML(artifact, model, xtw);
      }
      
      xtw.writeEndElement();
      
    } else {
    
      BaseBpmnXMLConverter converter = convertersToXMLMap.get(flowElement.getClass());
      
      if (converter == null) {
        throw new XMLException("No converter for " + flowElement.getClass() + " found");
      }
      
      converter.convertToXML(xtw, flowElement, model);
    }
  }
  
  private void createXML(Artifact artifact, BpmnModel model, XMLStreamWriter xtw) {
    
    BaseBpmnXMLConverter converter = convertersToXMLMap.get(artifact.getClass());
      
    if (converter == null) {
      throw new XMLException("No converter for " + artifact.getClass() + " found");
    }
      
    converter.convertToXML(xtw, artifact, model);
  }
}
