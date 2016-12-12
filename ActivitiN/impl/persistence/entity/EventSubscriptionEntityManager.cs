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




	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public class EventSubscriptionEntityManager : AbstractManager
	{

	  /// <summary>
	  /// keep track of subscriptions created in the current command </summary>
	  protected internal IList<SignalEventSubscriptionEntity> createdSignalSubscriptions = new List<SignalEventSubscriptionEntity>();

	  public virtual void insert(EventSubscriptionEntity persistentObject)
	  {
		base.insert(persistentObject);
		if (persistentObject is SignalEventSubscriptionEntity)
		{
		  createdSignalSubscriptions.Add((SignalEventSubscriptionEntity)persistentObject);
		}
	  }

	  public virtual void deleteEventSubscription(EventSubscriptionEntity persistentObject)
	  {
		DbSqlSession.delete(persistentObject);
		if (persistentObject is SignalEventSubscriptionEntity)
		{
		  createdSignalSubscriptions.Remove(persistentObject);
		}
	  }

	  public virtual void deleteEventSubscriptionsForProcessDefinition(string processDefinitionId)
	  {
		  DbSqlSession.delete("deleteEventSubscriptionsForProcessDefinition", processDefinitionId);
	  }

	  public virtual EventSubscriptionEntity findEventSubscriptionbyId(string id)
	  {
		return (EventSubscriptionEntity) DbSqlSession.selectOne("selectEventSubscription", id);
	  }

	  public virtual long findEventSubscriptionCountByQueryCriteria(EventSubscriptionQueryImpl eventSubscriptionQueryImpl)
	  {
		const string query = "selectEventSubscriptionCountByQueryCriteria";
		return (long?) DbSqlSession.selectOne(query, eventSubscriptionQueryImpl);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<EventSubscriptionEntity> findEventSubscriptionsByQueryCriteria(org.activiti.engine.impl.EventSubscriptionQueryImpl eventSubscriptionQueryImpl, org.activiti.engine.impl.Page page)
	  public virtual IList<EventSubscriptionEntity> findEventSubscriptionsByQueryCriteria(EventSubscriptionQueryImpl eventSubscriptionQueryImpl, Page page)
	  {
		const string query = "selectEventSubscriptionByQueryCriteria";
		return DbSqlSession.selectList(query, eventSubscriptionQueryImpl, page);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<SignalEventSubscriptionEntity> findSignalEventSubscriptionsByEventName(String eventName, String tenantId)
	  public virtual IList<SignalEventSubscriptionEntity> findSignalEventSubscriptionsByEventName(string eventName, string tenantId)
	  {
		const string query = "selectSignalEventSubscriptionsByEventName";

		Set<SignalEventSubscriptionEntity> selectList = null;
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["eventName"] = eventName;
		if (tenantId != null && !tenantId.Equals(ProcessEngineConfiguration.NO_TENANT_ID))
		{
			@params["tenantId"] = tenantId;
		  selectList = new HashSet<SignalEventSubscriptionEntity>(DbSqlSession.selectList(query, @params));
		}
		else
		{
			selectList = new HashSet<SignalEventSubscriptionEntity>(DbSqlSession.selectList(query, @params));
		}

		// add events created in this command (not visible yet in query)
		foreach (SignalEventSubscriptionEntity entity in createdSignalSubscriptions)
		{
		  if (eventName.Equals(entity.EventName))
		  {
			selectList.add(entity);
		  }
		}

		return new List<SignalEventSubscriptionEntity>(selectList);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<SignalEventSubscriptionEntity> findSignalEventSubscriptionsByProcessInstanceAndEventName(String processInstanceId, String eventName)
	  public virtual IList<SignalEventSubscriptionEntity> findSignalEventSubscriptionsByProcessInstanceAndEventName(string processInstanceId, string eventName)
	  {
		const string query = "selectSignalEventSubscriptionsByProcessInstanceAndEventName";
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["processInstanceId"] = processInstanceId;
		@params["eventName"] = eventName;
		Set<SignalEventSubscriptionEntity> selectList = new HashSet<SignalEventSubscriptionEntity>(DbSqlSession.selectList(query, @params));

		// add events created in this command (not visible yet in query)
		foreach (SignalEventSubscriptionEntity entity in createdSignalSubscriptions)
		{
		  if (processInstanceId.Equals(entity.ProcessInstanceId) && eventName.Equals(entity.EventName))
		  {
			selectList.add(entity);
		  }
		}

		return new List<SignalEventSubscriptionEntity>(selectList);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<SignalEventSubscriptionEntity> findSignalEventSubscriptionsByExecution(String executionId)
	  public virtual IList<SignalEventSubscriptionEntity> findSignalEventSubscriptionsByExecution(string executionId)
	  {
		const string query = "selectSignalEventSubscriptionsByExecution";
		Set<SignalEventSubscriptionEntity> selectList = new HashSet<SignalEventSubscriptionEntity>(DbSqlSession.selectList(query, executionId));

		// add events created in this command (not visible yet in query)
		foreach (SignalEventSubscriptionEntity entity in createdSignalSubscriptions)
		{
		  if (executionId.Equals(entity.ExecutionId))
		  {
			selectList.add((SignalEventSubscriptionEntity) entity);
		  }
		}

		return new List<SignalEventSubscriptionEntity>(selectList);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<SignalEventSubscriptionEntity> findSignalEventSubscriptionsByNameAndExecution(String name, String executionId)
	  public virtual IList<SignalEventSubscriptionEntity> findSignalEventSubscriptionsByNameAndExecution(string name, string executionId)
	  {
		const string query = "selectSignalEventSubscriptionsByNameAndExecution";
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["executionId"] = executionId;
		@params["eventName"] = name;
		Set<SignalEventSubscriptionEntity> selectList = new HashSet<SignalEventSubscriptionEntity>(DbSqlSession.selectList(query, @params));

		// add events created in this command (not visible yet in query)
		foreach (SignalEventSubscriptionEntity entity in createdSignalSubscriptions)
		{
		  if (executionId.Equals(entity.ExecutionId) && name.Equals(entity.EventName))
		  {
			selectList.add((SignalEventSubscriptionEntity) entity);
		  }
		}

		return new List<SignalEventSubscriptionEntity>(selectList);
	  }

	  public virtual IList<EventSubscriptionEntity> findEventSubscriptionsByExecutionAndType(string executionId, string type)
	  {
		const string query = "selectEventSubscriptionsByExecutionAndType";
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["executionId"] = executionId;
		@params["eventType"] = type;
		return DbSqlSession.selectList(query, @params);
	  }

	  public virtual IList<EventSubscriptionEntity> findEventSubscriptionsByExecution(string executionId)
	  {
		const string query = "selectEventSubscriptionsByExecution";
		return DbSqlSession.selectList(query, executionId);
	  }

	  public virtual IList<EventSubscriptionEntity> findEventSubscriptions(string executionId, string type, string activityId)
	  {
		const string query = "selectEventSubscriptionsByExecutionTypeAndActivity";
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["executionId"] = executionId;
		@params["eventType"] = type;
		@params["activityId"] = activityId;
		return DbSqlSession.selectList(query, @params);
	  }

	  public virtual IList<EventSubscriptionEntity> findEventSubscriptionsByConfiguration(string type, string configuration, string tenantId)
	  {
		const string query = "selectEventSubscriptionsByConfiguration";
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["eventType"] = type;
		@params["configuration"] = configuration;
		if (tenantId != null && !tenantId.Equals(ProcessEngineConfiguration.NO_TENANT_ID))
		{
			@params["tenantId"] = tenantId;
		}
		return DbSqlSession.selectList(query, @params);
	  }

	  public virtual IList<EventSubscriptionEntity> findEventSubscriptionsByTypeAndProcessDefinitionId(string type, string processDefinitionId, string tenantId)
	  {
		const string query = "selectEventSubscriptionsByTypeAndProcessDefinitionId";
		IDictionary<string, string> @params = new Dictionary<string, string>();
		if (type != null)
		{
		  @params["eventType"] = type;
		}
		@params["processDefinitionId"] = processDefinitionId;
		if (tenantId != null && !tenantId.Equals(ProcessEngineConfiguration.NO_TENANT_ID))
		{
		  @params["tenantId"] = tenantId;
		}
		return DbSqlSession.selectList(query, @params);
	  }

	  public virtual IList<EventSubscriptionEntity> findEventSubscriptionsByName(string type, string eventName, string tenantId)
	  {
		const string query = "selectEventSubscriptionsByName";
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["eventType"] = type;
		@params["eventName"] = eventName;
		if (tenantId != null && !tenantId.Equals(ProcessEngineConfiguration.NO_TENANT_ID))
		{
			@params["tenantId"] = tenantId;
		}
		return DbSqlSession.selectList(query, @params);
	  }

	  public virtual IList<EventSubscriptionEntity> findEventSubscriptionsByNameAndExecution(string type, string eventName, string executionId)
	  {
		const string query = "selectEventSubscriptionsByNameAndExecution";
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["eventType"] = type;
		@params["eventName"] = eventName;
		@params["executionId"] = executionId;
		return DbSqlSession.selectList(query, @params);
	  }

	  public virtual MessageEventSubscriptionEntity findMessageStartEventSubscriptionByName(string messageName, string tenantId)
	  {
		  IDictionary<string, string> @params = new Dictionary<string, string>();
		  @params["eventName"] = messageName;
		   if (tenantId != null && !tenantId.Equals(ProcessEngineConfiguration.NO_TENANT_ID))
		   {
			 @params["tenantId"] = tenantId;
		   }
		MessageEventSubscriptionEntity entity = (MessageEventSubscriptionEntity) DbSqlSession.selectOne("selectMessageStartEventSubscriptionByName", @params);
		return entity;
	  }

	  public virtual void updateEventSubscriptionTenantId(string oldTenantId, string newTenantId)
	  {
		  IDictionary<string, string> @params = new Dictionary<string, string>();
		  @params["oldTenantId"] = oldTenantId;
		  @params["newTenantId"] = newTenantId;
		  DbSqlSession.update("updateTenantIdOfEventSubscriptions", @params);
	  }

	}

}