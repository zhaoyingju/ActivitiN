using System;

/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace org.activiti.engine.impl.persistence.entity
{

	using Context = org.activiti.engine.impl.context.Context;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;


	/// <summary>
	/// @author Christian Stettler
	/// </summary>
	[Serializable]
	public abstract class HistoricScopeInstanceEntity : PersistentObject
	{
		public abstract object PersistentState {get;}

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal string processInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string processDefinitionName;
	  protected internal int? processDefinitionVersion;
	  protected internal string deploymentId;
	  protected internal DateTime startTime;
	  protected internal DateTime endTime;
	  protected internal long? durationInMillis;
	  protected internal string deleteReason;

	  public virtual void markEnded(string deleteReason)
	  {
		this.deleteReason = deleteReason;
		this.endTime = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		this.durationInMillis = endTime.Ticks - startTime.Ticks;
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }
	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
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
	  public virtual DateTime StartTime
	  {
		  get
		  {
			return startTime;
		  }
		  set
		  {
			this.startTime = value;
		  }
	  }
	  public virtual DateTime EndTime
	  {
		  get
		  {
			return endTime;
		  }
		  set
		  {
			this.endTime = value;
		  }
	  }
	  public virtual long? DurationInMillis
	  {
		  get
		  {
			return durationInMillis;
		  }
		  set
		  {
			this.durationInMillis = value;
		  }
	  }
	  public virtual string Id
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
	}

}