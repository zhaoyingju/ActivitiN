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
	using NoExecutionVariableScope = org.activiti.engine.impl.el.NoExecutionVariableScope;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class FormPropertyHandler
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal string name;
	  protected internal AbstractFormType type;
	  protected internal bool isReadable;
	  protected internal bool isWritable;
	  protected internal bool isRequired;
	  protected internal string variableName;
	  protected internal Expression variableExpression;
	  protected internal Expression defaultExpression;

	  public virtual FormProperty createFormProperty(ExecutionEntity execution)
	  {
		FormPropertyImpl formProperty = new FormPropertyImpl(this);
		object modelValue = null;

		if (execution != null)
		{
		  if (variableName != null || variableExpression == null)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String varName = variableName != null ? variableName : id;
			string varName = variableName != null ? variableName : id;
			if (execution.hasVariable(varName))
			{
			  modelValue = execution.getVariable(varName);
			}
			else if (defaultExpression != null)
			{
			  modelValue = defaultExpression.getValue(execution);
			}
		  }
		  else
		  {
			modelValue = variableExpression.getValue(execution);
		  }
		}
		else
		{
		  // Execution is null, the form-property is used in a start-form. Default value
		  // should be available (ACT-1028) even though no execution is available.
		  if (defaultExpression != null)
		  {
			modelValue = defaultExpression.getValue(NoExecutionVariableScope.SharedInstance);
		  }
		}

		if (modelValue is string)
		{
		  formProperty.Value = (string) modelValue;
		}
		else if (type != null)
		{
		  string formValue = type.convertModelValueToFormValue(modelValue);
		  formProperty.Value = formValue;
		}
		else if (modelValue != null)
		{
		  formProperty.Value = modelValue.ToString();
		}

		return formProperty;
	  }

	  public virtual void submitFormProperty(ExecutionEntity execution, IDictionary<string, string> properties)
	  {
		if (!isWritable && properties.ContainsKey(id))
		{
		  throw new ActivitiException("form property '" + id + "' is not writable");
		}

		if (isRequired && !properties.ContainsKey(id) && defaultExpression == null)
		{
		  throw new ActivitiException("form property '" + id + "' is required");
		}
		bool propertyExits = false;
		object modelValue = null;
		if (properties.ContainsKey(id))
		{
			propertyExits = true;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String propertyValue = properties.remove(id);
		  string propertyValue = properties.Remove(id);
		  if (type != null)
		  {
			modelValue = type.convertFormValueToModelValue(propertyValue);
		  }
		  else
		  {
			modelValue = propertyValue;
		  }
		}
		else if (defaultExpression != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object expressionValue = defaultExpression.getValue(execution);
		  object expressionValue = defaultExpression.getValue(execution);
		  if (type != null && expressionValue != null)
		  {
			modelValue = type.convertFormValueToModelValue(expressionValue.ToString());
		  }
		  else if (expressionValue != null)
		  {
			modelValue = expressionValue.ToString();
		  }
		  else if (isRequired)
		  {
			throw new ActivitiException("form property '" + id + "' is required");
		  }
		}
		if (propertyExits || (modelValue != null))
		{
		  if (variableName != null)
		  {
			execution.setVariable(variableName, modelValue);
		  }
		  else if (variableExpression != null)
		  {
			variableExpression.setValue(modelValue, execution);
		  }
		  else
		  {
			execution.setVariable(id, modelValue);
		  }
		}
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual AbstractFormType Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }


	  public virtual bool Readable
	  {
		  get
		  {
			return isReadable;
		  }
		  set
		  {
			this.isReadable = value;
		  }
	  }


	  public virtual bool Required
	  {
		  get
		  {
			return isRequired;
		  }
		  set
		  {
			this.isRequired = value;
		  }
	  }


	  public virtual string VariableName
	  {
		  get
		  {
			return variableName;
		  }
		  set
		  {
			this.variableName = value;
		  }
	  }


	  public virtual Expression VariableExpression
	  {
		  get
		  {
			return variableExpression;
		  }
		  set
		  {
			this.variableExpression = value;
		  }
	  }


	  public virtual Expression DefaultExpression
	  {
		  get
		  {
			return defaultExpression;
		  }
		  set
		  {
			this.defaultExpression = value;
		  }
	  }


	  public virtual bool Writable
	  {
		  get
		  {
			return isWritable;
		  }
		  set
		  {
			this.isWritable = value;
		  }
	  }

	}

}