namespace org.activiti.engine.impl.cmd
{

	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;

	/// <summary>
	/// Command that dispatches a JOB_CANCELLED event and deletes the job entity.
	/// </summary>
	public class CancelJobCmd : DeleteJobCmd
	{

	  private const long serialVersionUID = 1L;

	  public CancelJobCmd(string jobId) : base(jobId)
	  {
	  }

	  public override object execute(CommandContext commandContext)
	  {
		JobEntity jobToDelete = getJobToDelete(commandContext);

		sendCancelEvent(jobToDelete);

		jobToDelete.delete();
		return null;
	  }

	  private void sendCancelEvent(JobEntity jobToDelete)
	  {
		if (Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
		  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.JOB_CANCELED, jobToDelete));
		}
	  }

	}

}