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


	using EventLogEntry = org.activiti.engine.@event.EventLogEntry;
	using CancelJobCmd = org.activiti.engine.impl.cmd.CancelJobCmd;
	using org.activiti.engine.impl.cmd;
	using DeleteEventLogEntry = org.activiti.engine.impl.cmd.DeleteEventLogEntry;
	using org.activiti.engine.impl.cmd;
	using ExecuteJobsCmd = org.activiti.engine.impl.cmd.ExecuteJobsCmd;
	using GetEventLogEntriesCmd = org.activiti.engine.impl.cmd.GetEventLogEntriesCmd;
	using GetJobExceptionStacktraceCmd = org.activiti.engine.impl.cmd.GetJobExceptionStacktraceCmd;
	using GetPropertiesCmd = org.activiti.engine.impl.cmd.GetPropertiesCmd;
	using GetTableCountCmd = org.activiti.engine.impl.cmd.GetTableCountCmd;
	using GetTableMetaDataCmd = org.activiti.engine.impl.cmd.GetTableMetaDataCmd;
	using GetTableNameCmd = org.activiti.engine.impl.cmd.GetTableNameCmd;
	using SetJobRetriesCmd = org.activiti.engine.impl.cmd.SetJobRetriesCmd;
	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using DbSqlSessionFactory = org.activiti.engine.impl.db.DbSqlSessionFactory;
	using org.activiti.engine.impl.interceptor;
	using CommandConfig = org.activiti.engine.impl.interceptor.CommandConfig;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using TableMetaData = org.activiti.engine.management.TableMetaData;
	using TablePageQuery = org.activiti.engine.management.TablePageQuery;
	using JobQuery = org.activiti.engine.runtime.JobQuery;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Falko Menge
	/// @author Saeid Mizaei
	/// </summary>
	public class ManagementServiceImpl : ServiceImpl, ManagementService
	{

	  public virtual IDictionary<string, long?> TableCount
	  {
		  get
		  {
			return commandExecutor.execute(new GetTableCountCmd());
		  }
	  }

	  public virtual string getTableName(Type activitiEntityClass)
	  {
		return commandExecutor.execute(new GetTableNameCmd(activitiEntityClass));
	  }

	  public virtual TableMetaData getTableMetaData(string tableName)
	  {
		return commandExecutor.execute(new GetTableMetaDataCmd(tableName));
	  }

	  public virtual void executeJob(string jobId)
	  {
		commandExecutor.execute(new ExecuteJobsCmd(jobId));
	  }

	  public virtual void deleteJob(string jobId)
	  {
		commandExecutor.execute(new CancelJobCmd(jobId));
	  }

	  public virtual void setJobRetries(string jobId, int retries)
	  {
		commandExecutor.execute(new SetJobRetriesCmd(jobId, retries));
	  }

	  public virtual TablePageQuery createTablePageQuery()
	  {
		return new TablePageQueryImpl(commandExecutor);
	  }

	  public virtual JobQuery createJobQuery()
	  {
		return new JobQueryImpl(commandExecutor);
	  }

	  public virtual string getJobExceptionStacktrace(string jobId)
	  {
		return commandExecutor.execute(new GetJobExceptionStacktraceCmd(jobId));
	  }

	  public virtual IDictionary<string, string> Properties
	  {
		  get
		  {
			return commandExecutor.execute(new GetPropertiesCmd());
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public String databaseSchemaUpgrade(final java.sql.Connection connection, final String catalog, final String schema)
	  public virtual string databaseSchemaUpgrade(Connection connection, string catalog, string schema)
	  {
		CommandConfig config = commandExecutor.DefaultConfig.transactionNotSupported();
		return commandExecutor.execute(config, new CommandAnonymousInnerClassHelper(this, connection, catalog, schema));
	  }

	  private class CommandAnonymousInnerClassHelper : Command<string>
	  {
		  private readonly ManagementServiceImpl outerInstance;

		  private Connection connection;
		  private string catalog;
		  private string schema;

		  public CommandAnonymousInnerClassHelper(ManagementServiceImpl outerInstance, Connection connection, string catalog, string schema)
		  {
			  this.outerInstance = outerInstance;
			  this.connection = connection;
			  this.catalog = catalog;
			  this.schema = schema;
		  }

		  public virtual string execute(CommandContext commandContext)
		  {
			DbSqlSessionFactory dbSqlSessionFactory = (DbSqlSessionFactory) commandContext.SessionFactories[typeof(DbSqlSession)];
			DbSqlSession dbSqlSession = new DbSqlSession(dbSqlSessionFactory, connection, catalog, schema);
			commandContext.Sessions[typeof(DbSqlSession)] = dbSqlSession;
			return dbSqlSession.dbSchemaUpdate();
		  }
	  }

	  public virtual T executeCommand<T>(Command<T> command)
	  {
		if (command == null)
		{
		  throw new ActivitiIllegalArgumentException("The command is null");
		}
		return commandExecutor.execute(command);
	  }

	  public virtual T executeCommand<T>(CommandConfig config, Command<T> command)
	  {
		if (config == null)
		{
		  throw new ActivitiIllegalArgumentException("The config is null");
		}
		if (command == null)
		{
		  throw new ActivitiIllegalArgumentException("The command is null");
		}
		return commandExecutor.execute(config, command);
	  }

	  public override ResultType executeCustomSql<MapperType, ResultType>(CustomSqlExecution<MapperType, ResultType> customSqlExecution)
	  {
		  Type mapperClass = customSqlExecution.MapperClass;
		  return commandExecutor.execute(new ExecuteCustomSqlCmd<MapperType, ResultType>(mapperClass, customSqlExecution));
	  }

	  public override IList<EventLogEntry> getEventLogEntries(long? startLogNr, long? pageSize)
	  {
		  return commandExecutor.execute(new GetEventLogEntriesCmd(startLogNr, pageSize));
	  }

	  public override IList<EventLogEntry> getEventLogEntriesByProcessInstanceId(string processInstanceId)
	  {
		return commandExecutor.execute(new GetEventLogEntriesCmd(processInstanceId));
	  }

	  public override void deleteEventLogEntry(long logNr)
	  {
		  commandExecutor.execute(new DeleteEventLogEntry(logNr));
	  }

	}

}