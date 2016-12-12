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
namespace org.activiti.engine.impl.persistence.entity
{


	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using Context = org.activiti.engine.impl.context.Context;
	using BulkDeleteable = org.activiti.engine.impl.db.BulkDeleteable;
	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using JobHandler = org.activiti.engine.impl.jobexecutor.JobHandler;
	using Job = org.activiti.engine.runtime.Job;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	/// <summary>
	/// Stub of the common parts of a Job. You will normally work with a subclass of
	/// JobEntity, such as <seealso cref="TimerEntity"/> or <seealso cref="MessageEntity"/>.
	/// 
	/// @author Tom Baeyens
	/// @author Nick Burch
	/// @author Dave Syer
	/// @author Frederik Heremans
	/// </summary>
	[Serializable]
	public abstract class JobEntity : Job, PersistentObject, HasRevision, BulkDeleteable
	{

	  public const bool DEFAULT_EXCLUSIVE = true;
	  public const int DEFAULT_RETRIES = 3;
	  private const int MAX_EXCEPTION_MESSAGE_LENGTH = 255;

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;

	  protected internal DateTime duedate;

	  protected internal string lockOwner = null;
	  protected internal DateTime lockExpirationTime = null;

	  protected internal string executionId = null;
	  protected internal string processInstanceId = null;
	  protected internal string processDefinitionId = null;

	  protected internal bool isExclusive = DEFAULT_EXCLUSIVE;

	  protected internal int retries = DEFAULT_RETRIES;

	  protected internal string jobHandlerType = null;
	  protected internal string jobHandlerConfiguration = null;

	  protected internal readonly ByteArrayRef exceptionByteArrayRef = new ByteArrayRef();

	  protected internal string exceptionMessage;

	  protected internal string tenantId = ProcessEngineConfiguration.NO_TENANT_ID;
	  protected internal string jobType;

	  public virtual void execute(CommandContext commandContext)
	  {
		ExecutionEntity execution = null;
		if (executionId != null)
		{
		  execution = commandContext.ExecutionEntityManager.findExecutionById(executionId);
		}

		IDictionary<string, JobHandler> jobHandlers = Context.ProcessEngineConfiguration.JobHandlers;
		JobHandler jobHandler = jobHandlers[jobHandlerType];
		jobHandler.execute(this, jobHandlerConfiguration, execution, commandContext);
	  }

	  public virtual void insert()
	  {
		Context.CommandContext.DbSqlSession.insert(this);

		// add link to execution
		if (executionId != null)
		{
		  ExecutionEntity execution = Context.CommandContext.ExecutionEntityManager.findExecutionById(executionId);
		  execution.addJob(this);

		  // Inherit tenant if (if applicable)
		  if (execution.TenantId != null)
		  {
			  TenantId = execution.TenantId;
		  }
		}

		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, this));
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_INITIALIZED, this));
		}
	  }

	  public virtual void delete()
	  {
		Context.CommandContext.DbSqlSession.delete(this);

		// Also delete the job's exception byte array
		exceptionByteArrayRef.delete();

		// remove link to execution
		if (executionId != null)
		{
		  ExecutionEntity execution = Context.CommandContext.ExecutionEntityManager.findExecutionById(executionId);
		  execution.removeJob(this);
		}

		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_DELETED, this));
		}
	  }

	  public virtual ExecutionEntity Execution
	  {
		  set
		  {
			executionId = value.Id;
			processInstanceId = value.ProcessInstanceId;
			processDefinitionId = value.ProcessDefinitionId;
			value.addJob(this);
		  }
	  }

	  public virtual string ExceptionStacktrace
	  {
		  get
		  {
			sbyte[] bytes = exceptionByteArrayRef.Bytes;
			if (bytes == null)
			{
			  return null;
			}
			try
			{
			  return StringHelperClass.NewString(bytes, "UTF-8");
			}
			catch (UnsupportedEncodingException)
			{
			  throw new ActivitiException("UTF-8 is not a supported encoding");
			}
		  }
		  set
		  {
			exceptionByteArrayRef.setValue("stacktrace", getUtf8Bytes(value));
		  }
	  }


	  private sbyte[] getUtf8Bytes(string str)
	  {
		if (str == null)
		{
		  return null;
		}
		try
		{
		  return str.GetBytes("UTF-8");
		}
		catch (UnsupportedEncodingException)
		{
		  throw new ActivitiException("UTF-8 is not a supported encoding");
		}
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["lockOwner"] = lockOwner;
			persistentState["lockExpirationTime"] = lockExpirationTime;
			persistentState["retries"] = retries;
			persistentState["duedate"] = duedate;
			persistentState["exceptionMessage"] = exceptionMessage;
			persistentState["exceptionByteArrayId"] = exceptionByteArrayRef.Id;
			return persistentState;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
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
	  public virtual DateTime Duedate
	  {
		  get
		  {
			return duedate;
		  }
		  set
		  {
			this.duedate = value;
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
	  public virtual int Retries
	  {
		  get
		  {
			return retries;
		  }
		  set
		  {
			this.retries = value;
		  }
	  }
	  public virtual string LockOwner
	  {
		  get
		  {
			return lockOwner;
		  }
		  set
		  {
			this.lockOwner = value;
		  }
	  }
	  public virtual DateTime LockExpirationTime
	  {
		  get
		  {
			return lockExpirationTime;
		  }
		  set
		  {
			this.lockExpirationTime = value;
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
	  public virtual string JobHandlerType
	  {
		  get
		  {
			return jobHandlerType;
		  }
		  set
		  {
			this.jobHandlerType = value;
		  }
	  }
	  public virtual string JobHandlerConfiguration
	  {
		  get
		  {
			return jobHandlerConfiguration;
		  }
		  set
		  {
			this.jobHandlerConfiguration = value;
		  }
	  }
	  public virtual string ExceptionMessage
	  {
		  get
		  {
			return exceptionMessage;
		  }
		  set
		  {
			this.exceptionMessage = StringUtils.abbreviate(value, MAX_EXCEPTION_MESSAGE_LENGTH);
		  }
	  }
	  public virtual string JobType
	  {
		  get
		  {
			return jobType;
		  }
		  set
		  {
			this.jobType = value;
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

	  // common methods  //////////////////////////////////////////////////////////

		public override string ToString()
		{
		return "JobEntity [id=" + id + "]";
		}

	}

}