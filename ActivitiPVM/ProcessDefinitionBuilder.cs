using System.Collections.Generic;


namespace org.activiti.engine.impl.pvm
{


    using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;
    using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
    using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
    using ProcessDefinitionImpl = org.activiti.engine.impl.pvm.process.ProcessDefinitionImpl;
    using ProcessElementImpl = org.activiti.engine.impl.pvm.process.ProcessElementImpl;
    using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
    using TransitionImpl = org.activiti.engine.impl.pvm.process.TransitionImpl;



    /// <summary>
    /// 流程定义构造器
    /// </summary>
    public class ProcessDefinitionBuilder
    {
        private bool InstanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            processElement = processDefinition;
        }


        protected internal ProcessDefinitionImpl processDefinition;
        protected internal Stack<ScopeImpl> scopeStack = new Stack<ScopeImpl>();
        protected internal ProcessElementImpl processElement;
        protected internal TransitionImpl transition_Renamed;
        protected internal IList<object[]> unresolvedTransitions = new List<object[]>();

        public ProcessDefinitionBuilder() : this(null)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        public ProcessDefinitionBuilder(string processDefinitionId)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            processDefinition = new ProcessDefinitionImpl(processDefinitionId);
            scopeStack.Push(processDefinition);
        }

        public virtual ProcessDefinitionBuilder createActivity(string id)
        {
            ActivityImpl activity = scopeStack.Peek().createActivity(id);
            scopeStack.Push(activity);
            processElement = activity;

            transition_Renamed = null;

            return this;
        }

        public virtual ProcessDefinitionBuilder endActivity()
        {
            scopeStack.Pop();
            processElement = scopeStack.Peek();

            transition_Renamed = null;

            return this;
        }

        public virtual ProcessDefinitionBuilder initial()
        {
            processDefinition.setInitial(Activity);
            return this;
        }

        public virtual ProcessDefinitionBuilder startTransition(string destinationActivityId)
        {
            return startTransition(destinationActivityId, null);
        }

        public virtual ProcessDefinitionBuilder startTransition(string destinationActivityId, string transitionId)
        {
            if (destinationActivityId == null)
            {
                throw new PvmException("destinationActivityId is null");
            }
            ActivityImpl activity = Activity;
            transition_Renamed = activity.createOutgoingTransition(transitionId);
            unresolvedTransitions.Add(new object[] { transition_Renamed, destinationActivityId });
            processElement = transition_Renamed;
            return this;
        }

        public virtual ProcessDefinitionBuilder endTransition()
        {
            processElement = scopeStack.Peek();
            transition_Renamed = null;
            return this;
        }

        public virtual ProcessDefinitionBuilder transition(string destinationActivityId)
        {
            return transition(destinationActivityId, null);
        }

        public virtual ProcessDefinitionBuilder transition(string destinationActivityId, string transitionId)
        {
            startTransition(destinationActivityId, transitionId);
            endTransition();
            return this;
        }

        public virtual ProcessDefinitionBuilder behavior(ActivityBehavior activityBehaviour)
        {
            Activity.ActivityBehavior = activityBehaviour;
            return this;
        }

        public virtual ProcessDefinitionBuilder property(string name, object value)
        {
            processElement.setProperty(name, value);
            return this;
        }

        public virtual PvmProcessDefinition buildProcessDefinition()
        {
            foreach (object[] unresolvedTransition in unresolvedTransitions)
            {
                TransitionImpl transition = (TransitionImpl)unresolvedTransition[0];
                string destinationActivityName = (string)unresolvedTransition[1];
                ActivityImpl destination = processDefinition.findActivity(destinationActivityName);
                if (destination == null)
                {
                    throw new ActivitiException("destination '" + destinationActivityName + "' not found.  (referenced from transition in '" + transition.getSource().Id + "')");
                }
                transition.setDestination(destination);
            }
            return processDefinition;
        }

        protected internal virtual ActivityImpl Activity
        {
            get
            {
                return (ActivityImpl)scopeStack.Peek();
            }
        }

        public virtual ProcessDefinitionBuilder scope()
        {
            Activity.Scope = true;
            return this;
        }

        public virtual ProcessDefinitionBuilder executionListener(ExecutionListener executionListener)
        {
            if (transition_Renamed != null)
            {
                transition_Renamed.addExecutionListener(executionListener);
            }
            else
            {
                throw new PvmException("not in a transition scope");
            }
            return this;
        }

        public virtual ProcessDefinitionBuilder executionListener(string eventName, ExecutionListener executionListener)
        {
            if (transition_Renamed == null)
            {
                scopeStack.Peek().addExecutionListener(eventName, executionListener);
            }
            else
            {
                throw new PvmException("not in an activity- or process definition scope. (but in a transition scope)");
            }
            return this;
        }
    }

}