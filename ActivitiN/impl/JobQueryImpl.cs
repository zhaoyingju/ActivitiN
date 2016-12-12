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

namespace org.activiti.engine.impl
{


	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using Job = org.activiti.engine.runtime.Job;
	using JobQuery = org.activiti.engine.runtime.JobQuery;


	/// <summary>
	/// @author Joram Barrez
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// </summary>
	[Serializable]
	public class JobQueryImpl : AbstractQuery<JobQuery, Job>, JobQuery
	{

	  private const long serialVersionUID = 1L;
	  protected internal string id;
	  protected internal string processInstanceId_Renamed;
	  protected internal string executionId_Renamed;
	  protected internal string processDefinitionId_Renamed;
	  protected internal bool retriesLeft;
	  protected internal bool executable_Renamed;
	  protected internal bool onlyTimers;
	  protected internal bool onlyMessages;
	  protected internal DateTime duedateHigherThan_Renamed;
	  protected internal DateTime duedateLowerThan_Renamed;
	  protected internal DateTime duedateHigherThanOrEqual;
	  protected internal DateTime duedateLowerThanOrEqual;
	  protected internal bool withException_Renamed;
	  protected internal string exceptionMessage_Renamed;
	  protected internal string tenantId;
	  protected internal string tenantIdLike;
	  protected internal bool withoutTenantId;
	  protected internal bool noRetriesLeft_Renamed;


	  public JobQueryImpl()
	  {
	  }

	  public JobQueryImpl(CommandContext commandContext) : base(commandContext)
	  {
	  }

	  public JobQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual JobQuery jobId(string jobId)
	  {
		if (jobId == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided job id is null");
		}
		this.id = jobId;
		return this;
	  }

	  public virtual JobQueryImpl processInstanceId(string processInstanceId)
	  {
		if (processInstanceId == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided process instance id is null");
		}
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual JobQueryImpl processDefinitionId(string processDefinitionId)
	  {
		if (processDefinitionId == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided process definition id is null");
		}
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual JobQueryImpl executionId(string executionId)
	  {
		if (executionId == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided execution id is null");
		}
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual JobQuery withRetriesLeft()
	  {
		retriesLeft = true;
		return this;
	  }

	  public virtual JobQuery executable()
	  {
		executable_Renamed = true;
		return this;
	  }

	  public virtual JobQuery timers()
	  {
		if (onlyMessages)
		{
		  throw new ActivitiIllegalArgumentException("Cannot combine onlyTimers() with onlyMessages() in the same query");
		}
		this.onlyTimers = true;
		return this;
	  }

	  public virtual JobQuery messages()
	  {
		if (onlyTimers)
		{
		  throw new ActivitiIllegalArgumentException("Cannot combine onlyTimers() with onlyMessages() in the same query");
		}
		this.onlyMessages = true;
		return this;
	  }

	  public virtual JobQuery duedateHigherThan(DateTime date)
	  {
		if (date == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided date is null");
		}
		this.duedateHigherThan_Renamed = date;
		return this;
	  }

	  public virtual JobQuery duedateLowerThan(DateTime date)
	  {
		if (date == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided date is null");
		}
		this.duedateLowerThan_Renamed = date;
		return this;
	  }

	  public virtual JobQuery duedateHigherThen(DateTime date)
	  {
		return duedateHigherThan(date);
	  }

	  public virtual JobQuery duedateHigherThenOrEquals(DateTime date)
	  {
		if (date == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided date is null");
		}
		this.duedateHigherThanOrEqual = date;
		return this;
	  }

	  public virtual JobQuery duedateLowerThen(DateTime date)
	  {
		return duedateLowerThan(date);
	  }

	  public virtual JobQuery duedateLowerThenOrEquals(DateTime date)
	  {
		if (date == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided date is null");
		}
		this.duedateLowerThanOrEqual = date;
		return this;
	  }

	  public virtual JobQuery noRetriesLeft()
	  {
		 noRetriesLeft_Renamed = true;
		 return this;
	  }

	  public virtual JobQuery withException()
	  {
		this.withException_Renamed = true;
		return this;
	  }

	  public virtual JobQuery exceptionMessage(string exceptionMessage)
	  {
		if (exceptionMessage == null)
		{
		  throw new ActivitiIllegalArgumentException("Provided exception message is null");
		}
		this.exceptionMessage_Renamed = exceptionMessage;
		return this;
	  }

	  public virtual JobQuery jobTenantId(string tenantId)
	  {
		  if (tenantId == null)
		  {
			  throw new ActivitiIllegalArgumentException("job is null");
		  }
		  this.tenantId = tenantId;
		  return this;
	  }

	  public virtual JobQuery jobTenantIdLike(string tenantIdLike)
	  {
		  if (tenantIdLike == null)
		  {
			  throw new ActivitiIllegalArgumentException("job is null");
		  }
		  this.tenantIdLike = tenantIdLike;
		  return this;
	  }

	  public virtual JobQuery jobWithoutTenantId()
	  {
		  this.withoutTenantId = true;
		  return this;
	  }

	  //sorting //////////////////////////////////////////

	  public virtual JobQuery orderByJobDuedate()
	  {
		return orderBy(JobQueryProperty.DUEDATE);
	  }

	  public virtual JobQuery orderByExecutionId()
	  {
		return orderBy(JobQueryProperty.EXECUTION_ID);
	  }

	  public virtual JobQuery orderByJobId()
	  {
		return orderBy(JobQueryProperty.JOB_ID);
	  }

	  public virtual JobQuery orderByProcessInstanceId()
	  {
		return orderBy(JobQueryProperty.PROCESS_INSTANCE_ID);
	  }

	  public virtual JobQuery orderByJobRetries()
	  {
		return orderBy(JobQueryProperty.RETRIES);
	  }

	  public virtual JobQuery orderByTenantId()
	  {
		   return orderBy(JobQueryProperty.TENANT_ID);
	  }

	  //results //////////////////////////////////////////

	  public virtual long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.JobEntityManager.findJobCountByQueryCriteria(this);
	  }

	  public virtual IList<Job> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		return commandContext.JobEntityManager.findJobsByQueryCriteria(this, page);
	  }

	  //getters //////////////////////////////////////////

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Renamed;
		  }
	  }
	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Renamed;
		  }
	  }
	  public virtual bool RetriesLeft
	  {
		  get
		  {
			return retriesLeft;
		  }
	  }
	  public virtual bool Executable
	  {
		  get
		  {
			return executable_Renamed;
		  }
	  }
	  public virtual DateTime Now
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.Clock.CurrentTime;
		  }
	  }
	  public virtual bool WithException
	  {
		  get
		  {
			return withException_Renamed;
		  }
	  }
	  public virtual string ExceptionMessage
	  {
		  get
		  {
			return exceptionMessage_Renamed;
		  }
	  }
		public virtual string TenantId
		{
			get
			{
				return tenantId;
			}
		}
		public virtual string TenantIdLike
		{
			get
			{
				return tenantIdLike;
			}
		}
		public virtual bool WithoutTenantId
		{
			get
			{
				return withoutTenantId;
			}
		}

	}

}