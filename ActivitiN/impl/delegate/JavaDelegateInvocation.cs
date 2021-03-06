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

	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.activiti.engine.@delegate.JavaDelegate;

	/// <summary>
	/// Class handling invocations of JavaDelegates
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class JavaDelegateInvocation : DelegateInvocation
	{

	  protected internal readonly JavaDelegate delegateInstance;
	  protected internal readonly DelegateExecution execution;

	  public JavaDelegateInvocation(JavaDelegate delegateInstance, DelegateExecution execution)
	  {
		this.delegateInstance = delegateInstance;
		this.execution = execution;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void invoke() throws Exception
	  protected internal override void invoke()
	  {
		delegateInstance.execute((DelegateExecution) execution);
	  }

	  public override object Target
	  {
		  get
		  {
			return delegateInstance;
		  }
	  }

	}

}