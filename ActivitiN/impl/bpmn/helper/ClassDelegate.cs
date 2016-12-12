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

namespace org.activiti.engine.impl.bpmn.helper
{


	using MapExceptionEntry = org.activiti.bpmn.model.MapExceptionEntry;
	using BpmnError = org.activiti.engine.@delegate.BpmnError;
	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using DelegateTask = org.activiti.engine.@delegate.DelegateTask;
	using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;
	using Expression = org.activiti.engine.@delegate.Expression;
	using JavaDelegate = org.activiti.engine.@delegate.JavaDelegate;
	using TaskListener = org.activiti.engine.@delegate.TaskListener;
	using AbstractBpmnActivityBehavior = org.activiti.engine.impl.bpmn.behavior.AbstractBpmnActivityBehavior;
	using ServiceTaskJavaDelegateActivityBehavior = org.activiti.engine.impl.bpmn.behavior.ServiceTaskJavaDelegateActivityBehavior;
	using FieldDeclaration = org.activiti.engine.impl.bpmn.parser.FieldDeclaration;
	using Context = org.activiti.engine.impl.context.Context;
	using ExecutionListenerInvocation = org.activiti.engine.impl.@delegate.ExecutionListenerInvocation;
	using TaskListenerInvocation = org.activiti.engine.impl.@delegate.TaskListenerInvocation;
	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using SignallableActivityBehavior = org.activiti.engine.impl.pvm.@delegate.SignallableActivityBehavior;
	using SubProcessActivityBehavior = org.activiti.engine.impl.pvm.@delegate.SubProcessActivityBehavior;
	using ReflectUtil = org.activiti.engine.impl.util.ReflectUtil;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;


	/// <summary>
	/// Helper class for bpmn constructs that allow class delegation.
	/// 
	/// This class will lazily instantiate the referenced classes when needed at runtime.
	/// 
	/// @author Joram Barrez
	/// @author Falko Menge
	/// @author Saeid Mirzaei
	/// </summary>
	[Serializable]
	public class ClassDelegate : AbstractBpmnActivityBehavior, TaskListener, ExecutionListener, SubProcessActivityBehavior
	{

	  protected internal string className;
	  protected internal IList<FieldDeclaration> fieldDeclarations;
	  protected internal ExecutionListener executionListenerInstance;
	  protected internal TaskListener taskListenerInstance;
	  protected internal ActivityBehavior activityBehaviorInstance;
	  protected internal Expression skipExpression;
	  protected internal IList<MapExceptionEntry> mapExceptions;
	  protected internal string serviceTaskId;

	  public ClassDelegate(string className, IList<FieldDeclaration> fieldDeclarations, Expression skipExpression)
	  {
		this.className = className;
		this.fieldDeclarations = fieldDeclarations;
		this.skipExpression = skipExpression;
	  }

	  public ClassDelegate(string id, string className, IList<FieldDeclaration> fieldDeclarations, Expression skipExpression, IList<MapExceptionEntry> mapExceptions) : this(className, fieldDeclarations, skipExpression)
	  {
		this.serviceTaskId = id;
		this.mapExceptions = mapExceptions;
	  }

	  public ClassDelegate(string className, IList<FieldDeclaration> fieldDeclarations) : this(className, fieldDeclarations, null)
	  {
	  }

	  public ClassDelegate(Type clazz, IList<FieldDeclaration> fieldDeclarations) : this(clazz.FullName, fieldDeclarations, null)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  }

	  public ClassDelegate(Type clazz, IList<FieldDeclaration> fieldDeclarations, Expression skipExpression) : this(clazz.FullName, fieldDeclarations, skipExpression)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  }

	  // Execution listener
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.activiti.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		if (executionListenerInstance == null)
		{
		  executionListenerInstance = ExecutionListenerInstance;
		}
		Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new ExecutionListenerInvocation(executionListenerInstance, execution));
	  }

	  protected internal virtual ExecutionListener ExecutionListenerInstance
	  {
		  get
		  {
			object delegateInstance = instantiateDelegate(className, fieldDeclarations);
			if (delegateInstance is ExecutionListener)
			{
			  return (ExecutionListener) delegateInstance;
			}
			else if (delegateInstance is JavaDelegate)
			{
			  return new ServiceTaskJavaDelegateActivityBehavior((JavaDelegate) delegateInstance);
			}
			else
			{
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  throw new ActivitiIllegalArgumentException(delegateInstance.GetType().FullName + " doesn't implement " + typeof(ExecutionListener) + " nor " + typeof(JavaDelegate));
			}
		  }
	  }

	  // Task listener
	  public virtual void notify(DelegateTask delegateTask)
	  {
		if (taskListenerInstance == null)
		{
		  taskListenerInstance = TaskListenerInstance;
		}
		try
		{
		  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new TaskListenerInvocation(taskListenerInstance, delegateTask));
		}
		catch (Exception e)
		{
		  throw new ActivitiException("Exception while invoking TaskListener: " + e.Message, e);
		}
	  }

	  protected internal virtual TaskListener TaskListenerInstance
	  {
		  get
		  {
			object delegateInstance = instantiateDelegate(className, fieldDeclarations);
			if (delegateInstance is TaskListener)
			{
			  return (TaskListener) delegateInstance;
			}
			else
			{
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  throw new ActivitiIllegalArgumentException(delegateInstance.GetType().FullName + " doesn't implement " + typeof(TaskListener));
			}
		  }
	  }

	  // Activity Behavior
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		bool isSkipExpressionEnabled = SkipExpressionUtil.isSkipExpressionEnabled(execution, skipExpression);
		if (!isSkipExpressionEnabled || (isSkipExpressionEnabled && !SkipExpressionUtil.shouldSkipFlowElement(execution, skipExpression)))
		{

		  if (Context.ProcessEngineConfiguration.EnableProcessDefinitionInfoCache)
		  {
			ObjectNode taskElementProperties = Context.getBpmnOverrideElementProperties(serviceTaskId, execution.ProcessDefinitionId);
			if (taskElementProperties != null && taskElementProperties.has(org.activiti.engine.DynamicBpmnConstants_Fields.SERVICE_TASK_CLASS_NAME))
			{
			  string overrideClassName = taskElementProperties.get(org.activiti.engine.DynamicBpmnConstants_Fields.SERVICE_TASK_CLASS_NAME).asText();
			  if (StringUtils.isNotEmpty(overrideClassName) && overrideClassName.Equals(className) == false)
			  {
				className = overrideClassName;
				activityBehaviorInstance = null;
			  }
			}
		  }

		  if (activityBehaviorInstance == null)
		  {
			activityBehaviorInstance = getActivityBehaviorInstance(execution);
		  }

		  try
		  {
			activityBehaviorInstance.execute(execution);
		  }
		  catch (BpmnError error)
		  {
			ErrorPropagation.propagateError(error, execution);
		  }
		  catch (Exception e)
		  {
			if (!ErrorPropagation.mapException(e, execution, mapExceptions))
			{
				throw e;
			}
		  }
		}
	  }

	  // Signallable activity behavior
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void signal(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		if (activityBehaviorInstance == null)
		{
		  activityBehaviorInstance = getActivityBehaviorInstance(execution);
		}

		if (activityBehaviorInstance is SignallableActivityBehavior)
		{
		  ((SignallableActivityBehavior) activityBehaviorInstance).signal(execution, signalName, signalData);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ActivitiException("signal() can only be called on a " + typeof(SignallableActivityBehavior).FullName + " instance");
		}
	  }

	  // Subprocess activityBehaviour

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void completing(org.activiti.engine.delegate.DelegateExecution execution, org.activiti.engine.delegate.DelegateExecution subProcessInstance) throws Exception
	  public override void completing(DelegateExecution execution, DelegateExecution subProcessInstance)
	  {
		  if (activityBehaviorInstance == null)
		  {
		  activityBehaviorInstance = getActivityBehaviorInstance((ActivityExecution) execution);
		  }

		if (activityBehaviorInstance is SubProcessActivityBehavior)
		{
		  ((SubProcessActivityBehavior) activityBehaviorInstance).completing(execution, subProcessInstance);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ActivitiException("completing() can only be called on a " + typeof(SubProcessActivityBehavior).FullName + " instance");
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void completed(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public override void completed(ActivityExecution execution)
	  {
		  if (activityBehaviorInstance == null)
		  {
		  activityBehaviorInstance = getActivityBehaviorInstance(execution);
		  }

		if (activityBehaviorInstance is SubProcessActivityBehavior)
		{
		  ((SubProcessActivityBehavior) activityBehaviorInstance).completed(execution);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ActivitiException("completed() can only be called on a " + typeof(SubProcessActivityBehavior).FullName + " instance");
		}
	  }

	  protected internal virtual ActivityBehavior getActivityBehaviorInstance(ActivityExecution execution)
	  {
		object delegateInstance = instantiateDelegate(className, fieldDeclarations);

		if (delegateInstance is ActivityBehavior)
		{
		  return determineBehaviour((ActivityBehavior) delegateInstance, execution);
		}
		else if (delegateInstance is JavaDelegate)
		{
		  return determineBehaviour(new ServiceTaskJavaDelegateActivityBehavior((JavaDelegate) delegateInstance), execution);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ActivitiIllegalArgumentException(delegateInstance.GetType().FullName + " doesn't implement " + typeof(JavaDelegate).FullName + " nor " + typeof(ActivityBehavior).FullName);
		}
	  }

	  // Adds properties to the given delegation instance (eg multi instance) if needed
	  protected internal virtual ActivityBehavior determineBehaviour(ActivityBehavior delegateInstance, ActivityExecution execution)
	  {
		if (hasMultiInstanceCharacteristics())
		{
		  multiInstanceActivityBehavior.InnerActivityBehavior = (AbstractBpmnActivityBehavior) delegateInstance;
		  return multiInstanceActivityBehavior;
		}
		return delegateInstance;
	  }

	  protected internal virtual object instantiateDelegate(string className, IList<FieldDeclaration> fieldDeclarations)
	  {
		return ClassDelegate.defaultInstantiateDelegate(className, fieldDeclarations);
	  }

	  // --HELPER METHODS (also usable by external classes) ----------------------------------------

	  public static object defaultInstantiateDelegate(Type clazz, IList<FieldDeclaration> fieldDeclarations)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return defaultInstantiateDelegate(clazz.FullName, fieldDeclarations);
	  }

	  public static object defaultInstantiateDelegate(string className, IList<FieldDeclaration> fieldDeclarations)
	  {
		object @object = ReflectUtil.instantiate(className);
		applyFieldDeclaration(fieldDeclarations, @object);
		return @object;
	  }



	  public static void applyFieldDeclaration(IList<FieldDeclaration> fieldDeclarations, object target)
	  {
		applyFieldDeclaration(fieldDeclarations, target, true);
	  }

	  public static void applyFieldDeclaration(IList<FieldDeclaration> fieldDeclarations, object target, bool throwExceptionOnMissingField)
	  {
		if (fieldDeclarations != null)
		{
		  foreach (FieldDeclaration declaration in fieldDeclarations)
		  {
			applyFieldDeclaration(declaration, target, throwExceptionOnMissingField);
		  }
		}
	  }

	  public static void applyFieldDeclaration(FieldDeclaration declaration, object target)
	  {
		applyFieldDeclaration(declaration, target, true);
	  }

	  public static void applyFieldDeclaration(FieldDeclaration declaration, object target, bool throwExceptionOnMissingField)
	  {
		Method setterMethod = ReflectUtil.getSetter(declaration.Name, target.GetType(), declaration.Value.GetType());

		if (setterMethod != null)
		{
		  try
		  {
			setterMethod.invoke(target, declaration.Value);
		  }
		  catch (System.ArgumentException e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new ActivitiException("Error while invoking '" + declaration.Name + "' on class " + target.GetType().FullName, e);
		  }
		  catch (IllegalAccessException e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new ActivitiException("Illegal acces when calling '" + declaration.Name + "' on class " + target.GetType().FullName, e);
		  }
		  catch (InvocationTargetException e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new ActivitiException("Exception while invoking '" + declaration.Name + "' on class " + target.GetType().FullName, e);
		  }
		}
		else
		{
		  Field field = ReflectUtil.getField(declaration.Name, target);
		  if (field == null)
		  {
			if (throwExceptionOnMissingField)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  throw new ActivitiIllegalArgumentException("Field definition uses unexisting field '" + declaration.Name + "' on class " + target.GetType().FullName);
			}
			else
			{
			  return;
			}
		  }

		  // Check if the delegate field's type is correct
		 if (!fieldTypeCompatible(declaration, field))
		 {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		   throw new ActivitiIllegalArgumentException("Incompatible type set on field declaration '" + declaration.Name + "' for class " + target.GetType().FullName + ". Declared value has type " + declaration.Value.GetType().FullName + ", while expecting " + field.Type.Name);
		 }
		 ReflectUtil.setField(field, target, declaration.Value);

		}
	  }

	  public static bool fieldTypeCompatible(FieldDeclaration declaration, Field field)
	  {
		if (declaration.Value != null)
		{
		  return declaration.Value.GetType().IsSubclassOf(field.Type);
		}
		else
		{
		  // Null can be set any field type
		  return true;
		}
	  }

	  /// <summary>
	  /// returns the class name this <seealso cref="ClassDelegate"/> is configured to. Comes in handy if you want to
	  /// check which delegates you already have e.g. in a list of listeners
	  /// </summary>
	  public virtual string ClassName
	  {
		  get
		  {
			return className;
		  }
	  }

	  public virtual IList<FieldDeclaration> FieldDeclarations
	  {
		  get
		  {
			return fieldDeclarations;
		  }
	  }

	}

}