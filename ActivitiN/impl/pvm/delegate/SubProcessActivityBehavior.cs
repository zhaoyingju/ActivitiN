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

	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;



	/// <summary>
	/// behavior for activities that delegate to a complete separate execution of 
	/// a process definition.  In BPMN terminology this can be used to implement a reusable subprocess.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public interface SubProcessActivityBehavior : ActivityBehavior
	{

	  /// <summary>
	  /// called before the process instance is destroyed to allow 
	  /// this activity to extract data from the sub process instance.
	  /// No control flow should be done on the execution yet. 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void completing(org.activiti.engine.delegate.DelegateExecution execution, org.activiti.engine.delegate.DelegateExecution subProcessInstance) throws Exception;
	  void completing(DelegateExecution execution, DelegateExecution subProcessInstance);

	  /// <summary>
	  /// called after the process instance is destroyed for  
	  /// this activity to perform its outgoing control flow logic. 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void completed(ActivityExecution execution) throws Exception;
	  void completed(ActivityExecution execution);
	}

}