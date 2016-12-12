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

	using BpmnDeployer = org.activiti.engine.impl.bpmn.deployer.BpmnDeployer;
	using ActivityBehaviorFactory = org.activiti.engine.impl.bpmn.parser.factory.ActivityBehaviorFactory;
	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ReflectUtil = org.activiti.engine.impl.util.ReflectUtil;
	using DeploymentBuilder = org.activiti.engine.repository.DeploymentBuilder;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;
	using Deployment = org.activiti.engine.test.Deployment;
	using TestActivityBehaviorFactory = org.activiti.engine.test.TestActivityBehaviorFactory;
	using ActivitiMockSupport = org.activiti.engine.test.mock.ActivitiMockSupport;
	using MockServiceTask = org.activiti.engine.test.mock.MockServiceTask;
	using MockServiceTasks = org.activiti.engine.test.mock.MockServiceTasks;
	using NoOpServiceTasks = org.activiti.engine.test.mock.NoOpServiceTasks;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public abstract class TestHelper
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(TestHelper));

	  public const string EMPTY_LINE = "\n";

	  public static readonly IList<string> TABLENAMES_EXCLUDED_FROM_DB_CLEAN_CHECK = Arrays.asList("ACT_GE_PROPERTY");

	  internal static IDictionary<string, ProcessEngine> processEngines = new Dictionary<string, ProcessEngine>();

	  // Assertion methods ///////////////////////////////////////////////////

	  public static void assertProcessEnded(ProcessEngine processEngine, string processInstanceId)
	  {
		ProcessInstance processInstance = processEngine.RuntimeService.createProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();

		if (processInstance != null)
		{
		  throw new AssertionFailedError("expected finished process instance '" + processInstanceId + "' but it was still in the db");
		}
	  }

	  // Test annotation support /////////////////////////////////////////////

	  public static string annotationDeploymentSetUp(ProcessEngine processEngine, Type testClass, string methodName)
	  {
		string deploymentId = null;
		Method method = null;
		try
		{
		  method = testClass.GetMethod(methodName, (Type[]) null);
		}
		catch (Exception e)
		{
			log.warn("Could not get method by reflection. This could happen if you are using @Parameters in combination with annotations.", e);
			return null;
		}
		Deployment deploymentAnnotation = method.getAnnotation(typeof(Deployment));
		if (deploymentAnnotation != null)
		{
		  log.debug("annotation @Deployment creates deployment for {}.{}", testClass.Name, methodName);
		  string[] resources = deploymentAnnotation.resources();
		  if (resources.Length == 0)
		  {
			string name = method.Name;
			string resource = getBpmnProcessDefinitionResource(testClass, name);
			resources = new string[]{resource};
		  }

		  DeploymentBuilder deploymentBuilder = processEngine.RepositoryService.createDeployment().name(testClass.Name + "." + methodName);

		  foreach (string resource in resources)
		  {
			deploymentBuilder.addClasspathResource(resource);
		  }

		  deploymentId = deploymentBuilder.deploy().Id;
		}

		return deploymentId;
	  }

	  public static void annotationDeploymentTearDown(ProcessEngine processEngine, string deploymentId, Type testClass, string methodName)
	  {
		log.debug("annotation @Deployment deletes deployment for {}.{}", testClass.Name, methodName);
		if (deploymentId != null)
		{
		  try
		  {
			processEngine.RepositoryService.deleteDeployment(deploymentId, true);
		  }
		  catch (ActivitiObjectNotFoundException)
		  {
			// Deployment was already deleted by the test case. Ignore.
		  }
		}
	  }

	  public static void annotationMockSupportSetup(Type testClass, string methodName, ActivitiMockSupport mockSupport)
	  {

		  // Get method
		   Method method = null;
		 try
		 {
		   method = testClass.GetMethod(methodName, (Type[]) null);
		 }
		 catch (Exception e)
		 {
			log.warn("Could not get method by reflection. This could happen if you are using @Parameters in combination with annotations.", e);
			return;
		 }

		 handleMockServiceTaskAnnotation(mockSupport, method);
		 handleMockServiceTasksAnnotation(mockSupport, method);
		 handleNoOpServiceTasksAnnotation(mockSupport, method);
	  }

		protected internal static void handleMockServiceTaskAnnotation(ActivitiMockSupport mockSupport, Method method)
		{
		  MockServiceTask mockedServiceTask = method.getAnnotation(typeof(MockServiceTask));
		 if (mockedServiceTask != null)
		 {
			 handleMockServiceTaskAnnotation(mockSupport, mockedServiceTask);
		 }
		}

		protected internal static void handleMockServiceTaskAnnotation(ActivitiMockSupport mockSupport, MockServiceTask mockedServiceTask)
		{
		  mockSupport.mockServiceTaskWithClassDelegate(mockedServiceTask.originalClassName(), mockedServiceTask.mockedClassName());
		}

		protected internal static void handleMockServiceTasksAnnotation(ActivitiMockSupport mockSupport, Method method)
		{
		  MockServiceTasks mockedServiceTasks = method.getAnnotation(typeof(MockServiceTasks));
		 if (mockedServiceTasks != null)
		 {
			 foreach (MockServiceTask mockedServiceTask in mockedServiceTasks.value())
			 {
				 handleMockServiceTaskAnnotation(mockSupport, mockedServiceTask);
			 }
		 }
		}

		protected internal static void handleNoOpServiceTasksAnnotation(ActivitiMockSupport mockSupport, Method method)
		{
			NoOpServiceTasks noOpServiceTasks = method.getAnnotation(typeof(NoOpServiceTasks));
			if (noOpServiceTasks != null)
			{

				string[] ids = noOpServiceTasks.ids();
				Type[] classes = noOpServiceTasks.classes();
				string[] classNames = noOpServiceTasks.classNames();

				if ((ids == null || ids.Length == 0) && (classes == null || classes.Length == 0) && (classNames == null || classNames.Length == 0))
				{
					mockSupport.setAllServiceTasksNoOp();
				}
				else
				{

					if (ids != null && ids.Length > 0)
					{
						foreach (string id in ids)
						{
							mockSupport.addNoOpServiceTaskById(id);
						}
					}

					if (classes != null && classes.Length > 0)
					{
						foreach (Type clazz in classes)
						{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							mockSupport.addNoOpServiceTaskByClassName(clazz.FullName);
						}
					}

					if (classNames != null && classNames.Length > 0)
					{
						foreach (string className in classNames)
						{
							mockSupport.addNoOpServiceTaskByClassName(className);
						}
					}

				}

			}
		}

	  public static void annotationMockSupportTeardown(ActivitiMockSupport mockSupport)
	  {
		  mockSupport.reset();
	  }

	  /// <summary>
	  /// get a resource location by convention based on a class (type) and a
	  /// relative resource name. The return value will be the full classpath
	  /// location of the type, plus a suffix built from the name parameter:
	  /// <code>BpmnDeployer.BPMN_RESOURCE_SUFFIXES</code>. 
	  /// The first resource matching a suffix will be returned.
	  /// </summary>
	  public static string getBpmnProcessDefinitionResource(Type type, string name)
	  {
		foreach (string suffix in BpmnDeployer.BPMN_RESOURCE_SUFFIXES)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  string resource = type.FullName.Replace('.', '/') + "." + name + "." + suffix;
		  InputStream inputStream = ReflectUtil.getResourceAsStream(resource);
		  if (inputStream == null)
		  {
			continue;
		  }
		  else
		  {
			return resource;
		  }
		}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return type.FullName.Replace('.', '/') + "." + name + "." + BpmnDeployer.BPMN_RESOURCE_SUFFIXES[0];
	  }


	  // Engine startup and shutdown helpers  ///////////////////////////////////////////////////

	  public static ProcessEngine getProcessEngine(string configurationResource)
	  {
		ProcessEngine processEngine = processEngines[configurationResource];
		if (processEngine == null)
		{
		  log.debug("==== BUILDING PROCESS ENGINE ========================================================================");
		  processEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource(configurationResource).buildProcessEngine();
			log.debug("==== PROCESS ENGINE CREATED =========================================================================");
			processEngines[configurationResource] = processEngine;
		}
		  return processEngine;
	  }

	  public static void closeProcessEngines()
	  {
		foreach (ProcessEngine processEngine in processEngines.Values)
		{
		  processEngine.close();
		}
		processEngines.Clear();
	  }

	  /// <summary>
	  /// Each test is assumed to clean up all DB content it entered.
	  /// After a test method executed, this method scans all tables to see if the DB is completely clean. 
	  /// It throws AssertionFailed in case the DB is not clean.
	  /// If the DB is not clean, it is cleaned by performing a create a drop. 
	  /// </summary>
	  public static void assertAndEnsureCleanDb(ProcessEngine processEngine)
	  {
		log.debug("verifying that db is clean after test");
		IDictionary<string, long?> tableCounts = processEngine.ManagementService.TableCount;
		StringBuilder outputMessage = new StringBuilder();
		foreach (string tableName in tableCounts.Keys)
		{
		  if (!TABLENAMES_EXCLUDED_FROM_DB_CLEAN_CHECK.Contains(tableName))
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

		  ((ProcessEngineImpl)processEngine).ProcessEngineConfiguration.CommandExecutor.execute(new CommandAnonymousInnerClassHelper());

		  throw new AssertionError(outputMessage.ToString());
		}
	  }

	  private class CommandAnonymousInnerClassHelper : Command<object>
	  {
		  public CommandAnonymousInnerClassHelper()
		  {
		  }

		  public virtual object execute(CommandContext commandContext)
		  {
			DbSqlSession dbSqlSession = commandContext.getSession(typeof(DbSqlSession));
			dbSqlSession.dbSchemaDrop();
			dbSqlSession.dbSchemaCreate();
			return null;
		  }
	  }

	  // Mockup support ////////////////////////////////////////////////////////

	  public static TestActivityBehaviorFactory initializeTestActivityBehaviorFactory(ActivityBehaviorFactory existingActivityBehaviorFactory)
	  {
		return new TestActivityBehaviorFactory(existingActivityBehaviorFactory);
	  }

	}

}