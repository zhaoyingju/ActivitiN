using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

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

namespace org.activiti.engine.impl.db
{


	using IdGenerator = org.activiti.engine.impl.cfg.IdGenerator;
	using Session = org.activiti.engine.impl.interceptor.Session;
	using SessionFactory = org.activiti.engine.impl.interceptor.SessionFactory;
	using EventLogEntryEntity = org.activiti.engine.impl.persistence.entity.EventLogEntryEntity;
	using SqlSessionFactory = org.apache.ibatis.session.SqlSessionFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DbSqlSessionFactory : SessionFactory
	{

	  protected internal static readonly IDictionary<string, IDictionary<string, string>> databaseSpecificStatements = new Dictionary<string, IDictionary<string, string>>();

	  public static readonly IDictionary<string, string> databaseSpecificLimitBeforeStatements = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificLimitAfterStatements = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificLimitBetweenStatements = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificOrderByStatements = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseOuterJoinLimitBetweenStatements = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificLimitBeforeNativeQueryStatements = new Dictionary<string, string>();

	  static DbSqlSessionFactory()
	  {

		string defaultOrderBy = " order by ${orderByColumns} ";

		// h2
		databaseSpecificLimitBeforeStatements["h2"] = "";
		databaseSpecificLimitAfterStatements["h2"] = "LIMIT #{maxResults} OFFSET #{firstResult}";
		databaseSpecificLimitBetweenStatements["h2"] = "";
		databaseOuterJoinLimitBetweenStatements["h2"] = "";
		databaseSpecificOrderByStatements["h2"] = defaultOrderBy;

		// hsql
		databaseSpecificLimitBeforeStatements["hsql"] = "";
		databaseSpecificLimitAfterStatements["hsql"] = "LIMIT #{maxResults} OFFSET #{firstResult}";
		databaseSpecificLimitBetweenStatements["hsql"] = "";
		databaseOuterJoinLimitBetweenStatements["hsql"] = "";
		databaseSpecificOrderByStatements["hsql"] = defaultOrderBy;


		  //mysql specific
		databaseSpecificLimitBeforeStatements["mysql"] = "";
		databaseSpecificLimitAfterStatements["mysql"] = "LIMIT #{maxResults} OFFSET #{firstResult}";
		databaseSpecificLimitBetweenStatements["mysql"] = "";
		databaseOuterJoinLimitBetweenStatements["mysql"] = "";
		databaseSpecificOrderByStatements["mysql"] = defaultOrderBy;
		addDatabaseSpecificStatement("mysql", "selectProcessDefinitionsByQueryCriteria", "selectProcessDefinitionsByQueryCriteria_mysql");
		addDatabaseSpecificStatement("mysql", "selectProcessDefinitionCountByQueryCriteria", "selectProcessDefinitionCountByQueryCriteria_mysql");
		addDatabaseSpecificStatement("mysql", "selectDeploymentsByQueryCriteria", "selectDeploymentsByQueryCriteria_mysql");
		addDatabaseSpecificStatement("mysql", "selectDeploymentCountByQueryCriteria", "selectDeploymentCountByQueryCriteria_mysql");
		addDatabaseSpecificStatement("mysql", "selectModelCountByQueryCriteria", "selectModelCountByQueryCriteria_mysql");
		addDatabaseSpecificStatement("mysql", "updateExecutionTenantIdForDeployment", "updateExecutionTenantIdForDeployment_mysql");
		addDatabaseSpecificStatement("mysql", "updateTaskTenantIdForDeployment", "updateTaskTenantIdForDeployment_mysql");
		addDatabaseSpecificStatement("mysql", "updateJobTenantIdForDeployment", "updateJobTenantIdForDeployment_mysql");

		//postgres specific
		databaseSpecificLimitBeforeStatements["postgres"] = "";
		databaseSpecificLimitAfterStatements["postgres"] = "LIMIT #{maxResults} OFFSET #{firstResult}";
		databaseSpecificLimitBetweenStatements["postgres"] = "";
		databaseOuterJoinLimitBetweenStatements["postgres"] = "";
		databaseSpecificOrderByStatements["postgres"] = defaultOrderBy;
		addDatabaseSpecificStatement("postgres", "insertByteArray", "insertByteArray_postgres");
		addDatabaseSpecificStatement("postgres", "bulkInsertByteArray", "bulkInsertByteArray_postgres");
		addDatabaseSpecificStatement("postgres", "updateByteArray", "updateByteArray_postgres");
		addDatabaseSpecificStatement("postgres", "selectByteArray", "selectByteArray_postgres");
		addDatabaseSpecificStatement("postgres", "selectResourceByDeploymentIdAndResourceName", "selectResourceByDeploymentIdAndResourceName_postgres");
		addDatabaseSpecificStatement("postgres", "selectResourcesByDeploymentId", "selectResourcesByDeploymentId_postgres");
		addDatabaseSpecificStatement("postgres", "insertIdentityInfo", "insertIdentityInfo_postgres");
		addDatabaseSpecificStatement("postgres", "bulkInsertIdentityInfo", "bulkInsertIdentityInfo_postgres");
		addDatabaseSpecificStatement("postgres", "updateIdentityInfo", "updateIdentityInfo_postgres");
		addDatabaseSpecificStatement("postgres", "selectIdentityInfoById", "selectIdentityInfoById_postgres");
		addDatabaseSpecificStatement("postgres", "selectIdentityInfoByUserIdAndKey", "selectIdentityInfoByUserIdAndKey_postgres");
		addDatabaseSpecificStatement("postgres", "selectIdentityInfoByUserId", "selectIdentityInfoByUserId_postgres");
		addDatabaseSpecificStatement("postgres", "selectIdentityInfoDetails", "selectIdentityInfoDetails_postgres");
		addDatabaseSpecificStatement("postgres", "insertComment", "insertComment_postgres");
		addDatabaseSpecificStatement("postgres", "bulkInsertComment", "bulkInsertComment_postgres");
		addDatabaseSpecificStatement("postgres", "selectComment", "selectComment_postgres");
		addDatabaseSpecificStatement("postgres", "selectCommentsByTaskId", "selectCommentsByTaskId_postgres");
		addDatabaseSpecificStatement("postgres", "selectCommentsByProcessInstanceId", "selectCommentsByProcessInstanceId_postgres");
		addDatabaseSpecificStatement("postgres", "selectCommentsByProcessInstanceIdAndType", "selectCommentsByProcessInstanceIdAndType_postgres");
		addDatabaseSpecificStatement("postgres", "selectCommentsByType", "selectCommentsByType_postgres");
		addDatabaseSpecificStatement("postgres", "selectCommentsByTaskIdAndType", "selectCommentsByTaskIdAndType_postgres");
		addDatabaseSpecificStatement("postgres", "selectEventsByTaskId", "selectEventsByTaskId_postgres");
		addDatabaseSpecificStatement("postgres", "insertEventLogEntry", "insertEventLogEntry_postgres");
		addDatabaseSpecificStatement("postgres", "bulkInsertEventLogEntry", "bulkInsertEventLogEntry_postgres");
		addDatabaseSpecificStatement("postgres", "selectAllEventLogEntries", "selectAllEventLogEntries_postgres");
		addDatabaseSpecificStatement("postgres", "selectEventLogEntries", "selectEventLogEntries_postgres");
		addDatabaseSpecificStatement("postgres", "selectEventLogEntriesByProcessInstanceId", "selectEventLogEntriesByProcessInstanceId_postgres");

		// oracle
		databaseSpecificLimitBeforeStatements["oracle"] = "select * from ( select a.*, ROWNUM rnum from (";
		databaseSpecificLimitAfterStatements["oracle"] = "  ) a where ROWNUM < #{lastRow}) where rnum  >= #{firstRow}";
		databaseSpecificLimitBetweenStatements["oracle"] = "";
		databaseOuterJoinLimitBetweenStatements["oracle"] = "";
		databaseSpecificOrderByStatements["oracle"] = defaultOrderBy;
		addDatabaseSpecificStatement("oracle", "selectExclusiveJobsToExecute", "selectExclusiveJobsToExecute_integerBoolean");
		addDatabaseSpecificStatement("oracle", "selectUnlockedTimersByDuedate", "selectUnlockedTimersByDuedate_oracle");
		addDatabaseSpecificStatement("oracle", "insertEventLogEntry", "insertEventLogEntry_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertVariableInstance", "bulkInsertVariableInstance_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertUser", "bulkInsertUser_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertTask", "bulkInsertTask_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertResource", "bulkInsertResource_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertProperty", "bulkInsertProperty_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertProcessDefinition", "bulkInsertProcessDefinition_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertModel", "bulkInsertModel_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertMembership", "bulkInsertMembership_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertTimer", "bulkInsertTimer_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertMessage", "bulkInsertMessage_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertIdentityInfo", "bulkInsertIdentityInfo_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertIdentityLink", "bulkInsertIdentityLink_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertMembership", "bulkInsertMembership_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertTimer", "bulkInsertTimer_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertMessage", "bulkInsertMessage_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertHistoricVariableInstance", "bulkInsertHistoricVariableInstance_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertHistoricTaskInstance", "bulkInsertHistoricTaskInstance_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertHistoricProcessInstance", "bulkInsertHistoricProcessInstance_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertHistoricIdentityLink", "bulkInsertHistoricIdentityLink_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertHistoricDetailVariableInstanceUpdate", "bulkInsertHistoricDetailVariableInstanceUpdate_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertHistoricFormProperty", "bulkInsertHistoricFormProperty_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertHistoricActivityInstance", "bulkInsertHistoricActivityInstance_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertGroup", "bulkInsertGroup_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertExecution", "bulkInsertExecution_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertMessageEventSubscription", "bulkInsertMessageEventSubscription_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertSignalEventSubscription", "bulkInsertSignalEventSubscription_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertCompensateEventSubscription", "bulkInsertCompensateEventSubscription_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertEventLogEntry", "bulkInsertEventLogEntry_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertDeployment", "bulkInsertDeployment_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertComment", "bulkInsertComment_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertByteArray", "bulkInsertByteArray_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertEventLogEntry", "bulkInsertEventLogEntry_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertDeployment", "bulkInsertDeployment_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertComment", "bulkInsertComment_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertByteArray", "bulkInsertByteArray_oracle");
		addDatabaseSpecificStatement("oracle", "bulkInsertAttachment", "bulkInsertAttachment_oracle");

		// db2
		databaseSpecificLimitBeforeStatements["db2"] = "SELECT SUB.* FROM (";
		databaseSpecificLimitAfterStatements["db2"] = ")RES ) SUB WHERE SUB.rnk >= #{firstRow} AND SUB.rnk < #{lastRow}";
		databaseSpecificLimitBetweenStatements["db2"] = ", row_number() over (ORDER BY ${orderByColumns}) rnk FROM ( select distinct RES.* ";
		databaseOuterJoinLimitBetweenStatements["db2"] = ", row_number() over (ORDER BY ${mssqlOrDB2OrderBy}) rnk FROM ( select distinct ";
		databaseSpecificOrderByStatements["db2"] = "";
		databaseSpecificLimitBeforeNativeQueryStatements["db2"] = "SELECT SUB.* FROM ( select RES.* , row_number() over (ORDER BY ${orderByColumns}) rnk FROM (";
		addDatabaseSpecificStatement("db2", "selectExclusiveJobsToExecute", "selectExclusiveJobsToExecute_integerBoolean");
		addDatabaseSpecificStatement("db2", "selectExecutionByNativeQuery", "selectExecutionByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectHistoricActivityInstanceByNativeQuery", "selectHistoricActivityInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectHistoricProcessInstanceByNativeQuery", "selectHistoricProcessInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectHistoricTaskInstanceByNativeQuery", "selectHistoricTaskInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectTaskByNativeQuery", "selectTaskByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectProcessDefinitionByNativeQuery", "selectProcessDefinitionByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectDeploymentByNativeQuery", "selectDeploymentByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectGroupByNativeQuery", "selectGroupByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectUserByNativeQuery", "selectUserByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectModelByNativeQuery", "selectModelByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectHistoricDetailByNativeQuery", "selectHistoricDetailByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectHistoricVariableInstanceByNativeQuery", "selectHistoricVariableInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectTaskWithVariablesByQueryCriteria", "selectTaskWithVariablesByQueryCriteria_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectProcessInstanceWithVariablesByQueryCriteria", "selectProcessInstanceWithVariablesByQueryCriteria_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectHistoricProcessInstancesWithVariablesByQueryCriteria", "selectHistoricProcessInstancesWithVariablesByQueryCriteria_mssql_or_db2");
		addDatabaseSpecificStatement("db2", "selectHistoricTaskInstancesWithVariablesByQueryCriteria", "selectHistoricTaskInstancesWithVariablesByQueryCriteria_mssql_or_db2");

		// mssql
		databaseSpecificLimitBeforeStatements["mssql"] = "SELECT SUB.* FROM (";
		databaseSpecificLimitAfterStatements["mssql"] = ")RES ) SUB WHERE SUB.rnk >= #{firstRow} AND SUB.rnk < #{lastRow}";
		databaseSpecificLimitBetweenStatements["mssql"] = ", row_number() over (ORDER BY ${orderByColumns}) rnk FROM ( select distinct RES.* ";
		databaseOuterJoinLimitBetweenStatements["mssql"] = ", row_number() over (ORDER BY ${mssqlOrDB2OrderBy}) rnk FROM ( select distinct ";
		databaseSpecificOrderByStatements["mssql"] = "";
		databaseSpecificLimitBeforeNativeQueryStatements["mssql"] = "SELECT SUB.* FROM ( select RES.* , row_number() over (ORDER BY ${orderByColumns}) rnk FROM (";
		addDatabaseSpecificStatement("mssql", "selectExclusiveJobsToExecute", "selectExclusiveJobsToExecute_integerBoolean");
		addDatabaseSpecificStatement("mssql", "selectExecutionByNativeQuery", "selectExecutionByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectHistoricActivityInstanceByNativeQuery", "selectHistoricActivityInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectHistoricProcessInstanceByNativeQuery", "selectHistoricProcessInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectHistoricTaskInstanceByNativeQuery", "selectHistoricTaskInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectTaskByNativeQuery", "selectTaskByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectProcessDefinitionByNativeQuery", "selectProcessDefinitionByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectDeploymentByNativeQuery", "selectDeploymentByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectGroupByNativeQuery", "selectGroupByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectUserByNativeQuery", "selectUserByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectModelByNativeQuery", "selectModelByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectHistoricDetailByNativeQuery", "selectHistoricDetailByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectHistoricVariableInstanceByNativeQuery", "selectHistoricVariableInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectTaskWithVariablesByQueryCriteria", "selectTaskWithVariablesByQueryCriteria_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectProcessInstanceWithVariablesByQueryCriteria", "selectProcessInstanceWithVariablesByQueryCriteria_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectHistoricProcessInstancesWithVariablesByQueryCriteria", "selectHistoricProcessInstancesWithVariablesByQueryCriteria_mssql_or_db2");
		addDatabaseSpecificStatement("mssql", "selectHistoricTaskInstancesWithVariablesByQueryCriteria", "selectHistoricTaskInstancesWithVariablesByQueryCriteria_mssql_or_db2");
	  }


	  /// <summary>
	  /// A map {class, boolean}, to indicate whether or not a certain <seealso cref="PersistentObject"/> class can be bulk inserted.
	  /// </summary>
	  protected internal static IDictionary<Type, bool?> bulkInsertableMap;

	  protected internal string databaseType;
	  protected internal string databaseTablePrefix = "";
	  private bool tablePrefixIsSchema;

	  protected internal string databaseCatalog;
	  /// <summary>
	  /// In some situations you want to set the schema to use for table checks /
	  /// generation if the database metadata doesn't return that correctly, see
	  /// https://activiti.atlassian.net/browse/ACT-1220,
	  /// https://activiti.atlassian.net/browse/ACT-1062
	  /// </summary>
	  protected internal string databaseSchema;
	  protected internal SqlSessionFactory sqlSessionFactory;
	  protected internal IdGenerator idGenerator;
	  protected internal IDictionary<string, string> statementMappings;
	  protected internal IDictionary<Type, string> insertStatements = new ConcurrentDictionary<Type, string>();
	  protected internal IDictionary<Type, string> bulkInsertStatements = new ConcurrentDictionary<Type, string>();
	  protected internal IDictionary<Type, string> updateStatements = new ConcurrentDictionary<Type, string>();
	  protected internal IDictionary<Type, string> deleteStatements = new ConcurrentDictionary<Type, string>();
	  protected internal IDictionary<Type, string> bulkDeleteStatements = new ConcurrentDictionary<Type, string>();
	  protected internal IDictionary<Type, string> selectStatements = new ConcurrentDictionary<Type, string>();
	  protected internal bool isDbIdentityUsed = true;
	  protected internal bool isDbHistoryUsed = true;
	  protected internal int maxNrOfStatementsInBulkInsert = 100;


	  public virtual Type SessionType
	  {
		  get
		  {
			return typeof(DbSqlSession);
		  }
	  }

	  public virtual Session openSession()
	  {
		return new DbSqlSession(this);
	  }

	  // insert, update and delete statements /////////////////////////////////////

	  public virtual string getInsertStatement(PersistentObject @object)
	  {
		return getStatement(@object.GetType(), insertStatements, "insert");
	  }


	  public virtual string getInsertStatement(Type clazz)
	  {
		return getStatement(clazz, insertStatements, "insert");
	  }

	  public virtual string getBulkInsertStatement(Type clazz)
	  {
		return getStatement(clazz, bulkInsertStatements, "bulkInsert");
	  }

	  public virtual string getUpdateStatement(PersistentObject @object)
	  {
		return getStatement(@object.GetType(), updateStatements, "update");
	  }

	  public virtual string getDeleteStatement(Type persistentObjectClass)
	  {
		return getStatement(persistentObjectClass, deleteStatements, "delete");
	  }

	  public virtual string getBulkDeleteStatement(Type persistentObjectClass)
	  {
		return getStatement(persistentObjectClass, bulkDeleteStatements, "bulkDelete");
	  }

	  public virtual string getSelectStatement(Type persistentObjectClass)
	  {
		return getStatement(persistentObjectClass, selectStatements, "select");
	  }

	  private string getStatement(Type persistentObjectClass, IDictionary<Type, string> cachedStatements, string prefix)
	  {
		string statement = cachedStatements[persistentObjectClass];
		if (statement != null)
		{
		  return statement;
		}
		statement = prefix + persistentObjectClass.Name;
		statement = statement.Substring(0, statement.Length - 6); // removing 'entity'
		cachedStatements[persistentObjectClass] = statement;
		return statement;
	  }

	  // db specific mappings /////////////////////////////////////////////////////

	  protected internal static void addDatabaseSpecificStatement(string databaseType, string activitiStatement, string ibatisStatement)
	  {
		IDictionary<string, string> specificStatements = databaseSpecificStatements[databaseType];
		if (specificStatements == null)
		{
		  specificStatements = new Dictionary<string, string>();
		  databaseSpecificStatements[databaseType] = specificStatements;
		}
		specificStatements[activitiStatement] = ibatisStatement;
	  }

	  public virtual string mapStatement(string statement)
	  {
		if (statementMappings == null)
		{
		  return statement;
		}
		string mappedStatement = statementMappings[statement];
		return (mappedStatement != null ? mappedStatement : statement);
	  }

	  // customized getters and setters ///////////////////////////////////////////

	  public virtual string DatabaseType
	  {
		  set
		  {
			this.databaseType = value;
			this.statementMappings = databaseSpecificStatements[value];
		  }
		  get
		  {
			return databaseType;
		  }
	  }

	  public virtual void setBulkInsertEnabled(bool isBulkInsertEnabled, string databaseType)
	  {
		  // If false, just keep don't initialize the map. Memory saved.
		  if (isBulkInsertEnabled)
		  {
			  initBulkInsertEnabledMap(databaseType);
		  }
	  }

	  protected internal virtual void initBulkInsertEnabledMap(string databaseType)
	  {
		  bulkInsertableMap = new Dictionary<Type, bool?>();

		  foreach (Type clazz in EntityDependencyOrder.INSERT_ORDER)
		  {
			  bulkInsertableMap[clazz] = true;
		  }

		  // Only Oracle is making a fuss in one specific case right now
			if ("oracle".Equals(databaseType))
			{
				bulkInsertableMap[typeof(EventLogEntryEntity)] = false;
			}
	  }

	  public virtual bool? isBulkInsertable(Type persistentObjectClass)
	  {
		  return bulkInsertableMap != null && bulkInsertableMap.ContainsKey(persistentObjectClass) && bulkInsertableMap[persistentObjectClass] == true;
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual SqlSessionFactory SqlSessionFactory
	  {
		  get
		  {
			return sqlSessionFactory;
		  }
		  set
		  {
			this.sqlSessionFactory = value;
		  }
	  }


	  public virtual IdGenerator IdGenerator
	  {
		  get
		  {
			return idGenerator;
		  }
		  set
		  {
			this.idGenerator = value;
		  }
	  }





	  public virtual IDictionary<string, string> StatementMappings
	  {
		  get
		  {
			return statementMappings;
		  }
		  set
		  {
			this.statementMappings = value;
		  }
	  }




	  public virtual IDictionary<Type, string> InsertStatements
	  {
		  get
		  {
			return insertStatements;
		  }
		  set
		  {
			this.insertStatements = value;
		  }
	  }




	  public virtual IDictionary<Type, string> BulkInsertStatements
	  {
		  get
		  {
			return bulkInsertStatements;
		  }
		  set
		  {
			this.bulkInsertStatements = value;
		  }
	  }




	  public virtual IDictionary<Type, string> UpdateStatements
	  {
		  get
		  {
			return updateStatements;
		  }
		  set
		  {
			this.updateStatements = value;
		  }
	  }




	  public virtual IDictionary<Type, string> DeleteStatements
	  {
		  get
		  {
			return deleteStatements;
		  }
		  set
		  {
			this.deleteStatements = value;
		  }
	  }




	  public virtual IDictionary<Type, string> BulkDeleteStatements
	  {
		  get
		  {
				return bulkDeleteStatements;
		  }
		  set
		  {
				this.bulkDeleteStatements = value;
		  }
	  }


		public virtual IDictionary<Type, string> SelectStatements
		{
			get
			{
			return selectStatements;
			}
			set
			{
			this.selectStatements = value;
			}
		}



	  public virtual bool DbIdentityUsed
	  {
		  get
		  {
			return isDbIdentityUsed;
		  }
		  set
		  {
			this.isDbIdentityUsed = value;
		  }
	  }


	  public virtual bool DbHistoryUsed
	  {
		  get
		  {
			return isDbHistoryUsed;
		  }
		  set
		  {
			this.isDbHistoryUsed = value;
		  }
	  }


	  public virtual string DatabaseTablePrefix
	  {
		  set
		  {
			this.databaseTablePrefix = value;
		  }
		  get
		  {
			return databaseTablePrefix;
		  }
	  }


	  public virtual string DatabaseCatalog
	  {
		  get
		  {
			return databaseCatalog;
		  }
		  set
		  {
			this.databaseCatalog = value;
		  }
	  }


	  public virtual string DatabaseSchema
	  {
		  get
		  {
			return databaseSchema;
		  }
		  set
		  {
			this.databaseSchema = value;
		  }
	  }


		public virtual bool TablePrefixIsSchema
		{
			set
			{
				this.tablePrefixIsSchema = value;
			}
			get
			{
			  return tablePrefixIsSchema;
			}
		}


		public virtual int MaxNrOfStatementsInBulkInsert
		{
			get
			{
				return maxNrOfStatementsInBulkInsert;
			}
			set
			{
				this.maxNrOfStatementsInBulkInsert = value;
			}
		}


	}

}