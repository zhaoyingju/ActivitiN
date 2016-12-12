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


	using ActivitiListener = org.activiti.bpmn.model.ActivitiListener;
	using EventListener = org.activiti.bpmn.model.EventListener;
	using ImplementationType = org.activiti.bpmn.model.ImplementationType;
	using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;
	using TaskListener = org.activiti.engine.@delegate.TaskListener;
	using ActivitiEventListener = org.activiti.engine.@delegate.@event.ActivitiEventListener;
	using BaseDelegateEventListener = org.activiti.engine.impl.bpmn.helper.BaseDelegateEventListener;
	using ClassDelegate = org.activiti.engine.impl.bpmn.helper.ClassDelegate;
	using DelegateActivitiEventListener = org.activiti.engine.impl.bpmn.helper.DelegateActivitiEventListener;
	using DelegateExpressionActivitiEventListener = org.activiti.engine.impl.bpmn.helper.DelegateExpressionActivitiEventListener;
	using ErrorThrowingEventListener = org.activiti.engine.impl.bpmn.helper.ErrorThrowingEventListener;
	using MessageThrowingEventListener = org.activiti.engine.impl.bpmn.helper.MessageThrowingEventListener;
	using SignalThrowingEventListener = org.activiti.engine.impl.bpmn.helper.SignalThrowingEventListener;
	using DelegateExpressionExecutionListener = org.activiti.engine.impl.bpmn.listener.DelegateExpressionExecutionListener;
	using DelegateExpressionTaskListener = org.activiti.engine.impl.bpmn.listener.DelegateExpressionTaskListener;
	using ExpressionExecutionListener = org.activiti.engine.impl.bpmn.listener.ExpressionExecutionListener;
	using ExpressionTaskListener = org.activiti.engine.impl.bpmn.listener.ExpressionTaskListener;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using Execution = org.activiti.engine.runtime.Execution;
	using Job = org.activiti.engine.runtime.Job;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;
	using Attachment = org.activiti.engine.task.Attachment;
	using Comment = org.activiti.engine.task.Comment;
	using IdentityLink = org.activiti.engine.task.IdentityLink;
	using Task = org.activiti.engine.task.Task;

	/// <summary>
	/// Default implementation of the <seealso cref="ListenerFactory"/>. 
	/// Used when no custom <seealso cref="ListenerFactory"/> is injected on 
	/// the <seealso cref="ProcessEngineConfigurationImpl"/>.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class DefaultListenerFactory : AbstractBehaviorFactory, ListenerFactory
	{

		public static readonly IDictionary<string, Type> ENTITY_MAPPING = new Dictionary<string, Type>();
		static DefaultListenerFactory()
		{
			ENTITY_MAPPING["attachment"] = typeof(Attachment);
			ENTITY_MAPPING["comment"] = typeof(Comment);
			ENTITY_MAPPING["execution"] = typeof(Execution);
			ENTITY_MAPPING["identity-link"] = typeof(IdentityLink);
			ENTITY_MAPPING["job"] = typeof(Job);
			ENTITY_MAPPING["process-definition"] = typeof(ProcessDefinition);
			ENTITY_MAPPING["process-instance"] = typeof(ProcessInstance);
			ENTITY_MAPPING["task"] = typeof(Task);
		}

	  public virtual TaskListener createClassDelegateTaskListener(ActivitiListener activitiListener)
	  {
		return new ClassDelegate(activitiListener.Implementation, createFieldDeclarations(activitiListener.FieldExtensions));
	  }

	  public virtual TaskListener createExpressionTaskListener(ActivitiListener activitiListener)
	  {
		return new ExpressionTaskListener(expressionManager.createExpression(activitiListener.Implementation));
	  }

	  public virtual TaskListener createDelegateExpressionTaskListener(ActivitiListener activitiListener)
	  {
		return new DelegateExpressionTaskListener(expressionManager.createExpression(activitiListener.Implementation), createFieldDeclarations(activitiListener.FieldExtensions));
	  }

	  public virtual ExecutionListener createClassDelegateExecutionListener(ActivitiListener activitiListener)
	  {
		return new ClassDelegate(activitiListener.Implementation, createFieldDeclarations(activitiListener.FieldExtensions));
	  }

	  public virtual ExecutionListener createExpressionExecutionListener(ActivitiListener activitiListener)
	  {
		return new ExpressionExecutionListener(expressionManager.createExpression(activitiListener.Implementation));
	  }

	  public virtual ExecutionListener createDelegateExpressionExecutionListener(ActivitiListener activitiListener)
	  {
		return new DelegateExpressionExecutionListener(expressionManager.createExpression(activitiListener.Implementation), createFieldDeclarations(activitiListener.FieldExtensions));
	  }

		public override ActivitiEventListener createClassDelegateEventListener(EventListener eventListener)
		{
			return new DelegateActivitiEventListener(eventListener.Implementation, getEntityType(eventListener.EntityType));
		}

		public override ActivitiEventListener createDelegateExpressionEventListener(EventListener eventListener)
		{
			return new DelegateExpressionActivitiEventListener(expressionManager.createExpression(eventListener.Implementation), getEntityType(eventListener.EntityType));
		}

		public override ActivitiEventListener createEventThrowingEventListener(EventListener eventListener)
		{
			BaseDelegateEventListener result = null;
			if (ImplementationType.IMPLEMENTATION_TYPE_THROW_SIGNAL_EVENT.Equals(eventListener.ImplementationType))
			{
				result = new SignalThrowingEventListener();
				((SignalThrowingEventListener) result).SignalName = eventListener.Implementation;
				((SignalThrowingEventListener) result).ProcessInstanceScope = true;
			}
			else if (ImplementationType.IMPLEMENTATION_TYPE_THROW_GLOBAL_SIGNAL_EVENT.Equals(eventListener.ImplementationType))
			{
				result = new SignalThrowingEventListener();
				((SignalThrowingEventListener) result).SignalName = eventListener.Implementation;
				((SignalThrowingEventListener) result).ProcessInstanceScope = false;
			}
			else if (ImplementationType.IMPLEMENTATION_TYPE_THROW_MESSAGE_EVENT.Equals(eventListener.ImplementationType))
			{
				result = new MessageThrowingEventListener();
				((MessageThrowingEventListener) result).MessageName = eventListener.Implementation;
			}
			else if (ImplementationType.IMPLEMENTATION_TYPE_THROW_ERROR_EVENT.Equals(eventListener.ImplementationType))
			{
				result = new ErrorThrowingEventListener();
				((ErrorThrowingEventListener) result).ErrorCode = eventListener.Implementation;
			}

			if (result == null)
			{
				throw new ActivitiIllegalArgumentException("Cannot create an event-throwing event-listener, unknown implementation type: " + eventListener.ImplementationType);
			}

			result.EntityClass = getEntityType(eventListener.EntityType);
			return result;
		}

		/// <param name="entityType"> the name of the entity
		/// @return </param>
		/// <exception cref="ActivitiIllegalArgumentException"> when the given entity name </exception>
		protected internal virtual Type getEntityType(string entityType)
		{
			if (entityType != null)
			{
				Type entityClass = ENTITY_MAPPING[entityType.Trim()];
				if (entityClass == null)
				{
					throw new ActivitiIllegalArgumentException("Unsupported entity-type for an ActivitiEventListener: " + entityType);
				}
				return entityClass;
			}
			return null;
		}
	}

}