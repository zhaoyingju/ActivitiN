using System;

namespace org.activiti.engine.impl.cmd
{

	/// <summary>
	/// @author jbarrez
	/// </summary>
	public interface CustomSqlExecution<Mapper, ResultType>
	{

		Type MapperClass {get;}

		ResultType execute(Mapper mapper);

	}
}