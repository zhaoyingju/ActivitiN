using System;
using System.Collections.Generic;

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
	using FieldExtension = org.activiti.bpmn.model.FieldExtension;
	using IOParameter = org.activiti.bpmn.model.IOParameter;
	using InclusiveGateway = org.activiti.bpmn.model.InclusiveGateway;
	using IntermediateCatchEvent = org.activiti.bpmn.model.IntermediateCatchEvent;
	using ManualTask = org.activiti.bpmn.model.ManualTask;
	using MapExceptionEntry = org.activiti.bpmn.model.MapExceptionEntry;
	using ParallelGateway = org.activiti.bpmn.model.ParallelGateway;
	using ReceiveTask = org.activiti.bpmn.model.ReceiveTask;
	using ScriptTask = org.activiti.bpmn.model.ScriptTask;
	using SendTask = org.activiti.bpmn.model.SendTask;
	using ServiceTask = org.activiti.bpmn.model.ServiceTask;
	using Signal = org.activiti.bpmn.model.Signal;
	using StartEvent = org.activiti.bpmn.model.StartEvent;
	using SubProcess = org.activiti.bpmn.model.SubProcess;
	using Task = org.activiti.bpmn.model.Task;
	using TaskWithFieldExtensions = org.activiti.bpmn.model.TaskWithFieldExtensions;
	using ThrowEvent = org.activiti.bpmn.model.ThrowEvent;
	using Transaction = org.activiti.bpmn.model.Transaction;
	using UserTask = org.activiti.bpmn.model.UserTask;
	using BusinessRuleTaskDelegate = org.activiti.engine.@delegate.BusinessRuleTaskDelegate;
	using Expression = org.activiti.engine.@delegate.Expression;
	using AbstractBpmnActivityBehavior = org.activiti.engine.impl.bpmn.behavior.AbstractBpmnActivityBehavior;
	using BoundaryEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.BoundaryEventActivityBehavior;
	using BusinessRuleTaskActivityBehavior = org.activiti.engine.impl.bpmn.behavior.BusinessRuleTaskActivityBehavior;
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
	using SimpleDataInputAssociation = org.activiti.engine.impl.bpmn.data.SimpleDataInputAssociation;
	using ClassDelegate = org.activiti.engine.impl.bpmn.helper.ClassDelegate;
	using MessageImplicitDataOutputAssociation = org.activiti.engine.impl.bpmn.webservice.MessageImplicitDataOutputAssociation;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ScriptingEngines = org.activiti.engine.impl.scripting.ScriptingEngines;
	using TaskDefinition = org.activiti.engine.impl.task.TaskDefinition;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	/// <summary>
	/// Default implementation of the <seealso cref="ActivityBehaviorFactory"/>. Used when no
	/// custom <seealso cref="ActivityBehaviorFactory"/> is injected on the
	/// <seealso cref="ProcessEngineConfigurationImpl"/>.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class DefaultActivityBehaviorFactory : AbstractBehaviorFactory, ActivityBehaviorFactory
	{

	  // Start event
	  public const string EXCEPTION_MAP_FIELD = "mapExceptions";

	  public virtual NoneStartEventActivityBehavior createNoneStartEventActivityBehavior(StartEvent startEvent)
	  {
		return new NoneStartEventActivityBehavior();
	  }

	  public virtual EventSubProcessStartEventActivityBehavior createEventSubProcessStartEventActivityBehavior(StartEvent startEvent, string activityId)
	  {
		return new EventSubProcessStartEventActivityBehavior(activityId);
	  }

	  // Task

	  public virtual TaskActivityBehavior createTaskActivityBehavior(Task task)
	  {
		return new TaskActivityBehavior();
	  }

	  public virtual ManualTaskActivityBehavior createManualTaskActivityBehavior(ManualTask manualTask)
	  {
		return new ManualTaskActivityBehavior();
	  }

	  public virtual ReceiveTaskActivityBehavior createReceiveTaskActivityBehavior(ReceiveTask receiveTask)
	  {
		return new ReceiveTaskActivityBehavior();
	  }

	  public virtual UserTaskActivityBehavior createUserTaskActivityBehavior(UserTask userTask, TaskDefinition taskDefinition)
	  {
		return new UserTaskActivityBehavior(userTask.Id, taskDefinition);
	  }

	  // Service task

	  public virtual ClassDelegate createClassDelegateServiceTask(ServiceTask serviceTask)
	  {
		Expression skipExpression;
		if (StringUtils.isNotEmpty(serviceTask.SkipExpression))
		{
		  skipExpression = expressionManager.createExpression(serviceTask.SkipExpression);
		}
		else
		{
		  skipExpression = null;
		}
		return new ClassDelegate(serviceTask.Id, serviceTask.Implementation, createFieldDeclarations(serviceTask.FieldExtensions), skipExpression, serviceTask.MapExceptions);
	  }

	  public virtual ServiceTaskDelegateExpressionActivityBehavior createServiceTaskDelegateExpressionActivityBehavior(ServiceTask serviceTask)
	  {
		Expression delegateExpression = expressionManager.createExpression(serviceTask.Implementation);
		Expression skipExpression;
		if (StringUtils.isNotEmpty(serviceTask.SkipExpression))
		{
		  skipExpression = expressionManager.createExpression(serviceTask.SkipExpression);
		}
		else
		{
		  skipExpression = null;
		}
		return new ServiceTaskDelegateExpressionActivityBehavior(serviceTask.Id, delegateExpression, skipExpression, createFieldDeclarations(serviceTask.FieldExtensions));
	  }

	  public virtual ServiceTaskExpressionActivityBehavior createServiceTaskExpressionActivityBehavior(ServiceTask serviceTask)
	  {
		Expression expression = expressionManager.createExpression(serviceTask.Implementation);
		Expression skipExpression;
		if (StringUtils.isNotEmpty(serviceTask.SkipExpression))
		{
		  skipExpression = expressionManager.createExpression(serviceTask.SkipExpression);
		}
		else
		{
		  skipExpression = null;
		}
		return new ServiceTaskExpressionActivityBehavior(serviceTask.Id, expression, skipExpression, serviceTask.ResultVariableName);
	  }

	  public virtual WebServiceActivityBehavior createWebServiceActivityBehavior(ServiceTask serviceTask)
	  {
		return new WebServiceActivityBehavior();
	  }

	  public virtual WebServiceActivityBehavior createWebServiceActivityBehavior(SendTask sendTask)
	  {
		return new WebServiceActivityBehavior();
	  }

	  public virtual MailActivityBehavior createMailActivityBehavior(ServiceTask serviceTask)
	  {
		return createMailActivityBehavior(serviceTask.Id, serviceTask.FieldExtensions);
	  }

	  public virtual MailActivityBehavior createMailActivityBehavior(SendTask sendTask)
	  {
		return createMailActivityBehavior(sendTask.Id, sendTask.FieldExtensions);
	  }

	  protected internal virtual MailActivityBehavior createMailActivityBehavior(string taskId, IList<FieldExtension> fields)
	  {
		IList<FieldDeclaration> fieldDeclarations = createFieldDeclarations(fields);
		return (MailActivityBehavior) ClassDelegate.defaultInstantiateDelegate(typeof(MailActivityBehavior), fieldDeclarations);
	  }

	  // We do not want a hard dependency on Mule, hence we return ActivityBehavior and instantiate 
	  // the delegate instance using a string instead of the Class itself.
	  public virtual ActivityBehavior createMuleActivityBehavior(ServiceTask serviceTask, BpmnModel bpmnModel)
	  {
		return createMuleActivityBehavior(serviceTask, serviceTask.FieldExtensions, bpmnModel);
	  }

	  public virtual ActivityBehavior createMuleActivityBehavior(SendTask sendTask, BpmnModel bpmnModel)
	  {
		return createMuleActivityBehavior(sendTask, sendTask.FieldExtensions, bpmnModel);
	  }

	  protected internal virtual ActivityBehavior createMuleActivityBehavior(TaskWithFieldExtensions task, IList<FieldExtension> fieldExtensions, BpmnModel bpmnModel)
	  {
		try
		{

		  Type theClass = Type.GetType("org.activiti.mule.MuleSendActivitiBehavior");
		  IList<FieldDeclaration> fieldDeclarations = createFieldDeclarations(fieldExtensions);
		  return (ActivityBehavior) ClassDelegate.defaultInstantiateDelegate(theClass, fieldDeclarations);

		}
		catch (ClassNotFoundException e)
		{
			throw new ActivitiException("Could not find org.activiti.mule.MuleSendActivitiBehavior: ", e);
		}
	  }

	  // We do not want a hard dependency on Camel, hence we return ActivityBehavior and instantiate 
	  // the delegate instance using a string instead of the Class itself.
	  public virtual ActivityBehavior createCamelActivityBehavior(ServiceTask serviceTask, BpmnModel bpmnModel)
	  {
		return createCamelActivityBehavior(serviceTask, serviceTask.FieldExtensions, bpmnModel);
	  }

	  public virtual ActivityBehavior createCamelActivityBehavior(SendTask sendTask, BpmnModel bpmnModel)
	  {
		return createCamelActivityBehavior(sendTask, sendTask.FieldExtensions, bpmnModel);
	  }

	  protected internal virtual ActivityBehavior createCamelActivityBehavior(TaskWithFieldExtensions task, IList<FieldExtension> fieldExtensions, BpmnModel bpmnModel)
	  {
		try
		{
		  Type theClass = null;
		  FieldExtension behaviorExtension = null;
		  foreach (FieldExtension fieldExtension in fieldExtensions)
		  {
			if ("camelBehaviorClass".Equals(fieldExtension.FieldName) && StringUtils.isNotEmpty(fieldExtension.StringValue))
			{
			  theClass = Type.GetType(fieldExtension.StringValue);
			  behaviorExtension = fieldExtension;
			  break;
			}
		  }

		  if (behaviorExtension != null)
		  {
			fieldExtensions.Remove(behaviorExtension);
		  }

		  if (theClass == null)
		  {
			// Default Camel behavior class
			theClass = Type.GetType("org.activiti.camel.impl.CamelBehaviorDefaultImpl");
		  }

		  IList<FieldDeclaration> fieldDeclarations = createFieldDeclarations(fieldExtensions);
		  addExceptionMapAsFieldDeclaraion(fieldDeclarations, task.MapExceptions);
		  return (ActivityBehavior) ClassDelegate.defaultInstantiateDelegate(theClass, fieldDeclarations);

		}
		catch (ClassNotFoundException e)
		{
			throw new ActivitiException("Could not find org.activiti.camel.CamelBehavior: ", e);
		}
	  }

	  private void addExceptionMapAsFieldDeclaraion(IList<FieldDeclaration> fieldDeclarations, IList<MapExceptionEntry> mapExceptions)
	  {
		FieldDeclaration exceptionMapsFieldDeclaration = new FieldDeclaration(EXCEPTION_MAP_FIELD, mapExceptions.GetType().ToString(), mapExceptions);
		fieldDeclarations.Add(exceptionMapsFieldDeclaration);

	  }

	  public virtual ShellActivityBehavior createShellActivityBehavior(ServiceTask serviceTask)
	  {
		IList<FieldDeclaration> fieldDeclarations = createFieldDeclarations(serviceTask.FieldExtensions);
		return (ShellActivityBehavior) ClassDelegate.defaultInstantiateDelegate(typeof(ShellActivityBehavior), fieldDeclarations);
	  }

	  public virtual ActivityBehavior createBusinessRuleTaskActivityBehavior(BusinessRuleTask businessRuleTask)
	  {
		BusinessRuleTaskDelegate ruleActivity = null;
		if (StringUtils.isNotEmpty(businessRuleTask.ClassName))
		{
		  try
		  {
			Type clazz = Type.GetType(businessRuleTask.ClassName);
			ruleActivity = (BusinessRuleTaskDelegate) clazz.newInstance();
		  }
		  catch (Exception e)
		  {
			throw new ActivitiException("Could not instantiate businessRuleTask (id:" + businessRuleTask.Id + ") class: " + businessRuleTask.ClassName, e);
		  }
		}
		else
		{
		  ruleActivity = new BusinessRuleTaskActivityBehavior();
		}

		foreach (string ruleVariableInputObject in businessRuleTask.InputVariables)
		{
		  ruleActivity.addRuleVariableInputIdExpression(expressionManager.createExpression(ruleVariableInputObject.Trim()));
		}

		foreach (string rule in businessRuleTask.RuleNames)
		{
		  ruleActivity.addRuleIdExpression(expressionManager.createExpression(rule.Trim()));
		}

		ruleActivity.Exclude = businessRuleTask.Exclude;

		if (businessRuleTask.ResultVariableName != null && businessRuleTask.ResultVariableName.length() > 0)
		{
		  ruleActivity.ResultVariable = businessRuleTask.ResultVariableName;
		}
		else
		{
		  ruleActivity.ResultVariable = "org.activiti.engine.rules.OUTPUT";
		}

		return ruleActivity;
	  }

	  // Script task

	  public virtual ScriptTaskActivityBehavior createScriptTaskActivityBehavior(ScriptTask scriptTask)
	  {
		string language = scriptTask.ScriptFormat;
		if (language == null)
		{
		  language = ScriptingEngines.DEFAULT_SCRIPTING_LANGUAGE;
		}
		return new ScriptTaskActivityBehavior(scriptTask.Id, scriptTask.Script, language, scriptTask.ResultVariable, scriptTask.AutoStoreVariables);
	  }

	  // Gateways

	  public virtual ExclusiveGatewayActivityBehavior createExclusiveGatewayActivityBehavior(ExclusiveGateway exclusiveGateway)
	  {
		return new ExclusiveGatewayActivityBehavior();
	  }

	  public virtual ParallelGatewayActivityBehavior createParallelGatewayActivityBehavior(ParallelGateway parallelGateway)
	  {
		return new ParallelGatewayActivityBehavior();
	  }

	  public virtual InclusiveGatewayActivityBehavior createInclusiveGatewayActivityBehavior(InclusiveGateway inclusiveGateway)
	  {
		return new InclusiveGatewayActivityBehavior();
	  }

	  public virtual EventBasedGatewayActivityBehavior createEventBasedGatewayActivityBehavior(EventGateway eventGateway)
	  {
		return new EventBasedGatewayActivityBehavior();
	  }

	  // Multi Instance

	  public virtual SequentialMultiInstanceBehavior createSequentialMultiInstanceBehavior(ActivityImpl activity, AbstractBpmnActivityBehavior innerActivityBehavior)
	  {
		return new SequentialMultiInstanceBehavior(activity, innerActivityBehavior);
	  }

	  public virtual ParallelMultiInstanceBehavior createParallelMultiInstanceBehavior(ActivityImpl activity, AbstractBpmnActivityBehavior innerActivityBehavior)
	  {
		return new ParallelMultiInstanceBehavior(activity, innerActivityBehavior);
	  }

	  // Subprocess

	  public virtual SubProcessActivityBehavior createSubprocActivityBehavior(SubProcess subProcess)
	  {
		return new SubProcessActivityBehavior();
	  }

	  // Call activity

	  public virtual CallActivityBehavior createCallActivityBehavior(CallActivity callActivity)
	  {
		string expressionRegex = "\\$+\\{+.+\\}";

		CallActivityBehavior callActivityBehaviour = null;
		if (StringUtils.isNotEmpty(callActivity.CalledElement) && callActivity.CalledElement.matches(expressionRegex))
		{
		  callActivityBehaviour = new CallActivityBehavior(expressionManager.createExpression(callActivity.CalledElement), callActivity.MapExceptions);
		}
		else
		{
		  callActivityBehaviour = new CallActivityBehavior(callActivity.CalledElement, callActivity.MapExceptions);
		}
		callActivityBehaviour.InheritVariables = callActivity.InheritVariables;

		foreach (IOParameter ioParameter in callActivity.InParameters)
		{
		  if (StringUtils.isNotEmpty(ioParameter.SourceExpression))
		  {
			Expression expression = expressionManager.createExpression(ioParameter.SourceExpression.Trim());
			callActivityBehaviour.addDataInputAssociation(new SimpleDataInputAssociation(expression, ioParameter.Target));
		  }
		  else
		  {
			callActivityBehaviour.addDataInputAssociation(new SimpleDataInputAssociation(ioParameter.Source, ioParameter.Target));
		  }
		}

		foreach (IOParameter ioParameter in callActivity.OutParameters)
		{
		  if (StringUtils.isNotEmpty(ioParameter.SourceExpression))
		  {
			Expression expression = expressionManager.createExpression(ioParameter.SourceExpression.Trim());
			callActivityBehaviour.addDataOutputAssociation(new MessageImplicitDataOutputAssociation(ioParameter.Target, expression));
		  }
		  else
		  {
			callActivityBehaviour.addDataOutputAssociation(new MessageImplicitDataOutputAssociation(ioParameter.Target, ioParameter.Source));
		  }
		}

		return callActivityBehaviour;
	  }

	  // Transaction

	  public virtual TransactionActivityBehavior createTransactionActivityBehavior(Transaction transaction)
	  {
		return new TransactionActivityBehavior();
	  }

	  // Intermediate Events

	  public virtual IntermediateCatchEventActivityBehavior createIntermediateCatchEventActivityBehavior(IntermediateCatchEvent intermediateCatchEvent)
	  {
		return new IntermediateCatchEventActivityBehavior();
	  }

	  public virtual IntermediateThrowNoneEventActivityBehavior createIntermediateThrowNoneEventActivityBehavior(ThrowEvent throwEvent)
	  {
		return new IntermediateThrowNoneEventActivityBehavior();
	  }

	  public virtual IntermediateThrowSignalEventActivityBehavior createIntermediateThrowSignalEventActivityBehavior(ThrowEvent throwEvent, Signal signal, EventSubscriptionDeclaration eventSubscriptionDeclaration)
	  {
		return new IntermediateThrowSignalEventActivityBehavior(throwEvent, signal, eventSubscriptionDeclaration);
	  }

	  public virtual IntermediateThrowCompensationEventActivityBehavior createIntermediateThrowCompensationEventActivityBehavior(ThrowEvent throwEvent, CompensateEventDefinition compensateEventDefinition)
	  {
		return new IntermediateThrowCompensationEventActivityBehavior(compensateEventDefinition);
	  }

	  // End events

	  public virtual NoneEndEventActivityBehavior createNoneEndEventActivityBehavior(EndEvent endEvent)
	  {
		return new NoneEndEventActivityBehavior();
	  }

	  public virtual ErrorEndEventActivityBehavior createErrorEndEventActivityBehavior(EndEvent endEvent, ErrorEventDefinition errorEventDefinition)
	  {
		return new ErrorEndEventActivityBehavior(errorEventDefinition.ErrorCode);
	  }

	  public virtual CancelEndEventActivityBehavior createCancelEndEventActivityBehavior(EndEvent endEvent)
	  {
		return new CancelEndEventActivityBehavior();
	  }

	  public virtual TerminateEndEventActivityBehavior createTerminateEndEventActivityBehavior(EndEvent endEvent)
	  {
		return new TerminateEndEventActivityBehavior(endEvent);
	  }

	  // Boundary Events

	  public virtual BoundaryEventActivityBehavior createBoundaryEventActivityBehavior(BoundaryEvent boundaryEvent, bool interrupting, ActivityImpl activity)
	  {
		return new BoundaryEventActivityBehavior(interrupting, activity.Id);
	  }

	  public virtual CancelBoundaryEventActivityBehavior createCancelBoundaryEventActivityBehavior(CancelEventDefinition cancelEventDefinition)
	  {
		return new CancelBoundaryEventActivityBehavior();
	  }

	}

}