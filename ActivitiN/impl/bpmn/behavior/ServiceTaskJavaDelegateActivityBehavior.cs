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

namespace org.activiti.engine.impl.bpmn.behavior
{

	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;
	using JavaDelegate = org.activiti.engine.@delegate.JavaDelegate;
	using Context = org.activiti.engine.impl.context.Context;
	using JavaDelegateInvocation = org.activiti.engine.impl.@delegate.JavaDelegateInvocation;
	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class ServiceTaskJavaDelegateActivityBehavior : TaskActivityBehavior, ActivityBehavior, ExecutionListener
	{

	  protected internal JavaDelegate javaDelegate;

	  protected internal ServiceTaskJavaDelegateActivityBehavior()
	  {
	  }

	  public ServiceTaskJavaDelegateActivityBehavior(JavaDelegate javaDelegate)
	  {
		this.javaDelegate = javaDelegate;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		execute((DelegateExecution) execution);
		leave(execution);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.activiti.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		execute((DelegateExecution) execution);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void execute(DelegateExecution execution)
	  {
		Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new JavaDelegateInvocation(javaDelegate, execution));
	  }
	}

}