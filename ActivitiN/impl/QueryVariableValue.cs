using System;

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

	using VariableInstanceEntity = org.activiti.engine.impl.persistence.entity.VariableInstanceEntity;
	using ByteArrayType = org.activiti.engine.impl.variable.ByteArrayType;
	using JPAEntityListVariableType = org.activiti.engine.impl.variable.JPAEntityListVariableType;
	using JPAEntityVariableType = org.activiti.engine.impl.variable.JPAEntityVariableType;
	using VariableType = org.activiti.engine.impl.variable.VariableType;
	using VariableTypes = org.activiti.engine.impl.variable.VariableTypes;


	/// <summary>
	/// Represents a variable value used in queries.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	[Serializable]
	public class QueryVariableValue
	{
	  private const long serialVersionUID = 1L;
	  private string name;
	  private object value;
	  private QueryOperator @operator;

	  private VariableInstanceEntity variableInstanceEntity;
	  private bool local;

	  public QueryVariableValue(string name, object value, QueryOperator @operator, bool local)
	  {
		this.name = name;
		this.value = value;
		this.@operator = @operator;
		this.local = local;
	  }

	  public virtual void initialize(VariableTypes types)
	  {
		if (variableInstanceEntity == null)
		{
		  VariableType type = types.findVariableType(value);
		  if (type is ByteArrayType)
		  {
			throw new ActivitiIllegalArgumentException("Variables of type ByteArray cannot be used to query");
		  }
		  else if (type is JPAEntityVariableType && @operator != QueryOperator.EQUALS)
		  {
			throw new ActivitiIllegalArgumentException("JPA entity variables can only be used in 'variableValueEquals'");
		  }
		  else if (type is JPAEntityListVariableType)
		  {
			throw new ActivitiIllegalArgumentException("Variables containing a list of JPA entities cannot be used to query");
		  }
		  else
		  {
			// Type implementation determines which fields are set on the entity
			variableInstanceEntity = VariableInstanceEntity.create(name, type, value);
		  }
		}
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string Operator
	  {
		  get
		  {
			if (@operator != null)
			{
			  return @operator.ToString();
			}
			return QueryOperator.EQUALS.ToString();
		  }
	  }

	  public virtual string TextValue
	  {
		  get
		  {
			if (variableInstanceEntity != null)
			{
			  return variableInstanceEntity.TextValue;
			}
			return null;
		  }
	  }

	  public virtual long? LongValue
	  {
		  get
		  {
			if (variableInstanceEntity != null)
			{
			  return variableInstanceEntity.LongValue;
			}
			return null;
		  }
	  }

	  public virtual double? DoubleValue
	  {
		  get
		  {
			if (variableInstanceEntity != null)
			{
			  return variableInstanceEntity.DoubleValue;
			}
			return null;
		  }
	  }

	  public virtual string TextValue2
	  {
		  get
		  {
			if (variableInstanceEntity != null)
			{
			  return variableInstanceEntity.TextValue2;
			}
			return null;
		  }
	  }

	  public virtual string Type
	  {
		  get
		  {
			if (variableInstanceEntity != null)
			{
			  return variableInstanceEntity.Type.TypeName;
			}
			return null;
		  }
	  }

	  public virtual bool Local
	  {
		  get
		  {
			return local;
		  }
	  }
	}
}