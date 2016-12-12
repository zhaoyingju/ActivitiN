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
namespace org.activiti.engine.impl.@delegate
{

	using DelegateTask = org.activiti.engine.@delegate.DelegateTask;
	using TaskListener = org.activiti.engine.@delegate.TaskListener;

	/// <summary>
	/// Class handling invocations of <seealso cref="TaskListener TaskListeners"/>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class TaskListenerInvocation : DelegateInvocation
	{

	  protected internal readonly TaskListener executionListenerInstance;
	  protected internal readonly DelegateTask delegateTask;

	  public TaskListenerInvocation(TaskListener executionListenerInstance, DelegateTask delegateTask)
	  {
		this.executionListenerInstance = executionListenerInstance;
		this.delegateTask = delegateTask;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void invoke() throws Exception
	  protected internal override void invoke()
	  {
		executionListenerInstance.notify(delegateTask);
	  }

	  public override object Target
	  {
		  get
		  {
			return executionListenerInstance;
		  }
	  }

	}

}