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
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using JSONObject = org.activiti.engine.impl.util.json.JSONObject;
	using LogMDC = org.activiti.engine.logging.LogMDC;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	public class TimerCatchIntermediateEventJobHandler : TimerEventHandler, JobHandler
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(TimerCatchIntermediateEventJobHandler));

	  public const string TYPE = "timer-intermediate-transition";

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

		ActivityImpl intermediateEventActivity = execution.ProcessDefinition.findActivity(nestedActivityId);

		if (intermediateEventActivity == null)
		{
		  throw new ActivitiException("Error while firing timer: intermediate event activity " + nestedActivityId + " not found");
		}

		try
		{
		  if (commandContext.EventDispatcher.Enabled)
		  {
			commandContext.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.TIMER_FIRED, job));
		  }

		  if (!execution.Activity.Id.Equals(intermediateEventActivity.Id))
		  {
			execution.Activity = intermediateEventActivity;
		  }
		  execution.signal(null, null);
		}
		catch (Exception e)
		{
		  LogMDC.putMDCExecution(execution);
		  log.error("exception during timer execution", e);
		  LogMDC.clear();
		  throw e;
		}
		catch (Exception e)
		{
		  LogMDC.putMDCExecution(execution);
		  log.error("exception during timer execution", e);
		  LogMDC.clear();
		  throw new ActivitiException("exception during timer execution: " + e.Message, e);
		}
	  }

	}

}