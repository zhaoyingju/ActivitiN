using System.Collections.Generic;

namespace org.activiti.engine.impl.cmd
{

	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;

	/// <summary>
	/// @author roman.smirnov
	/// @author Joram Barrez
	/// </summary>
	public class RemoveTaskVariablesCmd : NeedsActiveTaskCmd<Void>
	{

	  private const long serialVersionUID = 1L;

	  private readonly ICollection<string> variableNames;
	  private readonly bool isLocal;

	  public RemoveTaskVariablesCmd(string taskId, ICollection<string> variableNames, bool isLocal) : base(taskId)
	  {
		this.variableNames = variableNames;
		this.isLocal = isLocal;
	  }

	  protected internal virtual Void execute(CommandContext commandContext, TaskEntity task)
	  {

		if (isLocal)
		{
		  task.removeVariablesLocal(variableNames);
		}
		else
		{
		  task.removeVariables(variableNames);
		}

		return null;
	  }

	  protected internal override string SuspendedTaskException
	  {
		  get
		  {
			return "Cannot remove variables from a suspended task.";
		  }
	  }

	}

}