using System;
using System.Collections;
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

namespace org.activiti.engine.impl.persistence.entity
{


	using HistoricActivityInstance = org.activiti.engine.history.HistoricActivityInstance;
	using HistoricDetail = org.activiti.engine.history.HistoricDetail;
	using HistoricFormProperty = org.activiti.engine.history.HistoricFormProperty;
	using HistoricProcessInstance = org.activiti.engine.history.HistoricProcessInstance;
	using HistoricTaskInstance = org.activiti.engine.history.HistoricTaskInstance;
	using HistoricVariableInstance = org.activiti.engine.history.HistoricVariableInstance;
	using HistoricVariableUpdate = org.activiti.engine.history.HistoricVariableUpdate;
	using Group = org.activiti.engine.identity.Group;
	using User = org.activiti.engine.identity.User;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using TableMetaData = org.activiti.engine.management.TableMetaData;
	using TablePage = org.activiti.engine.management.TablePage;
	using Deployment = org.activiti.engine.repository.Deployment;
	using Model = org.activiti.engine.repository.Model;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using Execution = org.activiti.engine.runtime.Execution;
	using Job = org.activiti.engine.runtime.Job;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;
	using Task = org.activiti.engine.task.Task;
	using RowBounds = org.apache.ibatis.session.RowBounds;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class TableDataManager : AbstractManager
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(TableDataManager));

	  public static IDictionary<Type, string> apiTypeToTableNameMap = new Dictionary<Type, string>();
	  public static IDictionary<Type, string> persistentObjectToTableNameMap = new Dictionary<Type, string>();

	  static TableDataManager()
	  {
		// runtime
		persistentObjectToTableNameMap[typeof(TaskEntity)] = "ACT_RU_TASK";
		persistentObjectToTableNameMap[typeof(ExecutionEntity)] = "ACT_RU_EXECUTION";
		persistentObjectToTableNameMap[typeof(IdentityLinkEntity)] = "ACT_RU_IDENTITYLINK";
		persistentObjectToTableNameMap[typeof(VariableInstanceEntity)] = "ACT_RU_VARIABLE";

		persistentObjectToTableNameMap[typeof(JobEntity)] = "ACT_RU_JOB";
		persistentObjectToTableNameMap[typeof(MessageEntity)] = "ACT_RU_JOB";
		persistentObjectToTableNameMap[typeof(TimerEntity)] = "ACT_RU_JOB";

		persistentObjectToTableNameMap[typeof(EventSubscriptionEntity)] = "ACT_RU_EVENT_SUBSCR";
		persistentObjectToTableNameMap[typeof(CompensateEventSubscriptionEntity)] = "ACT_RU_EVENT_SUBSCR";
		persistentObjectToTableNameMap[typeof(MessageEventSubscriptionEntity)] = "ACT_RU_EVENT_SUBSCR";
		persistentObjectToTableNameMap[typeof(SignalEventSubscriptionEntity)] = "ACT_RU_EVENT_SUBSCR";

		// repository
		persistentObjectToTableNameMap[typeof(DeploymentEntity)] = "ACT_RE_DEPLOYMENT";
		persistentObjectToTableNameMap[typeof(ProcessDefinitionEntity)] = "ACT_RE_PROCDEF";
		persistentObjectToTableNameMap[typeof(ModelEntity)] = "ACT_RE_MODEL";

		// history
		persistentObjectToTableNameMap[typeof(CommentEntity)] = "ACT_HI_COMMENT";

		persistentObjectToTableNameMap[typeof(HistoricActivityInstanceEntity)] = "ACT_HI_ACTINST";
		persistentObjectToTableNameMap[typeof(AttachmentEntity)] = "ACT_HI_ATTACHMEN";
		persistentObjectToTableNameMap[typeof(HistoricProcessInstanceEntity)] = "ACT_HI_PROCINST";
		persistentObjectToTableNameMap[typeof(HistoricVariableInstanceEntity)] = "ACT_HI_VARINST";
		persistentObjectToTableNameMap[typeof(HistoricTaskInstanceEntity)] = "ACT_HI_TASKINST";
		persistentObjectToTableNameMap[typeof(HistoricIdentityLinkEntity)] = "ACT_HI_IDENTITYLINK";

		// a couple of stuff goes to the same table
		persistentObjectToTableNameMap[typeof(HistoricDetailAssignmentEntity)] = "ACT_HI_DETAIL";
		persistentObjectToTableNameMap[typeof(HistoricDetailTransitionInstanceEntity)] = "ACT_HI_DETAIL";
		persistentObjectToTableNameMap[typeof(HistoricFormPropertyEntity)] = "ACT_HI_DETAIL";
		persistentObjectToTableNameMap[typeof(HistoricDetailVariableInstanceUpdateEntity)] = "ACT_HI_DETAIL";
		persistentObjectToTableNameMap[typeof(HistoricDetailEntity)] = "ACT_HI_DETAIL";


		// Identity module
		persistentObjectToTableNameMap[typeof(GroupEntity)] = "ACT_ID_GROUP";
		persistentObjectToTableNameMap[typeof(MembershipEntity)] = "ACT_ID_MEMBERSHIP";
		persistentObjectToTableNameMap[typeof(UserEntity)] = "ACT_ID_USER";
		persistentObjectToTableNameMap[typeof(IdentityInfoEntity)] = "ACT_ID_INFO";

		// general
		persistentObjectToTableNameMap[typeof(PropertyEntity)] = "ACT_GE_PROPERTY";
		persistentObjectToTableNameMap[typeof(ByteArrayEntity)] = "ACT_GE_BYTEARRAY";
		persistentObjectToTableNameMap[typeof(ResourceEntity)] = "ACT_GE_BYTEARRAY";

		// and now the map for the API types (does not cover all cases)
		apiTypeToTableNameMap[typeof(Task)] = "ACT_RU_TASK";
		apiTypeToTableNameMap[typeof(Execution)] = "ACT_RU_EXECUTION";
		apiTypeToTableNameMap[typeof(ProcessInstance)] = "ACT_RU_EXECUTION";
		apiTypeToTableNameMap[typeof(ProcessDefinition)] = "ACT_RE_PROCDEF";
		apiTypeToTableNameMap[typeof(Deployment)] = "ACT_RE_DEPLOYMENT";
		apiTypeToTableNameMap[typeof(Job)] = "ACT_RU_JOB";
		apiTypeToTableNameMap[typeof(Model)] = "ACT_RE_MODEL";

		// history
		apiTypeToTableNameMap[typeof(HistoricProcessInstance)] = "ACT_HI_PROCINST";
		apiTypeToTableNameMap[typeof(HistoricActivityInstance)] = "ACT_HI_ACTINST";
		apiTypeToTableNameMap[typeof(HistoricDetail)] = "ACT_HI_DETAIL";
		apiTypeToTableNameMap[typeof(HistoricVariableUpdate)] = "ACT_HI_DETAIL";
		apiTypeToTableNameMap[typeof(HistoricFormProperty)] = "ACT_HI_DETAIL";
		apiTypeToTableNameMap[typeof(HistoricTaskInstance)] = "ACT_HI_TASKINST";
		apiTypeToTableNameMap[typeof(HistoricVariableInstance)] = "ACT_HI_VARINST";

		// identity
		apiTypeToTableNameMap[typeof(Group)] = "ACT_ID_GROUP";
		apiTypeToTableNameMap[typeof(User)] = "ACT_ID_USER";

		// TODO: Identity skipped for the moment as no SQL injection is provided here
	  }

	  public virtual IDictionary<string, long?> TableCount
	  {
		  get
		  {
			IDictionary<string, long?> tableCount = new Dictionary<string, long?>();
			try
			{
			  foreach (string tableName in TablesPresentInDatabase)
			  {
				tableCount[tableName] = getTableCount(tableName);
			  }
			  log.debug("Number of rows per activiti table: {}", tableCount);
			}
			catch (Exception e)
			{
			  throw new ActivitiException("couldn't get table counts", e);
			}
			return tableCount;
		  }
	  }

	  public virtual IList<string> TablesPresentInDatabase
	  {
		  get
		  {
			IList<string> tableNames = new List<string>();
			Connection connection = null;
			try
			{
			  connection = DbSqlSession.SqlSession.Connection;
			  DatabaseMetaData databaseMetaData = connection.MetaData;
			  ResultSet tables = null;
			  try
			  {
				log.debug("retrieving activiti tables from jdbc metadata");
				string databaseTablePrefix = DbSqlSession.DbSqlSessionFactory.DatabaseTablePrefix;
				string tableNameFilter = databaseTablePrefix + "ACT_%";
				if ("postgres".Equals(DbSqlSession.DbSqlSessionFactory.DatabaseType))
				{
				  tableNameFilter = databaseTablePrefix + "act_%";
				}
				if ("oracle".Equals(DbSqlSession.DbSqlSessionFactory.DatabaseType))
				{
				  tableNameFilter = databaseTablePrefix + "ACT" + databaseMetaData.SearchStringEscape + "_%";
				}
    
				string catalog = null;
				if (ProcessEngineConfiguration.DatabaseCatalog != null && ProcessEngineConfiguration.DatabaseCatalog.Length > 0)
				{
				  catalog = ProcessEngineConfiguration.DatabaseCatalog;
				}
    
				string schema = null;
				if (ProcessEngineConfiguration.DatabaseSchema != null && ProcessEngineConfiguration.DatabaseSchema.Length > 0)
				{
				  if ("oracle".Equals(DbSqlSession.DbSqlSessionFactory.DatabaseType))
				  {
					schema = ProcessEngineConfiguration.DatabaseSchema.ToUpper();
				  }
				  else
				  {
					schema = ProcessEngineConfiguration.DatabaseSchema;
				  }
				}
    
				tables = databaseMetaData.getTables(catalog, schema, tableNameFilter, DbSqlSession.JDBC_METADATA_TABLE_TYPES);
				while (tables.next())
				{
				  string tableName = tables.getString("TABLE_NAME");
				  tableName = tableName.ToUpper();
				  tableNames.Add(tableName);
				  log.debug("  retrieved activiti table name {}", tableName);
				}
			  }
			  finally
			  {
				tables.close();
			  }
			}
			catch (Exception e)
			{
			  throw new ActivitiException("couldn't get activiti table names using metadata: " + e.Message, e);
			}
			return tableNames;
		  }
	  }

	  protected internal virtual long getTableCount(string tableName)
	  {
		log.debug("selecting table count for {}", tableName);
		long? count = (long?) DbSqlSession.selectOne("selectTableCount", Collections.singletonMap("tableName", tableName));
		return count;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public org.activiti.engine.management.TablePage getTablePage(org.activiti.engine.impl.TablePageQueryImpl tablePageQuery, int firstResult, int maxResults)
	  public virtual TablePage getTablePage(TablePageQueryImpl tablePageQuery, int firstResult, int maxResults)
	  {

		TablePage tablePage = new TablePage();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") java.util.List tableData = getDbSqlSession().getSqlSession().selectList("selectTableData", tablePageQuery, new org.apache.ibatis.session.RowBounds(firstResult, maxResults));
		IList tableData = DbSqlSession.SqlSession.selectList("selectTableData", tablePageQuery, new RowBounds(firstResult, maxResults));

		tablePage.TableName = tablePageQuery.TableName;
		tablePage.Total = getTableCount(tablePageQuery.TableName);
		tablePage.Rows = (IList<IDictionary<string, object>>)tableData;
		tablePage.FirstResult = firstResult;

		return tablePage;
	  }

	  public virtual string getTableName(Type entityClass, bool withPrefix)
	  {
		string databaseTablePrefix = DbSqlSession.DbSqlSessionFactory.DatabaseTablePrefix;
		string tableName = null;

		if (entityClass.IsSubclassOf(typeof(PersistentObject)))
		{
		  tableName = persistentObjectToTableNameMap[entityClass];
		}
		else
		{
		  tableName = apiTypeToTableNameMap[entityClass];
		}
		if (withPrefix)
		{
		  return databaseTablePrefix + tableName;
		}
		else
		{
		  return tableName;
		}
	  }

	  public virtual TableMetaData getTableMetaData(string tableName)
	  {
		TableMetaData result = new TableMetaData();
		try
		{
		  result.TableName = tableName;
		  DatabaseMetaData metaData = DbSqlSession.SqlSession.Connection.MetaData;

		  if ("postgres".Equals(DbSqlSession.DbSqlSessionFactory.DatabaseType))
		  {
			tableName = tableName.ToLower();
		  }

		  string catalog = null;
		  if (ProcessEngineConfiguration.DatabaseCatalog != null && ProcessEngineConfiguration.DatabaseCatalog.Length > 0)
		  {
			catalog = ProcessEngineConfiguration.DatabaseCatalog;
		  }

		  string schema = null;
		  if (ProcessEngineConfiguration.DatabaseSchema != null && ProcessEngineConfiguration.DatabaseSchema.Length > 0)
		  {
			if ("oracle".Equals(DbSqlSession.DbSqlSessionFactory.DatabaseType))
			{
			  schema = ProcessEngineConfiguration.DatabaseSchema.ToUpper();
			}
			else
			{
			  schema = ProcessEngineConfiguration.DatabaseSchema;
			}
		  }

		  ResultSet resultSet = metaData.getColumns(catalog, schema, tableName, null);
		  while (resultSet.next())
		  {
			bool wrongSchema = false;
			if (schema != null && schema.Length > 0)
			{
			  for (int i = 0; i < resultSet.MetaData.ColumnCount; i++)
			  {
				string columnName = resultSet.MetaData.getColumnName(i + 1);
				if ("TABLE_SCHEM".Equals(columnName, StringComparison.CurrentCultureIgnoreCase) || "TABLE_SCHEMA".Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
				{
				  if (schema.Equals(resultSet.getString(resultSet.MetaData.getColumnName(i + 1)), StringComparison.CurrentCultureIgnoreCase) == false)
				  {
					wrongSchema = true;
				  }
				  break;
				}
			  }
			}

			if (wrongSchema == false)
			{
			  string name = resultSet.getString("COLUMN_NAME").ToUpper();
			  string type = resultSet.getString("TYPE_NAME").ToUpper();
			  result.addColumnMetaData(name, type);
			}
		  }

		}
		catch (SQLException e)
		{
		  throw new ActivitiException("Could not retrieve database metadata: " + e.Message);
		}

		if (result.ColumnNames.Count == 0)
		{
		  // According to API, when a table doesn't exist, null should be returned
		  result = null;
		}
		return result;
	  }

	}

}