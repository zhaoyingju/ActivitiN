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

	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using org.activiti.engine.impl.cmd;
	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using DeploymentManager = org.activiti.engine.impl.persistence.deploy.DeploymentManager;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	public class TimerStartEventJobHandler : TimerEventHandler, JobHandler
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(TimerStartEventJobHandler));

	  public const string TYPE = "timer-start-event";

	  public virtual string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public virtual void execute(JobEntity job, string configuration, ExecutionEntity execution, CommandContext commandContext)
	  {

		DeploymentManager deploymentManager = Context.ProcessEngineConfiguration.DeploymentManager;

		if (TimerEventHandler.hasRealActivityId(configuration))
		{
		  startProcessInstanceWithInitialActivity(job, configuration, deploymentManager, commandContext);
		}
		else
		{
		  startProcessDefinitionByKey(job, configuration, deploymentManager, commandContext);
		}
	  }

	  protected internal virtual void startProcessInstanceWithInitialActivity(JobEntity job, string configuration, DeploymentManager deploymentManager, CommandContext commandContext)
	  {
		ProcessDefinitionEntity processDefinition = deploymentManager.findDeployedProcessDefinitionById(job.ProcessDefinitionId);

		string activityId = getActivityIdFromConfiguration(configuration);
		ActivityImpl startActivity = processDefinition.findActivity(activityId);

		if (!deploymentManager.isProcessDefinitionSuspended(processDefinition.Id))
		{
		  dispatchTimerFiredEvent(job, commandContext);

		  ExecutionEntity processInstance = processDefinition.createProcessInstance(null, startActivity);
		  processInstance.start();

		}
		else
		{
		  log.debug("Ignoring timer of suspended process definition {}", processDefinition.Id);
		}

	  }

	  protected internal virtual void startProcessDefinitionByKey(JobEntity job, string configuration, DeploymentManager deploymentManager, CommandContext commandContext)
	  {

		// it says getActivityId, but < 5.21, this would have the process definition key stored
		string processDefinitionKey = TimerEventHandler.getActivityIdFromConfiguration(configuration);

		ProcessDefinition processDefinition = null;
		if (job.TenantId == null || ProcessEngineConfiguration.NO_TENANT_ID.Equals(job.TenantId))
		{
				processDefinition = deploymentManager.findDeployedLatestProcessDefinitionByKey(processDefinitionKey);
		}
		else
		{
			processDefinition = deploymentManager.findDeployedLatestProcessDefinitionByKeyAndTenantId(processDefinitionKey, job.TenantId);
		}

		if (processDefinition == null)
		{
			throw new ActivitiException("Could not find process definition needed for timer start event");
		}

		try
		{
		  if (!deploymentManager.isProcessDefinitionSuspended(processDefinition.Id))
		  {
			dispatchTimerFiredEvent(job, commandContext);

			(new StartProcessInstanceCmd<ProcessInstance>(processDefinitionKey, null, null, null, job.TenantId)).execute(commandContext);
		  }
		  else
		  {
			log.debug("Ignoring timer of suspended process definition {}", processDefinition.Id);
		  }
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

	  protected internal virtual void dispatchTimerFiredEvent(JobEntity job, CommandContext commandContext)
	  {
		if (commandContext.EventDispatcher.Enabled)
		{
		  commandContext.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.TIMER_FIRED, job));
		}
	  }


	}

}