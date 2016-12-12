using System;

namespace org.activiti.engine.impl.interceptor
{
	public interface SessionFactory
	{
	  Type SessionType {get;}

	  Session openSession();
	}
}