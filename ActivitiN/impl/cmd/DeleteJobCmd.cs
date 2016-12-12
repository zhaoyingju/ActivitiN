using System;

namespace org.activiti.engine.impl.cmd
{

	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using Job = org.activiti.engine.runtime.Job;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// @author Saeid Mirzaei
	/// @author Joram Barrez
	/// </summary>

	[Serializable]
	public class DeleteJobCmd : Command<object>
	{

	  private static readonly Logger log = LoggerFactory.getLogger(typeof(DeleteJobCmd));
	  private const long serialVersionUID = 1L;

	  protected internal string jobId;

	  public DeleteJobCmd(string jobId)
	  {
		this.jobId = jobId;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		JobEntity jobToDelete = getJobToDelete(commandContext);

		jobToDelete.delete();
		return null;
	  }

	  protected internal virtual JobEntity getJobToDelete(CommandContext commandContext)
	  {
		if (jobId == null)
		{
		  throw new ActivitiIllegalArgumentException("jobId is null");
		}
		if (log.DebugEnabled)
		{
		  log.debug("Deleting job {}", jobId);
		}

		JobEntity job = commandContext.JobEntityManager.findJobById(jobId);
		if (job == null)
		{
		  throw new ActivitiObjectNotFoundException("No job found with id '" + jobId + "'", typeof(Job));
		}

		// We need to check if the job was locked, ie acquired by the job acquisition thread
		// This happens if the the job was already acquired, but not yet executed.
		// In that case, we can't allow to delete the job.
		if (job.LockOwner != null)
		{
		  throw new ActivitiException("Cannot delete job when the job is being executed. Try again later.");
		}
		return job;
	  }

	}

}