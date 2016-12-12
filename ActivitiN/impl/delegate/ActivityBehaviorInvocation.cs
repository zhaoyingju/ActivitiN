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

	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class ActivityBehaviorInvocation : DelegateInvocation
	{

	  protected internal readonly ActivityBehavior behaviorInstance;

	  protected internal readonly ActivityExecution execution;

	  public ActivityBehaviorInvocation(ActivityBehavior behaviorInstance, ActivityExecution execution)
	  {
		this.behaviorInstance = behaviorInstance;
		this.execution = execution;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void invoke() throws Exception
	  protected internal override void invoke()
	  {
		behaviorInstance.execute(execution);
	  }

	  public override object Target
	  {
		  get
		  {
			return behaviorInstance;
		  }
	  }

	}

}