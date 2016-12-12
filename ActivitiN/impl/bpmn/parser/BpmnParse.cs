using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
namespace org.activiti.engine.impl.bpmn.parser
{


	using BpmnXMLConstants = org.activiti.bpmn.constants.BpmnXMLConstants;
	using BpmnXMLConverter = org.activiti.bpmn.converter.BpmnXMLConverter;
	using XMLException = org.activiti.bpmn.exceptions.XMLException;
	using BoundaryEvent = org.activiti.bpmn.model.BoundaryEvent;
	using BpmnModel = org.activiti.bpmn.model.BpmnModel;
	using Event = org.activiti.bpmn.model.Event;
	using FlowElement = org.activiti.bpmn.model.FlowElement;
	using FlowNode = org.activiti.bpmn.model.FlowNode;
	using GraphicInfo = org.activiti.bpmn.model.GraphicInfo;
	using Import = org.activiti.bpmn.model.Import;
	using Interface = org.activiti.bpmn.model.Interface;
	using Message = org.activiti.bpmn.model.Message;
	using Process = org.activiti.bpmn.model.Process;
	using SequenceFlow = org.activiti.bpmn.model.SequenceFlow;
	using SubProcess = org.activiti.bpmn.model.SubProcess;
	using ClassStructureDefinition = org.activiti.engine.impl.bpmn.data.ClassStructureDefinition;
	using ItemDefinition = org.activiti.engine.impl.bpmn.data.ItemDefinition;
	using ItemKind = org.activiti.engine.impl.bpmn.data.ItemKind;
	using StructureDefinition = org.activiti.engine.impl.bpmn.data.StructureDefinition;
	using ActivityBehaviorFactory = org.activiti.engine.impl.bpmn.parser.factory.ActivityBehaviorFactory;
	using ListenerFactory = org.activiti.engine.impl.bpmn.parser.factory.ListenerFactory;
	using BpmnInterface = org.activiti.engine.impl.bpmn.webservice.BpmnInterface;
	using BpmnInterfaceImplementation = org.activiti.engine.impl.bpmn.webservice.BpmnInterfaceImplementation;
	using MessageDefinition = org.activiti.engine.impl.bpmn.webservice.MessageDefinition;
	using Operation = org.activiti.engine.impl.bpmn.webservice.Operation;
	using OperationImplementation = org.activiti.engine.impl.bpmn.webservice.OperationImplementation;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.activiti.engine.impl.context.Context;
	using ExpressionManager = org.activiti.engine.impl.el.ExpressionManager;
	using DeploymentEntity = org.activiti.engine.impl.persistence.entity.DeploymentEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using HasDIBounds = org.activiti.engine.impl.pvm.process.HasDIBounds;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.activiti.engine.impl.pvm.process.TransitionImpl;
	using ReflectUtil = org.activiti.engine.impl.util.ReflectUtil;
	using InputStreamSource = org.activiti.engine.impl.util.io.InputStreamSource;
	using ResourceStreamSource = org.activiti.engine.impl.util.io.ResourceStreamSource;
	using StreamSource = org.activiti.engine.impl.util.io.StreamSource;
	using StringStreamSource = org.activiti.engine.impl.util.io.StringStreamSource;
	using UrlStreamSource = org.activiti.engine.impl.util.io.UrlStreamSource;
	using ProcessValidator = org.activiti.validation.ProcessValidator;
	using ValidationError = org.activiti.validation.ValidationError;
	using StringUtils = org.apache.commons.lang3.StringUtils;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// Specific parsing of one BPMN 2.0 XML file, created by the <seealso cref="BpmnParser"/>.
	/// 
	/// @author Tijs Rademakers
	/// @author Joram Barrez
	/// </summary>
	public class BpmnParse : BpmnXMLConstants
	{

	  protected internal static readonly Logger LOGGER = LoggerFactory.getLogger(typeof(BpmnParse));

	  public const string PROPERTYNAME_INITIAL = "initial";
	  public const string PROPERTYNAME_INITIATOR_VARIABLE_NAME = "initiatorVariableName";
	  public const string PROPERTYNAME_CONDITION = "condition";
	  public const string PROPERTYNAME_CONDITION_TEXT = "conditionText";
	  public const string PROPERTYNAME_TIMER_DECLARATION = "timerDeclarations";
	  public const string PROPERTYNAME_ISEXPANDED = "isExpanded";
	  public const string PROPERTYNAME_START_TIMER = "timerStart";
	  public const string PROPERTYNAME_COMPENSATION_HANDLER_ID = "compensationHandler";
	  public const string PROPERTYNAME_IS_FOR_COMPENSATION = "isForCompensation";
	  public const string PROPERTYNAME_ERROR_EVENT_DEFINITIONS = "errorEventDefinitions";
	  public const string PROPERTYNAME_EVENT_SUBSCRIPTION_DECLARATION = "eventDefinitions";

	  protected internal string name_Renamed;

	  protected internal bool validateSchema = true;
	  protected internal bool validateProcess = true;

	  protected internal StreamSource streamSource;
	  protected internal string sourceSystemId;

	  protected internal BpmnModel bpmnModel;

	  protected internal string targetNamespace;

	  /// <summary>
	  /// The deployment to which the parsed process definitions will be added. </summary>
	  protected internal DeploymentEntity deployment_Renamed;

	  /// <summary>
	  /// The end result of the parsing: a list of process definition. </summary>
	  protected internal IList<ProcessDefinitionEntity> processDefinitions = new List<ProcessDefinitionEntity>();

	  /// <summary>
	  /// A map for storing sequence flow based on their id during parsing. </summary>
	  protected internal IDictionary<string, TransitionImpl> sequenceFlows;

	  protected internal BpmnParseHandlers bpmnParserHandlers;

	  protected internal ProcessDefinitionEntity currentProcessDefinition;

	  protected internal FlowElement currentFlowElement;

	  protected internal ActivityImpl currentActivity;

	  protected internal LinkedList<SubProcess> currentSubprocessStack = new LinkedList<SubProcess>();

	  protected internal LinkedList<ScopeImpl> currentScopeStack = new LinkedList<ScopeImpl>();

	  /// <summary>
	  /// Mapping containing values stored during the first phase of parsing since
	  /// other elements can reference these messages.
	  /// 
	  /// All the map's elements are defined outside the process definition(s), which
	  /// means that this map doesn't need to be re-initialized for each new process
	  /// definition.
	  /// </summary>
	  protected internal IDictionary<string, MessageDefinition> messages = new Dictionary<string, MessageDefinition>();
	  protected internal IDictionary<string, StructureDefinition> structures = new Dictionary<string, StructureDefinition>();
	  protected internal IDictionary<string, BpmnInterfaceImplementation> interfaceImplementations = new Dictionary<string, BpmnInterfaceImplementation>();
	  protected internal IDictionary<string, OperationImplementation> operationImplementations = new Dictionary<string, OperationImplementation>();
	  protected internal IDictionary<string, ItemDefinition> itemDefinitions = new Dictionary<string, ItemDefinition>();
	  protected internal IDictionary<string, BpmnInterface> bpmnInterfaces = new Dictionary<string, BpmnInterface>();
	  protected internal IDictionary<string, Operation> operations = new Dictionary<string, Operation>();
	  protected internal IDictionary<string, XMLImporter> importers = new Dictionary<string, XMLImporter>();
	  protected internal IDictionary<string, string> prefixs = new Dictionary<string, string>();

	  // Factories
	  protected internal ExpressionManager expressionManager;
	  protected internal ActivityBehaviorFactory activityBehaviorFactory;
	  protected internal ListenerFactory listenerFactory;

	  /// <summary>
	  /// Constructor to be called by the <seealso cref="BpmnParser"/>.
	  /// </summary>
	  public BpmnParse(BpmnParser parser)
	  {
		this.expressionManager = parser.ExpressionManager;
		this.activityBehaviorFactory = parser.ActivityBehaviorFactory;
		this.listenerFactory = parser.ListenerFactory;
		this.bpmnParserHandlers = parser.getBpmnParserHandlers();
		this.initializeXSDItemDefinitions();
	  }

	  protected internal virtual void initializeXSDItemDefinitions()
	  {
		this.itemDefinitions["http://www.w3.org/2001/XMLSchema:string"] = new ItemDefinition("http://www.w3.org/2001/XMLSchema:string", new ClassStructureDefinition(typeof(string)));
	  }

	  public virtual BpmnParse deployment(DeploymentEntity deployment)
	  {
		this.deployment_Renamed = deployment;
		return this;
	  }

	  public virtual BpmnParse execute()
	  {
		try
		{

			ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		  BpmnXMLConverter converter = new BpmnXMLConverter();

		  bool enableSafeBpmnXml = false;
		  string encoding = null;
		  if (processEngineConfiguration != null)
		  {
			enableSafeBpmnXml = processEngineConfiguration.EnableSafeBpmnXml;
			encoding = processEngineConfiguration.XmlEncoding;
		  }

		  if (encoding != null)
		  {
			bpmnModel = converter.convertToBpmnModel(streamSource, validateSchema, enableSafeBpmnXml, encoding);
		  }
		  else
		  {
			bpmnModel = converter.convertToBpmnModel(streamSource, validateSchema, enableSafeBpmnXml);
		  }

		  // XSD validation goes first, then process/semantic validation
		  if (validateProcess)
		  {
			  ProcessValidator processValidator = processEngineConfiguration.ProcessValidator;
			  if (processValidator == null)
			  {
				  LOGGER.warn("Process should be validated, but no process validator is configured on the process engine configuration!");
			  }
			  else
			  {
				  IList<ValidationError> validationErrors = processValidator.validate(bpmnModel);
				  if (validationErrors != null && validationErrors.Count > 0)
				  {

					  StringBuilder warningBuilder = new StringBuilder();
					  StringBuilder errorBuilder = new StringBuilder();

				  foreach (ValidationError error in validationErrors)
				  {
					  if (error.Warning)
					  {
						  warningBuilder.Append(error.ToString());
						  warningBuilder.Append("\n");
					  }
					  else
					  {
						  errorBuilder.Append(error.ToString());
						  errorBuilder.Append("\n");
					  }
				  }

				  // Throw exception if there is any error
				  if (errorBuilder.Length > 0)
				  {
					  throw new ActivitiException("Errors while parsing:\n" + errorBuilder.ToString());
				  }

				  // Write out warnings (if any)
				  if (warningBuilder.Length > 0)
				  {
					  LOGGER.warn("Following warnings encountered during process validation: " + warningBuilder.ToString());
				  }

				  }
			  }
		  }

		  // Validation successfull (or no validation)
		  createImports();
		  createItemDefinitions();
		  createMessages();
		  createOperations();
		  transformProcessDefinitions();

		}
		catch (Exception e)
		{
		  if (e is ActivitiException)
		  {
			throw (ActivitiException) e;
		  }
		  else if (e is XMLException)
		  {
			throw (XMLException) e;
		  }
		  else
		  {
			throw new ActivitiException("Error parsing XML", e);
		  }
		}

		return this;
	  }

	  public virtual BpmnParse name(string name)
	  {
		this.name_Renamed = name;
		return this;
	  }

	  public virtual BpmnParse sourceInputStream(InputStream inputStream)
	  {
		if (name_Renamed == null)
		{
		  name("inputStream");
		}
		StreamSource = new InputStreamSource(inputStream);
		return this;
	  }

	  public virtual BpmnParse sourceResource(string resource)
	  {
		return sourceResource(resource, null);
	  }

	  public virtual BpmnParse sourceUrl(URL url)
	  {
		if (name_Renamed == null)
		{
		  name(url.ToString());
		}
		StreamSource = new UrlStreamSource(url);
		return this;
	  }

	  public virtual BpmnParse sourceUrl(string url)
	  {
		try
		{
		  return sourceUrl(new URL(url));
		}
		catch (MalformedURLException e)
		{
		  throw new ActivitiIllegalArgumentException("malformed url: " + url, e);
		}
	  }

	  public virtual BpmnParse sourceResource(string resource, ClassLoader classLoader)
	  {
		if (name_Renamed == null)
		{
		  name(resource);
		}
		StreamSource = new ResourceStreamSource(resource, classLoader);
		return this;
	  }

	  public virtual BpmnParse sourceString(string @string)
	  {
		if (name_Renamed == null)
		{
		  name("string");
		}
		StreamSource = new StringStreamSource(@string);
		return this;
	  }

	  protected internal virtual StreamSource StreamSource
	  {
		  set
		  {
			if (this.streamSource != null)
			{
			  throw new ActivitiIllegalArgumentException("invalid: multiple sources " + this.streamSource + " and " + value);
			}
			this.streamSource = value;
		  }
	  }

	  protected internal virtual void createImports()
	  {
		foreach (Import theImport in bpmnModel.Imports)
		{
		  XMLImporter importer = this.getImporter(theImport);
		  if (importer == null)
		  {
			throw new ActivitiException("Could not import item of type " + theImport.ImportType);
		  }
		  else
		  {
			importer.importFrom(theImport, this);
		  }
		}
	  }

	  protected internal virtual XMLImporter getImporter(Import theImport)
	  {
		if (this.importers.ContainsKey(theImport.ImportType))
		{
		  return this.importers[theImport.ImportType];
		}
		else
		{
		  if (theImport.ImportType.Equals("http://schemas.xmlsoap.org/wsdl/"))
		  {
			Type wsdlImporterClass;
			try
			{
			  wsdlImporterClass = Type.GetType("org.activiti.engine.impl.webservice.CxfWSDLImporter", true, Thread.CurrentThread.ContextClassLoader);
			  XMLImporter newInstance = (XMLImporter) wsdlImporterClass.newInstance();
			  this.importers[theImport.ImportType] = newInstance;
			  return newInstance;
			}
			catch (Exception)
			{
			  throw new ActivitiException("Could not find importer for type " + theImport.ImportType);
			}
		  }
		  return null;
		}
	  }

	  public virtual void createMessages()
	  {
		foreach (Message messageElement in bpmnModel.Messages)
		{
		  MessageDefinition messageDefinition = new MessageDefinition(messageElement.Id, name_Renamed);
		  if (StringUtils.isNotEmpty(messageElement.ItemRef))
		  {
			if (this.itemDefinitions.ContainsKey(messageElement.ItemRef))
			{
			  ItemDefinition itemDefinition = this.itemDefinitions[messageElement.ItemRef];
			  messageDefinition.ItemDefinition = itemDefinition;
			}
		  }
		  this.messages[messageDefinition.Id] = messageDefinition;

		}
	  }

	  protected internal virtual void createItemDefinitions()
	  {
		foreach (org.activiti.bpmn.model.ItemDefinition itemDefinitionElement in bpmnModel.ItemDefinitions.values())
		{
		  StructureDefinition structure = null;

		  try
		  {
			// it is a class
			Type classStructure = ReflectUtil.loadClass(itemDefinitionElement.StructureRef);
			structure = new ClassStructureDefinition(classStructure);
		  }
		  catch (ActivitiException)
		  {
			// it is a reference to a different structure
			structure = this.structures[itemDefinitionElement.StructureRef];
		  }

		  ItemDefinition itemDefinition = new ItemDefinition(itemDefinitionElement.Id, structure);
		  if (StringUtils.isNotEmpty(itemDefinitionElement.ItemKind))
		  {
			itemDefinition.setItemKind(Enum.Parse(typeof(ItemKind), itemDefinitionElement.ItemKind));
		  }
		  itemDefinitions[itemDefinition.Id] = itemDefinition;
		}
	  }

	  protected internal virtual void createOperations()
	  {
		foreach (Interface interfaceObject in bpmnModel.Interfaces)
		{
		  BpmnInterface bpmnInterface = new BpmnInterface(interfaceObject.Id, interfaceObject.Name);
		  bpmnInterface.setImplementation(this.interfaceImplementations[interfaceObject.ImplementationRef]);

		  foreach (org.activiti.bpmn.model.Operation operationObject in interfaceObject.Operations)
		  {
			if (this.messages.ContainsKey(operationObject.InMessageRef))
			{
			  MessageDefinition inMessage = this.messages[operationObject.InMessageRef];
			  Operation operation = new Operation(operationObject.Id, operationObject.Name, bpmnInterface, inMessage);
			  operation.setImplementation(this.operationImplementations[operationObject.ImplementationRef]);

			  if (StringUtils.isNotEmpty(operationObject.OutMessageRef))
			  {
				if (this.messages.ContainsKey(operationObject.OutMessageRef))
				{
				  MessageDefinition outMessage = this.messages[operationObject.OutMessageRef];
				  operation.setOutMessage(outMessage);
				}
			  }

			  operations[operation.Id] = operation;
			}
		  }
		}
	  }

	  /// <summary>
	  /// Parses the 'definitions' root element
	  /// </summary>
	  protected internal virtual void transformProcessDefinitions()
	  {
		sequenceFlows = new Dictionary<string, TransitionImpl>();
		foreach (Process process in bpmnModel.Processes)
		{
		  if (process.Executable)
		  {
			bpmnParserHandlers.parseElement(this, process);
		  }
		}

		if (processDefinitions.Count > 0)
		{
		  processDI();
		}
	  }

	  public virtual void processFlowElements(ICollection<FlowElement> flowElements)
	  {

		// Parsing the elements is done in a strict order of types,
		// as otherwise certain information might not be available when parsing a
		// certain type.

		// Using lists as we want to keep the order in which they are defined
		IList<SequenceFlow> sequenceFlowToParse = new List<SequenceFlow>();
		IList<BoundaryEvent> boundaryEventsToParse = new List<BoundaryEvent>();

		// Flow elements that depend on other elements are parse after the first run-through
		IList<FlowElement> defferedFlowElementsToParse = new List<FlowElement>();

		// Activities are parsed first
		foreach (FlowElement flowElement in flowElements)
		{

		  // Sequence flow are also flow elements, but are only parsed once everyactivity is found
		  if (flowElement is SequenceFlow)
		  {
			sequenceFlowToParse.Add((SequenceFlow) flowElement);
		  }
		  else if (flowElement is BoundaryEvent)
		  {
			boundaryEventsToParse.Add((BoundaryEvent) flowElement);
		  }
		  else if (flowElement is Event)
		  {
			defferedFlowElementsToParse.Add(flowElement);
		  }
		  else
		  {
			bpmnParserHandlers.parseElement(this, flowElement);
		  }

		}

		// Deferred elements
		foreach (FlowElement flowElement in defferedFlowElementsToParse)
		{
		  bpmnParserHandlers.parseElement(this, flowElement);
		}

		// Boundary events are parsed after all the regular activities are parsed
		foreach (BoundaryEvent boundaryEvent in boundaryEventsToParse)
		{
		  bpmnParserHandlers.parseElement(this, boundaryEvent);
		}

		// sequence flows
		foreach (SequenceFlow sequenceFlow in sequenceFlowToParse)
		{
		  bpmnParserHandlers.parseElement(this, sequenceFlow);
		}

	  }

	  // Diagram interchange
	  // /////////////////////////////////////////////////////////////////

	  public virtual void processDI()
	  {
		if (!bpmnModel.LocationMap.Empty)
		{

		  // Verify if all referenced elements exist
		  foreach (string bpmnReference in bpmnModel.LocationMap.Keys)
		  {
			if (bpmnModel.getFlowElement(bpmnReference) == null)
			{
				// ACT-1625: don't warn when	artifacts are referenced from DI
				if (bpmnModel.getArtifact(bpmnReference) == null)
				{
				  // check if it's a Pool or Lane, then DI is ok
				if (bpmnModel.getPool(bpmnReference) == null && bpmnModel.getLane(bpmnReference) == null)
				{
				  LOGGER.warn("Invalid reference in diagram interchange definition: could not find " + bpmnReference);
				}
				}
			}
			else if (!(bpmnModel.getFlowElement(bpmnReference) is FlowNode))
			{
			  LOGGER.warn("Invalid reference in diagram interchange definition: " + bpmnReference + " does not reference a flow node");
			}
		  }
		  foreach (string bpmnReference in bpmnModel.FlowLocationMap.Keys)
		  {
			if (bpmnModel.getFlowElement(bpmnReference) == null)
			{
			  // ACT-1625: don't warn when	artifacts are referenced from DI
				if (bpmnModel.getArtifact(bpmnReference) == null)
				{
					LOGGER.warn("Invalid reference in diagram interchange definition: could not find " + bpmnReference);
				}
			}
			else if (!(bpmnModel.getFlowElement(bpmnReference) is SequenceFlow))
			{
			  LOGGER.warn("Invalid reference in diagram interchange definition: " + bpmnReference + " does not reference a sequence flow");
			}
		  }

		  foreach (Process process in bpmnModel.Processes)
		  {
			if (!process.Executable)
			{
			  continue;
			}

			// Parse diagram interchange information
			ProcessDefinitionEntity processDefinition = getProcessDefinition(process.Id);
			if (processDefinition != null)
			{
			  processDefinition.GraphicalNotationDefined = true;
			  foreach (string shapeId in bpmnModel.LocationMap.Keys)
			  {
				if (processDefinition.findActivity(shapeId) != null)
				{
				  createBPMNShape(shapeId, bpmnModel.getGraphicInfo(shapeId), processDefinition);
				}
			  }

			  foreach (string edgeId in bpmnModel.FlowLocationMap.Keys)
			  {
				if (bpmnModel.getFlowElement(edgeId) != null)
				{
				  createBPMNEdge(edgeId, bpmnModel.getFlowLocationGraphicInfo(edgeId));
				}
			  }
			}
		  }
		}
	  }

	  public virtual void createBPMNShape(string key, GraphicInfo graphicInfo, ProcessDefinitionEntity processDefinition)
	  {
		ActivityImpl activity = processDefinition.findActivity(key);
		if (activity != null)
		{
		  createDIBounds(graphicInfo, activity);

		}
		else
		{
		  org.activiti.engine.impl.pvm.process.Lane lane = processDefinition.getLaneForId(key);

		  if (lane != null)
		  {
			// The shape represents a lane
			createDIBounds(graphicInfo, lane);
		  }
		}
	  }

	  protected internal virtual void createDIBounds(GraphicInfo graphicInfo, HasDIBounds target)
	  {
		target.X = (int) graphicInfo.X;
		target.Y = (int) graphicInfo.Y;
		target.Width = (int) graphicInfo.Width;
		target.Height = (int) graphicInfo.Height;
	  }

	  public virtual void createBPMNEdge(string key, IList<GraphicInfo> graphicList)
	  {
		FlowElement flowElement = bpmnModel.getFlowElement(key);
		if (flowElement != null && sequenceFlows.ContainsKey(key))
		{
		  TransitionImpl sequenceFlow = sequenceFlows[key];
		  IList<int?> waypoints = new List<int?>();
		  foreach (GraphicInfo waypointInfo in graphicList)
		  {
			waypoints.Add((int) waypointInfo.X);
			waypoints.Add((int) waypointInfo.Y);
		  }
		  sequenceFlow.Waypoints = waypoints;
		}
		else if (bpmnModel.getArtifact(key) != null)
		{
		  // it's an association, so nothing to do
		}
		else
		{
		  LOGGER.warn("Invalid reference in 'bpmnElement' attribute, sequenceFlow " + key + " not found");
		}
	  }

	  public virtual ProcessDefinitionEntity getProcessDefinition(string processDefinitionKey)
	  {
		foreach (ProcessDefinitionEntity processDefinition in processDefinitions)
		{
		  if (processDefinition.Key.Equals(processDefinitionKey))
		  {
			return processDefinition;
		  }
		}
		return null;
	  }

	  public virtual void addStructure(StructureDefinition structure)
	  {
		this.structures[structure.Id] = structure;
	  }

	  public virtual void addService(BpmnInterfaceImplementation bpmnInterfaceImplementation)
	  {
		this.interfaceImplementations[bpmnInterfaceImplementation.Name] = bpmnInterfaceImplementation;
	  }

	  public virtual void addOperation(OperationImplementation operationImplementation)
	  {
		this.operationImplementations[operationImplementation.Id] = operationImplementation;
	  }

	  /*
	   * ------------------- GETTERS AND SETTERS -------------------
	   */

	  public virtual bool ValidateSchema
	  {
		  get
		  {
				return validateSchema;
		  }
		  set
		  {
				this.validateSchema = value;
		  }
	  }


		public virtual bool ValidateProcess
		{
			get
			{
				return validateProcess;
			}
			set
			{
				this.validateProcess = value;
			}
		}


		public virtual IList<ProcessDefinitionEntity> ProcessDefinitions
		{
			get
			{
				return processDefinitions;
			}
		}

		public virtual string TargetNamespace
		{
			get
			{
			return targetNamespace;
			}
		}

	  public virtual BpmnParseHandlers getBpmnParserHandlers()
	  {
		return bpmnParserHandlers;
	  }

	  public virtual void setBpmnParserHandlers(BpmnParseHandlers bpmnParserHandlers)
	  {
		this.bpmnParserHandlers = bpmnParserHandlers;
	  }

	  public virtual DeploymentEntity Deployment
	  {
		  get
		  {
			return deployment_Renamed;
		  }
		  set
		  {
			this.deployment_Renamed = value;
		  }
	  }


	  public virtual BpmnModel BpmnModel
	  {
		  get
		  {
			return bpmnModel;
		  }
		  set
		  {
			this.bpmnModel = value;
		  }
	  }


	  public virtual ActivityBehaviorFactory ActivityBehaviorFactory
	  {
		  get
		  {
			return activityBehaviorFactory;
		  }
		  set
		  {
			this.activityBehaviorFactory = value;
		  }
	  }


	  public virtual ListenerFactory ListenerFactory
	  {
		  get
		  {
			return listenerFactory;
		  }
		  set
		  {
			this.listenerFactory = value;
		  }
	  }


	  public virtual ExpressionManager ExpressionManager
	  {
		  get
		  {
			return expressionManager;
		  }
		  set
		  {
			this.expressionManager = value;
		  }
	  }


	  public virtual IDictionary<string, TransitionImpl> SequenceFlows
	  {
		  get
		  {
			return sequenceFlows;
		  }
	  }

	  public virtual IDictionary<string, MessageDefinition> Messages
	  {
		  get
		  {
			return messages;
		  }
	  }

	  public virtual IDictionary<string, BpmnInterfaceImplementation> InterfaceImplementations
	  {
		  get
		  {
			return interfaceImplementations;
		  }
	  }

	  public virtual IDictionary<string, ItemDefinition> ItemDefinitions
	  {
		  get
		  {
			return itemDefinitions;
		  }
	  }

	  public virtual IDictionary<string, XMLImporter> Importers
	  {
		  get
		  {
			return importers;
		  }
	  }

	  public virtual IDictionary<string, Operation> Operations
	  {
		  get
		  {
			return operations;
		  }
		  set
		  {
			this.operations = value;
		  }
	  }


	  public virtual ProcessDefinitionEntity CurrentProcessDefinition
	  {
		  get
		  {
			return currentProcessDefinition;
		  }
		  set
		  {
			this.currentProcessDefinition = value;
		  }
	  }


	  public virtual FlowElement CurrentFlowElement
	  {
		  get
		  {
			return currentFlowElement;
		  }
		  set
		  {
			this.currentFlowElement = value;
		  }
	  }


	  public virtual ActivityImpl CurrentActivity
	  {
		  get
		  {
			return currentActivity;
		  }
		  set
		  {
			this.currentActivity = value;
		  }
	  }


	  public virtual SubProcess CurrentSubProcess
	  {
		  set
		  {
			currentSubprocessStack.push(value);
		  }
		  get
		  {
			return currentSubprocessStack.First.Value;
		  }
	  }


	  public virtual void removeCurrentSubProcess()
	  {
		currentSubprocessStack.pop();
	  }

	  public virtual ScopeImpl CurrentScope
	  {
		  set
		  {
			currentScopeStack.push(value);
		  }
		  get
		  {
			return currentScopeStack.First.Value;
		  }
	  }


	  public virtual void removeCurrentScope()
	  {
		currentScopeStack.pop();
	  }

	  public virtual BpmnParse setSourceSystemId(string systemId)
	  {
		sourceSystemId = systemId;
		return this;
	  }

	  public virtual string SourceSystemId
	  {
		  get
		  {
			return this.sourceSystemId;
		  }
	  }

	}
}