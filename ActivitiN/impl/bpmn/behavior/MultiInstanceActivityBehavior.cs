using System;
using System.Collections;
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


	using BpmnError = org.activiti.engine.@delegate.BpmnError;
	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;
	using Expression = org.activiti.engine.@delegate.Expression;
	using ErrorPropagation = org.activiti.engine.impl.bpmn.helper.ErrorPropagation;
	using ScopeUtil = org.activiti.engine.impl.bpmn.helper.ScopeUtil;
	using Context = org.activiti.engine.impl.context.Context;
	using ExecutionListenerInvocation = org.activiti.engine.impl.@delegate.ExecutionListenerInvocation;
	using ActivityInstanceStartHandler = org.activiti.engine.impl.history.handler.ActivityInstanceStartHandler;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using CompositeActivityBehavior = org.activiti.engine.impl.pvm.@delegate.CompositeActivityBehavior;
	using SubProcessActivityBehavior = org.activiti.engine.impl.pvm.@delegate.SubProcessActivityBehavior;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using AtomicOperation = org.activiti.engine.impl.pvm.runtime.AtomicOperation;
	using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// Implementation of the multi-instance functionality as described in the BPMN 2.0 spec.
	/// 
	/// Multi instance functionality is implemented as an <seealso cref="ActivityBehavior"/> that
	/// wraps the original <seealso cref="ActivityBehavior"/> of the activity.
	/// 
	/// Only subclasses of <seealso cref="AbstractBpmnActivityBehavior"/> can have multi-instance
	/// behavior. As such, special logic is contained in the <seealso cref="AbstractBpmnActivityBehavior"/>
	/// to delegate to the <seealso cref="MultiInstanceActivityBehavior"/> if needed.
	/// 
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
	[Serializable]
	public abstract class MultiInstanceActivityBehavior : FlowNodeActivityBehavior, CompositeActivityBehavior, SubProcessActivityBehavior
	{

	  protected internal static readonly Logger LOGGER = LoggerFactory.getLogger(typeof(MultiInstanceActivityBehavior));

	  // Variable names for outer instance(as described in spec)
	  protected internal readonly string NUMBER_OF_INSTANCES = "nrOfInstances";
	  protected internal readonly string NUMBER_OF_ACTIVE_INSTANCES = "nrOfActiveInstances";
	  protected internal readonly string NUMBER_OF_COMPLETED_INSTANCES = "nrOfCompletedInstances";

	  // Instance members
	  protected internal ActivityImpl activity;
	  protected internal AbstractBpmnActivityBehavior innerActivityBehavior;
	  protected internal Expression loopCardinalityExpression;
	  protected internal Expression completionConditionExpression;
	  protected internal Expression collectionExpression;
	  protected internal string collectionVariable;
	  protected internal string collectionElementVariable;
	  // default variable name for loop counter for inner instances (as described in the spec)
	  protected internal string collectionElementIndexVariable = "loopCounter";

	  /// <param name="innerActivityBehavior"> The original <seealso cref="ActivityBehavior"/> of the activity 
	  ///                         that will be wrapped inside this behavior. </param>
	  /// <param name="isSequential"> Indicates whether the multi instance behavior
	  ///                     must be sequential or parallel </param>
	  public MultiInstanceActivityBehavior(ActivityImpl activity, AbstractBpmnActivityBehavior innerActivityBehavior)
	  {
		this.activity = activity;
		InnerActivityBehavior = innerActivityBehavior;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		if (getLocalLoopVariable(execution, CollectionElementIndexVariable) == null)
		{
		  try
		  {
			createInstances(execution);
		  }
		  catch (BpmnError error)
		  {
			ErrorPropagation.propagateError(error, execution);
		  }

		  if (resolveNrOfInstances(execution) == 0)
		  {
			leave(execution);
		  }
		}
		else
		{
			innerActivityBehavior.execute(execution);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void createInstances(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception;
	  protected internal abstract void createInstances(ActivityExecution execution);

	  // Intercepts signals, and delegates it to the wrapped {@link ActivityBehavior}.
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void signal(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		innerActivityBehavior.signal(execution, signalName, signalData);
	  }

	  // required for supporting embedded subprocesses
	  public virtual void lastExecutionEnded(ActivityExecution execution)
	  {
		ScopeUtil.createEventScopeExecution((ExecutionEntity) execution);
		leave(execution);
	  }

	  // required for supporting external subprocesses
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void completing(org.activiti.engine.delegate.DelegateExecution execution, org.activiti.engine.delegate.DelegateExecution subProcessInstance) throws Exception
	  public virtual void completing(DelegateExecution execution, DelegateExecution subProcessInstance)
	  {
	  }

	  // required for supporting external subprocesses
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void completed(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void completed(ActivityExecution execution)
	  {
		leave(execution);
	  }

	  // Helpers //////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") protected int resolveNrOfInstances(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution)
	  protected internal virtual int resolveNrOfInstances(ActivityExecution execution)
	  {
		int nrOfInstances = -1;
		if (loopCardinalityExpression != null)
		{
		  nrOfInstances = resolveLoopCardinality(execution);
		}
		else if (collectionExpression != null)
		{
		  object obj = collectionExpression.getValue(execution);
		  if (!(obj is ICollection))
		  {
			throw new ActivitiIllegalArgumentException(collectionExpression.ExpressionText + "' didn't resolve to a Collection");
		  }
		  nrOfInstances = ((ICollection) obj).Count;
		}
		else if (collectionVariable != null)
		{
		  object obj = execution.getVariable(collectionVariable);
		  if (obj == null)
		  {
			throw new ActivitiIllegalArgumentException("Variable " + collectionVariable + " is not found");
		  }
		  if (!(obj is ICollection))
		  {
			throw new ActivitiIllegalArgumentException("Variable " + collectionVariable + "' is not a Collection");
		  }
		  nrOfInstances = ((ICollection) obj).Count;
		}
		else
		{
		  throw new ActivitiIllegalArgumentException("Couldn't resolve collection expression nor variable reference");
		}
		return nrOfInstances;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") protected void executeOriginalBehavior(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution, int loopCounter) throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  protected internal virtual void executeOriginalBehavior(ActivityExecution execution, int loopCounter)
	  {
		if (usesCollection() && collectionElementVariable != null)
		{
		  ICollection collection = null;
		  if (collectionExpression != null)
		  {
			collection = (ICollection) collectionExpression.getValue(execution);
		  }
		  else if (collectionVariable != null)
		  {
			collection = (ICollection) execution.getVariable(collectionVariable);
		  }

		  object value = null;
		  int index = 0;
		  IEnumerator it = collection.GetEnumerator();
		  while (index <= loopCounter)
		  {
			value = it.next();
			index++;
		  }
		  setLoopVariable(execution, collectionElementVariable, value);
		}

		// If loopcounter == 1, then historic activity instance already created, no need to
		// pass through executeActivity again since it will create a new historic activity
		if (loopCounter == 0)
		{
			callCustomActivityStartListeners(execution);
		  innerActivityBehavior.execute(execution);
		}
		else
		{
		  execution.executeActivity(activity);
		}
	  }

	  protected internal virtual bool usesCollection()
	  {
		return collectionExpression != null || collectionVariable != null;
	  }

	  protected internal virtual bool ExtraScopeNeeded
	  {
		  get
		  {
			// special care is needed when the behavior is an embedded subprocess (not very clean, but it works)
			return innerActivityBehavior is org.activiti.engine.impl.bpmn.behavior.SubProcessActivityBehavior;
		  }
	  }

	  protected internal virtual int resolveLoopCardinality(ActivityExecution execution)
	  {
		// Using Number since expr can evaluate to eg. Long (which is also the default for Juel)
		object value = loopCardinalityExpression.getValue(execution);
		if (value is Number)
		{
		  return (int)((Number) value);
		}
		else if (value is string)
		{
		  return Convert.ToInt32((string) value);
		}
		else
		{
		  throw new ActivitiIllegalArgumentException("Could not resolve loopCardinality expression '" + loopCardinalityExpression.ExpressionText + "': not a number nor number String");
		}
	  }

	  protected internal virtual bool completionConditionSatisfied(ActivityExecution execution)
	  {
		if (completionConditionExpression != null)
		{
		  object value = completionConditionExpression.getValue(execution);
		  if (!(value is bool?))
		  {
			throw new ActivitiIllegalArgumentException("completionCondition '" + completionConditionExpression.ExpressionText + "' does not evaluate to a boolean value");
		  }
		  bool? booleanValue = (bool?) value;
		  if (LOGGER.DebugEnabled)
		  {
			LOGGER.debug("Completion condition of multi-instance satisfied: {}", booleanValue);
		  }
		  return booleanValue;
		}
		return false;
	  }

	  protected internal virtual void setLoopVariable(ActivityExecution execution, string variableName, object value)
	  {
		execution.setVariableLocal(variableName, value);
	  }

	  protected internal virtual int? getLoopVariable(ActivityExecution execution, string variableName)
	  {
		object value = execution.getVariableLocal(variableName);
		ActivityExecution parent = execution.Parent;
		while (value == null && parent != null)
		{
		  value = parent.getVariableLocal(variableName);
		  parent = parent.Parent;
		}
		return (int?)(value != null ? value : 0);
	  }

	  protected internal virtual int? getLocalLoopVariable(ActivityExecution execution, string variableName)
	  {
		return (int?) execution.getVariableLocal(variableName);
	  }

	  /// <summary>
	  /// Since the first loop of the multi instance is not executed as a regular activity,
	  /// it is needed to call the start listeners yourself.
	  /// </summary>
	  protected internal virtual void callCustomActivityStartListeners(ActivityExecution execution)
	  {
		IList<ExecutionListener> listeners = activity.getExecutionListeners(org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_START);

		IList<ExecutionListener> filteredExecutionListeners = new List<ExecutionListener>(listeners.Count);
		if (listeners != null)
		{
			// Sad that we have to do this, but it's the only way I could find (which is also safe for backwards compatibility)

			foreach (ExecutionListener executionListener in listeners)
			{
				if (!(executionListener is ActivityInstanceStartHandler))
				{
					filteredExecutionListeners.Add(executionListener);
				}
			}

			CallActivityListenersOperation atomicOperation = new CallActivityListenersOperation(filteredExecutionListeners);
			Context.CommandContext.performOperation(atomicOperation, (InterpretableExecution)execution);
		}

	  }

	  /// <summary>
	  /// Since no transitions are followed when leaving the inner activity,
	  /// it is needed to call the end listeners yourself.
	  /// </summary>
	  protected internal virtual void callActivityEndListeners(ActivityExecution execution)
	  {
		IList<ExecutionListener> listeners = activity.getExecutionListeners(org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_END);
		CallActivityListenersOperation atomicOperation = new CallActivityListenersOperation(listeners);
		Context.CommandContext.performOperation(atomicOperation, (InterpretableExecution)execution);
	  }

	  protected internal virtual void logLoopDetails(ActivityExecution execution, string custom, int loopCounter, int nrOfCompletedInstances, int nrOfActiveInstances, int nrOfInstances)
	  {
		if (LOGGER.DebugEnabled)
		{
		  LOGGER.debug("Multi-instance '{}' {}. Details: loopCounter={}, nrOrCompletedInstances={},nrOfActiveInstances={},nrOfInstances={}", execution.Activity, custom, loopCounter, nrOfCompletedInstances, nrOfActiveInstances, nrOfInstances);
		}
	  }


	  // Getters and Setters ///////////////////////////////////////////////////////////

	  public virtual Expression LoopCardinalityExpression
	  {
		  get
		  {
			return loopCardinalityExpression;
		  }
		  set
		  {
			this.loopCardinalityExpression = value;
		  }
	  }
	  public virtual Expression CompletionConditionExpression
	  {
		  get
		  {
			return completionConditionExpression;
		  }
		  set
		  {
			this.completionConditionExpression = value;
		  }
	  }
	  public virtual Expression CollectionExpression
	  {
		  get
		  {
			return collectionExpression;
		  }
		  set
		  {
			this.collectionExpression = value;
		  }
	  }
	  public virtual string CollectionVariable
	  {
		  get
		  {
			return collectionVariable;
		  }
		  set
		  {
			this.collectionVariable = value;
		  }
	  }
	  public virtual string CollectionElementVariable
	  {
		  get
		  {
			return collectionElementVariable;
		  }
		  set
		  {
			this.collectionElementVariable = value;
		  }
	  }
	  public virtual string CollectionElementIndexVariable
	  {
		  get
		  {
			return collectionElementIndexVariable;
		  }
		  set
		  {
			this.collectionElementIndexVariable = value;
		  }
	  }
	  public virtual AbstractBpmnActivityBehavior InnerActivityBehavior
	  {
		  set
		  {
			this.innerActivityBehavior = value;
			this.innerActivityBehavior.setMultiInstanceActivityBehavior(this);
		  }
		  get
		  {
			  return innerActivityBehavior;
		  }
	  }

	  /// <summary>
	  /// ACT-1339. Calling ActivityEndListeners within an <seealso cref="AtomicOperation"/> 
	  /// so that an executionContext is present.
	  /// 
	  /// @author Aris Tzoumas
	  /// @author Joram Barrez
	  /// 
	  /// </summary>
	  private sealed class CallActivityListenersOperation : AtomicOperation
	  {

		internal IList<ExecutionListener> listeners;

		internal CallActivityListenersOperation(IList<ExecutionListener> listeners)
		{
			this.listeners = listeners;
		}

		public override void execute(InterpretableExecution execution)
		{
			foreach (ExecutionListener executionListener in listeners)
			{
		  try
		  {
			Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new ExecutionListenerInvocation(executionListener, execution));
		  }
		  catch (Exception e)
		  {
			throw new ActivitiException("Couldn't execute listener", e);
		  }
			}
		}

		public override bool isAsync(InterpretableExecution execution)
		{
			return false;
		}

	  }
	}

}