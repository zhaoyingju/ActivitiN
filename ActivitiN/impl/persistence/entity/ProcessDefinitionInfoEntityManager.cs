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
	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;


	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	public class ProcessDefinitionInfoEntityManager : AbstractManager
	{

	  public virtual void insertProcessDefinitionInfo(ProcessDefinitionInfoEntity processDefinitionInfo)
	  {
		DbSqlSession.insert((PersistentObject) processDefinitionInfo);

		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, processDefinitionInfo));
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_INITIALIZED, processDefinitionInfo));
		}
	  }

	  public virtual void updateProcessDefinitionInfo(ProcessDefinitionInfoEntity updatedProcessDefinitionInfo)
	  {
		CommandContext commandContext = Context.CommandContext;
		DbSqlSession dbSqlSession = commandContext.DbSqlSession;
		dbSqlSession.update(updatedProcessDefinitionInfo);

		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, updatedProcessDefinitionInfo));
		}
	  }

	  public virtual void deleteProcessDefinitionInfo(string processDefinitionId)
	  {
		ProcessDefinitionInfoEntity processDefinitionInfo = findProcessDefinitionInfoByProcessDefinitionId(processDefinitionId);
		if (processDefinitionInfo != null)
		{
		  DbSqlSession.delete(processDefinitionInfo);
		  deleteInfoJson(processDefinitionInfo);

		  if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
			  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_DELETED, processDefinitionInfo));
		  }
		}
	  }

	  public virtual void updateInfoJson(string id, sbyte[] json)
	  {
		ProcessDefinitionInfoEntity processDefinitionInfo = DbSqlSession.selectById(typeof(ProcessDefinitionInfoEntity), id);
		if (processDefinitionInfo != null)
		{
		  ByteArrayRef @ref = new ByteArrayRef(processDefinitionInfo.InfoJsonId);
		  @ref.setValue("json", json);

		  if (processDefinitionInfo.InfoJsonId == null)
		  {
			processDefinitionInfo.InfoJsonId = @ref.Id;
			updateProcessDefinitionInfo(processDefinitionInfo);
		  }
		}
	  }

	  public virtual void deleteInfoJson(ProcessDefinitionInfoEntity processDefinitionInfo)
	  {
		if (processDefinitionInfo.InfoJsonId != null)
		{
		  ByteArrayRef @ref = new ByteArrayRef(processDefinitionInfo.InfoJsonId);
		  @ref.delete();
		}
	  }

	  public virtual ProcessDefinitionInfoEntity findProcessDefinitionInfoById(string id)
	  {
		return (ProcessDefinitionInfoEntity) DbSqlSession.selectOne("selectProcessDefinitionInfo", id);
	  }

	  public virtual ProcessDefinitionInfoEntity findProcessDefinitionInfoByProcessDefinitionId(string processDefinitionId)
	  {
		return (ProcessDefinitionInfoEntity) DbSqlSession.selectOne("selectProcessDefinitionInfoByProcessDefinitionId", processDefinitionId);
	  }

	  public virtual sbyte[] findInfoJsonById(string infoJsonId)
	  {
		ByteArrayRef @ref = new ByteArrayRef(infoJsonId);
		return @ref.Bytes;
	  }
	}

}