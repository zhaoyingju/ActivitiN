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
	using VariableScope = org.activiti.engine.@delegate.VariableScope;
	using BusinessCalendar = org.activiti.engine.impl.calendar.BusinessCalendar;
	using Context = org.activiti.engine.impl.context.Context;
	using NoExecutionVariableScope = org.activiti.engine.impl.el.NoExecutionVariableScope;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using TimerEntity = org.activiti.engine.impl.persistence.entity.TimerEntity;
	using DateTime = org.joda.time.DateTime;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class TimerDeclarationImpl
	{

	  private const long serialVersionUID = 1L;

	  protected internal Expression description;
	  protected internal TimerDeclarationType type;
	  protected internal Expression endDateExpression;
	  protected internal Expression calendarNameExpression;

	  protected internal string jobHandlerType;
	  protected internal string jobHandlerConfiguration = null;
	  protected internal string repeat;
	  protected internal bool exclusive = TimerEntity.DEFAULT_EXCLUSIVE;
	  protected internal int retries = TimerEntity.DEFAULT_RETRIES;
	  protected internal bool isInterruptingTimer; // For boundary timers

	  public TimerDeclarationImpl(Expression expression, TimerDeclarationType type, string jobHandlerType, Expression endDateExpression, Expression calendarNameExpression) : this(expression,type,jobHandlerType)
	  {
		this.endDateExpression = endDateExpression;
		this.calendarNameExpression = calendarNameExpression;
	  }

	  public TimerDeclarationImpl(Expression expression, TimerDeclarationType type, string jobHandlerType)
	  {
		this.jobHandlerType = jobHandlerType;
		this.description = expression;
		this.type = type;
	  }

	  public virtual Expression Description
	  {
		  get
		  {
			return description;
		  }
	  }
	  public virtual string JobHandlerType
	  {
		  get
		  {
			return jobHandlerType;
		  }
		  set
		  {
			this.jobHandlerType = value;
		  }
	  }

	  public virtual string JobHandlerConfiguration
	  {
		  get
		  {
			return jobHandlerConfiguration;
		  }
		  set
		  {
			this.jobHandlerConfiguration = value;
		  }
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


	  public virtual bool Exclusive
	  {
		  get
		  {
			return exclusive;
		  }
		  set
		  {
			this.exclusive = value;
		  }
	  }


	  public virtual int Retries
	  {
		  get
		  {
			return retries;
		  }
		  set
		  {
			this.retries = value;
		  }
	  }



	  public virtual bool InterruptingTimer
	  {
		  get
		  {
			return isInterruptingTimer;
		  }
		  set
		  {
			this.isInterruptingTimer = value;
		  }
	  }


	  public virtual TimerEntity prepareTimerEntity(ExecutionEntity executionEntity)
	  {
		// ACT-1415: timer-declaration on start-event may contain expressions NOT
		// evaluating variables but other context, evaluating should happen nevertheless
		VariableScope scopeForExpression = executionEntity;
		if (scopeForExpression == null)
		{
		  scopeForExpression = NoExecutionVariableScope.SharedInstance;
		}

		string calendarNameValue = type.calendarName;
		if (this.calendarNameExpression != null)
		{
		  calendarNameValue = (string) this.calendarNameExpression.getValue(scopeForExpression);
		}

		BusinessCalendar businessCalendar = Context.ProcessEngineConfiguration.BusinessCalendarManager.getBusinessCalendar(calendarNameValue);

		if (description == null)
		{
		  // Prevent NPE from happening in the next line
		  throw new ActivitiIllegalArgumentException("Timer '" + executionEntity.ActivityId + "' was not configured with a valid duration/time");
		}

		string endDateString = null;
		string dueDateString = null;
		DateTime duedate = null;
		DateTime endDate = null;

		if (endDateExpression != null && !(scopeForExpression is NoExecutionVariableScope))
		{
		  object endDateValue = endDateExpression.getValue(scopeForExpression);
		  if (endDateValue is string)
		  {
			endDateString = (string) endDateValue;
		  }
		  else if (endDateValue is DateTime)
		  {
			endDate = (DateTime) endDateValue;
		  }
		  else if (endDateValue is DateTime)
		  {
			// Joda DateTime support
			duedate = ((DateTime) endDateValue).toDate();
		  }
		  else
		  {
			throw new ActivitiException("Timer '" + executionEntity.ActivityId + "' was not configured with a valid duration/time, either hand in a java.util.Date or a String in format 'yyyy-MM-dd'T'hh:mm:ss'");
		  }

		  if (endDate == null)
		  {
			endDate = businessCalendar.resolveEndDate(endDateString);
		  }
		}

		object dueDateValue = description.getValue(scopeForExpression);
		if (dueDateValue is string)
		{
		  dueDateString = (string) dueDateValue;
		}
		else if (dueDateValue is DateTime)
		{
		  duedate = (DateTime)dueDateValue;
		}
		else if (dueDateValue is DateTime)
		{
		  // Joda DateTime support
		  duedate = ((DateTime) dueDateValue).toDate();
		}
		else if (dueDateValue != null)
		{
		  //dueDateValue==null is OK - but unexpected class type must throw an error.
		  throw new ActivitiException("Timer '" + executionEntity.ActivityId + "' was not configured with a valid duration/time, either hand in a java.util.Date or a String in format 'yyyy-MM-dd'T'hh:mm:ss'");
		}

		if (duedate == null && dueDateString != null)
		{
		  duedate = businessCalendar.resolveDuedate(dueDateString);
		}

		TimerEntity timer = null;
		// if dueDateValue is null -> this is OK - timer will be null and job not scheduled
		   if (duedate != null)
		   {
			   timer = new TimerEntity(this);
			   timer.Duedate = duedate;
			   timer.EndDate = endDate;

			   if (executionEntity != null)
			   {
				 timer.Execution = executionEntity;
				 timer.ProcessDefinitionId = executionEntity.ProcessDefinitionId;
				 timer.ProcessInstanceId = executionEntity.ProcessInstanceId;

				 // Inherit tenant identifier (if applicable)
				 if (executionEntity != null && executionEntity.TenantId != null)
				 {
				   timer.TenantId = executionEntity.TenantId;
				 }
			   }

		  if (type == TimerDeclarationType.CYCLE)
		  {

			  // See ACT-1427: A boundary timer with a cancelActivity='true', doesn't need to repeat itself
			  bool repeat = !isInterruptingTimer;

			  // ACT-1951: intermediate catching timer events shouldn't repeat according to spec
			  if (TimerCatchIntermediateEventJobHandler.TYPE.Equals(jobHandlerType))
			  {
				  repeat = false;
			  if (endDate != null)
			  {
				long endDateMiliss = endDate.Ticks;
				long dueDateMiliss = duedate.Ticks;
				long dueDate = Math.Min(endDateMiliss,dueDateMiliss);
				timer.Duedate = new DateTime(dueDate);
			  }
			  }

			if (repeat)
			{
			  string prepared = prepareRepeat(dueDateString);
			  timer.Repeat = prepared;
			}
		  }
		   }
		return timer;
	  }

	  private string prepareRepeat(string dueDate)
	  {
		if (dueDate.StartsWith("R") && dueDate.Split("/", true).length == 2)
		{
		  SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		  return dueDate.Replace("/","/" + sdf.format(Context.ProcessEngineConfiguration.Clock.CurrentTime) + "/");
		}
		return dueDate;
	  }
	}

}