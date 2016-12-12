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


	using MailServerInfo = org.activiti.engine.cfg.MailServerInfo;
	using AsyncExecutor = org.activiti.engine.impl.asyncexecutor.AsyncExecutor;
	using BeansConfigurationHelper = org.activiti.engine.impl.cfg.BeansConfigurationHelper;
	using StandaloneInMemProcessEngineConfiguration = org.activiti.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using StandaloneProcessEngineConfiguration = org.activiti.engine.impl.cfg.StandaloneProcessEngineConfiguration;
	using HistoryLevel = org.activiti.engine.impl.history.HistoryLevel;
	using JobExecutor = org.activiti.engine.impl.jobexecutor.JobExecutor;
	using Clock = org.activiti.engine.runtime.Clock;
	using ProcessDiagramGenerator = org.activiti.image.ProcessDiagramGenerator;


	/// <summary>
	/// Configuration information from which a process engine can be build.
	/// 
	/// <para>Most common is to create a process engine based on the default configuration file:
	/// <pre>ProcessEngine processEngine = ProcessEngineConfiguration
	///   .createProcessEngineConfigurationFromResourceDefault()
	///   .buildProcessEngine();
	/// </pre>
	/// </para>
	/// 
	/// <para>To create a process engine programatic, without a configuration file, 
	/// the first option is <seealso cref="#createStandaloneProcessEngineConfiguration()"/>
	/// <pre>ProcessEngine processEngine = ProcessEngineConfiguration
	///   .createStandaloneProcessEngineConfiguration()
	///   .buildProcessEngine();
	/// </pre>
	/// This creates a new process engine with all the defaults to connect to 
	/// a remote h2 database (jdbc:h2:tcp://localhost/activiti) in standalone 
	/// mode.  Standalone mode means that Activiti will manage the transactions 
	/// on the JDBC connections that it creates.  One transaction per 
	/// service method.
	/// For a description of how to write the configuration files, see the 
	/// userguide.
	/// </para>
	/// 
	/// <para>The second option is great for testing: <seealso cref="#createStandalonInMemeProcessEngineConfiguration()"/>
	/// <pre>ProcessEngine processEngine = ProcessEngineConfiguration
	///   .createStandaloneInMemProcessEngineConfiguration()
	///   .buildProcessEngine();
	/// </pre>
	/// This creates a new process engine with all the defaults to connect to 
	/// an memory h2 database (jdbc:h2:tcp://localhost/activiti) in standalone 
	/// mode.  The DB schema strategy default is in this case <code>create-drop</code>.  
	/// Standalone mode means that Activiti will manage the transactions 
	/// on the JDBC connections that it creates.  One transaction per 
	/// service method.
	/// </para>
	/// 
	/// <para>On all forms of creating a process engine, you can first customize the configuration 
	/// before calling the <seealso cref="#buildProcessEngine()"/> method by calling any of the 
	/// setters like this:
	/// <pre>ProcessEngine processEngine = ProcessEngineConfiguration
	///   .createProcessEngineConfigurationFromResourceDefault()
	///   .setMailServerHost("gmail.com")
	///   .setJdbcUsername("mickey")
	///   .setJdbcPassword("mouse")
	///   .buildProcessEngine();
	/// </pre>
	/// </para>
	/// </summary>
	/// <seealso cref= ProcessEngines 
	/// @author Tom Baeyens </seealso>
	public abstract class ProcessEngineConfiguration : EngineServices
	{
		public abstract ProcessEngineConfiguration ProcessEngineConfiguration {get;}
		public abstract DynamicBpmnService DynamicBpmnService {get;}
		public abstract ManagementService ManagementService {get;}
		public abstract IdentityService IdentityService {get;}
		public abstract HistoryService HistoryService {get;}
		public abstract TaskService TaskService {get;}
		public abstract FormService FormService {get;}
		public abstract RuntimeService RuntimeService {get;}
		public abstract RepositoryService RepositoryService {get;}

	  /// <summary>
	  /// Checks the version of the DB schema against the library when 
	  /// the process engine is being created and throws an exception
	  /// if the versions don't match. 
	  /// </summary>
	  public const string DB_SCHEMA_UPDATE_FALSE = "false";

	  /// <summary>
	  /// Creates the schema when the process engine is being created and 
	  /// drops the schema when the process engine is being closed. 
	  /// </summary>
	  public const string DB_SCHEMA_UPDATE_CREATE_DROP = "create-drop";

	  /// <summary>
	  /// Upon building of the process engine, a check is performed and 
	  /// an update of the schema is performed if it is necessary. 
	  /// </summary>
	  public const string DB_SCHEMA_UPDATE_TRUE = "true";

	  /// <summary>
	  /// The tenant id indicating 'no tenant' </summary>
	  public const string NO_TENANT_ID = "";

	  protected internal string processEngineName = ProcessEngines.NAME_DEFAULT;
	  protected internal int idBlockSize = 2500;
	  protected internal string history = HistoryLevel.AUDIT.Key;
	  protected internal bool jobExecutorActivate;
	  protected internal bool asyncExecutorEnabled;
	  protected internal bool asyncExecutorActivate;

	  protected internal string mailServerHost = "localhost";
	  protected internal string mailServerUsername; // by default no name and password are provided, which
	  protected internal string mailServerPassword; // means no authentication for mail server
	  protected internal int mailServerPort = 25;
	  protected internal bool useSSL = false;
	  protected internal bool useTLS = false;
	  protected internal string mailServerDefaultFrom = "activiti@localhost";
	  protected internal string mailSessionJndi;
	  protected internal IDictionary<string, MailServerInfo> mailServers = new Dictionary<string, MailServerInfo>();
	  protected internal IDictionary<string, string> mailSessionsJndi = new Dictionary<string, string>();

	  protected internal string databaseType;
	  protected internal string databaseSchemaUpdate = DB_SCHEMA_UPDATE_FALSE;
	  protected internal string jdbcDriver = "org.h2.Driver";
	  protected internal string jdbcUrl = "jdbc:h2:tcp://localhost/~/activiti";
	  protected internal string jdbcUsername = "sa";
	  protected internal string jdbcPassword = "";
	  protected internal string dataSourceJndiName = null;
	  protected internal bool isDbIdentityUsed = true;
	  protected internal bool isDbHistoryUsed = true;
	  protected internal HistoryLevel historyLevel;
	  protected internal int jdbcMaxActiveConnections;
	  protected internal int jdbcMaxIdleConnections;
	  protected internal int jdbcMaxCheckoutTime;
	  protected internal int jdbcMaxWaitTime;
	  protected internal bool jdbcPingEnabled = false;
	  protected internal string jdbcPingQuery = null;
	  protected internal int jdbcPingConnectionNotUsedFor;
	  protected internal int jdbcDefaultTransactionIsolationLevel;
	  protected internal DataSource dataSource;
	  protected internal bool transactionsExternallyManaged = false;

	  protected internal string jpaPersistenceUnitName;
	  protected internal object jpaEntityManagerFactory;
	  protected internal bool jpaHandleTransaction;
	  protected internal bool jpaCloseEntityManager;

	  protected internal Clock clock;
	  protected internal JobExecutor jobExecutor;
	  protected internal AsyncExecutor asyncExecutor;

	  /// <summary>
	  /// Define the default lock time for an async job in seconds.
	  /// The lock time is used when creating an async job and when it expires the async executor
	  /// assumes that the job has failed. It will be retried again.  
	  /// </summary>
	  protected internal int lockTimeAsyncJobWaitTime = 60;
	  /// <summary>
	  /// define the default wait time for a failed job in seconds </summary>
	  protected internal int defaultFailedJobWaitTime = 10;
	  /// <summary>
	  /// define the default wait time for a failed async job in seconds </summary>
	  protected internal int asyncFailedJobWaitTime = 10;

	  /// <summary>
	  /// process diagram generator. Default value is DefaulProcessDiagramGenerator </summary>
	  protected internal ProcessDiagramGenerator processDiagramGenerator;

	  /// <summary>
	  /// Allows configuring a database table prefix which is used for all runtime operations of the process engine.
	  /// For example, if you specify a prefix named 'PRE1.', activiti will query for executions in a table named
	  /// 'PRE1.ACT_RU_EXECUTION_'. 
	  /// 
	  /// <p />
	  /// <strong>NOTE: the prefix is not respected by automatic database schema management. If you use 
	  /// <seealso cref="ProcessEngineConfiguration#DB_SCHEMA_UPDATE_CREATE_DROP"/> 
	  /// or <seealso cref="ProcessEngineConfiguration#DB_SCHEMA_UPDATE_TRUE"/>, activiti will create the database tables 
	  /// using the default names, regardless of the prefix configured here.</strong>  
	  /// 
	  /// @since 5.9
	  /// </summary>
	  protected internal string databaseTablePrefix = "";

	  /// <summary>
	  /// Escape character for doing wildcard searches.
	  /// 
	  /// This will be added at then end of queries that include for example a LIKE clause.
	  /// For example: SELECT * FROM table WHERE column LIKE '%\%%' ESCAPE '\';
	  /// </summary>
	  protected internal string databaseWildcardEscapeCharacter;

	  /// <summary>
	  /// database catalog to use
	  /// </summary>
	  protected internal string databaseCatalog = "";

	  /// <summary>
	  /// In some situations you want to set the schema to use for table checks / generation if the database metadata
	  /// doesn't return that correctly, see https://activiti.atlassian.net/browse/ACT-1220,
	  /// https://activiti.atlassian.net/browse/ACT-1062
	  /// </summary>
	  protected internal string databaseSchema = null;

	  /// <summary>
	  /// Set to true in case the defined databaseTablePrefix is a schema-name, instead of an actual table name
	  /// prefix. This is relevant for checking if Activiti-tables exist, the databaseTablePrefix will not be used here
	  /// - since the schema is taken into account already, adding a prefix for the table-check will result in wrong table-names.
	  /// 
	  ///  @since 5.15
	  /// </summary>
	  protected internal bool tablePrefixIsSchema = false;

	  protected internal bool isCreateDiagramOnDeploy = true;

	  protected internal string xmlEncoding = "UTF-8";

	  protected internal string defaultCamelContext = "camelContext";

	  protected internal string activityFontName = "Arial";
	  protected internal string labelFontName = "Arial";
	  protected internal string annotationFontName = "Arial";

	  protected internal ClassLoader classLoader;
	  /// <summary>
	  /// Either use Class.forName or ClassLoader.loadClass for class loading.
	  /// See http://forums.activiti.org/content/reflectutilloadclass-and-custom-classloader
	  /// </summary>
	  protected internal bool useClassForNameClassLoading = true;
	  protected internal ProcessEngineLifecycleListener processEngineLifecycleListener;

	  protected internal bool enableProcessDefinitionInfoCache = false;

	  /// <summary>
	  /// use one of the static createXxxx methods instead </summary>
	  protected internal ProcessEngineConfiguration()
	  {
	  }

	  public abstract ProcessEngine buildProcessEngine();

	  public static ProcessEngineConfiguration createProcessEngineConfigurationFromResourceDefault()
	  {
		return createProcessEngineConfigurationFromResource("activiti.cfg.xml", "processEngineConfiguration");
	  }

	  public static ProcessEngineConfiguration createProcessEngineConfigurationFromResource(string resource)
	  {
		return createProcessEngineConfigurationFromResource(resource, "processEngineConfiguration");
	  }

	  public static ProcessEngineConfiguration createProcessEngineConfigurationFromResource(string resource, string beanName)
	  {
		return BeansConfigurationHelper.parseProcessEngineConfigurationFromResource(resource, beanName);
	  }

	  public static ProcessEngineConfiguration createProcessEngineConfigurationFromInputStream(InputStream inputStream)
	  {
		return createProcessEngineConfigurationFromInputStream(inputStream, "processEngineConfiguration");
	  }

	  public static ProcessEngineConfiguration createProcessEngineConfigurationFromInputStream(InputStream inputStream, string beanName)
	  {
		return BeansConfigurationHelper.parseProcessEngineConfigurationFromInputStream(inputStream, beanName);
	  }

	  public static ProcessEngineConfiguration createStandaloneProcessEngineConfiguration()
	  {
		return new StandaloneProcessEngineConfiguration();
	  }

	  public static ProcessEngineConfiguration createStandaloneInMemProcessEngineConfiguration()
	  {
		return new StandaloneInMemProcessEngineConfiguration();
	  }

	// TODO add later when we have test coverage for this
	//  public static ProcessEngineConfiguration createJtaProcessEngineConfiguration() {
	//    return new JtaProcessEngineConfiguration();
	//  }


	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string ProcessEngineName
	  {
		  get
		  {
			return processEngineName;
		  }
	  }

	  public virtual ProcessEngineConfiguration setProcessEngineName(string processEngineName)
	  {
		this.processEngineName = processEngineName;
		return this;
	  }

	  public virtual int IdBlockSize
	  {
		  get
		  {
			return idBlockSize;
		  }
	  }

	  public virtual ProcessEngineConfiguration setIdBlockSize(int idBlockSize)
	  {
		this.idBlockSize = idBlockSize;
		return this;
	  }

	  public virtual string History
	  {
		  get
		  {
			return history;
		  }
	  }

	  public virtual ProcessEngineConfiguration setHistory(string history)
	  {
		this.history = history;
		return this;
	  }

	  public virtual string MailServerHost
	  {
		  get
		  {
			return mailServerHost;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServerHost(string mailServerHost)
	  {
		this.mailServerHost = mailServerHost;
		return this;
	  }

	  public virtual string MailServerUsername
	  {
		  get
		  {
			return mailServerUsername;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServerUsername(string mailServerUsername)
	  {
		this.mailServerUsername = mailServerUsername;
		return this;
	  }

	  public virtual string MailServerPassword
	  {
		  get
		  {
			return mailServerPassword;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServerPassword(string mailServerPassword)
	  {
		this.mailServerPassword = mailServerPassword;
		return this;
	  }

	  public virtual string MailSessionJndi
	  {
		  get
		  {
			return mailSessionJndi;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailSessionJndi(string mailSessionJndi)
	  {
		this.mailSessionJndi = mailSessionJndi;
		return this;
	  }

	  public virtual int MailServerPort
	  {
		  get
		  {
			return mailServerPort;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServerPort(int mailServerPort)
	  {
		this.mailServerPort = mailServerPort;
		return this;
	  }

	  public virtual bool MailServerUseSSL
	  {
		  get
		  {
			  return useSSL;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServerUseSSL(bool useSSL)
	  {
		  this.useSSL = useSSL;
		  return this;
	  }

	  public virtual bool MailServerUseTLS
	  {
		  get
		  {
			return useTLS;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServerUseTLS(bool useTLS)
	  {
		this.useTLS = useTLS;
		return this;
	  }

	  public virtual string MailServerDefaultFrom
	  {
		  get
		  {
			return mailServerDefaultFrom;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServerDefaultFrom(string mailServerDefaultFrom)
	  {
		this.mailServerDefaultFrom = mailServerDefaultFrom;
		return this;
	  }

	  public virtual MailServerInfo getMailServer(string tenantId)
	  {
		return mailServers[tenantId];
	  }

	  public virtual IDictionary<string, MailServerInfo> MailServers
	  {
		  get
		  {
			return mailServers;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServers(IDictionary<string, MailServerInfo> mailServers)
	  {
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		this.mailServers.putAll(mailServers);
		return this;
	  }

	  public virtual string getMailSessionJndi(string tenantId)
	  {
		return mailSessionsJndi[tenantId];
	  }

	  public virtual IDictionary<string, string> MailSessionsJndi
	  {
		  get
		  {
			return mailSessionsJndi;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailSessionsJndi(IDictionary<string, string> mailSessionsJndi)
	  {
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		this.mailSessionsJndi.putAll(mailSessionsJndi);
		return this;
	  }

	  public virtual string DatabaseType
	  {
		  get
		  {
			return databaseType;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDatabaseType(string databaseType)
	  {
		this.databaseType = databaseType;
		return this;
	  }

	  public virtual string DatabaseSchemaUpdate
	  {
		  get
		  {
			return databaseSchemaUpdate;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDatabaseSchemaUpdate(string databaseSchemaUpdate)
	  {
		this.databaseSchemaUpdate = databaseSchemaUpdate;
		return this;
	  }

	  public virtual DataSource DataSource
	  {
		  get
		  {
			return dataSource;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDataSource(DataSource dataSource)
	  {
		this.dataSource = dataSource;
		return this;
	  }

	  public virtual string JdbcDriver
	  {
		  get
		  {
			return jdbcDriver;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcDriver(string jdbcDriver)
	  {
		this.jdbcDriver = jdbcDriver;
		return this;
	  }

	  public virtual string JdbcUrl
	  {
		  get
		  {
			return jdbcUrl;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcUrl(string jdbcUrl)
	  {
		this.jdbcUrl = jdbcUrl;
		return this;
	  }

	  public virtual string JdbcUsername
	  {
		  get
		  {
			return jdbcUsername;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcUsername(string jdbcUsername)
	  {
		this.jdbcUsername = jdbcUsername;
		return this;
	  }

	  public virtual string JdbcPassword
	  {
		  get
		  {
			return jdbcPassword;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcPassword(string jdbcPassword)
	  {
		this.jdbcPassword = jdbcPassword;
		return this;
	  }

	  public virtual bool TransactionsExternallyManaged
	  {
		  get
		  {
			return transactionsExternallyManaged;
		  }
	  }

	  public virtual ProcessEngineConfiguration setTransactionsExternallyManaged(bool transactionsExternallyManaged)
	  {
		this.transactionsExternallyManaged = transactionsExternallyManaged;
		return this;
	  }

	  public virtual HistoryLevel HistoryLevel
	  {
		  get
		  {
			return historyLevel;
		  }
	  }

	  public virtual ProcessEngineConfiguration setHistoryLevel(HistoryLevel historyLevel)
	  {
		this.historyLevel = historyLevel;
		return this;
	  }

	  public virtual bool DbIdentityUsed
	  {
		  get
		  {
			return isDbIdentityUsed;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDbIdentityUsed(bool isDbIdentityUsed)
	  {
		this.isDbIdentityUsed = isDbIdentityUsed;
		return this;
	  }

	  public virtual bool DbHistoryUsed
	  {
		  get
		  {
			return isDbHistoryUsed;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDbHistoryUsed(bool isDbHistoryUsed)
	  {
		this.isDbHistoryUsed = isDbHistoryUsed;
		return this;
	  }

	  public virtual int JdbcMaxActiveConnections
	  {
		  get
		  {
			return jdbcMaxActiveConnections;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcMaxActiveConnections(int jdbcMaxActiveConnections)
	  {
		this.jdbcMaxActiveConnections = jdbcMaxActiveConnections;
		return this;
	  }

	  public virtual int JdbcMaxIdleConnections
	  {
		  get
		  {
			return jdbcMaxIdleConnections;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcMaxIdleConnections(int jdbcMaxIdleConnections)
	  {
		this.jdbcMaxIdleConnections = jdbcMaxIdleConnections;
		return this;
	  }

	  public virtual int JdbcMaxCheckoutTime
	  {
		  get
		  {
			return jdbcMaxCheckoutTime;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcMaxCheckoutTime(int jdbcMaxCheckoutTime)
	  {
		this.jdbcMaxCheckoutTime = jdbcMaxCheckoutTime;
		return this;
	  }

	  public virtual int JdbcMaxWaitTime
	  {
		  get
		  {
			return jdbcMaxWaitTime;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcMaxWaitTime(int jdbcMaxWaitTime)
	  {
		this.jdbcMaxWaitTime = jdbcMaxWaitTime;
		return this;
	  }

	  public virtual bool JdbcPingEnabled
	  {
		  get
		  {
			return jdbcPingEnabled;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcPingEnabled(bool jdbcPingEnabled)
	  {
		this.jdbcPingEnabled = jdbcPingEnabled;
		return this;
	  }

	  public virtual string JdbcPingQuery
	  {
		  get
		  {
			  return jdbcPingQuery;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcPingQuery(string jdbcPingQuery)
	  {
		this.jdbcPingQuery = jdbcPingQuery;
		return this;
	  }

	  public virtual int JdbcPingConnectionNotUsedFor
	  {
		  get
		  {
			  return jdbcPingConnectionNotUsedFor;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcPingConnectionNotUsedFor(int jdbcPingNotUsedFor)
	  {
		this.jdbcPingConnectionNotUsedFor = jdbcPingNotUsedFor;
		return this;
	  }

	  public virtual int JdbcDefaultTransactionIsolationLevel
	  {
		  get
		  {
			return jdbcDefaultTransactionIsolationLevel;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcDefaultTransactionIsolationLevel(int jdbcDefaultTransactionIsolationLevel)
	  {
		this.jdbcDefaultTransactionIsolationLevel = jdbcDefaultTransactionIsolationLevel;
		return this;
	  }

	  public virtual bool JobExecutorActivate
	  {
		  get
		  {
			return jobExecutorActivate;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJobExecutorActivate(bool jobExecutorActivate)
	  {
		this.jobExecutorActivate = jobExecutorActivate;
		return this;
	  }

	  public virtual bool AsyncExecutorEnabled
	  {
		  get
		  {
			return asyncExecutorEnabled;
		  }
	  }

	  public virtual ProcessEngineConfiguration setAsyncExecutorEnabled(bool asyncExecutorEnabled)
	  {
		this.asyncExecutorEnabled = asyncExecutorEnabled;
		return this;
	  }

	  public virtual bool AsyncExecutorActivate
	  {
		  get
		  {
			return asyncExecutorActivate;
		  }
	  }

	  public virtual ProcessEngineConfiguration setAsyncExecutorActivate(bool asyncExecutorActivate)
	  {
		this.asyncExecutorActivate = asyncExecutorActivate;
		return this;
	  }

	  public virtual ClassLoader ClassLoader
	  {
		  get
		  {
			return classLoader;
		  }
	  }

	  public virtual ProcessEngineConfiguration setClassLoader(ClassLoader classLoader)
	  {
		this.classLoader = classLoader;
		return this;
	  }

	  public virtual bool UseClassForNameClassLoading
	  {
		  get
		  {
			return useClassForNameClassLoading;
		  }
	  }

	  public virtual ProcessEngineConfiguration setUseClassForNameClassLoading(bool useClassForNameClassLoading)
	  {
		this.useClassForNameClassLoading = useClassForNameClassLoading;
		return this;
	  }

	  public virtual object JpaEntityManagerFactory
	  {
		  get
		  {
			return jpaEntityManagerFactory;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJpaEntityManagerFactory(object jpaEntityManagerFactory)
	  {
		this.jpaEntityManagerFactory = jpaEntityManagerFactory;
		return this;
	  }

	  public virtual bool JpaHandleTransaction
	  {
		  get
		  {
			return jpaHandleTransaction;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJpaHandleTransaction(bool jpaHandleTransaction)
	  {
		this.jpaHandleTransaction = jpaHandleTransaction;
		return this;
	  }

	  public virtual bool JpaCloseEntityManager
	  {
		  get
		  {
			return jpaCloseEntityManager;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJpaCloseEntityManager(bool jpaCloseEntityManager)
	  {
		this.jpaCloseEntityManager = jpaCloseEntityManager;
		return this;
	  }

	  public virtual string JpaPersistenceUnitName
	  {
		  get
		  {
			return jpaPersistenceUnitName;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJpaPersistenceUnitName(string jpaPersistenceUnitName)
	  {
		this.jpaPersistenceUnitName = jpaPersistenceUnitName;
		return this;
	  }

	  public virtual string DataSourceJndiName
	  {
		  get
		  {
			return dataSourceJndiName;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDataSourceJndiName(string dataSourceJndiName)
	  {
		this.dataSourceJndiName = dataSourceJndiName;
		return this;
	  }

	  public virtual string DefaultCamelContext
	  {
		  get
		  {
			return defaultCamelContext;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDefaultCamelContext(string defaultCamelContext)
	  {
		this.defaultCamelContext = defaultCamelContext;
		return this;
	  }

	  public virtual bool CreateDiagramOnDeploy
	  {
		  get
		  {
			return isCreateDiagramOnDeploy;
		  }
	  }

	  public virtual ProcessEngineConfiguration setCreateDiagramOnDeploy(bool createDiagramOnDeploy)
	  {
		this.isCreateDiagramOnDeploy = createDiagramOnDeploy;
		return this;
	  }

	  public virtual string ActivityFontName
	  {
		  get
		  {
			return activityFontName;
		  }
	  }

	  public virtual ProcessEngineConfiguration setActivityFontName(string activityFontName)
	  {
		this.activityFontName = activityFontName;
		return this;
	  }

	  public virtual ProcessEngineConfiguration setProcessEngineLifecycleListener(ProcessEngineLifecycleListener processEngineLifecycleListener)
	  {
		this.processEngineLifecycleListener = processEngineLifecycleListener;
		return this;
	  }

	  public virtual ProcessEngineLifecycleListener ProcessEngineLifecycleListener
	  {
		  get
		  {
			return processEngineLifecycleListener;
		  }
	  }

	  public virtual string LabelFontName
	  {
		  get
		  {
			return labelFontName;
		  }
	  }

	  public virtual ProcessEngineConfiguration setLabelFontName(string labelFontName)
	  {
		this.labelFontName = labelFontName;
		return this;
	  }

	  public virtual string AnnotationFontName
	  {
		  get
		  {
			  return annotationFontName;
		  }
	  }

	  public virtual ProcessEngineConfiguration setAnnotationFontName(string annotationFontName)
	  {
		  this.annotationFontName = annotationFontName;
		  return this;
	  }

	  public virtual string DatabaseTablePrefix
	  {
		  get
		  {
			return databaseTablePrefix;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDatabaseTablePrefix(string databaseTablePrefix)
	  {
		this.databaseTablePrefix = databaseTablePrefix;
		return this;
	  }

	  public virtual ProcessEngineConfiguration setTablePrefixIsSchema(bool tablePrefixIsSchema)
	  {
		  this.tablePrefixIsSchema = tablePrefixIsSchema;
		  return this;
	  }

	  public virtual bool TablePrefixIsSchema
	  {
		  get
		  {
			  return tablePrefixIsSchema;
		  }
	  }

	  public virtual string DatabaseWildcardEscapeCharacter
	  {
		  get
		  {
			return databaseWildcardEscapeCharacter;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDatabaseWildcardEscapeCharacter(string databaseWildcardEscapeCharacter)
	  {
		this.databaseWildcardEscapeCharacter = databaseWildcardEscapeCharacter;
		return this;
	  }

	  public virtual string DatabaseCatalog
	  {
		  get
		  {
			return databaseCatalog;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDatabaseCatalog(string databaseCatalog)
	  {
		this.databaseCatalog = databaseCatalog;
		return this;
	  }

	  public virtual string DatabaseSchema
	  {
		  get
		  {
			return databaseSchema;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDatabaseSchema(string databaseSchema)
	  {
		this.databaseSchema = databaseSchema;
		return this;
	  }

	  public virtual string XmlEncoding
	  {
		  get
		  {
			return xmlEncoding;
		  }
	  }

	  public virtual ProcessEngineConfiguration setXmlEncoding(string xmlEncoding)
	  {
		this.xmlEncoding = xmlEncoding;
		return this;
	  }

	  public virtual Clock Clock
	  {
		  get
		  {
			return clock;
		  }
	  }

	  public virtual ProcessEngineConfiguration setClock(Clock clock)
	  {
		this.clock = clock;
		return this;
	  }

	  public virtual ProcessDiagramGenerator ProcessDiagramGenerator
	  {
		  get
		  {
			return this.processDiagramGenerator;
		  }
	  }

	  public virtual ProcessEngineConfiguration setProcessDiagramGenerator(ProcessDiagramGenerator processDiagramGenerator)
	  {
		this.processDiagramGenerator = processDiagramGenerator;
		return this;
	  }

	  public virtual JobExecutor JobExecutor
	  {
		  get
		  {
			return jobExecutor;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJobExecutor(JobExecutor jobExecutor)
	  {
		this.jobExecutor = jobExecutor;
		return this;
	  }

	  public virtual AsyncExecutor AsyncExecutor
	  {
		  get
		  {
			return asyncExecutor;
		  }
	  }

	  public virtual ProcessEngineConfiguration setAsyncExecutor(AsyncExecutor asyncExecutor)
	  {
		this.asyncExecutor = asyncExecutor;
		return this;
	  }

	  public virtual int LockTimeAsyncJobWaitTime
	  {
		  get
		  {
			return lockTimeAsyncJobWaitTime;
		  }
	  }

	  public virtual ProcessEngineConfiguration setLockTimeAsyncJobWaitTime(int lockTimeAsyncJobWaitTime)
	  {
		this.lockTimeAsyncJobWaitTime = lockTimeAsyncJobWaitTime;
		return this;
	  }

	  public virtual int DefaultFailedJobWaitTime
	  {
		  get
		  {
			return defaultFailedJobWaitTime;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDefaultFailedJobWaitTime(int defaultFailedJobWaitTime)
	  {
		this.defaultFailedJobWaitTime = defaultFailedJobWaitTime;
		return this;
	  }

	  public virtual int AsyncFailedJobWaitTime
	  {
		  get
		  {
			return asyncFailedJobWaitTime;
		  }
	  }

	  public virtual ProcessEngineConfiguration setAsyncFailedJobWaitTime(int asyncFailedJobWaitTime)
	  {
		this.asyncFailedJobWaitTime = asyncFailedJobWaitTime;
		return this;
	  }

	  public virtual bool EnableProcessDefinitionInfoCache
	  {
		  get
		  {
			return enableProcessDefinitionInfoCache;
		  }
	  }

	  public virtual ProcessEngineConfiguration setEnableProcessDefinitionInfoCache(bool enableProcessDefinitionInfoCache)
	  {
		this.enableProcessDefinitionInfoCache = enableProcessDefinitionInfoCache;
		return this;
	  }
	}

}