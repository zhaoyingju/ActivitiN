using System;

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
namespace org.activiti.engine.impl.bpmn.behavior
{

	using BpmnError = org.activiti.engine.@delegate.BpmnError;
	using ErrorPropagation = org.activiti.engine.impl.bpmn.helper.ErrorPropagation;
	using Context = org.activiti.engine.impl.context.Context;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ScriptingEngines = org.activiti.engine.impl.scripting.ScriptingEngines;
	using StringUtils = org.apache.commons.lang3.StringUtils;
	using ExceptionUtils = org.apache.commons.lang3.exception.ExceptionUtils;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;


	/// <summary>
	/// activity implementation of the BPMN 2.0 script task.
	/// 
	/// @author Joram Barrez
	/// @author Christian Stettler
	/// @author Falko Menge
	/// </summary>
	public class ScriptTaskActivityBehavior : TaskActivityBehavior
	{

	  private const long serialVersionUID = 1L;

	  private static readonly Logger LOGGER = LoggerFactory.getLogger(typeof(ScriptTaskActivityBehavior));

	  protected internal string scriptTaskId;
	  protected internal string script;
	  protected internal string language;
	  protected internal string resultVariable;
	  protected internal bool storeScriptVariables = false; // https://activiti.atlassian.net/browse/ACT-1626

	  public ScriptTaskActivityBehavior(string script, string language, string resultVariable)
	  {
		this.script = script;
		this.language = language;
		this.resultVariable = resultVariable;
	  }

	  public ScriptTaskActivityBehavior(string scriptTaskId, string script, string language, string resultVariable, bool storeScriptVariables) : this(script, language, resultVariable)
	  {
		this.scriptTaskId = scriptTaskId;
		this.storeScriptVariables = storeScriptVariables;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		ScriptingEngines scriptingEngines = Context.ProcessEngineConfiguration.ScriptingEngines;

		if (Context.ProcessEngineConfiguration.EnableProcessDefinitionInfoCache)
		{
		  ObjectNode taskElementProperties = Context.getBpmnOverrideElementProperties(scriptTaskId, execution.ProcessDefinitionId);
		  if (taskElementProperties != null && taskElementProperties.has(org.activiti.engine.DynamicBpmnConstants_Fields.SCRIPT_TASK_SCRIPT))
		  {
			string overrideScript = taskElementProperties.get(org.activiti.engine.DynamicBpmnConstants_Fields.SCRIPT_TASK_SCRIPT).asText();
			if (StringUtils.isNotEmpty(overrideScript) && overrideScript.Equals(script) == false)
			{
			  script = overrideScript;
			}
		  }
		}

		bool noErrors = true;
		try
		{
		  object result = scriptingEngines.evaluate(script, language, execution, storeScriptVariables);

		  if (resultVariable != null)
		  {
			execution.setVariable(resultVariable, result);
		  }

		}
		catch (ActivitiException e)
		{

		  LOGGER.warn("Exception while executing " + execution.Activity.Id + " : " + e.Message);

		  noErrors = false;
		  Exception rootCause = ExceptionUtils.getRootCause(e);
		  if (rootCause is BpmnError)
		  {
			ErrorPropagation.propagateError((BpmnError) rootCause, execution);
		  }
		  else
		  {
			throw e;
		  }
		}

		if (noErrors)
		{
		  leave(execution);
		}
	  }

	}

}