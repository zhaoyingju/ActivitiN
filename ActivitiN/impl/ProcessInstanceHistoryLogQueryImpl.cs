using System.Collections.Generic;

namespace org.activiti.engine.impl
{

	using HistoricActivityInstance = org.activiti.engine.history.HistoricActivityInstance;
	using HistoricData = org.activiti.engine.history.HistoricData;
	using HistoricVariableInstance = org.activiti.engine.history.HistoricVariableInstance;
	using HistoricVariableUpdate = org.activiti.engine.history.HistoricVariableUpdate;
	using ProcessInstanceHistoryLog = org.activiti.engine.history.ProcessInstanceHistoryLog;
	using ProcessInstanceHistoryLogQuery = org.activiti.engine.history.ProcessInstanceHistoryLogQuery;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using HistoricProcessInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricProcessInstanceEntity;
	using HistoricVariableInstanceEntity = org.activiti.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using CacheableVariable = org.activiti.engine.impl.variable.CacheableVariable;
	using JPAEntityListVariableType = org.activiti.engine.impl.variable.JPAEntityListVariableType;
	using JPAEntityVariableType = org.activiti.engine.impl.variable.JPAEntityVariableType;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ProcessInstanceHistoryLogQueryImpl : ProcessInstanceHistoryLogQuery, Command<ProcessInstanceHistoryLog>
	{

		protected internal CommandExecutor commandExecutor;

		protected internal string processInstanceId;
		protected internal bool includeTasks_Renamed;
		protected internal bool includeActivities_Renamed;
		protected internal bool includeVariables_Renamed;
		protected internal bool includeComments_Renamed;
		protected internal bool includeVariableUpdates_Renamed;
		protected internal bool includeFormProperties_Renamed;

		public ProcessInstanceHistoryLogQueryImpl(CommandExecutor commandExecutor, string processInstanceId)
		{
		this.commandExecutor = commandExecutor;
		this.processInstanceId = processInstanceId;
		}

		public override ProcessInstanceHistoryLogQuery includeTasks()
		{
		  this.includeTasks_Renamed = true;
		  return this;
		}

		public override ProcessInstanceHistoryLogQuery includeComments()
		{
			this.includeComments_Renamed = true;
		  return this;
		}

		public override ProcessInstanceHistoryLogQuery includeActivities()
		{
			this.includeActivities_Renamed = true;
			return this;
		}

		public override ProcessInstanceHistoryLogQuery includeVariables()
		{
			this.includeVariables_Renamed = true;
			return this;
		}

		public override ProcessInstanceHistoryLogQuery includeVariableUpdates()
		{
			this.includeVariableUpdates_Renamed = true;
			return this;
		}

		public override ProcessInstanceHistoryLogQuery includeFormProperties()
		{
		  this.includeFormProperties_Renamed = true;
		  return this;
		}

		public override ProcessInstanceHistoryLog singleResult()
		{
			return commandExecutor.execute(this);
		}

		public override ProcessInstanceHistoryLog execute(CommandContext commandContext)
		{

			// Fetch historic process instance
			HistoricProcessInstanceEntity historicProcessInstance = commandContext.HistoricProcessInstanceEntityManager.findHistoricProcessInstance(processInstanceId);

			if (historicProcessInstance == null)
			{
				return null;
			}

			// Create a log using this historic process instance
			ProcessInstanceHistoryLogImpl processInstanceHistoryLog = new ProcessInstanceHistoryLogImpl(historicProcessInstance);

			// Add events, based on query settings

			// Tasks
			if (includeTasks_Renamed)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<? extends org.activiti.engine.history.HistoricData> tasks = commandContext.getHistoricTaskInstanceEntityManager().findHistoricTaskInstancesByQueryCriteria(new HistoricTaskInstanceQueryImpl(commandExecutor).processInstanceId(processInstanceId));
				IList<?> tasks = commandContext.HistoricTaskInstanceEntityManager.findHistoricTaskInstancesByQueryCriteria((new HistoricTaskInstanceQueryImpl(commandExecutor)).processInstanceId(processInstanceId));
				processInstanceHistoryLog.addHistoricData(tasks);
			}

			// Activities
			if (includeActivities_Renamed)
			{
				IList<HistoricActivityInstance> activities = commandContext.HistoricActivityInstanceEntityManager.findHistoricActivityInstancesByQueryCriteria((new HistoricActivityInstanceQueryImpl(commandExecutor)).processInstanceId(processInstanceId), null);
				processInstanceHistoryLog.addHistoricData(activities);
			}

			// Variables
			if (includeVariables_Renamed)
			{
				IList<HistoricVariableInstance> variables = commandContext.HistoricVariableInstanceEntityManager.findHistoricVariableInstancesByQueryCriteria((new HistoricVariableInstanceQueryImpl(commandExecutor)).processInstanceId(processInstanceId), null);

				// Make sure all variables values are fetched (similar to the HistoricVariableInstance query)
				foreach (HistoricVariableInstance historicVariableInstance in variables)
				{
					historicVariableInstance.Value;

					// make sure JPA entities are cached for later retrieval
					HistoricVariableInstanceEntity variableEntity = (HistoricVariableInstanceEntity) historicVariableInstance;
					if (JPAEntityVariableType.TYPE_NAME.Equals(variableEntity.VariableType.TypeName) || JPAEntityListVariableType.TYPE_NAME.Equals(variableEntity.VariableType.TypeName))
					{
						((CacheableVariable) variableEntity.VariableType).ForceCacheable = true;
					}
				}

				processInstanceHistoryLog.addHistoricData(variables);
			}

			// Comment
			if (includeComments_Renamed)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<? extends org.activiti.engine.history.HistoricData> comments = commandContext.getCommentEntityManager().findCommentsByProcessInstanceId(processInstanceId);
				IList<?> comments = commandContext.CommentEntityManager.findCommentsByProcessInstanceId(processInstanceId);
				processInstanceHistoryLog.addHistoricData(comments);
			}

			// Details: variables
			if (includeVariableUpdates_Renamed)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<? extends org.activiti.engine.history.HistoricData> variableUpdates = commandContext.getHistoricDetailEntityManager().findHistoricDetailsByQueryCriteria(new HistoricDetailQueryImpl(commandExecutor).variableUpdates(), null);
				IList<?> variableUpdates = commandContext.HistoricDetailEntityManager.findHistoricDetailsByQueryCriteria((new HistoricDetailQueryImpl(commandExecutor)).variableUpdates(), null);

				// Make sure all variables values are fetched (similar to the HistoricVariableInstance query)
				foreach (HistoricData historicData in variableUpdates)
				{
				  HistoricVariableUpdate variableUpdate = (HistoricVariableUpdate) historicData;
				  variableUpdate.Value;
				}

				processInstanceHistoryLog.addHistoricData(variableUpdates);
			}

			// Details: form properties
			if (includeFormProperties_Renamed)
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<? extends org.activiti.engine.history.HistoricData> formProperties = commandContext.getHistoricDetailEntityManager().findHistoricDetailsByQueryCriteria(new HistoricDetailQueryImpl(commandExecutor).formProperties(), null);
				IList<?> formProperties = commandContext.HistoricDetailEntityManager.findHistoricDetailsByQueryCriteria((new HistoricDetailQueryImpl(commandExecutor)).formProperties(), null);
				processInstanceHistoryLog.addHistoricData(formProperties);
			}

			// All events collected. Sort them by date.
			processInstanceHistoryLog.orderHistoricData();

			return processInstanceHistoryLog;
		}

	}

}