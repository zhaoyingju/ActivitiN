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


	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.activiti.engine.impl.context.Context;
	using ListQueryParameterObject = org.activiti.engine.impl.db.ListQueryParameterObject;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using org.activiti.engine.query;
	using QueryProperty = org.activiti.engine.query.QueryProperty;


	/// <summary>
	/// Abstract superclass for all query types.
	///  
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public abstract class AbstractQuery<T extends org.activiti.engine.query.Query<?,?>, U> extends org.activiti.engine.impl.db.ListQueryParameterObject implements org.activiti.engine.impl.interceptor.Command<Object>, org.activiti.engine.query.Query<T,U>, java.io.Serializable
	public abstract class AbstractQuery<T, U> : ListQueryParameterObject, Command<object>, Query<T, U> where T : org.activiti.engine.query.Query<?,?>
	{

	  private const long serialVersionUID = 1L;

	  public const string SORTORDER_ASC = "asc";
	  public const string SORTORDER_DESC = "desc";

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

	  protected internal new string databaseType;

	  protected internal string orderBy_Renamed;

	  protected internal ResultType resultType;

	  protected internal QueryProperty orderProperty;

	  public enum NullHandlingOnOrder
	  {
		  NULLS_FIRST,
		  NULLS_LAST
	  }

	  protected internal NullHandlingOnOrder nullHandlingOnOrder;

	  protected internal AbstractQuery()
	  {
		parameter = this;
	  }

	  protected internal AbstractQuery(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
	  }

	  public AbstractQuery(CommandContext commandContext)
	  {
		this.commandContext = commandContext;
	  }

	  // To be used by custom queries
	  public AbstractQuery(ManagementService managementService) : this(((ManagementServiceImpl) managementService).CommandExecutor)
	  {
	  }

	  public virtual AbstractQuery<T, U> setCommandExecutor(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
		return this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T orderBy(org.activiti.engine.query.QueryProperty property)
	  public virtual T orderBy(QueryProperty property)
	  {
		this.orderProperty = property;
		return (T) this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T orderBy(org.activiti.engine.query.QueryProperty property, NullHandlingOnOrder nullHandlingOnOrder)
	  public virtual T orderBy(QueryProperty property, NullHandlingOnOrder nullHandlingOnOrder)
	  {
		  orderBy(property);
		  this.nullHandlingOnOrder = nullHandlingOnOrder;
		  return (T) this;
	  }

	  public virtual T asc()
	  {
		return direction(Direction.ASCENDING);
	  }

	  public virtual T desc()
	  {
		return direction(Direction.DESCENDING);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T direction(Direction direction)
	  public virtual T direction(Direction direction)
	  {
		if (orderProperty == null)
		{
		  throw new ActivitiIllegalArgumentException("You should call any of the orderBy methods first before specifying a direction");
		}
		addOrder(orderProperty.Name, direction.Name, nullHandlingOnOrder);
		orderProperty = null;
		nullHandlingOnOrder = null;
		return (T) this;
	  }

	  protected internal virtual void checkQueryOk()
	  {
		if (orderProperty != null)
		{
		  throw new ActivitiIllegalArgumentException("Invalid query: call asc() or desc() after using orderByXX()");
		}
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
		return executeList(Context.CommandContext, null);
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
		return executeList(Context.CommandContext, new Page(firstResult, maxResults));
	  }

	  public virtual long count()
	  {
		this.resultType = ResultType.COUNT;
		if (commandExecutor != null)
		{
		  return (long?) commandExecutor.execute(this);
		}
		return executeCount(Context.CommandContext);
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		if (resultType == ResultType.LIST)
		{
		  return executeList(commandContext, null);
		}
		else if (resultType == ResultType.SINGLE_RESULT)
		{
		  return executeSingleResult(commandContext);
		}
		else if (resultType == ResultType.LIST_PAGE)
		{
		  return executeList(commandContext, null);
		}
		else
		{
		  return executeCount(commandContext);
		}
	  }

	  public abstract long executeCount(CommandContext commandContext);

	  /// <summary>
	  /// Executes the actual query to retrieve the list of results. </summary>
	  /// <param name="page"> used if the results must be paged. If null, no paging will be applied.  </param>
	  public abstract IList<U> executeList(CommandContext commandContext, Page page);

	  public virtual U executeSingleResult(CommandContext commandContext)
	  {
		IList<U> results = executeList(commandContext, null);
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

	  protected internal virtual void addOrder(string column, string sortOrder, NullHandlingOnOrder nullHandlingOnOrder)
	  {

		  if (orderBy_Renamed == null)
		  {
		  orderBy_Renamed = "";
		  }
		else
		{
		  orderBy_Renamed = orderBy_Renamed + ", ";
		}

		  string defaultOrderByClause = column + " " + sortOrder;

		  if (nullHandlingOnOrder != null)
		  {

			  if (nullHandlingOnOrder.Equals(NullHandlingOnOrder.NULLS_FIRST))
			  {

				  if (ProcessEngineConfigurationImpl.DATABASE_TYPE_H2.Equals(databaseType) || ProcessEngineConfigurationImpl.DATABASE_TYPE_HSQL.Equals(databaseType) || ProcessEngineConfigurationImpl.DATABASE_TYPE_POSTGRES.Equals(databaseType) || ProcessEngineConfigurationImpl.DATABASE_TYPE_ORACLE.Equals(databaseType))
				  {
					  orderBy_Renamed = orderBy_Renamed + defaultOrderByClause + " NULLS FIRST";
				  }
				  else if (ProcessEngineConfigurationImpl.DATABASE_TYPE_MYSQL.Equals(databaseType))
				  {
					orderBy_Renamed = orderBy_Renamed + "isnull(" + column + ") desc," + defaultOrderByClause;
				  }
				else if (ProcessEngineConfigurationImpl.DATABASE_TYPE_DB2.Equals(databaseType) || ProcessEngineConfigurationImpl.DATABASE_TYPE_MSSQL.Equals(databaseType))
				{
					orderBy_Renamed = orderBy_Renamed + "case when " + column + " is null then 0 else 1 end," + defaultOrderByClause;
				}
				else
				{
					orderBy_Renamed = orderBy_Renamed + defaultOrderByClause;
				}


			  }
		  else if (nullHandlingOnOrder.Equals(NullHandlingOnOrder.NULLS_LAST))
		  {

				  if (ProcessEngineConfigurationImpl.DATABASE_TYPE_H2.Equals(databaseType) || ProcessEngineConfigurationImpl.DATABASE_TYPE_HSQL.Equals(databaseType) || ProcessEngineConfigurationImpl.DATABASE_TYPE_POSTGRES.Equals(databaseType) || ProcessEngineConfigurationImpl.DATABASE_TYPE_ORACLE.Equals(databaseType))
				  {
					  orderBy_Renamed = orderBy_Renamed + column + " " + sortOrder + " NULLS LAST";
				  }
				  else if (ProcessEngineConfigurationImpl.DATABASE_TYPE_MYSQL.Equals(databaseType))
				  {
					orderBy_Renamed = orderBy_Renamed + "isnull(" + column + ") asc," + defaultOrderByClause;
				  }
				else if (ProcessEngineConfigurationImpl.DATABASE_TYPE_DB2.Equals(databaseType) || ProcessEngineConfigurationImpl.DATABASE_TYPE_MSSQL.Equals(databaseType))
				{
					orderBy_Renamed = orderBy_Renamed + "case when " + column + " is null then 1 else 0 end," + defaultOrderByClause;
				}
				else
				{
					orderBy_Renamed = orderBy_Renamed + defaultOrderByClause;
				}

		  }

		  }
		  else
		  {
			  orderBy_Renamed = orderBy_Renamed + defaultOrderByClause;
		  }

	  }

	  public override string OrderBy
	  {
		  get
		  {
			if (orderBy_Renamed == null)
			{
			  return base.OrderBy;
			}
			else
			{
			  return orderBy_Renamed;
			}
		  }
	  }

	  public override string OrderByColumns
	  {
		  get
		  {
			  return OrderBy;
		  }
	  }

		public override string DatabaseType
		{
			get
			{
				return databaseType;
			}
			set
			{
				this.databaseType = value;
			}
		}


	}

}