using System;
using System.Collections.Generic;

namespace org.activiti.engine.impl.db
{


	using AttachmentEntity = org.activiti.engine.impl.persistence.entity.AttachmentEntity;
	using ByteArrayEntity = org.activiti.engine.impl.persistence.entity.ByteArrayEntity;
	using CommentEntity = org.activiti.engine.impl.persistence.entity.CommentEntity;
	using CompensateEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.CompensateEventSubscriptionEntity;
	using DeploymentEntity = org.activiti.engine.impl.persistence.entity.DeploymentEntity;
	using EventLogEntryEntity = org.activiti.engine.impl.persistence.entity.EventLogEntryEntity;
	using EventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.EventSubscriptionEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using GroupEntity = org.activiti.engine.impl.persistence.entity.GroupEntity;
	using HistoricActivityInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricActivityInstanceEntity;
	using HistoricDetailAssignmentEntity = org.activiti.engine.impl.persistence.entity.HistoricDetailAssignmentEntity;
	using HistoricDetailEntity = org.activiti.engine.impl.persistence.entity.HistoricDetailEntity;
	using HistoricDetailTransitionInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricDetailTransitionInstanceEntity;
	using HistoricDetailVariableInstanceUpdateEntity = org.activiti.engine.impl.persistence.entity.HistoricDetailVariableInstanceUpdateEntity;
	using HistoricFormPropertyEntity = org.activiti.engine.impl.persistence.entity.HistoricFormPropertyEntity;
	using HistoricIdentityLinkEntity = org.activiti.engine.impl.persistence.entity.HistoricIdentityLinkEntity;
	using HistoricProcessInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricProcessInstanceEntity;
	using HistoricScopeInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricScopeInstanceEntity;
	using HistoricTaskInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricTaskInstanceEntity;
	using HistoricVariableInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using IdentityInfoEntity = org.activiti.engine.impl.persistence.entity.IdentityInfoEntity;
	using IdentityLinkEntity = org.activiti.engine.impl.persistence.entity.IdentityLinkEntity;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using MembershipEntity = org.activiti.engine.impl.persistence.entity.MembershipEntity;
	using MessageEntity = org.activiti.engine.impl.persistence.entity.MessageEntity;
	using MessageEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.MessageEventSubscriptionEntity;
	using ModelEntity = org.activiti.engine.impl.persistence.entity.ModelEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PropertyEntity = org.activiti.engine.impl.persistence.entity.PropertyEntity;
	using ResourceEntity = org.activiti.engine.impl.persistence.entity.ResourceEntity;
	using SignalEventSubscriptionEntity = org.activiti.engine.impl.persistence.entity.SignalEventSubscriptionEntity;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;
	using TimerEntity = org.activiti.engine.impl.persistence.entity.TimerEntity;
	using UserEntity = org.activiti.engine.impl.persistence.entity.UserEntity;
	using VariableInstanceEntity = org.activiti.engine.impl.persistence.entity.VariableInstanceEntity;


	/// <summary>
	/// Maintains a list of all the entities in order of dependency.
	/// </summary>
	public class EntityDependencyOrder
	{

		public static IList<Type> DELETE_ORDER = new List<Type>();
		public static IList<Type> INSERT_ORDER = new List<Type>();

		static EntityDependencyOrder()
		{

			/*
			 * In the comments below:
			 * 
			 * 'FK to X' : X should be BELOW the entity
			 * 
			 * 'FK from X': X should be ABOVE the entity
			 * 
			 */

			/* No FK */
			DELETE_ORDER.Add(typeof(PropertyEntity));

			/* No FK */
			DELETE_ORDER.Add(typeof(AttachmentEntity));

			/* No FK */
			DELETE_ORDER.Add(typeof(CommentEntity));

			/* No FK */
			DELETE_ORDER.Add(typeof(EventLogEntryEntity));

			/*
			 * FK to Deployment
			 * FK to ByteArray 
			 */
			DELETE_ORDER.Add(typeof(ModelEntity));

			/* Subclass of JobEntity */
			DELETE_ORDER.Add(typeof(MessageEntity));

			/* Subclass of TimerEntity */
			DELETE_ORDER.Add(typeof(TimerEntity));

			/*
			 * FK to ByteArray
			 */
			DELETE_ORDER.Add(typeof(JobEntity));

			/*
			 * FK to ByteArray
			 * FK to Exeution
			 */
			DELETE_ORDER.Add(typeof(VariableInstanceEntity));

			/*
			 * FK from ModelEntity
			 * FK from JobEntity
			 * FK from VariableInstanceEntity
			 * 
			 * FK to DeploymentEntity
			 */
			DELETE_ORDER.Add(typeof(ByteArrayEntity));

			/*
			 * FK from ModelEntity
			 * FK from JobEntity
			 * FK from VariableInstanceEntity
			 * 
			 * FK to DeploymentEntity
			 */
			DELETE_ORDER.Add(typeof(ResourceEntity));

			/*
			 * FK from ByteArray
			 */
			DELETE_ORDER.Add(typeof(DeploymentEntity));

			/*
			 * FK to Execution
			 */
			DELETE_ORDER.Add(typeof(EventSubscriptionEntity));

			/*
			 * FK to Execution
			 */
			DELETE_ORDER.Add(typeof(CompensateEventSubscriptionEntity));

			/*
			 * FK to Execution
			 */
			DELETE_ORDER.Add(typeof(MessageEventSubscriptionEntity));

			/*
			 * FK to Execution
			 */
			DELETE_ORDER.Add(typeof(SignalEventSubscriptionEntity));


			/*
			 * FK to process definition
			 * FK to Execution
			 * FK to Task
			 */
			DELETE_ORDER.Add(typeof(IdentityLinkEntity));

			/*
			 * FK from IdentityLink
			 * 
			 * FK to Execution
			 * FK to process definition
			 */
			DELETE_ORDER.Add(typeof(TaskEntity));

			/*
			 * FK from VariableInstance 
			 * FK from EventSubscription
			 * FK from IdentityLink
			 * FK from Task
			 * 
			 * FK to ProcessDefinition
			 */
			DELETE_ORDER.Add(typeof(ExecutionEntity));

			/*
			 * FK from Task
			 * FK from IdentityLink
			 * FK from execution
			 */
			DELETE_ORDER.Add(typeof(ProcessDefinitionEntity));

			/*
		   * FK to User
		   * FK to Group
		   */
			DELETE_ORDER.Add(typeof(MembershipEntity));

			/*
			 * Fk from Membership
			 */
			DELETE_ORDER.Add(typeof(UserEntity));

		  /*
		   * FK from Membership
		   */
			DELETE_ORDER.Add(typeof(GroupEntity));


		  // History entities have no FK's

			DELETE_ORDER.Add(typeof(HistoricIdentityLinkEntity));

			DELETE_ORDER.Add(typeof(IdentityInfoEntity));

			DELETE_ORDER.Add(typeof(HistoricActivityInstanceEntity));
			DELETE_ORDER.Add(typeof(HistoricProcessInstanceEntity));
			DELETE_ORDER.Add(typeof(HistoricTaskInstanceEntity));
			DELETE_ORDER.Add(typeof(HistoricScopeInstanceEntity));

			DELETE_ORDER.Add(typeof(HistoricVariableInstanceEntity));

			DELETE_ORDER.Add(typeof(HistoricDetailAssignmentEntity));
			DELETE_ORDER.Add(typeof(HistoricDetailTransitionInstanceEntity));
			DELETE_ORDER.Add(typeof(HistoricDetailVariableInstanceUpdateEntity));
			DELETE_ORDER.Add(typeof(HistoricFormPropertyEntity));
			DELETE_ORDER.Add(typeof(HistoricDetailEntity));

			INSERT_ORDER = new List<Type>(DELETE_ORDER);
			INSERT_ORDER.Reverse();

		}

	}

}