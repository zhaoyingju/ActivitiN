using System;
using System.Collections.Generic;
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

namespace org.activiti.engine.impl.test
{


	using AssertionFailedError = junit.framework.AssertionFailedError;

	using BpmnModel = org.activiti.bpmn.model.BpmnModel;
	using EndEvent = org.activiti.bpmn.model.EndEvent;
	using SequenceFlow = org.activiti.bpmn.model.SequenceFlow;
	using StartEvent = org.activiti.bpmn.model.StartEvent;
	using UserTask = org.activiti.bpmn.model.UserTask;
	using HistoricActivityInstance = org.activiti.engine.history.HistoricActivityInstance;
	using HistoricProcessInstance = org.activiti.engine.history.HistoricProcessInstance;
	using HistoricTaskInstance = org.activiti.engine.history.HistoricTaskInstance;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using HistoryLevel = org.activiti.engine.impl.history.HistoryLevel;
	using org.activiti.engine.impl.interceptor;
	using CommandConfig = org.activiti.engine.impl.interceptor.CommandConfig;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using Deployment = org.activiti.engine.repository.Deployment;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;
	using Assert = org.junit.Assert;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public abstract class AbstractActivitiTestCase : PvmTestCase
	{

	  private static readonly IList<string> TABLENAMES_EXCLUDED_FROM_DB_CLEAN_CHECK = Arrays.asList("ACT_GE_PROPERTY");

	  protected internal ProcessEngine processEngine;

	  protected internal string deploymentIdFromDeploymentAnnotation;
	  protected internal IList<string> deploymentIdsForAutoCleanup = new List<string>();
	  protected internal Exception exception;

	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal FormService formService;
	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;
	  protected internal ManagementService managementService;
	  protected internal DynamicBpmnService dynamicBpmnService;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();

		// Always reset authenticated user to avoid any mistakes
		identityService.AuthenticatedUserId = null;
	  }

	  protected internal abstract void initializeProcessEngine();

	  // Default: do nothing
	  protected internal virtual void closeDownProcessEngine()
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void runBare() throws Throwable
	  public override void runBare()
	  {
		initializeProcessEngine();
		if (repositoryService == null)
		{
		  initializeServices();
		}

		try
		{

			deploymentIdFromDeploymentAnnotation = TestHelper.annotationDeploymentSetUp(processEngine, this.GetType(), Name);

		  base.runBare();

		}
		catch (AssertionFailedError e)
		{
		  log.error(EMPTY_LINE);
		  log.error("ASSERTION FAILED: {}", e, e);
		  exception = e;
		  throw e;

		}
		catch (Exception e)
		{
		  log.error(EMPTY_LINE);
		  log.error("EXCEPTION: {}",e, e);
		  exception = e;
		  throw e;

		}
		finally
		{
			if (deploymentIdFromDeploymentAnnotation != null)
			{
				TestHelper.annotationDeploymentTearDown(processEngine, deploymentIdFromDeploymentAnnotation, this.GetType(), Name);
				deploymentIdFromDeploymentAnnotation = null;
			}

			foreach (string autoDeletedDeploymentId in deploymentIdsForAutoCleanup)
			{
				repositoryService.deleteDeployment(autoDeletedDeploymentId, true);
			}
			deploymentIdsForAutoCleanup.Clear();

		  assertAndEnsureCleanDb();
		  processEngineConfiguration.Clock.reset();

		  // Can't do this in the teardown, as the teardown will be called as part of the super.runBare
		  closeDownProcessEngine();
		}
	  }

	  /// <summary>
	  /// Each test is assumed to clean up all DB content it entered.
	  /// After a test method executed, this method scans all tables to see if the DB is completely clean. 
	  /// It throws AssertionFailed in case the DB is not clean.
	  /// If the DB is not clean, it is cleaned by performing a create a drop. 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void assertAndEnsureCleanDb() throws Throwable
	  protected internal virtual void assertAndEnsureCleanDb()
	  {
		log.debug("verifying that db is clean after test");
		IDictionary<string, long?> tableCounts = managementService.TableCount;
		StringBuilder outputMessage = new StringBuilder();
		foreach (string tableName in tableCounts.Keys)
		{
		  string tableNameWithoutPrefix = tableName.Replace(processEngineConfiguration.DatabaseTablePrefix, "");
		  if (!TABLENAMES_EXCLUDED_FROM_DB_CLEAN_CHECK.Contains(tableNameWithoutPrefix))
		  {
			long? count = tableCounts[tableName];
			if (count != 0L)
			{
			  outputMessage.Append("  ").Append(tableName).Append(": ").Append(count).Append(" record(s) ");
			}
		  }
		}
		if (outputMessage.Length > 0)
		{
		  outputMessage.Insert(0, "DB NOT CLEAN: \n");
		  log.error(EMPTY_LINE);
		  log.error(outputMessage.ToString());

		  log.info("dropping and recreating db");

		  CommandExecutor commandExecutor = ((ProcessEngineImpl)processEngine).ProcessEngineConfiguration.CommandExecutor;
		  CommandConfig config = (new CommandConfig()).transactionNotSupported();
		  commandExecutor.execute(config, new CommandAnonymousInnerClassHelper(this));

		  if (exception != null)
		  {
			throw exception;
		  }
		  else
		  {
			Assert.fail(outputMessage.ToString());
		  }
		}
		else
		{
		  log.info("database was clean");
		}
	  }

	  private class CommandAnonymousInnerClassHelper : Command<object>
	  {
		  private readonly AbstractActivitiTestCase outerInstance;

		  public CommandAnonymousInnerClassHelper(AbstractActivitiTestCase outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public virtual object execute(CommandContext commandContext)
		  {
			DbSqlSession session = commandContext.getSession(typeof(DbSqlSession));
			session.dbSchemaDrop();
			session.dbSchemaCreate();
			return null;
		  }
	  }


	  protected internal virtual void initializeServices()
	  {
		processEngineConfiguration = ((ProcessEngineImpl) processEngine).ProcessEngineConfiguration;
		repositoryService = processEngine.RepositoryService;
		runtimeService = processEngine.RuntimeService;
		taskService = processEngine.TaskService;
		formService = processEngine.FormService;
		historyService = processEngine.HistoryService;
		identityService = processEngine.IdentityService;
		managementService = processEngine.ManagementService;
		dynamicBpmnService = processEngine.DynamicBpmnService;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void assertProcessEnded(final String processInstanceId)
	  public virtual void assertProcessEnded(string processInstanceId)
	  {
		ProcessInstance processInstance = processEngine.RuntimeService.createProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		if (processInstance != null)
		{
		  throw new AssertionFailedError("Expected finished process instance '" + processInstanceId + "' but it was still in the db");
		}

		// Verify historical data if end times are correctly set
		if (processEngineConfiguration.HistoryLevel.isAtLeast(HistoryLevel.AUDIT))
		{

		  // process instance
		  HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();
		  assertEquals(processInstanceId, historicProcessInstance.Id);
		  assertNotNull("Historic process instance has no start time", historicProcessInstance.StartTime);
		  assertNotNull("Historic process instance has no end time", historicProcessInstance.EndTime);

		  // tasks
		  IList<HistoricTaskInstance> historicTaskInstances = historyService.createHistoricTaskInstanceQuery().processInstanceId(processInstanceId).list();
		  if (historicTaskInstances != null && historicTaskInstances.Count > 0)
		  {
			foreach (HistoricTaskInstance historicTaskInstance in historicTaskInstances)
			{
			  assertEquals(processInstanceId, historicTaskInstance.ProcessInstanceId);
			  assertNotNull("Historic task " + historicTaskInstance.TaskDefinitionKey + " has no start time", historicTaskInstance.StartTime);
			  assertNotNull("Historic task " + historicTaskInstance.TaskDefinitionKey + " has no end time", historicTaskInstance.EndTime);
			}
		  }

		  // activities
		  IList<HistoricActivityInstance> historicActivityInstances = historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstanceId).list();
		  if (historicActivityInstances != null && historicActivityInstances.Count > 0)
		  {
			foreach (HistoricActivityInstance historicActivityInstance in historicActivityInstances)
			{
			  assertEquals(processInstanceId, historicActivityInstance.ProcessInstanceId);
			  assertNotNull("Historic activity instance " + historicActivityInstance.ActivityId + " has no start time", historicActivityInstance.StartTime);
			  assertNotNull("Historic activity instance " + historicActivityInstance.ActivityId + " has no end time", historicActivityInstance.EndTime);
			}
		  }
		}
	  }

	  public virtual void waitForJobExecutorToProcessAllJobs(long maxMillisToWait, long intervalMillis)
	  {
		JobTestHelper.waitForJobExecutorToProcessAllJobs(processEngineConfiguration, managementService, maxMillisToWait, intervalMillis);
	  }

	  public virtual void waitForJobExecutorOnCondition(long maxMillisToWait, long intervalMillis, Callable<bool?> condition)
	  {
		JobTestHelper.waitForJobExecutorOnCondition(processEngineConfiguration, maxMillisToWait, intervalMillis, condition);
	  }

	  public virtual void executeJobExecutorForTime(long maxMillisToWait, long intervalMillis)
	  {
		JobTestHelper.executeJobExecutorForTime(processEngineConfiguration, maxMillisToWait, intervalMillis);
	  }

	  /// <summary>
	  /// Since the 'one task process' is used everywhere the actual process content
	  /// doesn't matter, instead of copying around the BPMN 2.0 xml one could use 
	  /// this method which gives a <seealso cref="BpmnModel"/> version of the same process back.
	  /// </summary>
	  public virtual BpmnModel createOneTaskTestProcess()
	  {
		  BpmnModel model = new BpmnModel();
		  org.activiti.bpmn.model.Process process = new org.activiti.bpmn.model.Process();
		model.addProcess(process);
		process.Id = "oneTaskProcess";
		process.Name = "The one task process";

		StartEvent startEvent = new StartEvent();
		startEvent.Id = "start";
		process.addFlowElement(startEvent);

		UserTask userTask = new UserTask();
		userTask.Name = "The Task";
		userTask.Id = "theTask";
		userTask.Assignee = "kermit";
		process.addFlowElement(userTask);

		EndEvent endEvent = new EndEvent();
		endEvent.Id = "theEnd";
		process.addFlowElement(endEvent);

		process.addFlowElement(new SequenceFlow("start", "theTask"));
		process.addFlowElement(new SequenceFlow("theTask", "theEnd"));

		return model;
	  }

	  public virtual BpmnModel createTwoTasksTestProcess()
	  {
		  BpmnModel model = new BpmnModel();
		  org.activiti.bpmn.model.Process process = new org.activiti.bpmn.model.Process();
		model.addProcess(process);
		process.Id = "twoTasksProcess";
		process.Name = "The two tasks process";

		StartEvent startEvent = new StartEvent();
		startEvent.Id = "start";
		process.addFlowElement(startEvent);

		UserTask userTask = new UserTask();
		userTask.Name = "The First Task";
		userTask.Id = "task1";
		userTask.Assignee = "kermit";
		process.addFlowElement(userTask);

		UserTask userTask2 = new UserTask();
		userTask2.Name = "The Second Task";
		userTask2.Id = "task2";
		userTask2.Assignee = "kermit";
		process.addFlowElement(userTask2);

		EndEvent endEvent = new EndEvent();
		endEvent.Id = "theEnd";
		process.addFlowElement(endEvent);

		process.addFlowElement(new SequenceFlow("start", "task1"));
		process.addFlowElement(new SequenceFlow("start", "task2"));
		process.addFlowElement(new SequenceFlow("task1", "theEnd"));
		process.addFlowElement(new SequenceFlow("task2", "theEnd"));

		return model;
	  }

	  /// <summary>
	  /// Creates and deploys the one task process. See <seealso cref="#createOneTaskTestProcess()"/>.
	  /// </summary>
	  /// <returns> The process definition id (NOT the process definition key) of deployed one task process. </returns>
	  public virtual string deployOneTaskTestProcess()
	  {
		  BpmnModel bpmnModel = createOneTaskTestProcess();
		  Deployment deployment = repositoryService.createDeployment().addBpmnModel("oneTasktest.bpmn20.xml", bpmnModel).deploy();

		  deploymentIdsForAutoCleanup.Add(deployment.Id); // For auto-cleanup

		  ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id).singleResult();
		  return processDefinition.Id;
	  }

	  public virtual string deployTwoTasksTestProcess()
	  {
		  BpmnModel bpmnModel = createTwoTasksTestProcess();
		  Deployment deployment = repositoryService.createDeployment().addBpmnModel("twoTasksTestProcess.bpmn20.xml", bpmnModel).deploy();

		  deploymentIdsForAutoCleanup.Add(deployment.Id); // For auto-cleanup

		  ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().deploymentId(deployment.Id).singleResult();
		  return processDefinition.Id;
	  }
	}

}