using System;

namespace org.activiti.engine.impl.cmd
{

	/// <summary>
	/// @author jbarrez
	/// </summary>
	public abstract class AbstractCustomSqlExecution<Mapper, ResultType> : CustomSqlExecution<Mapper, ResultType>
	{

		protected internal Type mapperClass;

		public AbstractCustomSqlExecution(Type mapperClass)
		{
			this.mapperClass = mapperClass;
		}

		public override Type MapperClass
		{
			get
			{
			  return mapperClass;
			}
		}

	}

}