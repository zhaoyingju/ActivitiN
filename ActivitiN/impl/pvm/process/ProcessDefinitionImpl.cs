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


	using ExecutionImpl = org.activiti.engine.impl.pvm.runtime.ExecutionImpl;
	using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class ProcessDefinitionImpl : ScopeImpl, PvmProcessDefinition
	{

	  private const long serialVersionUID = 1L;

	  protected internal string name;
	  protected internal string key;
	  protected internal string description;
	  protected internal ActivityImpl initial;
	  protected internal IDictionary<ActivityImpl, IList<ActivityImpl>> initialActivityStacks = new Dictionary<ActivityImpl, IList<ActivityImpl>>();
	  protected internal IList<LaneSet> laneSets;
	  protected internal ParticipantProcess participantProcess;

	  public ProcessDefinitionImpl(string id) : base(id, null)
	  {
		processDefinition = this;
	  }

	  public virtual PvmProcessInstance createProcessInstance()
	  {
		if (initial == null)
		{
		  throw new ActivitiException("Process '" + name + "' has no default start activity (e.g. none start event), hence you cannot use 'startProcessInstanceBy...' but have to start it using one of the modeled start events (e.g. message start events).");
		}
		return createProcessInstanceForInitial(initial);
	  }

	  /// <summary>
	  /// creates a process instance using the provided activity as initial </summary>
	  public virtual PvmProcessInstance createProcessInstanceForInitial(ActivityImpl initial)
	  {

		if (initial == null)
		{
		  throw new ActivitiException("Cannot start process instance, initial activity where the process instance should start is null.");
		}

		InterpretableExecution processInstance = newProcessInstance(initial);
		processInstance.ProcessDefinition = this;
		processInstance.ProcessInstance = processInstance;
		processInstance.initialize();

		InterpretableExecution scopeInstance = processInstance;

		IList<ActivityImpl> initialActivityStack = getInitialActivityStack(initial);

		foreach (ActivityImpl initialActivity in initialActivityStack)
		{
		  if (initialActivity.Scope)
		  {
			scopeInstance = (InterpretableExecution) scopeInstance.createExecution();
			scopeInstance.Activity = initialActivity;
			if (initialActivity.Scope)
			{
			  scopeInstance.initialize();
			}
		  }
		}

		scopeInstance.Activity = initial;

		return processInstance;
	  }

	  public virtual IList<ActivityImpl> InitialActivityStack
	  {
		  get
		  {
			return getInitialActivityStack(initial);
		  }
	  }

	  public virtual IList<ActivityImpl> getInitialActivityStack(ActivityImpl startActivity)
	  {
		  lock (this)
		  {
			IList<ActivityImpl> initialActivityStack = initialActivityStacks[startActivity];
			if (initialActivityStack == null)
			{
			  initialActivityStack = new List<ActivityImpl>();
			  ActivityImpl activity = startActivity;
			  while (activity != null)
			  {
				initialActivityStack.Insert(0, activity);
				activity = activity.ParentActivity;
			  }
			  initialActivityStacks[startActivity] = initialActivityStack;
			}
			return initialActivityStack;
		  }
	  }

	  protected internal virtual InterpretableExecution newProcessInstance(ActivityImpl startActivity)
	  {
		return new ExecutionImpl(startActivity);
	  }

	  public virtual string DiagramResourceName
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual string DeploymentId
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual void addLaneSet(LaneSet newLaneSet)
	  {
		LaneSets.Add(newLaneSet);
	  }

	  public virtual Lane getLaneForId(string id)
	  {
		if (laneSets != null && laneSets.Count > 0)
		{
		  Lane lane;
		  foreach (LaneSet set in laneSets)
		  {
			lane = set.getLaneForId(id);
			if (lane != null)
			{
			  return lane;
			}
		  }
		}
		return null;
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual ActivityImpl getInitial()
	  {
		return initial;
	  }

	  public virtual void setInitial(ActivityImpl initial)
	  {
		this.initial = initial;
	  }

	  public override string ToString()
	  {
		return "ProcessDefinition(" + id + ")";
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual string Key
	  {
		  get
		  {
			return key;
		  }
		  set
		  {
			this.key = value;
		  }
	  }


	  public virtual string Description
	  {
		  get
		  {
			return (string) getProperty("documentation");
		  }
	  }

	  /// <returns> all lane-sets defined on this process-instance. Returns an empty list if none are defined.  </returns>
	  public virtual IList<LaneSet> LaneSets
	  {
		  get
		  {
			if (laneSets == null)
			{
			  laneSets = new List<LaneSet>();
			}
			return laneSets;
		  }
	  }


	  public virtual ParticipantProcess ParticipantProcess
	  {
		  set
		  {
			this.participantProcess = value;
		  }
		  get
		  {
			return participantProcess;
		  }
	  }

	}

}