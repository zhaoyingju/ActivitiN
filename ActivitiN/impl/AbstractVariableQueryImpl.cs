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


	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using VariableTypes = org.activiti.engine.impl.variable.VariableTypes;
	using org.activiti.engine.query;


	/// <summary>
	/// Abstract query class that adds methods to query for variable values.
	/// 
	/// @author Frederik Heremans
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public abstract class AbstractVariableQueryImpl<T extends org.activiti.engine.query.Query<?,?>, U> extends AbstractQuery<T, U>
	public abstract class AbstractVariableQueryImpl<T, U> : AbstractQuery<T, U> where T : org.activiti.engine.query.Query<?,?>
	{

	  private const long serialVersionUID = 1L;

	  protected internal IList<QueryVariableValue> queryVariableValues = new List<QueryVariableValue>();

	  public AbstractVariableQueryImpl()
	  {
	  }

	  public AbstractVariableQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public AbstractVariableQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public override abstract long executeCount(CommandContext commandContext);

	  public override abstract IList<U> executeList(CommandContext commandContext, Page page);

	  public virtual T variableValueEquals(string name, object value)
	  {
		return variableValueEquals(name, value, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected T variableValueEquals(String name, Object value, boolean localScope)
	  protected internal virtual T variableValueEquals(string name, object value, bool localScope)
	  {
		addVariable(name, value, QueryOperator.EQUALS, localScope);
		return (T) this;
	  }

	  public virtual T variableValueEquals(object value)
	  {
		return variableValueEquals(value, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected T variableValueEquals(Object value, boolean localScope)
	  protected internal virtual T variableValueEquals(object value, bool localScope)
	  {
		queryVariableValues.Add(new QueryVariableValue(null, value, QueryOperator.EQUALS, localScope));
		return (T) this;
	  }

	  public virtual T variableValueEqualsIgnoreCase(string name, string value)
	  {
		return variableValueEqualsIgnoreCase(name, value, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected T variableValueEqualsIgnoreCase(String name, String value, boolean localScope)
	  protected internal virtual T variableValueEqualsIgnoreCase(string name, string value, bool localScope)
	  {
		if (value == null)
		{
		  throw new ActivitiIllegalArgumentException("value is null");
		}
		addVariable(name, value.ToLower(), QueryOperator.EQUALS_IGNORE_CASE, localScope);
		return (T)this;
	  }

	  public virtual T variableValueNotEqualsIgnoreCase(string name, string value)
	  {
		return variableValueNotEqualsIgnoreCase(name, value, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected T variableValueNotEqualsIgnoreCase(String name, String value, boolean localScope)
	  protected internal virtual T variableValueNotEqualsIgnoreCase(string name, string value, bool localScope)
	  {
		if (value == null)
		{
		  throw new ActivitiIllegalArgumentException("value is null");
		}
		addVariable(name, value.ToLower(), QueryOperator.NOT_EQUALS_IGNORE_CASE, localScope);
		return (T)this;
	  }

	  public virtual T variableValueNotEquals(string name, object value)
	  {
		return variableValueNotEquals(name, value, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected T variableValueNotEquals(String name, Object value, boolean localScope)
	  protected internal virtual T variableValueNotEquals(string name, object value, bool localScope)
	  {
		addVariable(name, value, QueryOperator.NOT_EQUALS, localScope);
		return (T) this;
	  }

	  public virtual T variableValueGreaterThan(string name, object value)
	  {
		return variableValueGreaterThan(name, value, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected T variableValueGreaterThan(String name, Object value, boolean localScope)
	  protected internal virtual T variableValueGreaterThan(string name, object value, bool localScope)
	  {
		addVariable(name, value, QueryOperator.GREATER_THAN, localScope);
		return (T) this;
	  }

	  public virtual T variableValueGreaterThanOrEqual(string name, object value)
	  {
		return variableValueGreaterThanOrEqual(name, value, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected T variableValueGreaterThanOrEqual(String name, Object value, boolean localScope)
	  protected internal virtual T variableValueGreaterThanOrEqual(string name, object value, bool localScope)
	  {
		addVariable(name, value, QueryOperator.GREATER_THAN_OR_EQUAL, localScope);
		return (T) this;
	  }

	  public virtual T variableValueLessThan(string name, object value)
	  {
		return variableValueLessThan(name, value, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected T variableValueLessThan(String name, Object value, boolean localScope)
	  protected internal virtual T variableValueLessThan(string name, object value, bool localScope)
	  {
		addVariable(name, value, QueryOperator.LESS_THAN, localScope);
		return (T) this;
	  }

	  public virtual T variableValueLessThanOrEqual(string name, object value)
	  {
		return variableValueLessThanOrEqual(name, value, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected T variableValueLessThanOrEqual(String name, Object value, boolean localScope)
	  protected internal virtual T variableValueLessThanOrEqual(string name, object value, bool localScope)
	  {
		addVariable(name, value, QueryOperator.LESS_THAN_OR_EQUAL, localScope);
		return (T) this;
	  }

	  public virtual T variableValueLike(string name, string value)
	  {
		return variableValueLike(name, value, true);
	  }

	  public virtual T variableValueLikeIgnoreCase(string name, string value)
	  {
		return variableValueLikeIgnoreCase(name, value, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected T variableValueLike(String name, String value, boolean localScope)
	  protected internal virtual T variableValueLike(string name, string value, bool localScope)
	  {
		addVariable(name, value, QueryOperator.LIKE, localScope);
		return (T) this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected T variableValueLikeIgnoreCase(String name, String value, boolean localScope)
	  protected internal virtual T variableValueLikeIgnoreCase(string name, string value, bool localScope)
	  {
		addVariable(name, value.ToLower(), QueryOperator.LIKE_IGNORE_CASE, localScope);
		return (T) this;
	  }

	  protected internal virtual void addVariable(string name, object value, QueryOperator @operator, bool localScope)
	  {
		if (name == null)
		{
		  throw new ActivitiIllegalArgumentException("name is null");
		}
		if (value == null || isBoolean(value))
		{
		  // Null-values and booleans can only be used in EQUALS and NOT_EQUALS
		  switch (@operator)
		  {
		  case org.activiti.engine.impl.QueryOperator.GREATER_THAN:
			throw new ActivitiIllegalArgumentException("Booleans and null cannot be used in 'greater than' condition");
		  case org.activiti.engine.impl.QueryOperator.LESS_THAN:
			throw new ActivitiIllegalArgumentException("Booleans and null cannot be used in 'less than' condition");
		  case org.activiti.engine.impl.QueryOperator.GREATER_THAN_OR_EQUAL:
			throw new ActivitiIllegalArgumentException("Booleans and null cannot be used in 'greater than or equal' condition");
		  case org.activiti.engine.impl.QueryOperator.LESS_THAN_OR_EQUAL:
			throw new ActivitiIllegalArgumentException("Booleans and null cannot be used in 'less than or equal' condition");
		  }

		  if (@operator == QueryOperator.EQUALS_IGNORE_CASE && !(value is string))
		  {
			throw new ActivitiIllegalArgumentException("Only string values can be used with 'equals ignore case' condition");
		  }

		  if (@operator == QueryOperator.NOT_EQUALS_IGNORE_CASE && !(value is string))
		  {
			throw new ActivitiIllegalArgumentException("Only string values can be used with 'not equals ignore case' condition");
		  }

		  if ((@operator == QueryOperator.LIKE || @operator == QueryOperator.LIKE_IGNORE_CASE) && !(value is string))
		  {
			throw new ActivitiIllegalArgumentException("Only string values can be used with 'like' condition");
		  }
		}
		queryVariableValues.Add(new QueryVariableValue(name, value, @operator, localScope));
	  }

	  protected internal virtual bool isBoolean(object value)
	  {
		if (value == null)
		{
		  return false;
		}
		return value.GetType().IsSubclassOf(typeof(bool?)) || value.GetType().IsSubclassOf(typeof(bool));
	  }

	  protected internal virtual void ensureVariablesInitialized()
	  {
		if (queryVariableValues.Count > 0)
		{
		  VariableTypes variableTypes = Context.ProcessEngineConfiguration.VariableTypes;
		  foreach (QueryVariableValue queryVariableValue in queryVariableValues)
		  {
			queryVariableValue.initialize(variableTypes);
		  }
		}
	  }

	  public virtual IList<QueryVariableValue> QueryVariableValues
	  {
		  get
		  {
			return queryVariableValues;
		  }
	  }

	  public virtual bool hasLocalQueryVariableValue()
	  {
		foreach (QueryVariableValue qvv in queryVariableValues)
		{
			if (qvv.Local)
			{
				return true;
			}
		}
		return false;
	  }

	  public virtual bool hasNonLocalQueryVariableValue()
	  {
		foreach (QueryVariableValue qvv in queryVariableValues)
		{
			if (!qvv.Local)
			{
				return true;
			}
		}
		return false;
	  }

	}

}