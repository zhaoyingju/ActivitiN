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


	using Context = org.activiti.engine.impl.context.Context;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using org.activiti.engine.query;
	using ObjectUtils = org.apache.commons.lang3.ObjectUtils;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	/// <summary>
	/// Abstract superclass for all native query types.
	/// 
	/// @author Bernd Ruecker (camunda)
	/// </summary>
	[Serializable]
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public abstract class AbstractNativeQuery<T extends org.activiti.engine.query.NativeQuery< ?, ? >, U> implements org.activiti.engine.impl.interceptor.Command<Object>, org.activiti.engine.query.NativeQuery<T, U>, java.io.Serializable
	public abstract class AbstractNativeQuery<T, U> : Command<object>, NativeQuery<T, U> where T : org.activiti.engine.query.NativeQuery< ?, ? >
	{

	  private const long serialVersionUID = 1L;

	  private enum ResultType
	  {
		LIST,
		LIST_PAGE,
		SINGLE_RESULT,
		COUNT
	  }

	  [NonSerialized]
	  protected internal CommandExecutor commandExecutor;
	  [NonSerialized]
	  protected internal CommandContext commandContext;

	  protected internal int maxResults = int.MaxValue;
	  protected internal int firstResult = 0;
	  protected internal ResultType resultType;

	  private IDictionary<string, object> parameters = new Dictionary<string, object>();
	  private string sqlStatement;

	  protected internal AbstractNativeQuery(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
	  }

	  public AbstractNativeQuery(CommandContext commandContext)
	  {
		this.commandContext = commandContext;
	  }

	  public virtual AbstractNativeQuery<T, U> setCommandExecutor(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
		return this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T sql(String sqlStatement)
	  public virtual T sql(string sqlStatement)
	  {
		this.sqlStatement = sqlStatement;
		return (T) this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T parameter(String name, Object value)
	  public virtual T parameter(string name, object value)
	  {
		parameters[name] = value;
		return (T) this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public U singleResult()
	  public virtual U singleResult()
	  {
		this.resultType = ResultType.SINGLE_RESULT;
		if (commandExecutor != null)
		{
		  return (U) commandExecutor.execute(this);
		}
		return executeSingleResult(Context.CommandContext);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<U> list()
	  public virtual IList<U> list()
	  {
		this.resultType = ResultType.LIST;
		if (commandExecutor != null)
		{
		  return (IList<U>) commandExecutor.execute(this);
		}
		return executeList(Context.CommandContext, ParameterMap, 0, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<U> listPage(int firstResult, int maxResults)
	  public virtual IList<U> listPage(int firstResult, int maxResults)
	  {
		this.firstResult = firstResult;
		this.maxResults = maxResults;
		this.resultType = ResultType.LIST_PAGE;
		if (commandExecutor != null)
		{
		  return (IList<U>) commandExecutor.execute(this);
		}
		return executeList(Context.CommandContext, ParameterMap, firstResult, maxResults);
	  }

	  public virtual long count()
	  {
		this.resultType = ResultType.COUNT;
		if (commandExecutor != null)
		{
		  return (long?) commandExecutor.execute(this);
		}
		return executeCount(Context.CommandContext, ParameterMap);
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		if (resultType == ResultType.LIST)
		{
		  return executeList(commandContext, ParameterMap, 0, int.MaxValue);

		}
		else if (resultType == ResultType.LIST_PAGE)
		{
		  IDictionary<string, object> parameterMap = ParameterMap;
		  parameterMap["resultType"] = "LIST_PAGE";
		  parameterMap["firstResult"] = firstResult;
		  parameterMap["maxResults"] = maxResults;
		  if (StringUtils.isNotBlank(ObjectUtils.ToString(parameterMap["orderBy"])))
		  {
			parameterMap["orderByColumns"] = "RES." + parameterMap["orderBy"];
		  }
		  else
		  {
			parameterMap["orderByColumns"] = "RES.ID_ asc";
		  }

		  int firstRow = firstResult + 1;
		  parameterMap["firstRow"] = firstRow;
		  int lastRow = 0;
		  if (maxResults == int.MaxValue)
		  {
			lastRow = maxResults;
		  }
		  else
		  {
			lastRow = firstResult + maxResults + 1;
		  }
		  parameterMap["lastRow"] = lastRow;
		  return executeList(commandContext, parameterMap, firstResult, maxResults);
		}
		else if (resultType == ResultType.SINGLE_RESULT)
		{
		  return executeSingleResult(commandContext);
		}
		else
		{
		  return executeCount(commandContext, ParameterMap);
		}
	  }

	  public abstract long executeCount(CommandContext commandContext, IDictionary<string, object> parameterMap);

	  /// <summary>
	  /// Executes the actual query to retrieve the list of results. </summary>
	  /// <param name="maxResults"> </param>
	  /// <param name="firstResult"> 
	  /// </param>
	  /// <param name="page">
	  ///          used if the results must be paged. If null, no paging will be
	  ///          applied. </param>
	  public abstract IList<U> executeList(CommandContext commandContext, IDictionary<string, object> parameterMap, int firstResult, int maxResults);

	  public virtual U executeSingleResult(CommandContext commandContext)
	  {
		IList<U> results = executeList(commandContext, ParameterMap, 0, int.MaxValue);
		if (results.Count == 1)
		{
		  return results[0];
		}
		else if (results.Count > 1)
		{
		  throw new ActivitiException("Query return " + results.Count + " results instead of max 1");
		}
		return null;
	  }

	  private IDictionary<string, object> ParameterMap
	  {
		  get
		  {
			Dictionary<string, object> parameterMap = new Dictionary<string, object>();
			parameterMap["sql"] = sqlStatement;
	//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			parameterMap.putAll(parameters);
			return parameterMap;
		  }
	  }

	  public virtual IDictionary<string, object> Parameters
	  {
		  get
		  {
			return parameters;
		  }
	  }

	}

}