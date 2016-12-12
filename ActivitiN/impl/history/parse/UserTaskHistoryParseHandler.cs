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
namespace org.activiti.engine.impl.history.parse
{

	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using UserTask = org.activiti.bpmn.model.UserTask;
	using TaskListener = org.activiti.engine.@delegate.TaskListener;
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using org.activiti.engine.impl.bpmn.parser.handler;
	using UserTaskParseHandler = org.activiti.engine.impl.bpmn.parser.handler.UserTaskParseHandler;
	using UserTaskAssignmentHandler = org.activiti.engine.impl.history.handler.UserTaskAssignmentHandler;
	using UserTaskIdHandler = org.activiti.engine.impl.history.handler.UserTaskIdHandler;
	using TaskDefinition = org.activiti.engine.impl.task.TaskDefinition;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class UserTaskHistoryParseHandler : AbstractBpmnParseHandler<UserTask>
	{

	  protected internal static readonly UserTaskAssignmentHandler USER_TASK_ASSIGNMENT_HANDLER = new UserTaskAssignmentHandler();

	  protected internal static readonly UserTaskIdHandler USER_TASK_ID_HANDLER = new UserTaskIdHandler();

	  protected internal virtual Type HandledType
	  {
		  get
		  {
			return typeof(UserTask);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, UserTask element)
	  {
		TaskDefinition taskDefinition = (TaskDefinition) bpmnParse.CurrentActivity.getProperty(UserTaskParseHandler.PROPERTY_TASK_DEFINITION);
		taskDefinition.addTaskListener(org.activiti.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT, USER_TASK_ASSIGNMENT_HANDLER);
		taskDefinition.addTaskListener(org.activiti.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE, USER_TASK_ID_HANDLER);
	  }

	}

}