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

namespace org.activiti.engine.impl.persistence.entity
{


	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using MultiInstanceActivityBehavior = org.activiti.engine.impl.bpmn.behavior.MultiInstanceActivityBehavior;
	using UserTaskActivityBehavior = org.activiti.engine.impl.bpmn.behavior.UserTaskActivityBehavior;
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using EventSubscriptionDeclaration = org.activiti.engine.impl.bpmn.parser.EventSubscriptionDeclaration;
	using Context = org.activiti.engine.impl.context.Context;
	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using HistoryManager = org.activiti.engine.impl.history.HistoryManager;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using AsyncContinuationJobHandler = org.activiti.engine.impl.jobexecutor.AsyncContinuationJobHandler;
	using TimerDeclarationImpl = org.activiti.engine.impl.jobexecutor.TimerDeclarationImpl;
	using PvmActivity = org.activiti.engine.impl.pvm.PvmActivity;
	using PvmException = org.activiti.engine.impl.pvm.PvmException;
	using PvmExecution = org.activiti.engine.impl.pvm.PvmExecution;
	using PvmProcessDefinition = org.activiti.engine.impl.pvm.PvmProcessDefinition;
	using PvmProcessElement = org.activiti.engine.impl.pvm.PvmProcessElement;
	using PvmProcessInstance = org.activiti.engine.impl.pvm.PvmProcessInstance;
	using PvmTransition = org.activiti.engine.impl.pvm.PvmTransition;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ExecutionListenerExecution = org.activiti.engine.impl.pvm.@delegate.ExecutionListenerExecution;
	using SignallableActivityBehavior = org.activiti.engine.impl.pvm.@delegate.SignallableActivityBehavior;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.activiti.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.activiti.engine.impl.pvm.process.TransitionImpl;
	using AtomicOperation = org.activiti.engine.impl.pvm.runtime.AtomicOperation;
	using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;
	using OutgoingExecution = org.activiti.engine.impl.pvm.runtime.OutgoingExecution;
	using StartingExecution = org.activiti.engine.impl.pvm.runtime.StartingExecution;
	using BitMaskUtil = org.activiti.engine.impl.util.BitMaskUtil;
	using Execution = org.activiti.engine.runtime.Execution;
	using Job = org.activiti.engine.runtime.Job;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// @author Saeid Mirzaei
	/// </summary>

	public class ExecutionEntity : VariableScopeImpl, ActivityExecution, ExecutionListenerExecution, Execution, PvmExecution, ProcessInstance, InterpretableExecution, PersistentObject, HasRevision
	{

	  private const long serialVersionUID = 1L;

	  private static Logger log = LoggerFactory.getLogger(typeof(ExecutionEntity));

	  // Persistent refrenced entities state //////////////////////////////////////
	  protected internal const int EVENT_SUBSCRIPTIONS_STATE_BIT = 1;
	  protected internal const int TASKS_STATE_BIT = 2;
	  protected internal const int JOBS_STATE_BIT = 3;

	  // current position /////////////////////////////////////////////////////////

	  protected internal ProcessDefinitionImpl processDefinition;

	  /// <summary>
	  /// current activity </summary>
	  protected internal ActivityImpl activity;

	  /// <summary>
	  /// current transition.  is null when there is no transition being taken. </summary>
	  protected internal TransitionImpl transition = null;

	  /// <summary>
	  /// transition that will be taken.  is null when there is no transition being taken. </summary>
	  protected internal TransitionImpl transitionBeingTaken = null;

	  /// <summary>
	  /// the process instance.  this is the root of the execution tree.  
	  /// the processInstance of a process instance is a self reference. 
	  /// </summary>
	  protected internal ExecutionEntity processInstance;

	  /// <summary>
	  /// the parent execution </summary>
	  protected internal ExecutionEntity parent;

	  /// <summary>
	  /// nested executions representing scopes or concurrent paths </summary>
	  protected internal IList<ExecutionEntity> executions;

	  /// <summary>
	  /// super execution, not-null if this execution is part of a subprocess </summary>
	  protected internal ExecutionEntity superExecution;

	  /// <summary>
	  /// reference to a subprocessinstance, not-null if currently subprocess is started from this execution </summary>
	  protected internal ExecutionEntity subProcessInstance;

	  protected internal StartingExecution startingExecution;

	  /// <summary>
	  /// The tenant identifier (if any) </summary>
	  protected internal string tenantId = ProcessEngineConfiguration.NO_TENANT_ID;
	  protected internal string name;
	  protected internal string description;
	  protected internal string localizedName;
	  protected internal string localizedDescription;

	  protected internal DateTime lockTime;

	  // state/type of execution ////////////////////////////////////////////////// 

	  /// <summary>
	  /// indicates if this execution represents an active path of execution.
	  /// Executions are made inactive in the following situations:
	  /// <ul>
	  ///   <li>an execution enters a nested scope</li>
	  ///   <li>an execution is split up into multiple concurrent executions, then the parent is made inactive.</li>
	  ///   <li>an execution has arrived in a parallel gateway or join and that join has not yet activated/fired.</li>
	  ///   <li>an execution is ended.</li>
	  /// </ul>
	  /// </summary>
	  protected internal bool isActive_Renamed = true;
	  protected internal bool isScope = true;
	  protected internal bool isConcurrent = false;
	  protected internal bool isEnded = false;
	  protected internal bool isEventScope = false;

	  // events ///////////////////////////////////////////////////////////////////

	  protected internal string eventName;
	  protected internal PvmProcessElement eventSource;
	  protected internal int executionListenerIndex = 0;

	  // associated entities /////////////////////////////////////////////////////

	  // (we cache associated entities here to minimize db queries) 
	  protected internal IList<EventSubscriptionEntity> eventSubscriptions;
	  protected internal IList<JobEntity> jobs;
	  protected internal IList<TaskEntity> tasks;
	  protected internal IList<IdentityLinkEntity> identityLinks;
	  protected internal int cachedEntityState;

	  // cascade deletion ////////////////////////////////////////////////////////

	  protected internal bool deleteRoot;
	  protected internal string deleteReason;

	  // replaced by //////////////////////////////////////////////////////////////

	  /// <summary>
	  /// when execution structure is pruned during a takeAll, then 
	  /// the original execution has to be resolved to the replaced execution. </summary>
	  /// <seealso cref= <seealso cref="#takeAll(List, List)"/> <seealso cref="OutgoingExecution"/>  </seealso>
	  protected internal ExecutionEntity replacedBy;

	  // atomic operations ////////////////////////////////////////////////////////

	  /// <summary>
	  /// next operation.  process execution is in fact runtime interpretation of the process model.
	  /// each operation is a logical unit of interpretation of the process.  so sequentially processing 
	  /// the operations drives the interpretation or execution of a process. </summary>
	  /// <seealso cref= AtomicOperation </seealso>
	  /// <seealso cref= #performOperation(AtomicOperation)  </seealso>
	  protected internal AtomicOperation nextOperation;
	  protected internal bool isOperating = false;

	  protected internal int revision = 1;
	  protected internal int suspensionState = SuspensionState_Fields.ACTIVE.StateCode;

	  /// <summary>
	  /// persisted reference to the processDefinition.
	  /// </summary>
	  /// <seealso cref= #processDefinition </seealso>
	  /// <seealso cref= #setProcessDefinition(ProcessDefinitionImpl) </seealso>
	  /// <seealso cref= #getProcessDefinition() </seealso>
	  protected internal string processDefinitionId;

	  /// <summary>
	  /// persisted reference to the process definition key.
	  /// </summary>
	  protected internal string processDefinitionKey;

	  /// <summary>
	  /// persisted reference to the process definition name.
	  /// </summary>
	  protected internal string processDefinitionName;

	  /// <summary>
	  /// persisted reference to the process definition version.
	  /// </summary>
	  protected internal int? processDefinitionVersion;

	  /// <summary>
	  /// persisted reference to the deployment id.
	  /// </summary>
	  protected internal string deploymentId;

	  /// <summary>
	  /// persisted reference to the current position in the diagram within the
	  /// <seealso cref="#processDefinition"/>.
	  /// </summary>
	  /// <seealso cref= #activity </seealso>
	  /// <seealso cref= #setActivity(ActivityImpl) </seealso>
	  /// <seealso cref= #getActivity() </seealso>
	  protected internal string activityId;

	  /// <summary>
	  /// The name of the current activity position
	  /// </summary>
	  protected internal string activityName;

	  /// <summary>
	  /// persisted reference to the process instance.
	  /// </summary>
	  /// <seealso cref= #getProcessInstance() </seealso>
	  protected internal string processInstanceId;

	  /// <summary>
	  /// persisted reference to the business key.
	  /// </summary>
	  protected internal string businessKey;

	  /// <summary>
	  /// persisted reference to the parent of this execution.
	  /// </summary>
	  /// <seealso cref= #getParent() </seealso>
	  /// <seealso cref= #setParentId(String) </seealso>
	  protected internal string parentId;

	  /// <summary>
	  /// persisted reference to the super execution of this execution
	  /// 
	  /// @See <seealso cref="#getSuperExecution()"/> </summary>
	  /// <seealso cref= #setSuperExecution(ExecutionEntity) </seealso>
	  protected internal string superExecutionId;

	  protected internal bool forcedUpdate;

	  protected internal IList<VariableInstanceEntity> queryVariables;

	  public ExecutionEntity(ActivityImpl activityImpl)
	  {
		this.startingExecution = new StartingExecution(activityImpl);
	  }

	  public ExecutionEntity()
	  {
	  }

	  /// <summary>
	  /// creates a new execution. properties processDefinition, processInstance and activity will be initialized. </summary>
	  public virtual ExecutionEntity createExecution()
	  {
		// create the new child execution
		ExecutionEntity createdExecution = newExecution();

		// manage the bidirectional parent-child relation
		ensureExecutionsInitialized();
		executions.Add(createdExecution);
		createdExecution.setParent(this);

		// initialize the new execution
		createdExecution.ProcessDefinition = ProcessDefinition;
		createdExecution.setProcessInstance(getProcessInstance());
		createdExecution.Activity = Activity;

		if (log.DebugEnabled)
		{
		  log.debug("Child execution {} created with parent ", createdExecution, this);
		}

		if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
		  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, createdExecution));
		  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_INITIALIZED, createdExecution));
		}

		return createdExecution;
	  }

	  public virtual PvmProcessInstance createSubProcessInstance(PvmProcessDefinition processDefinition)
	  {
		ExecutionEntity subProcessInstance = newExecution();

		// manage bidirectional super-subprocess relation
		subProcessInstance.setSuperExecution(this);
		this.setSubProcessInstance(subProcessInstance);

		// Initialize the new execution
		subProcessInstance.ProcessDefinition = (ProcessDefinitionImpl) processDefinition;
		subProcessInstance.setProcessInstance(subProcessInstance);

		// initialize the template-defined data objects as variables first
		IDictionary<string, object> dataObjectVars = ((ProcessDefinitionEntity) processDefinition).Variables;
		if (dataObjectVars != null)
		{
		  subProcessInstance.Variables = dataObjectVars;
		}

		Context.CommandContext.HistoryManager.recordSubProcessInstanceStart(this, subProcessInstance);

		if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
		  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, subProcessInstance));
		}
		return subProcessInstance;
	  }

	  protected internal virtual ExecutionEntity newExecution()
	  {
		ExecutionEntity newExecution = new ExecutionEntity();
		newExecution.executions = new List<ExecutionEntity>();

		// Inherit tenant id (if any)
		if (TenantId != null)
		{
			newExecution.TenantId = TenantId;
		}

		Context.CommandContext.DbSqlSession.insert(newExecution);

		return newExecution;
	  }


	  // scopes ///////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void initialize()
	  public virtual void initialize()
	  {
		log.debug("initializing {}", this);

		ScopeImpl scope = ScopeObject;
		ensureParentInitialized();

		// initialize the lists of referenced objects (prevents db queries)
		variableInstances = new Dictionary<string, VariableInstanceEntity>();
		eventSubscriptions = new List<EventSubscriptionEntity>();

		// Cached entity-state initialized to null, all bits are zero, indicating NO entities present
		cachedEntityState = 0;

		IList<TimerDeclarationImpl> timerDeclarations = (IList<TimerDeclarationImpl>) scope.getProperty(BpmnParse.PROPERTYNAME_TIMER_DECLARATION);
		if (timerDeclarations != null)
		{
		  foreach (TimerDeclarationImpl timerDeclaration in timerDeclarations)
		  {
			TimerEntity timer = timerDeclaration.prepareTimerEntity(this);
			if (timer != null)
			{
			  Context.CommandContext.JobEntityManager.schedule(timer);
			}
		  }
		}

		// create event subscriptions for the current scope
		IList<EventSubscriptionDeclaration> eventSubscriptionDeclarations = (IList<EventSubscriptionDeclaration>) scope.getProperty(BpmnParse.PROPERTYNAME_EVENT_SUBSCRIPTION_DECLARATION);
		if (eventSubscriptionDeclarations != null)
		{
		  foreach (EventSubscriptionDeclaration eventSubscriptionDeclaration in eventSubscriptionDeclarations)
		  {
			if (!eventSubscriptionDeclaration.StartEvent)
			{
			  EventSubscriptionEntity eventSubscriptionEntity = eventSubscriptionDeclaration.prepareEventSubscriptionEntity(this);
			  if (TenantId != null)
			  {
				  eventSubscriptionEntity.TenantId = TenantId;
			  }
			  eventSubscriptionEntity.insert();
			}
		  }
		}
	  }

	  public virtual void start()
	  {
		if (startingExecution == null && ProcessInstanceType)
		{
		  startingExecution = new StartingExecution(processDefinition.getInitial());
		}
		performOperation(org.activiti.engine.impl.pvm.runtime.AtomicOperation_Fields.PROCESS_START);
	  }

	  public virtual void destroy()
	  {
		log.debug("destroying {}", this);

		ensureParentInitialized();
		deleteVariablesInstanceForLeavingScope();

		Scope = false;
	  }

	  /// <summary>
	  /// removes an execution. if there are nested executions, those will be ended recursively.
	  /// if there is a parent, this method removes the bidirectional relation 
	  /// between parent and this execution. 
	  /// </summary>
	  public virtual void end()
	  {
		isActive_Renamed = false;
		isEnded = true;
		performOperation(org.activiti.engine.impl.pvm.runtime.AtomicOperation_Fields.ACTIVITY_END);
	  }


	  // methods that translate to operations /////////////////////////////////////

	  public virtual void signal(string signalName, object signalData)
	  {
		ensureActivityInitialized();
		SignallableActivityBehavior activityBehavior = (SignallableActivityBehavior) activity.ActivityBehavior;
		try
		{
			string signalledActivityId = activity.Id;
		  activityBehavior.signal(this, signalName, signalData);

		  // If needed, dispatch an event indicating an activity was signalled
		  bool isUserTask = (activityBehavior is UserTaskActivityBehavior) || ((activityBehavior is MultiInstanceActivityBehavior) && ((MultiInstanceActivityBehavior) activityBehavior).InnerActivityBehavior is UserTaskActivityBehavior);

		  if (!isUserTask && Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
			  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createSignalEvent(ActivitiEventType.ACTIVITY_SIGNALED, signalledActivityId, signalName, signalData, this.id, this.processInstanceId, this.processDefinitionId));
		  }

		}
		catch (Exception e)
		{
		  throw e;
		}
		catch (Exception e)
		{
		  throw new PvmException("couldn't process signal '" + signalName + "' on activity '" + activity.Id + "': " + e.Message, e);
		}
	  }

	  public virtual void take(PvmTransition transition)
	  {
		  take(transition, true);
	  }

	  /// <param name="fireActivityCompletionEvent"> This method can be called from other places
	  /// (like <seealso cref="#takeAll(List, List)"/>), where the event is already fired.
	  /// In that case, false is passed an no second event is fired. </param>
	  public virtual void take(PvmTransition transition, bool fireActivityCompletionEvent)
	  {

		  if (fireActivityCompletionEvent)
		  {
			  fireActivityCompletedEvent();
		  }

		if (this.transition != null)
		{
		  throw new PvmException("already taking a transition");
		}
		if (transition == null)
		{
		  throw new PvmException("transition is null");
		}
		Activity = (ActivityImpl)transition.Source;
		Transition = (TransitionImpl) transition;
		performOperation(org.activiti.engine.impl.pvm.runtime.AtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_END);
	  }

	  public virtual void executeActivity(PvmActivity activity)
	  {
		Activity = (ActivityImpl) activity;
		performOperation(org.activiti.engine.impl.pvm.runtime.AtomicOperation_Fields.ACTIVITY_START);
	  }

	  public virtual IList<ActivityExecution> findInactiveConcurrentExecutions(PvmActivity activity)
	  {
		IList<ActivityExecution> inactiveConcurrentExecutionsInActivity = new List<ActivityExecution>();
		IList<ActivityExecution> otherConcurrentExecutions = new List<ActivityExecution>();
		if (Concurrent)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List< ? extends org.activiti.engine.impl.pvm.delegate.ActivityExecution> concurrentExecutions = getParent().getAllChildExecutions();
		  IList<?> concurrentExecutions = getParent().AllChildExecutions;
		  foreach (ActivityExecution concurrentExecution in concurrentExecutions)
		  {
			if (concurrentExecution.Activity != null && concurrentExecution.Activity.Id.Equals(activity.Id))
			{
			  if (!concurrentExecution.Active)
			  {
				inactiveConcurrentExecutionsInActivity.Add(concurrentExecution);
			  }
			}
			else
			{
			  otherConcurrentExecutions.Add(concurrentExecution);
			}
		  }
		}
		else
		{
		  if (!Active)
		  {
			inactiveConcurrentExecutionsInActivity.Add(this);
		  }
		  else
		  {
			otherConcurrentExecutions.Add(this);
		  }
		}
		if (log.DebugEnabled)
		{
		  log.debug("inactive concurrent executions in '{}': {}", activity, inactiveConcurrentExecutionsInActivity);
		  log.debug("other concurrent executions: {}", otherConcurrentExecutions);
		}
		return inactiveConcurrentExecutionsInActivity;
	  }

	  protected internal virtual IList<ExecutionEntity> AllChildExecutions
	  {
		  get
		  {
			IList<ExecutionEntity> childExecutions = new List<ExecutionEntity>();
			foreach (ExecutionEntity childExecution in Executions)
			{
			  childExecutions.Add(childExecution);
			  childExecutions.AddRange(childExecution.AllChildExecutions);
			}
			return childExecutions;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) public void takeAll(java.util.List<org.activiti.engine.impl.pvm.PvmTransition> transitions, java.util.List<org.activiti.engine.impl.pvm.delegate.ActivityExecution> recyclableExecutions)
	  public virtual void takeAll(IList<PvmTransition> transitions, IList<ActivityExecution> recyclableExecutions)
	  {

		  fireActivityCompletedEvent();

		transitions = new List<PvmTransition>(transitions);
		recyclableExecutions = (recyclableExecutions != null ? new List<ActivityExecution>(recyclableExecutions) : new List<ActivityExecution>());

		if (recyclableExecutions.Count > 1)
		{
		  foreach (ActivityExecution recyclableExecution in recyclableExecutions)
		  {
			if (((ExecutionEntity)recyclableExecution).Scope)
			{
			  throw new PvmException("joining scope executions is not allowed");
			}
		  }
		}

		ExecutionEntity concurrentRoot = ((isConcurrent && !isScope) ? getParent() : this);
		IList<ExecutionEntity> concurrentActiveExecutions = new List<ExecutionEntity>();
		IList<ExecutionEntity> concurrentInActiveExecutions = new List<ExecutionEntity>();
		foreach (ExecutionEntity execution in concurrentRoot.Executions)
		{
		  if (execution.Active)
		  {
			concurrentActiveExecutions.Add(execution);
		  }
		  else
		  {
			concurrentInActiveExecutions.Add(execution);
		  }
		}

		if (log.DebugEnabled)
		{
		  log.debug("transitions to take concurrent: {}", transitions);
		  log.debug("active concurrent executions: {}", concurrentActiveExecutions);
		}

		if ((transitions.Count == 1) && (concurrentActiveExecutions.Count == 0) && allExecutionsInSameActivity(concurrentInActiveExecutions))
		{

		  IList<ExecutionEntity> recyclableExecutionImpls = (IList) recyclableExecutions;
		  recyclableExecutions.Remove(concurrentRoot);
		  foreach (ExecutionEntity prunedExecution in recyclableExecutionImpls)
		  {

			// End the pruned executions if necessary.
			// Some recyclable executions are inactivated (joined executions)
			// Others are already ended (end activities)

			// Need to call the activity end here. If we would do it later,
			// the executions are removed and the historic activity instances are
			// never ended as the combination of {activityId,executionId} is not valid anymor
			Context.CommandContext.HistoryManager.recordActivityEnd(prunedExecution);

			log.debug("pruning execution {}", prunedExecution);
			prunedExecution.remove();

		  }

		  log.debug("activating the concurrent root {} as the single path of execution going forward", concurrentRoot);
		  concurrentRoot.Active = true;
		  concurrentRoot.Activity = activity;
		  concurrentRoot.Concurrent = false;
		  concurrentRoot.take(transitions[0], false);

		}
		else
		{

		  IList<OutgoingExecution> outgoingExecutions = new List<OutgoingExecution>();

		  recyclableExecutions.Remove(concurrentRoot);

		  log.debug("recyclable executions for reuse: {}", recyclableExecutions);

		  // first create the concurrent executions
		  while (transitions.Count > 0)
		  {
			PvmTransition outgoingTransition = transitions.Remove(0);

			ExecutionEntity outgoingExecution = null;
			if (recyclableExecutions.Count == 0)
			{
			  outgoingExecution = concurrentRoot.createExecution();
			  log.debug("new {} with parent {} created to take transition {}", outgoingExecution, outgoingExecution.getParent(), outgoingTransition);
			}
			else
			{
			  outgoingExecution = (ExecutionEntity) recyclableExecutions.Remove(0);
			  log.debug("recycled {} to take transition {}", outgoingExecution, outgoingTransition);
			}

			outgoingExecution.Active = true;
			outgoingExecution.Scope = false;
			outgoingExecution.Concurrent = true;
			outgoingExecution.TransitionBeingTaken = (TransitionImpl) outgoingTransition;
			outgoingExecutions.Add(new OutgoingExecution(outgoingExecution, outgoingTransition, true));
		  }

		  // prune the executions that are not recycled 
		  foreach (ActivityExecution prunedExecution in recyclableExecutions)
		  {
			log.debug("pruning execution {}", prunedExecution);
			prunedExecution.end();
		  }

		  // then launch all the concurrent executions
		  foreach (OutgoingExecution outgoingExecution in outgoingExecutions)
		  {
			outgoingExecution.take(false);
		  }
		}
	  }

		protected internal virtual void fireActivityCompletedEvent()
		{
		  if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createActivityEvent(ActivitiEventType.ACTIVITY_COMPLETED, Activity != null ? Activity.Id : ActivityId, Activity != null ? (string) Activity.Properties["name"] : null, Id, ProcessInstanceId, ProcessDefinitionId, Activity != null ? (string) Activity.Properties["type"] : null, Activity != null ? Activity.ActivityBehavior.GetType().FullName : null));
		  }
		}

	  protected internal virtual bool allExecutionsInSameActivity(IList<ExecutionEntity> executions)
	  {
		if (executions.Count > 1)
		{
		  string activityId = executions[0].ActivityId;
		  foreach (ExecutionEntity execution in executions)
		  {
			string otherActivityId = execution.ActivityId;
			if (!execution.isEnded)
			{
			  if ((activityId == null && otherActivityId != null) || (activityId != null && otherActivityId == null) || (activityId != null && otherActivityId != null && !otherActivityId.Equals(activityId)))
			  {
				return false;
			  }
			}
		  }
		}
		return true;
	  }

	  public virtual void performOperation(AtomicOperation executionOperation)
	  {
		if (executionOperation.isAsync(this))
		{
		  scheduleAtomicOperationAsync(executionOperation);
		}
		else
		{
		  performOperationSync(executionOperation);
		}
	  }

	  protected internal virtual void performOperationSync(AtomicOperation executionOperation)
	  {
		Context.CommandContext.performOperation(executionOperation, this);
	  }

	  protected internal virtual void scheduleAtomicOperationAsync(AtomicOperation executionOperation)
	  {
		MessageEntity message = new MessageEntity();
		message.Execution = this;
		message.Exclusive = Activity.Exclusive;
		message.JobHandlerType = AsyncContinuationJobHandler.TYPE;
		// At the moment, only AtomicOperationTransitionCreateScope can be performed asynchronously,
		// so there is no need to pass it to the handler

		GregorianCalendar expireCal = new GregorianCalendar();
		ProcessEngineConfiguration processEngineConfig = Context.CommandContext.ProcessEngineConfiguration;
		expireCal.Time = processEngineConfig.Clock.CurrentTime;
		expireCal.add(DateTime.SECOND, processEngineConfig.LockTimeAsyncJobWaitTime);
		message.LockExpirationTime = expireCal.Time;

		// Inherit tenant id (if applicable)
		if (TenantId != null)
		{
			message.TenantId = TenantId;
		}

		Context.CommandContext.JobEntityManager.send(message);
	  }

	  public virtual bool isActive(string activityId)
	  {
		return findExecution(activityId) != null;
	  }

	  public virtual void inactivate()
	  {
		this.isActive_Renamed = false;
	  }

	  // executions ///////////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the non-null executions list </summary>
	  public virtual IList<ExecutionEntity> Executions
	  {
		  get
		  {
			ensureExecutionsInitialized();
			return executions;
		  }
		  set
		  {
			this.executions = value;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) protected void ensureExecutionsInitialized()
	  protected internal virtual void ensureExecutionsInitialized()
	  {
		if (executions == null)
		{
		  this.executions = (IList) Context.CommandContext.ExecutionEntityManager.findChildExecutionsByParentExecutionId(id);
		}
	  }


	  /// <summary>
	  /// searches for an execution positioned in the given activity </summary>
	  public virtual ExecutionEntity findExecution(string activityId)
	  {
		if ((Activity != null) && (Activity.Id.Equals(activityId)))
		{
		  return this;
		}
		foreach (ExecutionEntity nestedExecution in Executions)
		{
		  ExecutionEntity result = nestedExecution.findExecution(activityId);
		  if (result != null)
		  {
			return result;
		  }
		}
		return null;
	  }

	  public virtual IList<string> findActiveActivityIds()
	  {
		IList<string> activeActivityIds = new List<string>();
		collectActiveActivityIds(activeActivityIds);
		return activeActivityIds;
	  }

	  protected internal virtual void collectActiveActivityIds(IList<string> activeActivityIds)
	  {
		ensureActivityInitialized();
		if (isActive_Renamed && activity != null)
		{
		  activeActivityIds.Add(activity.Id);
		}
		ensureExecutionsInitialized();
		foreach (ExecutionEntity execution in executions)
		{
		  execution.collectActiveActivityIds(activeActivityIds);
		}
	  }


	  // bussiness key ////////////////////////////////////////////////////////////

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
		  set
		  {
			this.businessKey = value;
		  }
	  }


	  public virtual string ProcessBusinessKey
	  {
		  get
		  {
			return getProcessInstance().BusinessKey;
		  }
	  }

	  // process definition ///////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the process definition. </summary>
	  public virtual ProcessDefinitionImpl ProcessDefinition
	  {
		  get
		  {
			ensureProcessDefinitionInitialized();
			return processDefinition;
		  }
		  set
		  {
			this.processDefinition = value;
			this.processDefinitionId = value.Id;
			this.processDefinitionKey = value.Key;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  set
		  {
			this.processDefinitionId = value;
		  }
		  get
		  {
			return processDefinitionId;
		  }
	  }


	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }


	  public virtual string ProcessDefinitionName
	  {
		  get
		  {
			return processDefinitionName;
		  }
		  set
		  {
			this.processDefinitionName = value;
		  }
	  }


	  public virtual int? ProcessDefinitionVersion
	  {
		  get
		  {
			return processDefinitionVersion;
		  }
		  set
		  {
			this.processDefinitionVersion = value;
		  }
	  }


	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
		  }
	  }


	  /// <summary>
	  /// for setting the process definition, this setter must be used as subclasses can override </summary>
	  protected internal virtual void ensureProcessDefinitionInitialized()
	  {
		if ((processDefinition == null) && (processDefinitionId != null))
		{
		  ProcessDefinitionEntity deployedProcessDefinition = Context.ProcessEngineConfiguration.DeploymentManager.findDeployedProcessDefinitionById(processDefinitionId);
		  ProcessDefinition = deployedProcessDefinition;
		}
	  }


	  // process instance /////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the process instance. </summary>
	  public virtual ExecutionEntity getProcessInstance()
	  {
		ensureProcessInstanceInitialized();
		return processInstance;
	  }

	  protected internal virtual void ensureProcessInstanceInitialized()
	  {
		if ((processInstance == null) && (processInstanceId != null))
		{
		  processInstance = Context.CommandContext.ExecutionEntityManager.findExecutionById(processInstanceId);
		}
	  }

	  public virtual void setProcessInstance(InterpretableExecution processInstance)
	  {
		this.processInstance = (ExecutionEntity) processInstance;
		if (processInstance != null)
		{
		  this.processInstanceId = this.processInstance.Id;
		}
	  }

	  public virtual bool ProcessInstanceType
	  {
		  get
		  {
			return parentId == null;
		  }
	  }

	  // activity /////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the activity </summary>
	  public virtual ActivityImpl Activity
	  {
		  get
		  {
			ensureActivityInitialized();
			return activity;
		  }
		  set
		  {
			this.activity = value;
			if (value != null)
			{
			  this.activityId = value.Id;
			  this.activityName = (string) value.getProperty("name");
			}
			else
			{
			  this.activityId = null;
			  this.activityName = null;
			}
		  }
	  }

	  /// <summary>
	  /// must be called before the activity member field or getActivity() is called </summary>
	  protected internal virtual void ensureActivityInitialized()
	  {
		if ((activity == null) && (activityId != null))
		{
		  activity = ProcessDefinition.findActivity(activityId);
		}
	  }


	  // parent ///////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// ensures initialization and returns the parent </summary>
	  public virtual ExecutionEntity getParent()
	  {
		ensureParentInitialized();
		return parent;
	  }

	  protected internal virtual void ensureParentInitialized()
	  {
		if (parent == null && parentId != null)
		{
		  parent = Context.CommandContext.ExecutionEntityManager.findExecutionById(parentId);
		}
	  }

	  public virtual void setParent(InterpretableExecution parent)
	  {
		this.parent = (ExecutionEntity) parent;

		if (parent != null)
		{
		  this.parentId = ((ExecutionEntity)parent).Id;
		}
		else
		{
		  this.parentId = null;
		}
	  }

	  // super- and subprocess executions /////////////////////////////////////////

	  public virtual string SuperExecutionId
	  {
		  get
		  {
			return superExecutionId;
		  }
	  }

	  public virtual ExecutionEntity getSuperExecution()
	  {
		ensureSuperExecutionInitialized();
		return superExecution;
	  }

	  public virtual void setSuperExecution(ExecutionEntity superExecution)
	  {
		this.superExecution = superExecution;
		if (superExecution != null)
		{
		  superExecution.setSubProcessInstance(null);
		}

		if (superExecution != null)
		{
		  this.superExecutionId = ((ExecutionEntity)superExecution).Id;
		}
		else
		{
		  this.superExecutionId = null;
		}
	  }

	  protected internal virtual void ensureSuperExecutionInitialized()
	  {
		if (superExecution == null && superExecutionId != null)
		{
		  superExecution = Context.CommandContext.ExecutionEntityManager.findExecutionById(superExecutionId);
		}
	  }

	  public virtual ExecutionEntity getSubProcessInstance()
	  {
		ensureSubProcessInstanceInitialized();
		return subProcessInstance;
	  }

	  public virtual void setSubProcessInstance(InterpretableExecution subProcessInstance)
	  {
		this.subProcessInstance = (ExecutionEntity) subProcessInstance;
	  }

	  protected internal virtual void ensureSubProcessInstanceInitialized()
	  {
		if (subProcessInstance == null)
		{
		  subProcessInstance = Context.CommandContext.ExecutionEntityManager.findSubProcessInstanceBySuperExecutionId(id);
		}
	  }

	  // scopes ///////////////////////////////////////////////////////////////////

	  protected internal virtual ScopeImpl ScopeObject
	  {
		  get
		  {
			ScopeImpl scope = null;
			if (ProcessInstanceType)
			{
			  scope = ProcessDefinition;
			}
			else
			{
			  scope = Activity;
			}
			return scope;
		  }
	  }

	  public virtual bool Scope
	  {
		  get
		  {
			return isScope;
		  }
		  set
		  {
			this.isScope = value;
		  }
	  }


	  // customized persistence behaviour /////////////////////////////////////////

	  public virtual void remove()
	  {
		ensureParentInitialized();
		if (parent != null)
		{
		  parent.ensureExecutionsInitialized();
		  parent.executions.Remove(this);
		}

		// delete all the variable instances
		ensureVariableInstancesInitialized();
		deleteVariablesInstanceForLeavingScope();

		// delete all the tasks
		removeTasks(null);

		// remove all jobs
		removeJobs();

		// remove all event subscriptions for this scope, if the scope has event subscriptions:
		removeEventSubscriptions();

		// remove event scopes:            
		removeEventScopes();

		// remove identity links
		removeIdentityLinks();

		if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_DELETED, this));
		}

		// finally delete this execution
		Context.CommandContext.DbSqlSession.delete(this);
	  }

	  public virtual void destroyScope(string reason)
	  {

		if (log.DebugEnabled)
		{
		  log.debug("performing destroy scope behavior for execution {}", this);
		}

		// remove all child executions and sub process instances:
		HistoryManager historyManager = Context.CommandContext.HistoryManager;
		IList<InterpretableExecution> executions = new List<InterpretableExecution>(Executions);
		foreach (InterpretableExecution childExecution in executions)
		{
		  if (childExecution.getSubProcessInstance() != null)
		  {
			childExecution.getSubProcessInstance().deleteCascade(reason);
		  }
		  historyManager.recordActivityEnd((ExecutionEntity) childExecution);
		  childExecution.deleteCascade(reason);
		}

		if (activityId != null)
		{
		  historyManager.recordActivityEnd(this);
		}

		removeTasks(reason);
		removeJobs();
	  }

	  private void removeEventScopes()
	  {
		IList<InterpretableExecution> childExecutions = new List<InterpretableExecution>(Executions);
		foreach (InterpretableExecution childExecution in childExecutions)
		{
		  if (childExecution.EventScope)
		  {
			log.debug("removing eventScope {}", childExecution);
			childExecution.destroy();
			childExecution.remove();
		  }
		}
	  }

	  private void removeEventSubscriptions()
	  {
		foreach (EventSubscriptionEntity eventSubscription in EventSubscriptions)
		{
		  eventSubscription.delete();
		}
	  }

	  private void removeJobs()
	  {
		foreach (Job job in Jobs)
		{
		  ((JobEntity) job).delete();
		}
	  }

	  private void removeTasks(string reason)
	  {
		if (reason == null)
		{
		  reason = TaskEntity.DELETE_REASON_DELETED;
		}
		foreach (TaskEntity task in Tasks)
		{
		  if (replacedBy != null)
		  {
			if (task.getExecution() == null || task.getExecution() != replacedBy)
			{
			  // All tasks should have been moved when "replacedBy" has been set. Just in case tasks where added,
			  // wo do an additional check here and move it
			  task.setExecution(replacedBy);
			  this.replacedBy.addTask(task);
			}
		  }
		  else
		  {
			Context.CommandContext.TaskEntityManager.deleteTask(task, reason, false);
		  }
		}
	  }

	  public virtual ExecutionEntity getReplacedBy()
	  {
		return replacedBy;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) public void setReplacedBy(org.activiti.engine.impl.pvm.runtime.InterpretableExecution replacedBy)
	  public virtual void setReplacedBy(InterpretableExecution replacedBy)
	  {
		this.replacedBy = (ExecutionEntity) replacedBy;

		CommandContext commandContext = Context.CommandContext;
		DbSqlSession dbSqlSession = commandContext.DbSqlSession;

		// update the related tasks

		IList<TaskEntity> allTasks = new List<TaskEntity>();
		allTasks.AddRange(Tasks);

		IList<TaskEntity> cachedTasks = dbSqlSession.findInCache(typeof(TaskEntity));
		foreach (TaskEntity cachedTask in cachedTasks)
		{
			if (cachedTask.ExecutionId.Equals(this.Id))
			{
				allTasks.Add(cachedTask);
			}
		}

		foreach (TaskEntity task in allTasks)
		{
		  task.ExecutionId = replacedBy.Id;
		  task.setExecution(this.replacedBy);

		  // update the related local task variables
		  IList<VariableInstanceEntity> variables = (IList) commandContext.VariableInstanceEntityManager.findVariableInstancesByTaskId(task.Id);

		  foreach (VariableInstanceEntity variable in variables)
		  {
			variable.Execution = this.replacedBy;
		  }

		  this.replacedBy.addTask(task);
		}

		// All tasks have been moved to 'replacedBy', safe to clear the list 
		this.tasks.Clear();

		tasks = dbSqlSession.findInCache(typeof(TaskEntity));
		foreach (TaskEntity task in tasks)
		{
		  if (id.Equals(task.ExecutionId))
		  {
			task.ExecutionId = replacedBy.Id;
		  }
		}

		// update the related jobs
		IList<JobEntity> jobs = Jobs;
		foreach (JobEntity job in jobs)
		{
		  job.Execution = (ExecutionEntity) replacedBy;
		}

		// update the related event subscriptions
		IList<EventSubscriptionEntity> eventSubscriptions = EventSubscriptions;
		foreach (EventSubscriptionEntity subscriptionEntity in eventSubscriptions)
		{
		  subscriptionEntity.setExecution((ExecutionEntity) replacedBy);
		}

		// update the related process variables
		IList<VariableInstanceEntity> variables = (IList) commandContext.VariableInstanceEntityManager.findVariableInstancesByExecutionId(id);

		foreach (VariableInstanceEntity variable in variables)
		{
		  variable.ExecutionId = replacedBy.Id;
		}
		variables = dbSqlSession.findInCache(typeof(VariableInstanceEntity));
		foreach (VariableInstanceEntity variable in variables)
		{
		  if (id.Equals(variable.ExecutionId))
		  {
			variable.ExecutionId = replacedBy.Id;
		  }
		}

		commandContext.HistoryManager.recordExecutionReplacedBy(this, replacedBy);
	  }

	  // variables ////////////////////////////////////////////////////////////////

	  protected internal override void initializeVariableInstanceBackPointer(VariableInstanceEntity variableInstance)
	  {
		variableInstance.ProcessInstanceId = processInstanceId;
		variableInstance.ExecutionId = id;
	  }

	  protected internal override IList<VariableInstanceEntity> loadVariableInstances()
	  {
		return Context.CommandContext.VariableInstanceEntityManager.findVariableInstancesByExecutionId(id);
	  }

	  protected internal override VariableScopeImpl ParentVariableScope
	  {
		  get
		  {
			return getParent();
		  }
	  }

	  /// <summary>
	  /// used to calculate the sourceActivityExecution for method <seealso cref="#updateActivityInstanceIdInHistoricVariableUpdate(HistoricDetailVariableInstanceUpdateEntity, ExecutionEntity)"/> </summary>
	  protected internal override ExecutionEntity SourceActivityExecution
	  {
		  get
		  {
			return (activityId != null ? this : null);
		  }
	  }

	  protected internal override bool ActivityIdUsedForDetails
	  {
		  get
		  {
			return true;
		  }
	  }

	  protected internal override VariableInstanceEntity createVariableInstance(string variableName, object value, ExecutionEntity sourceActivityExecution)
	  {
		VariableInstanceEntity result = base.createVariableInstance(variableName, value, sourceActivityExecution);

		// Dispatch event, if needed
		if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createVariableEvent(ActivitiEventType.VARIABLE_CREATED, variableName, value, result.Type, result.TaskId, result.ExecutionId, ProcessInstanceId, ProcessDefinitionId));
		}
		return result;
	  }

	  protected internal override void updateVariableInstance(VariableInstanceEntity variableInstance, object value, ExecutionEntity sourceActivityExecution)
	  {
		base.updateVariableInstance(variableInstance, value, sourceActivityExecution);

		// Dispatch event, if needed
		if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createVariableEvent(ActivitiEventType.VARIABLE_UPDATED, variableInstance.Name, value, variableInstance.Type, variableInstance.TaskId, variableInstance.ExecutionId, ProcessInstanceId, ProcessDefinitionId));
		}
	  }

	  protected internal override VariableInstanceEntity getSpecificVariable(string variableName)
	  {

		  CommandContext commandContext = Context.CommandContext;
		if (commandContext == null)
		{
		  throw new ActivitiException("lazy loading outside command context");
		}
		VariableInstanceEntity variableInstance = commandContext.VariableInstanceEntityManager.findVariableInstanceByExecutionAndName(id, variableName);

		return variableInstance;
	  }

	  protected internal override IList<VariableInstanceEntity> getSpecificVariables(ICollection<string> variableNames)
	  {
		  CommandContext commandContext = Context.CommandContext;
		if (commandContext == null)
		{
		  throw new ActivitiException("lazy loading outside command context");
		}
		return commandContext.VariableInstanceEntityManager.findVariableInstancesByExecutionAndNames(id, variableNames);
	  }

	  // persistent state /////////////////////////////////////////////////////////

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["processDefinitionId"] = this.processDefinitionId;
			persistentState["businessKey"] = this.businessKey;
			persistentState["activityId"] = this.activityId;
			persistentState["isActive"] = this.isActive_Renamed;
			persistentState["isConcurrent"] = this.isConcurrent;
			persistentState["isScope"] = this.isScope;
			persistentState["isEventScope"] = this.isEventScope;
			persistentState["parentId"] = parentId;
			persistentState["name"] = name;
			persistentState["lockTime"] = lockTime;
			persistentState["superExecution"] = this.superExecutionId;
			if (forcedUpdate)
			{
			  persistentState["forcedUpdate"] = true;
			}
			persistentState["suspensionState"] = this.suspensionState;
			persistentState["cachedEntityState"] = this.cachedEntityState;
			return persistentState;
		  }
	  }

	  public virtual void insert()
	  {
		Context.CommandContext.DbSqlSession.insert(this);
	  }

	  public virtual void deleteCascade(string deleteReason)
	  {
		this.deleteReason = deleteReason;
		this.deleteRoot = true;
		performOperation(org.activiti.engine.impl.pvm.runtime.AtomicOperation_Fields.DELETE_CASCADE);
	  }

	  public virtual bool DeleteRoot
	  {
		  set
		  {
			  this.deleteRoot = value;
		  }
		  get
		  {
			return deleteRoot;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public virtual void forceUpdate()
	  {
		this.forcedUpdate = true;
	  }

	  // process engine convience access /////////////////////////////////////////////////////////////////

	  public virtual EngineServices EngineServices
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration;
		  }
	  }

	  // toString /////////////////////////////////////////////////////////////////

	  public override string ToString()
	  {
		if (ProcessInstanceType)
		{
		  return "ProcessInstance[" + ToStringIdentity + "]";
		}
		else
		{
		  return (isConcurrent? "Concurrent" : "") + (isScope ? "Scope" : "") + "Execution[" + ToStringIdentity + "]";
		}
	  }

	  protected internal virtual string ToStringIdentity
	  {
		  get
		  {
			return id;
		  }
	  }

	  // event subscription support //////////////////////////////////////////////

	  public virtual IList<EventSubscriptionEntity> EventSubscriptionsInternal
	  {
		  get
		  {
			ensureEventSubscriptionsInitialized();
			return eventSubscriptions;
		  }
	  }

	  public virtual IList<EventSubscriptionEntity> EventSubscriptions
	  {
		  get
		  {
			return new List<EventSubscriptionEntity>(EventSubscriptionsInternal);
		  }
	  }

	  public virtual IList<CompensateEventSubscriptionEntity> CompensateEventSubscriptions
	  {
		  get
		  {
			IList<EventSubscriptionEntity> eventSubscriptions = EventSubscriptionsInternal;
			IList<CompensateEventSubscriptionEntity> result = new List<CompensateEventSubscriptionEntity>(eventSubscriptions.Count);
			foreach (EventSubscriptionEntity eventSubscriptionEntity in eventSubscriptions)
			{
			  if (eventSubscriptionEntity is CompensateEventSubscriptionEntity)
			  {
				result.Add((CompensateEventSubscriptionEntity) eventSubscriptionEntity);
			  }
			}
			return result;
		  }
	  }

	  public virtual IList<CompensateEventSubscriptionEntity> getCompensateEventSubscriptions(string activityId)
	  {
		IList<EventSubscriptionEntity> eventSubscriptions = EventSubscriptionsInternal;
		IList<CompensateEventSubscriptionEntity> result = new List<CompensateEventSubscriptionEntity>(eventSubscriptions.Count);
		foreach (EventSubscriptionEntity eventSubscriptionEntity in eventSubscriptions)
		{
		  if (eventSubscriptionEntity is CompensateEventSubscriptionEntity)
		  {
			if (activityId.Equals(eventSubscriptionEntity.ActivityId))
			{
			  result.Add((CompensateEventSubscriptionEntity) eventSubscriptionEntity);
			}
		  }
		}
		return result;
	  }

	  protected internal virtual void ensureEventSubscriptionsInitialized()
	  {
		if (eventSubscriptions == null)
		{
		  eventSubscriptions = Context.CommandContext.EventSubscriptionEntityManager.findEventSubscriptionsByExecution(id);
		}
	  }

	  public virtual void addEventSubscription(EventSubscriptionEntity eventSubscriptionEntity)
	  {
		EventSubscriptionsInternal.Add(eventSubscriptionEntity);

	  }

	  public virtual void removeEventSubscription(EventSubscriptionEntity eventSubscriptionEntity)
	  {
		EventSubscriptionsInternal.Remove(eventSubscriptionEntity);
	  }

	  // referenced job entities //////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) protected void ensureJobsInitialized()
	  protected internal virtual void ensureJobsInitialized()
	  {
		if (jobs == null)
		{
		  jobs = (IList)Context.CommandContext.JobEntityManager.findJobsByExecutionId(id);
		}
	  }

	  protected internal virtual IList<JobEntity> JobsInternal
	  {
		  get
		  {
			ensureJobsInitialized();
			return jobs;
		  }
	  }

	  public virtual IList<JobEntity> Jobs
	  {
		  get
		  {
			return new List<JobEntity>(JobsInternal);
		  }
	  }

	  public virtual void addJob(JobEntity jobEntity)
	  {
		JobsInternal.Add(jobEntity);
	  }

	  public virtual void removeJob(JobEntity job)
	  {
		JobsInternal.Remove(job);
	  }

	  // referenced task entities ///////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) protected void ensureTasksInitialized()
	  protected internal virtual void ensureTasksInitialized()
	  {
		if (tasks == null)
		{
		  tasks = (IList)Context.CommandContext.TaskEntityManager.findTasksByExecutionId(id);
		}
	  }

	  protected internal virtual IList<TaskEntity> TasksInternal
	  {
		  get
		  {
			ensureTasksInitialized();
			return tasks;
		  }
	  }

	  public virtual IList<TaskEntity> Tasks
	  {
		  get
		  {
			return new List<TaskEntity>(TasksInternal);
		  }
	  }

	  public virtual void addTask(TaskEntity taskEntity)
	  {
		TasksInternal.Add(taskEntity);
	  }

	  public virtual void removeTask(TaskEntity task)
	  {
		TasksInternal.Remove(task);
	  }

	  // identity links ///////////////////////////////////////////////////////////

	  public virtual IList<IdentityLinkEntity> IdentityLinks
	  {
		  get
		  {
			if (identityLinks == null)
			{
			  identityLinks = Context.CommandContext.IdentityLinkEntityManager.findIdentityLinksByProcessInstanceId(id);
			}
    
			return identityLinks;
		  }
	  }

	  public virtual IdentityLinkEntity addIdentityLink(string userId, string groupId, string type)
	  {
		IdentityLinkEntity identityLinkEntity = new IdentityLinkEntity();
		IdentityLinks.Add(identityLinkEntity);
		identityLinkEntity.setProcessInstance(this);
		identityLinkEntity.UserId = userId;
		identityLinkEntity.GroupId = groupId;
		identityLinkEntity.Type = type;
		identityLinkEntity.insert();
		return identityLinkEntity;
	  }

	  /// <summary>
	  /// Adds an IdentityLink for this user with the specified type, 
	  /// but only if the user is not associated with this instance yet.
	  /// 
	  /// </summary>
	  public virtual IdentityLinkEntity involveUser(string userId, string type)
	  {
		foreach (IdentityLinkEntity identityLink in IdentityLinks)
		{
		  if (identityLink.User && identityLink.UserId.Equals(userId))
		  {
			return identityLink;
		  }
		}
		return addIdentityLink(userId, null, type);
	  }

	  public virtual void removeIdentityLinks()
	  {
		Context.CommandContext.IdentityLinkEntityManager.deleteIdentityLinksByProcInstance(id);
	  }

	  // getters and setters //////////////////////////////////////////////////////


	  public virtual int CachedEntityState
	  {
		  set
		  {
			this.cachedEntityState = value;
    
			// Check for flags that are down. These lists can be safely initialized as empty, preventing
			// additional queries that end up in an empty list anyway
			if (jobs == null && !BitMaskUtil.isBitOn(value, JOBS_STATE_BIT))
			{
			  jobs = new List<JobEntity>();
			}
			if (tasks == null && !BitMaskUtil.isBitOn(value, TASKS_STATE_BIT))
			{
			  tasks = new List<TaskEntity>();
			}
			if (eventSubscriptions == null && !BitMaskUtil.isBitOn(value, EVENT_SUBSCRIPTIONS_STATE_BIT))
			{
			  eventSubscriptions = new List<EventSubscriptionEntity>();
			}
		  }
		  get
		  {
			cachedEntityState = 0;
    
			// Only mark a flag as false when the list is not-null and empty. If null, we can't be sure there are no entries in it since
			// the list hasn't been initialized/queried yet.
			cachedEntityState = BitMaskUtil.setBit(cachedEntityState, TASKS_STATE_BIT, (tasks == null || tasks.Count > 0));
			cachedEntityState = BitMaskUtil.setBit(cachedEntityState, EVENT_SUBSCRIPTIONS_STATE_BIT, (eventSubscriptions == null || eventSubscriptions.Count > 0));
			cachedEntityState = BitMaskUtil.setBit(cachedEntityState, JOBS_STATE_BIT, (jobs == null || jobs.Count > 0));
    
			return cachedEntityState;
		  }
	  }


	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }
	  public virtual string ParentId
	  {
		  get
		  {
			return parentId;
		  }
		  set
		  {
			this.parentId = value;
		  }
	  }
	  public override string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }
	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }
	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public virtual TransitionImpl Transition
	  {
		  get
		  {
			return transition;
		  }
		  set
		  {
			this.transition = value;
			if (replacedBy != null)
			{
				replacedBy.Transition = value;
			}
		  }
	  }
	  public virtual TransitionImpl TransitionBeingTaken
	  {
		  get
		  {
			return transitionBeingTaken;
		  }
		  set
		  {
			this.transitionBeingTaken = value;
			if (replacedBy != null)
			{
				replacedBy.TransitionBeingTaken = value;
			}
		  }
	  }
	  public virtual int? ExecutionListenerIndex
	  {
		  get
		  {
			return executionListenerIndex;
		  }
		  set
		  {
			this.executionListenerIndex = value;
		  }
	  }
	  public virtual bool Concurrent
	  {
		  get
		  {
			return isConcurrent;
		  }
		  set
		  {
			this.isConcurrent = value;
		  }
	  }
	  public virtual bool Active
	  {
		  get
		  {
			return isActive_Renamed;
		  }
		  set
		  {
			this.isActive_Renamed = value;
		  }
	  }
	  public virtual bool Ended
	  {
		  get
		  {
			return isEnded;
		  }
		  set
		  {
			  this.isEnded = value;
		  }
	  }
	  public virtual string EventName
	  {
		  get
		  {
			return eventName;
		  }
		  set
		  {
			this.eventName = value;
		  }
	  }
	  public virtual PvmProcessElement EventSource
	  {
		  get
		  {
			return eventSource;
		  }
		  set
		  {
			this.eventSource = value;
		  }
	  }
	  public virtual string DeleteReason
	  {
		  get
		  {
			return deleteReason;
		  }
		  set
		  {
			this.deleteReason = value;
		  }
	  }

	  public virtual int SuspensionState
	  {
		  get
		  {
			return suspensionState;
		  }
		  set
		  {
			this.suspensionState = value;
		  }
	  }


	  public virtual bool Suspended
	  {
		  get
		  {
			return suspensionState == SuspensionState_Fields.SUSPENDED.StateCode;
		  }
	  }

	  public virtual bool EventScope
	  {
		  get
		  {
			return isEventScope;
		  }
		  set
		  {
			this.isEventScope = value;
		  }
	  }


	  public virtual StartingExecution StartingExecution
	  {
		  get
		  {
			return startingExecution;
		  }
	  }

	  public virtual void disposeStartingExecution()
	  {
		startingExecution = null;
	  }

	  public virtual string CurrentActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public virtual string CurrentActivityName
	  {
		  get
		  {
			return activityName;
		  }
	  }

	  public override string Name
	  {
		  get
		  {
			if (localizedName != null && localizedName.Length > 0)
			{
			  return localizedName;
			}
			else
			{
			  return name;
			}
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual string Description
	  {
		  get
		  {
			if (localizedDescription != null && localizedDescription.Length > 0)
			{
			  return localizedDescription;
			}
			else
			{
			  return description;
			}
		  }
		  set
		  {
			this.description = value;
		  }
	  }


	  public virtual string LocalizedName
	  {
		  get
		  {
			return localizedName;
		  }
		  set
		  {
			this.localizedName = value;
		  }
	  }


	  public virtual string LocalizedDescription
	  {
		  get
		  {
			return localizedDescription;
		  }
		  set
		  {
			this.localizedDescription = value;
		  }
	  }


	  public virtual string TenantId
	  {
		  get
		  {
				return tenantId;
		  }
		  set
		  {
				this.tenantId = value;
		  }
	  }


		public virtual DateTime LockTime
		{
			get
			{
			return lockTime;
			}
			set
			{
			this.lockTime = value;
			}
		}


	  public virtual IDictionary<string, object> ProcessVariables
	  {
		  get
		  {
			IDictionary<string, object> variables = new Dictionary<string, object>();
			if (queryVariables != null)
			{
			  foreach (VariableInstanceEntity variableInstance in queryVariables)
			  {
				if (variableInstance.Id != null && variableInstance.TaskId == null)
				{
				  variables[variableInstance.Name] = variableInstance.Value;
				}
			  }
			}
			return variables;
		  }
	  }

	  public virtual IList<VariableInstanceEntity> QueryVariables
	  {
		  get
		  {
			if (queryVariables == null && Context.CommandContext != null)
			{
			  queryVariables = new VariableInitializingList();
			}
			return queryVariables;
		  }
		  set
		  {
			this.queryVariables = value;
		  }
	  }


	  public virtual string updateProcessBusinessKey(string bzKey)
	  {
		if (ProcessInstanceType && bzKey != null)
		{
		  BusinessKey = bzKey;
		  Context.CommandContext.HistoryManager.updateProcessBusinessKeyInHistory(this);

		  if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
			  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, this));
		  }

		  return bzKey;
		}
		return null;
	  }

	  public virtual void deleteIdentityLink(string userId, string groupId, string type)
	  {
		IList<IdentityLinkEntity> identityLinks = Context.CommandContext.IdentityLinkEntityManager.findIdentityLinkByProcessInstanceUserGroupAndType(id, userId, groupId, type);

		foreach (IdentityLinkEntity identityLink in identityLinks)
		{
		  Context.CommandContext.IdentityLinkEntityManager.deleteIdentityLink(identityLink, true);
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		IdentityLinks.removeAll(identityLinks);

	  }

	}

}