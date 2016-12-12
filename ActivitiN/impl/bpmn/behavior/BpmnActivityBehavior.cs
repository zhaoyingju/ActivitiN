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

namespace org.activiti.engine.impl.bpmn.behavior
{


	using Expression = org.activiti.engine.@delegate.Expression;
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using SkipExpressionUtil = org.activiti.engine.impl.bpmn.helper.SkipExpressionUtil;
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using Context = org.activiti.engine.impl.context.Context;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using PvmTransition = org.activiti.engine.impl.pvm.PvmTransition;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// Helper class for implementing BPMN 2.0 activities, offering convenience
	/// methods specific to BPMN 2.0.
	/// 
	/// This class can be used by inheritance or aggregation.
	/// 
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class BpmnActivityBehavior
	{

	  private const long serialVersionUID = 1L;

	  private static Logger log = LoggerFactory.getLogger(typeof(BpmnActivityBehavior));

	  /// <summary>
	  /// Performs the default outgoing BPMN 2.0 behavior, which is having parallel
	  /// paths of executions for the outgoing sequence flow.
	  /// 
	  /// More precisely: every sequence flow that has a condition which evaluates to
	  /// true (or which doesn't have a condition), is selected for continuation of
	  /// the process instance. If multiple sequencer flow are selected, multiple,
	  /// parallel paths of executions are created.
	  /// </summary>
	  public virtual void performDefaultOutgoingBehavior(ActivityExecution activityExecution)
	  {
		ActivityImpl activity = (ActivityImpl) activityExecution.Activity;
		if (!(activity.ActivityBehavior is IntermediateCatchEventActivityBehavior))
		{
		  dispatchJobCanceledEvents(activityExecution);
		}
		performOutgoingBehavior(activityExecution, true, false, null);
	  }

	  /// <summary>
	  /// Performs the default outgoing BPMN 2.0 behavior (@see
	  /// <seealso cref="#performDefaultOutgoingBehavior(ActivityExecution)"/>), but without
	  /// checking the conditions on the outgoing sequence flow.
	  /// 
	  /// This means that every outgoing sequence flow is selected for continuing the
	  /// process instance, regardless of having a condition or not. In case of
	  /// multiple outgoing sequence flow, multiple parallel paths of executions will
	  /// be created.
	  /// </summary>
	  public virtual void performIgnoreConditionsOutgoingBehavior(ActivityExecution activityExecution)
	  {
		performOutgoingBehavior(activityExecution, false, false, null);
	  }

	  /// <summary>
	  /// dispatch job canceled event for job associated with given execution entity </summary>
	  /// <param name="activityExecution"> </param>
	  protected internal virtual void dispatchJobCanceledEvents(ActivityExecution activityExecution)
	  {
		if (activityExecution is ExecutionEntity)
		{
		  IList<JobEntity> jobs = ((ExecutionEntity) activityExecution).Jobs;
		  foreach (JobEntity job in jobs)
		  {
			if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
			{
			  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.JOB_CANCELED, job));
			}
		  }
		}
	  }

	  /// <summary>
	  /// Actual implementation of leaving an activity.
	  /// </summary>
	  /// <param name="execution">
	  ///          The current execution context </param>
	  /// <param name="checkConditions">
	  ///          Whether or not to check conditions before determining whether or
	  ///          not to take a transition. </param>
	  /// <param name="throwExceptionIfExecutionStuck">
	  ///          If true, an <seealso cref="ActivitiException"/> will be thrown in case no
	  ///          transition could be found to leave the activity. </param>
	  protected internal virtual void performOutgoingBehavior(ActivityExecution execution, bool checkConditions, bool throwExceptionIfExecutionStuck, IList<ActivityExecution> reusableExecutions)
	  {

		if (log.DebugEnabled)
		{
		  log.debug("Leaving activity '{}'", execution.Activity.Id);
		}

		string defaultSequenceFlow = (string) execution.Activity.getProperty("default");
		IList<PvmTransition> transitionsToTake = new List<PvmTransition>();

		IList<PvmTransition> outgoingTransitions = execution.Activity.OutgoingTransitions;
		foreach (PvmTransition outgoingTransition in outgoingTransitions)
		{
		  Expression skipExpression = outgoingTransition.SkipExpression;

		  if (!SkipExpressionUtil.isSkipExpressionEnabled(execution, skipExpression))
		  {
			if (defaultSequenceFlow == null || !outgoingTransition.Id.Equals(defaultSequenceFlow))
			{
			  Condition condition = (Condition) outgoingTransition.getProperty(BpmnParse.PROPERTYNAME_CONDITION);
			  if (condition == null || !checkConditions || condition.evaluate(outgoingTransition.Id, execution))
			  {
				transitionsToTake.Add(outgoingTransition);
			  }
			}

		  }
		  else if (SkipExpressionUtil.shouldSkipFlowElement(execution, skipExpression))
		  {
			transitionsToTake.Add(outgoingTransition);
		  }
		}

		if (transitionsToTake.Count == 1)
		{

		  execution.take(transitionsToTake[0]);

		}
		else if (transitionsToTake.Count >= 1)
		{

		  execution.inactivate();
		  if (reusableExecutions == null || reusableExecutions.Count == 0)
		  {
			execution.takeAll(transitionsToTake, Arrays.asList(execution));
		  }
		  else
		  {
			execution.takeAll(transitionsToTake, reusableExecutions);
		  }

		}
		else
		{

		  if (defaultSequenceFlow != null)
		  {
			PvmTransition defaultTransition = execution.Activity.findOutgoingTransition(defaultSequenceFlow);
			if (defaultTransition != null)
			{
			  execution.take(defaultTransition);
			}
			else
			{
			  throw new ActivitiException("Default sequence flow '" + defaultSequenceFlow + "' could not be not found");
			}
		  }
		  else
		  {

			object isForCompensation = execution.Activity.getProperty(BpmnParse.PROPERTYNAME_IS_FOR_COMPENSATION);
			if (isForCompensation != null && (bool?) isForCompensation)
			{
			  if (execution is ExecutionEntity)
			  {
				Context.CommandContext.HistoryManager.recordActivityEnd((ExecutionEntity) execution);
			  }
			  InterpretableExecution parentExecution = (InterpretableExecution) execution.Parent;
			  ((InterpretableExecution)execution).remove();
			  parentExecution.signal("compensationDone", null);

			}
			else
			{

			  if (log.DebugEnabled)
			  {
				log.debug("No outgoing sequence flow found for {}. Ending execution.", execution.Activity.Id);
			  }
			  execution.end();

			  if (throwExceptionIfExecutionStuck)
			  {
				throw new ActivitiException("No outgoing sequence flow of the inclusive gateway '" + execution.Activity.Id + "' could be selected for continuing the process");
			  }
			}

		  }
		}
	  }

	}

}