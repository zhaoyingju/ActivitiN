using System;

namespace org.activiti.engine.impl.cmd
{

	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;

	[Serializable]
	public class GetTableNameCmd : Command<string>
	{

	  private const long serialVersionUID = 1L;

	  private Type entityClass;

	  public GetTableNameCmd(Type entityClass)
	  {
		this.entityClass = entityClass;
	  }

	  public virtual string execute(CommandContext commandContext)
	  {
		if (entityClass == null)
		{
		  throw new ActivitiIllegalArgumentException("entityClass is null");
		}
		return commandContext.TableDataManager.getTableName(entityClass, true);
	  }

	}

}