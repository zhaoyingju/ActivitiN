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
namespace org.activiti.engine.impl.jobexecutor
{

	using Expression = org.activiti.engine.@delegate.Expression;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using BoundaryEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.BoundaryEventActivityBehavior;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using JSONObject = org.activiti.engine.impl.util.json.JSONObject;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class TimerExecuteNestedActivityJobHandler : TimerEventHandler, JobHandler
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(TimerExecuteNestedActivityJobHandler));

	  public const string TYPE = "timer-transition";
	  public new const string PROPERTYNAME_TIMER_ACTIVITY_ID = "activityId";
	  public new const string PROPERTYNAME_END_DATE_EXPRESSION = "timerEndDate";

	  public virtual string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public virtual void execute(JobEntity job, string configuration, ExecutionEntity execution, CommandContext commandContext)
	  {

		string nestedActivityId = TimerEventHandler.getActivityIdFromConfiguration(configuration);

		ActivityImpl borderEventActivity = execution.ProcessDefinition.findActivity(nestedActivityId);

		if (borderEventActivity == null)
		{
		  throw new ActivitiException("Error while firing timer: border event activity " + nestedActivityId + " not found");
		}

		try
		{
		  if (commandContext.EventDispatcher.Enabled)
		  {
			commandContext.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.TIMER_FIRED, job));
			dispatchActivityTimeoutIfNeeded(job, execution, commandContext);
		  }

		  borderEventActivity.ActivityBehavior.execute(execution);
		}
		catch (Exception e)
		{
		  log.error("exception during timer execution", e);
		  throw e;

		}
		catch (Exception e)
		{
		  log.error("exception during timer execution", e);
		  throw new ActivitiException("exception during timer execution: " + e.Message, e);
		}
	  }

	  protected internal virtual void dispatchActivityTimeoutIfNeeded(JobEntity timerEntity, ExecutionEntity execution, CommandContext commandContext)
	  {

		string nestedActivityId = TimerEventHandler.getActivityIdFromConfiguration(timerEntity.JobHandlerConfiguration);

		ActivityImpl boundaryEventActivity = execution.ProcessDefinition.findActivity(nestedActivityId);
		ActivityBehavior boundaryActivityBehavior = boundaryEventActivity.ActivityBehavior;
		if (boundaryActivityBehavior is BoundaryEventActivityBehavior)
		{
		  BoundaryEventActivityBehavior boundaryEventActivityBehavior = (BoundaryEventActivityBehavior) boundaryActivityBehavior;
		  if (boundaryEventActivityBehavior.Interrupting)
		  {
			dispatchExecutionTimeOut(timerEntity, execution, commandContext);
		  }
		}
	  }

	  protected internal virtual void dispatchExecutionTimeOut(JobEntity timerEntity, ExecutionEntity execution, CommandContext commandContext)
	  {
		// subprocesses
		foreach (ExecutionEntity subExecution in execution.Executions)
		{
		  dispatchExecutionTimeOut(timerEntity, subExecution, commandContext);
		}

		// call activities
		ExecutionEntity subProcessInstance = commandContext.ExecutionEntityManager.findSubProcessInstanceBySuperExecutionId(execution.Id);
		if (subProcessInstance != null)
		{
		  dispatchExecutionTimeOut(timerEntity, subProcessInstance, commandContext);
		}

		// activity with timer boundary event
		ActivityImpl activity = execution.Activity;
		if (activity != null && activity.ActivityBehavior != null)
		{
		  dispatchActivityTimeOut(timerEntity, activity, execution, commandContext);
		}
	  }

	  protected internal virtual void dispatchActivityTimeOut(JobEntity timerEntity, ActivityImpl activity, ExecutionEntity execution, CommandContext commandContext)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		commandContext.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createActivityCancelledEvent(activity.Id, (string) activity.Properties["name"], execution.Id, execution.ProcessInstanceId, execution.ProcessDefinitionId, (string) activity.Properties["type"], activity.ActivityBehavior.GetType().FullName, timerEntity));
	  }

	}

}