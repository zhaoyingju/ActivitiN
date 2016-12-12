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

namespace org.activiti.engine.impl.pvm.runtime
{

	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ExecutionListenerExecution = org.activiti.engine.impl.pvm.@delegate.ExecutionListenerExecution;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.activiti.engine.impl.pvm.process.ProcessDefinitionImpl;
	using TransitionImpl = org.activiti.engine.impl.pvm.process.TransitionImpl;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public interface InterpretableExecution : ActivityExecution, ExecutionListenerExecution, PvmProcessInstance
	{

	  void take(PvmTransition transition);

	  void take(PvmTransition transition, bool fireActivityCompletedEvent);

	  string EventName {set;}

	  PvmProcessElement EventSource {set;}

	  int? ExecutionListenerIndex {get;set;}

	  ProcessDefinitionImpl ProcessDefinition {get;set;}

	  ActivityImpl Activity {set;}

	  void performOperation(AtomicOperation etomicOperation);

	  bool Scope {get;}

	  void destroy();

	  void remove();

	  InterpretableExecution ReplacedBy {get;set;}

	  InterpretableExecution SubProcessInstance {get;set;}

	  InterpretableExecution SuperExecution {get;}

	  void deleteCascade(string deleteReason);

	  bool DeleteRoot {get;}

	  TransitionImpl Transition {get;set;}

	  void initialize();

	  InterpretableExecution Parent {set;}


	  InterpretableExecution ProcessInstance {set;}

	  bool EventScope {get;set;}


	  StartingExecution StartingExecution {get;}

	  void disposeStartingExecution();
	}

}