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

	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using AsyncExecutor = org.activiti.engine.impl.asyncexecutor.AsyncExecutor;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using TransactionContextFactory = org.activiti.engine.impl.cfg.TransactionContextFactory;
	using ExpressionManager = org.activiti.engine.impl.el.ExpressionManager;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using SessionFactory = org.activiti.engine.impl.interceptor.SessionFactory;
	using JobExecutor = org.activiti.engine.impl.jobexecutor.JobExecutor;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class ProcessEngineImpl : ProcessEngine
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(ProcessEngineImpl));

	  protected internal string name;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historicDataService;
	  protected internal IdentityService identityService;
	  protected internal TaskService taskService;
	  protected internal FormService formService;
	  protected internal ManagementService managementService;
	  protected internal DynamicBpmnService dynamicBpmnService;
	  protected internal JobExecutor jobExecutor;
	  protected internal AsyncExecutor asyncExecutor;
	  protected internal CommandExecutor commandExecutor;
	  protected internal IDictionary<Type, SessionFactory> sessionFactories;
	  protected internal ExpressionManager expressionManager;
	  protected internal TransactionContextFactory transactionContextFactory;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

	  public ProcessEngineImpl(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		this.processEngineConfiguration = processEngineConfiguration;
		this.name = processEngineConfiguration.ProcessEngineName;
		this.repositoryService = processEngineConfiguration.RepositoryService;
		this.runtimeService = processEngineConfiguration.RuntimeService;
		this.historicDataService = processEngineConfiguration.HistoryService;
		this.identityService = processEngineConfiguration.IdentityService;
		this.taskService = processEngineConfiguration.TaskService;
		this.formService = processEngineConfiguration.FormService;
		this.managementService = processEngineConfiguration.ManagementService;
		this.dynamicBpmnService = processEngineConfiguration.DynamicBpmnService;
		this.jobExecutor = processEngineConfiguration.JobExecutor;
		this.asyncExecutor = processEngineConfiguration.AsyncExecutor;
		this.commandExecutor = processEngineConfiguration.CommandExecutor;
		this.sessionFactories = processEngineConfiguration.SessionFactories;
		this.transactionContextFactory = processEngineConfiguration.getTransactionContextFactory();

		commandExecutor.execute(processEngineConfiguration.SchemaCommandConfig, new SchemaOperationsProcessEngineBuild());

		if (name == null)
		{
		  log.info("default activiti ProcessEngine created");
		}
		else
		{
		  log.info("ProcessEngine {} created", name);
		}

		ProcessEngines.registerProcessEngine(this);

		if (jobExecutor != null && jobExecutor.AutoActivate)
		{
		  jobExecutor.start();
		}

		if (asyncExecutor != null && asyncExecutor.AutoActivate)
		{
		  asyncExecutor.start();
		}

		if (processEngineConfiguration.ProcessEngineLifecycleListener != null)
		{
		  processEngineConfiguration.ProcessEngineLifecycleListener.onProcessEngineBuilt(this);
		}

		processEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createGlobalEvent(ActivitiEventType.ENGINE_CREATED));
	  }

	  public virtual void close()
	  {
		ProcessEngines.unregister(this);
		if (jobExecutor != null && jobExecutor.Active)
		{
		  jobExecutor.shutdown();
		}

		if (asyncExecutor != null && asyncExecutor.Active)
		{
		  asyncExecutor.shutdown();
		}

		commandExecutor.execute(processEngineConfiguration.SchemaCommandConfig, new SchemaOperationProcessEngineClose());

		if (processEngineConfiguration.ProcessEngineLifecycleListener != null)
		{
		  processEngineConfiguration.ProcessEngineLifecycleListener.onProcessEngineClosed(this);
		}

		processEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createGlobalEvent(ActivitiEventType.ENGINE_CLOSED));
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual IdentityService IdentityService
	  {
		  get
		  {
			return identityService;
		  }
	  }

	  public virtual ManagementService ManagementService
	  {
		  get
		  {
			return managementService;
		  }
	  }

	  public virtual TaskService TaskService
	  {
		  get
		  {
			return taskService;
		  }
	  }

	  public virtual HistoryService HistoryService
	  {
		  get
		  {
			return historicDataService;
		  }
	  }

	  public virtual RuntimeService RuntimeService
	  {
		  get
		  {
			return runtimeService;
		  }
	  }

	  public virtual RepositoryService RepositoryService
	  {
		  get
		  {
			return repositoryService;
		  }
	  }

	  public virtual FormService FormService
	  {
		  get
		  {
			return formService;
		  }
	  }

	  public virtual DynamicBpmnService DynamicBpmnService
	  {
		  get
		  {
			return dynamicBpmnService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
	  {
		  get
		  {
			return processEngineConfiguration;
		  }
	  }
	}

}