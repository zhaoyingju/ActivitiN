using System.Collections.Generic;

namespace org.activiti.engine.impl.cmd
{

	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// @author roman.smirnov
	/// @author Joram Barrez
	/// </summary>
	public class RemoveExecutionVariablesCmd : NeedsActiveExecutionCmd<Void>
	{

	  private const long serialVersionUID = 1L;

	  private ICollection<string> variableNames;
	  private bool isLocal;

	  public RemoveExecutionVariablesCmd(string executionId, ICollection<string> variableNames, bool isLocal) : base(executionId)
	  {
		this.variableNames = variableNames;
		this.isLocal = isLocal;
	  }

	  protected internal virtual Void execute(CommandContext commandContext, ExecutionEntity execution)
	  {

		if (isLocal)
		{
		  execution.removeVariablesLocal(variableNames);
		}
		else
		{
		  execution.removeVariables(variableNames);
		}

		return null;
	  }

	  protected internal override string SuspendedExceptionMessage
	  {
		  get
		  {
			return "Cannot remove variables because execution '" + executionId + "' is suspended";
		  }
	  }

	}

}