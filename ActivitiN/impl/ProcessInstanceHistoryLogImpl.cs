using System;
using System.Collections.Generic;

namespace org.activiti.engine.impl
{


	using HistoricData = org.activiti.engine.history.HistoricData;
	using HistoricProcessInstance = org.activiti.engine.history.HistoricProcessInstance;
	using ProcessInstanceHistoryLog = org.activiti.engine.history.ProcessInstanceHistoryLog;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ProcessInstanceHistoryLogImpl : ProcessInstanceHistoryLog
	{

		protected internal HistoricProcessInstance historicProcessInstance;

		protected internal IList<HistoricData> historicData = new List<HistoricData>();

		public ProcessInstanceHistoryLogImpl(HistoricProcessInstance historicProcessInstance)
		{
			this.historicProcessInstance = historicProcessInstance;
		}

		public override string Id
		{
			get
			{
				return historicProcessInstance.Id;
			}
		}

		public override string BusinessKey
		{
			get
			{
				return historicProcessInstance.BusinessKey;
			}
		}

		public override string ProcessDefinitionId
		{
			get
			{
				return historicProcessInstance.ProcessDefinitionId;
			}
		}

		public override DateTime StartTime
		{
			get
			{
				return historicProcessInstance.StartTime;
			}
		}

		public override DateTime EndTime
		{
			get
			{
				return historicProcessInstance.EndTime;
			}
		}

		public override long? DurationInMillis
		{
			get
			{
				return historicProcessInstance.DurationInMillis;
			}
		}

		public override string StartUserId
		{
			get
			{
				return historicProcessInstance.StartUserId;
			}
		}

		public override string StartActivityId
		{
			get
			{
				return historicProcessInstance.StartActivityId;
			}
		}

		public override string DeleteReason
		{
			get
			{
				return historicProcessInstance.DeleteReason;
			}
		}

		public override string SuperProcessInstanceId
		{
			get
			{
				return historicProcessInstance.SuperProcessInstanceId;
			}
		}

		public override string TenantId
		{
			get
			{
				return historicProcessInstance.TenantId;
			}
		}

		public override IList<HistoricData> HistoricData
		{
			get
			{
				return historicData;
			}
		}

		public virtual void addHistoricData(HistoricData historicEvent)
		{
			historicData.Add(historicEvent);
		}

		public virtual void addHistoricData<T1>(ICollection<T1> historicEvents) where T1 : org.activiti.engine.history.HistoricData
		{
			historicData.AddRange(historicEvents);
		}

		public virtual void orderHistoricData()
		{
			historicData.Sort(new ComparatorAnonymousInnerClassHelper(this));
		}

		private class ComparatorAnonymousInnerClassHelper : IComparer<HistoricData>
		{
			private readonly ProcessInstanceHistoryLogImpl outerInstance;

			public ComparatorAnonymousInnerClassHelper(ProcessInstanceHistoryLogImpl outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual int Compare(HistoricData data1, HistoricData data2)
			{
				return data1.Time.compareTo(data2.Time);
			}
		}

	}

}