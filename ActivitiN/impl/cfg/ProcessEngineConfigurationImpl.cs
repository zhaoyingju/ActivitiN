using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

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

namespace org.activiti.engine.impl.cfg
{



	using BpmnModel = org.activiti.bpmn.model.BpmnModel;
	using ProcessEngineConfigurator = org.activiti.engine.cfg.ProcessEngineConfigurator;
	using ActivitiEventDispatcher = org.activiti.engine.@delegate.@event.ActivitiEventDispatcher;
	using ActivitiEventListener = org.activiti.engine.@delegate.@event.ActivitiEventListener;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventDispatcherImpl = org.activiti.engine.@delegate.@event.impl.ActivitiEventDispatcherImpl;
	using AbstractFormType = org.activiti.engine.form.AbstractFormType;
	using DefaultAsyncJobExecutor = org.activiti.engine.impl.asyncexecutor.DefaultAsyncJobExecutor;
	using ExecuteAsyncRunnableFactory = org.activiti.engine.impl.asyncexecutor.ExecuteAsyncRunnableFactory;
	using ItemInstance = org.activiti.engine.impl.bpmn.data.ItemInstance;
	using BpmnDeployer = org.activiti.engine.impl.bpmn.deployer.BpmnDeployer;
	using BpmnParseHandlers = org.activiti.engine.impl.bpmn.parser.BpmnParseHandlers;
	using BpmnParser = org.activiti.engine.impl.bpmn.parser.BpmnParser;
	using AbstractBehaviorFactory = org.activiti.engine.impl.bpmn.parser.factory.AbstractBehaviorFactory;
	using ActivityBehaviorFactory = org.activiti.engine.impl.bpmn.parser.factory.ActivityBehaviorFactory;
	using DefaultActivityBehaviorFactory = org.activiti.engine.impl.bpmn.parser.factory.DefaultActivityBehaviorFactory;
	using DefaultListenerFactory = org.activiti.engine.impl.bpmn.parser.factory.DefaultListenerFactory;
	using ListenerFactory = org.activiti.engine.impl.bpmn.parser.factory.ListenerFactory;
	using BoundaryEventParseHandler = org.activiti.engine.impl.bpmn.parser.handler.BoundaryEventParseHandler;
	using BusinessRuleParseHandler = org.activiti.engine.impl.bpmn.parser.handler.BusinessRuleParseHandler;
	using CallActivityParseHandler = org.activiti.engine.impl.bpmn.parser.handler.CallActivityParseHandler;
	using CancelEventDefinitionParseHandler = org.activiti.engine.impl.bpmn.parser.handler.CancelEventDefinitionParseHandler;
	using CompensateEventDefinitionParseHandler = org.activiti.engine.impl.bpmn.parser.handler.CompensateEventDefinitionParseHandler;
	using EndEventParseHandler = org.activiti.engine.impl.bpmn.parser.handler.EndEventParseHandler;
	using ErrorEventDefinitionParseHandler = org.activiti.engine.impl.bpmn.parser.handler.ErrorEventDefinitionParseHandler;
	using EventBasedGatewayParseHandler = org.activiti.engine.impl.bpmn.parser.handler.EventBasedGatewayParseHandler;
	using EventSubProcessParseHandler = org.activiti.engine.impl.bpmn.parser.handler.EventSubProcessParseHandler;
	using ExclusiveGatewayParseHandler = org.activiti.engine.impl.bpmn.parser.handler.ExclusiveGatewayParseHandler;
	using InclusiveGatewayParseHandler = org.activiti.engine.impl.bpmn.parser.handler.InclusiveGatewayParseHandler;
	using IntermediateCatchEventParseHandler = org.activiti.engine.impl.bpmn.parser.handler.IntermediateCatchEventParseHandler;
	using IntermediateThrowEventParseHandler = org.activiti.engine.impl.bpmn.parser.handler.IntermediateThrowEventParseHandler;
	using ManualTaskParseHandler = org.activiti.engine.impl.bpmn.parser.handler.ManualTaskParseHandler;
	using MessageEventDefinitionParseHandler = org.activiti.engine.impl.bpmn.parser.handler.MessageEventDefinitionParseHandler;
	using ParallelGatewayParseHandler = org.activiti.engine.impl.bpmn.parser.handler.ParallelGatewayParseHandler;
	using ProcessParseHandler = org.activiti.engine.impl.bpmn.parser.handler.ProcessParseHandler;
	using ReceiveTaskParseHandler = org.activiti.engine.impl.bpmn.parser.handler.ReceiveTaskParseHandler;
	using ScriptTaskParseHandler = org.activiti.engine.impl.bpmn.parser.handler.ScriptTaskParseHandler;
	using SendTaskParseHandler = org.activiti.engine.impl.bpmn.parser.handler.SendTaskParseHandler;
	using SequenceFlowParseHandler = org.activiti.engine.impl.bpmn.parser.handler.SequenceFlowParseHandler;
	using ServiceTaskParseHandler = org.activiti.engine.impl.bpmn.parser.handler.ServiceTaskParseHandler;
	using SignalEventDefinitionParseHandler = org.activiti.engine.impl.bpmn.parser.handler.SignalEventDefinitionParseHandler;
	using StartEventParseHandler = org.activiti.engine.impl.bpmn.parser.handler.StartEventParseHandler;
	using SubProcessParseHandler = org.activiti.engine.impl.bpmn.parser.handler.SubProcessParseHandler;
	using TaskParseHandler = org.activiti.engine.impl.bpmn.parser.handler.TaskParseHandler;
	using TimerEventDefinitionParseHandler = org.activiti.engine.impl.bpmn.parser.handler.TimerEventDefinitionParseHandler;
	using TransactionParseHandler = org.activiti.engine.impl.bpmn.parser.handler.TransactionParseHandler;
	using UserTaskParseHandler = org.activiti.engine.impl.bpmn.parser.handler.UserTaskParseHandler;
	using MessageInstance = org.activiti.engine.impl.bpmn.webservice.MessageInstance;
	using BusinessCalendarManager = org.activiti.engine.impl.calendar.BusinessCalendarManager;
	using CycleBusinessCalendar = org.activiti.engine.impl.calendar.CycleBusinessCalendar;
	using DueDateBusinessCalendar = org.activiti.engine.impl.calendar.DueDateBusinessCalendar;
	using DurationBusinessCalendar = org.activiti.engine.impl.calendar.DurationBusinessCalendar;
	using MapBusinessCalendarManager = org.activiti.engine.impl.calendar.MapBusinessCalendarManager;
	using StandaloneMybatisTransactionContextFactory = org.activiti.engine.impl.cfg.standalone.StandaloneMybatisTransactionContextFactory;
	using DbIdGenerator = org.activiti.engine.impl.db.DbIdGenerator;
	using DbSqlSessionFactory = org.activiti.engine.impl.db.DbSqlSessionFactory;
	using IbatisVariableTypeHandler = org.activiti.engine.impl.db.IbatisVariableTypeHandler;
	using DefaultDelegateInterceptor = org.activiti.engine.impl.@delegate.DefaultDelegateInterceptor;
	using ExpressionManager = org.activiti.engine.impl.el.ExpressionManager;
	using CompensationEventHandler = org.activiti.engine.impl.@event.CompensationEventHandler;
	using EventHandler = org.activiti.engine.impl.@event.EventHandler;
	using MessageEventHandler = org.activiti.engine.impl.@event.MessageEventHandler;
	using SignalEventHandler = org.activiti.engine.impl.@event.SignalEventHandler;
	using EventLogger = org.activiti.engine.impl.@event.logger.EventLogger;
	using BooleanFormType = org.activiti.engine.impl.form.BooleanFormType;
	using DateFormType = org.activiti.engine.impl.form.DateFormType;
	using DoubleFormType = org.activiti.engine.impl.form.DoubleFormType;
	using FormEngine = org.activiti.engine.impl.form.FormEngine;
	using FormTypes = org.activiti.engine.impl.form.FormTypes;
	using JuelFormEngine = org.activiti.engine.impl.form.JuelFormEngine;
	using LongFormType = org.activiti.engine.impl.form.LongFormType;
	using StringFormType = org.activiti.engine.impl.form.StringFormType;
	using HistoryLevel = org.activiti.engine.impl.history.HistoryLevel;
	using FlowNodeHistoryParseHandler = org.activiti.engine.impl.history.parse.FlowNodeHistoryParseHandler;
	using ProcessHistoryParseHandler = org.activiti.engine.impl.history.parse.ProcessHistoryParseHandler;
	using StartEventHistoryParseHandler = org.activiti.engine.impl.history.parse.StartEventHistoryParseHandler;
	using UserTaskHistoryParseHandler = org.activiti.engine.impl.history.parse.UserTaskHistoryParseHandler;
	using CommandConfig = org.activiti.engine.impl.interceptor.CommandConfig;
	using CommandContextFactory = org.activiti.engine.impl.interceptor.CommandContextFactory;
	using CommandContextInterceptor = org.activiti.engine.impl.interceptor.CommandContextInterceptor;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using CommandInterceptor = org.activiti.engine.impl.interceptor.CommandInterceptor;
	using CommandInvoker = org.activiti.engine.impl.interceptor.CommandInvoker;
	using DelegateInterceptor = org.activiti.engine.impl.interceptor.DelegateInterceptor;
	using LogInterceptor = org.activiti.engine.impl.interceptor.LogInterceptor;
	using SessionFactory = org.activiti.engine.impl.interceptor.SessionFactory;
	using AsyncContinuationJobHandler = org.activiti.engine.impl.jobexecutor.AsyncContinuationJobHandler;
	using CallerRunsRejectedJobsHandler = org.activiti.engine.impl.jobexecutor.CallerRunsRejectedJobsHandler;
	using DefaultFailedJobCommandFactory = org.activiti.engine.impl.jobexecutor.DefaultFailedJobCommandFactory;
	using DefaultJobExecutor = org.activiti.engine.impl.jobexecutor.DefaultJobExecutor;
	using FailedJobCommandFactory = org.activiti.engine.impl.jobexecutor.FailedJobCommandFactory;
	using JobHandler = org.activiti.engine.impl.jobexecutor.JobHandler;
	using ProcessEventJobHandler = org.activiti.engine.impl.jobexecutor.ProcessEventJobHandler;
	using RejectedJobsHandler = org.activiti.engine.impl.jobexecutor.RejectedJobsHandler;
	using TimerActivateProcessDefinitionHandler = org.activiti.engine.impl.jobexecutor.TimerActivateProcessDefinitionHandler;
	using TimerCatchIntermediateEventJobHandler = org.activiti.engine.impl.jobexecutor.TimerCatchIntermediateEventJobHandler;
	using TimerExecuteNestedActivityJobHandler = org.activiti.engine.impl.jobexecutor.TimerExecuteNestedActivityJobHandler;
	using TimerStartEventJobHandler = org.activiti.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using TimerSuspendProcessDefinitionHandler = org.activiti.engine.impl.jobexecutor.TimerSuspendProcessDefinitionHandler;
	using DefaultHistoryManagerSessionFactory = org.activiti.engine.impl.persistence.DefaultHistoryManagerSessionFactory;
	using GenericManagerFactory = org.activiti.engine.impl.persistence.GenericManagerFactory;
	using GroupEntityManagerFactory = org.activiti.engine.impl.persistence.GroupEntityManagerFactory;
	using MembershipEntityManagerFactory = org.activiti.engine.impl.persistence.MembershipEntityManagerFactory;
	using UserEntityManagerFactory = org.activiti.engine.impl.persistence.UserEntityManagerFactory;
	using org.activiti.engine.impl.persistence.deploy;
	using Deployer = org.activiti.engine.impl.persistence.deploy.Deployer;
	using org.activiti.engine.impl.persistence.deploy;
	using DeploymentManager = org.activiti.engine.impl.persistence.deploy.DeploymentManager;
	using ProcessDefinitionInfoCache = org.activiti.engine.impl.persistence.deploy.ProcessDefinitionInfoCache;
	using AttachmentEntityManager = org.activiti.engine.impl.persistence.entity.AttachmentEntityManager;
	using ByteArrayEntityManager = org.activiti.engine.impl.persistence.entity.ByteArrayEntityManager;
	using CommentEntityManager = org.activiti.engine.impl.persistence.entity.CommentEntityManager;
	using DeploymentEntityManager = org.activiti.engine.impl.persistence.entity.DeploymentEntityManager;
	using EventLogEntryEntityManager = org.activiti.engine.impl.persistence.entity.EventLogEntryEntityManager;
	using EventSubscriptionEntityManager = org.activiti.engine.impl.persistence.entity.EventSubscriptionEntityManager;
	using ExecutionEntityManager = org.activiti.engine.impl.persistence.entity.ExecutionEntityManager;
	using HistoricActivityInstanceEntityManager = org.activiti.engine.impl.persistence.entity.HistoricActivityInstanceEntityManager;
	using HistoricDetailEntityManager = org.activiti.engine.impl.persistence.entity.HistoricDetailEntityManager;
	using HistoricIdentityLinkEntityManager = org.activiti.engine.impl.persistence.entity.HistoricIdentityLinkEntityManager;
	using HistoricProcessInstanceEntityManager = org.activiti.engine.impl.persistence.entity.HistoricProcessInstanceEntityManager;
	using HistoricTaskInstanceEntityManager = org.activiti.engine.impl.persistence.entity.HistoricTaskInstanceEntityManager;
	using HistoricVariableInstanceEntityManager = org.activiti.engine.impl.persistence.entity.HistoricVariableInstanceEntityManager;
	using IdentityInfoEntityManager = org.activiti.engine.impl.persistence.entity.IdentityInfoEntityManager;
	using IdentityLinkEntityManager = org.activiti.engine.impl.persistence.entity.IdentityLinkEntityManager;
	using JobEntityManager = org.activiti.engine.impl.persistence.entity.JobEntityManager;
	using ModelEntityManager = org.activiti.engine.impl.persistence.entity.ModelEntityManager;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinitionEntityManager = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntityManager;
	using ProcessDefinitionInfoEntityManager = org.activiti.engine.impl.persistence.entity.ProcessDefinitionInfoEntityManager;
	using PropertyEntityManager = org.activiti.engine.impl.persistence.entity.PropertyEntityManager;
	using ResourceEntityManager = org.activiti.engine.impl.persistence.entity.ResourceEntityManager;
	using TableDataManager = org.activiti.engine.impl.persistence.entity.TableDataManager;
	using TaskEntityManager = org.activiti.engine.impl.persistence.entity.TaskEntityManager;
	using VariableInstanceEntityManager = org.activiti.engine.impl.persistence.entity.VariableInstanceEntityManager;
	using BeansResolverFactory = org.activiti.engine.impl.scripting.BeansResolverFactory;
	using ResolverFactory = org.activiti.engine.impl.scripting.ResolverFactory;
	using ScriptBindingsFactory = org.activiti.engine.impl.scripting.ScriptBindingsFactory;
	using ScriptingEngines = org.activiti.engine.impl.scripting.ScriptingEngines;
	using VariableScopeResolverFactory = org.activiti.engine.impl.scripting.VariableScopeResolverFactory;
	using DefaultClockImpl = org.activiti.engine.impl.util.DefaultClockImpl;
	using IoUtil = org.activiti.engine.impl.util.IoUtil;
	using ReflectUtil = org.activiti.engine.impl.util.ReflectUtil;
	using BooleanType = org.activiti.engine.impl.variable.BooleanType;
	using ByteArrayType = org.activiti.engine.impl.variable.ByteArrayType;
	using CustomObjectType = org.activiti.engine.impl.variable.CustomObjectType;
	using DateType = org.activiti.engine.impl.variable.DateType;
	using DefaultVariableTypes = org.activiti.engine.impl.variable.DefaultVariableTypes;
	using DoubleType = org.activiti.engine.impl.variable.DoubleType;
	using EntityManagerSession = org.activiti.engine.impl.variable.EntityManagerSession;
	using EntityManagerSessionFactory = org.activiti.engine.impl.variable.EntityManagerSessionFactory;
	using IntegerType = org.activiti.engine.impl.variable.IntegerType;
	using JPAEntityListVariableType = org.activiti.engine.impl.variable.JPAEntityListVariableType;
	using JPAEntityVariableType = org.activiti.engine.impl.variable.JPAEntityVariableType;
	using JsonType = org.activiti.engine.impl.variable.JsonType;
	using LongJsonType = org.activiti.engine.impl.variable.LongJsonType;
	using LongStringType = org.activiti.engine.impl.variable.LongStringType;
	using LongType = org.activiti.engine.impl.variable.LongType;
	using NullType = org.activiti.engine.impl.variable.NullType;
	using SerializableType = org.activiti.engine.impl.variable.SerializableType;
	using ShortType = org.activiti.engine.impl.variable.ShortType;
	using StringType = org.activiti.engine.impl.variable.StringType;
	using UUIDType = org.activiti.engine.impl.variable.UUIDType;
	using VariableType = org.activiti.engine.impl.variable.VariableType;
	using VariableTypes = org.activiti.engine.impl.variable.VariableTypes;
	using BpmnParseHandler = org.activiti.engine.parse.BpmnParseHandler;
	using DefaultProcessDiagramGenerator = org.activiti.image.impl.DefaultProcessDiagramGenerator;
	using ProcessValidator = org.activiti.validation.ProcessValidator;
	using ProcessValidatorFactory = org.activiti.validation.ProcessValidatorFactory;
	using ObjectUtils = org.apache.commons.lang3.ObjectUtils;
	using XMLConfigBuilder = org.apache.ibatis.builder.xml.XMLConfigBuilder;
	using XMLMapperBuilder = org.apache.ibatis.builder.xml.XMLMapperBuilder;
	using PooledDataSource = org.apache.ibatis.datasource.pooled.PooledDataSource;
	using Environment = org.apache.ibatis.mapping.Environment;
	using Configuration = org.apache.ibatis.session.Configuration;
	using SqlSessionFactory = org.apache.ibatis.session.SqlSessionFactory;
	using DefaultSqlSessionFactory = org.apache.ibatis.session.defaults.DefaultSqlSessionFactory;
	using TransactionFactory = org.apache.ibatis.transaction.TransactionFactory;
	using JdbcTransactionFactory = org.apache.ibatis.transaction.jdbc.JdbcTransactionFactory;
	using ManagedTransactionFactory = org.apache.ibatis.transaction.managed.ManagedTransactionFactory;
	using JdbcType = org.apache.ibatis.type.JdbcType;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public abstract class ProcessEngineConfigurationImpl : ProcessEngineConfiguration
	{
		private bool InstanceFieldsInitialized = false;

		public ProcessEngineConfigurationImpl()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			historyService = new HistoryServiceImpl(this);
			taskService = new TaskServiceImpl(this);
			dynamicBpmnService = new DynamicBpmnServiceImpl(this);
		}


	  private static Logger log = LoggerFactory.getLogger(typeof(ProcessEngineConfigurationImpl));

	  public const int DEFAULT_GENERIC_MAX_LENGTH_STRING = 4000;
	  public const int DEFAULT_ORACLE_MAX_LENGTH_STRING = 2000;

	  public const string DB_SCHEMA_UPDATE_CREATE = "create";
	  public const string DB_SCHEMA_UPDATE_DROP_CREATE = "drop-create";

	  public const string DEFAULT_WS_SYNC_FACTORY = "org.activiti.engine.impl.webservice.CxfWebServiceClientFactory";

	  public const string DEFAULT_MYBATIS_MAPPING_FILE = "org/activiti/db/mapping/mappings.xml";

	  // SERVICES /////////////////////////////////////////////////////////////////

	  protected internal RepositoryService repositoryService = new RepositoryServiceImpl();
	  protected internal RuntimeService runtimeService = new RuntimeServiceImpl();
	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService = new IdentityServiceImpl();
	  protected internal TaskService taskService;
	  protected internal FormService formService = new FormServiceImpl();
	  protected internal ManagementService managementService = new ManagementServiceImpl();
	  protected internal DynamicBpmnService dynamicBpmnService;

	  // COMMAND EXECUTORS ////////////////////////////////////////////////////////

	  protected internal CommandConfig defaultCommandConfig;
	  protected internal CommandConfig schemaCommandConfig;

	  protected internal CommandInterceptor commandInvoker;

	  /// <summary>
	  /// the configurable list which will be <seealso cref="#initInterceptorChain(java.util.List) processed"/> to build the <seealso cref="#commandExecutor"/> </summary>
	  protected internal IList<CommandInterceptor> customPreCommandInterceptors;
	  protected internal IList<CommandInterceptor> customPostCommandInterceptors;

	  protected internal IList<CommandInterceptor> commandInterceptors;

	  /// <summary>
	  /// this will be initialized during the configurationComplete() </summary>
	  protected internal CommandExecutor commandExecutor;

	  // SESSION FACTORIES ////////////////////////////////////////////////////////

	  protected internal IList<SessionFactory> customSessionFactories;
	  protected internal DbSqlSessionFactory dbSqlSessionFactory;
	  protected internal IDictionary<Type, SessionFactory> sessionFactories;

	  // Configurators ////////////////////////////////////////////////////////////

	  protected internal bool enableConfiguratorServiceLoader = true; // Enabled by default. In certain environments this should be set to false (eg osgi)
	  protected internal IList<ProcessEngineConfigurator> configurators; // The injected configurators
	  protected internal IList<ProcessEngineConfigurator> allConfigurators; // Including auto-discovered configurators

	  // DEPLOYERS ////////////////////////////////////////////////////////////////

	  protected internal BpmnDeployer bpmnDeployer;
	  protected internal BpmnParser bpmnParser;
	  protected internal IList<Deployer> customPreDeployers;
	  protected internal IList<Deployer> customPostDeployers;
	  protected internal IList<Deployer> deployers;
	  protected internal DeploymentManager deploymentManager;

	  protected internal int processDefinitionCacheLimit = -1; // By default, no limit
	  protected internal DeploymentCache<ProcessDefinitionEntity> processDefinitionCache;
	  protected internal int bpmnModelCacheLimit = -1; // By default, no limit
	  protected internal DeploymentCache<BpmnModel> bpmnModelCache;
	  protected internal int processDefinitionInfoCacheLimit = -1; // By default, no limit
	  protected internal ProcessDefinitionInfoCache processDefinitionInfoCache;

	  protected internal int knowledgeBaseCacheLimit = -1;
	  protected internal DeploymentCache<object> knowledgeBaseCache;

	  // JOB EXECUTOR /////////////////////////////////////////////////////////////

	  protected internal IList<JobHandler> customJobHandlers;
	  protected internal IDictionary<string, JobHandler> jobHandlers;

	  // ASYNC EXECUTOR ///////////////////////////////////////////////////////////

	  /// <summary>
	  /// The minimal number of threads that are kept alive in the threadpool for job execution. Default value = 2.
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal int asyncExecutorCorePoolSize = 2;

	  /// <summary>
	  /// The maximum number of threads that are kept alive in the threadpool for job execution. Default value = 10.
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal int asyncExecutorMaxPoolSize = 10;

	  /// <summary>
	  /// The time (in milliseconds) a thread used for job execution must be kept alive before it is
	  /// destroyed. Default setting is 5 seconds. Having a setting > 0 takes resources,
	  /// but in the case of many job executions it avoids creating new threads all the time.
	  /// If 0, threads will be destroyed after they've been used for job execution. 
	  /// 
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal long asyncExecutorThreadKeepAliveTime = 5000L;

		/// <summary>
		/// The size of the queue on which jobs to be executed are placed, before they are actually executed. Default value = 100.
		/// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
		/// </summary>
	  protected internal int asyncExecutorThreadPoolQueueSize = 100;

	  /// <summary>
	  /// The queue onto which jobs will be placed before they are actually executed.
	  /// Threads form the async executor threadpool will take work from this queue.
	  /// 
	  /// By default null. If null, an <seealso cref="ArrayBlockingQueue"/> will be created of size <seealso cref="#asyncExecutorThreadPoolQueueSize"/>.
	  /// 
	  /// When the queue is full, the job will be executed by the calling thread (ThreadPoolExecutor.CallerRunsPolicy())
	  /// 
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal BlockingQueue<Runnable> asyncExecutorThreadPoolQueue;

	  /// <summary>
	  /// The time (in seconds) that is waited to gracefully shut down the threadpool used for job execution
	  /// when the a shutdown on the executor (or process engine) is requested. Default value = 60.
	  /// 
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal long asyncExecutorSecondsToWaitOnShutdown = 60L;

	  /// <summary>
	  /// The number of timer jobs that are acquired during one query (before a job is executed, an acquirement thread 
	  /// fetches jobs from the database and puts them on the queue). 
	  /// 
	  /// Default value = 1, as this lowers the potential on optimistic locking exceptions. 
	  /// Change this value if you know what you are doing.
	  /// 
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal int asyncExecutorMaxTimerJobsPerAcquisition = 1;

	  /// <summary>
	  /// The number of async jobs that are acquired during one query (before a job is executed, an acquirement thread 
	  /// fetches jobs from the database and puts them on the queue). 
	  /// 
	  /// Default value = 1, as this lowers the potential on optimistic locking exceptions. 
	  /// Change this value if you know what you are doing.
	  /// 
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal int asyncExecutorMaxAsyncJobsDuePerAcquisition = 1;

	  /// <summary>
	  /// The time (in milliseconds) the timer acquisition thread will wait to execute the next acquirement query.
	  /// This happens when no new timer jobs were found or when less timer jobs have been fetched 
	  /// than set in <seealso cref="#asyncExecutorMaxTimerJobsPerAcquisition"/>. Default value = 10 seconds. 
	  /// 
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal int asyncExecutorDefaultTimerJobAcquireWaitTime = 10 * 1000;

	  /// <summary>
	  /// The time (in milliseconds) the async job acquisition thread will wait to execute the next acquirement query.
	  /// This happens when no new async jobs were found or when less async jobs have been fetched 
	  /// than set in <seealso cref="#asyncExecutorMaxAsyncJobsDuePerAcquisition"/>. Default value = 10 seconds. 
	  /// 
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal int asyncExecutorDefaultAsyncJobAcquireWaitTime = 10 * 1000;

	  /// <summary>
	  /// The time (in milliseconds) the async job (both timer and async continuations) acquisition thread will 
	  /// wait when the queueu is full to execute the next query. By default set to 0 (for backwards compatibility)
	  /// </summary>
	  protected internal int asyncExecutorDefaultQueueSizeFullWaitTime = 0;

	  /// <summary>
	  /// When a job is acquired, it is locked so other async executors can't lock and execute it.
	  /// While doing this, the 'name' of the lock owner is written into a column of the job.
	  /// 
	  /// By default, a random UUID will be generated when the executor is created.
	  /// 
	  /// It is important that each async executor instance in a cluster of Activiti engines
	  /// has a different name!
	  /// 
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal string asyncExecutorLockOwner;

	  /// <summary>
	  /// The amount of time (in milliseconds) a timer job is locked when acquired by the async executor.
	  /// During this period of time, no other async executor will try to acquire and lock this job.
	  /// 
	  /// Default value = 5 minutes;
	  /// 
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal int asyncExecutorTimerLockTimeInMillis = 5 * 60 * 1000;

	  /// <summary>
	  /// The amount of time (in milliseconds) an async job is locked when acquired by the async executor.
	  /// During this period of time, no other async executor will try to acquire and lock this job.
	  /// 
	  /// Default value = 5 minutes;
	  /// 
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal int asyncExecutorAsyncJobLockTimeInMillis = 5 * 60 * 1000;

	  /// <summary>
	  /// The amount of time (in milliseconds) that is waited before trying locking again,
	  /// when an exclusive job is tried to be locked, but fails and the locking.
	  /// 
	  /// Default value = 500. If 0, this would stress database traffic a lot in case when a retry is needed,
	  /// as exclusive jobs would be constantly tried to be locked.
	  /// 
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal int asyncExecutorLockRetryWaitTimeInMillis = 500;

	  /// <summary>
	  /// Allows to define a custom factory for creating the <seealso cref="Runnable"/> that is executed by the async executor.
	  /// 
	  /// (This property is only applicable when using the <seealso cref="DefaultAsyncJobExecutor"/>).
	  /// </summary>
	  protected internal ExecuteAsyncRunnableFactory asyncExecutorExecuteAsyncRunnableFactory;

	  // MYBATIS SQL SESSION FACTORY //////////////////////////////////////////////

	  protected internal SqlSessionFactory sqlSessionFactory;
	  protected internal TransactionFactory transactionFactory;

	  protected internal Set<Type> customMybatisMappers;
	  protected internal Set<string> customMybatisXMLMappers;

	  // ID GENERATOR /////////////////////////////////////////////////////////////

	  protected internal IdGenerator idGenerator;
	  protected internal DataSource idGeneratorDataSource;
	  protected internal string idGeneratorDataSourceJndiName;

	  // BPMN PARSER //////////////////////////////////////////////////////////////

	  protected internal IList<BpmnParseHandler> preBpmnParseHandlers;
	  protected internal IList<BpmnParseHandler> postBpmnParseHandlers;
	  protected internal IList<BpmnParseHandler> customDefaultBpmnParseHandlers;
	  protected internal ActivityBehaviorFactory activityBehaviorFactory;
	  protected internal ListenerFactory listenerFactory;
	  protected internal BpmnParseFactory bpmnParseFactory;

	  // PROCESS VALIDATION 

	  protected internal ProcessValidator processValidator;

	  // OTHER ////////////////////////////////////////////////////////////////////

	  protected internal IList<FormEngine> customFormEngines;
	  protected internal IDictionary<string, FormEngine> formEngines;

	  protected internal IList<AbstractFormType> customFormTypes;
	  protected internal FormTypes formTypes;

	  protected internal IList<VariableType> customPreVariableTypes;
	  protected internal IList<VariableType> customPostVariableTypes;
	  protected internal VariableTypes variableTypes;

	  protected internal ExpressionManager expressionManager;
	  protected internal IList<string> customScriptingEngineClasses;
	  protected internal ScriptingEngines scriptingEngines;
	  protected internal IList<ResolverFactory> resolverFactories;

	  protected internal BusinessCalendarManager businessCalendarManager;

	  protected internal int executionQueryLimit = 20000;
	  protected internal int taskQueryLimit = 20000;
	  protected internal int historicTaskQueryLimit = 20000;
	  protected internal int historicProcessInstancesQueryLimit = 20000;

	  protected internal string wsSyncFactoryClassName = DEFAULT_WS_SYNC_FACTORY;
	  protected internal ConcurrentMap<QName, URL> wsOverridenEndpointAddresses = new ConcurrentDictionary<QName, URL>();

	  protected internal CommandContextFactory commandContextFactory;
	  protected internal TransactionContextFactory transactionContextFactory;

	  protected internal IDictionary<object, object> beans;

	  protected internal DelegateInterceptor delegateInterceptor;

	  protected internal RejectedJobsHandler customRejectedJobsHandler;

	  protected internal IDictionary<string, EventHandler> eventHandlers;
	  protected internal IList<EventHandler> customEventHandlers;

	  protected internal FailedJobCommandFactory failedJobCommandFactory;

	  /// <summary>
	  /// Set this to true if you want to have extra checks on the BPMN xml that is parsed.
	  /// See http://www.jorambarrez.be/blog/2013/02/19/uploading-a-funny-xml-can-bring-down-your-server/
	  /// 
	  /// Unfortunately, this feature is not available on some platforms (JDK 6, JBoss),
	  /// hence the reason why it is disabled by default. If your platform allows 
	  /// the use of StaxSource during XML parsing, do enable it.
	  /// </summary>
	  protected internal bool enableSafeBpmnXml = false;

	  /// <summary>
	  /// The following settings will determine the amount of entities loaded at once when the engine 
	  /// needs to load multiple entities (eg. when suspending a process definition with all its process instances).
	  /// 
	  /// The default setting is quite low, as not to surprise anyone with sudden memory spikes.
	  /// Change it to something higher if the environment Activiti runs in allows it.
	  /// </summary>
	  protected internal int batchSizeProcessInstances = 25;
	  protected internal int batchSizeTasks = 25;

	  /// <summary>
	  /// If set to true, enables bulk insert (grouping sql inserts together).
	  /// Default true. For some databases (eg DB2 on Zos: https://activiti.atlassian.net/browse/ACT-4042) needs to be set to false
	  /// </summary>
	  protected internal bool isBulkInsertEnabled = true;

	  /// <summary>
	  /// Some databases have a limit of how many parameters one sql insert can have (eg SQL Server, 2000 params (!= insert statements) ).
	  /// Tweak this parameter in case of exceptions indicating too much is being put into one bulk insert,
	  /// or make it higher if your database can cope with it and there are inserts with a huge amount of data.
	  /// 
	  /// By default: 100.
	  /// </summary>
	  protected internal int maxNrOfStatementsInBulkInsert = 100;

	  protected internal bool enableEventDispatcher = true;
	  protected internal ActivitiEventDispatcher eventDispatcher;
	  protected internal IList<ActivitiEventListener> eventListeners;
	  protected internal IDictionary<string, IList<ActivitiEventListener>> typedEventListeners;

	  // Event logging to database
	  protected internal bool enableDatabaseEventLogging = false;

	  /// <summary>
	  /// Using field injection together with a delegate expression for a service
	  /// task / execution listener / task listener is not thread-sade , see user
	  /// guide section 'Field Injection' for more information.
	  /// 
	  /// Set this flag to false to throw an exception at runtime when a field is
	  /// injected and a delegateExpression is used. Default is true for backwards compatibility.
	  /// 
	  /// @since 5.21
	  /// </summary>
	  protected internal DelegateExpressionFieldInjectionMode delegateExpressionFieldInjectionMode = DelegateExpressionFieldInjectionMode.COMPATIBILITY;

	  /// <summary>
	  ///  Define a max length for storing String variable types in the database.
	  ///  Mainly used for the Oracle NVARCHAR2 limit of 2000 characters
	  /// </summary>
	  protected internal int maxLengthStringVariableType = -1;

	  protected internal ObjectMapper objectMapper = new ObjectMapper();

	  // buildProcessEngine ///////////////////////////////////////////////////////

	  public override ProcessEngine buildProcessEngine()
	  {
		init();
		return new ProcessEngineImpl(this);
	  }

	  // init /////////////////////////////////////////////////////////////////////

	  protected internal virtual void init()
	  {
		  initConfigurators();
		  configuratorsBeforeInit();
		initProcessDiagramGenerator();
		initHistoryLevel();
		initExpressionManager();
		initDataSource();
		initVariableTypes();
		initBeans();
		initFormEngines();
		initFormTypes();
		initScriptingEngines();
		initClock();
		initBusinessCalendarManager();
		initCommandContextFactory();
		initTransactionContextFactory();
		initCommandExecutors();
		initServices();
		initIdGenerator();
		initDeployers();
		initJobHandlers();
		initJobExecutor();
		initAsyncExecutor();
		initTransactionFactory();
		initSqlSessionFactory();
		initSessionFactories();
		initJpa();
		initDelegateInterceptor();
		initEventHandlers();
		initFailedJobCommandFactory();
		initEventDispatcher();
		initProcessValidator();
		initDatabaseEventLogging();
		configuratorsAfterInit();
	  }

	  // failedJobCommandFactory ////////////////////////////////////////////////////////

	  protected internal virtual void initFailedJobCommandFactory()
	  {
		if (failedJobCommandFactory == null)
		{
		  failedJobCommandFactory = new DefaultFailedJobCommandFactory();
		}
	  }

	  // command executors ////////////////////////////////////////////////////////

	  protected internal virtual void initCommandExecutors()
	  {
		initDefaultCommandConfig();
		initSchemaCommandConfig();
		initCommandInvoker();
		initCommandInterceptors();
		initCommandExecutor();
	  }

	  protected internal virtual void initDefaultCommandConfig()
	  {
		if (defaultCommandConfig == null)
		{
		  defaultCommandConfig = new CommandConfig();
		}
	  }

	  private void initSchemaCommandConfig()
	  {
		if (schemaCommandConfig == null)
		{
		  schemaCommandConfig = (new CommandConfig()).transactionNotSupported();
		}
	  }

	  protected internal virtual void initCommandInvoker()
	  {
		if (commandInvoker == null)
		{
		  commandInvoker = new CommandInvoker();
		}
	  }

	  protected internal virtual void initCommandInterceptors()
	  {
		if (commandInterceptors == null)
		{
		  commandInterceptors = new List<CommandInterceptor>();
		  if (customPreCommandInterceptors != null)
		  {
			commandInterceptors.AddRange(customPreCommandInterceptors);
		  }
		  commandInterceptors.AddRange(DefaultCommandInterceptors);
		  if (customPostCommandInterceptors != null)
		  {
			commandInterceptors.AddRange(customPostCommandInterceptors);
		  }
		  commandInterceptors.Add(commandInvoker);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: protected java.util.Collection< ? extends org.activiti.engine.impl.interceptor.CommandInterceptor> getDefaultCommandInterceptors()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: protected java.util.Collection< ? extends org.activiti.engine.impl.interceptor.CommandInterceptor> getDefaultCommandInterceptors()
	  protected internal virtual ICollection<?> DefaultCommandInterceptors where ? : org.activiti.engine.impl.interceptor.CommandInterceptor
	  {
		  get
		  {
			IList<CommandInterceptor> interceptors = new List<CommandInterceptor>();
			interceptors.Add(new LogInterceptor());
    
			CommandInterceptor transactionInterceptor = createTransactionInterceptor();
			if (transactionInterceptor != null)
			{
			  interceptors.Add(transactionInterceptor);
			}
    
			interceptors.Add(new CommandContextInterceptor(commandContextFactory, this));
			return interceptors;
		  }
	  }

	  protected internal virtual void initCommandExecutor()
	  {
		if (commandExecutor == null)
		{
		  CommandInterceptor first = initInterceptorChain(commandInterceptors);
		  commandExecutor = new CommandExecutorImpl(DefaultCommandConfig, first);
		}
	  }

	  protected internal virtual CommandInterceptor initInterceptorChain(IList<CommandInterceptor> chain)
	  {
		if (chain == null || chain.Count == 0)
		{
		  throw new ActivitiException("invalid command interceptor chain configuration: " + chain);
		}
		for (int i = 0; i < chain.Count - 1; i++)
		{
		  chain[i].setNext(chain[i + 1]);
		}
		return chain[0];
	  }

	  protected internal abstract CommandInterceptor createTransactionInterceptor();

	  // services /////////////////////////////////////////////////////////////////

	  protected internal virtual void initServices()
	  {
		initService(repositoryService);
		initService(runtimeService);
		initService(historyService);
		initService(identityService);
		initService(taskService);
		initService(formService);
		initService(managementService);
		initService(dynamicBpmnService);
	  }

	  protected internal virtual void initService(object service)
	  {
		if (service is ServiceImpl)
		{
		  ((ServiceImpl)service).CommandExecutor = commandExecutor;
		}
	  }

	  // DataSource ///////////////////////////////////////////////////////////////

	  protected internal virtual void initDataSource()
	  {
		if (dataSource == null)
		{
		  if (dataSourceJndiName != null)
		  {
			try
			{
			  dataSource = (DataSource) (new InitialContext()).lookup(dataSourceJndiName);
			}
			catch (Exception e)
			{
			  throw new ActivitiException("couldn't lookup datasource from " + dataSourceJndiName + ": " + e.Message, e);
			}

		  }
		  else if (jdbcUrl != null)
		  {
			if ((jdbcDriver == null) || (jdbcUrl == null) || (jdbcUsername == null))
			{
			  throw new ActivitiException("DataSource or JDBC properties have to be specified in a process engine configuration");
			}

			log.debug("initializing datasource to db: {}", jdbcUrl);

			PooledDataSource pooledDataSource = new PooledDataSource(ReflectUtil.ClassLoader, jdbcDriver, jdbcUrl, jdbcUsername, jdbcPassword);

			if (jdbcMaxActiveConnections > 0)
			{
			  pooledDataSource.PoolMaximumActiveConnections = jdbcMaxActiveConnections;
			}
			if (jdbcMaxIdleConnections > 0)
			{
			  pooledDataSource.PoolMaximumIdleConnections = jdbcMaxIdleConnections;
			}
			if (jdbcMaxCheckoutTime > 0)
			{
			  pooledDataSource.PoolMaximumCheckoutTime = jdbcMaxCheckoutTime;
			}
			if (jdbcMaxWaitTime > 0)
			{
			  pooledDataSource.PoolTimeToWait = jdbcMaxWaitTime;
			}
			if (jdbcPingEnabled == true)
			{
			  pooledDataSource.PoolPingEnabled = true;
			  if (jdbcPingQuery != null)
			  {
				pooledDataSource.PoolPingQuery = jdbcPingQuery;
			  }
			  pooledDataSource.PoolPingConnectionsNotUsedFor = jdbcPingConnectionNotUsedFor;
			}
			if (jdbcDefaultTransactionIsolationLevel > 0)
			{
			  pooledDataSource.DefaultTransactionIsolationLevel = jdbcDefaultTransactionIsolationLevel;
			}
			dataSource = pooledDataSource;
		  }

		  if (dataSource is PooledDataSource)
		  {
			// ACT-233: connection pool of Ibatis is not properely initialized if this is not called!
			((PooledDataSource)dataSource).forceCloseAll();
		  }
		}

		if (databaseType == null)
		{
		  initDatabaseType();
		}
	  }

	  protected internal static Properties databaseTypeMappings = DefaultDatabaseTypeMappings;

	  public const string DATABASE_TYPE_H2 = "h2";
	  public const string DATABASE_TYPE_HSQL = "hsql";
	  public const string DATABASE_TYPE_MYSQL = "mysql";
	  public const string DATABASE_TYPE_ORACLE = "oracle";
	  public const string DATABASE_TYPE_POSTGRES = "postgres";
	  public const string DATABASE_TYPE_MSSQL = "mssql";
	  public const string DATABASE_TYPE_DB2 = "db2";

	  protected internal static Properties DefaultDatabaseTypeMappings
	  {
		  get
		  {
			Properties databaseTypeMappings = new Properties();
			databaseTypeMappings.setProperty("H2", DATABASE_TYPE_H2);
			databaseTypeMappings.setProperty("HSQL Database Engine", DATABASE_TYPE_HSQL);
			databaseTypeMappings.setProperty("MySQL", DATABASE_TYPE_MYSQL);
			databaseTypeMappings.setProperty("Oracle", DATABASE_TYPE_ORACLE);
			databaseTypeMappings.setProperty("PostgreSQL", DATABASE_TYPE_POSTGRES);
			databaseTypeMappings.setProperty("Microsoft SQL Server", DATABASE_TYPE_MSSQL);
			databaseTypeMappings.setProperty(DATABASE_TYPE_DB2,DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/NT",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/NT64",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2 UDP",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/LINUX",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/LINUX390",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/LINUXX8664",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/LINUXZ64",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/LINUXPPC64",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/LINUXPPC64LE",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/400 SQL",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/6000",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2 UDB iSeries",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/AIX64",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/HPUX",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/HP64",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/SUN",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/SUN64",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/PTX",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2/2",DATABASE_TYPE_DB2);
			databaseTypeMappings.setProperty("DB2 UDB AS400", DATABASE_TYPE_DB2);
			return databaseTypeMappings;
		  }
	  }

	  public virtual void initDatabaseType()
	  {
		Connection connection = null;
		try
		{
		  connection = dataSource.Connection;
		  DatabaseMetaData databaseMetaData = connection.MetaData;
		  string databaseProductName = databaseMetaData.DatabaseProductName;
		  log.debug("database product name: '{}'", databaseProductName);
		  databaseType = databaseTypeMappings.getProperty(databaseProductName);
		  if (databaseType == null)
		  {
			throw new ActivitiException("couldn't deduct database type from database product name '" + databaseProductName + "'");
		  }
		  log.debug("using database type: {}", databaseType);

		}
		catch (SQLException e)
		{
		  log.error("Exception while initializing Database connection", e);
		}
		finally
		{
		  try
		  {
			if (connection != null)
			{
			  connection.close();
			}
		  }
		  catch (SQLException e)
		  {
			  log.error("Exception while closing the Database connection", e);
		  }
		}
	  }

	  // myBatis SqlSessionFactory ////////////////////////////////////////////////

	  protected internal virtual void initTransactionFactory()
	  {
		if (transactionFactory == null)
		{
		  if (transactionsExternallyManaged)
		  {
			transactionFactory = new ManagedTransactionFactory();
		  }
		  else
		  {
			transactionFactory = new JdbcTransactionFactory();
		  }
		}
	  }

	  protected internal virtual void initSqlSessionFactory()
	  {
		if (sqlSessionFactory == null)
		{
		  InputStream inputStream = null;
		  try
		  {
			inputStream = MyBatisXmlConfigurationSteam;

			// update the jdbc parameters to the configured ones...
			Environment environment = new Environment("default", transactionFactory, dataSource);
			Reader reader = new InputStreamReader(inputStream);
			Properties properties = new Properties();
			properties.put("prefix", databaseTablePrefix);
			string wildcardEscapeClause = "";
			if ((databaseWildcardEscapeCharacter != null) && (databaseWildcardEscapeCharacter.Length != 0))
			{
			  wildcardEscapeClause = " escape '" + databaseWildcardEscapeCharacter + "'";
			}
			properties.put("wildcardEscapeClause", wildcardEscapeClause);
			if (databaseType != null)
			{
			  properties.put("limitBefore", DbSqlSessionFactory.databaseSpecificLimitBeforeStatements[databaseType]);
			  properties.put("limitAfter", DbSqlSessionFactory.databaseSpecificLimitAfterStatements[databaseType]);
			  properties.put("limitBetween", DbSqlSessionFactory.databaseSpecificLimitBetweenStatements[databaseType]);
			  properties.put("limitOuterJoinBetween", DbSqlSessionFactory.databaseOuterJoinLimitBetweenStatements[databaseType]);
			  properties.put("orderBy", DbSqlSessionFactory.databaseSpecificOrderByStatements[databaseType]);
			  properties.put("limitBeforeNativeQuery", ObjectUtils.ToString(DbSqlSessionFactory.databaseSpecificLimitBeforeNativeQueryStatements[databaseType]));
			}

			Configuration configuration = initMybatisConfiguration(environment, reader, properties);
			sqlSessionFactory = new DefaultSqlSessionFactory(configuration);

		  }
		  catch (Exception e)
		  {
			throw new ActivitiException("Error while building ibatis SqlSessionFactory: " + e.Message, e);
		  }
		  finally
		  {
			IoUtil.closeSilently(inputStream);
		  }
		}
	  }

		protected internal virtual Configuration initMybatisConfiguration(Environment environment, Reader reader, Properties properties)
		{
		  XMLConfigBuilder parser = new XMLConfigBuilder(reader,"", properties);
		  Configuration configuration = parser.Configuration;
		  configuration.Environment = environment;

		  initMybatisTypeHandlers(configuration);
		  initCustomMybatisMappers(configuration);

		  configuration = parseMybatisConfiguration(configuration, parser);
		  return configuration;
		}

		protected internal virtual void initMybatisTypeHandlers(Configuration configuration)
		{
		  configuration.TypeHandlerRegistry.register(typeof(VariableType), JdbcType.VARCHAR, new IbatisVariableTypeHandler());
		}

		protected internal virtual void initCustomMybatisMappers(Configuration configuration)
		{
		  if (CustomMybatisMappers != null)
		  {
			  foreach (Type clazz in CustomMybatisMappers)
			  {
				  configuration.addMapper(clazz);
			  }
		  }
		}

		protected internal virtual Configuration parseMybatisConfiguration(Configuration configuration, XMLConfigBuilder parser)
		{
		  return parseCustomMybatisXMLMappers(parser.parse());
		}

		protected internal virtual Configuration parseCustomMybatisXMLMappers(Configuration configuration)
		{
		  if (CustomMybatisXMLMappers != null)
		  {
		// see XMLConfigBuilder.mapperElement()
		foreach (string resource in CustomMybatisXMLMappers)
		{
		  XMLMapperBuilder mapperParser = new XMLMapperBuilder(getResourceAsStream(resource), configuration, resource, configuration.SqlFragments);
		  mapperParser.parse();
		}
		  }
		return configuration;
		}

		protected internal virtual InputStream getResourceAsStream(string resource)
		{
		return ReflectUtil.getResourceAsStream(resource);
		}

	  protected internal virtual InputStream MyBatisXmlConfigurationSteam
	  {
		  get
		  {
			return getResourceAsStream(DEFAULT_MYBATIS_MAPPING_FILE);
		  }
	  }

	  public virtual Set<Type> CustomMybatisMappers
	  {
		  get
		  {
			return customMybatisMappers;
		  }
		  set
		  {
			this.customMybatisMappers = value;
		  }
	  }


	  public virtual Set<string> CustomMybatisXMLMappers
	  {
		  get
		  {
			return customMybatisXMLMappers;
		  }
		  set
		  {
			this.customMybatisXMLMappers = value;
		  }
	  }


	  // session factories ////////////////////////////////////////////////////////


	  protected internal virtual void initSessionFactories()
	  {
		if (sessionFactories == null)
		{
		  sessionFactories = new Dictionary<Type, SessionFactory>();

		  if (dbSqlSessionFactory == null)
		  {
			dbSqlSessionFactory = new DbSqlSessionFactory();
		  }
		  dbSqlSessionFactory.DatabaseType = databaseType;
		  dbSqlSessionFactory.IdGenerator = idGenerator;
		  dbSqlSessionFactory.SqlSessionFactory = sqlSessionFactory;
		  dbSqlSessionFactory.DbIdentityUsed = isDbIdentityUsed;
		  dbSqlSessionFactory.DbHistoryUsed = isDbHistoryUsed;
		  dbSqlSessionFactory.DatabaseTablePrefix = databaseTablePrefix;
		  dbSqlSessionFactory.TablePrefixIsSchema = tablePrefixIsSchema;
		  dbSqlSessionFactory.DatabaseCatalog = databaseCatalog;
		  dbSqlSessionFactory.DatabaseSchema = databaseSchema;
		  dbSqlSessionFactory.setBulkInsertEnabled(isBulkInsertEnabled, databaseType);
		  dbSqlSessionFactory.MaxNrOfStatementsInBulkInsert = maxNrOfStatementsInBulkInsert;
		  addSessionFactory(dbSqlSessionFactory);

		  addSessionFactory(new GenericManagerFactory(typeof(AttachmentEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(CommentEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(DeploymentEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(ModelEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(ExecutionEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricActivityInstanceEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricDetailEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricProcessInstanceEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricVariableInstanceEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricTaskInstanceEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricIdentityLinkEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(IdentityInfoEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(IdentityLinkEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(JobEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(ProcessDefinitionEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(ProcessDefinitionInfoEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(PropertyEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(ResourceEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(ByteArrayEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(TableDataManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(TaskEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(VariableInstanceEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(EventSubscriptionEntityManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(EventLogEntryEntityManager)));

		  addSessionFactory(new DefaultHistoryManagerSessionFactory());

		  addSessionFactory(new UserEntityManagerFactory());
		  addSessionFactory(new GroupEntityManagerFactory());
		  addSessionFactory(new MembershipEntityManagerFactory());
		}

		if (customSessionFactories != null)
		{
		  foreach (SessionFactory sessionFactory in customSessionFactories)
		  {
			addSessionFactory(sessionFactory);
		  }
		}
	  }

	  protected internal virtual void addSessionFactory(SessionFactory sessionFactory)
	  {
		sessionFactories[sessionFactory.SessionType] = sessionFactory;
	  }

	  protected internal virtual void initConfigurators()
	  {

		  allConfigurators = new List<ProcessEngineConfigurator>();

		  // Configurators that are explicitely added to the config
		if (configurators != null)
		{
		  foreach (ProcessEngineConfigurator configurator in configurators)
		  {
			allConfigurators.Add(configurator);
		  }
		}

		// Auto discovery through ServiceLoader
		if (enableConfiguratorServiceLoader)
		{
			ClassLoader classLoader = ClassLoader;
			if (classLoader == null)
			{
				classLoader = ReflectUtil.ClassLoader;
			}

			ServiceLoader<ProcessEngineConfigurator> configuratorServiceLoader = ServiceLoader.load(typeof(ProcessEngineConfigurator), classLoader);
			int nrOfServiceLoadedConfigurators = 0;
			foreach (ProcessEngineConfigurator configurator in configuratorServiceLoader)
			{
				allConfigurators.Add(configurator);
				nrOfServiceLoadedConfigurators++;
			}

			if (nrOfServiceLoadedConfigurators > 0)
			{
				log.info("Found {} auto-discoverable Process Engine Configurator{}", nrOfServiceLoadedConfigurators++, nrOfServiceLoadedConfigurators > 1 ? "s" : "");
			}

			if (allConfigurators.Count > 0)
			{

				// Order them according to the priorities (usefule for dependent configurator)
				allConfigurators.Sort(new ComparatorAnonymousInnerClassHelper(this));

				// Execute the configurators
				log.info("Found {} Process Engine Configurators in total:", allConfigurators.Count);
				foreach (ProcessEngineConfigurator configurator in allConfigurators)
				{
					log.info("{} (priority:{})", configurator.GetType(), configurator.Priority);
				}

			}

		}
	  }

	  private class ComparatorAnonymousInnerClassHelper : IComparer<ProcessEngineConfigurator>
	  {
		  private readonly ProcessEngineConfigurationImpl outerInstance;

		  public ComparatorAnonymousInnerClassHelper(ProcessEngineConfigurationImpl outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public virtual int Compare(ProcessEngineConfigurator configurator1, ProcessEngineConfigurator configurator2)
		  {
			  int priority1 = configurator1.Priority;
			  int priority2 = configurator2.Priority;

			  if (priority1 < priority2)
			  {
				  return -1;
			  }
			  else if (priority1 > priority2)
			  {
				  return 1;
			  }
			  return 0;
		  }
	  }

	  protected internal virtual void configuratorsBeforeInit()
	  {
		  foreach (ProcessEngineConfigurator configurator in allConfigurators)
		  {
			  log.info("Executing beforeInit() of {} (priority:{})", configurator.GetType(), configurator.Priority);
			  configurator.beforeInit(this);
		  }
	  }

	  protected internal virtual void configuratorsAfterInit()
	  {
		  foreach (ProcessEngineConfigurator configurator in allConfigurators)
		  {
			  log.info("Executing configure() of {} (priority:{})", configurator.GetType(), configurator.Priority);
			  configurator.configure(this);
		  }
	  }

	  // deployers ////////////////////////////////////////////////////////////////

	  protected internal virtual void initDeployers()
	  {
		if (this.deployers == null)
		{
		  this.deployers = new List<Deployer>();
		  if (customPreDeployers != null)
		  {
			this.deployers.AddRange(customPreDeployers);
		  }
		  this.deployers.AddRange(DefaultDeployers);
		  if (customPostDeployers != null)
		  {
			this.deployers.AddRange(customPostDeployers);
		  }
		}
		if (deploymentManager == null)
		{
		  deploymentManager = new DeploymentManager();
		  deploymentManager.Deployers = deployers;

		  // Process Definition cache
		  if (processDefinitionCache == null)
		  {
			if (processDefinitionCacheLimit <= 0)
			{
			  processDefinitionCache = new DefaultDeploymentCache<ProcessDefinitionEntity>();
			}
			else
			{
			  processDefinitionCache = new DefaultDeploymentCache<ProcessDefinitionEntity>(processDefinitionCacheLimit);
			}
		  }

		  // BpmnModel cache
		  if (bpmnModelCache == null)
		  {
			if (bpmnModelCacheLimit <= 0)
			{
			  bpmnModelCache = new DefaultDeploymentCache<BpmnModel>();
			}
			else
			{
			  bpmnModelCache = new DefaultDeploymentCache<BpmnModel>(bpmnModelCacheLimit);
			}
		  }

		  if (processDefinitionInfoCache == null)
		  {
			if (processDefinitionInfoCacheLimit <= 0)
			{
			  processDefinitionInfoCache = new ProcessDefinitionInfoCache(commandExecutor);
			}
			else
			{
			  processDefinitionInfoCache = new ProcessDefinitionInfoCache(commandExecutor, processDefinitionInfoCacheLimit);
			}
		  }

		  // Knowledge base cache (used for Drools business task)
		  if (knowledgeBaseCache == null)
		  {
			if (knowledgeBaseCacheLimit <= 0)
			{
			  knowledgeBaseCache = new DefaultDeploymentCache<object>();
			}
			else
			{
			  knowledgeBaseCache = new DefaultDeploymentCache<object>(knowledgeBaseCacheLimit);
			}
		  }

		  deploymentManager.setProcessDefinitionCache(processDefinitionCache);
		  deploymentManager.setBpmnModelCache(bpmnModelCache);
		  deploymentManager.setProcessDefinitionInfoCache(processDefinitionInfoCache);
		  deploymentManager.setKnowledgeBaseCache(knowledgeBaseCache);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: protected java.util.Collection< ? extends org.activiti.engine.impl.persistence.deploy.Deployer> getDefaultDeployers()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: protected java.util.Collection< ? extends org.activiti.engine.impl.persistence.deploy.Deployer> getDefaultDeployers()
	  protected internal virtual ICollection<?> DefaultDeployers where ? : org.activiti.engine.impl.persistence.deploy.Deployer
	  {
		  get
		  {
			IList<Deployer> defaultDeployers = new List<Deployer>();
    
			if (bpmnDeployer == null)
			{
			  bpmnDeployer = new BpmnDeployer();
			}
    
			bpmnDeployer.ExpressionManager = expressionManager;
			bpmnDeployer.IdGenerator = idGenerator;
    
			if (bpmnParseFactory == null)
			{
			  bpmnParseFactory = new DefaultBpmnParseFactory();
			}
    
			if (activityBehaviorFactory == null)
			{
			  DefaultActivityBehaviorFactory defaultActivityBehaviorFactory = new DefaultActivityBehaviorFactory();
			  defaultActivityBehaviorFactory.ExpressionManager = expressionManager;
			  activityBehaviorFactory = defaultActivityBehaviorFactory;
			}
			else if ((activityBehaviorFactory is AbstractBehaviorFactory) && ((AbstractBehaviorFactory) activityBehaviorFactory).ExpressionManager == null)
			{
				((AbstractBehaviorFactory) activityBehaviorFactory).ExpressionManager = expressionManager;
			}
    
			if (listenerFactory == null)
			{
			  DefaultListenerFactory defaultListenerFactory = new DefaultListenerFactory();
			  defaultListenerFactory.ExpressionManager = expressionManager;
			  listenerFactory = defaultListenerFactory;
			}
			else if ((listenerFactory is AbstractBehaviorFactory) && ((AbstractBehaviorFactory) listenerFactory).ExpressionManager == null)
			{
				((AbstractBehaviorFactory) listenerFactory).ExpressionManager = expressionManager;
			}
    
			if (bpmnParser == null)
			{
			  bpmnParser = new BpmnParser();
			}
    
			bpmnParser.ExpressionManager = expressionManager;
			bpmnParser.BpmnParseFactory = bpmnParseFactory;
			bpmnParser.ActivityBehaviorFactory = activityBehaviorFactory;
			bpmnParser.ListenerFactory = listenerFactory;
    
			IList<BpmnParseHandler> parseHandlers = new List<BpmnParseHandler>();
			if (PreBpmnParseHandlers != null)
			{
			  parseHandlers.AddRange(PreBpmnParseHandlers);
			}
			parseHandlers.AddRange(DefaultBpmnParseHandlers);
			if (PostBpmnParseHandlers != null)
			{
			  parseHandlers.AddRange(PostBpmnParseHandlers);
			}
    
			BpmnParseHandlers bpmnParseHandlers = new BpmnParseHandlers();
			bpmnParseHandlers.addHandlers(parseHandlers);
			bpmnParser.setBpmnParserHandlers(bpmnParseHandlers);
    
			bpmnDeployer.BpmnParser = bpmnParser;
    
			defaultDeployers.Add(bpmnDeployer);
			return defaultDeployers;
		  }
	  }

	  protected internal virtual IList<BpmnParseHandler> DefaultBpmnParseHandlers
	  {
		  get
		  {
    
			// Alpabetic list of default parse handler classes
			IList<BpmnParseHandler> bpmnParserHandlers = new List<BpmnParseHandler>();
			bpmnParserHandlers.Add(new BoundaryEventParseHandler());
			bpmnParserHandlers.Add(new BusinessRuleParseHandler());
			bpmnParserHandlers.Add(new CallActivityParseHandler());
			bpmnParserHandlers.Add(new CancelEventDefinitionParseHandler());
			bpmnParserHandlers.Add(new CompensateEventDefinitionParseHandler());
			bpmnParserHandlers.Add(new EndEventParseHandler());
			bpmnParserHandlers.Add(new ErrorEventDefinitionParseHandler());
			bpmnParserHandlers.Add(new EventBasedGatewayParseHandler());
			bpmnParserHandlers.Add(new ExclusiveGatewayParseHandler());
			bpmnParserHandlers.Add(new InclusiveGatewayParseHandler());
			bpmnParserHandlers.Add(new IntermediateCatchEventParseHandler());
			bpmnParserHandlers.Add(new IntermediateThrowEventParseHandler());
			bpmnParserHandlers.Add(new ManualTaskParseHandler());
			bpmnParserHandlers.Add(new MessageEventDefinitionParseHandler());
			bpmnParserHandlers.Add(new ParallelGatewayParseHandler());
			bpmnParserHandlers.Add(new ProcessParseHandler());
			bpmnParserHandlers.Add(new ReceiveTaskParseHandler());
			bpmnParserHandlers.Add(new ScriptTaskParseHandler());
			bpmnParserHandlers.Add(new SendTaskParseHandler());
			bpmnParserHandlers.Add(new SequenceFlowParseHandler());
			bpmnParserHandlers.Add(new ServiceTaskParseHandler());
			bpmnParserHandlers.Add(new SignalEventDefinitionParseHandler());
			bpmnParserHandlers.Add(new StartEventParseHandler());
			bpmnParserHandlers.Add(new SubProcessParseHandler());
			bpmnParserHandlers.Add(new EventSubProcessParseHandler());
			bpmnParserHandlers.Add(new TaskParseHandler());
			bpmnParserHandlers.Add(new TimerEventDefinitionParseHandler());
			bpmnParserHandlers.Add(new TransactionParseHandler());
			bpmnParserHandlers.Add(new UserTaskParseHandler());
    
			// Replace any default handler if the user wants to replace them
			if (customDefaultBpmnParseHandlers != null)
			{
    
			  IDictionary<Type, BpmnParseHandler> customParseHandlerMap = new Dictionary<Type, BpmnParseHandler>();
			  foreach (BpmnParseHandler bpmnParseHandler in customDefaultBpmnParseHandlers)
			  {
				foreach (Type handledType in bpmnParseHandler.HandledTypes)
				{
				  customParseHandlerMap[handledType] = bpmnParseHandler;
				}
			  }
    
			  for (int i = 0; i < bpmnParserHandlers.Count; i++)
			  {
				// All the default handlers support only one type
				BpmnParseHandler defaultBpmnParseHandler = bpmnParserHandlers[i];
				if (defaultBpmnParseHandler.HandledTypes.Count != 1)
				{
				  StringBuilder supportedTypes = new StringBuilder();
				  foreach (Type type in defaultBpmnParseHandler.HandledTypes)
				  {
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
					supportedTypes.Append(" ").Append(type.FullName).Append(" ");
				  }
				  throw new ActivitiException("The default BPMN parse handlers should only support one type, but " + defaultBpmnParseHandler.GetType() + " supports " + supportedTypes.ToString() + ". This is likely a programmatic error");
				}
				else
				{
				  Type handledType = defaultBpmnParseHandler.HandledTypes.GetEnumerator().next();
				  if (customParseHandlerMap.ContainsKey(handledType))
				  {
					BpmnParseHandler newBpmnParseHandler = customParseHandlerMap[handledType];
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.info("Replacing default BpmnParseHandler " + defaultBpmnParseHandler.GetType().FullName + " with " + newBpmnParseHandler.GetType().FullName);
					bpmnParserHandlers[i] = newBpmnParseHandler;
				  }
				}
			  }
			}
    
			// History
			foreach (BpmnParseHandler handler in DefaultHistoryParseHandlers)
			{
			  bpmnParserHandlers.Add(handler);
			}
    
			return bpmnParserHandlers;
		  }
	  }

	  protected internal virtual IList<BpmnParseHandler> DefaultHistoryParseHandlers
	  {
		  get
		  {
			IList<BpmnParseHandler> parseHandlers = new List<BpmnParseHandler>();
			parseHandlers.Add(new FlowNodeHistoryParseHandler());
			parseHandlers.Add(new ProcessHistoryParseHandler());
			parseHandlers.Add(new StartEventHistoryParseHandler());
			parseHandlers.Add(new UserTaskHistoryParseHandler());
			return parseHandlers;
		  }
	  }

	  private void initClock()
	  {
		if (clock == null)
		{
		  clock = new DefaultClockImpl();
		}
	  }

	  protected internal virtual void initProcessDiagramGenerator()
	  {
		if (processDiagramGenerator == null)
		{
		  processDiagramGenerator = new DefaultProcessDiagramGenerator();
		}
	  }

	  protected internal virtual void initJobHandlers()
	  {
		jobHandlers = new Dictionary<string, JobHandler>();
		TimerExecuteNestedActivityJobHandler timerExecuteNestedActivityJobHandler = new TimerExecuteNestedActivityJobHandler();
		jobHandlers[timerExecuteNestedActivityJobHandler.Type] = timerExecuteNestedActivityJobHandler;

		TimerCatchIntermediateEventJobHandler timerCatchIntermediateEvent = new TimerCatchIntermediateEventJobHandler();
		jobHandlers[timerCatchIntermediateEvent.Type] = timerCatchIntermediateEvent;

		TimerStartEventJobHandler timerStartEvent = new TimerStartEventJobHandler();
		jobHandlers[timerStartEvent.Type] = timerStartEvent;

		AsyncContinuationJobHandler asyncContinuationJobHandler = new AsyncContinuationJobHandler();
		jobHandlers[asyncContinuationJobHandler.Type] = asyncContinuationJobHandler;

		ProcessEventJobHandler processEventJobHandler = new ProcessEventJobHandler();
		jobHandlers[processEventJobHandler.Type] = processEventJobHandler;

		TimerSuspendProcessDefinitionHandler suspendProcessDefinitionHandler = new TimerSuspendProcessDefinitionHandler();
		jobHandlers[suspendProcessDefinitionHandler.Type] = suspendProcessDefinitionHandler;

		TimerActivateProcessDefinitionHandler activateProcessDefinitionHandler = new TimerActivateProcessDefinitionHandler();
		jobHandlers[activateProcessDefinitionHandler.Type] = activateProcessDefinitionHandler;

		// if we have custom job handlers, register them
		if (CustomJobHandlers != null)
		{
		  foreach (JobHandler customJobHandler in CustomJobHandlers)
		  {
			jobHandlers[customJobHandler.Type] = customJobHandler;
		  }
		}
	  }

	  // job executor /////////////////////////////////////////////////////////////

	  protected internal virtual void initJobExecutor()
	  {
		if (AsyncExecutorEnabled == false)
		{
		  if (jobExecutor == null)
		  {
			jobExecutor = new DefaultJobExecutor();
		  }

		  jobExecutor.ClockReader = this.clock;

		  jobExecutor.CommandExecutor = commandExecutor;
		  jobExecutor.AutoActivate = jobExecutorActivate;

		  if (jobExecutor.getRejectedJobsHandler() == null)
		  {
			if (customRejectedJobsHandler != null)
			{
			  jobExecutor.setRejectedJobsHandler(customRejectedJobsHandler);
			}
			else
			{
			  jobExecutor.setRejectedJobsHandler(new CallerRunsRejectedJobsHandler());
			}
		  }
		}
	  }

	  // async executor /////////////////////////////////////////////////////////////

	  protected internal virtual void initAsyncExecutor()
	  {
		if (AsyncExecutorEnabled)
		{
		  if (asyncExecutor == null)
		  {
			DefaultAsyncJobExecutor defaultAsyncExecutor = new DefaultAsyncJobExecutor();

			// Thread pool config
			defaultAsyncExecutor.CorePoolSize = asyncExecutorCorePoolSize;
			defaultAsyncExecutor.MaxPoolSize = asyncExecutorMaxPoolSize;
			defaultAsyncExecutor.KeepAliveTime = asyncExecutorThreadKeepAliveTime;

			// Threadpool queue
			if (asyncExecutorThreadPoolQueue != null)
			{
				defaultAsyncExecutor.ThreadPoolQueue = asyncExecutorThreadPoolQueue;
			}
			defaultAsyncExecutor.QueueSize = asyncExecutorThreadPoolQueueSize;

			// Acquisition wait time
			defaultAsyncExecutor.DefaultTimerJobAcquireWaitTimeInMillis = asyncExecutorDefaultTimerJobAcquireWaitTime;
			defaultAsyncExecutor.DefaultAsyncJobAcquireWaitTimeInMillis = asyncExecutorDefaultAsyncJobAcquireWaitTime;

			// Queue full wait time
			defaultAsyncExecutor.DefaultQueueSizeFullWaitTimeInMillis = asyncExecutorDefaultQueueSizeFullWaitTime;

			// Job locking
			defaultAsyncExecutor.TimerLockTimeInMillis = asyncExecutorTimerLockTimeInMillis;
			defaultAsyncExecutor.AsyncJobLockTimeInMillis = asyncExecutorAsyncJobLockTimeInMillis;
			if (asyncExecutorLockOwner != null)
			{
				defaultAsyncExecutor.LockOwner = asyncExecutorLockOwner;
			}

			// Retry
			defaultAsyncExecutor.RetryWaitTimeInMillis = asyncExecutorLockRetryWaitTimeInMillis;

			// Shutdown
			defaultAsyncExecutor.SecondsToWaitOnShutdown = asyncExecutorSecondsToWaitOnShutdown;

			asyncExecutor = defaultAsyncExecutor;
		  }

		  asyncExecutor.CommandExecutor = commandExecutor;
		  asyncExecutor.AutoActivate = asyncExecutorActivate;
		}
	  }

	  // history //////////////////////////////////////////////////////////////////

	  public virtual void initHistoryLevel()
	  {
		  if (historyLevel == null)
		  {
			  historyLevel = HistoryLevel.getHistoryLevelForKey(History);
		  }
	  }

	  // id generator /////////////////////////////////////////////////////////////

	  protected internal virtual void initIdGenerator()
	  {
		if (idGenerator == null)
		{
		  CommandExecutor idGeneratorCommandExecutor = null;
		  if (idGeneratorDataSource != null)
		  {
			ProcessEngineConfigurationImpl processEngineConfiguration = new StandaloneProcessEngineConfiguration();
			processEngineConfiguration.DataSource = idGeneratorDataSource;
			processEngineConfiguration.DatabaseSchemaUpdate = DB_SCHEMA_UPDATE_FALSE;
			processEngineConfiguration.init();
			idGeneratorCommandExecutor = processEngineConfiguration.CommandExecutor;
		  }
		  else if (idGeneratorDataSourceJndiName != null)
		  {
			ProcessEngineConfigurationImpl processEngineConfiguration = new StandaloneProcessEngineConfiguration();
			processEngineConfiguration.DataSourceJndiName = idGeneratorDataSourceJndiName;
			processEngineConfiguration.DatabaseSchemaUpdate = DB_SCHEMA_UPDATE_FALSE;
			processEngineConfiguration.init();
			idGeneratorCommandExecutor = processEngineConfiguration.CommandExecutor;
		  }
		  else
		  {
			idGeneratorCommandExecutor = CommandExecutor;
		  }

		  DbIdGenerator dbIdGenerator = new DbIdGenerator();
		  dbIdGenerator.IdBlockSize = idBlockSize;
		  dbIdGenerator.CommandExecutor = idGeneratorCommandExecutor;
		  dbIdGenerator.CommandConfig = DefaultCommandConfig.transactionRequiresNew();
		  idGenerator = dbIdGenerator;
		}
	  }

	  // OTHER ////////////////////////////////////////////////////////////////////

	  protected internal virtual void initCommandContextFactory()
	  {
		if (commandContextFactory == null)
		{
		  commandContextFactory = new CommandContextFactory();
		}
		commandContextFactory.ProcessEngineConfiguration = this;
	  }

	  protected internal virtual void initTransactionContextFactory()
	  {
		if (transactionContextFactory == null)
		{
		  transactionContextFactory = new StandaloneMybatisTransactionContextFactory();
		}
	  }

	  protected internal virtual void initVariableTypes()
	  {
		if (variableTypes == null)
		{
		  variableTypes = new DefaultVariableTypes();
		  if (customPreVariableTypes != null)
		  {
			foreach (VariableType customVariableType in customPreVariableTypes)
			{
			  variableTypes.addType(customVariableType);
			}
		  }
		  variableTypes.addType(new NullType());
		  variableTypes.addType(new StringType(MaxLengthString));
		  variableTypes.addType(new LongStringType(MaxLengthString + 1));
		  variableTypes.addType(new BooleanType());
		  variableTypes.addType(new ShortType());
		  variableTypes.addType(new IntegerType());
		  variableTypes.addType(new LongType());
		  variableTypes.addType(new DateType());
		  variableTypes.addType(new DoubleType());
		  variableTypes.addType(new UUIDType());
		  variableTypes.addType(new JsonType(MaxLengthString, objectMapper));
		  variableTypes.addType(new LongJsonType(MaxLengthString + 1, objectMapper));
		  variableTypes.addType(new ByteArrayType());
		  variableTypes.addType(new SerializableType());
		  variableTypes.addType(new CustomObjectType("item", typeof(ItemInstance)));
		  variableTypes.addType(new CustomObjectType("message", typeof(MessageInstance)));
		  if (customPostVariableTypes != null)
		  {
			foreach (VariableType customVariableType in customPostVariableTypes)
			{
			  variableTypes.addType(customVariableType);
			}
		  }
		}
	  }

	  protected internal virtual int MaxLengthString
	  {
		  get
		  {
			if (maxLengthStringVariableType == -1)
			{
			  if ("oracle".Equals(databaseType, StringComparison.CurrentCultureIgnoreCase) == true)
			  {
				return DEFAULT_ORACLE_MAX_LENGTH_STRING;
			  }
			  else
			  {
				return DEFAULT_GENERIC_MAX_LENGTH_STRING;
			  }
			}
			else
			{
			  return maxLengthStringVariableType;
			}
		  }
	  }

	  protected internal virtual void initFormEngines()
	  {
		if (formEngines == null)
		{
		  formEngines = new Dictionary<string, FormEngine>();
		  FormEngine defaultFormEngine = new JuelFormEngine();
		  formEngines[null] = defaultFormEngine; // default form engine is looked up with null
		  formEngines[defaultFormEngine.Name] = defaultFormEngine;
		}
		if (customFormEngines != null)
		{
		  foreach (FormEngine formEngine in customFormEngines)
		  {
			formEngines[formEngine.Name] = formEngine;
		  }
		}
	  }

	  protected internal virtual void initFormTypes()
	  {
		if (formTypes == null)
		{
		  formTypes = new FormTypes();
		  formTypes.addFormType(new StringFormType());
		  formTypes.addFormType(new LongFormType());
		  formTypes.addFormType(new DateFormType("dd/MM/yyyy"));
		  formTypes.addFormType(new BooleanFormType());
		  formTypes.addFormType(new DoubleFormType());
		}
		if (customFormTypes != null)
		{
		  foreach (AbstractFormType customFormType in customFormTypes)
		  {
			formTypes.addFormType(customFormType);
		  }
		}
	  }

	  protected internal virtual void initScriptingEngines()
	  {
		if (resolverFactories == null)
		{
		  resolverFactories = new List<ResolverFactory>();
		  resolverFactories.Add(new VariableScopeResolverFactory());
		  resolverFactories.Add(new BeansResolverFactory());
		}
		if (scriptingEngines == null)
		{
		  scriptingEngines = new ScriptingEngines(new ScriptBindingsFactory(resolverFactories));
		}
	  }

	  protected internal virtual void initExpressionManager()
	  {
		if (expressionManager == null)
		{
		  expressionManager = new ExpressionManager(beans);
		}
	  }

	  protected internal virtual void initBusinessCalendarManager()
	  {
		if (businessCalendarManager == null)
		{
		  MapBusinessCalendarManager mapBusinessCalendarManager = new MapBusinessCalendarManager();
		  mapBusinessCalendarManager.addBusinessCalendar(DurationBusinessCalendar.NAME, new DurationBusinessCalendar(this.clock));
		  mapBusinessCalendarManager.addBusinessCalendar(DueDateBusinessCalendar.NAME, new DueDateBusinessCalendar(this.clock));
		  mapBusinessCalendarManager.addBusinessCalendar(CycleBusinessCalendar.NAME, new CycleBusinessCalendar(this.clock));

		  businessCalendarManager = mapBusinessCalendarManager;
		}
	  }

	  protected internal virtual void initDelegateInterceptor()
	  {
		if (delegateInterceptor == null)
		{
		  delegateInterceptor = new DefaultDelegateInterceptor();
		}
	  }

	  protected internal virtual void initEventHandlers()
	  {
		if (eventHandlers == null)
		{
		  eventHandlers = new Dictionary<string, EventHandler>();

		  SignalEventHandler signalEventHander = new SignalEventHandler();
		  eventHandlers[signalEventHander.EventHandlerType] = signalEventHander;

		  CompensationEventHandler compensationEventHandler = new CompensationEventHandler();
		  eventHandlers[compensationEventHandler.EventHandlerType] = compensationEventHandler;

		  MessageEventHandler messageEventHandler = new MessageEventHandler();
		  eventHandlers[messageEventHandler.EventHandlerType] = messageEventHandler;

		}
		if (customEventHandlers != null)
		{
		  foreach (EventHandler eventHandler in customEventHandlers)
		  {
			eventHandlers[eventHandler.EventHandlerType] = eventHandler;
		  }
		}
	  }

	  // JPA //////////////////////////////////////////////////////////////////////

	  protected internal virtual void initJpa()
	  {
		if (jpaPersistenceUnitName != null)
		{
		  jpaEntityManagerFactory = JpaHelper.createEntityManagerFactory(jpaPersistenceUnitName);
		}
		if (jpaEntityManagerFactory != null)
		{
		  sessionFactories[typeof(EntityManagerSession)] = new EntityManagerSessionFactory(jpaEntityManagerFactory, jpaHandleTransaction, jpaCloseEntityManager);
		  VariableType jpaType = variableTypes.getVariableType(JPAEntityVariableType.TYPE_NAME);
		  // Add JPA-type
		  if (jpaType == null)
		  {
			// We try adding the variable right before SerializableType, if available
			int serializableIndex = variableTypes.getTypeIndex(SerializableType.TYPE_NAME);
			if (serializableIndex > -1)
			{
			  variableTypes.addType(new JPAEntityVariableType(), serializableIndex);
			}
			else
			{
			  variableTypes.addType(new JPAEntityVariableType());
			}
		  }

		  jpaType = variableTypes.getVariableType(JPAEntityListVariableType.TYPE_NAME);

		  // Add JPA-list type after regular JPA type if not already present
		  if (jpaType == null)
		  {
			variableTypes.addType(new JPAEntityListVariableType(), variableTypes.getTypeIndex(JPAEntityVariableType.TYPE_NAME));
		  }
		}
	  }

	  protected internal virtual void initBeans()
	  {
		if (beans == null)
		{
		  beans = new Dictionary<object, object>();
		}
	  }

	  protected internal virtual void initEventDispatcher()
	  {
		  if (this.eventDispatcher == null)
		  {
			  this.eventDispatcher = new ActivitiEventDispatcherImpl();
		  }

		  this.eventDispatcher.Enabled = enableEventDispatcher;

		  if (eventListeners != null)
		  {
			  foreach (ActivitiEventListener listenerToAdd in eventListeners)
			  {
				  this.eventDispatcher.addEventListener(listenerToAdd);
			  }
		  }

		  if (typedEventListeners != null)
		  {
			  foreach (KeyValuePair<string, IList<ActivitiEventListener>> listenersToAdd in typedEventListeners)
			  {
				  // Extract types from the given string
				  ActivitiEventType[] types = ActivitiEventType.getTypesFromString(listenersToAdd.Key);

				  foreach (ActivitiEventListener listenerToAdd in listenersToAdd.Value)
				  {
					  this.eventDispatcher.addEventListener(listenerToAdd, types);
				  }
			  }
		  }

	  }

	  protected internal virtual void initProcessValidator()
	  {
		  if (this.processValidator == null)
		  {
			  this.processValidator = (new ProcessValidatorFactory()).createDefaultProcessValidator();
		  }
	  }

	  protected internal virtual void initDatabaseEventLogging()
	  {
		  if (enableDatabaseEventLogging)
		  {
			  // Database event logging uses the default logging mechanism and adds
			  // a specific event listener to the list of event listeners
			  EventDispatcher.addEventListener(new EventLogger(clock, objectMapper));
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual CommandConfig DefaultCommandConfig
	  {
		  get
		  {
			return defaultCommandConfig;
		  }
		  set
		  {
			this.defaultCommandConfig = value;
		  }
	  }


	  public virtual CommandConfig SchemaCommandConfig
	  {
		  get
		  {
			return schemaCommandConfig;
		  }
		  set
		  {
			this.schemaCommandConfig = value;
		  }
	  }


	  public virtual CommandInterceptor CommandInvoker
	  {
		  get
		  {
			return commandInvoker;
		  }
		  set
		  {
			this.commandInvoker = value;
		  }
	  }


	  public virtual IList<CommandInterceptor> CustomPreCommandInterceptors
	  {
		  get
		  {
			return customPreCommandInterceptors;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomPreCommandInterceptors(IList<CommandInterceptor> customPreCommandInterceptors)
	  {
		this.customPreCommandInterceptors = customPreCommandInterceptors;
		return this;
	  }

	  public virtual IList<CommandInterceptor> CustomPostCommandInterceptors
	  {
		  get
		  {
			return customPostCommandInterceptors;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomPostCommandInterceptors(IList<CommandInterceptor> customPostCommandInterceptors)
	  {
		this.customPostCommandInterceptors = customPostCommandInterceptors;
		return this;
	  }

	  public virtual IList<CommandInterceptor> CommandInterceptors
	  {
		  get
		  {
			return commandInterceptors;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCommandInterceptors(IList<CommandInterceptor> commandInterceptors)
	  {
		this.commandInterceptors = commandInterceptors;
		return this;
	  }

	  public virtual CommandExecutor CommandExecutor
	  {
		  get
		  {
			return commandExecutor;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCommandExecutor(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
		return this;
	  }

	  public override RepositoryService RepositoryService
	  {
		  get
		  {
			return repositoryService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setRepositoryService(RepositoryService repositoryService)
	  {
		this.repositoryService = repositoryService;
		return this;
	  }

	  public override RuntimeService RuntimeService
	  {
		  get
		  {
			return runtimeService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setRuntimeService(RuntimeService runtimeService)
	  {
		this.runtimeService = runtimeService;
		return this;
	  }

	  public override HistoryService HistoryService
	  {
		  get
		  {
			return historyService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setHistoryService(HistoryService historyService)
	  {
		this.historyService = historyService;
		return this;
	  }

	  public override IdentityService IdentityService
	  {
		  get
		  {
			return identityService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setIdentityService(IdentityService identityService)
	  {
		this.identityService = identityService;
		return this;
	  }

	  public override TaskService TaskService
	  {
		  get
		  {
			return taskService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setTaskService(TaskService taskService)
	  {
		this.taskService = taskService;
		return this;
	  }

	  public override FormService FormService
	  {
		  get
		  {
			return formService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setFormService(FormService formService)
	  {
		this.formService = formService;
		return this;
	  }

	  public override ManagementService ManagementService
	  {
		  get
		  {
			return managementService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setManagementService(ManagementService managementService)
	  {
		this.managementService = managementService;
		return this;
	  }

	  public override DynamicBpmnService DynamicBpmnService
	  {
		  get
		  {
			return dynamicBpmnService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setDynamicBpmnService(DynamicBpmnService dynamicBpmnService)
	  {
		this.dynamicBpmnService = dynamicBpmnService;
		return this;
	  }

	  public override ProcessEngineConfiguration ProcessEngineConfiguration
	  {
		  get
		  {
			return this;
		  }
	  }

	  public virtual IDictionary<Type, SessionFactory> SessionFactories
	  {
		  get
		  {
			return sessionFactories;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setSessionFactories(IDictionary<Type, SessionFactory> sessionFactories)
	  {
		this.sessionFactories = sessionFactories;
		return this;
	  }

	  public virtual IList<ProcessEngineConfigurator> Configurators
	  {
		  get
		  {
			return configurators;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl addConfigurator(ProcessEngineConfigurator configurator)
	  {
		if (this.configurators == null)
		{
		  this.configurators = new List<ProcessEngineConfigurator>();
		}
		this.configurators.Add(configurator);
		return this;
	  }

	  public virtual ProcessEngineConfigurationImpl setConfigurators(IList<ProcessEngineConfigurator> configurators)
	  {
		this.configurators = configurators;
		return this;
	  }

	  public virtual bool EnableConfiguratorServiceLoader
	  {
		  set
		  {
			  this.enableConfiguratorServiceLoader = value;
		  }
	  }

	  public virtual IList<ProcessEngineConfigurator> AllConfigurators
	  {
		  get
		  {
				return allConfigurators;
		  }
	  }

		public virtual BpmnDeployer BpmnDeployer
		{
			get
			{
			return bpmnDeployer;
			}
		}

	  public virtual ProcessEngineConfigurationImpl setBpmnDeployer(BpmnDeployer bpmnDeployer)
	  {
		this.bpmnDeployer = bpmnDeployer;
		return this;
	  }

	  public virtual BpmnParser BpmnParser
	  {
		  get
		  {
			return bpmnParser;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setBpmnParser(BpmnParser bpmnParser)
	  {
		this.bpmnParser = bpmnParser;
		return this;
	  }

	  public virtual IList<Deployer> Deployers
	  {
		  get
		  {
			return deployers;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setDeployers(IList<Deployer> deployers)
	  {
		this.deployers = deployers;
		return this;
	  }

	  public virtual IdGenerator getIdGenerator()
	  {
		return idGenerator;
	  }

	  public virtual ProcessEngineConfigurationImpl setIdGenerator(IdGenerator idGenerator)
	  {
		this.idGenerator = idGenerator;
		return this;
	  }

	  public virtual string WsSyncFactoryClassName
	  {
		  get
		  {
			return wsSyncFactoryClassName;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setWsSyncFactoryClassName(string wsSyncFactoryClassName)
	  {
		this.wsSyncFactoryClassName = wsSyncFactoryClassName;
		return this;
	  }

	  /// <summary>
	  /// Add or replace the address of the given web-service endpoint with the given value </summary>
	  /// <param name="endpointName"> The endpoint name for which a new address must be set </param>
	  /// <param name="address"> The new address of the endpoint </param>
	  public virtual ProcessEngineConfiguration addWsEndpointAddress(QName endpointName, URL address)
	  {
		  this.wsOverridenEndpointAddresses.put(endpointName, address);
		  return this;
	  }

	  /// <summary>
	  /// Remove the address definition of the given web-service endpoint </summary>
	  /// <param name="endpointName"> The endpoint name for which the address definition must be removed </param>
	  public virtual ProcessEngineConfiguration removeWsEndpointAddress(QName endpointName)
	  {
		  this.wsOverridenEndpointAddresses.remove(endpointName);
		  return this;
	  }

	  public virtual ConcurrentMap<QName, URL> WsOverridenEndpointAddresses
	  {
		  get
		  {
			  return this.wsOverridenEndpointAddresses;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.activiti.engine.ProcessEngineConfiguration setWsOverridenEndpointAddresses(final java.util.concurrent.ConcurrentMap<javax.xml.namespace.QName, java.net.URL> wsOverridenEndpointAdress)
	  public virtual ProcessEngineConfiguration setWsOverridenEndpointAddresses(ConcurrentMap<QName, URL> wsOverridenEndpointAdress)
	  {
		this.wsOverridenEndpointAddresses.putAll(wsOverridenEndpointAdress);
		return this;
	  }

	  public virtual IDictionary<string, FormEngine> FormEngines
	  {
		  get
		  {
			return formEngines;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setFormEngines(IDictionary<string, FormEngine> formEngines)
	  {
		this.formEngines = formEngines;
		return this;
	  }

	  public virtual FormTypes FormTypes
	  {
		  get
		  {
			return formTypes;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setFormTypes(FormTypes formTypes)
	  {
		this.formTypes = formTypes;
		return this;
	  }

	  public virtual ScriptingEngines ScriptingEngines
	  {
		  get
		  {
			return scriptingEngines;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setScriptingEngines(ScriptingEngines scriptingEngines)
	  {
		this.scriptingEngines = scriptingEngines;
		return this;
	  }

	  public virtual VariableTypes VariableTypes
	  {
		  get
		  {
			return variableTypes;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setVariableTypes(VariableTypes variableTypes)
	  {
		this.variableTypes = variableTypes;
		return this;
	  }

	  public virtual ExpressionManager ExpressionManager
	  {
		  get
		  {
			return expressionManager;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setExpressionManager(ExpressionManager expressionManager)
	  {
		this.expressionManager = expressionManager;
		return this;
	  }

	  public virtual BusinessCalendarManager BusinessCalendarManager
	  {
		  get
		  {
			return businessCalendarManager;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setBusinessCalendarManager(BusinessCalendarManager businessCalendarManager)
	  {
		this.businessCalendarManager = businessCalendarManager;
		return this;
	  }

	  public virtual int ExecutionQueryLimit
	  {
		  get
		  {
			return executionQueryLimit;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setExecutionQueryLimit(int executionQueryLimit)
	  {
		this.executionQueryLimit = executionQueryLimit;
		return this;
	  }

	  public virtual int TaskQueryLimit
	  {
		  get
		  {
			return taskQueryLimit;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setTaskQueryLimit(int taskQueryLimit)
	  {
		this.taskQueryLimit = taskQueryLimit;
		return this;
	  }

	  public virtual int HistoricTaskQueryLimit
	  {
		  get
		  {
			return historicTaskQueryLimit;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setHistoricTaskQueryLimit(int historicTaskQueryLimit)
	  {
		this.historicTaskQueryLimit = historicTaskQueryLimit;
		return this;
	  }

	  public virtual int HistoricProcessInstancesQueryLimit
	  {
		  get
		  {
			return historicProcessInstancesQueryLimit;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setHistoricProcessInstancesQueryLimit(int historicProcessInstancesQueryLimit)
	  {
		this.historicProcessInstancesQueryLimit = historicProcessInstancesQueryLimit;
		return this;
	  }

	  public virtual CommandContextFactory CommandContextFactory
	  {
		  get
		  {
			return commandContextFactory;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCommandContextFactory(CommandContextFactory commandContextFactory)
	  {
		this.commandContextFactory = commandContextFactory;
		return this;
	  }

	  public virtual TransactionContextFactory getTransactionContextFactory()
	  {
		return transactionContextFactory;
	  }

	  public virtual ProcessEngineConfigurationImpl setTransactionContextFactory(TransactionContextFactory transactionContextFactory)
	  {
		this.transactionContextFactory = transactionContextFactory;
		return this;
	  }

	  public virtual IList<Deployer> CustomPreDeployers
	  {
		  get
		  {
			return customPreDeployers;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomPreDeployers(IList<Deployer> customPreDeployers)
	  {
		this.customPreDeployers = customPreDeployers;
		return this;
	  }

	  public virtual IList<Deployer> CustomPostDeployers
	  {
		  get
		  {
			return customPostDeployers;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomPostDeployers(IList<Deployer> customPostDeployers)
	  {
		this.customPostDeployers = customPostDeployers;
		return this;
	  }

	  public virtual IDictionary<string, JobHandler> JobHandlers
	  {
		  get
		  {
			return jobHandlers;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setJobHandlers(IDictionary<string, JobHandler> jobHandlers)
	  {
		this.jobHandlers = jobHandlers;
		return this;
	  }

	  public virtual int AsyncExecutorCorePoolSize
	  {
		  get
		  {
				return asyncExecutorCorePoolSize;
		  }
	  }

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorCorePoolSize(int asyncExecutorCorePoolSize)
		{
			this.asyncExecutorCorePoolSize = asyncExecutorCorePoolSize;
			return this;
		}

		public virtual int AsyncExecutorMaxPoolSize
		{
			get
			{
				return asyncExecutorMaxPoolSize;
			}
		}

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorMaxPoolSize(int asyncExecutorMaxPoolSize)
		{
			this.asyncExecutorMaxPoolSize = asyncExecutorMaxPoolSize;
			return this;
		}

		public virtual long AsyncExecutorThreadKeepAliveTime
		{
			get
			{
				return asyncExecutorThreadKeepAliveTime;
			}
		}

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorThreadKeepAliveTime(long asyncExecutorThreadKeepAliveTime)
		{
			this.asyncExecutorThreadKeepAliveTime = asyncExecutorThreadKeepAliveTime;
			return this;
		}

		public virtual int AsyncExecutorThreadPoolQueueSize
		{
			get
			{
				return asyncExecutorThreadPoolQueueSize;
			}
		}

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorThreadPoolQueueSize(int asyncExecutorThreadPoolQueueSize)
		{
			this.asyncExecutorThreadPoolQueueSize = asyncExecutorThreadPoolQueueSize;
			return this;
		}

		public virtual BlockingQueue<Runnable> AsyncExecutorThreadPoolQueue
		{
			get
			{
				return asyncExecutorThreadPoolQueue;
			}
		}

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorThreadPoolQueue(BlockingQueue<Runnable> asyncExecutorThreadPoolQueue)
		{
			this.asyncExecutorThreadPoolQueue = asyncExecutorThreadPoolQueue;
			return this;
		}

		public virtual long AsyncExecutorSecondsToWaitOnShutdown
		{
			get
			{
				return asyncExecutorSecondsToWaitOnShutdown;
			}
		}

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorSecondsToWaitOnShutdown(long asyncExecutorSecondsToWaitOnShutdown)
		{
			this.asyncExecutorSecondsToWaitOnShutdown = asyncExecutorSecondsToWaitOnShutdown;
			return this;
		}

		public virtual int AsyncExecutorMaxTimerJobsPerAcquisition
		{
			get
			{
				return asyncExecutorMaxTimerJobsPerAcquisition;
			}
		}

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorMaxTimerJobsPerAcquisition(int asyncExecutorMaxTimerJobsPerAcquisition)
		{
			this.asyncExecutorMaxTimerJobsPerAcquisition = asyncExecutorMaxTimerJobsPerAcquisition;
			return this;
		}

		public virtual int AsyncExecutorMaxAsyncJobsDuePerAcquisition
		{
			get
			{
				return asyncExecutorMaxAsyncJobsDuePerAcquisition;
			}
		}

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorMaxAsyncJobsDuePerAcquisition(int asyncExecutorMaxAsyncJobsDuePerAcquisition)
		{
			this.asyncExecutorMaxAsyncJobsDuePerAcquisition = asyncExecutorMaxAsyncJobsDuePerAcquisition;
			return this;
		}

		public virtual int AsyncExecutorTimerJobAcquireWaitTime
		{
			get
			{
				return asyncExecutorDefaultTimerJobAcquireWaitTime;
			}
		}

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorDefaultTimerJobAcquireWaitTime(int asyncExecutorDefaultTimerJobAcquireWaitTime)
		{
			this.asyncExecutorDefaultTimerJobAcquireWaitTime = asyncExecutorDefaultTimerJobAcquireWaitTime;
			return this;
		}

		public virtual int AsyncExecutorDefaultAsyncJobAcquireWaitTime
		{
			get
			{
				return asyncExecutorDefaultAsyncJobAcquireWaitTime;
			}
		}

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorDefaultAsyncJobAcquireWaitTime(int asyncExecutorDefaultAsyncJobAcquireWaitTime)
		{
			this.asyncExecutorDefaultAsyncJobAcquireWaitTime = asyncExecutorDefaultAsyncJobAcquireWaitTime;
			return this;
		}

		public virtual int AsyncExecutorDefaultQueueSizeFullWaitTime
		{
			get
			{
			return asyncExecutorDefaultQueueSizeFullWaitTime;
			}
		}

	  public virtual ProcessEngineConfigurationImpl setAsyncExecutorDefaultQueueSizeFullWaitTime(int asyncExecutorDefaultQueueSizeFullWaitTime)
	  {
		this.asyncExecutorDefaultQueueSizeFullWaitTime = asyncExecutorDefaultQueueSizeFullWaitTime;
		return this;
	  }

	  public virtual string AsyncExecutorLockOwner
	  {
		  get
		  {
				return asyncExecutorLockOwner;
		  }
	  }

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorLockOwner(string asyncExecutorLockOwner)
		{
			this.asyncExecutorLockOwner = asyncExecutorLockOwner;
			return this;
		}

		public virtual int AsyncExecutorTimerLockTimeInMillis
		{
			get
			{
				return asyncExecutorTimerLockTimeInMillis;
			}
		}

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorTimerLockTimeInMillis(int asyncExecutorTimerLockTimeInMillis)
		{
			this.asyncExecutorTimerLockTimeInMillis = asyncExecutorTimerLockTimeInMillis;
			return this;
		}

		public virtual int AsyncExecutorAsyncJobLockTimeInMillis
		{
			get
			{
				return asyncExecutorAsyncJobLockTimeInMillis;
			}
		}

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorAsyncJobLockTimeInMillis(int asyncExecutorAsyncJobLockTimeInMillis)
		{
			this.asyncExecutorAsyncJobLockTimeInMillis = asyncExecutorAsyncJobLockTimeInMillis;
			return this;
		}

		public virtual int AsyncExecutorLockRetryWaitTimeInMillis
		{
			get
			{
				return asyncExecutorLockRetryWaitTimeInMillis;
			}
		}

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorLockRetryWaitTimeInMillis(int asyncExecutorLockRetryWaitTimeInMillis)
		{
			this.asyncExecutorLockRetryWaitTimeInMillis = asyncExecutorLockRetryWaitTimeInMillis;
			return this;
		}

		public virtual ExecuteAsyncRunnableFactory AsyncExecutorExecuteAsyncRunnableFactory
		{
			get
			{
				return asyncExecutorExecuteAsyncRunnableFactory;
			}
		}

		public virtual ProcessEngineConfigurationImpl setAsyncExecutorExecuteAsyncRunnableFactory(ExecuteAsyncRunnableFactory asyncExecutorExecuteAsyncRunnableFactory)
		{
			this.asyncExecutorExecuteAsyncRunnableFactory = asyncExecutorExecuteAsyncRunnableFactory;
			return this;
		}

		public virtual SqlSessionFactory SqlSessionFactory
		{
			get
			{
			return sqlSessionFactory;
			}
		}

	  public virtual ProcessEngineConfigurationImpl setSqlSessionFactory(SqlSessionFactory sqlSessionFactory)
	  {
		this.sqlSessionFactory = sqlSessionFactory;
		return this;
	  }

	  public virtual DbSqlSessionFactory DbSqlSessionFactory
	  {
		  get
		  {
			return dbSqlSessionFactory;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setDbSqlSessionFactory(DbSqlSessionFactory dbSqlSessionFactory)
	  {
		this.dbSqlSessionFactory = dbSqlSessionFactory;
		return this;
	  }

	  public virtual TransactionFactory TransactionFactory
	  {
		  get
		  {
			return transactionFactory;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setTransactionFactory(TransactionFactory transactionFactory)
	  {
		this.transactionFactory = transactionFactory;
		return this;
	  }

	  public virtual IList<SessionFactory> CustomSessionFactories
	  {
		  get
		  {
			return customSessionFactories;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomSessionFactories(IList<SessionFactory> customSessionFactories)
	  {
		this.customSessionFactories = customSessionFactories;
		return this;
	  }

	  public virtual IList<JobHandler> CustomJobHandlers
	  {
		  get
		  {
			return customJobHandlers;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomJobHandlers(IList<JobHandler> customJobHandlers)
	  {
		this.customJobHandlers = customJobHandlers;
		return this;
	  }

	  public virtual IList<FormEngine> CustomFormEngines
	  {
		  get
		  {
			return customFormEngines;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomFormEngines(IList<FormEngine> customFormEngines)
	  {
		this.customFormEngines = customFormEngines;
		return this;
	  }

	  public virtual IList<AbstractFormType> CustomFormTypes
	  {
		  get
		  {
			return customFormTypes;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomFormTypes(IList<AbstractFormType> customFormTypes)
	  {
		this.customFormTypes = customFormTypes;
		return this;
	  }

	  public virtual IList<string> CustomScriptingEngineClasses
	  {
		  get
		  {
			return customScriptingEngineClasses;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomScriptingEngineClasses(IList<string> customScriptingEngineClasses)
	  {
		this.customScriptingEngineClasses = customScriptingEngineClasses;
		return this;
	  }

	  public virtual IList<VariableType> CustomPreVariableTypes
	  {
		  get
		  {
			return customPreVariableTypes;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomPreVariableTypes(IList<VariableType> customPreVariableTypes)
	  {
		this.customPreVariableTypes = customPreVariableTypes;
		return this;
	  }

	  public virtual IList<VariableType> CustomPostVariableTypes
	  {
		  get
		  {
			return customPostVariableTypes;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomPostVariableTypes(IList<VariableType> customPostVariableTypes)
	  {
		this.customPostVariableTypes = customPostVariableTypes;
		return this;
	  }

	  public virtual IList<BpmnParseHandler> PreBpmnParseHandlers
	  {
		  get
		  {
			return preBpmnParseHandlers;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setPreBpmnParseHandlers(IList<BpmnParseHandler> preBpmnParseHandlers)
	  {
		this.preBpmnParseHandlers = preBpmnParseHandlers;
		return this;
	  }

	  public virtual IList<BpmnParseHandler> CustomDefaultBpmnParseHandlers
	  {
		  get
		  {
			return customDefaultBpmnParseHandlers;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomDefaultBpmnParseHandlers(IList<BpmnParseHandler> customDefaultBpmnParseHandlers)
	  {
		this.customDefaultBpmnParseHandlers = customDefaultBpmnParseHandlers;
		return this;
	  }

	  public virtual IList<BpmnParseHandler> PostBpmnParseHandlers
	  {
		  get
		  {
			return postBpmnParseHandlers;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setPostBpmnParseHandlers(IList<BpmnParseHandler> postBpmnParseHandlers)
	  {
		this.postBpmnParseHandlers = postBpmnParseHandlers;
		return this;
	  }

	  public virtual ActivityBehaviorFactory ActivityBehaviorFactory
	  {
		  get
		  {
			return activityBehaviorFactory;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setActivityBehaviorFactory(ActivityBehaviorFactory activityBehaviorFactory)
	  {
		this.activityBehaviorFactory = activityBehaviorFactory;
		return this;
	  }

	  public virtual ListenerFactory ListenerFactory
	  {
		  get
		  {
			return listenerFactory;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setListenerFactory(ListenerFactory listenerFactory)
	  {
		this.listenerFactory = listenerFactory;
		return this;
	  }

	  public virtual BpmnParseFactory getBpmnParseFactory()
	  {
		return bpmnParseFactory;
	  }

	  public virtual ProcessEngineConfigurationImpl setBpmnParseFactory(BpmnParseFactory bpmnParseFactory)
	  {
		this.bpmnParseFactory = bpmnParseFactory;
		return this;
	  }

	  public virtual IDictionary<object, object> Beans
	  {
		  get
		  {
			return beans;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setBeans(IDictionary<object, object> beans)
	  {
		this.beans = beans;
		return this;
	  }

	  public virtual IList<ResolverFactory> ResolverFactories
	  {
		  get
		  {
			return resolverFactories;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setResolverFactories(IList<ResolverFactory> resolverFactories)
	  {
		this.resolverFactories = resolverFactories;
		return this;
	  }

	  public virtual DeploymentManager DeploymentManager
	  {
		  get
		  {
			return deploymentManager;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setDeploymentManager(DeploymentManager deploymentManager)
	  {
		this.deploymentManager = deploymentManager;
		return this;
	  }

	  public virtual ProcessEngineConfigurationImpl setDelegateInterceptor(DelegateInterceptor delegateInterceptor)
	  {
		this.delegateInterceptor = delegateInterceptor;
		return this;
	  }

	  public virtual DelegateInterceptor DelegateInterceptor
	  {
		  get
		  {
			return delegateInterceptor;
		  }
	  }

	  public virtual RejectedJobsHandler CustomRejectedJobsHandler
	  {
		  get
		  {
			return customRejectedJobsHandler;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomRejectedJobsHandler(RejectedJobsHandler customRejectedJobsHandler)
	  {
		this.customRejectedJobsHandler = customRejectedJobsHandler;
		return this;
	  }

	  public virtual EventHandler getEventHandler(string eventType)
	  {
		return eventHandlers[eventType];
	  }

	  public virtual ProcessEngineConfigurationImpl setEventHandlers(IDictionary<string, EventHandler> eventHandlers)
	  {
		this.eventHandlers = eventHandlers;
		return this;
	  }

	  public virtual IDictionary<string, EventHandler> EventHandlers
	  {
		  get
		  {
			return eventHandlers;
		  }
	  }

	  public virtual IList<EventHandler> CustomEventHandlers
	  {
		  get
		  {
			return customEventHandlers;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomEventHandlers(IList<EventHandler> customEventHandlers)
	  {
		this.customEventHandlers = customEventHandlers;
		return this;
	  }

	  public virtual FailedJobCommandFactory FailedJobCommandFactory
	  {
		  get
		  {
			return failedJobCommandFactory;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setFailedJobCommandFactory(FailedJobCommandFactory failedJobCommandFactory)
	  {
		this.failedJobCommandFactory = failedJobCommandFactory;
		return this;
	  }

	  public virtual DataSource IdGeneratorDataSource
	  {
		  get
		  {
			return idGeneratorDataSource;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setIdGeneratorDataSource(DataSource idGeneratorDataSource)
	  {
		this.idGeneratorDataSource = idGeneratorDataSource;
		return this;
	  }

	  public virtual string IdGeneratorDataSourceJndiName
	  {
		  get
		  {
			return idGeneratorDataSourceJndiName;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setIdGeneratorDataSourceJndiName(string idGeneratorDataSourceJndiName)
	  {
		this.idGeneratorDataSourceJndiName = idGeneratorDataSourceJndiName;
		return this;
	  }

	  public virtual int BatchSizeProcessInstances
	  {
		  get
		  {
			return batchSizeProcessInstances;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setBatchSizeProcessInstances(int batchSizeProcessInstances)
	  {
		this.batchSizeProcessInstances = batchSizeProcessInstances;
		return this;
	  }

	  public virtual int BatchSizeTasks
	  {
		  get
		  {
			return batchSizeTasks;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setBatchSizeTasks(int batchSizeTasks)
	  {
		this.batchSizeTasks = batchSizeTasks;
		return this;
	  }

	  public virtual int ProcessDefinitionCacheLimit
	  {
		  get
		  {
			return processDefinitionCacheLimit;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setProcessDefinitionCacheLimit(int processDefinitionCacheLimit)
	  {
		this.processDefinitionCacheLimit = processDefinitionCacheLimit;
		return this;
	  }

	  public virtual DeploymentCache<ProcessDefinitionEntity> ProcessDefinitionCache
	  {
		  get
		  {
			return processDefinitionCache;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setProcessDefinitionCache(DeploymentCache<ProcessDefinitionEntity> processDefinitionCache)
	  {
		this.processDefinitionCache = processDefinitionCache;
		return this;
	  }

	  public virtual int KnowledgeBaseCacheLimit
	  {
		  get
		  {
			return knowledgeBaseCacheLimit;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setKnowledgeBaseCacheLimit(int knowledgeBaseCacheLimit)
	  {
		this.knowledgeBaseCacheLimit = knowledgeBaseCacheLimit;
		return this;
	  }

	  public virtual DeploymentCache<object> KnowledgeBaseCache
	  {
		  get
		  {
			return knowledgeBaseCache;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setKnowledgeBaseCache(DeploymentCache<object> knowledgeBaseCache)
	  {
		this.knowledgeBaseCache = knowledgeBaseCache;
		return this;
	  }

	  public virtual bool EnableSafeBpmnXml
	  {
		  get
		  {
			return enableSafeBpmnXml;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setEnableSafeBpmnXml(bool enableSafeBpmnXml)
	  {
		this.enableSafeBpmnXml = enableSafeBpmnXml;
		return this;
	  }

	  public virtual ActivitiEventDispatcher EventDispatcher
	  {
		  get
		  {
			  return eventDispatcher;
		  }
		  set
		  {
			  this.eventDispatcher = value;
		  }
	  }


	  public virtual bool EnableEventDispatcher
	  {
		  set
		  {
			  this.enableEventDispatcher = value;
		  }
		  get
		  {
				return enableEventDispatcher;
		  }
	  }

	  public virtual IDictionary<string, IList<ActivitiEventListener>> TypedEventListeners
	  {
		  set
		  {
			  this.typedEventListeners = value;
		  }
	  }

	  public virtual IList<ActivitiEventListener> EventListeners
	  {
		  set
		  {
			  this.eventListeners = value;
		  }
	  }

		public virtual ProcessValidator ProcessValidator
		{
			get
			{
				return processValidator;
			}
			set
			{
				this.processValidator = value;
			}
		}



		public virtual bool EnableDatabaseEventLogging
		{
			get
			{
				return enableDatabaseEventLogging;
			}
		}

		public virtual ProcessEngineConfigurationImpl setEnableDatabaseEventLogging(bool enableDatabaseEventLogging)
		{
			this.enableDatabaseEventLogging = enableDatabaseEventLogging;
		return this;
		}

	  public virtual int MaxLengthStringVariableType
	  {
		  get
		  {
			return maxLengthStringVariableType;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setMaxLengthStringVariableType(int maxLengthStringVariableType)
	  {
		this.maxLengthStringVariableType = maxLengthStringVariableType;
		return this;
	  }

		public virtual ProcessEngineConfigurationImpl setBulkInsertEnabled(bool isBulkInsertEnabled)
		{
			this.isBulkInsertEnabled = isBulkInsertEnabled;
			return this;
		}

		public virtual bool BulkInsertEnabled
		{
			get
			{
				return isBulkInsertEnabled;
			}
		}

		public virtual int MaxNrOfStatementsInBulkInsert
		{
			get
			{
				return maxNrOfStatementsInBulkInsert;
			}
		}

		public virtual ProcessEngineConfigurationImpl setMaxNrOfStatementsInBulkInsert(int maxNrOfStatementsInBulkInsert)
		{
			this.maxNrOfStatementsInBulkInsert = maxNrOfStatementsInBulkInsert;
			return this;
		}

	  public virtual DelegateExpressionFieldInjectionMode getDelegateExpressionFieldInjectionMode()
	  {
		return delegateExpressionFieldInjectionMode;
	  }

	  public virtual void setDelegateExpressionFieldInjectionMode(DelegateExpressionFieldInjectionMode delegateExpressionFieldInjectionMode)
	  {
		this.delegateExpressionFieldInjectionMode = delegateExpressionFieldInjectionMode;
	  }

	  public virtual ObjectMapper ObjectMapper
	  {
		  get
		  {
			return objectMapper;
		  }
	  }
	}

}