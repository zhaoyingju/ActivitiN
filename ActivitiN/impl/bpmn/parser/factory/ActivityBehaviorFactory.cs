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

	using BoundaryEvent = org.activiti.bpmn.model.BoundaryEvent;
	using BpmnModel = org.activiti.bpmn.model.BpmnModel;
	using BusinessRuleTask = org.activiti.bpmn.model.BusinessRuleTask;
	using CallActivity = org.activiti.bpmn.model.CallActivity;
	using CancelEventDefinition = org.activiti.bpmn.model.CancelEventDefinition;
	using EndEvent = org.activiti.bpmn.model.EndEvent;
	using ErrorEventDefinition = org.activiti.bpmn.model.ErrorEventDefinition;
	using EventGateway = org.activiti.bpmn.model.EventGateway;
	using ExclusiveGateway = org.activiti.bpmn.model.ExclusiveGateway;
	using InclusiveGateway = org.activiti.bpmn.model.InclusiveGateway;
	using IntermediateCatchEvent = org.activiti.bpmn.model.IntermediateCatchEvent;
	using ManualTask = org.activiti.bpmn.model.ManualTask;
	using ParallelGateway = org.activiti.bpmn.model.ParallelGateway;
	using ReceiveTask = org.activiti.bpmn.model.ReceiveTask;
	using ScriptTask = org.activiti.bpmn.model.ScriptTask;
	using SendTask = org.activiti.bpmn.model.SendTask;
	using ServiceTask = org.activiti.bpmn.model.ServiceTask;
	using Signal = org.activiti.bpmn.model.Signal;
	using StartEvent = org.activiti.bpmn.model.StartEvent;
	using SubProcess = org.activiti.bpmn.model.SubProcess;
	using Task = org.activiti.bpmn.model.Task;
	using ThrowEvent = org.activiti.bpmn.model.ThrowEvent;
	using Transaction = org.activiti.bpmn.model.Transaction;
	using UserTask = org.activiti.bpmn.model.UserTask;
	using AbstractBpmnActivityBehavior = org.activiti.engine.impl.bpmn.behavior.AbstractBpmnActivityBehavior;
	using BoundaryEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.BoundaryEventActivityBehavior;
	using CallActivityBehavior = org.activiti.engine.impl.bpmn.behavior.CallActivityBehavior;
	using CancelBoundaryEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.CancelBoundaryEventActivityBehavior;
	using CancelEndEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.CancelEndEventActivityBehavior;
	using ErrorEndEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.ErrorEndEventActivityBehavior;
	using EventBasedGatewayActivityBehavior = org.activiti.engine.impl.bpmn.behavior.EventBasedGatewayActivityBehavior;
	using EventSubProcessStartEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.EventSubProcessStartEventActivityBehavior;
	using ExclusiveGatewayActivityBehavior = org.activiti.engine.impl.bpmn.behavior.ExclusiveGatewayActivityBehavior;
	using InclusiveGatewayActivityBehavior = org.activiti.engine.impl.bpmn.behavior.InclusiveGatewayActivityBehavior;
	using IntermediateCatchEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.IntermediateCatchEventActivityBehavior;
	using IntermediateThrowCompensationEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.IntermediateThrowCompensationEventActivityBehavior;
	using IntermediateThrowNoneEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.IntermediateThrowNoneEventActivityBehavior;
	using IntermediateThrowSignalEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.IntermediateThrowSignalEventActivityBehavior;
	using MailActivityBehavior = org.activiti.engine.impl.bpmn.behavior.MailActivityBehavior;
	using ManualTaskActivityBehavior = org.activiti.engine.impl.bpmn.behavior.ManualTaskActivityBehavior;
	using NoneEndEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.NoneEndEventActivityBehavior;
	using NoneStartEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.NoneStartEventActivityBehavior;
	using ParallelGatewayActivityBehavior = org.activiti.engine.impl.bpmn.behavior.ParallelGatewayActivityBehavior;
	using ParallelMultiInstanceBehavior = org.activiti.engine.impl.bpmn.behavior.ParallelMultiInstanceBehavior;
	using ReceiveTaskActivityBehavior = org.activiti.engine.impl.bpmn.behavior.ReceiveTaskActivityBehavior;
	using ScriptTaskActivityBehavior = org.activiti.engine.impl.bpmn.behavior.ScriptTaskActivityBehavior;
	using SequentialMultiInstanceBehavior = org.activiti.engine.impl.bpmn.behavior.SequentialMultiInstanceBehavior;
	using ServiceTaskDelegateExpressionActivityBehavior = org.activiti.engine.impl.bpmn.behavior.ServiceTaskDelegateExpressionActivityBehavior;
	using ServiceTaskExpressionActivityBehavior = org.activiti.engine.impl.bpmn.behavior.ServiceTaskExpressionActivityBehavior;
	using ShellActivityBehavior = org.activiti.engine.impl.bpmn.behavior.ShellActivityBehavior;
	using SubProcessActivityBehavior = org.activiti.engine.impl.bpmn.behavior.SubProcessActivityBehavior;
	using TaskActivityBehavior = org.activiti.engine.impl.bpmn.behavior.TaskActivityBehavior;
	using TerminateEndEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.TerminateEndEventActivityBehavior;
	using TransactionActivityBehavior = org.activiti.engine.impl.bpmn.behavior.TransactionActivityBehavior;
	using UserTaskActivityBehavior = org.activiti.engine.impl.bpmn.behavior.UserTaskActivityBehavior;
	using WebServiceActivityBehavior = org.activiti.engine.impl.bpmn.behavior.WebServiceActivityBehavior;
	using ClassDelegate = org.activiti.engine.impl.bpmn.helper.ClassDelegate;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using TaskDefinition = org.activiti.engine.impl.task.TaskDefinition;

	/// <summary>
	/// Factory class used by the <seealso cref="BpmnParser"/> and <seealso cref="BpmnParse"/> to instantiate
	/// the behaviour classes. For example when parsing an exclusive gateway, this factory
	/// will be requested to create a new <seealso cref="ActivityBehavior"/> that will be set on the 
	/// <seealso cref="ActivityImpl"/> of that step of the process and will implement the spec-compliant
	/// behavior of the exclusive gateway.
	/// 
	/// You can provide your own implementation of this class. This way, you can give
	/// different execution semantics to a standard bpmn xml construct. Eg. you could
	/// tweak the exclusive gateway to do something completely different if you would want that.
	/// Creating your own <seealso cref="ActivityBehaviorFactory"/> is only advisable if you
	/// want to change the default behavior of any BPMN default construct.
	/// And even then, think twice, because it won't be spec compliant bpmn anymore.
	/// 
	/// Note that you can always express any custom step as a service task with a class delegation.
	/// 
	/// The easiest and advisable way to implement your own <seealso cref="ActivityBehaviorFactory"/> 
	/// is to extend the <seealso cref="DefaultActivityBehaviorFactory"/> class and override 
	/// the method specific to the <seealso cref="ActivityBehavior"/> you want to change. 
	/// 
	/// An instance of this interface can be injected in the <seealso cref="ProcessEngineConfigurationImpl"/>
	/// and its subclasses. 
	/// 
	/// @author Joram Barrez
	/// </summary>
	public interface ActivityBehaviorFactory
	{

	  NoneStartEventActivityBehavior createNoneStartEventActivityBehavior(StartEvent startEvent);

	  EventSubProcessStartEventActivityBehavior createEventSubProcessStartEventActivityBehavior(StartEvent startEvent, string activityId);

	  TaskActivityBehavior createTaskActivityBehavior(Task task);

	  ManualTaskActivityBehavior createManualTaskActivityBehavior(ManualTask manualTask);

	  ReceiveTaskActivityBehavior createReceiveTaskActivityBehavior(ReceiveTask receiveTask);

	  UserTaskActivityBehavior createUserTaskActivityBehavior(UserTask userTask, TaskDefinition taskDefinition);

	  ClassDelegate createClassDelegateServiceTask(ServiceTask serviceTask);

	  ServiceTaskDelegateExpressionActivityBehavior createServiceTaskDelegateExpressionActivityBehavior(ServiceTask serviceTask);

	  ServiceTaskExpressionActivityBehavior createServiceTaskExpressionActivityBehavior(ServiceTask serviceTask);

	  WebServiceActivityBehavior createWebServiceActivityBehavior(ServiceTask serviceTask);

	  WebServiceActivityBehavior createWebServiceActivityBehavior(SendTask sendTask);

	  MailActivityBehavior createMailActivityBehavior(ServiceTask serviceTask);

	  MailActivityBehavior createMailActivityBehavior(SendTask sendTask);

	  // We do not want a hard dependency on the Mule module, hence we return ActivityBehavior and instantiate 
	  // the delegate instance using a string instead of the Class itself.
	  ActivityBehavior createMuleActivityBehavior(ServiceTask serviceTask, BpmnModel bpmnModel);

	  ActivityBehavior createMuleActivityBehavior(SendTask sendTask, BpmnModel bpmnModel);

	  ActivityBehavior createCamelActivityBehavior(ServiceTask serviceTask, BpmnModel bpmnModel);

	  ActivityBehavior createCamelActivityBehavior(SendTask sendTask, BpmnModel bpmnModel);

	  ShellActivityBehavior createShellActivityBehavior(ServiceTask serviceTask);

	  ActivityBehavior createBusinessRuleTaskActivityBehavior(BusinessRuleTask businessRuleTask);

	  ScriptTaskActivityBehavior createScriptTaskActivityBehavior(ScriptTask scriptTask);

	  ExclusiveGatewayActivityBehavior createExclusiveGatewayActivityBehavior(ExclusiveGateway exclusiveGateway);

	  ParallelGatewayActivityBehavior createParallelGatewayActivityBehavior(ParallelGateway parallelGateway);

	  InclusiveGatewayActivityBehavior createInclusiveGatewayActivityBehavior(InclusiveGateway inclusiveGateway);

	  EventBasedGatewayActivityBehavior createEventBasedGatewayActivityBehavior(EventGateway eventGateway);

	  SequentialMultiInstanceBehavior createSequentialMultiInstanceBehavior(ActivityImpl activity, AbstractBpmnActivityBehavior innerActivityBehavior);

	  ParallelMultiInstanceBehavior createParallelMultiInstanceBehavior(ActivityImpl activity, AbstractBpmnActivityBehavior innerActivityBehavior);

	  SubProcessActivityBehavior createSubprocActivityBehavior(SubProcess subProcess);

	  CallActivityBehavior createCallActivityBehavior(CallActivity callActivity);

	  TransactionActivityBehavior createTransactionActivityBehavior(Transaction transaction);

	  IntermediateCatchEventActivityBehavior createIntermediateCatchEventActivityBehavior(IntermediateCatchEvent intermediateCatchEvent);

	  IntermediateThrowNoneEventActivityBehavior createIntermediateThrowNoneEventActivityBehavior(ThrowEvent throwEvent);

	  IntermediateThrowSignalEventActivityBehavior createIntermediateThrowSignalEventActivityBehavior(ThrowEvent throwEvent, Signal signal, EventSubscriptionDeclaration eventSubscriptionDeclaration);

	  IntermediateThrowCompensationEventActivityBehavior createIntermediateThrowCompensationEventActivityBehavior(ThrowEvent throwEvent, CompensateEventDefinition compensateEventDefinition);

	  NoneEndEventActivityBehavior createNoneEndEventActivityBehavior(EndEvent endEvent);

	  ErrorEndEventActivityBehavior createErrorEndEventActivityBehavior(EndEvent endEvent, ErrorEventDefinition errorEventDefinition);

	  CancelEndEventActivityBehavior createCancelEndEventActivityBehavior(EndEvent endEvent);

	  TerminateEndEventActivityBehavior createTerminateEndEventActivityBehavior(EndEvent endEvent);

	  BoundaryEventActivityBehavior createBoundaryEventActivityBehavior(BoundaryEvent boundaryEvent, bool interrupting, ActivityImpl activity);

	  CancelBoundaryEventActivityBehavior createCancelBoundaryEventActivityBehavior(CancelEventDefinition cancelEventDefinition);

	}
}