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

	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class EventBasedGatewayActivityBehavior : FlowNodeActivityBehavior
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public override void execute(ActivityExecution execution)
	  {
		// the event based gateway doesn't really do anything
		// ignoring outgoing sequence flows (they're only parsed for the diagram)
	  }

	}

}