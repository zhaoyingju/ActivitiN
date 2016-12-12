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
namespace org.activiti.engine.impl.persistence.entity
{


	using Expression = org.activiti.engine.@delegate.Expression;
	using VariableScope = org.activiti.engine.@delegate.VariableScope;
	using BusinessCalendar = org.activiti.engine.impl.calendar.BusinessCalendar;
	using CycleBusinessCalendar = org.activiti.engine.impl.calendar.CycleBusinessCalendar;
	using Context = org.activiti.engine.impl.context.Context;
	using NoExecutionVariableScope = org.activiti.engine.impl.el.NoExecutionVariableScope;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using TimerCatchIntermediateEventJobHandler = org.activiti.engine.impl.jobexecutor.TimerCatchIntermediateEventJobHandler;
	using TimerDeclarationImpl = org.activiti.engine.impl.jobexecutor.TimerDeclarationImpl;
	using TimerEventHandler = org.activiti.engine.impl.jobexecutor.TimerEventHandler;
	using TimerExecuteNestedActivityJobHandler = org.activiti.engine.impl.jobexecutor.TimerExecuteNestedActivityJobHandler;
	using TimerStartEventJobHandler = org.activiti.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using StringUtils = org.apache.commons.lang3.StringUtils;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class TimerEntity : JobEntity
	{

	  private const long serialVersionUID = 1L;

	  private static Logger log = LoggerFactory.getLogger(typeof(TimerEntity));

	  protected internal int maxIterations;
	  protected internal string repeat;
	  protected internal DateTime endDate;

	  public TimerEntity() : base()
	  {
		this.jobType = "timer";
	  }

	  public TimerEntity(TimerDeclarationImpl timerDeclaration)
	  {
		jobHandlerType = timerDeclaration.JobHandlerType;
		jobHandlerConfiguration = timerDeclaration.JobHandlerConfiguration;
		isExclusive = timerDeclaration.Exclusive;
		repeat = timerDeclaration.Repeat;
		retries = timerDeclaration.Retries;
		this.jobType = "timer";
	  }

	  private TimerEntity(TimerEntity te)
	  {
		jobHandlerConfiguration = te.jobHandlerConfiguration;
		jobHandlerType = te.jobHandlerType;
		isExclusive = te.isExclusive;
		repeat = te.repeat;
		retries = te.retries;
		endDate = te.endDate;
		executionId = te.executionId;
		processInstanceId = te.processInstanceId;
		processDefinitionId = te.processDefinitionId;

		// Inherit tenant
		tenantId = te.tenantId;
		this.jobType = "timer";
	  }

	  public override void execute(CommandContext commandContext)
	  {

		//set endDate if it was set to the definition
		restoreExtraData(commandContext, jobHandlerConfiguration);

		if (this.Duedate != null && !isValidTime(this.Duedate))
		{
		  if (log.DebugEnabled)
		  {
			log.debug("Timer {} fired. but the dueDate is after the endDate.  Deleting timer.", Id);
		  }
		  delete();
		  return;
		}

		base.execute(commandContext);

		if (log.DebugEnabled)
		{
		  log.debug("Timer {} fired. Deleting timer.", Id);
		}
		delete();

		if (repeat != null)
		{
		  int repeatValue = calculateRepeatValue();
		  if (repeatValue != 0)
		  {
			if (repeatValue > 0)
			{
			  NewRepeat = repeatValue;
			}

			DateTime newTimer = calculateNextTimer();
			if (newTimer != null && isValidTime(newTimer))
			{
			  TimerEntity te = new TimerEntity(this);
			  te.Duedate = newTimer;
			  Context.CommandContext.JobEntityManager.schedule(te);
			}
		  }
		}
	  }

	  protected internal virtual void restoreExtraData(CommandContext commandContext, string jobHandlerConfiguration)
	  {
		string embededActivityId = jobHandlerConfiguration;

		if (jobHandlerType.Equals(TimerExecuteNestedActivityJobHandler.TYPE, StringComparison.CurrentCultureIgnoreCase) || jobHandlerType.Equals(TimerCatchIntermediateEventJobHandler.TYPE, StringComparison.CurrentCultureIgnoreCase) || jobHandlerType.Equals(TimerStartEventJobHandler.TYPE, StringComparison.CurrentCultureIgnoreCase))
		{

		  embededActivityId = TimerEventHandler.getActivityIdFromConfiguration(jobHandlerConfiguration);

		  string endDateExpressionString = TimerEventHandler.getEndDateFromConfiguration(jobHandlerConfiguration);

		  if (endDateExpressionString != null)
		  {
			 Expression endDateExpression = Context.ProcessEngineConfiguration.ExpressionManager.createExpression(endDateExpressionString);

			string endDateString = null;

			BusinessCalendar businessCalendar = Context.ProcessEngineConfiguration.BusinessCalendarManager.getBusinessCalendar(getBusinessCalendarName(TimerEventHandler.geCalendarNameFromConfiguration(jobHandlerConfiguration)));

			VariableScope executionEntity = null;
			if (executionId != null)
			{
			  executionEntity = commandContext.ExecutionEntityManager.findExecutionById(executionId);
			}

			if (executionEntity == null)
			{
			  executionEntity = NoExecutionVariableScope.SharedInstance;
			}

			if (endDateExpression != null)
			{
			  object endDateValue = endDateExpression.getValue(executionEntity);
			  if (endDateValue is string)
			  {
				endDateString = (string) endDateValue;
			  }
			  else if (endDateValue is DateTime)
			  {
				endDate = (DateTime) endDateValue;
			  }
			  else
			  {
				throw new ActivitiException("Timer '" + ((ExecutionEntity) executionEntity).ActivityId + "' was not configured with a valid duration/time, either hand in a java.util.Date or a String in format 'yyyy-MM-dd'T'hh:mm:ss'");
			  }

			  if (endDate == null)
			  {
				endDate = businessCalendar.resolveEndDate(endDateString);
			  }
			}
		  }
		}

		 if (processDefinitionId != null)
		 {
		  ProcessDefinition definition = commandContext.ProcessEngineConfiguration.DeploymentManager.findDeployedProcessDefinitionById(processDefinitionId);
		  maxIterations = checkStartEventDefinitions(definition, embededActivityId);
		  if (maxIterations <= 1)
		  {
			maxIterations = checkBoundaryEventsDefinitions(definition, embededActivityId);
		  }
		 }
		else
		{
		  maxIterations = 1;
		}
	  }

	  protected internal virtual int checkStartEventDefinitions(ProcessDefinition def, string embededActivityId)
	  {
		IList<TimerDeclarationImpl> startTimerDeclarations = (IList<TimerDeclarationImpl>)((ProcessDefinitionEntity) def).getProperty("timerStart");

		if (startTimerDeclarations != null && startTimerDeclarations.Count > 0)
		{
		  TimerDeclarationImpl timerDeclaration = null;

		  foreach (TimerDeclarationImpl startTimerDeclaration in startTimerDeclarations)
		  {
			string definitionActivityId = TimerEventHandler.getActivityIdFromConfiguration(startTimerDeclaration.JobHandlerConfiguration);
			if (startTimerDeclaration.JobHandlerType.Equals(jobHandlerType, StringComparison.CurrentCultureIgnoreCase) && (definitionActivityId.Equals(embededActivityId, StringComparison.CurrentCultureIgnoreCase)))
			{
			  timerDeclaration = startTimerDeclaration;
			}
		  }

		  if (timerDeclaration != null)
		  {
			return calculateMaxIterationsValue(timerDeclaration.Description.ExpressionText);
		  }
		}
		return 1;
	  }

	  protected internal virtual int checkBoundaryEventsDefinitions(ProcessDefinition def, string embededActivityId)
	  {
		return checkBoundaryEventsDefinitions(((ProcessDefinitionEntity) def).Activities, embededActivityId);
	  }

	  protected internal virtual int checkBoundaryEventsDefinitions(IList<ActivityImpl> activities, string embededActivityId)
	  {
		// should check level by level, first check provided activities list 
		foreach (ActivityImpl activity in activities)
		{
		  IList<TimerDeclarationImpl> activityTimerDeclarations = (IList<TimerDeclarationImpl>) activity.getProperty("timerDeclarations");
		  if (activityTimerDeclarations != null)
		  {
			foreach (TimerDeclarationImpl timerDeclaration in activityTimerDeclarations)
			{
			  string definitionActivityId = TimerEventHandler.getActivityIdFromConfiguration(timerDeclaration.JobHandlerConfiguration);
			  if (timerDeclaration.JobHandlerType.Equals(jobHandlerType, StringComparison.CurrentCultureIgnoreCase) && (definitionActivityId.Equals(embededActivityId, StringComparison.CurrentCultureIgnoreCase)))
			  {
				return calculateMaxIterationsValue(timerDeclaration.Description.ExpressionText);
			  }
			}
		  }
		}

		// now check sub activities 
		foreach (ActivityImpl activity in activities)
		{
		  return checkBoundaryEventsDefinitions(activity.Activities, embededActivityId);
		}

		return 1;
	  }

	  protected internal virtual int calculateMaxIterationsValue(string originalExpression)
	  {
		int times = int.MaxValue;
		IList<string> expression = Arrays.asList(originalExpression.Split("/", true));
		if (expression.Count > 1 && expression[0].StartsWith("R"))
		{
		  times = int.MaxValue;
		  if (expression[0].Length > 1)
		  {
			times = Convert.ToInt32(expression[0].Substring(1));
		  }
		}
		return times;
	  }

	  protected internal virtual bool isValidTime(DateTime newTimer)
	  {
		BusinessCalendar businessCalendar = Context.ProcessEngineConfiguration.BusinessCalendarManager.getBusinessCalendar(getBusinessCalendarName(TimerEventHandler.geCalendarNameFromConfiguration(jobHandlerConfiguration)));
		return businessCalendar.validateDuedate(repeat, maxIterations, endDate, newTimer);
	  }

	  protected internal virtual int calculateRepeatValue()
	  {
		int times = -1;
		IList<string> expression = Arrays.asList(repeat.Split("/", true));
		if (expression.Count > 1 && expression[0].StartsWith("R") && expression[0].Length > 1)
		{
		  times = Convert.ToInt32(expression[0].Substring(1));
		  if (times > 0)
		  {
			times--;
		  }
		}
		return times;
	  }

	  protected internal virtual int NewRepeat
	  {
		  set
		  {
			IList<string> expression = Arrays.asList(repeat.Split("/", true));
			expression = expression.subList(1, expression.Count);
			StringBuilder repeatBuilder = new StringBuilder("R");
			repeatBuilder.Append(value);
			foreach (string value in expression)
			{
			  repeatBuilder.Append("/");
			  repeatBuilder.Append(value);
			}
			repeat = repeatBuilder.ToString();
		  }
	  }

	  protected internal virtual DateTime calculateNextTimer()
	  {
		BusinessCalendar businessCalendar = Context.ProcessEngineConfiguration.BusinessCalendarManager.getBusinessCalendar(getBusinessCalendarName(TimerEventHandler.geCalendarNameFromConfiguration(jobHandlerConfiguration)));
		return businessCalendar.resolveDuedate(repeat,maxIterations);
	  }

	  protected internal virtual string getBusinessCalendarName(string calendarName)
	  {
		string businessCalendarName = CycleBusinessCalendar.NAME;
		if (StringUtils.isNotEmpty(calendarName))
		{
		  VariableScope execution = NoExecutionVariableScope.SharedInstance;
		  if (StringUtils.isNotEmpty(this.executionId))
		  {
			execution = Context.CommandContext.ExecutionEntityManager.findExecutionById(this.executionId);
		  }
		  businessCalendarName = (string) Context.ProcessEngineConfiguration.ExpressionManager.createExpression(calendarName).getValue(execution);
		}
		return businessCalendarName;
	  }

	  public virtual string Repeat
	  {
		  get
		  {
			return repeat;
		  }
		  set
		  {
			this.repeat = value;
		  }
	  }


	  public virtual DateTime EndDate
	  {
		  get
		  {
			return endDate;
		  }
		  set
		  {
			this.endDate = value;
		  }
	  }

	}

}