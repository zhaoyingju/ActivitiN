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
namespace org.activiti.engine
{


	using EventLogEntry = org.activiti.engine.@event.EventLogEntry;
	using org.activiti.engine.impl.cmd;
	using org.activiti.engine.impl.interceptor;
	using CommandConfig = org.activiti.engine.impl.interceptor.CommandConfig;
	using TableMetaData = org.activiti.engine.management.TableMetaData;
	using TablePage = org.activiti.engine.management.TablePage;
	using TablePageQuery = org.activiti.engine.management.TablePageQuery;
	using JobQuery = org.activiti.engine.runtime.JobQuery;



	/// <summary>
	/// Service for admin and maintenance operations on the process engine.
	/// 
	/// These operations will typically not be used in a workflow driven application,
	/// but are used in for example the operational console.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
	public interface ManagementService
	{

	  /// <summary>
	  /// Get the mapping containing {table name, row count} entries of the
	  /// Activiti database schema.
	  /// </summary>
	  IDictionary<string, long?> TableCount {get;}

	  /// <summary>
	  /// Gets the table name (including any configured prefix) for an Activiti entity like Task, Execution or the like.
	  /// </summary>
	  string getTableName(Type activitiEntityClass);

	  /// <summary>
	  /// Gets the metadata (column names, column types, etc.) of a certain table. 
	  /// Returns null when no table exists with the given name.
	  /// </summary>
	  TableMetaData getTableMetaData(string tableName);

	  /// <summary>
	  /// Creates a <seealso cref="TablePageQuery"/> that can be used to fetch <seealso cref="TablePage"/>
	  /// containing specific sections of table row data.
	  /// </summary>
	  TablePageQuery createTablePageQuery();

	  /// <summary>
	  /// Returns a new JobQuery implementation, that can be used
	  /// to dynamically query the jobs.
	  /// </summary>
	  JobQuery createJobQuery();

	  /// <summary>
	  /// Forced synchronous execution of a job (eg. for administation or testing)
	  /// The job will be executed, even if the process definition and/or the process instance
	  /// is in suspended state.
	  /// </summary>
	  /// <param name="jobId"> id of the job to execute, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when there is no job with the given id.  </exception>
	  void executeJob(string jobId);

	  /// <summary>
	  /// Delete the job with the provided id. </summary>
	  /// <param name="jobId"> id of the job to execute, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when there is no job with the given id.  </exception>
	  void deleteJob(string jobId);

	  /// <summary>
	  /// Sets the number of retries that a job has left.
	  /// 
	  /// Whenever the JobExecutor fails to execute a job, this value is decremented. 
	  /// When it hits zero, the job is supposed to be dead and not retried again.
	  /// In that case, this method can be used to increase the number of retries. </summary>
	  /// <param name="jobId"> id of the job to modify, cannot be null. </param>
	  /// <param name="retries"> number of retries. </param>
	  void setJobRetries(string jobId, int retries);

	  /// <summary>
	  /// Returns the full stacktrace of the exception that occurs when the job
	  /// with the given id was last executed. Returns null when the job has no
	  /// exception stacktrace. </summary>
	  /// <param name="jobId"> id of the job, cannot be null. </param>
	  /// <exception cref="ActivitiObjectNotFoundException"> when no job exists with the given id. </exception>
	  string getJobExceptionStacktrace(string jobId);

	  /// <summary>
	  /// get the list of properties. </summary>
	  IDictionary<string, string> Properties {get;}

	  /// <summary>
	  /// programmatic schema update on a given connection returning feedback about what happened </summary>
	  string databaseSchemaUpgrade(Connection connection, string catalog, string schema);

	  /// <summary>
	  /// Executes a given command with the default <seealso cref="CommandConfig"/>. </summary>
	  /// <param name="command"> the command, cannot be null. </param>
	  /// <returns> the result of command execution </returns>
	  T executeCommand<T>(Command<T> command);

	  /// <summary>
	  /// Executes a given command with the specified <seealso cref="CommandConfig"/>. </summary>
	  /// <param name="config"> the command execution configuration, cannot be null. </param>
	  /// <param name="command"> the command, cannot be null. </param>
	  /// <returns> the result of command execution </returns>
	  T executeCommand<T>(CommandConfig config, Command<T> command);

	  /// <summary>
	  /// [EXPERIMENTAL]
	  /// 
	  /// Executes the sql contained in the <seealso cref="CustomSqlExecution"/> parameter.
	  /// </summary>
	  ResultType executeCustomSql<MapperType, ResultType>(CustomSqlExecution<MapperType, ResultType> customSqlExecution);

	  /// <summary>
	  /// [EXPERIMENTAL]
	  /// 
	  /// Returns a list of event log entries, describing everything the engine has processed.
	  /// Note that the event logging must specifically must be enabled in the process engine configuration.
	  /// 
	  /// Passing null as arguments will effectively fetch ALL event log entries. 
	  /// Be careful, as this list might be huge!
	  /// </summary>
	  IList<EventLogEntry> getEventLogEntries(long? startLogNr, long? pageSize);

	  /// <summary>
	  /// [EXPERIMENTAL]
	  /// 
	  /// Returns a list of event log entries for a specific process instance id.
	  /// Note that the event logging must specifically must be enabled in the process engine configuration.
	  /// 
	  /// Passing null as arguments will effectively fetch ALL event log entries. 
	  /// Be careful, as this list might be huge!
	  /// </summary>
	  IList<EventLogEntry> getEventLogEntriesByProcessInstanceId(string processInstanceId);

	  /// <summary>
	  /// Delete a EventLogEntry.
	  /// Typically only used in testing, as deleting log entries defeats the whole purpose of keeping a log.
	  /// </summary>
	  void deleteEventLogEntry(long logNr);

	}

}