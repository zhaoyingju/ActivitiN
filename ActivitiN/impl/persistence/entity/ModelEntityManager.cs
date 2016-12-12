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
	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using Model = org.activiti.engine.repository.Model;
	using ModelQuery = org.activiti.engine.repository.ModelQuery;


	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	public class ModelEntityManager : AbstractManager
	{

	  public virtual Model createNewModel()
	  {
		return new ModelEntity();
	  }

	  public virtual void insertModel(Model model)
	  {
		((ModelEntity) model).CreateTime = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		((ModelEntity) model).LastUpdateTime = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		DbSqlSession.insert((PersistentObject) model);

		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, model));
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_INITIALIZED, model));
		}
	  }

	  public virtual void updateModel(ModelEntity updatedModel)
	  {
		CommandContext commandContext = Context.CommandContext;
		updatedModel.LastUpdateTime = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		DbSqlSession dbSqlSession = commandContext.DbSqlSession;
		dbSqlSession.update(updatedModel);

		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, updatedModel));
		}
	  }

	  public virtual void deleteModel(string modelId)
	  {
		ModelEntity model = DbSqlSession.selectById(typeof(ModelEntity), modelId);
		DbSqlSession.delete(model);
		deleteEditorSource(model);
		deleteEditorSourceExtra(model);

		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_DELETED, model));
		}
	  }

	  public virtual void insertEditorSourceForModel(string modelId, sbyte[] modelSource)
	  {
		ModelEntity model = findModelById(modelId);
		if (model != null)
		{
		  ByteArrayRef @ref = new ByteArrayRef(model.EditorSourceValueId);
		  @ref.setValue("source", modelSource);

		  if (model.EditorSourceValueId == null)
		  {
			model.EditorSourceValueId = @ref.Id;
			updateModel(model);
		  }
		}
	  }

	  public virtual void deleteEditorSource(ModelEntity model)
	  {
		if (model.EditorSourceValueId != null)
		{
		  ByteArrayRef @ref = new ByteArrayRef(model.EditorSourceValueId);
		  @ref.delete();
		}
	  }

	  public virtual void deleteEditorSourceExtra(ModelEntity model)
	  {
		if (model.EditorSourceExtraValueId != null)
		{
		  ByteArrayRef @ref = new ByteArrayRef(model.EditorSourceExtraValueId);
		  @ref.delete();
		}
	  }

	  public virtual void insertEditorSourceExtraForModel(string modelId, sbyte[] modelSource)
	  {
		ModelEntity model = findModelById(modelId);
		if (model != null)
		{
		  ByteArrayRef @ref = new ByteArrayRef(model.EditorSourceExtraValueId);
		  @ref.setValue("source-extra", modelSource);

		  if (model.EditorSourceExtraValueId == null)
		  {
			model.EditorSourceExtraValueId = @ref.Id;
			updateModel(model);
		  }
		}
	  }

	  public virtual ModelQuery createNewModelQuery()
	  {
		return new ModelQueryImpl(Context.ProcessEngineConfiguration.CommandExecutor);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.repository.Model> findModelsByQueryCriteria(org.activiti.engine.impl.ModelQueryImpl query, org.activiti.engine.impl.Page page)
	  public virtual IList<Model> findModelsByQueryCriteria(ModelQueryImpl query, Page page)
	  {
		return DbSqlSession.selectList("selectModelsByQueryCriteria", query, page);
	  }

	  public virtual long findModelCountByQueryCriteria(ModelQueryImpl query)
	  {
		return (long?) DbSqlSession.selectOne("selectModelCountByQueryCriteria", query);
	  }

	  public virtual ModelEntity findModelById(string modelId)
	  {
		return (ModelEntity) DbSqlSession.selectOne("selectModel", modelId);
	  }

	  public virtual sbyte[] findEditorSourceByModelId(string modelId)
	  {
		ModelEntity model = findModelById(modelId);
		if (model == null || model.EditorSourceValueId == null)
		{
		  return null;
		}

		ByteArrayRef @ref = new ByteArrayRef(model.EditorSourceValueId);
		return @ref.Bytes;
	  }

	  public virtual sbyte[] findEditorSourceExtraByModelId(string modelId)
	  {
		ModelEntity model = findModelById(modelId);
		if (model == null || model.EditorSourceExtraValueId == null)
		{
		  return null;
		}

		ByteArrayRef @ref = new ByteArrayRef(model.EditorSourceExtraValueId);
		return @ref.Bytes;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.repository.Model> findModelsByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<Model> findModelsByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbSqlSession.selectListWithRawParameter("selectModelByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findModelCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbSqlSession.selectOne("selectModelCountByNativeQuery", parameterMap);
	  }
	}

}