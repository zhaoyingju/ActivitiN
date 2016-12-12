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
namespace org.activiti.engine.impl.bpmn.parser.factory
{

	using ActivitiListener = org.activiti.bpmn.model.ActivitiListener;
	using EventListener = org.activiti.bpmn.model.EventListener;
	using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;
	using TaskListener = org.activiti.engine.@delegate.TaskListener;
	using ActivitiEventListener = org.activiti.engine.@delegate.@event.ActivitiEventListener;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;

	/// <summary>
	/// Factory class used by the <seealso cref="BpmnParser"/> and <seealso cref="BpmnParse"/> to instantiate
	/// the behaviour classes for <seealso cref="TaskListener"/> and <seealso cref="ExecutionListener"/> usages.
	/// 
	/// You can provide your own implementation of this class. This way, you can give
	/// different execution semantics to the standard construct. 
	/// 
	/// The easiest and advisable way to implement your own <seealso cref="ListenerFactory"/> 
	/// is to extend the <seealso cref="DefaultListenerFactory"/>.
	/// 
	/// An instance of this interface can be injected in the <seealso cref="ProcessEngineConfigurationImpl"/>
	/// and its subclasses. 
	/// 
	/// @author Joram Barrez
	/// </summary>
	public interface ListenerFactory
	{

	  TaskListener createClassDelegateTaskListener(ActivitiListener activitiListener);

	  TaskListener createExpressionTaskListener(ActivitiListener activitiListener);

	  TaskListener createDelegateExpressionTaskListener(ActivitiListener activitiListener);

	  ExecutionListener createClassDelegateExecutionListener(ActivitiListener activitiListener);

	  ExecutionListener createExpressionExecutionListener(ActivitiListener activitiListener);

	  ExecutionListener createDelegateExpressionExecutionListener(ActivitiListener activitiListener);

	  ActivitiEventListener createClassDelegateEventListener(EventListener eventListener);

	  ActivitiEventListener createDelegateExpressionEventListener(EventListener eventListener);

	  ActivitiEventListener createEventThrowingEventListener(EventListener eventListener);

	}
}