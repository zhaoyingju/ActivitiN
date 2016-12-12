using System;

namespace org.activiti.engine.impl.persistence
{

	using DefaultHistoryManager = org.activiti.engine.impl.history.DefaultHistoryManager;
	using HistoryManager = org.activiti.engine.impl.history.HistoryManager;
	using Session = org.activiti.engine.impl.interceptor.Session;
	using SessionFactory = org.activiti.engine.impl.interceptor.SessionFactory;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class DefaultHistoryManagerSessionFactory : SessionFactory
	{

		public virtual Type SessionType
		{
			get
			{
				return typeof(HistoryManager);
			}
		}

		public override Session openSession()
		{
			return new DefaultHistoryManager();
		}

	}

}