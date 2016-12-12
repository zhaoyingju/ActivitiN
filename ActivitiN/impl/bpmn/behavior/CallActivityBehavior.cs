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

namespace org.activiti.engine.impl.bpmn.behavior
{


	using MapExceptionEntry = org.activiti.bpmn.model.MapExceptionEntry;
	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using Expression = org.activiti.engine.@delegate.Expression;
	using AbstractDataAssociation = org.activiti.engine.impl.bpmn.data.AbstractDataAssociation;
	using ErrorPropagation = org.activiti.engine.impl.bpmn.helper.ErrorPropagation;
	using Context = org.activiti.engine.impl.context.Context;
	using DeploymentManager = org.activiti.engine.impl.persistence.deploy.DeploymentManager;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PvmProcessInstance = org.activiti.engine.impl.pvm.PvmProcessInstance;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using SubProcessActivityBehavior = org.activiti.engine.impl.pvm.@delegate.SubProcessActivityBehavior;


	/// <summary>
	/// Implementation of the BPMN 2.0 call activity
	/// (limited currently to calling a subprocess and not (yet) a global task).
	/// 
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class CallActivityBehavior : AbstractBpmnActivityBehavior, SubProcessActivityBehavior
	{

	  protected internal string processDefinitonKey;
	  private IList<AbstractDataAssociation> dataInputAssociations = new List<AbstractDataAssociation>();
	  private IList<AbstractDataAssociation> dataOutputAssociations = new List<AbstractDataAssociation>();
	  private Expression processDefinitionExpression;
	  protected internal IList<MapExceptionEntry> mapExceptions;
	  protected internal bool inheritVariables;

	  public CallActivityBehavior(string processDefinitionKey, IList<MapExceptionEntry> mapExceptions)
	  {
		this.processDefinitonKey = processDefinitionKey;
		this.mapExceptions = mapExceptions;
	  }

	  public CallActivityBehavior(Expression processDefinitionExpression, IList<MapExceptionEntry> mapExceptions) : base()
	  {
		this.processDefinitionExpression = processDefinitionExpression;
		this.mapExceptions = mapExceptions;
	  }

	  public virtual void addDataInputAssociation(AbstractDataAssociation dataInputAssociation)
	  {
		this.dataInputAssociations.Add(dataInputAssociation);
	  }

	  public virtual void addDataOutputAssociation(AbstractDataAssociation dataOutputAssociation)
	  {
		this.dataOutputAssociations.Add(dataOutputAssociation);
	  }

	  public virtual bool InheritVariables
	  {
		  set
		  {
			this.inheritVariables = value;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {

		string processDefinitonKey = this.processDefinitonKey;
		if (processDefinitionExpression != null)
		{
		  processDefinitonKey = (string) processDefinitionExpression.getValue(execution);
		}

		DeploymentManager deploymentManager = Context.ProcessEngineConfiguration.DeploymentManager;

		ProcessDefinitionEntity processDefinition = null;
		if (execution.TenantId == null || ProcessEngineConfiguration.NO_TENANT_ID.Equals(execution.TenantId))
		{
			processDefinition = deploymentManager.findDeployedLatestProcessDefinitionByKey(processDefinitonKey);
		}
		else
		{
			processDefinition = deploymentManager.findDeployedLatestProcessDefinitionByKeyAndTenantId(processDefinitonKey, execution.TenantId);
		}

		// Do not start a process instance if the process definition is suspended
		if (deploymentManager.isProcessDefinitionSuspended(processDefinition.Id))
		{
		  throw new ActivitiException("Cannot start process instance. Process definition " + processDefinition.Name + " (id = " + processDefinition.Id + ") is suspended");
		}

		PvmProcessInstance subProcessInstance = execution.createSubProcessInstance(processDefinition);

		if (inheritVariables)
		{
		  IDictionary<string, object> variables = execution.Variables;
		  foreach (KeyValuePair<string, object> entry in variables)
		  {
			subProcessInstance.setVariable(entry.Key, entry.Value);
		  }
		}

		// copy process variables
		foreach (AbstractDataAssociation dataInputAssociation in dataInputAssociations)
		{
		  object value = null;
		  if (dataInputAssociation.SourceExpression != null)
		  {
			value = dataInputAssociation.SourceExpression.getValue(execution);
		  }
		  else
		  {
			value = execution.getVariable(dataInputAssociation.Source);
		  }
		  subProcessInstance.setVariable(dataInputAssociation.Target, value);
		}

		try
		{
		  subProcessInstance.start();
		}
		catch (Exception e)
		{
			if (!ErrorPropagation.mapException(e, execution, mapExceptions, true))
			{
				throw e;
			}

		}

	  }

	  public virtual string ProcessDefinitonKey
	  {
		  set
		  {
			this.processDefinitonKey = value;
		  }
		  get
		  {
			return processDefinitonKey;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void completing(org.activiti.engine.delegate.DelegateExecution execution, org.activiti.engine.delegate.DelegateExecution subProcessInstance) throws Exception
	  public virtual void completing(DelegateExecution execution, DelegateExecution subProcessInstance)
	  {
		// only data.  no control flow available on this execution.

		// copy process variables
		foreach (AbstractDataAssociation dataOutputAssociation in dataOutputAssociations)
		{
		  object value = null;
		  if (dataOutputAssociation.SourceExpression != null)
		  {
			value = dataOutputAssociation.SourceExpression.getValue(subProcessInstance);
		  }
		  else
		  {
			value = subProcessInstance.getVariable(dataOutputAssociation.Source);
		  }

		  execution.setVariable(dataOutputAssociation.Target, value);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void completed(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void completed(ActivityExecution execution)
	  {
		// only control flow.  no sub process instance data available
		leave(execution);
	  }

	}

}