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

namespace org.activiti.engine.impl.form
{


	using Expression = org.activiti.engine.@delegate.Expression;
	using AbstractFormType = org.activiti.engine.form.AbstractFormType;
	using FormProperty = org.activiti.engine.form.FormProperty;
	using Context = org.activiti.engine.impl.context.Context;
	using ExpressionManager = org.activiti.engine.impl.el.ExpressionManager;
	using DeploymentEntity = org.activiti.engine.impl.persistence.entity.DeploymentEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using StringUtils = org.apache.commons.lang3.StringUtils;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class DefaultFormHandler : FormHandler
	{

	  protected internal Expression formKey;
	  protected internal string deploymentId;
	  protected internal IList<FormPropertyHandler> formPropertyHandlers = new List<FormPropertyHandler>();

	  public virtual void parseConfiguration(IList<org.activiti.bpmn.model.FormProperty> formProperties, string formKey, DeploymentEntity deployment, ProcessDefinitionEntity processDefinition)
	  {
		this.deploymentId = deployment.Id;

		ExpressionManager expressionManager = Context.ProcessEngineConfiguration.ExpressionManager;

		if (StringUtils.isNotEmpty(formKey))
		{
		  this.formKey = expressionManager.createExpression(formKey);
		}

		FormTypes formTypes = Context.ProcessEngineConfiguration.FormTypes;

		foreach (org.activiti.bpmn.model.FormProperty formProperty in formProperties)
		{
		  FormPropertyHandler formPropertyHandler = new FormPropertyHandler();
		  formPropertyHandler.Id = formProperty.Id;
		  formPropertyHandler.Name = formProperty.Name;

		  AbstractFormType type = formTypes.parseFormPropertyType(formProperty);
		  formPropertyHandler.Type = type;
		  formPropertyHandler.Required = formProperty.Required;
		  formPropertyHandler.Readable = formProperty.Readable;
		  formPropertyHandler.Writable = formProperty.Writeable;
		  formPropertyHandler.VariableName = formProperty.Variable;

		  if (StringUtils.isNotEmpty(formProperty.Expression))
		  {
			Expression expression = expressionManager.createExpression(formProperty.Expression);
			formPropertyHandler.VariableExpression = expression;
		  }

		  if (StringUtils.isNotEmpty(formProperty.DefaultExpression))
		  {
			Expression defaultExpression = expressionManager.createExpression(formProperty.DefaultExpression);
			formPropertyHandler.DefaultExpression = defaultExpression;
		  }

		  formPropertyHandlers.Add(formPropertyHandler);
		}
	  }

	  protected internal virtual void initializeFormProperties(FormDataImpl formData, ExecutionEntity execution)
	  {
		IList<FormProperty> formProperties = new List<FormProperty>();
		foreach (FormPropertyHandler formPropertyHandler in formPropertyHandlers)
		{
		  if (formPropertyHandler.Readable)
		  {
			FormProperty formProperty = formPropertyHandler.createFormProperty(execution);
			formProperties.Add(formProperty);
		  }
		}
		formData.FormProperties = formProperties;
	  }

	  public virtual void submitFormProperties(IDictionary<string, string> properties, ExecutionEntity execution)
	  {
		IDictionary<string, string> propertiesCopy = new Dictionary<string, string>(properties);
		foreach (FormPropertyHandler formPropertyHandler in formPropertyHandlers)
		{
		  // submitFormProperty will remove all the keys which it takes care of
		  formPropertyHandler.submitFormProperty(execution, propertiesCopy);
		}
		foreach (string propertyId in propertiesCopy.Keys)
		{
		  execution.setVariable(propertyId, propertiesCopy[propertyId]);
		}
	  }


	  // getters and setters //////////////////////////////////////////////////////

	  public virtual Expression FormKey
	  {
		  get
		  {
			return formKey;
		  }
		  set
		  {
			this.formKey = value;
		  }
	  }


	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
		  }
	  }


	  public virtual IList<FormPropertyHandler> FormPropertyHandlers
	  {
		  get
		  {
			return formPropertyHandlers;
		  }
		  set
		  {
			this.formPropertyHandlers = value;
		  }
	  }

	}

}