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

namespace org.activiti.engine.impl.pvm.process
{


    using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;
    using Expression = org.activiti.engine.@delegate.Expression;



    [Serializable]
    public class TransitionImpl : ProcessElementImpl, PvmTransition
    {

        protected internal ActivityImpl source;
        protected internal ActivityImpl destination;
        protected internal IList<ExecutionListener> executionListeners;
        protected internal Expression skipExpression;


        protected internal IList<int?> waypoints = new List<int?>();

        public TransitionImpl(string id, Expression skipExpression, ProcessDefinitionImpl processDefinition) : base(id, processDefinition)
        {
            this.skipExpression = skipExpression;
        }

        public virtual ActivityImpl getSource()
        {
            return source;
        }

        public virtual ActivityImpl getDestination()
        {
            return destination;

        }

        public virtual void setDestination(ActivityImpl value)
        {
            this.destination = value;
            value.setIncomingTransitions.Add(this);
        }

        public virtual void addExecutionListener(ExecutionListener executionListener)
        {
            if (executionListeners == null)
            {
                executionListeners = new List<ExecutionListener>();
            }
            executionListeners.Add(executionListener);
        }

        public override string ToString()
        {
            return "(" + source.Id + ")--" + (id != null ? id + "-->(" : ">(") + destination.Id + ")";
        }

        public virtual IList<ExecutionListener> ExecutionListeners
        {
            get
            {
                if (executionListeners == null)
                {
                    return new List<ExecutionListener>();
                }
                return executionListeners;
            }
            set
            {
                this.executionListeners = value;
            }
        }

        // getters and setters //////////////////////////////////////////////////////

        protected internal virtual void setSource(ActivityImpl source)
        {
            this.source = source;
        }

        public virtual IList<int?> Waypoints
        {
            get
            {
                return waypoints;
            }
            set
            {
                this.waypoints = value;
            }
        }

        public virtual Expression SkipExpression
        {
            get
            {
                return skipExpression;
            }
            set
            {
                this.skipExpression = value;
            }
        }

        public PvmActivity Source
        {
            get
            {
                return source;
            }
        }

        public PvmActivity Destination
        {
            get
            {
                return this.source;
            }
        }
    }
}