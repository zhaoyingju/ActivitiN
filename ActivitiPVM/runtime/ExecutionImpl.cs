using System;
using System.Collections.Generic;

namespace org.activiti.engine.impl.pvm.runtime
{


    using Context = org.activiti.engine.impl.context.Context;
    using VariableInstance = org.activiti.engine.impl.persistence.entity.VariableInstance;
    using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
    using ExecutionListenerExecution = org.activiti.engine.impl.pvm.@delegate.ExecutionListenerExecution;
    using SignallableActivityBehavior = org.activiti.engine.impl.pvm.@delegate.SignallableActivityBehavior;
    using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
    using ProcessDefinitionImpl = org.activiti.engine.impl.pvm.process.ProcessDefinitionImpl;
    using TransitionImpl = org.activiti.engine.impl.pvm.process.TransitionImpl;
    using Logger = org.slf4j.Logger;
    using LoggerFactory = org.slf4j.LoggerFactory;

    /// <summary>
    /// @author Tom Baeyens
    /// @author Joram Barrez
    /// @author Daniel Meyer
    /// @author Falko Menge
    /// </summary>
    [Serializable]
    public class ExecutionImpl : ActivityExecution, ExecutionListenerExecution, PvmExecution, InterpretableExecution
    {

        private const long serialVersionUID = 1L;

        private static Logger log = LoggerFactory.getLogger(typeof(ExecutionImpl));

        // current position /////////////////////////////////////////////////////////

        protected internal ProcessDefinitionImpl processDefinition;

        /// <summary>
        /// current activity </summary>
        protected internal ActivityImpl activity;

        /// <summary>
        /// current transition.  is null when there is no transition being taken. </summary>
        protected internal TransitionImpl transition = null;

        /// <summary>
        /// the process instance.  this is the root of the execution tree.  
        /// the processInstance of a process instance is a self reference. 
        /// </summary>
        protected internal ExecutionImpl processInstance;

        /// <summary>
        /// the parent execution </summary>
        protected internal ExecutionImpl parent;

        /// <summary>
        /// nested executions representing scopes or concurrent paths </summary>
        protected internal IList<ExecutionImpl> executions;

        /// <summary>
        /// super execution, not-null if this execution is part of a subprocess </summary>
        protected internal ExecutionImpl superExecution;

        /// <summary>
        /// reference to a subprocessinstance, not-null if currently subprocess is started from this execution </summary>
        protected internal ExecutionImpl subProcessInstance;

        /// <summary>
        /// only available until the process instance is started </summary>
        protected internal StartingExecution startingExecution;

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

        protected internal IDictionary<string, object> variables = null;

        // events ///////////////////////////////////////////////////////////////////

        protected internal string eventName;
        protected internal PvmProcessElement eventSource;
        protected internal int executionListenerIndex = 0;

        // cascade deletion ////////////////////////////////////////////////////////

        protected internal bool deleteRoot;
        protected internal string deleteReason;

        // replaced by //////////////////////////////////////////////////////////////

        /// <summary>
        /// when execution structure is pruned during a takeAll, then 
        /// the original execution has to be resolved to the replaced execution. </summary>
        /// <seealso cref= <seealso cref="#takeAll(List, List)"/> <seealso cref="OutgoingExecution"/>  </seealso>
        protected internal ExecutionImpl replacedBy;

        // atomic operations ////////////////////////////////////////////////////////

        /// <summary>
        /// next operation.  process execution is in fact runtime interpretation of the process model.
        /// each operation is a logical unit of interpretation of the process.  so sequentially processing 
        /// the operations drives the interpretation or execution of a process. </summary>
        /// <seealso cref= AtomicOperation </seealso>
        /// <seealso cref= #performOperation(AtomicOperation)  </seealso>
        protected internal AtomicOperation nextOperation;
        protected internal bool isOperating = false;

        /* Default constructor for ibatis/jpa/etc. */
        public ExecutionImpl()
        {
        }

        public ExecutionImpl(ActivityImpl initial)
        {
            startingExecution = new StartingExecution(initial);
        }

        // lifecycle methods ////////////////////////////////////////////////////////

        /// <summary>
        /// creates a new execution. properties processDefinition, processInstance and activity will be initialized. </summary>
        public virtual ExecutionImpl createExecution()
        {
            // create the new child execution
            ExecutionImpl createdExecution = newExecution();

            // manage the bidirectional parent-child relation
            ensureExecutionsInitialized();
            executions.Add(createdExecution);
            createdExecution.setParent(this);

            // initialize the new execution
            createdExecution.ProcessDefinition = ProcessDefinition;
            createdExecution.setProcessInstance(getProcessInstance());
            createdExecution.Activity = Activity;

            return createdExecution;
        }

        /// <summary>
        /// instantiates a new execution.  can be overridden by subclasses </summary>
        protected internal virtual ExecutionImpl newExecution()
        {
            return new ExecutionImpl();
        }

        public virtual PvmProcessInstance createSubProcessInstance(PvmProcessDefinition processDefinition)
        {
            ExecutionImpl subProcessInstance = newExecution();

            // manage bidirectional super-subprocess relation
            subProcessInstance.setSuperExecution(this);
            this.setSubProcessInstance(subProcessInstance);

            // Initialize the new execution
            subProcessInstance.ProcessDefinition = (ProcessDefinitionImpl)processDefinition;
            subProcessInstance.setProcessInstance(subProcessInstance);

            return subProcessInstance;
        }

        public virtual void initialize()
        {
        }

        public virtual void destroy()
        {
            Scope = false;
        }

        public virtual void remove()
        {
            ensureParentInitialized();
            if (parent != null)
            {
                parent.ensureExecutionsInitialized();
                parent.executions.Remove(this);
            }

            // remove event scopes:            
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

        public override void destroyScope(string reason)
        {

            log.debug("performing destroy scope behavior for execution {}", this);

            // remove all child executions and sub process instances:
            IList<InterpretableExecution> executions = new List<InterpretableExecution>(Executions);
            foreach (InterpretableExecution childExecution in executions)
            {
                if (childExecution.getSubProcessInstance() != null)
                {
                    childExecution.getSubProcessInstance().deleteCascade(reason);
                }
                childExecution.deleteCascade(reason);
            }

        }

        // parent ///////////////////////////////////////////////////////////////////

        /// <summary>
        /// ensures initialization and returns the parent </summary>
        public virtual ExecutionImpl getParent()
        {
            ensureParentInitialized();
            return parent;
        }

        public override string SuperExecutionId
        {
            get
            {
                ensureActivityInitialized();
                if (superExecution != null)
                {
                    return superExecution.Id;
                }

                return null;
            }
        }

        public virtual string ParentId
        {
            get
            {
                ensureActivityInitialized();
                if (parent != null)
                {
                    return parent.Id;
                }
                return null;
            }
        }

        /// <summary>
        /// all updates need to go through this setter as subclasses can override this method </summary>
        public virtual void setParent(InterpretableExecution parent)
        {
            this.parent = (ExecutionImpl)parent;
        }

        /// <summary>
        /// must be called before memberfield parent is used. 
        /// can be used by subclasses to provide parent member field initialization. 
        /// </summary>
        protected internal virtual void ensureParentInitialized()
        {
        }

        // executions ///////////////////////////////////////////////////////////////  

        /// <summary>
        /// ensures initialization and returns the non-null executions list </summary>
        public virtual IList<ExecutionImpl> Executions
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

        public virtual ExecutionImpl getSuperExecution()
        {
            ensureSuperExecutionInitialized();
            return superExecution;
        }

        public virtual void setSuperExecution(ExecutionImpl superExecution)
        {
            this.superExecution = superExecution;
            if (superExecution != null)
            {
                superExecution.setSubProcessInstance(null);
            }
        }

        // Meant to be overridden by persistent subclasseses
        protected internal virtual void ensureSuperExecutionInitialized()
        {
        }

        public virtual ExecutionImpl getSubProcessInstance()
        {
            ensureSubProcessInstanceInitialized();
            return subProcessInstance;
        }

        public virtual void setSubProcessInstance(InterpretableExecution subProcessInstance)
        {
            this.subProcessInstance = (ExecutionImpl)subProcessInstance;
        }

        // Meant to be overridden by persistent subclasses
        protected internal virtual void ensureSubProcessInstanceInitialized()
        {
        }

        public virtual void deleteCascade(string deleteReason)
        {
            this.deleteReason = deleteReason;
            this.deleteRoot = true;
            performOperation(AtomicOperation_Fields.DELETE_CASCADE);
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
            performOperation(AtomicOperation_Fields.ACTIVITY_END);
        }

        /// <summary>
        /// searches for an execution positioned in the given activity </summary>
        public virtual ExecutionImpl findExecution(string activityId)
        {
            if ((Activity != null) && (Activity.Id.Equals(activityId)))
            {
                return this;
            }
            foreach (ExecutionImpl nestedExecution in Executions)
            {
                ExecutionImpl result = nestedExecution.findExecution(activityId);
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
            foreach (ExecutionImpl execution in executions)
            {
                execution.collectActiveActivityIds(activeActivityIds);
            }
        }

        /// <summary>
        /// must be called before memberfield executions is used. 
        /// can be used by subclasses to provide executions member field initialization. 
        /// </summary>
        protected internal virtual void ensureExecutionsInitialized()
        {
            if (executions == null)
            {
                executions = new List<ExecutionImpl>();
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
            }
        }

        public virtual string ProcessDefinitionId
        {
            get
            {
                return ProcessDefinition.Id;
            }
        }

        /// <summary>
        /// for setting the process definition, this setter must be used as subclasses can override </summary>

        /// <summary>
        /// must be called before memberfield processDefinition is used. 
        /// can be used by subclasses to provide processDefinition member field initialization. 
        /// </summary>
        protected internal virtual void ensureProcessDefinitionInitialized()
        {
        }

        // process instance /////////////////////////////////////////////////////////

        /// <summary>
        /// ensures initialization and returns the process instance. </summary>
        public virtual ExecutionImpl getProcessInstance()
        {
            ensureProcessInstanceInitialized();
            return processInstance;
        }

        public virtual string ProcessInstanceId
        {
            get
            {
                return getProcessInstance().Id;
            }
        }

        public virtual string BusinessKey
        {
            get
            {
                return getProcessInstance().BusinessKey;
            }
        }

        public virtual string ProcessBusinessKey
        {
            get
            {
                return getProcessInstance().BusinessKey;
            }
        }

        /// <summary>
        /// for setting the process instance, this setter must be used as subclasses can override </summary>
        public virtual void setProcessInstance(InterpretableExecution processInstance)
        {
            this.processInstance = (ExecutionImpl)processInstance;
        }

        /// <summary>
        /// must be called before memberfield processInstance is used. 
        /// can be used by subclasses to provide processInstance member field initialization. 
        /// </summary>
        protected internal virtual void ensureProcessInstanceInitialized()
        {
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
            }
        }


        /// <summary>
        /// must be called before the activity member field or getActivity() is called </summary>
        protected internal virtual void ensureActivityInitialized()
        {
        }

        // scopes ///////////////////////////////////////////////////////////////////

        protected internal virtual void ensureScopeInitialized()
        {
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

        // process instance start implementation ////////////////////////////////////

        public virtual void start()
        {
            if (startingExecution == null && ProcessInstanceType)
            {
                startingExecution = new StartingExecution(processDefinition.getInitial());
            }
            performOperation(AtomicOperation_Fields.PROCESS_START);
        }

        // methods that translate to operations /////////////////////////////////////

        public virtual void signal(string signalName, object signalData)
        {
            ensureActivityInitialized();
            SignallableActivityBehavior activityBehavior = (SignallableActivityBehavior)activity.ActivityBehavior;
            try
            {
                activityBehavior.signal(this, signalName, signalData);
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

        public override void take(PvmTransition transition, bool fireActivityCompletedEvent)
        {
            // No event firing on executionlevel impl
            take(transition);
        }

        public virtual void take(PvmTransition transition)
        {
            if (this.transition != null)
            {
                throw new PvmException("already taking a transition");
            }
            if (transition == null)
            {
                throw new PvmException("transition is null");
            }
            Transition = (TransitionImpl)transition;
            performOperation(AtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_END);
        }

        public virtual void executeActivity(PvmActivity activity)
        {
            Activity = (ActivityImpl)activity;
            performOperation(AtomicOperation_Fields.ACTIVITY_START);
        }

        public virtual IList<ActivityExecution> findInactiveConcurrentExecutions(PvmActivity activity)
        {
            IList<ActivityExecution> inactiveConcurrentExecutionsInActivity = new List<ActivityExecution>();
            IList<ActivityExecution> otherConcurrentExecutions = new List<ActivityExecution>();
            if (Concurrent)
            {
                IList<ActivityExecution> concurrentExecutions = getParent().Executions;
                foreach (ActivityExecution concurrentExecution in concurrentExecutions)
                {
                    if (concurrentExecution.Activity != null && concurrentExecution.Activity.Id.Equals(activity.Id))
                    {
                        if (concurrentExecution.Active)
                        {
                            throw new PvmException("didn't expect active execution in " + activity + ". bug?");
                        }
                        inactiveConcurrentExecutionsInActivity.Add(concurrentExecution);
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

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public void takeAll(java.util.List<org.activiti.engine.impl.pvm.PvmTransition> transitions, java.util.List<org.activiti.engine.impl.pvm.delegate.ActivityExecution> recyclableExecutions)
        public virtual void takeAll(IList<PvmTransition> transitions, IList<ActivityExecution> recyclableExecutions)
        {
            transitions = new List<PvmTransition>(transitions);
            recyclableExecutions = (recyclableExecutions != null ? new List<ActivityExecution>(recyclableExecutions) : new List<ActivityExecution>());

            if (recyclableExecutions.Count > 1)
            {
                foreach (ActivityExecution recyclableExecution in recyclableExecutions)
                {
                    if (((ExecutionImpl)recyclableExecution).Scope)
                    {
                        throw new PvmException("joining scope executions is not allowed");
                    }
                }
            }

            ExecutionImpl concurrentRoot = ((isConcurrent && !isScope) ? getParent() : this);
            IList<ExecutionImpl> concurrentActiveExecutions = new List<ExecutionImpl>();
            foreach (ExecutionImpl execution in concurrentRoot.Executions)
            {
                if (execution.Active)
                {
                    concurrentActiveExecutions.Add(execution);
                }
            }

            if (log.DebugEnabled)
            {
                log.debug("transitions to take concurrent: {}", transitions);
                log.debug("active concurrent executions: {}", concurrentActiveExecutions);
            }

            if ((transitions.Count == 1) && (concurrentActiveExecutions.Count == 0))
            {

                //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
                //ORIGINAL LINE: @SuppressWarnings("rawtypes") java.util.List<ExecutionImpl> recyclableExecutionImpls = (java.util.List) recyclableExecutions;
                IList<ExecutionImpl> recyclableExecutionImpls = (IList)recyclableExecutions;
                foreach (ExecutionImpl prunedExecution in recyclableExecutionImpls)
                {
                    // End the pruned executions if necessary.
                    // Some recyclable executions are inactivated (joined executions)
                    // Others are already ended (end activities)
                    if (!prunedExecution.Ended)
                    {
                        log.debug("pruning execution {}", prunedExecution);
                        prunedExecution.remove();
                    }
                }

                log.debug("activating the concurrent root {} as the single path of execution going forward", concurrentRoot);
                concurrentRoot.Active = true;
                concurrentRoot.Activity = activity;
                concurrentRoot.Concurrent = false;
                concurrentRoot.take(transitions[0]);

            }
            else
            {

                IList<OutgoingExecution> outgoingExecutions = new List<OutgoingExecution>();

                recyclableExecutions.Remove(concurrentRoot);

                log.debug("recyclable executions for reused: {}", recyclableExecutions);

                // first create the concurrent executions
                while (transitions.Count > 0)
                {
                    PvmTransition outgoingTransition = transitions.Remove(0);

                    ExecutionImpl outgoingExecution = null;
                    if (recyclableExecutions.Count == 0)
                    {
                        outgoingExecution = concurrentRoot.createExecution();
                        log.debug("new {} created to take transition {}", outgoingExecution, outgoingTransition);
                    }
                    else
                    {
                        outgoingExecution = (ExecutionImpl)recyclableExecutions.Remove(0);
                        log.debug("recycled {} to take transition {}", outgoingExecution, outgoingTransition);
                    }

                    outgoingExecution.Active = true;
                    outgoingExecution.Scope = false;
                    outgoingExecution.Concurrent = true;
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
                    outgoingExecution.take();
                }
            }
        }

        public virtual void performOperation(AtomicOperation executionOperation)
        {
            this.nextOperation = executionOperation;
            if (!isOperating)
            {
                isOperating = true;
                while (nextOperation != null)
                {
                    AtomicOperation currentOperation = this.nextOperation;
                    this.nextOperation = null;
                    if (log.TraceEnabled)
                    {
                        log.trace("AtomicOperation: {} on {}", currentOperation, this);
                    }
                    currentOperation.execute(this);
                }
                isOperating = false;
            }
        }


        public virtual bool isActive(string activityId)
        {
            return findExecution(activityId) != null;
        }

        // variables ////////////////////////////////////////////////////////////////

        public virtual object getVariable(string variableName)
        {
            ensureVariablesInitialized();

            // If value is found in this scope, return it
            if (variables.ContainsKey(variableName))
            {
                return variables[variableName];
            }

            // If value not found in this scope, check the parent scope
            ensureParentInitialized();
            if (parent != null)
            {
                return parent.getVariable(variableName);
            }

            // Variable is nowhere to be found
            return null;
        }

        public override object getVariable(string variableName, bool fetchAllVariables)
        {
            return getVariable(variableName); // No support for fetchAllVariables on ExecutionImpl
        }

        public virtual IDictionary<string, object> Variables
        {
            get
            {
                IDictionary<string, object> collectedVariables = new Dictionary<string, object>();
                collectVariables(collectedVariables);
                return collectedVariables;
            }
            set
            {
                ensureVariablesInitialized();
                if (value != null)
                {
                    foreach (string variableName in value.Keys)
                    {
                        setVariable(variableName, value[variableName]);
                    }
                }
            }
        }

        public override IDictionary<string, object> getVariables(ICollection<string> variableNames)
        {
            IDictionary<string, object> allVariables = Variables;
            IDictionary<string, object> filteredVariables = new Dictionary<string, object>();
            foreach (string variableName in variableNames)
            {
                filteredVariables[variableName] = allVariables[variableName];
            }
            return filteredVariables;
        }

        public override IDictionary<string, object> getVariables(ICollection<string> variableNames, bool fetchAllVariables)
        {
            return getVariables(variableNames); // No support for the boolean param
        }

        protected internal virtual void collectVariables(IDictionary<string, object> collectedVariables)
        {
            ensureParentInitialized();
            if (parent != null)
            {
                parent.collectVariables(collectedVariables);
            }
            ensureVariablesInitialized();
            foreach (string variableName in variables.Keys)
            {
                collectedVariables[variableName] = variables[variableName];
            }
        }


        public virtual void setVariable(string variableName, object value)
        {
            ensureVariablesInitialized();
            if (variables.ContainsKey(variableName))
            {
                setVariableLocally(variableName, value);
            }
            else
            {
                ensureParentInitialized();
                if (parent != null)
                {
                    parent.setVariable(variableName, value);
                }
                else
                {
                    setVariableLocally(variableName, value);
                }
            }
        }


        public override void setVariable(string variableName, object value, bool fetchAllVariables)
        {
            setVariable(variableName, value);
        }

        public virtual void setVariableLocally(string variableName, object value)
        {
            log.debug("setting variable '{}' to value '{}' on {}", variableName, value, this);
            variables[variableName] = value;
        }

        public override object setVariableLocal(string variableName, object value, bool fetchAllVariables)
        {
            return setVariableLocal(variableName, value);
        }

        public virtual bool hasVariable(string variableName)
        {
            ensureVariablesInitialized();
            if (variables.ContainsKey(variableName))
            {
                return true;
            }
            ensureParentInitialized();
            if (parent != null)
            {
                return parent.hasVariable(variableName);
            }
            return false;
        }

        protected internal virtual void ensureVariablesInitialized()
        {
            if (variables == null)
            {
                variables = new Dictionary<string, object>();
            }
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
                return (isEventScope ? "EventScope" : "") + (isConcurrent ? "Concurrent" : "") + (Scope ? "Scope" : "") + "Execution[" + ToStringIdentity + "]";
            }
        }

        protected internal virtual string ToStringIdentity
        {
            get
            {
                return Convert.ToString(System.identityHashCode(this));
            }
        }

        // customized getters and setters ///////////////////////////////////////////

        public virtual bool ProcessInstanceType
        {
            get
            {
                ensureParentInitialized();
                return parent == null;
            }
        }

        public virtual void inactivate()
        {
            this.isActive_Renamed = false;
        }

        // allow for subclasses to expose a real id /////////////////////////////////

        public virtual string Id
        {
            get
            {
                return null;
            }
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual TransitionImpl Transition
        {
            get
            {
                return transition;
            }
            set
            {
                this.transition = value;
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
        public virtual ExecutionImpl getReplacedBy()
        {
            return replacedBy;
        }
        public virtual void setReplacedBy(InterpretableExecution replacedBy)
        {
            this.replacedBy = (ExecutionImpl)replacedBy;
        }
        public virtual bool DeleteRoot
        {
            get
            {
                return deleteRoot;
            }
        }

        public virtual string CurrentActivityId
        {
            get
            {
                string currentActivityId = null;
                if (this.activity != null)
                {
                    currentActivityId = activity.Id;
                }
                return currentActivityId;
            }
        }

        public virtual string CurrentActivityName
        {
            get
            {
                string currentActivityName = null;
                if (this.activity != null)
                {
                    currentActivityName = (string)activity.getProperty("name");
                }
                return currentActivityName;
            }
        }

        public virtual IDictionary<string, VariableInstance> VariableInstances
        {
            get
            {
                return null;
            }
        }

        public virtual IDictionary<string, VariableInstance> getVariableInstances(ICollection<string> variableNames)
        {
            return null;
        }

        public virtual IDictionary<string, VariableInstance> getVariableInstances(ICollection<string> variableNames, bool fetchAllVariables)
        {
            return null;
        }

        public virtual IDictionary<string, VariableInstance> VariableInstancesLocal
        {
            get
            {
                return null;
            }
        }

        public virtual IDictionary<string, VariableInstance> getVariableInstancesLocal(ICollection<string> variableNames)
        {
            return null;
        }

        public virtual IDictionary<string, VariableInstance> getVariableInstancesLocal(ICollection<string> variableNames, bool fetchAllVariables)
        {
            return null;
        }

        public virtual VariableInstance getVariableInstance(string variableName)
        {
            return null;
        }

        public virtual VariableInstance getVariableInstance(string variableName, bool fetchAllVariables)
        {
            return null;
        }

        public virtual void createVariableLocal(string variableName, object value)
        {
        }

        public virtual void createVariablesLocal<T1>(IDictionary<T1> variables) where T1 : Object
        {
        }

        public virtual object getVariableLocal(string variableName)
        {
            return null;
        }

        public virtual VariableInstance getVariableInstanceLocal(string variableName)
        {
            return null;
        }

        public override object getVariableLocal(string variableName, bool fetchAllVariables)
        {
            return getVariableLocal(variableName); // No support for fetchAllVariables
        }

        public virtual VariableInstance getVariableInstanceLocal(string variableName, bool fetchAllVariables)
        {
            return null;
        }

        public override T getVariable<T>(string variableName, Type variableClass)
        {
            return variableClass.cast(getVariable(variableName));
        }

        public override T getVariableLocal<T>(string variableName, Type variableClass)
        {
            return variableClass.cast(getVariableLocal(variableName));
        }

        public virtual Set<string> VariableNames
        {
            get
            {
                return null;
            }
        }

        public virtual Set<string> VariableNamesLocal
        {
            get
            {
                return null;
            }
        }

        public virtual IDictionary<string, object> VariablesLocal
        {
            get
            {
                return null;
            }
            set
            {
            }
        }


        public override IDictionary<string, object> getVariablesLocal(ICollection<string> variableNames)
        {
            return null;
        }

        public override IDictionary<string, object> getVariablesLocal(ICollection<string> variableNames, bool fetchAllVariables)
        {
            return null;
        }

        public virtual bool hasVariableLocal(string variableName)
        {
            return false;
        }

        public virtual bool hasVariables()
        {
            return false;
        }

        public virtual bool hasVariablesLocal()
        {
            return false;
        }

        public virtual void removeVariable(string variableName)
        {
        }

        public virtual void removeVariableLocal(string variableName)
        {
        }

        public virtual void removeVariables(ICollection<string> variableNames)
        {
        }

        public virtual void removeVariablesLocal(ICollection<string> variableNames)
        {
        }

        public virtual void removeVariables()
        {
        }

        public virtual void removeVariablesLocal()
        {
        }

        public virtual void deleteVariablesLocal()
        {
        }

        public virtual object setVariableLocal(string variableName, object value)
        {
            return null;
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

        public virtual string updateProcessBusinessKey(string bzKey)
        {
            return getProcessInstance().updateProcessBusinessKey(bzKey);
        }

        public virtual string TenantId
        {
            get
            {
                return null; // Not implemented
            }
        }
    }

}