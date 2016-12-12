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
	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using EventSubProcessStartEventActivityBehavior = org.activiti.engine.impl.bpmn.behavior.EventSubProcessStartEventActivityBehavior;
	using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
	using ErrorEventDefinition = org.activiti.engine.impl.bpmn.parser.ErrorEventDefinition;
	using Context = org.activiti.engine.impl.context.Context;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.activiti.engine.impl.pvm.PvmActivity;
	using PvmException = org.activiti.engine.impl.pvm.PvmException;
	using PvmProcessDefinition = org.activiti.engine.impl.pvm.PvmProcessDefinition;
	using PvmScope = org.activiti.engine.impl.pvm.PvmScope;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.activiti.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
	using AtomicOperation = org.activiti.engine.impl.pvm.runtime.AtomicOperation;
	using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;
	using ReflectUtil = org.activiti.engine.impl.util.ReflectUtil;
	using StringUtils = org.apache.commons.lang3.StringUtils;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// This class is responsible for finding and executing error handlers for BPMN
	/// Errors.
	/// 
	/// Possible error handlers include Error Intermediate Events and Error Event
	/// Sub-Processes.
	/// 
	/// @author Falko Menge
	/// @author Daniel Meyer
	/// @author Saeid Mirzaei
	/// </summary>
	public class ErrorPropagation
	{

	  private static readonly Logger LOG = LoggerFactory.getLogger(typeof(ErrorPropagation));

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void propagateError(org.activiti.engine.delegate.BpmnError error, org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public static void propagateError(BpmnError error, ActivityExecution execution)
	  {
		propagateError(error.ErrorCode, execution);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void propagateError(String errorCode, org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public static void propagateError(string errorCode, ActivityExecution execution)
	  {

		while (execution != null)
		{
		  string eventHandlerId = findLocalErrorEventHandler(execution, errorCode);
		  if (eventHandlerId != null)
		  {
			executeCatch(eventHandlerId, execution, errorCode);
			break;
		  }

		  if (execution.ProcessInstanceType)
		  {
			// dispatch process completed event
			if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
			{
			  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.PROCESS_COMPLETED_WITH_ERROR_END_EVENT, execution));
			}
		  }
		  execution = getSuperExecution(execution);
		}
		if (execution == null)
		{
		  throw new BpmnError(errorCode, "No catching boundary event found for error with errorCode '" + errorCode + "', neither in same process nor in parent process");
		}
	  }


	  private static string findLocalErrorEventHandler(ActivityExecution execution, string errorCode)
	  {
		PvmScope scope = execution.Activity;
		while (scope != null)
		{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<org.activiti.engine.impl.bpmn.parser.ErrorEventDefinition> definitions = (java.util.List<org.activiti.engine.impl.bpmn.parser.ErrorEventDefinition>) scope.getProperty(org.activiti.engine.impl.bpmn.parser.BpmnParse.PROPERTYNAME_ERROR_EVENT_DEFINITIONS);
		  IList<ErrorEventDefinition> definitions = (IList<ErrorEventDefinition>) scope.getProperty(BpmnParse.PROPERTYNAME_ERROR_EVENT_DEFINITIONS);
		  if (definitions != null)
		  {
			// definitions are sorted by precedence, ie. event subprocesses first.      
			foreach (ErrorEventDefinition errorEventDefinition in definitions)
			{
			  if (errorEventDefinition.catches(errorCode))
			  {
				return scope.findActivity(errorEventDefinition.HandlerActivityId).Id;
			  }
			}
		  }

		  // search for error handlers in parent scopes 
		  if (scope is PvmActivity)
		  {
			scope = ((PvmActivity) scope).Parent;
		  }
		  else
		  {
			scope = null;
		  }
		}
		return null;
	  }


	  private static ActivityExecution getSuperExecution(ActivityExecution execution)
	  {
		ExecutionEntity executionEntity = (ExecutionEntity) execution;
		ExecutionEntity superExecution = executionEntity.getProcessInstance().getSuperExecution();
		if (superExecution != null && !superExecution.Scope)
		{
		  return superExecution.getParent();
		}
		return superExecution;
	  }

	  private static void executeCatch(string errorHandlerId, ActivityExecution execution, string errorCode)
	  {
		ProcessDefinitionImpl processDefinition = ((ExecutionEntity) execution).ProcessDefinition;
		ActivityImpl errorHandler = processDefinition.findActivity(errorHandlerId);
		if (errorHandler == null)
		{
		  throw new ActivitiException(errorHandlerId + " not found in process definition");
		}

		bool matchingParentFound = false;
		ActivityExecution leavingExecution = execution;
		ActivityImpl currentActivity = (ActivityImpl) execution.Activity;

		ScopeImpl catchingScope = errorHandler.Parent;
		if (catchingScope is ActivityImpl)
		{
		  ActivityImpl catchingScopeActivity = (ActivityImpl) catchingScope;
		  if (!catchingScopeActivity.Scope) // event subprocesses
		  {
			catchingScope = catchingScopeActivity.Parent;
		  }
		}

		if (catchingScope is PvmProcessDefinition)
		{
		  executeEventHandler(errorHandler, ((ExecutionEntity)execution).getProcessInstance(), errorCode);

		}
		else
		{
		  if (currentActivity.Id.Equals(catchingScope.Id))
		  {
			matchingParentFound = true;
		  }
		  else
		  {
			currentActivity = (ActivityImpl) currentActivity.Parent;

			// Traverse parents until one is found that is a scope 
			// and matches the activity the boundary event is defined on
			while (!matchingParentFound && leavingExecution != null && currentActivity != null)
			{
			  if (!leavingExecution.Concurrent && currentActivity.Id.Equals(catchingScope.Id))
			  {
				matchingParentFound = true;
			  }
			  else if (leavingExecution.Concurrent)
			  {
				leavingExecution = leavingExecution.Parent;
			  }
			  else
			  {
				currentActivity = currentActivity.ParentActivity;
				leavingExecution = leavingExecution.Parent;
			  }
			}

			// Follow parents up until matching scope can't be found anymore (needed to support for multi-instance)
			while (leavingExecution != null && leavingExecution.Parent != null && leavingExecution.Parent.Activity != null && leavingExecution.Parent.Activity.Id.Equals(catchingScope.Id))
			{
			  leavingExecution = leavingExecution.Parent;
			}
		  }

		  if (matchingParentFound && leavingExecution != null)
		  {
			executeEventHandler(errorHandler, leavingExecution, errorCode);
		  }
		  else
		  {
			throw new ActivitiException("No matching parent execution for activity " + errorHandlerId + " found");
		  }
		}

	  }

	  private static void executeEventHandler(ActivityImpl borderEventActivity, ActivityExecution leavingExecution, string errorCode)
	  {
		  if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		  {
			  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createErrorEvent(ActivitiEventType.ACTIVITY_ERROR_RECEIVED, borderEventActivity.Id, errorCode, leavingExecution.Id, leavingExecution.ProcessInstanceId, leavingExecution.ProcessDefinitionId));
		  }

		  // The current activity of the execution will be changed in the next lines.
		  // So we must make sure the activity is ended correctly here
		  // The other executions (for example when doing something parallel in a subprocess, will
		  // be destroyed by the destroy scope operation (but this execution will be used to do it and 
		  // will have list the original activity by then)
		  Context.CommandContext.HistoryManager.recordActivityEnd((ExecutionEntity) leavingExecution);

		if (borderEventActivity.ActivityBehavior is EventSubProcessStartEventActivityBehavior)
		{
		  InterpretableExecution execution = (InterpretableExecution) leavingExecution;
		  execution.Activity = borderEventActivity.ParentActivity;
		  execution.performOperation(org.activiti.engine.impl.pvm.runtime.AtomicOperation_Fields.ACTIVITY_START); // make sure the listeners are invoked!
		}
		else
		{
		  leavingExecution.executeActivity(borderEventActivity);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean mapException(Exception e, org.activiti.engine.impl.pvm.delegate.ActivityExecution execution, java.util.List<org.activiti.bpmn.model.MapExceptionEntry> exceptionMap) throws Exception
	  public static bool mapException(Exception e, ActivityExecution execution, IList<MapExceptionEntry> exceptionMap)
	  {
		return mapException(e, execution, exceptionMap, false);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean mapException(Exception e, org.activiti.engine.impl.pvm.delegate.ActivityExecution execution, java.util.List<org.activiti.bpmn.model.MapExceptionEntry> exceptionMap, boolean wrapped) throws Exception
	  public static bool mapException(Exception e, ActivityExecution execution, IList<MapExceptionEntry> exceptionMap, bool wrapped)
	  {
		if (exceptionMap == null)
		{
		  return false;
		}

		if (wrapped && e is PvmException)
		{
		  e = (Exception)((PvmException) e).InnerException;
		}

		string defaultMap = null;

		foreach (MapExceptionEntry me in exceptionMap)
		{
		  string exceptionClass = me.ClassName;
		  string errorCode = me.ErrorCode;

		  // save the first mapping with no exception class as default map
		  if (StringUtils.isNotEmpty(errorCode) && StringUtils.isEmpty(exceptionClass) && defaultMap == null)
		  {
			defaultMap = errorCode;
			continue;
		  }

		  // ignore if error code or class are not defined
		  if (StringUtils.isEmpty(errorCode) || StringUtils.isEmpty(exceptionClass))
		  {
			continue;
		  }

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  if (e.GetType().FullName.Equals(exceptionClass))
		  {
			propagateError(errorCode, execution);
			return true;
		  }
		  if (me.AndChildren)
		  {
			Type exceptionClassClass = ReflectUtil.loadClass(exceptionClass);
			if (exceptionClassClass.IsAssignableFrom(e.GetType()))
			{
			  propagateError(errorCode, execution);
			  return true;
			}
		  }
		}
		if (defaultMap != null)
		{
		  propagateError(defaultMap, execution);
		  return true;
		}

		return false;
	  }
	}

}