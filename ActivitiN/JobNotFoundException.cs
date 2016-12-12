namespace org.activiti.engine
{

	using Job = org.activiti.engine.runtime.Job;

	/// <summary>
	/// This exception is thrown when you try to execute a job that is not found (may
	/// be due to cancelActiviti="true" for instance)..
	/// 
	/// @author Prabhat Tripathi
	/// </summary>
	public class JobNotFoundException : ActivitiObjectNotFoundException
	{

	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// the id of the job </summary>
	  private string jobId;

	  public JobNotFoundException(string jobId) : base("No job found with id '" + jobId + "'.", typeof(Job))
	  {
		this.jobId = jobId;
	  }

	  public virtual string JobId
	  {
		  get
		  {
			return this.jobId;
		  }
	  }

	}

}