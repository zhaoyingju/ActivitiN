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
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using Comment = org.activiti.engine.task.Comment;
	using Event = org.activiti.engine.task.Event;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class CommentEntityManager : AbstractManager
	{

	  public override void delete(PersistentObject persistentObject)
	  {
		checkHistoryEnabled();
		base.delete(persistentObject);

		Comment comment = (Comment) persistentObject;
		if (ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			// Forced to fetch the process-instance to associate the right process definition
			string processDefinitionId = null;
			string processInstanceId = comment.ProcessInstanceId;
			if (comment.ProcessInstanceId != null)
			{
				ExecutionEntity process = ProcessInstanceManager.findExecutionById(comment.ProcessInstanceId);
				if (process != null)
				{
					processDefinitionId = process.ProcessDefinitionId;
				}
			}
			ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_DELETED, persistentObject, processInstanceId, processInstanceId, processDefinitionId));
		}
	  }

	  public override void insert(PersistentObject persistentObject)
	  {
		checkHistoryEnabled();
		base.insert(persistentObject);

		Comment comment = (Comment) persistentObject;
		if (ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			// Forced to fetch the process-instance to associate the right process definition
			string processDefinitionId = null;
			string processInstanceId = comment.ProcessInstanceId;
			if (comment.ProcessInstanceId != null)
			{
				ExecutionEntity process = ProcessInstanceManager.findExecutionById(comment.ProcessInstanceId);
				if (process != null)
				{
					processDefinitionId = process.ProcessDefinitionId;
				}
			}
			ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_CREATED, persistentObject, processInstanceId, processInstanceId, processDefinitionId));
			ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_INITIALIZED, persistentObject, processInstanceId, processInstanceId, processDefinitionId));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.task.Comment> findCommentsByTaskId(String taskId)
	  public virtual IList<Comment> findCommentsByTaskId(string taskId)
	  {
		checkHistoryEnabled();
		return DbSqlSession.selectList("selectCommentsByTaskId", taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.task.Comment> findCommentsByTaskIdAndType(String taskId, String type)
	  public virtual IList<Comment> findCommentsByTaskIdAndType(string taskId, string type)
	  {
		checkHistoryEnabled();
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskId"] = taskId;
		@params["type"] = type;
		return DbSqlSession.selectListWithRawParameter("selectCommentsByTaskIdAndType", @params, 0, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.task.Comment> findCommentsByType(String type)
	  public virtual IList<Comment> findCommentsByType(string type)
	  {
		checkHistoryEnabled();
		return DbSqlSession.selectList("selectCommentsByType", type);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.task.Event> findEventsByTaskId(String taskId)
	  public virtual IList<Event> findEventsByTaskId(string taskId)
	  {
		checkHistoryEnabled();
		return DbSqlSession.selectList("selectEventsByTaskId", taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.task.Event> findEventsByProcessInstanceId(String processInstanceId)
	  public virtual IList<Event> findEventsByProcessInstanceId(string processInstanceId)
	  {
		checkHistoryEnabled();
		return DbSqlSession.selectList("selectEventsByProcessInstanceId", processInstanceId);
	  }

	  public virtual void deleteCommentsByTaskId(string taskId)
	  {
		checkHistoryEnabled();
		DbSqlSession.delete("deleteCommentsByTaskId", taskId);
	  }

	  public virtual void deleteCommentsByProcessInstanceId(string processInstanceId)
	  {
		checkHistoryEnabled();
		DbSqlSession.delete("deleteCommentsByProcessInstanceId", processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.task.Comment> findCommentsByProcessInstanceId(String processInstanceId)
	  public virtual IList<Comment> findCommentsByProcessInstanceId(string processInstanceId)
	  {
		checkHistoryEnabled();
		return DbSqlSession.selectList("selectCommentsByProcessInstanceId", processInstanceId);
	  }

	  public virtual IList<Comment> findCommentsByProcessInstanceId(string processInstanceId, string type)
	  {
		checkHistoryEnabled();
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["processInstanceId"] = processInstanceId;
		@params["type"] = type;
		return DbSqlSession.selectListWithRawParameter("selectCommentsByProcessInstanceIdAndType", @params, 0, int.MaxValue);
	  }

	  public virtual Comment findComment(string commentId)
	  {
		return DbSqlSession.selectById(typeof(CommentEntity), commentId);
	  }

	  public virtual Event findEvent(string commentId)
	  {
		return DbSqlSession.selectById(typeof(CommentEntity), commentId);
	  }

	  protected internal virtual void checkHistoryEnabled()
	  {
		if (!HistoryManager.HistoryEnabled)
		{
		  throw new ActivitiException("In order to use comments, history should be enabled");
		}
	  }
	}

}