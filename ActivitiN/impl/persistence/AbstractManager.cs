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

namespace org.activiti.engine.impl.persistence
{

	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.activiti.engine.impl.context.Context;
	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using HistoryManager = org.activiti.engine.impl.history.HistoryManager;
	using Session = org.activiti.engine.impl.interceptor.Session;
	using AttachmentEntityManager = org.activiti.engine.impl.persistence.entity.AttachmentEntityManager;
	using ByteArrayEntityManager = org.activiti.engine.impl.persistence.entity.ByteArrayEntityManager;
	using DeploymentEntityManager = org.activiti.engine.impl.persistence.entity.DeploymentEntityManager;
	using EventSubscriptionEntityManager = org.activiti.engine.impl.persistence.entity.EventSubscriptionEntityManager;
	using ExecutionEntityManager = org.activiti.engine.impl.persistence.entity.ExecutionEntityManager;
	using GroupIdentityManager = org.activiti.engine.impl.persistence.entity.GroupIdentityManager;
	using HistoricActivityInstanceEntityManager = org.activiti.engine.impl.persistence.entity.HistoricActivityInstanceEntityManager;
	using HistoricDetailEntityManager = org.activiti.engine.impl.persistence.entity.HistoricDetailEntityManager;
	using HistoricIdentityLinkEntityManager = org.activiti.engine.impl.persistence.entity.HistoricIdentityLinkEntityManager;
	using HistoricProcessInstanceEntityManager = org.activiti.engine.impl.persistence.entity.HistoricProcessInstanceEntityManager;
	using HistoricTaskInstanceEntityManager = org.activiti.engine.impl.persistence.entity.HistoricTaskInstanceEntityManager;
	using HistoricVariableInstanceEntityManager = org.activiti.engine.impl.persistence.entity.HistoricVariableInstanceEntityManager;
	using IdentityInfoEntityManager = org.activiti.engine.impl.persistence.entity.IdentityInfoEntityManager;
	using IdentityLinkEntityManager = org.activiti.engine.impl.persistence.entity.IdentityLinkEntityManager;
	using MembershipIdentityManager = org.activiti.engine.impl.persistence.entity.MembershipIdentityManager;
	using ModelEntityManager = org.activiti.engine.impl.persistence.entity.ModelEntityManager;
	using ProcessDefinitionEntityManager = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntityManager;
	using ProcessDefinitionInfoEntityManager = org.activiti.engine.impl.persistence.entity.ProcessDefinitionInfoEntityManager;
	using ResourceEntityManager = org.activiti.engine.impl.persistence.entity.ResourceEntityManager;
	using TaskEntityManager = org.activiti.engine.impl.persistence.entity.TaskEntityManager;
	using UserIdentityManager = org.activiti.engine.impl.persistence.entity.UserIdentityManager;
	using VariableInstanceEntityManager = org.activiti.engine.impl.persistence.entity.VariableInstanceEntityManager;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public abstract class AbstractManager : Session
	{

	  public virtual void insert(PersistentObject persistentObject)
	  {
		DbSqlSession.insert(persistentObject);
	  }

	  public virtual void delete(PersistentObject persistentObject)
	  {
		DbSqlSession.delete(persistentObject);
	  }

	  protected internal virtual DbSqlSession DbSqlSession
	  {
		  get
		  {
			return getSession(typeof(DbSqlSession));
		  }
	  }

	  protected internal virtual T getSession<T>(Type sessionClass)
	  {
		return Context.CommandContext.getSession(sessionClass);
	  }

	  protected internal virtual DeploymentEntityManager DeploymentManager
	  {
		  get
		  {
			return getSession(typeof(DeploymentEntityManager));
		  }
	  }

	  protected internal virtual ResourceEntityManager ResourceManager
	  {
		  get
		  {
			return getSession(typeof(ResourceEntityManager));
		  }
	  }

	  protected internal virtual ByteArrayEntityManager ByteArrayManager
	  {
		  get
		  {
			return getSession(typeof(ByteArrayEntityManager));
		  }
	  }

	  protected internal virtual ProcessDefinitionEntityManager ProcessDefinitionManager
	  {
		  get
		  {
			return getSession(typeof(ProcessDefinitionEntityManager));
		  }
	  }

	  protected internal virtual ProcessDefinitionInfoEntityManager ProcessDefinitionInfoManager
	  {
		  get
		  {
			return getSession(typeof(ProcessDefinitionInfoEntityManager));
		  }
	  }

	  protected internal virtual ModelEntityManager ModelManager
	  {
		  get
		  {
			return getSession(typeof(ModelEntityManager));
		  }
	  }

	  protected internal virtual ExecutionEntityManager ProcessInstanceManager
	  {
		  get
		  {
			return getSession(typeof(ExecutionEntityManager));
		  }
	  }

	  protected internal virtual TaskEntityManager TaskManager
	  {
		  get
		  {
			return getSession(typeof(TaskEntityManager));
		  }
	  }

	  protected internal virtual IdentityLinkEntityManager IdentityLinkManager
	  {
		  get
		  {
			return getSession(typeof(IdentityLinkEntityManager));
		  }
	  }

	  protected internal virtual EventSubscriptionEntityManager EventSubscriptionManager
	  {
		  get
		  {
			  return (getSession(typeof(EventSubscriptionEntityManager)));
		  }
	  }

	  protected internal virtual VariableInstanceEntityManager VariableInstanceManager
	  {
		  get
		  {
			return getSession(typeof(VariableInstanceEntityManager));
		  }
	  }

	  protected internal virtual HistoricProcessInstanceEntityManager HistoricProcessInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricProcessInstanceEntityManager));
		  }
	  }

	  protected internal virtual HistoricDetailEntityManager HistoricDetailManager
	  {
		  get
		  {
			return getSession(typeof(HistoricDetailEntityManager));
		  }
	  }

	  protected internal virtual HistoricActivityInstanceEntityManager HistoricActivityInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricActivityInstanceEntityManager));
		  }
	  }

	  protected internal virtual HistoricVariableInstanceEntityManager HistoricVariableInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricVariableInstanceEntityManager));
		  }
	  }

	  protected internal virtual HistoricTaskInstanceEntityManager HistoricTaskInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricTaskInstanceEntityManager));
		  }
	  }

	  protected internal virtual HistoricIdentityLinkEntityManager HistoricIdentityLinkEntityManager
	  {
		  get
		  {
			return getSession(typeof(HistoricIdentityLinkEntityManager));
		  }
	  }

	  protected internal virtual UserIdentityManager UserIdentityManager
	  {
		  get
		  {
			return getSession(typeof(UserIdentityManager));
		  }
	  }

	  protected internal virtual GroupIdentityManager GroupIdentityManager
	  {
		  get
		  {
			return getSession(typeof(GroupIdentityManager));
		  }
	  }

	  protected internal virtual IdentityInfoEntityManager IdentityInfoManager
	  {
		  get
		  {
			return getSession(typeof(IdentityInfoEntityManager));
		  }
	  }

	  protected internal virtual MembershipIdentityManager MembershipIdentityManager
	  {
		  get
		  {
			return getSession(typeof(MembershipIdentityManager));
		  }
	  }

	  protected internal virtual AttachmentEntityManager AttachmentManager
	  {
		  get
		  {
			return getSession(typeof(AttachmentEntityManager));
		  }
	  }

	  protected internal virtual HistoryManager HistoryManager
	  {
		  get
		  {
			return getSession(typeof(HistoryManager));
		  }
	  }

	  protected internal virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
	  {
		  get
		  {
			  return Context.ProcessEngineConfiguration;
		  }
	  }

	  public virtual void close()
	  {
	  }

	  public virtual void flush()
	  {
	  }
	}

}