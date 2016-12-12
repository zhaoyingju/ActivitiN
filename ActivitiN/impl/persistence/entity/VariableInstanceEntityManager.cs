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
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Saeid Mirzaei
	/// </summary>
	public class VariableInstanceEntityManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<VariableInstanceEntity> findVariableInstancesByTaskId(String taskId)
	  public virtual IList<VariableInstanceEntity> findVariableInstancesByTaskId(string taskId)
	  {
		return DbSqlSession.selectList("selectVariablesByTaskId", taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<VariableInstanceEntity> findVariableInstancesByTaskIds(java.util.Set<String> taskIds)
	  public virtual IList<VariableInstanceEntity> findVariableInstancesByTaskIds(Set<string> taskIds)
	  {
		return DbSqlSession.selectList("selectVariablesByTaskIds", taskIds);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<VariableInstanceEntity> findVariableInstancesByExecutionId(String executionId)
	  public virtual IList<VariableInstanceEntity> findVariableInstancesByExecutionId(string executionId)
	  {
		return DbSqlSession.selectList("selectVariablesByExecutionId", executionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<VariableInstanceEntity> findVariableInstancesByExecutionIds(java.util.Set<String> executionIds)
	  public virtual IList<VariableInstanceEntity> findVariableInstancesByExecutionIds(Set<string> executionIds)
	  {
		return DbSqlSession.selectList("selectVariablesByExecutionIds", executionIds);
	  }

		public virtual VariableInstanceEntity findVariableInstanceByExecutionAndName(string executionId, string variableName)
		{
			IDictionary<string, string> @params = new Dictionary<string, string>(2);
			@params["executionId"] = executionId;
			@params["name"] = variableName;
			return (VariableInstanceEntity) DbSqlSession.selectOne("selectVariableInstanceByExecutionAndName", @params);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<VariableInstanceEntity> findVariableInstancesByExecutionAndNames(String executionId, java.util.Collection<String> names)
		public virtual IList<VariableInstanceEntity> findVariableInstancesByExecutionAndNames(string executionId, ICollection<string> names)
		{
			IDictionary<string, object> @params = new Dictionary<string, object>(2);
			@params["executionId"] = executionId;
			@params["names"] = names;
			return DbSqlSession.selectList("selectVariableInstancesByExecutionAndNames", @params);
		}

		public virtual VariableInstanceEntity findVariableInstanceByTaskAndName(string taskId, string variableName)
		{
			IDictionary<string, string> @params = new Dictionary<string, string>(2);
			@params["taskId"] = taskId;
			@params["name"] = variableName;
			return (VariableInstanceEntity) DbSqlSession.selectOne("selectVariableInstanceByTaskAndName", @params);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<VariableInstanceEntity> findVariableInstancesByTaskAndNames(String taskId, java.util.Collection<String> names)
		public virtual IList<VariableInstanceEntity> findVariableInstancesByTaskAndNames(string taskId, ICollection<string> names)
		{
			IDictionary<string, object> @params = new Dictionary<string, object>(2);
			@params["taskId"] = taskId;
			@params["names"] = names;
			return DbSqlSession.selectList("selectVariableInstancesByTaskAndNames", @params);
		}

	  public virtual void deleteVariableInstanceByTask(TaskEntity task)
	  {
		IDictionary<string, VariableInstanceEntity> variableInstances = task.VariableInstanceEntities;
		if (variableInstances != null)
		{
		  foreach (VariableInstanceEntity variableInstance in variableInstances.Values)
		  {
			variableInstance.delete();
		  }
		}
	  }
	}

}