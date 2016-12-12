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

namespace org.activiti.engine.impl
{


	using BpmnModel = org.activiti.bpmn.model.BpmnModel;
	using ActivateProcessDefinitionCmd = org.activiti.engine.impl.cmd.ActivateProcessDefinitionCmd;
	using AddEditorSourceExtraForModelCmd = org.activiti.engine.impl.cmd.AddEditorSourceExtraForModelCmd;
	using AddEditorSourceForModelCmd = org.activiti.engine.impl.cmd.AddEditorSourceForModelCmd;
	using AddIdentityLinkForProcessDefinitionCmd = org.activiti.engine.impl.cmd.AddIdentityLinkForProcessDefinitionCmd;
	using ChangeDeploymentTenantIdCmd = org.activiti.engine.impl.cmd.ChangeDeploymentTenantIdCmd;
	using CreateModelCmd = org.activiti.engine.impl.cmd.CreateModelCmd;
	using DeleteDeploymentCmd = org.activiti.engine.impl.cmd.DeleteDeploymentCmd;
	using DeleteIdentityLinkForProcessDefinitionCmd = org.activiti.engine.impl.cmd.DeleteIdentityLinkForProcessDefinitionCmd;
	using DeleteModelCmd = org.activiti.engine.impl.cmd.DeleteModelCmd;
	using org.activiti.engine.impl.cmd;
	using GetBpmnModelCmd = org.activiti.engine.impl.cmd.GetBpmnModelCmd;
	using GetDeploymentProcessDefinitionCmd = org.activiti.engine.impl.cmd.GetDeploymentProcessDefinitionCmd;
	using GetDeploymentProcessDiagramCmd = org.activiti.engine.impl.cmd.GetDeploymentProcessDiagramCmd;
	using GetDeploymentProcessDiagramLayoutCmd = org.activiti.engine.impl.cmd.GetDeploymentProcessDiagramLayoutCmd;
	using GetDeploymentProcessModelCmd = org.activiti.engine.impl.cmd.GetDeploymentProcessModelCmd;
	using GetDeploymentResourceCmd = org.activiti.engine.impl.cmd.GetDeploymentResourceCmd;
	using GetDeploymentResourceNamesCmd = org.activiti.engine.impl.cmd.GetDeploymentResourceNamesCmd;
	using GetIdentityLinksForProcessDefinitionCmd = org.activiti.engine.impl.cmd.GetIdentityLinksForProcessDefinitionCmd;
	using GetModelCmd = org.activiti.engine.impl.cmd.GetModelCmd;
	using GetModelEditorSourceCmd = org.activiti.engine.impl.cmd.GetModelEditorSourceCmd;
	using GetModelEditorSourceExtraCmd = org.activiti.engine.impl.cmd.GetModelEditorSourceExtraCmd;
	using IsProcessDefinitionSuspendedCmd = org.activiti.engine.impl.cmd.IsProcessDefinitionSuspendedCmd;
	using SaveModelCmd = org.activiti.engine.impl.cmd.SaveModelCmd;
	using SetDeploymentCategoryCmd = org.activiti.engine.impl.cmd.SetDeploymentCategoryCmd;
	using SetProcessDefinitionCategoryCmd = org.activiti.engine.impl.cmd.SetProcessDefinitionCategoryCmd;
	using SuspendProcessDefinitionCmd = org.activiti.engine.impl.cmd.SuspendProcessDefinitionCmd;
	using ValidateBpmnModelCmd = org.activiti.engine.impl.cmd.ValidateBpmnModelCmd;
	using ModelEntity = org.activiti.engine.impl.persistence.entity.ModelEntity;
	using ReadOnlyProcessDefinition = org.activiti.engine.impl.pvm.ReadOnlyProcessDefinition;
	using DeploymentBuilderImpl = org.activiti.engine.impl.repository.DeploymentBuilderImpl;
	using Deployment = org.activiti.engine.repository.Deployment;
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
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// @author Joram Barrez
	/// </summary>
	public class RepositoryServiceImpl : ServiceImpl, RepositoryService
	{

	  public virtual DeploymentBuilder createDeployment()
	  {
		return new DeploymentBuilderImpl(this);
	  }

	  public virtual Deployment deploy(DeploymentBuilderImpl deploymentBuilder)
	  {
		return commandExecutor.execute(new DeployCmd<Deployment>(deploymentBuilder));
	  }

	  public virtual void deleteDeployment(string deploymentId)
	  {
		commandExecutor.execute(new DeleteDeploymentCmd(deploymentId, false));
	  }

	  public virtual void deleteDeploymentCascade(string deploymentId)
	  {
		commandExecutor.execute(new DeleteDeploymentCmd(deploymentId, true));
	  }

	  public virtual void deleteDeployment(string deploymentId, bool cascade)
	  {
		commandExecutor.execute(new DeleteDeploymentCmd(deploymentId, cascade));
	  }

	  public virtual void setDeploymentCategory(string deploymentId, string category)
	  {
		commandExecutor.execute(new SetDeploymentCategoryCmd(deploymentId, category));
	  }

	  public virtual ProcessDefinitionQuery createProcessDefinitionQuery()
	  {
		return new ProcessDefinitionQueryImpl(commandExecutor);
	  }

	  public override NativeProcessDefinitionQuery createNativeProcessDefinitionQuery()
	  {
		return new NativeProcessDefinitionQueryImpl(commandExecutor);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<String> getDeploymentResourceNames(String deploymentId)
	  public virtual IList<string> getDeploymentResourceNames(string deploymentId)
	  {
		return commandExecutor.execute(new GetDeploymentResourceNamesCmd(deploymentId));
	  }

	  public virtual InputStream getResourceAsStream(string deploymentId, string resourceName)
	  {
		return commandExecutor.execute(new GetDeploymentResourceCmd(deploymentId, resourceName));
	  }

	  public override void changeDeploymentTenantId(string deploymentId, string newTenantId)
	  {
		  commandExecutor.execute(new ChangeDeploymentTenantIdCmd(deploymentId, newTenantId));
	  }

	  public virtual DeploymentQuery createDeploymentQuery()
	  {
		return new DeploymentQueryImpl(commandExecutor);
	  }

	  public override NativeDeploymentQuery createNativeDeploymentQuery()
	  {
		return new NativeDeploymentQueryImpl(commandExecutor);
	  }

	  public virtual ProcessDefinition getProcessDefinition(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetDeploymentProcessDefinitionCmd(processDefinitionId));
	  }

	  public virtual BpmnModel getBpmnModel(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetBpmnModelCmd(processDefinitionId));
	  }

	  public virtual ReadOnlyProcessDefinition getDeployedProcessDefinition(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetDeploymentProcessDefinitionCmd(processDefinitionId));
	  }

	  public virtual bool isProcessDefinitionSuspended(string processDefinitionId)
	  {
		return commandExecutor.execute(new IsProcessDefinitionSuspendedCmd(processDefinitionId));
	  }

	  public virtual void suspendProcessDefinitionById(string processDefinitionId)
	  {
		commandExecutor.execute(new SuspendProcessDefinitionCmd(processDefinitionId, null, false, null, null));
	  }

	  public virtual void suspendProcessDefinitionById(string processDefinitionId, bool suspendProcessInstances, DateTime suspensionDate)
	  {
		commandExecutor.execute(new SuspendProcessDefinitionCmd(processDefinitionId, null, suspendProcessInstances, suspensionDate, null));
	  }

	  public virtual void suspendProcessDefinitionByKey(string processDefinitionKey)
	  {
		commandExecutor.execute(new SuspendProcessDefinitionCmd(null, processDefinitionKey, false, null, null));
	  }

	  public virtual void suspendProcessDefinitionByKey(string processDefinitionKey, bool suspendProcessInstances, DateTime suspensionDate)
	  {
		commandExecutor.execute(new SuspendProcessDefinitionCmd(null, processDefinitionKey, suspendProcessInstances, suspensionDate, null));
	  }

	  public virtual void suspendProcessDefinitionByKey(string processDefinitionKey, string tenantId)
	  {
		  commandExecutor.execute(new SuspendProcessDefinitionCmd(null, processDefinitionKey, false, null, tenantId));
	  }

	  public virtual void suspendProcessDefinitionByKey(string processDefinitionKey, bool suspendProcessInstances, DateTime suspensionDate, string tenantId)
	  {
		  commandExecutor.execute(new SuspendProcessDefinitionCmd(null, processDefinitionKey, suspendProcessInstances, suspensionDate, tenantId));
	  }

	  public virtual void activateProcessDefinitionById(string processDefinitionId)
	  {
		commandExecutor.execute(new ActivateProcessDefinitionCmd(processDefinitionId, null, false, null, null));
	  }

	  public virtual void activateProcessDefinitionById(string processDefinitionId, bool activateProcessInstances, DateTime activationDate)
	  {
		commandExecutor.execute(new ActivateProcessDefinitionCmd(processDefinitionId, null, activateProcessInstances, activationDate, null));
	  }

	  public virtual void activateProcessDefinitionByKey(string processDefinitionKey)
	  {
		commandExecutor.execute(new ActivateProcessDefinitionCmd(null, processDefinitionKey, false, null, null));
	  }

	  public virtual void activateProcessDefinitionByKey(string processDefinitionKey, bool activateProcessInstances, DateTime activationDate)
	  {
		commandExecutor.execute(new ActivateProcessDefinitionCmd(null, processDefinitionKey, activateProcessInstances, activationDate, null));
	  }

	  public virtual void activateProcessDefinitionByKey(string processDefinitionKey, string tenantId)
	  {
		  commandExecutor.execute(new ActivateProcessDefinitionCmd(null, processDefinitionKey, false, null, tenantId));
	  }

	  public virtual void activateProcessDefinitionByKey(string processDefinitionKey, bool activateProcessInstances, DateTime activationDate, string tenantId)
	  {
		  commandExecutor.execute(new ActivateProcessDefinitionCmd(null, processDefinitionKey, activateProcessInstances, activationDate, tenantId));
	  }

	  public virtual void setProcessDefinitionCategory(string processDefinitionId, string category)
	  {
		commandExecutor.execute(new SetProcessDefinitionCategoryCmd(processDefinitionId, category));
	  }

	  public virtual InputStream getProcessModel(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetDeploymentProcessModelCmd(processDefinitionId));
	  }

	  public virtual InputStream getProcessDiagram(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetDeploymentProcessDiagramCmd(processDefinitionId));
	  }

	  public virtual DiagramLayout getProcessDiagramLayout(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetDeploymentProcessDiagramLayoutCmd(processDefinitionId));
	  }

	  public virtual Model newModel()
	  {
		return commandExecutor.execute(new CreateModelCmd());
	  }

	  public virtual void saveModel(Model model)
	  {
		commandExecutor.execute(new SaveModelCmd((ModelEntity) model));
	  }

	  public virtual void deleteModel(string modelId)
	  {
		commandExecutor.execute(new DeleteModelCmd(modelId));
	  }

	  public virtual void addModelEditorSource(string modelId, sbyte[] bytes)
	  {
		commandExecutor.execute(new AddEditorSourceForModelCmd(modelId, bytes));
	  }

	  public virtual void addModelEditorSourceExtra(string modelId, sbyte[] bytes)
	  {
		commandExecutor.execute(new AddEditorSourceExtraForModelCmd(modelId, bytes));
	  }

	  public virtual ModelQuery createModelQuery()
	  {
		return new ModelQueryImpl(commandExecutor);
	  }

	  public override NativeModelQuery createNativeModelQuery()
	  {
		return new NativeModelQueryImpl(commandExecutor);
	  }

	  public virtual Model getModel(string modelId)
	  {
		return commandExecutor.execute(new GetModelCmd(modelId));
	  }

	  public virtual sbyte[] getModelEditorSource(string modelId)
	  {
		return commandExecutor.execute(new GetModelEditorSourceCmd(modelId));
	  }

	  public virtual sbyte[] getModelEditorSourceExtra(string modelId)
	  {
		return commandExecutor.execute(new GetModelEditorSourceExtraCmd(modelId));
	  }

	  public virtual void addCandidateStarterUser(string processDefinitionId, string userId)
	  {
		commandExecutor.execute(new AddIdentityLinkForProcessDefinitionCmd(processDefinitionId, userId, null));
	  }

	  public virtual void addCandidateStarterGroup(string processDefinitionId, string groupId)
	  {
		commandExecutor.execute(new AddIdentityLinkForProcessDefinitionCmd(processDefinitionId, null, groupId));
	  }

	  public virtual void deleteCandidateStarterGroup(string processDefinitionId, string groupId)
	  {
		commandExecutor.execute(new DeleteIdentityLinkForProcessDefinitionCmd(processDefinitionId, null, groupId));
	  }

	  public virtual void deleteCandidateStarterUser(string processDefinitionId, string userId)
	  {
		commandExecutor.execute(new DeleteIdentityLinkForProcessDefinitionCmd(processDefinitionId, userId, null));
	  }

	  public virtual IList<IdentityLink> getIdentityLinksForProcessDefinition(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetIdentityLinksForProcessDefinitionCmd(processDefinitionId));
	  }

	  public virtual IList<ValidationError> validateProcess(BpmnModel bpmnModel)
	  {
		  return commandExecutor.execute(new ValidateBpmnModelCmd(bpmnModel));
	  }

	}

}