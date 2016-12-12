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


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class TransitionImpl : ProcessElementImpl, PvmTransition
	{

	  private const long serialVersionUID = 1L;

	  protected internal ActivityImpl source;
	  protected internal ActivityImpl destination;
	  protected internal IList<ExecutionListener> executionListeners;
	  protected internal Expression skipExpression;

	  /// <summary>
	  /// Graphical information: a list of waypoints: x1, y1, x2, y2, x3, y3, .. </summary>
	  protected internal IList<int?> waypoints = new List<int?>();

	  public TransitionImpl(string id, Expression skipExpression, ProcessDefinitionImpl processDefinition) : base(id, processDefinition)
	  {
		this.skipExpression = skipExpression;
	  }

	  public virtual ActivityImpl getSource()
	  {
		return source;
	  }

	  public virtual ActivityImpl Destination
	  {
		  set
		  {
			this.destination = value;
			value.IncomingTransitions.Add(this);
		  }
		  get
		  {
			return destination;
		  }
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
		return "(" + source.Id + ")--" + (id != null?id + "-->(":">(") + destination.Id + ")";
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.delegate.ExecutionListener> getExecutionListeners()
	  public virtual IList<ExecutionListener> ExecutionListeners
	  {
		  get
		  {
			if (executionListeners == null)
			{
			  return Collections.EMPTY_LIST;
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

	}

}