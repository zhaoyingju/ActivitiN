using System;
using System.Collections.Generic;

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

namespace org.activiti.engine
{


	using BpmnModel = org.activiti.bpmn.model.BpmnModel;
	using DeploymentBuilder = org.activiti.engine.repository.DeploymentBuilder;
	using DeploymentQuery = org.activiti.engine.repository.DeploymentQuery;
	using DiagramLayout = org.activiti.engine.repository.DiagramLayout;
	using Model = org.activiti.engine.repository.Model;
	using ModelQuery = org.activiti.engine.repository.ModelQuery;
	using NativeDeploymentQuery = org.activiti.engine.repository.NativeDeploymentQuery;
	using NativeModelQuery = org.activiti.engine.repository.NativeModelQuery;
	using NativeProcessDefinitionQuery = org.activiti.engine.repository.NativeProcessDefinitionQuery;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.activiti.engine.repository.ProcessDefinitionQuery;
	using IdentityLink = org.activiti.engine.task.IdentityLink;
	using ValidationError = org.activiti.validation.ValidationError;

	/// <summary>
	/// Service providing access to the repository of process definitions and deployments.
	/// 
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// @author Tijs Rademakers
	/// @author Joram Barrez
	/// @author Henry Yan
	/// </summary>
	public interface RepositoryService
	{

	  /// <summary>
	  /// Starts creating a new deployment </summary>
	  DeploymentBuilder createDeployment();

	  /// <summary>
	  /// Deletes the given deployment. </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null.
	  /// @throwns RuntimeException if there are still runtime or history process 
	  /// instances or jobs.  </param>
	  void deleteDeployment(string deploymentId);

	  /// <summary>
	  /// Deletes the given deployment and cascade deletion to process instances, 
	  /// history process instances and jobs. </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null. </param>
	  /// @deprecated use <seealso cref="#deleteDeployment(String, boolean)"/>.  This methods may be deleted from 5.3.  
	  void deleteDeploymentCascade(string deploymentId);

	  /// <summary>
	  /// Deletes the given deployment and cascade deletion to process instances, 
	  /// history process instances and jobs. </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null. </param>
	  void deleteDeployment(string deploymentId, bool cascade);

	  /// <summary>
	  /// Sets the category of the deployment.
	  /// Deployments can be queried by category: see <seealso cref="DeploymentQuery#deploymentCategory(String)"/>.
	  /// </summary>
	  /// <exception cref="ActivitiObjectNotFoundException"> if no deployment with the provided id can be found. </exception>
	  void setDeploymentCategory(string deploymentId, string category);

	  /// <summary>
	  /// Retrieves a list of deployment resources for the given deployment, 
	  /// ordered alphabetically. </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null. </param>
	  IList<string> getDeploymentResourceNames(string deploymentId);

	  /// <summary>
	  /// Gives access to a deployment resource through a stream of bytes. </summary>
	  /// <param name="deploymentId"> id of the deployment, cannot be null. </param>
	  /// <param name="resourceName"> name of the resource, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the resource doesn't exist in the given deployment or when no deployment exists
	  /// for the given deploymentId. </exception>
	  InputStream getResourceAsStream(string deploymentId, string resourceName);

	  /// 
	  /// <summary>
	  /// EXPERIMENTAL FEATURE!
	  /// 
	  /// Changes the tenant identifier of a deployment to match the given tenant identifier.
	  /// This change will cascade to any related entity:
	  /// - process definitions related to the deployment
	  /// - process instances related to those process definitions
	  /// - executions related to those process instances
	  /// - tasks related to those process instances
	  /// - jobs related to the process definitions and process instances
	  /// 
	  /// This method can be used in the case that there was no tenant identifier set
	  /// on the deployment or those entities before.
	  /// 
	  /// This method can be used to remove a tenant identifier from the 
	  /// deployment and related entities (simply pass null).
	  /// 
	  /// Important: no optimistic locking will be done while executing the tenant identifier change!
	  /// 
	  /// This is an experimental feature, mainly because it WILL NOT work
	  /// properly in a clustered environment without special care:
	  /// suppose some process instance is in flight. The process definition is in the
	  /// process definition cache. When a task or job is created when continuing the process
	  /// instance, the process definition cache will be consulted to get the process definition
	  /// and from it the tenant identifier. Since it's cached, it will not be the new tenant identifier.
	  /// This method does clear the cache for this engineinstance , but it will not be cleared
	  /// on other nodes in a cluster (unless using a shared process definition cache). 
	  /// </summary>
	  /// <param name="deploymentId"> The id of the deployment of which the tenant identifier will be changed. </param>
	  /// <param name="newTenantId"> The new tenant identifier. </param>
	  void changeDeploymentTenantId(string deploymentId, string newTenantId);

	  /// <summary>
	  /// Query process definitions. </summary>
	  ProcessDefinitionQuery createProcessDefinitionQuery();

	  /// <summary>
	  /// Returns a new <seealso cref="org.activiti.engine.query.NativeQuery"/> for process definitions.
	  /// </summary>
	  NativeProcessDefinitionQuery createNativeProcessDefinitionQuery();

	  /// <summary>
	  /// Query deployment. </summary>
	  DeploymentQuery createDeploymentQuery();

	  /// <summary>
	  /// Returns a new <seealso cref="org.activiti.engine.query.NativeQuery"/> for deployment.
	  /// </summary>
	  NativeDeploymentQuery createNativeDeploymentQuery();

	  /// <summary>
	  /// Suspends the process definition with the given id. 
	  /// 
	  /// If a process definition is in state suspended, it will not be possible to start new process instances
	  /// based on the process definition.
	  /// 
	  /// <strong>Note: all the process instances of the process definition will still be active 
	  /// (ie. not suspended)!</strong>
	  /// </summary>
	  ///  <exception cref="ActivitiObjectNotFoundException"> if no such processDefinition can be found </exception>
	  ///  <exception cref="ActivitiException"> if the process definition is already in state suspended. </exception>
	  void suspendProcessDefinitionById(string processDefinitionId);

	  /// <summary>
	  /// Suspends the process definition with the given id. 
	  /// 
	  /// If a process definition is in state suspended, it will not be possible to start new process instances
	  /// based on the process definition.
	  /// </summary>
	  /// <param name="suspendProcessInstances"> If true, all the process instances of the provided process definition
	  ///                                will be suspended too. </param>
	  /// <param name="suspensionDate"> The date on which the process definition will be suspended. If null, the 
	  ///                       process definition is suspended immediately. 
	  ///                       Note: The job executor needs to be active to use this!                                
	  /// </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> if no such processDefinition can be found. </exception>
	  /// <exception cref="ActivitiException"> if the process definition is already in state suspended. </exception>
	  void suspendProcessDefinitionById(string processDefinitionId, bool suspendProcessInstances, DateTime suspensionDate);

	  /// <summary>
	  /// Suspends the <strong>all</strong> process definitions with the given key (= id in the bpmn20.xml file).
	  /// 
	  /// If a process definition is in state suspended, it will not be possible to start new process instances
	  /// based on the process definition.
	  /// 
	  /// <strong>Note: all the process instances of the process definition will still be active 
	  /// (ie. not suspended)!</strong>
	  /// </summary>
	  ///  <exception cref="ActivitiObjectNotFoundException"> if no such processDefinition can be found </exception>
	  ///  <exception cref="ActivitiException"> if the process definition is already in state suspended. </exception>
	  void suspendProcessDefinitionByKey(string processDefinitionKey);

	  /// <summary>
	  /// Suspends the <strong>all</strong> process definitions with the given key (= id in the bpmn20.xml file).
	  /// 
	  /// If a process definition is in state suspended, it will not be possible to start new process instances
	  /// based on the process definition.
	  /// </summary>
	  /// <param name="suspendProcessInstances"> If true, all the process instances of the provided process definition
	  ///                                will be suspended too. </param>
	  /// <param name="suspensionDate"> The date on which the process definition will be suspended. If null, the 
	  ///                       process definition is suspended immediately. 
	  ///                       Note: The job executor needs to be active to use this! </param>
	  ///  <exception cref="ActivitiObjectNotFoundException"> if no such processDefinition can be found </exception>
	  ///  <exception cref="ActivitiException"> if the process definition is already in state suspended. </exception>
	  void suspendProcessDefinitionByKey(string processDefinitionKey, bool suspendProcessInstances, DateTime suspensionDate);

	  /// <summary>
	  /// Similar to <seealso cref="#suspendProcessDefinitionByKey(String)"/>, but only applicable for the given tenant identifier.
	  /// </summary>
	  void suspendProcessDefinitionByKey(string processDefinitionKey, string tenantId);

	  /// <summary>
	  /// Similar to <seealso cref="#suspendProcessDefinitionByKey(String, boolean, Date)"/>, but only applicable for the given tenant identifier.
	  /// </summary>
	  void suspendProcessDefinitionByKey(string processDefinitionKey, bool suspendProcessInstances, DateTime suspensionDate, string tenantId);

	  /// <summary>
	  /// Activates the process definition with the given id. 
	  /// </summary>
	  /// <exception cref="ActivitiObjectNotFoundException"> if no such processDefinition can be found or if the process definition is already in state active. </exception>
	  void activateProcessDefinitionById(string processDefinitionId);

	  /// <summary>
	  /// Activates the process definition with the given id. 
	  /// </summary>
	  /// <param name="activationDate"> The date on which the process definition will be activated. If null, the
	  ///                       process definition is activated immediately. 
	  ///                       Note: The job executor needs to be active to use this!                                 
	  /// </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> if no such processDefinition can be found. </exception>
	  /// <exception cref="ActivitiException"> if the process definition is already in state active. </exception>
	  void activateProcessDefinitionById(string processDefinitionId, bool activateProcessInstances, DateTime activationDate);

	  /// <summary>
	  /// Activates the process definition with the given key (=id in the bpmn20.xml file). 
	  /// </summary>
	  /// <exception cref="ActivitiObjectNotFoundException"> if no such processDefinition can be found. </exception>
	  /// <exception cref="ActivitiException"> if the process definition is already in state active. </exception>
	  void activateProcessDefinitionByKey(string processDefinitionKey);

	  /// <summary>
	  /// Activates the process definition with the given key (=id in the bpmn20.xml file). 
	  /// </summary>
	  /// <param name="activationDate"> The date on which the process definition will be activated. If null, the
	  ///                       process definition is activated immediately. 
	  ///                       Note: The job executor needs to be active to use this!                                 
	  /// </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> if no such processDefinition can be found. </exception>
	  /// <exception cref="ActivitiException"> if the process definition is already in state active. </exception>
	  void activateProcessDefinitionByKey(string processDefinitionKey, bool activateProcessInstances, DateTime activationDate);

	  /// <summary>
	  /// Similar to <seealso cref="#activateProcessDefinitionByKey(String)"/>, but only applicable for the given tenant identifier. 
	  /// </summary>
	  void activateProcessDefinitionByKey(string processDefinitionKey, string tenantId);

	  /// <summary>
	  /// Similar to <seealso cref="#activateProcessDefinitionByKey(String, boolean, Date)"/>, but only applicable for the given tenant identifier. 
	  /// </summary>
	  void activateProcessDefinitionByKey(string processDefinitionKey, bool activateProcessInstances, DateTime activationDate, string tenantId);

	  /// <summary>
	  /// Sets the category of the process definition.
	  /// Process definitions can be queried by category: see <seealso cref="ProcessDefinitionQuery#processDefinitionCategory(String)"/>.
	  /// </summary>
	  /// <exception cref="ActivitiObjectNotFoundException"> if no process defintion with the provided id can be found. </exception>
	  void setProcessDefinitionCategory(string processDefinitionId, string category);

	  /// <summary>
	  /// Gives access to a deployed process model, e.g., a BPMN 2.0 XML file,
	  /// through a stream of bytes.
	  /// </summary>
	  /// <param name="processDefinitionId">
	  ///          id of a <seealso cref="ProcessDefinition"/>, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException">
	  ///           when the process model doesn't exist. </exception>
	  InputStream getProcessModel(string processDefinitionId);

	  /// <summary>
	  /// Gives access to a deployed process diagram, e.g., a PNG image, through a
	  /// stream of bytes.
	  /// </summary>
	  /// <param name="processDefinitionId">
	  ///          id of a <seealso cref="ProcessDefinition"/>, cannot be null. </param>
	  /// <returns> null when the diagram resource name of a <seealso cref="ProcessDefinition"/> is null. </returns>
	  /// <exception cref="ActivitiObjectNotFoundException">
	  ///           when the process diagram doesn't exist. </exception>
	  InputStream getProcessDiagram(string processDefinitionId);

	  /// <summary>
	  /// Returns the <seealso cref="ProcessDefinition"/> including all BPMN information like additional 
	  /// Properties (e.g. documentation).
	  /// </summary>
	  ProcessDefinition getProcessDefinition(string processDefinitionId);

	  /// <summary>
	  /// Returns the <seealso cref="BpmnModel"/> corresponding with the process definition with
	  /// the provided process definition id. The <seealso cref="BpmnModel"/> is a pojo versions
	  /// of the BPMN 2.0 xml and can be used to introspect the process definition
	  /// using regular Java.
	  /// </summary>
	  BpmnModel getBpmnModel(string processDefinitionId);

	  /// <summary>
	  /// Checks if the process definition is suspended.
	  /// </summary>
	  bool isProcessDefinitionSuspended(string processDefinitionId);

	  /// <summary>
	  /// Provides positions and dimensions of elements in a process diagram as
	  /// provided by <seealso cref="RepositoryService#getProcessDiagram(String)"/>.
	  /// 
	  /// This method requires a process model and a diagram image to be deployed. </summary>
	  /// <param name="processDefinitionId"> id of a <seealso cref="ProcessDefinition"/>, cannot be null. </param>
	  /// <returns> Map with process element ids as keys and positions and dimensions as values. </returns>
	  /// <returns> null when the input stream of a process diagram is null. </returns>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the process model or diagram doesn't exist. </exception>
	  DiagramLayout getProcessDiagramLayout(string processDefinitionId);

	  /// <summary>
	  /// Creates a new model. The model is transient and must be saved using 
	  /// <seealso cref="#saveModel(Model)"/>.
	  /// </summary>
	  Model newModel();

	  /// <summary>
	  /// Saves the model. If the model already existed, the model is updated
	  /// otherwise a new model is created. </summary>
	  /// <param name="model"> model to save, cannot be null. </param>
	  void saveModel(Model model);

	  /// <param name="modelId"> id of model to delete, cannot be null. When an id is passed
	  /// for an unexisting model, this operation is ignored. </param>
	  void deleteModel(string modelId);

	  /// <summary>
	  /// Saves the model editor source for a model </summary>
	  /// <param name="modelId"> id of model to delete, cannot be null. When an id is passed
	  /// for an unexisting model, this operation is ignored. </param>
	  void addModelEditorSource(string modelId, sbyte[] bytes);

	  /// <summary>
	  /// Saves the model editor source extra for a model </summary>
	  /// <param name="modelId"> id of model to delete, cannot be null. When an id is passed
	  /// for an unexisting model, this operation is ignored. </param>
	  void addModelEditorSourceExtra(string modelId, sbyte[] bytes);

	  /// <summary>
	  /// Query models. </summary>
	  ModelQuery createModelQuery();

	  /// <summary>
	  /// Returns a new <seealso cref="org.activiti.engine.query.NativeQuery"/> for process definitions.
	  /// </summary>
	  NativeModelQuery createNativeModelQuery();

	  /// <summary>
	  /// Returns the <seealso cref="Model"/> </summary>
	  /// <param name="modelId"> id of model </param>
	  Model getModel(string modelId);

	  /// <summary>
	  /// Returns the model editor source as a byte array </summary>
	  /// <param name="modelId"> id of model </param>
	  sbyte[] getModelEditorSource(string modelId);

	  /// <summary>
	  /// Returns the model editor source extra as a byte array </summary>
	  /// <param name="modelId"> id of model </param>
	  sbyte[] getModelEditorSourceExtra(string modelId);

	  /// <summary>
	  /// Authorizes a candidate user for a process definition. </summary>
	  /// <param name="processDefinitionId"> id of the process definition, cannot be null. </param>
	  /// <param name="userId"> id of the user involve, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the process definition or user doesn't exist. </exception>
	  void addCandidateStarterUser(string processDefinitionId, string userId);

	  /// <summary>
	  /// Authorizes a candidate group for a process definition. </summary>
	  /// <param name="processDefinitionId"> id of the process definition, cannot be null. </param>
	  /// <param name="groupId"> id of the group involve, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the process definition or group doesn't exist. </exception>
	  void addCandidateStarterGroup(string processDefinitionId, string groupId);

	  /// <summary>
	  /// Removes the authorization of a candidate user for a process definition. </summary>
	  /// <param name="processDefinitionId"> id of the process definition, cannot be null. </param>
	  /// <param name="userId"> id of the user involve, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the process definition or user doesn't exist. </exception>
	  void deleteCandidateStarterUser(string processDefinitionId, string userId);

	  /// <summary>
	  /// Removes the authorization of a candidate group for a process definition. </summary>
	  /// <param name="processDefinitionId"> id of the process definition, cannot be null. </param>
	  /// <param name="groupId"> id of the group involve, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when the process definition or group doesn't exist. </exception>
	  void deleteCandidateStarterGroup(string processDefinitionId, string groupId);

	  /// <summary>
	  /// Retrieves the <seealso cref="IdentityLink"/>s associated with the given process definition.
	  /// Such an <seealso cref="IdentityLink"/> informs how a certain identity (eg. group or user)
	  /// is authorized for a certain process definition
	  /// </summary>
	  IList<IdentityLink> getIdentityLinksForProcessDefinition(string processDefinitionId);

	  /// <summary>
	  /// Validates the given process definition against the rules for executing a process definition
	  /// on the Activiti engine.
	  /// 
	  /// To create such a <seealso cref="BpmnModel"/> from a String, following code may be used:
	  /// 
	  /// XMLInputFactory xif = XMLInputFactory.newInstance();
	  /// InputStreamReader in = new InputStreamReader(new ByteArrayInputStream(myProcess.getBytes()), "UTF-8"); // Change to other streams for eg from classpath
	  /// XMLStreamReader xtr = xif.createXMLStreamReader(in);
	  /// bpmnModel = new BpmnXMLConverter().convertToBpmnModel(xtr);
	  /// 
	  /// </summary>
	  IList<ValidationError> validateProcess(BpmnModel bpmnModel);

	}

}