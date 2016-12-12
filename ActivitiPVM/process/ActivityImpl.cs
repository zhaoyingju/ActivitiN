using System;
using System.Collections;
using System.Collections.Generic;

namespace org.activiti.engine.impl.pvm.process
{


    using Expression = org.activiti.engine.@delegate.Expression;
    using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;


    [Serializable]
    public class ActivityImpl : ScopeImpl, PvmActivity, HasDIBounds
    {
        protected internal IList<TransitionImpl> outgoingTransitions = new List<TransitionImpl>();
        protected internal IDictionary<string, TransitionImpl> namedOutgoingTransitions = new Dictionary<string, TransitionImpl>();
        protected internal IDictionary<string, object> variables;
        protected internal IList<TransitionImpl> incomingTransitions = new List<TransitionImpl>();
        protected internal ActivityBehavior activityBehavior;
        protected internal ScopeImpl parent;
        protected internal bool isScope;
        protected internal bool isAsync;
        protected internal bool isExclusive;
        protected internal string failedJobRetryTimeCycleValue;

        // Graphical information
        protected internal int x = -1;
        protected internal int y = -1;
        protected internal int width = -1;
        protected internal int height = -1;

        public ActivityImpl(string id, ProcessDefinitionImpl processDefinition) : base(id, processDefinition)
        {
        }

        public virtual string FailedJobRetryTimeCycleValue
        {
            get
            {
                return failedJobRetryTimeCycleValue;
            }
            set
            {
                this.failedJobRetryTimeCycleValue = value;
            }
        }


        public virtual TransitionImpl createOutgoingTransition()
        {
            return createOutgoingTransition(null);
        }

        public virtual TransitionImpl createOutgoingTransition(string transitionId)
        {
            return createOutgoingTransition(transitionId, null);
        }

        public virtual TransitionImpl createOutgoingTransition(string transitionId, Expression skipExpression)
        {
            TransitionImpl transition = new TransitionImpl(transitionId, skipExpression, processDefinition);
            transition.setSource(this);
            outgoingTransitions.Add(transition);

            if (transitionId != null)
            {
                if (namedOutgoingTransitions.ContainsKey(transitionId))
                {
                    throw new PvmException("activity '" + id + " has duplicate transition '" + transitionId + "'");
                }
                namedOutgoingTransitions[transitionId] = transition;
            }

            return transition;
        }

        public virtual PvmTransition findOutgoingTransition(string transitionId)
        {
            return namedOutgoingTransitions[transitionId];
        }

        public override string ToString()
        {
            return "Activity(" + id + ")";
        }

        public virtual ActivityImpl ParentActivity
        {
            get
            {
                if (parent is ActivityImpl)
                {
                    return (ActivityImpl)parent;
                }
                return null;
            }
        }


        // restricted setters ///////////////////////////////////////////////////////

        public virtual IList<TransitionImpl> setOutgoingTransitions
        {
            set
            {
                this.outgoingTransitions = value;
            }
            get
            {
                return outgoingTransitions;
            }
        }

        protected internal virtual ScopeImpl setParent
        {
            set
            {
                this.parent = value;
            }
            get
            {
                return parent;
            }
        }

        public virtual PvmScope Parent
        {
            get
            {
                return parent;
            }
        }

        public virtual IList<TransitionImpl> setIncomingTransitions
        {
            set
            {
                this.incomingTransitions = value;
            }
            get
            {
                return incomingTransitions;
            }
        }

        // getters and setters //////////////////////////////////////////////////////


        public virtual ActivityBehavior ActivityBehavior
        {
            get
            {
                return activityBehavior;
            }
            set
            {
                this.activityBehavior = value;
            }
        }




        public virtual IDictionary<string, object> Variables
        {
            get
            {
                return variables;
            }
            set
            {
                this.variables = value;
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


        public virtual int X
        {
            get
            {
                return x;
            }
            set
            {
                this.x = value;
            }
        }


        public virtual int Y
        {
            get
            {
                return y;
            }
            set
            {
                this.y = value;
            }
        }


        public virtual int Width
        {
            get
            {
                return width;
            }
            set
            {
                this.width = value;
            }
        }


        public virtual int Height
        {
            get
            {
                return height;
            }
            set
            {
                this.height = value;
            }
        }


        public virtual bool Async
        {
            get
            {
                return isAsync;
            }
            set
            {
                this.isAsync = value;
            }
        }


        public virtual bool Exclusive
        {
            get
            {
                return isExclusive;
            }
            set
            {
                this.isExclusive = value;
            }
        }

        public IList<PvmTransition> IncomingTransitions
        {
            get
            {
                return incomingTransitions as List<PvmTransition>;
            }
        }

        public IList<PvmTransition> OutgoingTransitions
        {
            get
            {
                return outgoingTransitions as List<PvmTransition>;
            }
        }
    }

}