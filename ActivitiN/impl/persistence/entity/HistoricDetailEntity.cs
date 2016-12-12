using System;

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


	using HistoricDetail = org.activiti.engine.history.HistoricDetail;
	using Context = org.activiti.engine.impl.context.Context;
	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public abstract class HistoricDetailEntity : HistoricDetail, PersistentObject
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal string processInstanceId;
	  protected internal string activityInstanceId;
	  protected internal string taskId;
	  protected internal string executionId;
	  protected internal DateTime time;
	  protected internal string detailType;

	  public virtual object PersistentState
	  {
		  get
		  {
			// details are not updatable so we always provide the same object as the state
			return typeof(HistoricDetailEntity);
		  }
	  }

	  public virtual void delete()
	  {
		DbSqlSession dbSqlSession = Context.CommandContext.DbSqlSession;

		dbSqlSession.delete(this);
	  }

	  // getters and setters //////////////////////////////////////////////////////

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

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
		  set
		  {
			this.activityInstanceId = value;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
		  set
		  {
			this.taskId = value;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
		  set
		  {
			this.executionId = value;
		  }
	  }

	  public virtual DateTime Time
	  {
		  get
		  {
			return time;
		  }
		  set
		  {
			this.time = value;
		  }
	  }

	  public virtual string DetailType
	  {
		  get
		  {
			return detailType;
		  }
		  set
		  {
			this.detailType = value;
		  }
	  }
	}

}