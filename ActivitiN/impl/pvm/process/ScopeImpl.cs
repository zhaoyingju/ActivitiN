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
	using IOSpecification = org.activiti.engine.impl.bpmn.data.IOSpecification;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public abstract class ScopeImpl : ProcessElementImpl, PvmScope
	{

	  private const long serialVersionUID = 1L;

	  protected internal IList<ActivityImpl> activities = new List<ActivityImpl>();
	  protected internal IDictionary<string, ActivityImpl> namedActivities = new Dictionary<string, ActivityImpl>();
	  protected internal IDictionary<string, IList<ExecutionListener>> executionListeners = new Dictionary<string, IList<ExecutionListener>>();
	  protected internal IOSpecification ioSpecification;

	  public ScopeImpl(string id, ProcessDefinitionImpl processDefinition) : base(id, processDefinition)
	  {
	  }

	  public virtual ActivityImpl findActivity(string activityId)
	  {
		ActivityImpl localActivity = namedActivities[activityId];
		if (localActivity != null)
		{
		  return localActivity;
		}
		foreach (ActivityImpl activity in activities)
		{
		  ActivityImpl nestedActivity = activity.findActivity(activityId);
		  if (nestedActivity != null)
		  {
			return nestedActivity;
		  }
		}
		return null;
	  }

	  public virtual ActivityImpl createActivity()
	  {
		return createActivity(null);
	  }

	  public virtual ActivityImpl createActivity(string activityId)
	  {
		ActivityImpl activity = new ActivityImpl(activityId, processDefinition);
		if (activityId != null)
		{
		  if (processDefinition.findActivity(activityId) != null)
		  {
			throw new PvmException("duplicate activity id '" + activityId + "'");
		  }
		  namedActivities[activityId] = activity;
		}
		activity.Parent = this;
		activities.Add(activity);
		return activity;
	  }

	  public virtual bool contains(ActivityImpl activity)
	  {
		if (namedActivities.ContainsKey(activity.Id))
		{
		  return true;
		}
		foreach (ActivityImpl nestedActivity in activities)
		{
		  if (nestedActivity.contains(activity))
		  {
			return true;
		  }
		}
		return false;
	  }

	  // event listeners //////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.delegate.ExecutionListener> getExecutionListeners(String eventName)
	  public virtual IList<ExecutionListener> getExecutionListeners(string eventName)
	  {
		IList<ExecutionListener> executionListenerList = ExecutionListeners[eventName];
		if (executionListenerList != null)
		{
		  return executionListenerList;
		}
		return Collections.EMPTY_LIST;
	  }

	  public virtual void addExecutionListener(string eventName, ExecutionListener executionListener)
	  {
		addExecutionListener(eventName, executionListener, -1);
	  }

	  public virtual void addExecutionListener(string eventName, ExecutionListener executionListener, int index)
	  {
		IList<ExecutionListener> listeners = executionListeners[eventName];
		if (listeners == null)
		{
		  listeners = new List<ExecutionListener>();
		  executionListeners[eventName] = listeners;
		}
		if (index < 0)
		{
		  listeners.Add(executionListener);
		}
		else
		{
		  listeners.Insert(index, executionListener);
		}
	  }

	  public virtual IDictionary<string, IList<ExecutionListener>> ExecutionListeners
	  {
		  get
		  {
			return executionListeners;
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual IList<ActivityImpl> Activities
	  {
		  get
		  {
			return activities;
		  }
	  }

	  public virtual IOSpecification IoSpecification
	  {
		  get
		  {
			return ioSpecification;
		  }
		  set
		  {
			this.ioSpecification = value;
		  }
	  }

	}

}