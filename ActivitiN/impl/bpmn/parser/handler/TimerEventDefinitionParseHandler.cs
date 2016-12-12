using System;
using System.Collections.Generic;

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


	using BaseElement = org.activiti.bpmn.model.BaseElement;
	using BoundaryEvent = org.activiti.bpmn.model.BoundaryEvent;
	using IntermediateCatchEvent = org.activiti.bpmn.model.IntermediateCatchEvent;
	using StartEvent = org.activiti.bpmn.model.StartEvent;
	using TimerEventDefinition = org.activiti.bpmn.model.TimerEventDefinition;
	using Expression = org.activiti.engine.@delegate.Expression;
	using Context = org.activiti.engine.impl.context.Context;
	using ExpressionManager = org.activiti.engine.impl.el.ExpressionManager;
	using JobHandler = org.activiti.engine.impl.jobexecutor.JobHandler;
	using TimerCatchIntermediateEventJobHandler = org.activiti.engine.impl.jobexecutor.TimerCatchIntermediateEventJobHandler;
	using TimerDeclarationImpl = org.activiti.engine.impl.jobexecutor.TimerDeclarationImpl;
	using TimerDeclarationType = org.activiti.engine.impl.jobexecutor.TimerDeclarationType;
	using TimerEventHandler = org.activiti.engine.impl.jobexecutor.TimerEventHandler;
	using TimerExecuteNestedActivityJobHandler = org.activiti.engine.impl.jobexecutor.TimerExecuteNestedActivityJobHandler;
	using TimerStartEventJobHandler = org.activiti.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
	using StringUtils = org.apache.commons.lang3.StringUtils;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class TimerEventDefinitionParseHandler : AbstractBpmnParseHandler<TimerEventDefinition>
	{

		private static readonly Logger logger = LoggerFactory.getLogger(typeof(TimerEventDefinitionParseHandler));

	  public const string PROPERTYNAME_START_TIMER = "timerStart";

	  public virtual Type HandledType
	  {
		  get
		  {
			return typeof(TimerEventDefinition);
		  }
	  }

	  protected internal virtual void executeParse(BpmnParse bpmnParse, TimerEventDefinition timerEventDefinition)
	  {

		ActivityImpl timerActivity = bpmnParse.CurrentActivity;
		if (bpmnParse.CurrentFlowElement is StartEvent)
		{

		  ProcessDefinitionEntity processDefinition = bpmnParse.CurrentProcessDefinition;
		  timerActivity.setProperty("type", "startTimerEvent");
		  TimerDeclarationImpl timerDeclaration = createTimer(bpmnParse, timerEventDefinition, timerActivity, TimerStartEventJobHandler.TYPE);

		  string jobHandlerConfiguration = timerDeclaration.JobHandlerConfiguration;
		  IDictionary<string, JobHandler> jobHandlers = Context.ProcessEngineConfiguration.JobHandlers;
		  JobHandler jobHandler = jobHandlers[TimerStartEventJobHandler.TYPE];
		  jobHandlerConfiguration = ((TimerEventHandler) jobHandler).setProcessDefinitionKeyToConfiguration(jobHandlerConfiguration, processDefinition.Key);
		  jobHandlerConfiguration = ((TimerEventHandler) jobHandler).setActivityIdToConfiguration(jobHandlerConfiguration, timerActivity.Id);
		  timerDeclaration.JobHandlerConfiguration = jobHandlerConfiguration;


		  IList<TimerDeclarationImpl> timerDeclarations = (IList<TimerDeclarationImpl>) processDefinition.getProperty(PROPERTYNAME_START_TIMER);
		  if (timerDeclarations == null)
		  {
			timerDeclarations = new List<TimerDeclarationImpl>();
			processDefinition.setProperty(PROPERTYNAME_START_TIMER, timerDeclarations);
		  }
		  timerDeclarations.Add(timerDeclaration);

		}
		else if (bpmnParse.CurrentFlowElement is IntermediateCatchEvent)
		{

		  timerActivity.setProperty("type", "intermediateTimer");
		  TimerDeclarationImpl timerDeclaration = createTimer(bpmnParse, timerEventDefinition, timerActivity, TimerCatchIntermediateEventJobHandler.TYPE);
		  if (getPrecedingEventBasedGateway(bpmnParse, (IntermediateCatchEvent) bpmnParse.CurrentFlowElement) != null)
		  {
			addTimerDeclaration(timerActivity.Parent, timerDeclaration);
		  }
		  else
		  {
			addTimerDeclaration(timerActivity, timerDeclaration);
			timerActivity.Scope = true;
		  }

		}
		else if (bpmnParse.CurrentFlowElement is BoundaryEvent)
		{

		  timerActivity.setProperty("type", "boundaryTimer");
		  TimerDeclarationImpl timerDeclaration = createTimer(bpmnParse, timerEventDefinition, timerActivity, TimerExecuteNestedActivityJobHandler.TYPE);

		  // ACT-1427
		  BoundaryEvent boundaryEvent = (BoundaryEvent) bpmnParse.CurrentFlowElement;
		  bool interrupting = boundaryEvent.CancelActivity;
		  if (interrupting)
		  {
			timerDeclaration.InterruptingTimer = true;
		  }

		  addTimerDeclaration(timerActivity.Parent, timerDeclaration);

		  if (timerActivity.Parent is ActivityImpl)
		  {
			((ActivityImpl) timerActivity.Parent).Scope = true;
		  }

		  timerActivity.ActivityBehavior = bpmnParse.ActivityBehaviorFactory.createBoundaryEventActivityBehavior((BoundaryEvent) bpmnParse.CurrentFlowElement, interrupting, timerActivity);

		}
	  }

	  protected internal virtual TimerDeclarationImpl createTimer(BpmnParse bpmnParse, TimerEventDefinition timerEventDefinition, ScopeImpl timerActivity, string jobHandlerType)
	  {
		TimerDeclarationType type = null;
		Expression expression = null;
		Expression endDate = null;
		Expression calendarName = null;
		ExpressionManager expressionManager = bpmnParse.ExpressionManager;
		if (StringUtils.isNotEmpty(timerEventDefinition.TimeDate))
		{
		  // TimeDate
		  type = TimerDeclarationType.DATE;
		  expression = expressionManager.createExpression(timerEventDefinition.TimeDate);
		}
		else if (StringUtils.isNotEmpty(timerEventDefinition.TimeCycle))
		{
		  // TimeCycle
		  type = TimerDeclarationType.CYCLE;
		  expression = expressionManager.createExpression(timerEventDefinition.TimeCycle);
		  //support for endDate
		  if (StringUtils.isNotEmpty(timerEventDefinition.EndDate))
		  {
			endDate = expressionManager.createExpression(timerEventDefinition.EndDate);
		  }
		}
		else if (StringUtils.isNotEmpty(timerEventDefinition.TimeDuration))
		{
		  // TimeDuration
		  type = TimerDeclarationType.DURATION;
		  expression = expressionManager.createExpression(timerEventDefinition.TimeDuration);
		}
		if (StringUtils.isNotEmpty(timerEventDefinition.CalendarName))
		{
		  calendarName = expressionManager.createExpression(timerEventDefinition.CalendarName);
		}

		// neither date, cycle or duration configured!
		if (expression == null)
		{
		  logger.warn("Timer needs configuration (either timeDate, timeCycle or timeDuration is needed) (" + timerActivity.Id + ")");
		}


		string jobHandlerConfiguration = timerActivity.Id;

		if (jobHandlerType.Equals(TimerExecuteNestedActivityJobHandler.TYPE, StringComparison.CurrentCultureIgnoreCase) || jobHandlerType.Equals(TimerCatchIntermediateEventJobHandler.TYPE, StringComparison.CurrentCultureIgnoreCase) || jobHandlerType.Equals(TimerStartEventJobHandler.TYPE, StringComparison.CurrentCultureIgnoreCase))
		{
		  jobHandlerConfiguration = TimerStartEventJobHandler.createConfiguration(timerActivity.Id, endDate, calendarName);
		}

		// Parse the timer declaration
		// TODO move the timer declaration into the bpmn activity or next to the
		// TimerSession
		TimerDeclarationImpl timerDeclaration = new TimerDeclarationImpl(expression, type, jobHandlerType, endDate, calendarName);
		timerDeclaration.JobHandlerConfiguration = jobHandlerConfiguration;

		timerDeclaration.Exclusive = true;
		return timerDeclaration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void addTimerDeclaration(org.activiti.engine.impl.pvm.process.ScopeImpl scope, org.activiti.engine.impl.jobexecutor.TimerDeclarationImpl timerDeclaration)
	  protected internal virtual void addTimerDeclaration(ScopeImpl scope, TimerDeclarationImpl timerDeclaration)
	  {
		IList<TimerDeclarationImpl> timerDeclarations = (IList<TimerDeclarationImpl>) scope.getProperty(PROPERTYNAME_TIMER_DECLARATION);
		if (timerDeclarations == null)
		{
		  timerDeclarations = new List<TimerDeclarationImpl>();
		  scope.setProperty(PROPERTYNAME_TIMER_DECLARATION, timerDeclarations);
		}
		timerDeclarations.Add(timerDeclaration);
	  }

	}

}