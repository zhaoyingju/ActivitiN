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

namespace org.activiti.engine.impl.pvm.@delegate
{

	using DelegateTask = org.activiti.engine.@delegate.DelegateTask;



	/// @deprecated use org.activiti.engine.delegate.TaskListener instead
	/// 
	/// @author Tom Baeyens 
	public interface TaskListener
	{

	  void notify(DelegateTask delegateTask);
	}

	public static class TaskListener_Fields
	{
	  public const string EVENTNAME_CREATE = "create";
	  public const string EVENTNAME_ASSIGNMENT = "assignment";
	  public const string EVENTNAME_COMPLETE = "complete";
	}

}