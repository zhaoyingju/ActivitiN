using System;

namespace org.activiti.engine.impl.history.handler
{

	using DelegateTask = org.activiti.engine.@delegate.DelegateTask;
	using TaskListener = org.activiti.engine.@delegate.TaskListener;
	using Context = org.activiti.engine.impl.context.Context;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;


	/// <summary>
	/// Called when a task is created for a user-task activity. Allows recoring task-id in
	/// historic activity.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	[Serializable]
	public class UserTaskIdHandler : TaskListener
	{

	  public virtual void notify(DelegateTask task)
	  {
		Context.CommandContext.HistoryManager.recordTaskId((TaskEntity) task);
	  }

	}

}