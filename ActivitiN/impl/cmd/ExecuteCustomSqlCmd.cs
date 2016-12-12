using System;

namespace org.activiti.engine.impl.cmd
{

	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author jbarrez
	/// </summary>
	public class ExecuteCustomSqlCmd<Mapper, ResultType> : Command<ResultType>
	{

		protected internal Type mapperClass;
		protected internal CustomSqlExecution<Mapper, ResultType> customSqlExecution;

		public ExecuteCustomSqlCmd(Type mapperClass, CustomSqlExecution<Mapper, ResultType> customSqlExecution)
		{
			this.mapperClass = mapperClass;
			this.customSqlExecution = customSqlExecution;
		}

		public override ResultType execute(CommandContext commandContext)
		{
			Mapper mapper = commandContext.DbSqlSession.SqlSession.getMapper(mapperClass);
			return customSqlExecution.execute(mapper);
		}

	}

}