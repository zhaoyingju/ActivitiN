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
namespace org.activiti.engine.task
{


	/// <summary>
	/// This is a helper class to help you work with the <seealso cref="TaskInfoQuery"/>, without having to care about the awful generics.
	/// 
	/// Example usage:
	/// 
	/// 	TaskInfoQueryWrapper taskInfoQueryWrapper = new TaskInfoQueryWrapper(taskService.createTaskQuery());
	///  List<? extends TaskInfo> taskInfos = taskInfoQueryWrapper.getTaskInfoQuery().or()
	/// 		.taskNameLike("%task%")
	/// 		.taskDescriptionLike("%blah%");
	/// 	.endOr()
	/// 	.list();
	/// 
	/// First line can be switched to TaskInfoQueryWrapper taskInfoQueryWrapper = new TaskInfoQueryWrapper(historyService.createTaskQuery());
	/// and the same methods can be used on the result.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class TaskInfoQueryWrapper
	{

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: protected TaskInfoQuery<? extends TaskInfoQuery<?,?>, ? extends TaskInfo> taskInfoQuery;
		protected internal TaskInfoQuery<?, ?> taskInfoQuery;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public TaskInfoQueryWrapper(TaskInfoQuery<? extends TaskInfoQuery<?,?>, ? extends TaskInfo> taskInfoQuery)
		public TaskInfoQueryWrapper<T1>(TaskInfoQuery<T1> taskInfoQuery) where T1 : TaskInfoQuery<T1,T1> where ? : TaskInfo
		{
			this.taskInfoQuery = taskInfoQuery;
		}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public TaskInfoQuery<? extends TaskInfoQuery<?, ?>, ? extends TaskInfo> getTaskInfoQuery()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public TaskInfoQuery<? extends TaskInfoQuery<?, ?>, ? extends TaskInfo> getTaskInfoQuery()
		public virtual TaskInfoQuery<?, ?> getTaskInfoQuery() where ? : TaskInfoQuery<?, ?> where ? : TaskInfo
		{
			return taskInfoQuery;
		}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public void setTaskInfoQuery(TaskInfoQuery<? extends TaskInfoQuery<?, ?>, ? extends TaskInfo> taskInfoQuery)
		public virtual void setTaskInfoQuery<T1>(TaskInfoQuery<T1> taskInfoQuery) where T1 : TaskInfoQuery<T1, T1> where ? : TaskInfo
		{
			this.taskInfoQuery = taskInfoQuery;
		}

	}

}