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
namespace org.activiti.engine.impl.bpmn.parser.handler
{

	using BpmnXMLConstants = org.activiti.bpmn.constants.BpmnXMLConstants;
	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using ScriptTask = org.activiti.bpmn.model.ScriptTask;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using StringUtils = org.apache.commons.lang3.StringUtils;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ScriptTaskParseHandler : AbstractActivityBpmnParseHandler<ScriptTask>
	{

		private static readonly Logger logger = LoggerFactory.getLogger(typeof(ScriptTaskParseHandler));

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(ScriptTask);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, ScriptTask scriptTask)
	  {

		if (StringUtils.isEmpty(scriptTask.Script))
		{
		  logger.warn("No script provided for scriptTask " + scriptTask.Id);
		}

		ActivityImpl activity = createActivityOnCurrentScope(bpmnParse, scriptTask, BpmnXMLConstants.ELEMENT_TASK_SCRIPT);

		activity.Async = scriptTask.Asynchronous;
		activity.Exclusive = !scriptTask.NotExclusive;

		activity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createScriptTaskActivityBehavior(scriptTask);

	  }

	}

}